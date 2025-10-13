using BLL;

namespace ConsoleUI;

public static class Ui
{

    public static void ShowNextPlayer(bool isNextPlayerX)
    {
        Console.WriteLine("Next Player: " + (isNextPlayerX ? "X" : "O"));
    }

    public static void DrawBoard(ECellState[,] gameBoard)
    {
        
        Console.Write("    ");
        Console.Write("   "); //cylindrical
        for (int x = 0; x < gameBoard.GetLength(0); x++)
        {
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(GetNumberRepresentation(x + 1));
            Console.ResetColor();
        }

        Console.Write("|   "); //cylindrical
        Console.WriteLine();

        // write cells
        for (int y = 0; y < gameBoard.GetLength(1); y++)
        {
            // main board with cylindrical padding left
            for (int x = 0; x < gameBoard.GetLength(0) + 1; x++)
            {
                // cylindrical cells left
                if (x == 1)
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write("---");
                    Console.ResetColor();
                    Console.Write("+");
                    Console.Write("---+");
                }
                else
                {
                    Console.Write("---+");
                }
            }
            
            // cylindrical padding right
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("---");
            Console.ResetColor();
            
            // write column numbers
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(GetNumberRepresentation(y + 1));
            Console.ResetColor();
            
            // mark cylindrical horizontal
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write("|" + GetCellRepresentation(gameBoard[gameBoard.GetLength(0) - 1, y]));
            Console.ResetColor();
            
            // mark x o main NORMAL CELLS
            for (int x = 0; x < gameBoard.GetLength(0); x++)
            {
                Console.Write("|" + GetCellRepresentation(gameBoard[x, y]));
            }
            
            // cylindrical cells input
            Console.Write("|");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(GetCellRepresentation(gameBoard[0, y]));
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