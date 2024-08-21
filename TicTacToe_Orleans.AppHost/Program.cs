using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("postgresdb")
                .AddDatabase("tictactoe");
var redis = builder.AddRedis("redis");

var backend = builder.AddProject<Projects.TicTacToe_Orleans>("tictactoe-orleans")
    .WithReference(redis)
    .WithReference(db)
    .WithReplicas(3)
    .WithEndpoint(name: "ORLEANS-SILO-PORT", port: 677, scheme: "http", env: "ORLEANS-SILO-PORT")
    .WithEndpoint(name: "ORLEANS-GATEWAY-PORT", port: 877, scheme: "http", env: "ORLEANS-GATEWAY-PORT")
    .WithEndpoint(name: "ORLEANS-SILO-DASHBOARD", port: 977, scheme: "http", env: "ORLEANS-SILO-DASHBOARD");
   

var frontend = builder.AddNpmApp("webclient", "../webapp", "dev")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .WithReference(backend);

backend.WithReference(frontend);

builder.Build().Run();
