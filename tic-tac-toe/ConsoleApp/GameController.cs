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

            Console.Write("Choice (x,y):");
            var input = Console.ReadLine();
            if (input?.ToLower() == "x")
            {
                gameOver = true;
            }

            try
            {
                if (input == null) continue;
                var parts = input.Split(",");
                if (parts.Length == 2)
                {
                    if (int.TryParse(parts[0], out var x) && int.TryParse(parts[1], out var y))
                    {
                        GameBrain.ProcessMove(x - 1, y - 1);

                        // display final board + winner
                        var winner = GameBrain.GetWinner(x - 1, y - 1);
                        if (winner != ECellState.Empty)
                        {
                            // TODO: move to ui (???)
                            Ui.DrawBoard(GameBrain.GetBoard()); // final board
                            Console.WriteLine("Winner is: " + (winner == ECellState.XWin ? "X" : "O")); // winner
                            break;
                        }
                    }
                    else
                    {
                        throw new FormatException("Input must be in format x,y");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Press any key to try again...");
                Console.ReadKey();
            }
        } while (gameOver == false);
    }
}