using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SinistrosApi.Infrastructure.Persistencia.Migrations
{
    [Migration("20260704000001_CriarTabelaClientes")]
    public class CriarTabelaClientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clientes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    sobrenome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    tipo_documento = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    numero_documento = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    data_referencia = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table => table.PrimaryKey("pk_clientes", x => x.id));

            migrationBuilder.CreateIndex(
                name: "ix_clientes_numero_documento",
                table: "clientes",
                column: "numero_documento",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "clientes");
        }
    }
}
