using BLL;

namespace ConsoleUI;

public static class Ui
{

    public static void ShowNextPlayer(bool isNextPlayerX)
    {
        Console.WriteLine("Next Player: " + (isNextPlayerX ? "X" : "O"));
    }

    public static void DrawBoard(List<List<ECellState>> gameBoard)
    {
        // write column numbers
        Console.Write("    ");
        Console.Write("   "); //cylindrical
        for (int x = 0; x < gameBoard.Count; x++)
        {
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write(GetNumberRepresentation(x + 1));
            Console.ResetColor();
        }
        Console.Write("|   "); //cylindrical
        Console.WriteLine();

        
        // write cells
        for (int y = 0; y < gameBoard.Count; y++)
        {
            // CELL LINES/OUTSIDES
            // number column lines
            Console.Write("---+");
            
            // left cylinder cells lines
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("---");
            Console.ResetColor();
            Console.Write("+");
            
            // main board lines
            for (int x = 1; x < gameBoard.Count + 1; x++)
            {
                Console.Write("---+");
            }

            // cylindrical cells right
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("---");
            Console.ResetColor();

            
            // CELL INSIDES 
            // write row numbers
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write(GetNumberRepresentation(y + 1));
            Console.ResetColor();
            
            // left cylinder cells
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(GetCellRepresentation(gameBoard[gameBoard.Count - 1] [y]));
            Console.ResetColor();
            
            // mark x o main
            for (int x = 0; x < gameBoard.Count; x++)
                if (x == gameBoard.Count - 1)
                {
                    Console.Write("|" + GetCellRepresentation(gameBoard[x][y]) + "|");
                }
                else Console.Write("|" + GetCellRepresentation(gameBoard[x][y]));

            // right cylinder cells
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(GetCellRepresentation(gameBoard[0][y]));;
            Console.ResetColor();
            
            Console.WriteLine();
        }
    }
    
    private static string GetNumberRepresentation(int number)
    {
        return " " + (number < 10 ? "0" + number : number.ToString());
    }

    private static string GetCellRepresentation(ECellState cellValue) =>
        cellValue switch
        {
            ECellState.Empty => "   ",
            ECellState.X => " X ",
            ECellState.O => " O ",
            ECellState.XWin => "XXX",
            ECellState.OWin => "OOO",
            _ => " ? "
        };
}
