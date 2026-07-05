using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinistrosApi.Infrastructure.Persistencia.Migrations
{
    [Migration("20260704000003_CriarTabelaSinistros")]
    public class CriarTabelaSinistros : Migration
    {
        private const string sinistros = "sinistros";

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: sinistros,
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    apolice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_abertura = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    valor_estimado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_aprovado = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    motivo_negativa = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sinistros", x => x.id);
                    table.ForeignKey(
                        name: "fk_sinistros_apolices_apolice_id",
                        column: x => x.apolice_id,
                        principalTable: "apolices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "historico_sinistros",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sinistro_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status_anterior = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    status_novo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_historico_sinistros", x => x.id);
                    table.ForeignKey(
                        name: "fk_historico_sinistros_sinistros_sinistro_id",
                        column: x => x.sinistro_id,
                        principalTable: sinistros,
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sinistros_apolice_id",
                table: sinistros,
                column: "apolice_id");

            migrationBuilder.CreateIndex(
                name: "ix_historico_sinistros_sinistro_id",
                table: "historico_sinistros",
                column: "sinistro_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "historico_sinistros");
            migrationBuilder.DropTable(name: sinistros);
        }
    }
}
