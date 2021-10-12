using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Assignment4.Entities.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagTask_Tasks_Tasksid",
                table: "TagTask");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Tasks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Tasksid",
                table: "TagTask",
                newName: "TasksId");

            migrationBuilder.RenameIndex(
                name: "IX_TagTask_Tasksid",
                table: "TagTask",
                newName: "IX_TagTask_TasksId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusUpdated",
                table: "Tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_TagTask_Tasks_TasksId",
                table: "TagTask",
                column: "TasksId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagTask_Tasks_TasksId",
                table: "TagTask");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StatusUpdated",
                table: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Tasks",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TasksId",
                table: "TagTask",
                newName: "Tasksid");

            migrationBuilder.RenameIndex(
                name: "IX_TagTask_TasksId",
                table: "TagTask",
                newName: "IX_TagTask_Tasksid");

            migrationBuilder.AddForeignKey(
                name: "FK_TagTask_Tasks_Tasksid",
                table: "TagTask",
                column: "Tasksid",
                principalTable: "Tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
