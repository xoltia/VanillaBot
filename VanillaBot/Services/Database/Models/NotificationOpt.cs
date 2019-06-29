using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanillaBot.Services.Database.Models
{
    public class NotificationOpt
    {
        // Because EF wants it
        public int Id { get; set; }
        public string ReceiverId { get; set; }
        public string OptedId { get; set; }
        public string GuildId { get; set; }
        public bool Enabled { get; set; } = true;
    }
}
