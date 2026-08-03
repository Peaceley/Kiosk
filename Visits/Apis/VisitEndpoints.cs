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

            // group.MapGet("/", GetVisits);
            // group.MapGet("/{VisitNO}", GetVisitByVisitNo);

            // group.MapDelete("/{VisitNO}", DeleteVisitByVisitNO);

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
                medicalid AS MedicalServiceId,
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
                MedicalId = medicalservice.MedicalServiceId,
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
                MedicalServiceId = medicalservice.MedicalServiceId,
                Token = tokenNumber,
                Status = "WAITING"
            });



        }
        
 


        //method which handles the get all visits

        // public static Task<IResult> GetVisits()

        //The method to hand the Get visit by VisitNo

        // public static Task<IResult> GetVisitByVisitNo()

        //The method to hand the the DeleteVistByvisitNO
        // public static Task<IResult> DeleteVisitByVisitNO()



        //the method for the endpoint and for the crud




    }
}