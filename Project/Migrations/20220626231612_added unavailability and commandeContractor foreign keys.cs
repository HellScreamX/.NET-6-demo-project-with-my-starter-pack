using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    public partial class addedunavailabilityandcommandeContractorforeignkeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ContractorId",
                table: "ContractorCommandes",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractorCommandes_CommandeId",
                table: "ContractorCommandes",
                column: "CommandeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorCommandes_ContractorId",
                table: "ContractorCommandes",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorCommandeDays_ContractorCommandeId",
                table: "ContractorCommandeDays",
                column: "ContractorCommandeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractorCommandeDays_ContractorCommandes_ContractorCommandeId",
                table: "ContractorCommandeDays",
                column: "ContractorCommandeId",
                principalTable: "ContractorCommandes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractorCommandes_AspNetUsers_ContractorId",
                table: "ContractorCommandes",
                column: "ContractorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractorCommandes_Commandes_CommandeId",
                table: "ContractorCommandes",
                column: "CommandeId",
                principalTable: "Commandes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractorCommandeDays_ContractorCommandes_ContractorCommandeId",
                table: "ContractorCommandeDays");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractorCommandes_AspNetUsers_ContractorId",
                table: "ContractorCommandes");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractorCommandes_Commandes_CommandeId",
                table: "ContractorCommandes");

            migrationBuilder.DropIndex(
                name: "IX_ContractorCommandes_CommandeId",
                table: "ContractorCommandes");

            migrationBuilder.DropIndex(
                name: "IX_ContractorCommandes_ContractorId",
                table: "ContractorCommandes");

            migrationBuilder.DropIndex(
                name: "IX_ContractorCommandeDays_ContractorCommandeId",
                table: "ContractorCommandeDays");

            migrationBuilder.AlterColumn<string>(
                name: "ContractorId",
                table: "ContractorCommandes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
