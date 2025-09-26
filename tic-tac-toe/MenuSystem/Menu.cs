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
            Console.Write("Please make a selection: ");
            var userInput = Console.ReadLine();
            
            // validate input
            if (userInput == "1")
                // create new + edit
            if (userInput == "2")   
                // load previous
            if (userInput == "3")  
                // settings
                
                
            // execute choice
            
            if(userInput == "x")
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