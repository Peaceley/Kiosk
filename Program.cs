//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();

using System.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true; 

string connectionString = "Host=PG6001.site4now.net:6432;Database=db_a4be89_queue;Username=a4be89_queue;Password=12345_CIL;Include Error Detail=true;";

//Add datbase server
builder.Services.AddTransient<IDbConnection>((sp) => new NpgsqlConnection(connectionString));

var app = builder.Build();

app.Map("/" , () => "Patient Api is running");
app.MapPatientEndpoints();
// app.MapVisitEndpoints();//
// app.MapTokenEndpoints();//



app.Run();
