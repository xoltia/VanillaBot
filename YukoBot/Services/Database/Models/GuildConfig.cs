using System.ComponentModel.DataAnnotations;

namespace YukoBot.Services.Database.Models
{
    public class GuildConfig
    {
        [Key]
        public string GuildId { get; set; }
        public string AutoRoleId { get; set; } = null;
        public string Prefix { get; set; } = null;
    }
}
