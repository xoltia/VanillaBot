using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanillaBot.Services.Database.Models
{
    public class Points
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Amount { get; set; }
    }
}
