using System.Data;
using Dapper;
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

        return routes;
    }

    private async static Task<IResult> CreatePatient(PatientCreateRequests patient, IDbConnection connection)
    {
        try
        {
            int nextPatientId = await PatientHelpers.GetNextPatientId(connection);
            string  patientNo = DateTime.Now.ToString("yy") + nextPatientId.ToString().PadLeft(3,'0');
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


        return Results.Created("/patients/{patientNo}", response );

    }
        catch(Exception ex)
        {
            return Results.InternalServerError(ex.Message);
            
        }
    }
        

    // Move the actual logic to private static methods to keep routing clean
    private async static Task<IResult> GetPatients(IDbConnection connection)
    {
        var sql =  """ 
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
           var parameter = new {PatientNo = patientNo};
    var sql = """ 
        SELECT 
        PatientId,
        PatientNo, 
        FirstName, 
        LastName, 
        Gender, 
        Contact,
        Address,
        CreatedAt,
        FROM Patients
        WHERE PatientNo = @PatientNo
        ;
        """;
    var patient = await connection.QuerySingleOrDefaultAsync<Patientresponse>(sql,parameter);

    return Results.Ok(patient);
    }

   
}