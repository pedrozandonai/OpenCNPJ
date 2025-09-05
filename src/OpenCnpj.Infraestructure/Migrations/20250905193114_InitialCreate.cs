using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OpenCnpj.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:public.age_ranges", "not_applicable,from0to12,from13to20,from21to30,from31to40,from41to50,from51to60,from61to70,from71to80,over80")
                .Annotation("Npgsql:Enum:public.application_steps", "application_step,downloading_files,extracting_files,processing_raw_files,formatting_raw_data")
                .Annotation("Npgsql:Enum:public.company_situations", "null,active,suspense,inapt,closed")
                .Annotation("Npgsql:Enum:public.company_sizes", "not_informed,micro_company,small_company,other")
                .Annotation("Npgsql:Enum:public.company_types", "head_office,branch")
                .Annotation("Npgsql:Enum:public.file_statuses", "downloading,created,processing,processed,deleted")
                .Annotation("Npgsql:Enum:public.partner_types", "legal_entity,individual,foreign");

            migrationBuilder.CreateTable(
                name: "address_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_address_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "batch_files",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    batch_id = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    file_path = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    file_status_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batch_files", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "batches",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    identifier = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    application_last_step_id = table.Column<int>(type: "integer", nullable: false),
                    Directory = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "economic_activities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_economic_activities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "individual_microentrepreneurs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date_opted = table.Column<DateOnly>(type: "date", nullable: true),
                    exclusion_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_individual_microentrepreneurs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "legal_natures",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_legal_natures", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "LegalRepresentative",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartnerQualificationId = table.Column<int>(type: "integer", nullable: false),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalRepresentative", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "partner_qualifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partner_qualifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "phones",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    area_code = table.Column<int>(type: "integer", nullable: false),
                    phone = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_phones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reasons",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reasons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "special_situations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_special_situations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address_type_id = table.Column<int>(type: "integer", nullable: false),
                    city_id = table.Column<long>(type: "bigint", nullable: false),
                    street = table.Column<string>(type: "text", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: true),
                    complement = table.Column<string>(type: "text", nullable: true),
                    neighborhood = table.Column<string>(type: "text", nullable: false),
                    zip_code = table.Column<int>(type: "integer", nullable: true),
                    federal_unit = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_addresses", x => x.id);
                    table.ForeignKey(
                        name: "FK_addresses_address_types",
                        column: x => x.address_type_id,
                        principalTable: "address_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_addresses_cities",
                        column: x => x.city_id,
                        principalTable: "cities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "company_special_situations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    special_situation_id = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_special_situations", x => x.id);
                    table.ForeignKey(
                        name: "FK_company_special_situations_special_situations",
                        column: x => x.special_situation_id,
                        principalTable: "special_situations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "company",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    legal_nature_id = table.Column<long>(type: "bigint", nullable: false),
                    main_partner_qualification_id = table.Column<long>(type: "bigint", nullable: true),
                    company_size_id = table.Column<int>(type: "integer", nullable: false),
                    company_type_id = table.Column<int>(type: "integer", nullable: false),
                    country_id = table.Column<long>(type: "bigint", nullable: true),
                    address_id = table.Column<long>(type: "bigint", nullable: false),
                    company_special_situation_id = table.Column<long>(type: "bigint", nullable: true),
                    main_economic_activity_id = table.Column<long>(type: "bigint", nullable: false),
                    identifier = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    share_capital = table.Column<decimal>(type: "decimal", nullable: false),
                    responsabile_federative_entity = table.Column<string>(type: "text", nullable: true),
                    fantasy_name = table.Column<string>(type: "text", nullable: true),
                    register_date = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    foreign_city_name = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamptz", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company", x => x.id);
                    table.ForeignKey(
                        name: "FK_companies_addresses",
                        column: x => x.address_id,
                        principalTable: "addresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_companies_companies_special_situations",
                        column: x => x.company_special_situation_id,
                        principalTable: "company_special_situations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_companies_countries",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_companies_economic_activities",
                        column: x => x.main_economic_activity_id,
                        principalTable: "economic_activities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_companies_legal_natures",
                        column: x => x.legal_nature_id,
                        principalTable: "legal_natures",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_companies_partner_qualifications",
                        column: x => x.main_partner_qualification_id,
                        principalTable: "partner_qualifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "partners",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    company_id = table.Column<long>(type: "bigint", nullable: false),
                    partner_type_id = table.Column<int>(type: "integer", nullable: false),
                    legal_representative_id = table.Column<long>(type: "bigint", nullable: true),
                    partner_qualification_id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    identifier = table.Column<string>(type: "text", nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    country_id = table.Column<long>(type: "bigint", nullable: true),
                    age_range_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_partners", x => x.id);
                    table.ForeignKey(
                        name: "FK_partners_company",
                        column: x => x.company_id,
                        principalTable: "company",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_partners_country",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_partners_legal_representative",
                        column: x => x.legal_representative_id,
                        principalTable: "LegalRepresentative",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_partners_partner_qualification",
                        column: x => x.partner_qualification_id,
                        principalTable: "partner_qualifications",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_addresses_address_type_id",
                table: "addresses",
                column: "address_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_addresses_city_id",
                table: "addresses",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_batches_identifier",
                table: "batches",
                column: "identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cities_code",
                table: "cities",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_company_address_id",
                table: "company",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_company_company_special_situation_id",
                table: "company",
                column: "company_special_situation_id");

            migrationBuilder.CreateIndex(
                name: "IX_company_country_id",
                table: "company",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_company_legal_nature_id",
                table: "company",
                column: "legal_nature_id");

            migrationBuilder.CreateIndex(
                name: "IX_company_main_economic_activity_id",
                table: "company",
                column: "main_economic_activity_id");

            migrationBuilder.CreateIndex(
                name: "IX_company_main_partner_qualification_id",
                table: "company",
                column: "main_partner_qualification_id");

            migrationBuilder.CreateIndex(
                name: "IX_company_special_situations_special_situation_id",
                table: "company_special_situations",
                column: "special_situation_id");

            migrationBuilder.CreateIndex(
                name: "IX_countries_code",
                table: "countries",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_economic_activities_code",
                table: "economic_activities",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_legal_natures_code",
                table: "legal_natures",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_partner_qualifications_code",
                table: "partner_qualifications",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_partners_company_id",
                table: "partners",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_partners_country_id",
                table: "partners",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_partners_legal_representative_id",
                table: "partners",
                column: "legal_representative_id");

            migrationBuilder.CreateIndex(
                name: "IX_partners_partner_qualification_id",
                table: "partners",
                column: "partner_qualification_id");

            migrationBuilder.CreateIndex(
                name: "IX_reasons_code",
                table: "reasons",
                column: "code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "batch_files");

            migrationBuilder.DropTable(
                name: "batches");

            migrationBuilder.DropTable(
                name: "individual_microentrepreneurs");

            migrationBuilder.DropTable(
                name: "partners");

            migrationBuilder.DropTable(
                name: "phones");

            migrationBuilder.DropTable(
                name: "reasons");

            migrationBuilder.DropTable(
                name: "company");

            migrationBuilder.DropTable(
                name: "LegalRepresentative");

            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.DropTable(
                name: "company_special_situations");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropTable(
                name: "economic_activities");

            migrationBuilder.DropTable(
                name: "legal_natures");

            migrationBuilder.DropTable(
                name: "partner_qualifications");

            migrationBuilder.DropTable(
                name: "address_types");

            migrationBuilder.DropTable(
                name: "cities");

            migrationBuilder.DropTable(
                name: "special_situations");
        }
    }
}
