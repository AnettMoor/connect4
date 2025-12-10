using BLL;
using ConsoleUI;

namespace ConsoleApp;

public class GameController
{
    public GameBrain GameBrain { get; set; }
    public Action<GameConfiguration>? OnSaveGame { get; set; }
    public bool GameSaved {get; set;}


    public GameController()
    {
        GameBrain = new GameBrain(new GameConfiguration(), "Player 1", "Player 2");
    }

    public GameController(GameConfiguration configuration, string player1, string player2, List<List<ECellState>>? board = null)
    {
        GameBrain = new GameBrain(configuration, player1, player2, board);
    }
    
    // TODO is there a way to use it straight up without the static warnings??
    public GameConfiguration GetConfiguration()
    {
        return GameBrain.GetConfiguration();
    }
    public void UpdateConfigurationBoard()
    {
        GameBrain.UpdateConfigurationBoard();
    }


    public void GameLoop()
    {
        // if loaded game is finished
        if (GameBrain.IsGameOver())
        {
            Console.Clear();
            Ui.DrawBoard(GameBrain.GameBoard);
            Console.WriteLine("This game has already ended! No more moves allowed.");
            return;
        }

        var gameOver = false;
        do
        {
            Console.Clear();
            Ui.DrawBoard(GameBrain.GetBoard());
            Ui.ShowNextPlayer(GameBrain.IsNextPlayerX());

            Console.Write("Choice (x = quit, s = save, or column number): ");
            var input = Console.ReadLine()?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(input)) continue;

            // quit (x)
            if (input == "x")
            {
                gameOver = true;
                break;
            }
            
            //save mid game
            if (input == "s")
            {
                GameBrain.UpdateConfigurationBoard();
                var midGameConfig = GameBrain.GetConfiguration();
                OnSaveGame?.Invoke(midGameConfig);
                GameSaved = true;
                gameOver = true;
                break;
            }

            // parse move (connect 4: only x coordinate)
            if (!int.TryParse(input, out var x))
            {
                Console.WriteLine("Invalid input. Enter a number, 'x' to quit");
                Console.ReadKey();
                continue;
            }
            
            // counter starts at 0
            x -= 1;

            if (!GameBrain.BoardCoordinatesAreValid(x, 0))
            {
                Console.WriteLine("Invalid column. Try again...");
                Console.ReadKey();
                continue;
            }

            // find first available row (y)
            int y = -1;
            for (var row = GameBrain.GetBoard().Count - 1; row >= 0; row--)
            {
                if (GameBrain.GetBoard()[x][row] == ECellState.Empty)
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
            
            if (!GameBrain.BoardCoordinatesAreValid(x, y))
            {
                Console.WriteLine("Invalid move coordinates.");
                Console.ReadKey();
                continue;
            }
            GameBrain.ProcessMove(x, y);

            // check winner after move
            var winner = GameBrain.GetWinner(x, y);
            if (winner != ECellState.Empty)
            {
                Console.Clear();
                Ui.DrawBoard(GameBrain.GetBoard());
                Console.WriteLine("Winner is: " + (winner == ECellState.XWin ? "X" : "O"));
                break;
            }

        } while (!gameOver);
    }
}