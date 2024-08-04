
namespace TicTacToe_Orleans_.Grains
{
    public interface IConnectionGrain : IGrainWithStringKey
    {
        Task AddUserAsync(string? userId, string connectionId);
        Task RemoveUserAsync(string? userId, string connectionId);
        Task<bool> IsConnectedAsync(string userId);
    }
}