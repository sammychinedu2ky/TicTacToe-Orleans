var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

builder.AddProject<Projects.TicTacToe_Orleans>("tictactoe-orleans").WithReference(redis);

builder.Build().Run();
