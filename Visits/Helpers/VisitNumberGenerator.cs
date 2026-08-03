using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Kiosk.Visits.Helpers
{
    public static class VisitNumberGenerator
    {
        public static async Task<int> GetNextVisitId(NpgsqlConnection connection)
        {
            var lastId = await connection.QueryFirstOrDefaultAsync<int?>(
            """
            SELECT VisitId
            FROM Visits
            ORDER BY VisitId DESC
            LIMIT 1;
            """
            );

            return (lastId ?? 0) + 1;
        }
    }
    
}