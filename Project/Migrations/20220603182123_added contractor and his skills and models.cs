using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    public partial class addedcontractorandhisskillsandmodels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContractorModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractorModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractorModels_AspNetUsers_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractorModels_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractorSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SkillId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractorSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractorSkills_AspNetUsers_ContractorId",
                        column: x => x.ContractorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContractorSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContractorModels_ContractorId",
                table: "ContractorModels",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorModels_ModelId",
                table: "ContractorModels",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorSkills_ContractorId",
                table: "ContractorSkills",
                column: "ContractorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContractorSkills_SkillId",
                table: "ContractorSkills",
                column: "SkillId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractorModels");

            migrationBuilder.DropTable(
                name: "ContractorSkills");
        }
    }
}
