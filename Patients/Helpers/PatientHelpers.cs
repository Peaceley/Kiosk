using System.ComponentModel.Design;
using System.Data;
using Dapper;
//using PatientEndpoints;
namespace PatientUtils;

public static class PatientHelpers
{
    public async static Task<int>GetNextPatientId(IDbConnection connection)
    {
        //sql to get the greatest patient id 
        //SELECT Max(PatientId)from Patients
        var sql = "SELECT MAX(PatientId) FROM Patients";

        // int? greatestId =  connection.ExecuteScalar<int?>(sql);

        // int id = greatestId ?? 0;

        int greatestId = connection.QuerySingleOrDefault<int>(sql);
        return greatestId + 1;


      //  int greatestId =  await connection.ExecuteScalarAsync<int>(sql);
       } //int id = await ExecuteScalarAsync
        // return highest + 1
    }

