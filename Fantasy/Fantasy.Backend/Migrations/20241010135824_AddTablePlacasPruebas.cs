using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fantasy.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddTablePlacasPruebas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlacasPruebas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Placa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlacaDesencriptada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Soat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rtm = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlacasPruebas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlacasPruebas");
        }
    }
}
