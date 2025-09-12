namespace MenuSystem;

public class Menu
{
    public List<MenuItem> MenuItems { get; set; } = [];


    public void AddMenuItems(List<MenuItem> items)
    {
        // check for validity
        // add 
        foreach (var item in items)
        {
            // control check - dictionary
            
            
            
            MenuItems.Add(item);
        }
    }
    
    public void Run()
    {
        var menuIsDone = false;
        do
        {
            //  display menu
            DisplayMenu();
            
            // get user input
            Console.Write("Please make a slection: ");
            var userInput = Console.ReadLine();
            // validate input
            // execute choice
            
            if(userInput == "X")
            {
                menuIsDone = true;
            }
            
        } while (!menuIsDone);
    }
    
        // loop back to top
        
        
        
        private void DisplayMenu()
        {
            foreach (var item in MenuItems)
                Console.WriteLine(item.Label);
        }
        
}