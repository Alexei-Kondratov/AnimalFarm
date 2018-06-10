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
            var seeder = new DocumentDBSeeder();
            await seeder.SeedAsync(new SeedData());
            await seeder.SeedAsync(new SeedConfiguraiton());
        }
    }
}
