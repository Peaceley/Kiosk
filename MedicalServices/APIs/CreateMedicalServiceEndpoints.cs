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

            //group.MapPut("/{medicalcode}", UpdateMedicalServiceById);

            // group.MapDelete("/{medicalcode}", DeleteMedicalServiceById);

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
            

            //calling the helper function to generate the MedicalServiceCode
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

    }




}