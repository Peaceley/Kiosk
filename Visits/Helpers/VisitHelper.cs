using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Kiosk.Visits.Helpers
{
    public static class VisitHelper
    {
        public async static Task<int>GetVisitId(IDbConnection connection)
    {
        //Sql for obtaining the visitid
        var sql = "SELECT visitid FROM Visits";

        int visitid = await connection.QuerySingleOrDefaultAsync<int>(sql);

        return visitid;

       } 
    }
}