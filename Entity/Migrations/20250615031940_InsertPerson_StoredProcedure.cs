using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entity.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
            CREATE PROCEDURE [dbo].[InsertPersones]
            (@PersonId uniqueidentifier, @PersonName nvarchar(50),@Email nvarchar(50),
            @DateOfBirth datetime2(7), @Gender nvarchar(10), @CountryId uniqueidentifier,
            @Address nvarchar(100), @RecieveNewsLetters bit)
            AS BEGIN
                Insert into [dbo].[Persons](PersonId,PersonName,Email,DateOfBirth,Gender,CountryId,
                Address,RecieveNewsLetters)  Values(@PersonId,@PersonName
,@Email,@DateOfBirth,@Gender,@CountryId,@Address,@RecieveNewsLetters)
            END
           ";
            migrationBuilder.Sql(sp_InsertPerson);
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
            DROP PROCEDURE [dbo].[InsertPersones]
"; migrationBuilder.Sql(sp_InsertPerson);
        }
    }
}
