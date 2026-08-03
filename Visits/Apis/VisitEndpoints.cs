using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Kiosk.Tokens.Requests;
using Kiosk.Visits.Requests;
using Kiosk.Visits.Responses;
using Kiosk.MedicalServices.Models;
using Kiosk.Tokens.TokenHelper;
using Kiosk.Patients;
using Npgsql;
using Kiosk.Visits.Helpers;

namespace Kiosk.Visits.Apis
{
    public static class VisitEndpoints
    {
    

        public static IEndpointRouteBuilder MapvisitEndpoints(this IEndpointRouteBuilder routes)
        {
        // Group endpoints to share a common /products prefix
            var group = routes.MapGroup("/visits");

            group.MapPost("/", CreateVisit);

            group.MapGet("/", GetVisits);
            
            group.MapGet("/{VisitNo}", GetVisitByVisitNo);

            group.MapPut("/{VisitNo}", UpdateVisit);

            group.MapDelete("/{VisitNO}", DeleteVisitByVisitNo);

            return routes;
        }

        //The method to hand the visit
        public static async Task<IResult> CreateVisit(VisitsCreateRequest visit,  IDbConnection connection)
        {
            //validating Patients existence

            if (string.IsNullOrWhiteSpace(visit.PatientNo))
            {
                return Results.BadRequest("PatientNo must be provided");
            }
            
            var sql = """
            
            SELECT *
            FROM Patients
            WHERE PatientNo = @PatientNO
            """;
            //parametor to pass in into the sql statement
            var parameter = new
            {
                visit.PatientNo
            };
            //finding patients
            var patient = await connection.QuerySingleOrDefaultAsync<Patient>(sql, parameter);

            if(patient == null)
            {
                return Results.NotFound("Patients not found");
            }

            //validating the medicalserive

            if (string.IsNullOrWhiteSpace(visit.MedicalServiceCode))
            {
                return Results.BadRequest("MedicalSeriveCode must be provided");
            }

            //sql for finding the medicalservicecode

            var sql_medical = """
            SELECT 
                medicalid AS MedicalId,
                medicalservicename AS MedicalServiceName,
                medicalservicecode AS MedicalServiceCode
            FROM MedicalServices
            WHERE medicalservicecode = @MedicalServiceCode
            """;


            var medicalservice = await connection.QuerySingleOrDefaultAsync<MedicalService>(
                sql_medical,
                new
                {
                    visit.MedicalServiceCode
                });

            //validating whether the medicalService exists

            if(medicalservice == null)
            {
                return Results.NotFound("MedicalService Code Not found");

            }

            //generating the token

            var tokenNumber = await TokenGenerator.GenerateToken(connection, medicalservice.MedicalServiceCode);


            var nextVisitId = await VisitNumberGenerator.GetNextVisitId((NpgsqlConnection)connection);

            string visitNo = "VST" + nextVisitId.ToString().PadLeft(3, '0');

            //inserting into the table
            var visitId = await connection.QuerySingleAsync<int>(
            """
            INSERT INTO Visits
            (
                VisitNo,
                PatientId,
                MedicalId,
                Visitdate,
                Status
            )
            VALUES
            (
                @VisitNo,
                @PatientId,
                @MedicalId,
                @Visitdate,
                @Status
            )
            RETURNING VisitId;
            """,
            new
            {
                VisitNo = visitNo,
                PatientId = patient.PatientId,
                MedicalId = medicalservice.MedicalId,
                Visitdate = DateTime.UtcNow,
                Status = "WAITING"
            });



            //inserting  into tokon table
    
            await connection.ExecuteAsync(
            """
            INSERT INTO Tokens
            (
                VisitId,
                TokenNo
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
                Token = tokenNumber
            });

            return  Results.Created($"/visits/{visitId}", new
            {
                VisitId = visitId,
                VisitNo = visitNo,
                PatientId = patient.PatientId,
                MedicalServiceId = medicalservice.MedicalId,
                Token = tokenNumber,
                Status = "WAITING"
            });



        }
    
        //method which handles the get all visits

        public static async Task<IResult> GetVisits(IDbConnection connection)
        {
            //sql command for the inner joins
        try
        {
           const string sql = """
            SELECT 
            v.VisitId,
            v.VisitNo,
            p.PatientNo,
            p.FirstName ||' '|| p.LastName AS PatientName,
            ms.MedicalServiceName,
            ms.MedicalServiceCode,
            t.TokenNo AS Token,
            v.Status,
            v.VisitDate
            FROM Visits v
            INNER JOIN Patients p
            ON v.PatientId = p.PatientId
            INNER JOIN MedicalServices ms
            ON v.MedicalId = ms.MedicalId
            LEFT JOIN Tokens t
            ON v.VisitId = t.VisitId
            ORDER BY v.VisitDate DESC;
            """;

            //


            var visits = await connection.QueryAsync<VisitResponse>(sql);

            return Results.Ok(visits); 
        }
        catch (Exception ex)
        {
            
            return Results.Problem(ex.Message);
        }
              
        }


        //method for the GetVisitByVisitNo


        public static async Task<IResult> GetVisitByVisitNo(string visitNo, IDbConnection connection)
        {
            //validating the input of the visitNo

            if (string.IsNullOrWhiteSpace(visitNo))
            {
                return Results.BadRequest("visit number must be provided");
            }

            try{

            const string sql = """
                SELECT
                Visits.VisitId,
                Visits.VisitNo,
                Patients.PatientNo,
                Patients.FirstName || ' ' || Patients.LastName AS PatientName,
                MedicalServices.MedicalServiceName,
                MedicalServices.MedicalServiceCode,
                Tokens.TokenNo AS Token,
                Visits.Status,
                Visits.VisitDate
                FROM Visits
                INNER JOIN Patients
                    ON Visits.PatientId = Patients.PatientId
                INNER JOIN MedicalServices
                    ON Visits.MedicalId = MedicalServices.MedicalId
                LEFT JOIN Tokens
                    ON Visits.VisitId = Tokens.VisitId
                WHERE Visits.VisitNo = @VisitNo;
            
            
            """;

            var visit = await connection.QuerySingleOrDefaultAsync<VisitResponse>(sql,
            new
            {
                VisitNo = visitNo
            });


            //handling the null

            if(visit== null)
            {
                return Results.NotFound("Visit not found");
            }

            

            return Results.Ok(visit);

            }catch(Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        //updating the visit.   //update the status

        public static async Task<IResult> UpdateVisit (string visitNo, UpdateVisitRequest request, IDbConnection connection)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(request.Status))
                    {
                        return Results.BadRequest("Status must be provided");
                    }

                //checking if the visist existis

            var visitId = await connection.QuerySingleOrDefaultAsync<int?>(
                """
                SELECT VisitId
                FROM Visits
                WHERE VisitNo = @VisitNo;
                """,
                new
                {
                    VisitNo = visitNo
                });

            if(visitId == null)
                {
                    return Results.NotFound("visitId not found");
                }

            await connection.ExecuteAsync(
                """
                UPDATE Visits
                SET Status = @Status
                WHERE VisitNo = @VisitNo;
                """,
                new
                {
                    Status = request.Status,
                    VisitNo = visitNo
                });           

//return object
            return Results.Ok(new
                {
                    VisitNo = visitNo,
                    Status = request.Status
                });
            }


            catch (Exception ex)
            {
                
                return Results.Problem(ex.Message);
            }
        }

      
        //The method to hand the the DeleteVistByvisitNO
        public static async Task<IResult> DeleteVisitByVisitNo(string visitNo, IDbConnection connection)
        {
            try
            {
                //deleting token first
                if (string.IsNullOrWhiteSpace(visitNo))
                {
                    return Results.BadRequest("Visitno must be provided");

                }
                //finding the visitid

                var visitId = await connection.QuerySingleOrDefaultAsync<int?>(
                    """
                    SELECT VisitId
                    FROM Visits
                    WHERE VisitNo = @VisitNo
                    """,
                    new
                    {
                        VisitNo = visitNo
                    }

                );

                //checking if it exisits


                if(visitId == null)
                {
                    return Results.NotFound("Visit not found");
                }


            //deleting the tokne

            await connection.ExecuteAsync(
            """
            DELETE FROM Tokens
            WHERE VisitId = @VisitId;
            """,
            new
            {
                VisitId = visitId
            });


            //deleting the visit
            await connection.ExecuteAsync(
            """
            DELETE FROM Visits
            WHERE VisitId = @VisitId;
            """,
            new
            {
                VisitId = visitId
            });

            return Results.Ok(new
            {
                Message = $"Visit {visitNo} deleted successfully"
            });
    
        

            }
            catch (Exception ex)
            {
                
                return Results.Problem(ex.Message);
            }
        }



    }
}