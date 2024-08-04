using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace TicTacToe_Orleans_.Model
{
    public class User
    {
        [EmailAddress]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}

