using TicTacToe_Orleans.Model;

namespace TicTacToe_Orleans.Grains
{
    public class ConnectionGrain : IConnectionGrain
    {
        private Dictionary<string, string> _authenticatedUsers { get; set; } = [];
        private HashSet<string> _anonymousUsers { get; set; } = [];
       
        public Task AddUserAsync(string? userId, string connectionId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                _anonymousUsers.Add(connectionId);
            }
            else
            {
                _authenticatedUsers.Add(userId, connectionId);
            }
            return Task.CompletedTask;
        }

        public Task<bool> IsConnectedAsync(string userId)
        {
            return Task.FromResult(_authenticatedUsers.ContainsKey(userId));
        }

        public Task RemoveUserAsync(string? userId, string connectionId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                _anonymousUsers.Remove(connectionId);
            }
            else
            {
                _authenticatedUsers.Remove(userId);
            }
            return Task.CompletedTask;
        }
    }
}
