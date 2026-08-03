using System.Data;
using Kiosk.MedicalServices.APIs;
using Kiosk.Visits.Apis;
using Npgsql;

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true; 

string connectionString = "Host=localhost;Port=5432;Database=kioskvisit;Username=postgres;Password=authentiano;Include Error Detail=true;";

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddTransient<IDbConnection>((sp) => new NpgsqlConnection(connectionString));

var app = builder.Build();

app.MapGet("/", () => "App is running!");

app.MapvisitEndpoints();
app.MapMedicalServiceEndpoints();

//app.MapPatientEndPoints();
// app.MapTokenEndpoints();//

app.Run();
