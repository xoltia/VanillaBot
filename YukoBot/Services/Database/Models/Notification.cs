namespace YukoBot.Services.Database.Models
{
    public class Notification
    {
        // Because EF wants it
        public int Id { get; set; }
        public string ReceiverId { get; set; }
        public string OptedId { get; set; }
        public string GuildId { get; set; }
        public bool Enabled { get; set; } = true;
    }
}
