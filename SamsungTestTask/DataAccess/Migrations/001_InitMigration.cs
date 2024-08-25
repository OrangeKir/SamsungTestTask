using SamsungTestTask.Infrastructure;
using FluentMigrator;

namespace SamsungTestTask.DataAccess.Migrations;

[Migration(1)]
public class InitMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\";");
        Create.Table(PgTables.Record)
            .WithColumn("id").AsGuid().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn("customer_id").AsString(20).NotNullable()
            .WithColumn("posting_date").AsDateTime().NotNullable()
            .WithColumn("amount").AsDecimal().NotNullable();
    }
}