using FluentMigrator;

namespace OpenCnpj.Core.Database.Migrations;

[Migration(0)]
public class InitialMigration : Migration
{
    public override void Up()
    {
        Create.Table("batches")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("period").AsString(50).NotNullable().Unique()
            .WithColumn("operation").AsInt16().NotNullable()
            .WithColumn("operation_status").AsInt16().NotNullable()
            .WithColumn("operation_failure_description").AsString(10000).Nullable()
            .WithColumn("directory").AsString(255).Nullable()
            .WithColumn("retry_date").AsDateTime().Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
            .WithColumn("finished_at").AsDateTime().Nullable();

        Create.Table("batch_files")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("parent_batch_file_id").AsInt32().Nullable()
            .WithColumn("batch_id").AsInt32().NotNullable()
            .WithColumn("url").AsString(255).Nullable()
            .WithColumn("extension").AsString(255).NotNullable()
            .WithColumn("file_name").AsString(255).NotNullable()
            .WithColumn("file_path").AsString(255).NotNullable()
            .WithColumn("file_operation").AsInt16().NotNullable()
            .WithColumn("operation_status").AsInt16().NotNullable()
            .WithColumn("type").AsInt16().NotNullable()
            .WithColumn("operation_failure_description").AsString(10000).Nullable()
            .WithColumn("is_file_deleted").AsBoolean().Nullable()
            .WithColumn("created_at").AsDateTime().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("batches").IfExists();
        Delete.Table("batch_files").IfExists();
    }
}