namespace NovatorTestTask.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnotation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Workers", "SecondName", c => c.String(nullable: false));
            AlterColumn("dbo.Workers", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Workers", "Patronymic", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Workers", "Patronymic", c => c.String());
            AlterColumn("dbo.Workers", "FirstName", c => c.String());
            AlterColumn("dbo.Workers", "SecondName", c => c.String());
        }
    }
}
