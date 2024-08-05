using TicTacToe_Orleans.Model;

namespace TicTacToe_Orleans.Grains
{
    public class ConnectionGrain : IConnectionGrain
    {
        public Dictionary<string, string> AuthenticatedUsers { get; set; } = [];
        public HashSet<string> AnonymousUsers { get; set; } = [];
       
        public Task AddUserAsync(string? userId, string connectionId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                AnonymousUsers.Add(connectionId);
            }
            else
            {
                AuthenticatedUsers.Add(userId, connectionId);
            }
            return Task.CompletedTask;
        }

        public Task<bool> IsConnectedAsync(string userId)
        {
            return Task.FromResult(AuthenticatedUsers.ContainsKey(userId));
        }

        public Task RemoveUserAsync(string? userId, string connectionId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                AnonymousUsers.Remove(connectionId);
            }
            else
            {
                AuthenticatedUsers.Remove(userId);
            }
            return Task.CompletedTask;
        }
    }
}
