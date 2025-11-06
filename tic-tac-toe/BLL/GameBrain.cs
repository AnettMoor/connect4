﻿﻿namespace BLL;

public class GameBrain
{
    private ECellState[,] GameBoard { get; set; }
    private GameConfiguration GameConfiguration { get; set; }
    private string Player1Name { get; set; }
    private string Player2Name { get; set; }

    private bool NextMoveByX { get; set; } = true;
    
    
    // for load
    public bool IsGameOver()
    {
        for (int x = 0; x < GameConfiguration.BoardWidth; x++)
        for (int y = 0; y < GameConfiguration.BoardHeight; y++)
            if (GetWinner(x, y) != ECellState.Empty)
                return true;
        return false;
    }

    public List<List<ECellState>> GetBoardAsList()
    {
        var boardList = new List<List<ECellState>>();
        for (int x = 0; x < GameConfiguration.BoardWidth; x++)
        {
            var col = new List<ECellState>();
            for (int y = 0; y < GameConfiguration.BoardHeight; y++)
            {
                col.Add(GameBoard[x, y]);
            }
            boardList.Add(col);
        }
        return boardList;
    }

    
    public GameBrain(GameConfiguration configuration, string player1Name, string player2Name, ECellState[,]? existingBoard = null)
    {
        GameConfiguration = configuration;
        Player1Name = player1Name;
        Player2Name = player2Name;

        
        // for loading existing configs
        if (existingBoard != null)
        {
            GameBoard = existingBoard;
        }
        else if (configuration.Board != null)
        {
            GameBoard = new ECellState[configuration.BoardWidth, configuration.BoardHeight];
            for (int x = 0; x < configuration.BoardWidth; x++)
            {
                for (int y = 0; y < configuration.BoardHeight; y++)
                {
                    GameBoard[x, y] = configuration.Board[x][y];
                }
            }
        }
        // create new board
        else
        {
            GameBoard = new ECellState[configuration.BoardWidth, configuration.BoardHeight];
        }
    }

    public ECellState[,] GetBoard()
    {
        var gameBoardCopy = new ECellState[GameConfiguration.BoardWidth, GameConfiguration.BoardHeight];
        Array.Copy(GameBoard, gameBoardCopy, GameBoard.Length);
        return gameBoardCopy;
    }

    public bool IsNextPlayerX() => NextMoveByX;


    public void ProcessMove(int x, int y)
    {
        if (GameBoard[x, y] == ECellState.Empty)
        {
            GameBoard[x, y] = NextMoveByX ? ECellState.X : ECellState.O; // place the correct symbol
            NextMoveByX = !NextMoveByX; // switch turns
        }
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
        if (GameBoard[x, y] == ECellState.Empty) return ECellState.Empty; // if cell is empty, there cannot be a winner 
        
        // go through all directions (horizontal, vertical, diagonal)
        for (int directionIndex = 0; directionIndex < 4; directionIndex++) 
        {
            var (dirX, dirY) = GetDirection(directionIndex);

            var count = 0;
            var nextX = x;
            var nextY = y;
            
            // check one direction forward
            while (BoardCoordinatesAreValid(nextX, nextY) &&
                   GameBoard[x, y] == GameBoard[nextX, nextY] &&
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
                       GameBoard[x, y] == GameBoard[nextX, nextY] &&
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
                return GameBoard[x, y] == ECellState.X ? ECellState.XWin : ECellState.OWin;
            }
        }
        return ECellState.Empty;
    }
}