namespace MenuSystem;


public class MenuItem

{
    public string Label { get; set; } = default!; // lables should not be fixed - tööül, they need to be updated
    public string Key { get; set; } = default!;
    
    
    // thing that should happen...
    // exit, return to prev, and return to main do not have actions
    // all other menu items should have actions
    public Action<string>? RunThisThingWhenMenuItemIsSelected{ get; set; }
    
    
    
    public override string ToString()
    {
        return $"{Key}) {Label}";
    }
    
}