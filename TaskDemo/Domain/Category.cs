using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Category : BaseEntity
{
    [MaxLength(128)]
    public string CategoryName { get; set; } = default!;
    public int SortValue { get; set; }
    
    public ICollection<ToDo>? ToDos { get; set; }
}