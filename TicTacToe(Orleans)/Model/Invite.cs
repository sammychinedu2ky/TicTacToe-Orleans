using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace TicTacToe_Orleans.Model
{
    public class Invite
    {
        public Guid Id { get; set; }
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public Guid GameRoom { get; set; }
        public bool NewInvite { get; set; }
        public bool Accept { get; set; }
    }
}
