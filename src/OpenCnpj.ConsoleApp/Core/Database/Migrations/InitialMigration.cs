using FluentMigrator;

namespace OpenCnpj.ConsoleApp.Core.Database.Migrations;

[Migration(0)]
public class InitialMigration : Migration
{
    public override void Up()
    {
        Create.Sequence("seq_batches").MinValue(1).Cache(10).IncrementBy(1);

        Create.Table("batches")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("identifier").AsString(50).NotNullable().Unique()
            .WithColumn("status").AsString(30).NotNullable();

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
            .WithColumn("description").AsString().NotNullable();

        Create.Table("cities")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("partner_qualifications")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("legal_natures")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("economic_activities")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("description").AsString().NotNullable();

        Create.Table("reasons")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
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

        //TODO: Adicionar FK's
            
        Create.Table("addresses")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("address_type_id").AsString().NotNullable()
            .WithColumn("city_id").AsInt64().NotNullable()
            .WithColumn("description").AsString().NotNullable()
            .WithColumn("number").AsInt32().NotNullable()
            .WithColumn("complement").AsString().Nullable()
            .WithColumn("neighborhood").AsString().NotNullable()
            .WithColumn("cep_number").AsInt16().NotNullable()
            .WithColumn("federal_unit").AsString().NotNullable();

        Create.Table("company")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("legal_nature_id").AsInt64().NotNullable()
            .WithColumn("main_partner_qualification_id").AsInt64().NotNullable()
            .WithColumn("company_size_id").AsInt32().NotNullable()
            .WithColumn("company_type_id").AsInt32().NotNullable()
            .WithColumn("reason_id").AsInt64().NotNullable()
            .WithColumn("country_id").AsInt64().NotNullable()
            .WithColumn("address_id").AsInt64().NotNullable()
            .WithColumn("main_economic_activity_id").AsInt64().NotNullable()
            .WithColumn("special_situation_id").AsInt64().NotNullable()
            .WithColumn("identifier").AsString().NotNullable()
            .WithColumn("name").AsString().NotNullable()
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
    }

    public override void Down()
    {
        Delete.Table("batches").IfExists();
        Delete.Table("age_ranges").IfExists();
        Delete.Table("company_sizes").IfExists();
        Delete.Table("company_situations").IfExists();
        Delete.Table("company_types").IfExists();
        Delete.Table("parter_types").IfExists();
        Delete.Table("countries").IfExists();
        Delete.Table("cities").IfExists();
        Delete.Table("partner_qualifications").IfExists();
        Delete.Table("legal_natures").IfExists();
        Delete.Table("economic_activities").IfExists();
        Delete.Table("reasons").IfExists();
        Delete.Table("adress_types").IfExists();

        Delete.Sequence("seq_batches");
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
