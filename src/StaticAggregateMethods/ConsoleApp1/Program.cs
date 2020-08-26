using System;
using Lib;

namespace ConsoleApp1
{
    public class Program
    {
        static void Main(string[] args)
        {
            _ = new ChildDing(); 
            var agg = AggregateRegistry.Resolve<State>();
            var state = agg.When(new State(), new Command());
            Console.WriteLine(state.Hoi);
        } 

        
    }
    public class Command
    {

    }

    public class State
    {
        public string Hoi { get; set; } = "hoi";
    }

    public class ChildDing : Aggregate<State>
    {
        public ChildDing()
        {
            When<Command>((s, _) => new State { Hoi = "doei" });
        }
    }
}
