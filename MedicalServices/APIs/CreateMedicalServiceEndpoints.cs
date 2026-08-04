using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Kiosk.MedicalServices.MedicalServiceHelper;
using Kiosk.MedicalServices.Models;
using Kiosk.MedicalServices.Models.Requests;
using Kiosk.MedicalServices.Models.Responses;

namespace Kiosk.MedicalServices.APIs
{
    public static class CreateMedicalServiceEndpoints
    {
        public static IEndpointRouteBuilder MapMedicalServiceEndpoints(this IEndpointRouteBuilder routes)
        {
        // Group endpoints to share a common /products prefix
            var group = routes.MapGroup("/medicalservices");

            group.MapPost("/", CreateMedicalService);

            group.MapGet("/", GetMedicalServices);
            group.MapGet("/{medicalId:int}", GetMedicalServiceById);

            group.MapPut("/{medicalId:int}", UpdateMedicalServiceById);

            group.MapDelete("/{medicalId:int}", DeleteMedicalServiceById);

            return routes;
        }

        public static async Task<IResult> CreateMedicalService(CreateMedicalService medicalservice, IDbConnection connection)
        {
            //the try block

            try
            {
              if (string.IsNullOrWhiteSpace(medicalservice.MedicalServiceName))
            {
                return Results.BadRequest("Medical Service name must be Provided");
            }
            

            //calling the helper function tKo generate the MedicalServiceCode
            string medicalServiceCode = MedicalHelper.GenerateServiceCode(medicalservice.MedicalServiceName);


        // Insert into database
            const string sql = """
                INSERT INTO MedicalServices
                (
                    MedicalServiceName,
                    MedicalServiceCode
                )
                VALUES
                (
                    @MedicalServiceName,
                    @MedicalServiceCode
                )
                RETURNING MedicalId, CreatedAt;
                """;


            var createdMedicalService = await connection.QuerySingleAsync<MedicalServiceCreateResponse>(
                sql,
                new
                {
                    MedicalServiceName = medicalservice.MedicalServiceName,
                    MedicalServiceCode = medicalServiceCode
                });
            return Results.Created($"/medicalservice/{createdMedicalService.MedicalId}", new
            {


            //the returned object in the response body
                MedicalId = createdMedicalService.MedicalId,
                MedicalServiceCode = medicalServiceCode,
                MedicalServiceName = medicalservice.MedicalServiceName,
                CreatedAt = createdMedicalService.CreatedAt
                
                
            });
            }
            catch (Exception ex)
            {
               return Results.Problem(ex.Message);
                
            }
  
        }
        public static async Task<IResult> GetMedicalServices(IDbConnection connection)
        {
            try
            {

                const string sql = """
                SELECT
                MedicalId,
                MedicalServiceName,
                MedicalServiceCode,
                CreatedAt
                FROM MedicalServices
                ORDER BY MedicalId;
                
                """;


                var medicalservices = await connection.QueryAsync<MedicalServiceResponse>(sql);

                return Results.Ok(medicalservices);
                
            }
            catch (Exception ex)
            {
                
                return Results.Problem(ex.Message);
            }
        }

        //getting medical service by medical code
        public static async Task<IResult> GetMedicalServiceById(int medicalId, IDbConnection connection)
        {

            try
            {

                const string sql = 
                """
                SELECT 
                MedicalId,
                MedicalServiceName,
                MedicalServiceCode,
                CreatedAt
                FROM MedicalServices
                WHERE MedicalId =@MedicalId
                
                """;
                

                var medicalService = await connection.QuerySingleOrDefaultAsync<MedicalServiceResponse>(
                    sql,
                    new
                    {
                        MedicalId = medicalId
                    }
                );

                if(medicalService == null)
                {
                    return Results.NotFound("MedicalService is not found");
                }

                return Results.Ok(medicalService);
            }
            catch (Exception ex)
            {
                
                return Results.Problem(ex.Message);
            }


        }
            //Updating the medical service by its id
        public static async Task<IResult> UpdateMedicalServiceById(
        int medicalId,
        UpdateMedicalServiceRequest request,
        IDbConnection connection)
        {
        try
        {
            // Validate request
            if (string.IsNullOrWhiteSpace(request.MedicalServiceName))
            {
                return Results.BadRequest("Medical Service Name is required.");
            }

            // Check if the medical service exists
            var medicalService = await connection.QuerySingleOrDefaultAsync<MedicalService>(
                """
                SELECT *
                FROM MedicalServices
                WHERE MedicalId = @MedicalId;
                """,
                new
                {
                    MedicalId = medicalId
                });

            if (medicalService is null)
            {
                return Results.NotFound("Medical Service not found.");
            }

            // Update the medical service name only
            await connection.ExecuteAsync(
                """
                UPDATE MedicalServices
                SET MedicalServiceName = @MedicalServiceName
                WHERE MedicalId = @MedicalId;
                """,
                new
                {
                    MedicalId = medicalId,
                    MedicalServiceName = request.MedicalServiceName
                });

            // Retrieve the updated record
            var updatedMedicalService = await connection.QuerySingleAsync<MedicalService>(
                """
                SELECT *
                FROM MedicalServices
                WHERE MedicalId = @MedicalId;
                """,
                new
                {
                    MedicalId = medicalId
                });

            return Results.Ok(updatedMedicalService);
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        
    public static async Task<IResult> DeleteMedicalServiceById(
    int medicalId,
    IDbConnection connection)
        {
            try
            {
                // Check if the medical service exists
                var medicalService = await connection.QuerySingleOrDefaultAsync<MedicalService>(
                    """
                    SELECT *
                    FROM MedicalServices
                    WHERE MedicalId = @MedicalId;
                    """,
                    new
                    {
                        MedicalId = medicalId
                    });

                if (medicalService is null)
                {
                    return Results.NotFound("Medical Service not found.");
                }

                // Delete the medical service
                await connection.ExecuteAsync(
                    """
                    DELETE FROM MedicalServices
                    WHERE MedicalId = @MedicalId;
                    """,
                    new
                    {
                        MedicalId = medicalId
                    });

                return Results.Ok(new
                {
                    Message = "Medical Service deleted successfully.",
                    DeletedMedicalService = medicalService
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

    }




}