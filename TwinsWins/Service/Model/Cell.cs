namespace TwinsWins.Service.Model
{
    public class Cell
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public bool IsMatched { get; set; }
        public bool IsClicked { get; set; }
    }
}
