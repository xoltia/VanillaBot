using System.ComponentModel.DataAnnotations;

namespace YukoBot.Services.Database.Models
{
    public class Points
    {
        [Key]
        public string UserId { get; set; }
        public int Amount { get; set; }
    }
}
