using FluentMigrator;

namespace OpenCnpj.Core.Database.Migrations;

[Migration(0)]
public class InitialMigration : Migration
{
    public override void Up()
    {
        CreateApplicationSteps();

        CreateBatches();

        CreateFileStatuses();

        Create.Table("batch_files")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("batch_id").AsInt32().NotNullable()
            .WithColumn("file_status_id").AsInt32().NotNullable()
            .WithColumn("file_name").AsString(50).NotNullable()
            .WithColumn("file_path").AsString(255).NotNullable();

    }

    public override void Down()
    {
        Delete.Table("batch_files").IfExists();
        Delete.Table("batches").IfExists();
        Delete.Table("application_steps").IfExists();
    }

    private void CreateApplicationSteps()
    {
        Create.Table("application_steps")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("description").AsString().NotNullable();

        Insert.IntoTable("application_steps").Row(new { id = 0, description = "APLICAÇÃO INICIALIZADA" });
        Insert.IntoTable("application_steps").Row(new { id = 1, description = "REALIZANDO DOWNLOAD DOS ARQUIVOS" });
        Insert.IntoTable("application_steps").Row(new { id = 2, description = "EXTRAINDO ARQUIVOS BRUTOS" });
        Insert.IntoTable("application_steps").Row(new { id = 3, description = "PROCESSANDO DADOS BRUTOS" });
        Insert.IntoTable("application_steps").Row(new { id = 4, description = "FORMATANDO DADOS BRUTOS" });
    }

    public void CreateBatches()
    {
        Create.Table("batches")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("identifier").AsString(50).NotNullable().Unique()
            .WithColumn("status").AsString(30).NotNullable()
            .WithColumn("application_last_step_id").AsInt32().NotNullable()
                .ForeignKey("application_steps", "id");
    }

    private void CreateFileStatuses()
    {
        Create.Table("file_statuses")
            .WithColumn("id").AsInt32().PrimaryKey()
            .WithColumn("description").AsString().NotNullable();

        Insert.IntoTable("file_statuses").Row(new { id = 1, description = "FAZENDO DOWNLOAD" });
        Insert.IntoTable("file_statuses").Row(new { id = 2, description = "ARQUIVO CRIADO" });
        Insert.IntoTable("file_statuses").Row(new { id = 3, description = "PROCESSANDO ARQUIVO" });
        Insert.IntoTable("file_statuses").Row(new { id = 4, description = "ARQUIVO PROCESSADO" });
        Insert.IntoTable("file_statuses").Row(new { id = 5, description = "ARQUIVO DELETADO" });
    }
}