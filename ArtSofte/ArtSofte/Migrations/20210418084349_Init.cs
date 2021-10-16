using Microsoft.EntityFrameworkCore.Migrations;

namespace Artsofte.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Departments",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    FloorNumber = table.Column<int>("int", nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Departments", x => x.Id); });

            migrationBuilder.CreateTable(
                "ProgrammingLanguages",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_ProgrammingLanguages", x => x.Id); });

            migrationBuilder.CreateTable(
                "Employees",
                table => new
                {
                    Id = table.Column<int>("int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>("nvarchar(max)", nullable: true),
                    LastName = table.Column<string>("nvarchar(max)", nullable: true),
                    Age = table.Column<string>("nvarchar(max)", nullable: true),
                    Hide = table.Column<bool>("bit", nullable: false),
                    DepartmentId = table.Column<int>("int", nullable: false),
                    ProgrammingLanguageId = table.Column<int>("int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        "FK_Employees_Departments_DepartmentId",
                        x => x.DepartmentId,
                        "Departments",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Employees_ProgrammingLanguages_ProgrammingLanguageId",
                        x => x.ProgrammingLanguageId,
                        "ProgrammingLanguages",
                        "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                "IX_Employees_DepartmentId",
                "Employees",
                "DepartmentId");

            migrationBuilder.CreateIndex(
                "IX_Employees_ProgrammingLanguageId",
                "Employees",
                "ProgrammingLanguageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Employees");

            migrationBuilder.DropTable(
                "Departments");

            migrationBuilder.DropTable(
                "ProgrammingLanguages");
        }
    }
}