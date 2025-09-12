namespace MenuSystem;

public class MenuItem
{
    public string Label { get; set; }
    public string Key { get; set; }
    
    // thing that should happen...
    
    
    public override string ToString()
    {
        return $"{Key}) {Label}";
    }
}