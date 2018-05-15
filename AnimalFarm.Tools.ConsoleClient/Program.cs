using System;
using System.Net.Http;

namespace AnimalFarm.Tools.ConsoleClient
{
    class Program
    {
        private const string baseAddress = @"http://localhost:88/"; 

        static void Main(string[] args)
        {
            Console.WriteLine("1. Request current ruleset");
            var key = Console.ReadKey(true);
            while (key.Key != ConsoleKey.D1 && key.Key != ConsoleKey.Escape)
            {
                key = Console.ReadKey(true);
            }

            if (key.Key == ConsoleKey.Escape)
                return;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseAddress);
                var response = client.GetStringAsync("ruleset").GetAwaiter().GetResult();
                Console.WriteLine(response);
            }
        }
    }
}
