using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductSklepInternetowyUI.Migrations
{
    /// <inheritdoc />
    public partial class TopNSellingProductsStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sql = @"
            EXEC ('CREATE PROCEDURE [dbo].[Usp_GetTopNSellingProductsByDate]
            @startDate datetime,
            @endDate datetime
            AS
            BEGIN
                SET NOCOUNT ON;

                WITH UnitSold AS
                (
                    SELECT od.ProductId, SUM(od.Quantity) AS TotalUnitSold 
                    FROM [Order] o 
                    JOIN OrderDetail od ON o.Id = od.OrderId
                    WHERE o.IsPaid = 1 AND o.IsDeleted = 0 AND o.CreateDate BETWEEN @startDate AND @endDate
                    GROUP BY od.ProductId
                )
                SELECT TOP 5 b.ProductName, b.AuthorName, b.[Image], us.TotalUnitSold 
                FROM UnitSold us
                JOIN [Product] b ON us.ProductId = b.Id
                ORDER BY us.TotalUnitSold DESC;
            END');
          ";
            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
