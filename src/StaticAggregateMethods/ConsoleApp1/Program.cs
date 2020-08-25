using System;
using GeneratedCheese;
using Lib;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var childDing = new ChildDing();
            var cheeseChooser = new CheeseChooser();
            Console.WriteLine($"The best cheese for pasta is: {cheeseChooser.BestCheeseForPasta}");
            Console.WriteLine($"The best cheese for potato is: {cheeseChooser.BestCheeseForBakedPotato}");
            Console.WriteLine(cheeseChooser.Test);
        } 

        class State
        {

        }

        class ChildDing: Aggregate<State>
        {

        }
    }

}
