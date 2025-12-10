using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace BLL;

public class GameConfiguration : BaseEntity
{
    public string Name { get; set; } = "";
    public int BoardWidth { get; set; } = 5;
    public int BoardHeight { get; set; } = 5;
    public int WinCondition { get; set; } = 4; // connect 4

    public string CreatedAt { get; set; } = DateTime.Now.ToString("HH_mm_ddMMyyyy");
    
    
    // functions for save
    [NotMapped] // do not create a database column, im storing a serialized board in database, not this one
    public List<List<ECellState>>? Board { get; set; } // jagged array, list of lists
    
    // serialized board for databases saving
    public string? BoardData
    {
        get => Board == null ? null : JsonSerializer.Serialize(Board);
        set => Board = value == null ? null : JsonSerializer.Deserialize<List<List<ECellState>>>(value);
    }
}
    