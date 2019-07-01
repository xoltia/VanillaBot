using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanillaBot.Services.Database.Models
{
    public class Points
    {
        [Key]
        public string UserId { get; set; }
        public int Amount { get; set; }
    }
}
