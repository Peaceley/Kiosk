using System.Data;
using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.VisualBasic;
using PatientUtils;

public static class PatientEndpoints
{
    public static IEndpointRouteBuilder MapPatientEndpoints(this IEndpointRouteBuilder routes)
    {
        // Group endpoints to share a common /products prefix
        var group = routes.MapGroup("/patients");

        group.MapPost("/", CreatePatient);

        group.MapGet("/", GetPatients);
        group.MapGet("/{PatientNo}", GetPatientsByPatientNo);
        group.MapDelete("/{PatientId}", DeletePatientsByPatientId);
        group.MapPatch("/{patientId}", UpdatePatient);


        return routes;
    }

    private async static Task<IResult> CreatePatient(PatientCreateRequests patient, IDbConnection connection)
    {
        try
        {
            int nextPatientId = await PatientHelpers.GetNextPatientId(connection);
            string patientNo = DateTime.Now.ToString("yy") + nextPatientId.ToString().PadLeft(3, '0');
            Console.WriteLine(patientNo);


            // Inside a real app, you would save 'product' to a database here
            PatientFullCreateRequests patientFullCreateRequests = new()
            {
                PatientNo = patientNo,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Gender = patient.Gender,
                Contact = patient.Contact,
                Address = patient.Address
            };



            var sql = """
    INSERT INTO patients(PatientNo, FirstName, LastName, Gender, Contact,Address)
    VALUES (@PatientNo, @FirstName, @LastName, @Gender, @Contact,@Address)
    returning *;
    """;

            Patientresponse response = await connection.QuerySingleAsync<Patientresponse>(sql, patientFullCreateRequests);


            return Results.Created("/patients/{patientNo}", response);

        }
        catch (Exception ex)
        {
            return Results.InternalServerError(ex.Message);

        }
    }


    // Move the actual logic to private static methods to keep routing clean
    private async static Task<IResult> GetPatients(IDbConnection connection)
    {
        var sql = """ 
        SELECT 
        PatientId, 
        PatientNo, 
        FirstName, 
        LastName, 
        Gender, 
        Contact,
        Address,
        CreatedAt FROM Patients;
        """;
        var patient = await connection.QueryAsync<Patientresponse>(sql);

        return Results.Ok(patient);
    }

    private static async Task<IResult> GetPatientsByPatientNo(string patientNo, IDbConnection connection)
    {
        var parameter = new { PatientNo = patientNo };
        var sql = """ 
            SELECT 
                PatientId,
                PatientNo, 
                FirstName, 
                LastName, 
                Gender, 
                Contact,
                Address,
                CreatedAt
            FROM Patients
            WHERE PatientNo = @PatientNo
        ;
        """;
        var patient = await connection.QuerySingleOrDefaultAsync<Patientresponse>(sql, parameter);

        return Results.Ok(patient);
    }
    private static async Task<IResult> DeletePatientsByPatientId(int patientId, IDbConnection connection)


    {
        try
        {
            var parameter1 = new { PatientId = patientId };
            var sql = @"DELETE FROM Students WHERE PatientId=patientId;";
            var rowsAffected = await connection.ExecuteAsync(sql, parameter1);
            if (rowsAffected != 0)
            {
                return Results.Ok("Patients deleted successfully.");
            }
            else
            {
                return Results.NotFound("Patients not found.");
            }
        }
        catch (Exception ex)
        {
            //return Results.NotFound("Patients not found.");
            return Results.InternalServerError(ex.Message);

        }
    }
 
    private static async Task<IResult> UpdatePatient(int patientId, PatientNoUpdateRequests  updateRequest, IDbConnection connection)
    {


        var sql =
        """
            UPDATE Patients SET   
            
                FirstName=@FirstName,
                LastName=@LastName,
                Gender=@Gender, 
                Contact=@Contact,
                Address=@Address,
                CreatedAt=CreatedAt
            WHERE PatientId = @PatientId;
        """;
        var parameter = new
        {
            PatientId = patientId,
            //PatientNo = updateRequest.PatientNo,
            FirstName = updateRequest.FirstName,
            LastName = updateRequest.LastName,
            Gender = updateRequest.Gender,
            Contact = updateRequest.Contact,
            Address = updateRequest.Address
        };
        var patient = await connection.QuerySingleOrDefaultAsync<Patientresponse>(sql, parameter);
        return Results.Ok(patient);
    }

    //group.MapPut("/patients/{patientId}", UpdatePatient);
    // if (rowsAffected != 0)
    // {
    //     return Results.Ok("Patient successfully updated");
    // }
    // else
    // {
    //     return Results.NotFound("Patient not found");
    // }


    // }
    // catch (Exception ex)
    // {
    //     return Results.InternalServerError(ex.Message);

    // }





};






