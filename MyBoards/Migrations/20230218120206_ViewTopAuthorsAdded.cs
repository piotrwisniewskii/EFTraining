using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoards.Migrations
{
    public partial class ViewTopAuthorsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            Create View View_TopAuthors AS
            SELECT TOP 5 u.FullName, Count(*) as[WorkItemsCreated]
            FROM Users u
            JOIN WorkItems wi on wi.AuthorId = u.Id
            Group BY u.Id, u.FullName
            ORDER BY [WorkItemsCreated] DESC
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            DROP View View_TopAuthors          
            ");
        }
    }
}
