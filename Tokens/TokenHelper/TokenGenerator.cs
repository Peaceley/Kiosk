using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Kiosk.Tokens.TokenHelper
{
    public static class TokenGenerator
    {
        //method for the token generation

        public static async Task<string> GenerateToken(IDbConnection connection, string medicalServiceCode)
        {

            //the 
            var lastNumber = await connection.QuerySingleOrDefaultAsync<int?>(
            """
            SELECT MAX(
                CAST(SUBSTRING(tokenno, 4) AS INTEGER)
            )
            FROM tokens
            WHERE tokenno LIKE @Prefix || '%'
            """,
            new
            {
                Prefix = medicalServiceCode
            });

            var nextNumber = (lastNumber ?? 0) + 1;

            return $"{medicalServiceCode}{nextNumber:D3}";

            
        }
    }
}