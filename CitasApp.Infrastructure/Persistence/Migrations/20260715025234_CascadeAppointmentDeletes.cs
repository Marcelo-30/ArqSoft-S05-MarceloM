using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CitasApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CascadeAppointmentDeletes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_citas_medicos_medico_id",
                table: "citas");

            migrationBuilder.DropForeignKey(
                name: "FK_citas_pacientes_paciente_id",
                table: "citas");

            migrationBuilder.AddForeignKey(
                name: "FK_citas_medicos_medico_id",
                table: "citas",
                column: "medico_id",
                principalTable: "medicos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_citas_pacientes_paciente_id",
                table: "citas",
                column: "paciente_id",
                principalTable: "pacientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_citas_medicos_medico_id",
                table: "citas");

            migrationBuilder.DropForeignKey(
                name: "FK_citas_pacientes_paciente_id",
                table: "citas");

            migrationBuilder.AddForeignKey(
                name: "FK_citas_medicos_medico_id",
                table: "citas",
                column: "medico_id",
                principalTable: "medicos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_citas_pacientes_paciente_id",
                table: "citas",
                column: "paciente_id",
                principalTable: "pacientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
