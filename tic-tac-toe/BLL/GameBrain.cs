namespace BLL;

public class GameBrain
{
    public List<List<ECellState>> GameBoard { get; set; }
    private GameConfiguration GameConfiguration { get; set; }
    public string Player1Name { get; set; }
    public string Player2Name { get; set; }

    public bool NextMoveByX { get; set; } = true;
    
    
    // for load
    public bool IsGameOver()
    {
        for (int x = 0; x < GameConfiguration.BoardWidth; x++)
        for (int y = 0; y < GameConfiguration.BoardHeight; y++)
            if (GetWinner(x, y) != ECellState.Empty)
                return true;
        return false;
    }
    
    
    public GameBrain(GameConfiguration configuration, string player1Name, string player2Name, List<List<ECellState>>? existingBoard = null)
    {
        GameConfiguration = configuration;
        Player1Name = player1Name;
        Player2Name = player2Name;
        
        // for loading existing configs
        if (existingBoard != null && 
            existingBoard.Count == configuration.BoardWidth &&
            existingBoard.All(col => col.Count == configuration.BoardHeight))
        {
            GameBoard = existingBoard
                .Select(col => new List<ECellState>(col))
                .ToList();
        }
        // create empty board
        else
        {
            GameBoard = new List<List<ECellState>>();
            for (int x = 0; x < configuration.BoardWidth; x++)
            {
                var col = new List<ECellState>();
                for (int y = 0; y < configuration.BoardHeight; y++)
                    col.Add(ECellState.Empty);
                GameBoard.Add(col);
            }
        }
    }

    public List<List<ECellState>> GetBoard() => GameBoard;

    public bool IsNextPlayerX() => NextMoveByX;


    public void ProcessMove(int x, int y)
    {
        if (GameBoard[x][y] == ECellState.Empty)
        {
            GameBoard[x][y] = NextMoveByX ? ECellState.X : ECellState.O; // place the correct symbol
            NextMoveByX = !NextMoveByX; // switch turns
        }
    }
    
    // see if move is possible
    public MoveResult TryMakeMove(int x)
    {
        var result = new MoveResult();
        
        // check if move is in column bounds
        if (x < 0 || x >= GameConfiguration.BoardWidth)
        {
            result.Success = false;
            result.ErrorMessage = "Invalid column.";
            return result;
        }

        // find the first empty cell in row
        int y = -1;
        for (int row = GameConfiguration.BoardHeight - 1; row >= 0; row--)
        {
            if (GameBoard[x][row] == ECellState.Empty)
            {
                y = row;
                break;
            }
        }

        if (y == -1)
        {
            result.Success = false;
            result.ErrorMessage = "Column full.";
            return result;
        }
        
        ProcessMove(x, y);
        result.Success = true;
        result.X = x;
        result.Y = y;
        result.Winner = GetWinner(x, y);

        return result;
    }



    private (int dirX, int dirY) GetDirection(int directionIndex) =>
        directionIndex switch
        {
            0 => (-1, -1), // Diagonal up-left
            1 => (0, -1), // Vertical
            2 => (1, -1), // Diagonal up-right
            3 => (1, 0), // horizontal
            _ => (0, 0)
        };

    private (int dirX, int dirY) FlipDirection((int dirX, int dirY) direction) =>
        (-direction.dirX, -direction.dirY);

    public bool BoardCoordinatesAreValid(int x, int y)
    {
        return x >= 0 && x < (GameConfiguration.BoardWidth) && 
               y >= 0 && y < (GameConfiguration.BoardHeight);
    }
    
    // Cylindrical wrapping
    private (int, int) Wrapping(int nextX, int nextY, int dirX, int dirY)
    {
        // horizontal wrap for cylinder
        if (nextX < 0) nextX = GameConfiguration.BoardWidth - 1;
        else if (nextX >= GameConfiguration.BoardWidth) nextX = 0;
                
        // diagonal wrap for cylinder
        if (dirX != 0 && dirY != 0) // check for diagonal = both x AND Y change 
        {
            if (nextY < 0) nextY = GameConfiguration.BoardHeight - 1; // left up - right down
            else if (nextY >= GameConfiguration.BoardHeight) nextY = 0; // left down - right up
        }
        return (nextX, nextY);
    }

    public ECellState GetWinner(int x, int y)
    {
        if (GameBoard[x][y] == ECellState.Empty) return ECellState.Empty; // if cell is empty, there cannot be a winner 
        
        // go through all directions (horizontal, vertical, diagonal)
        for (int directionIndex = 0; directionIndex < 4; directionIndex++) 
        {
            var (dirX, dirY) = GetDirection(directionIndex);

            var count = 0;
            var nextX = x;
            var nextY = y;
            
            // check one direction forward
            while (BoardCoordinatesAreValid(nextX, nextY) &&
                   GameBoard[x][y] == GameBoard[nextX][nextY] &&
                   count <= GameConfiguration.WinCondition)
            {
                count++;
                nextX += dirX;
                nextY += dirY;
                
                (nextX, nextY) = Wrapping(nextX, nextY, dirX, dirY);
            }


            // if we didn't find enough squares -> flip direction
            if (count < GameConfiguration.WinCondition)
            {
                (dirX, dirY) = FlipDirection((dirX, dirY));
                nextX = x + dirX;
                nextY = y + dirY;

                (nextX, nextY) = Wrapping(nextX, nextY, dirX, dirY);

                while (BoardCoordinatesAreValid(nextX, nextY) &&
                       GameBoard[x][y] == GameBoard[nextX][nextY] &&
                       count <= GameConfiguration.WinCondition)
                {
                    count++;
                    nextX += dirX;
                    nextY += dirY;
                    
                    (nextX, nextY) = Wrapping(nextX, nextY, dirX, dirY);
                }
            }
            if (count == GameConfiguration.WinCondition)
            {
                return GameBoard[x][y] == ECellState.X ? ECellState.XWin : ECellState.OWin;
            }
        }
        return ECellState.Empty;
    }

    public GameConfiguration GetConfiguration()
    {
        return GameConfiguration;
    }
    
    // for saving
    public void UpdateConfigurationBoard()
    {
        var boardList = new List<List<ECellState>>();
        for (int x = 0; x < GameConfiguration.BoardWidth; x++)
        {
            var col = new List<ECellState>();
            for (int y = 0; y < GameConfiguration.BoardHeight; y++)
            {
                col.Add(GameBoard[x][y]);
            }
            boardList.Add(col);
        }
        GameConfiguration.Board = boardList;
    }
}