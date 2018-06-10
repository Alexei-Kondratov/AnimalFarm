using AnimalFarm.Data.Seed;
using System.Threading.Tasks;

namespace AnimalFarm.Tools.ResetData
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var seedData = new SeedData();
            await new DocumentDBSeeder().SeedAsync(seedData);
        }
    }
}
