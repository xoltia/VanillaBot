using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanillaBot.Services.Database.Models
{
    public class GameNotification
    {
        public int Id { get; set; }
        public string ReceiverId { get; set; }
        public string Game { get; set; }
    }
}
