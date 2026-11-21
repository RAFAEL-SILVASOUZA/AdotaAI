using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdotaAI.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInstitutionForeignKeysToPetsAndVeterinarians : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Veterinarians_InstitutionId",
                table: "Veterinarians",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_InstitutionId",
                table: "Pets",
                column: "InstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Institutions_InstitutionId",
                table: "Pets",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Veterinarians_Institutions_InstitutionId",
                table: "Veterinarians",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Institutions_InstitutionId",
                table: "Pets");

            migrationBuilder.DropForeignKey(
                name: "FK_Veterinarians_Institutions_InstitutionId",
                table: "Veterinarians");

            migrationBuilder.DropIndex(
                name: "IX_Veterinarians_InstitutionId",
                table: "Veterinarians");

            migrationBuilder.DropIndex(
                name: "IX_Pets_InstitutionId",
                table: "Pets");
        }
    }
}
