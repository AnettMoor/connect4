namespace BLL;

public class GameConfiguration
{
    public string Name { get; set; } = "Classical 6x6";
    public int BoardWidth { get; set; } = 6;
    public int BoardHeight { get; set; } = 6;
    public int WinCondition { get; set; } = 4; // connect 4

}