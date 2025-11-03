using BLL;
using ConsoleUI;

namespace ConsoleApp;

public class GameController
{
    private GameBrain GameBrain { get; set; }

    public GameController()
    {
        GameBrain = new GameBrain(new GameConfiguration(), "Player 1", "Player 2");
    }

    public void GameLoop()
    {
        // Game loop logic here

        // get the player move
        // update gamebrain state
        // draw out the ui
        // when game over, stop

        var gameOver = false;
        do
        {
            Console.Clear();

            // draw the board
            Ui.DrawBoard(GameBrain.GetBoard());
            Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());

            Console.Write("Choice (x):");
            var input = Console.ReadLine();
            if (input?.ToLower() == "x")
            {
                gameOver = true;
            }

            if (input == null) continue;
            var parts = input.Split(",");

            //input format checks
            if (!int.TryParse(parts[0], out var x) ||
                x < 1 ||
                x > GameBrain.GetBoard().GetLength(0))
            {
                Console.WriteLine("Invalid input. Try again...");
                Console.ReadKey();
                continue;
            }

            // connect4 - drop the move in first free space in column
            int y = -1;
            for (var row = GameBrain.GetBoard().GetLength(1) - 1; row >= 0; row--)
            {
                if (GameBrain.GetBoard()[x-1, row] == ECellState.Empty)
                {
                    y = row;
                    break;
                }
            }
            if (y == -1)
            {
                Console.WriteLine("Column full, try again...");
                Console.ReadKey();
                continue;
            }

            
            GameBrain.ProcessMove(x - 1, y);

            // display final board + winner
            var winner = GameBrain.GetWinner(x - 1, y);
            if (winner != ECellState.Empty)
            {
                // TODO: move to ui (???)
                Ui.DrawBoard(GameBrain.GetBoard()); // final board
                Console.WriteLine("Winner is: " + (winner == ECellState.XWin ? "X" : "O")); // winner
                break;
            }
        } while (gameOver == false);
    }
}