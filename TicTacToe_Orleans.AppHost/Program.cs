var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("postgresdb")
                .AddDatabase("tictactoedb");
var redis = builder.AddRedis("redis");
var orleans = builder.AddOrleans("orleans-cluster")
    .WithClustering(redis);

builder.AddProject<Projects.DataBaseMigrator>("databasemigrator")
    .WithReference(db);

var backend = builder.AddProject<Projects.TicTacToe_Orleans>("tictactoe-orleans")
    .WithReference(redis)
    .WithReference(db)
    .WithReference(orleans)
    .WithReplicas(3)
    .WithEndpoint(name: "ORLEANS-SILO-DASHBOARD", port: 977, scheme: "http", env: "ORLEANS-SILO-DASHBOARD");


var frontend = builder.AddNpmApp("webclient", "../webapp", "dev")
    .WithHttpEndpoint(env: "PORT", port: 3000)
    .WithExternalHttpEndpoints()
    .WithReference(backend);

backend.WithReference(frontend);


builder.Build().Run();
