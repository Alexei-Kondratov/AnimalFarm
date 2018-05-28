using AnimalFarm.Data;
using AnimalFarm.Logic.AnimalBox;
using AnimalFarm.Logic.RulesetManagement;
using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalFarm.AnimalService.Controllers
{
    [Route("")]
    public class AnimalController : Controller
    {
        private ITransactionManager _transactionManager;
        private IRepository<Animal> _animals;
        private IRepository<Ruleset> _rulesets;
        private RulesetScheduleProvider _scheduleProvider;

        public AnimalController(ITransactionManager transactionManager, IRepository<Animal> animals, IRepository<Ruleset> rulesets, RulesetScheduleProvider scheduleProvider)
        {
            _transactionManager = transactionManager;
            _animals = animals;
            _rulesets = rulesets;
            _scheduleProvider = scheduleProvider;
        }

        private async Task<bool> RunEventsAsync(ITransaction tx, Animal animal, IEnumerable<AnimalEvent> userEvents)
        {
            var animalBox = new AnimalBox(_rulesets, _animals, _scheduleProvider);
            await animalBox.SetAnimalAsync(tx, animal.UserId, animal.Id);
            var rulesetChanges = await _scheduleProvider.GetActiveRulesetRecordsAsync(tx, animal.LastCalculated, DateTime.UtcNow);
            var rulesetChangeEvents =
                rulesetChanges.Select(r => new AnimalRulesetChangeEvent { Time = r.Key, NewVersionId = r.Value, AnimalId = animal.Id, OwnerUserId = animal.UserId });

            bool isSuccess = await animalBox.RunEventsAsync(rulesetChangeEvents.Concat(userEvents));
            if (isSuccess)
            {
                await animalBox.CommitAsync();
            }

            return isSuccess;
        }

        [Route("{userId}/{animalId}")]
        [HttpGet()]
        public async Task<IActionResult> Get(string userId, string animalId)
        {
            using (var tx = _transactionManager.CreateTransaction())
            {
                Animal animal = await _animals.ByIdAsync(tx, userId, animalId);
                VersionScheduleRecord currentRulesetRecord = await _scheduleProvider.GetActiveRulesetRecordAsync(tx, DateTime.UtcNow);
                if (currentRulesetRecord.Start > animal.LastCalculated)
                {
                    await RunEventsAsync(tx, animal, Enumerable.Empty<AnimalEvent>());
                }

                await tx.CommitAsync();
                return Json(animal);
            }
        }

        [Route("event")]
        [HttpPut()]
        public async Task<IActionResult> PushEvent([FromBody]AnimalEvent e)
        {
            using (var tx = _transactionManager.CreateTransaction())
            {
                Animal animal = await _animals.ByIdAsync(tx, e.OwnerUserId, e.AnimalId);
                bool isSuccess = await RunEventsAsync(tx, animal, new[] { e });
                if (isSuccess)
                {
                    await tx.CommitAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
        }
    }
}
