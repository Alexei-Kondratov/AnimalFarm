using AnimalFarm.Model;

namespace AnimalFarm.Logic.AnimalBox
{
    public class AnimalBoxEventContext : IAnimalEventContext
    {
        private readonly AnimalBox _box;

        public Animal Animal
        {
            get => _box.Animal;
            set => _box.Animal = value;
        }

        public Ruleset ActiveRuleset
        {
            get => _box.ActiveRuleset;
            set => _box.ActiveRuleset = value;
        }

        public AnimalBoxEventContext(AnimalBox box)
        {
            _box = box;
        }
    }
}