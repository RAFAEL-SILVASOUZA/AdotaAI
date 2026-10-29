using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdotaAI.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 70, nullable: false),
                    Species = table.Column<int>(type: "INTEGER", maxLength: 50, nullable: false),
                    Race = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Age = table.Column<int>(type: "INTEGER", nullable: false),
                    IsFemale = table.Column<bool>(type: "INTEGER", nullable: false),
                    PetSize = table.Column<int>(type: "INTEGER", nullable: false),
                    Photo = table.Column<string>(type: "TEXT", nullable: false),
                    VetRecord = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    BehaviourDesc = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    PetStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    InstitutionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_Email",
                table: "Institutions",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Institutions_Email",
                table: "Institutions");
        }
    }
}
