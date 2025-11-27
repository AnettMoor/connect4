namespace BLL;

public class MoveResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = "";
    public int X { get; set; }
    public int Y { get; set; }
    public ECellState Winner { get; set; } = ECellState.Empty;
}
