using Microsoft.EntityFrameworkCore.Migrations;

namespace MinhasTarefasAPI.Migrations
{
    public partial class Sicronizacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tarefas",
                table: "Tarefas");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tarefas",
                newName: "IdtarefaApi");

            migrationBuilder.AlterColumn<int>(
                name: "IdtarefaApi",
                table: "Tarefas",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "IdtarefaApp",
                table: "Tarefas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<bool>(
                name: "Excluido",
                table: "Tarefas",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tarefas",
                table: "Tarefas",
                column: "IdtarefaApp");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tarefas",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "IdtarefaApp",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "Excluido",
                table: "Tarefas");

            migrationBuilder.RenameColumn(
                name: "IdtarefaApi",
                table: "Tarefas",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tarefas",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tarefas",
                table: "Tarefas",
                column: "Id");
        }
    }
}
