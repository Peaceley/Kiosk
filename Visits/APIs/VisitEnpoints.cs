using Dapper;
using System.Data;
using Kiosk.Visits.Models.Requests;
using Kiosk.Visits.Models.Responses;
using Kiosk.MedicalServices.Models.Requests;
using Kiosk.Helpers;

namespace Kiosk.Visits.APIs;
public static class VisitEndpoints
{
    public static IEndpointRouteBuilder MapVisitEndpoints(
        this IEndpointRouteBuilder routes)
    {
        var group =
        routes.MapGroup("/visits");
        group.MapPost("/", CreateVisit);
        group.MapGet("/", GetVisits);
        return routes;

    }

    private static async Task<IResult> CreateVisit(
        VisitCreateRequest request,
        IDbConnection connection)
    {
        // Find medical service

        var service = 
        await connection.QuerySingleAsync<MedicalService>(
        """
        SELECT
        id,
        service_name,
        prefix
        FROM medical_services
        WHERE id=@Id;
        """,
        new
        {
            Id=request.MedicalServiceId
        });

        // Create Visit

        var visitId =
        await connection.ExecuteScalarAsync<int>
        (
        """
        INSERT INTO visits
        (
        patient_id,
        medical_service_id
        )
        VALUES
        (
        @PatientId,
        @MedicalServiceId
        )
        RETURNING id;
        """,
        request);

        // Increase token number
        var number =
        await connection.ExecuteScalarAsync<int>
        (
        """
        UPDATE token_sequences
        SET last_number =
        last_number + 1
        WHERE prefix=@Prefix
        RETURNING last_number;
        """,
        new
        {
            Prefix = service.Prefix
        });

        // Generate token
        var token = TokenGeneratorHelper.Generate(
            service.Prefix,
            number
        );

        // Save token

       int  affectedRows= await connection.ExecuteAsync
        (
        """
        INSERT INTO tokens
        (
        visit_id,
        token_number
        )
        VALUES
        (
        @VisitId,
        @Token
        );
        """,
        new
        {
            VisitId = visitId,
            Token = token
        });

        // Return response
        return Results.Created(
            "/visits",
            new VisitsResponses
            {
                Id = visitId,
                Token = token
                
            });

    }

    private static async Task<IResult> GetVisits(
        IDbConnection connection)
    {
        var sql =
        """

        SELECT
        id,
        patient_id,
        medical_service_id,
        status
        FROM visits;
        """;
        var visits =
        await connection.QueryAsync<VisitsResponses>(sql);
        return Results.Ok(visits);

    }

}
