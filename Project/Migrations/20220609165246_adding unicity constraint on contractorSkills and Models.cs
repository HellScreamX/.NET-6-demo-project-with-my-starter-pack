using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    public partial class addingunicityconstraintoncontractorSkillsandModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContractorSkills_SkillId",
                table: "ContractorSkills");

            migrationBuilder.DropIndex(
                name: "IX_ContractorModels_ModelId",
                table: "ContractorModels");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorSkills_SkillId_ContractorId",
                table: "ContractorSkills",
                columns: new[] { "SkillId", "ContractorId" },
                unique: true,
                filter: "[ContractorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorModels_ModelId_ContractorId",
                table: "ContractorModels",
                columns: new[] { "ModelId", "ContractorId" },
                unique: true,
                filter: "[ContractorId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContractorSkills_SkillId_ContractorId",
                table: "ContractorSkills");

            migrationBuilder.DropIndex(
                name: "IX_ContractorModels_ModelId_ContractorId",
                table: "ContractorModels");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorSkills_SkillId",
                table: "ContractorSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorModels_ModelId",
                table: "ContractorModels",
                column: "ModelId");
        }
    }
}
