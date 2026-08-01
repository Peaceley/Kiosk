using System.Data;
using Npgsql;
using Dapper;
using Kiosk.Visits.APIs;

var builder =
WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
builder.Services.AddTransient<IDbConnection>(sp =>
{

    return new NpgsqlConnection(
        "Host=localhost;Database=kiosk;Username=postgres;Password=authentiano"
    );
});
var app =
builder.Build();
app.MapGet("/", ()=>"Hospital Kiosk API Running");
app.MapVisitEndpoints();
app.Run();