using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class GetPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_getPerson = @"
            CREATE PROCEDURE [dbo].[GetPerson]
            AS BEGIN
                SELECT PersonId,PersonName,Email,DateOfBirth,Gender,CountryId,
Address,RecieveNewsLetters FROM [dbo].[Persons]
            END
";          migrationBuilder.Sql(sp_getPerson);
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_getPerson = @"
            DROP PROCEDURE [dbo].[GetPerson]
"; migrationBuilder.Sql(sp_getPerson);
        }
    }
}
