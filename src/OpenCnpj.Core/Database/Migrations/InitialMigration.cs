using FluentMigrator;

namespace OpenCnpj.Core.Database.Migrations;

[Migration(0)]
public class InitialMigration : Migration
{
    public override void Up()
    {
        Create.Table("batches")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("identifier").AsString(50).NotNullable().Unique()
            .WithColumn("operation").AsInt32().NotNullable()
            .WithColumn("operation_status").AsInt32().NotNullable()
            .WithColumn("operation_failure_description").AsString(10000).Nullable()
            .WithColumn("directory").AsString(255).Nullable()
            .WithColumn("retry_date").AsDateTime().Nullable();
    }

    public override void Down()
    {
        Delete.Table("batches").IfExists();
    }
}