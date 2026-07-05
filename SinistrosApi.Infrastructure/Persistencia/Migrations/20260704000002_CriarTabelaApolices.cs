using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinistrosApi.Infrastructure.Persistencia.Migrations
{
    [Migration("20260704000002_CriarTabelaApolices")]
    public class CriarTabelaApolices : Migration
    {
        private const string apolices = "apolices";
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: apolices,
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    numero_apolice = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cliente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ramo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    data_inicio_vigencia = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim_vigencia = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_apolices", x => x.id);
                    table.ForeignKey(
                        name: "fk_apolices_clientes_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_apolices_cliente_id",
                table: apolices,
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "ix_apolices_numero_apolice",
                table: apolices,
                column: "numero_apolice",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: apolices);
        }
    }
}
