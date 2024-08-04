using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace TicTacToe_Orleans_.Model
{
    public class GameRoom
    {
        public Guid Id { get; set; }
        public string X { get; set; } = string.Empty;
        public string O { get; set; } = string.Empty;
        public string Winner { get; set; } = string.Empty;
        
        [Column("Board", TypeName = "json")]
       
        public List<List<char>> Board { get; set; } = [];
        public List<string> Moves { get; set; } = [];
        public GameRoomType Type { get; set; }
    }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum GameRoomType
    {
        Computer,
        User
    }
}
