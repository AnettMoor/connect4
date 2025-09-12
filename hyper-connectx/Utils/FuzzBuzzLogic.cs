namespace Utils;

public static class FizzBuzzLogic
{
    public static void OutputFizzBuzzInstructions()
    {
        Console.WriteLine("FIZZBUZZ GAME");
    }

    public static int FizzBuzzCount()
    {
        Console.WriteLine("enter any number for the FizzBuzz game: ");
        var userInput = Console.ReadLine();

        var countTo = 100;

        if (userInput != null)
        {
            if (!int.TryParse(userInput, out countTo))
            {
                countTo = 100;
            }
        }
        else
        {
            Console.WriteLine("You did not enter a number, defaulting to 100");
        }

        return countTo;
    }

    public static void OutputFizzBuzz(int countTo)
    {
        for (var i = 1; i < countTo; i++)
        {
            if (i % 3 == 0)
            {
                Console.Write("fizz");

                if (i % 5 == 0)
                {
                    Console.Write("buzz");
                    continue;
                }

                Console.Write((", "));
                continue;
            }

            if (i % 5 == 0)
            {
                Console.Write("Buzz");
            }

            Console.Write((i + ", "));
        }
    }
}