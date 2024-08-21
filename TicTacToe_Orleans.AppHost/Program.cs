using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("postgresdb")
                .AddDatabase("tictactoe");
var redis = builder.AddRedis("redis");
var backend = builder.AddProject<Projects.TicTacToe_Orleans>("tictactoe-orleans")
    .WithReference(redis)
    .WithReference(db);

var frontend = builder.AddNpmApp("webclient", "../webapp", "dev")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .WithReference(backend);

backend.WithReference(frontend);

builder.Build().Run();
