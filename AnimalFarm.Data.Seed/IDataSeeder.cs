﻿using System.Threading.Tasks;

namespace AnimalFarm.Data.Seed
{
    public interface IDataSeeder
    {
        Task SeedAsync(ISeedData seedData);
    }
}