using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using System.IO.Pipelines;


namespace Kiosk.Visits.APIs
{
    public  static class VisitEnpoints
    {
        public static IEndpointRouteBuilder MapvisitEndPoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/visits");
            group.MapPost("/", CreateVisit);
            group.MapGet("/", GetVisit);

            return routes;


        }

        //creating the method for the Create  Visist

        public static async Task<IResult> CreateVisit(VisitEnpoints request, IDbConnection connection)
        {
            //get service prefix

            var serviceSql = """"
            SELECT
            id,
            service_name,
            prefix 
            FROM Medical_services
            WHERE id=@Id
            """";

            var service = await connection.QuerySingleAsync<MedicaService>(
                serviceSql,
                new
                {
                    Id = request.MedicalServiceId
                }
            );


            //creating the visit

            var visitSql = """"
            INSERT INTO Visits
            (patient_id),
            (medical_service_id)
            VALUES
            (@PatientId,
            @medicalServiceId)

            returning Id
            """";

            var VisitId = await connection.ExecuteScalarAsync<int>(visitSql, request);
            //generating token

            var tokenNumber = await connection.ExecuteScalarAsync<int>(
                """"
                    UPDATE token_sequence
                    SET last_number = last_number + 1
                    WHERE prefix =@Prefix
                    Returning last_number;
                """",
                new
                {
                    VisitId - visitId,
                    Token = Token
                }


            );


        }

        //creating the Visit
        public static async Task<IResult> GetVisits(IDbConnection connection)
        {

            //geting Medicalservices

           
            var getVisitSql = 
            """"
            SELECT * FROM Vists;
            """";
        }
    }
    
}