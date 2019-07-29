using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
