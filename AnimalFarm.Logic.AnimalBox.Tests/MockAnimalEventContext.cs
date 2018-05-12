using System;
using System.Collections.Generic;
using System.Text;
using AnimalFarm.Model;

namespace AnimalFarm.Logic.AnimalBox.Tests
{
    public class MockAnimalEventContext : IAnimalEventContext
    {
        public Animal Animal { get ; set; }
        public Ruleset ActiveRuleset { get; set; }
    }
}
