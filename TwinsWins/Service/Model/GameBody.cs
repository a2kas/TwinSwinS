namespace TwinsWins.Service.Model
{
    public class GameBody
    {
        public List<Cell> Cells { get; set; }
        public Dictionary<int, int> ImageIdMap { get; set; }
    }
}
