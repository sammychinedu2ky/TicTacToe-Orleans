using System.ComponentModel.DataAnnotations;

namespace TicTacToe_Orleans.Model
{
    public class User
    {
        [EmailAddress]
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}

