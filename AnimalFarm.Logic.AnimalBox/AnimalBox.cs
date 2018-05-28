using AnimalFarm.Data;
using AnimalFarm.Logic.RulesetManagement;
using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AnimalFarm.Logic.AnimalBox
{
    public class AnimalBox
    {
        private readonly IRepository<Animal> _animals;
        private readonly IRepository<Ruleset> _rulesets;
        private readonly RulesetScheduleProvider _scheduleProvider;
        private ITransaction _transaction;

        internal Animal Animal;
        internal Ruleset ActiveRuleset;

        private readonly Dictionary<Type, Type> _handlerTypeByEventType;

        private IAnimalEventHandler InstantiateEventHandler(AnimalEvent e)
        {
            if (!_handlerTypeByEventType.TryGetValue(e.GetType(), out Type handlerType))
                return null;

            ConstructorInfo emptyConstructor = handlerType.GetConstructor(new Type[] { });
            return emptyConstructor.Invoke(new object[] { }) as IAnimalEventHandler;
        }

        private AnimalBoxEventContext InstantiateBoxContext()
        {
            return new AnimalBoxEventContext(this, (rulesetId) => _rulesets.ByIdAsync(_transaction, rulesetId, rulesetId));
        }

        public AnimalBox(IRepository<Ruleset> rulesets, IRepository<Animal> animals, RulesetScheduleProvider scheduleProvider)
        {
            _animals = animals;
            _rulesets = rulesets;
            _scheduleProvider = scheduleProvider;

             //TODO: Extract the event handlers map.
             _handlerTypeByEventType = new Dictionary<Type, Type>
            {
                { typeof(CreateAnimalEvent), typeof(CreateAnimalEventHandler) },
                { typeof(AnimalActionEvent), typeof(AnimalActionEventHandler) },
                { typeof(AnimalRulesetChangeEvent), typeof(AnimalRulesetChangeEventHandler)}
            };
        }

        public async Task SetAnimalAsync(ITransaction transaction, string ownerId, string animalId)
        {
            _transaction = transaction;
            Animal = await _animals.ByIdAsync(_transaction, ownerId, animalId);
            VersionScheduleRecord activeRulesetRecord = await _scheduleProvider.GetActiveRulesetRecordAsync(_transaction, Animal.LastCalculated);
            ActiveRuleset = await _rulesets.ByIdAsync(_transaction, activeRulesetRecord.RulesetId, activeRulesetRecord.RulesetId);
        }

        private void AdvanceTime(DateTime stop)
        {
            if (Animal == null)
                return;

            decimal timePassedMins = (decimal)((stop - Animal.LastCalculated).TotalMinutes);

            if (timePassedMins <= 0)
                return;
            
            AnimalType animalType = ActiveRuleset.AnimalTypes[Animal.TypeId];
            foreach (string attributeId in animalType.Attributes.Keys)
            {
                if (!Animal.Attributes.ContainsKey(attributeId))
                    continue;

                AnimalTypeAttribute animalTypeAttribute = animalType.Attributes[attributeId];
                decimal minValue = animalTypeAttribute.MinValue;
                decimal maxValue = animalTypeAttribute.MaxValue;
                decimal ratioPerMin = animalTypeAttribute.RatioPerMinute;
                decimal changeOverTime = timePassedMins * ratioPerMin;
                decimal newValue = Animal.Attributes[attributeId] + changeOverTime;
                newValue = Math.Max(minValue, Math.Min(maxValue, newValue));
                Animal.Attributes[attributeId] = newValue;
            }

            Animal.LastCalculated = stop;
        }

        private bool RunEvent(AnimalEvent e, AnimalBoxEventContext context)
        {
            IAnimalEventHandler handler = InstantiateEventHandler(e);

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
                    // Rollback();
                    return false;
                }
            }

            return true;
        }

        public async Task CommitAsync()
        {
            if (Animal == null)
                return;

            await _animals.UpsertAsync(_transaction, Animal);
        }
    }
}
