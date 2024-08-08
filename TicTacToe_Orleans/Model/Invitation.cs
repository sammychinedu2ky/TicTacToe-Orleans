using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace TicTacToe_Orleans.Model
{
    public class Invitation
    {
        public Guid Id { get; set; }
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public Guid GameRoom { get; set; }
        public bool NewInvite { get; set; }
        public bool Accept { get; set; }


    }

    public class InvitationDTO
    {
        public Guid Id { get; set; }
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public Guid GameRoom { get; set; }
    }

    public static class InvitationExtensions
    {
        public static List<InvitationDTO> ToDTO(this List<Invitation> invitations)
        {
            if(invitations is null || invitations.Count == 0)
            {
                return new List<InvitationDTO>();
            }
            return invitations.Select(i => new InvitationDTO
            {
                Id = i.Id,
                From = i.From,
                To = i.To,
                GameRoom = i.GameRoom
            }).ToList();
        }
        public static InvitationDTO ToDTO(this Invitation invitation)
        {
            return new InvitationDTO
            {
                Id = invitation.Id,
                From = invitation.From,
                To = invitation.To,
                GameRoom = invitation.GameRoom
            };
        }
    }
}