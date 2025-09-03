using FluentMigrator;

namespace OpenCnpj.ConsoleApp.Core.Database.Migrations;

[Migration(0)]
public class InitialMigration : Migration
{
    public override void Up()
    {
        Create.Table("application_steps")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("description").AsString().NotNullable();

        CreateApplicationSteps();
        
        Create.Table("batches")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("identifier").AsString(50).NotNullable().Unique()
            .WithColumn("status").AsString(30).NotNullable()
            .WithColumn("application_last_step_id").AsInt32().NotNullable();
        
        Create.Table("file_statuses")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("description").AsString().NotNullable();

        CreateFileStatuses();

        Create.Table("batch_files")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("batch_id").AsInt32().NotNullable()
            .WithColumn("file_status_id").AsInt32().NotNullable()
            .WithColumn("file_name").AsString(50).NotNullable()
            .WithColumn("file_path").AsString(255).NotNullable();

        Create.Table("age_ranges")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("description").AsString().NotNullable();

        InsertAgeRanges();

        Create.Table("company_sizes")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("description").AsString().NotNullable();

        InsertCompanySizes();

        Create.Table("company_situations")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("description").AsString().NotNullable();

        InsertCompanySituations();

        Create.Table("company_types")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("description").AsString().NotNullable();

        InsertCompanyTypes();

        Create.Table("parter_types")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("description").AsString().NotNullable();

        InsertParterTypes();

        Create.Table("countries")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("code").AsString().Unique()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("cities")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("code").AsString().Unique()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("partner_qualifications")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("code").AsString().Unique()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("legal_natures")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("code").AsString().Unique()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("economic_activities")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("code").AsString().Unique()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("reasons")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("code").AsString().Unique()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("adress_types")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("special_situations")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("description").AsString().NotNullable()
            .WithColumn("situation_date").AsDate().NotNullable();

        Create.Table("address_types")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("phones")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("area_code").AsInt32().NotNullable()
            .WithColumn("phone").AsInt32().NotNullable();

        Create.Table("contacts")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("phone_id").AsInt64().NotNullable()
            .WithColumn("fax_area_code").AsInt32().NotNullable()
            .WithColumn("fax_number").AsInt32().NotNullable()
            .WithColumn("email_address").AsString().Nullable();
            
        Create.Table("addresses")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("address_type_id").AsInt64().NotNullable()
            .WithColumn("city_id").AsInt64().NotNullable()
            .WithColumn("street").AsString().NotNullable()
            .WithColumn("number").AsInt32().Nullable()
            .WithColumn("complement").AsString().Nullable()
            .WithColumn("neighborhood").AsString().NotNullable()
            .WithColumn("zip_code").AsInt32().Nullable()
            .WithColumn("federal_unit").AsString().NotNullable();

        Create.Table("company")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("legal_nature_id").AsInt64().NotNullable()
            .WithColumn("main_partner_qualification_id").AsInt64().NotNullable()
            .WithColumn("company_size_id").AsInt32().NotNullable()
            .WithColumn("company_type_id").AsInt32().NotNullable()
            .WithColumn("country_id").AsInt64().NotNullable()
            .WithColumn("address_id").AsInt64().NotNullable()
            .WithColumn("main_economic_activity_id").AsInt64().NotNullable()
            .WithColumn("special_situation_id").AsInt64().NotNullable()
            .WithColumn("identifier").AsString().NotNullable()
            .WithColumn("name").AsString().NotNullable()
            .WithColumn("fantasy_name").AsString().NotNullable()
            .WithColumn("share_capital").AsDecimal().Nullable()
            .WithColumn("responsabile_federative_entity").AsString().Nullable()
            .WithColumn("register_date").AsDate().NotNullable()
            .WithColumn("foreign_city_name").AsString().Nullable()
            .WithColumn("start_date").AsDate().NotNullable();

        Create.Table("mei")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("date_opted").AsDate().Nullable()
            .WithColumn("exclusion_date").AsDate().Nullable();

        Create.Table("simples")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("company_id").AsInt64().NotNullable()
            .WithColumn("mei_id").AsInt64().Nullable()
            .WithColumn("is_simple").AsBoolean().Nullable()
            .WithColumn("date_opted").AsDate().Nullable()
            .WithColumn("exclusion_date").AsDate().Nullable();

        Create.Table("company_contacts")
            .WithColumn("company_id").AsInt64().NotNullable()
            .WithColumn("contact_id").AsInt64().NotNullable();

        Create.Table("company_secondary_economic_activities")
            .WithColumn("company_id").AsInt64().NotNullable()
            .WithColumn("economic_activity_id").AsInt64().NotNullable();

        Create.Table("partners")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("partner_type_id").AsInt32().NotNullable();

        // FOREIGN KEYS
        Create.ForeignKey("FK_batch_application_steps")
            .FromTable("batches").ForeignColumn("application_last_step_id")
            .ToTable("application_steps").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_batch_files_file_statuses")
            .FromTable("batch_files").ForeignColumn("file_status_id")
            .ToTable("file_statuses").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);
        
