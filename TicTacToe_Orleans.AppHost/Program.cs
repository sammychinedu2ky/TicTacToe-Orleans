var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TicTacToe_Orleans>("tictactoe-orleans");

builder.Build().Run();
