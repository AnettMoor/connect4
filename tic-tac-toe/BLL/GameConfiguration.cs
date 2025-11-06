using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BLL;

public class GameConfiguration : BaseEntity
{
    public string Name { get; set; } = "Classical 6x6";
    public int BoardWidth { get; set; } = 5;
    public int BoardHeight { get; set; } = 5;
    public int WinCondition { get; set; } = 4; // connect 4

    public string CreatedAt { get; set; } = DateTime.Now.ToString("HH_mm_ddMMyyyy");


    // save functions
    public List<List<ECellState>>? Board { get; set; }

    // Convert array to list for saving
    public static List<List<ECellState>> ArrayToList(ECellState[,] board)
    {
        var list = new List<List<ECellState>>();
        for (int x = 0; x < board.GetLength(0); x++)
        {
            var col = new List<ECellState>();
            for (int y = 0; y < board.GetLength(1); y++)
                col.Add(board[x, y]);
            list.Add(col);
        }
        return list;
    }

    // Convert list to array for loading
    public static ECellState[,] ListToArray(List<List<ECellState>> boardList, int width, int height)
    {
        var array = new ECellState[width, height];
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
            array[x, y] = boardList[x][y];
        return array;
    }
}
    