        Create.ForeignKey("FK_contacts_phones")
            .FromTable("contacts").ForeignColumn("phone_id")
            .ToTable("phones").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_addresses_address_types")
            .FromTable("addresses").ForeignColumn("address_type_id")
            .ToTable("address_types").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_addresses_cities")
            .FromTable("addresses").ForeignColumn("city_id")
            .ToTable("cities").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_company_legal_natures")
            .FromTable("company").ForeignColumn("legal_nature_id")
            .ToTable("legal_natures").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_company_partner_qualifications")
            .FromTable("company").ForeignColumn("main_partner_qualification_id")
            .ToTable("partner_qualifications").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_company_company_sizes")
            .FromTable("company").ForeignColumn("company_size_id")
            .ToTable("company_sizes").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_company_company_types")
            .FromTable("company").ForeignColumn("company_type_id")
            .ToTable("company_types").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_company_countries")
            .FromTable("company").ForeignColumn("country_id")
            .ToTable("countries").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_company_addresses")
            .FromTable("company").ForeignColumn("address_id")
            .ToTable("addresses").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_company_economic_activities")
            .FromTable("company").ForeignColumn("main_economic_activity_id")
            .ToTable("economic_activities").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_company_special_situations")
            .FromTable("company").ForeignColumn("special_situation_id")
            .ToTable("special_situations").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_simples_company")
            .FromTable("simples").ForeignColumn("company_id")
            .ToTable("company").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_simples_mei")
            .FromTable("simples").ForeignColumn("mei_id")
            .ToTable("mei").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.SetNull);

        Create.ForeignKey("FK_company_contacts_company")
            .FromTable("company_contacts").ForeignColumn("company_id")
            .ToTable("company").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_company_contacts_contacts")
            .FromTable("company_contacts").ForeignColumn("contact_id")
            .ToTable("contacts").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_company_secondary_economic_activities_company")
            .FromTable("company_secondary_economic_activities").ForeignColumn("company_id")
            .ToTable("company").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_company_secondary_economic_activities_economic_activities")
            .FromTable("company_secondary_economic_activities").ForeignColumn("economic_activity_id")
            .ToTable("economic_activities").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.ForeignKey("FK_partners_parter_types")
            .FromTable("partners").ForeignColumn("partner_type_id")
            .ToTable("parter_types").PrimaryColumn("id")
            .OnDeleteOrUpdate(System.Data.Rule.Cascade);

        Create.PrimaryKey("PK_company_contacts")
            .OnTable("company_contacts")
            .Columns("company_id", "contact_id");

        Create.PrimaryKey("PK_company_secondary_economic_activities")
            .OnTable("company_secondary_economic_activities")
            .Columns("company_id", "economic_activity_id");
    }

    public override void Down()
    {
        // Remover foreign keys primeiro
        Delete.ForeignKey("FK_partners_parter_types").OnTable("partners");
        Delete.ForeignKey("FK_company_secondary_economic_activities_economic_activities").OnTable("company_secondary_economic_activities");
        Delete.ForeignKey("FK_company_secondary_economic_activities_company").OnTable("company_secondary_economic_activities");
        Delete.ForeignKey("FK_company_contacts_contacts").OnTable("company_contacts");
        Delete.ForeignKey("FK_company_contacts_company").OnTable("company_contacts");
        Delete.ForeignKey("FK_simples_mei").OnTable("simples");
        Delete.ForeignKey("FK_simples_company").OnTable("simples");
        Delete.ForeignKey("FK_company_special_situations").OnTable("company");
        Delete.ForeignKey("FK_company_economic_activities").OnTable("company");
        Delete.ForeignKey("FK_company_addresses").OnTable("company");
        Delete.ForeignKey("FK_company_countries").OnTable("company");
        Delete.ForeignKey("FK_company_reasons").OnTable("company");
        Delete.ForeignKey("FK_company_company_types").OnTable("company");
        Delete.ForeignKey("FK_company_company_sizes").OnTable("company");
        Delete.ForeignKey("FK_company_partner_qualifications").OnTable("company");
        Delete.ForeignKey("FK_company_legal_natures").OnTable("company");
        Delete.ForeignKey("FK_addresses_cities").OnTable("addresses");
        Delete.ForeignKey("FK_addresses_address_types").OnTable("addresses");
        Delete.ForeignKey("FK_contacts_phones").OnTable("contacts");

        // Remover tabelas
        Delete.Table("partners").IfExists();
        Delete.Table("company_secondary_economic_activities").IfExists();
        Delete.Table("company_contacts").IfExists();
        Delete.Table("simples").IfExists();
        Delete.Table("mei").IfExists();
        Delete.Table("company").IfExists();
        Delete.Table("addresses").IfExists();
        Delete.Table("contacts").IfExists();
        Delete.Table("phones").IfExists();
        Delete.Table("address_types").IfExists();
        Delete.Table("special_situations").IfExists();
        Delete.Table("adress_types").IfExists();
        Delete.Table("reasons").IfExists();
        Delete.Table("economic_activities").IfExists();
        Delete.Table("legal_natures").IfExists();
        Delete.Table("partner_qualifications").IfExists();
        Delete.Table("cities").IfExists();
        Delete.Table("countries").IfExists();
        Delete.Table("parter_types").IfExists();
        Delete.Table("company_types").IfExists();
        Delete.Table("company_situations").IfExists();
        Delete.Table("company_sizes").IfExists();
        Delete.Table("age_ranges").IfExists();
        Delete.Table("batches").IfExists();
    }

    private void CreateApplicationSteps()
    {
        Insert.IntoTable("application_steps").Row(new { id = 0, description = "APLICAÇÃO INICIALIZADA" });
        Insert.IntoTable("application_steps").Row(new { id = 1, description = "REALIZANDO DOWNLOAD DOS ARQUIVOS" });
        Insert.IntoTable("application_steps").Row(new { id = 2, description = "EXTRAINDO ARQUIVOS BRUTOS" });
        Insert.IntoTable("application_steps").Row(new { id = 3, description = "PROCESSANDO DADOS BRUTOS" });
        Insert.IntoTable("application_steps").Row(new { id = 4, description = "FORMATANDO DADOS BRUTOS" });
    }

    private void CreateFileStatuses()
    {
        Insert.IntoTable("file_statuses").Row(new { id = 1, description = "FAZENDO DOWNLOAD" });
        Insert.IntoTable("file_statuses").Row(new { id = 2, description = "ARQUIVO CRIADO" });
        Insert.IntoTable("file_statuses").Row(new { id = 3, description = "PROCESSANDO ARQUIVO" });
        Insert.IntoTable("file_statuses").Row(new { id = 4, description = "ARQUIVO PROCESSADO" });
        Insert.IntoTable("file_statuses").Row(new { id = 5, description = "ARQUIVO DELETADO" });
    }

    private void InsertAgeRanges()
    {
        Insert.IntoTable("age_ranges").Row(new { id = 0, description = "NÃO SE APLICA" });
        Insert.IntoTable("age_ranges").Row(new { id = 1, description = "ENTRE 0 A 12 ANOS" });
        Insert.IntoTable("age_ranges").Row(new { id = 2, description = "ENTRE 13 A 20 ANOS" });
        Insert.IntoTable("age_ranges").Row(new { id = 3, description = "ENTRE 21 A 30 ANOS" });
        Insert.IntoTable("age_ranges").Row(new { id = 4, description = "ENTRE 31 A 40 ANOS" });
        Insert.IntoTable("age_ranges").Row(new { id = 5, description = "ENTRE 41 A 50 ANOS" });
        Insert.IntoTable("age_ranges").Row(new { id = 6, description = "ENTRE 51 A 60 ANOS" });
        Insert.IntoTable("age_ranges").Row(new { id = 7, description = "ENTRE 61 A 70 ANOS" });
        Insert.IntoTable("age_ranges").Row(new { id = 8, description = "ENTRE 71 A 80 ANOS" });
        Insert.IntoTable("age_ranges").Row(new { id = 9, description = "MAIOR QUE 80 ANOS" });
    }

    private void InsertCompanySizes()
    {
        Insert.IntoTable("company_sizes").Row(new { id = 0, description = "NÃO INFORMADO" });
        Insert.IntoTable("company_sizes").Row(new { id = 1, description = "MICRO EMPRESA" });
        Insert.IntoTable("company_sizes").Row(new { id = 3, description = "EMPRESA DE PEQUENO PORTE" });
        Insert.IntoTable("company_sizes").Row(new { id = 5, description = "DEMAIS" });
    }

    private void InsertCompanySituations()
    {
        Insert.IntoTable("company_situations").Row(new { id = 1, description = "NULA" });
        Insert.IntoTable("company_situations").Row(new { id = 2, description = "ATIVA" });
        Insert.IntoTable("company_situations").Row(new { id = 3, description = "SUSPENSA" });
        Insert.IntoTable("company_situations").Row(new { id = 4, description = "INAPTA" });
        Insert.IntoTable("company_situations").Row(new { id = 8, description = "BAIXADA" });
    }

    private void InsertCompanyTypes()
    {
        Insert.IntoTable("company_types").Row(new { id = 1, description = "MATRIZ" });
        Insert.IntoTable("company_types").Row(new { id = 2, description = "FILIAL" });
    }

    private void InsertParterTypes()
    {
        Insert.IntoTable("parter_types").Row(new { id = 1, description = "PESSOA JURÍCIDA" });
        Insert.IntoTable("parter_types").Row(new { id = 2, description = "PESSOA FÍSICA" });
        Insert.IntoTable("parter_types").Row(new { id = 3, description = "ESTRANGEIRO" });
    }
}
