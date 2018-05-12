using AnimalFarm.Data;
using AnimalFarm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AnimalFarm.Logic.AnimalBox
{
    public class AnimalEvent
    {
        public string EventId { get; set; }
        public DateTime Time { get; set; }
        public string ActingUserId { get; set; }
        public string OwnerUserId { get; set; }
        public string AnimalId { get; set; }
    }

    public class AnimalActionEvent : AnimalEvent
    {
        public string AnimalActionId { get; set; }
    }

    public class AnimalCreateEvent : AnimalEvent
    {
        public string Name { get; set; }
        public string AnimalTypeId { get; set; }
    }

    public class AnimalRulesetChangeEvent : AnimalEvent
    {
        public string NewVersionId { get; set; }
    }

    public class AnimalBoxEventContext
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

    public interface IBoxAnimalEventHandler
    {
        bool Validate(AnimalEvent e, AnimalBoxEventContext context);
        void Apply(AnimalEvent e, AnimalBoxEventContext context);
    }

    public abstract class BoxAnimalEventHandler<TEvent> : IBoxAnimalEventHandler
        where TEvent : AnimalEvent
    {
        public abstract bool Validate(TEvent e, AnimalBoxEventContext context);

        public abstract void Apply(TEvent e, AnimalBoxEventContext context);

        bool IBoxAnimalEventHandler.Validate(AnimalEvent e, AnimalBoxEventContext context)
        {
            if (e is TEvent)
            {
                return Validate(e as TEvent, context);
            }

            return true;
        }

        void IBoxAnimalEventHandler.Apply(AnimalEvent e, AnimalBoxEventContext context)
        {
            if (e is TEvent)
            {
                Apply(e as TEvent, context);
            }
        }
    }

    public class BoxAnimalCreateEventHandler : BoxAnimalEventHandler<AnimalCreateEvent>
    {
        public override bool Validate(AnimalCreateEvent e, AnimalBoxEventContext context)
        {
            throw new NotImplementedException();
        }

        public override void Apply(AnimalCreateEvent e, AnimalBoxEventContext context)
        {
            AnimalType animalType = context.ActiveRuleset.AnimalTypes[e.AnimalId];

            var animal = new Animal
            {
                Id = e.AnimalId,
                Name = e.Name,
                UserId = e.OwnerUserId,
                TypeId = e.AnimalTypeId,
                Attributes = animalType.Attributes.ToDictionary(a => a.Key, a => a.Value.InitialValue),
                LastCalculated = e.Time,
                Created = e.Time
            };

            context.Animal = animal;
        }
    }

    public class BoxAnimalActionEventHandler
    {
        public void Validate(AnimalActionEvent e, AnimalBoxEventContext context)
        {

        }

        public void Apply(AnimalActionEvent e, AnimalBoxEventContext context)
        {
            Animal animal = context.Animal;
            AnimalAction action = context.ActiveRuleset.AnimalActions[e.AnimalActionId];
            AnimalType animalType = context.ActiveRuleset.AnimalTypes[animal.TypeId];

            foreach (string attributeId in action.AttributeEffects.Keys)
            {
                AnimalTypeAttribute animalTypeAttribute = animalType.Attributes[attributeId];
                decimal minValue = animalTypeAttribute.MinValue;
                decimal maxValue = animalTypeAttribute.MaxValue;
                decimal newValue = animal.Attributes[attributeId] + action.AttributeEffects[attributeId];
                newValue = Math.Max(minValue, Math.Min(maxValue, newValue));
                animal.Attributes[attributeId] = newValue;
            }
        }
    }

    public class AnimalBox
    {
        private readonly IRepository<Animal> _animals;
        private readonly IRepository<Ruleset> _rulesets;

        internal Animal Animal;
        internal Ruleset ActiveRuleset;

        private Dictionary<Type, Type> _handlerTypeByEventType;

        private IBoxAnimalEventHandler InstantiateEventHandler(AnimalEvent e)
        {
            Type handlerType;
            if (!_handlerTypeByEventType.TryGetValue(e.GetType(), out handlerType))
                return null;

            ConstructorInfo emptyConstructor = handlerType.GetConstructor(new Type[] { });
            return emptyConstructor.Invoke(new object[] { }) as IBoxAnimalEventHandler;
        }

        private AnimalBoxEventContext InstantiateBoxContext()
        {
            return new AnimalBoxEventContext(this);
        }

        public AnimalBox(IRepository<Ruleset> rulesets, IRepository<Animal> animals)
        {
            _animals = animals;
            _rulesets = rulesets;
        }

        public async Task SetAnimalAsync(string animalId)
        {
            Animal = await _animals.ByIdAsync(animalId); 
        }

        private void AdvanceTime(DateTime stop)
        {
            if (Animal == null)
                return;

            decimal timePassedMins = (decimal)((stop - Animal.LastCalculated).TotalMinutes);

            if (timePassedMins <= 0)
                return;
            
            AnimalType animalType = ActiveRuleset.AnimalTypes[Animal.TypeId];
            foreach (string attributeId in Animal.Attributes.Keys)
            {
                AnimalTypeAttribute animalTypeAttribute = animalType.Attributes[attributeId];
                decimal minValue = animalTypeAttribute.MinValue;
                decimal maxValue = animalTypeAttribute.MaxValue;
                decimal ratioPerMin = animalTypeAttribute.RatioPerMinute;
                decimal changeOverTime = timePassedMins * ratioPerMin;
                decimal newValue = Animal.Attributes[attributeId] + changeOverTime;
                newValue = Math.Max(minValue, Math.Min(maxValue, newValue));
                Animal.Attributes[attributeId] = newValue;
            }
        }

        private bool RunEvent(AnimalEvent e, AnimalBoxEventContext context)
        {
            IBoxAnimalEventHandler handler = InstantiateEventHandler(e);

            if (!handler.Validate(e, context))
                return false;
            handler.Apply(e, context);

            return true;
        }

        public async Task<bool> RunEventsAsync(IEnumerable<AnimalEvent> events)
        {
            AnimalBoxEventContext eventContext = InstantiateBoxContext();

            foreach (AnimalEvent e in events.OrderBy(e => e.Time))
            {
                AdvanceTime(e.Time);
                if (!RunEvent(e, eventContext))
                {
                    Rollback();
                    return false;
                }
            }

            return true;
        }

        public async Task CommitAsync()
        {
            if (Animal == null)
                return;

            await _animals.UpsertAsync(Animal);
        }
    }
}
