namespace YukoBot.Services.Database.Models
{
    public class GameNotification
    {
        public int Id { get; set; }
        public string ReceiverId { get; set; }
        public string Game { get; set; }
    }
}
