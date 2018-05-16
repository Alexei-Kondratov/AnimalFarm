using AnimalFarm.Data;
using AnimalFarm.Logic.AnimalBox;
using AnimalFarm.Model;
using AnimalFarm.Model.Events;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace AnimalFarm.AnimalService.Controllers
{
    [Route("")]
    public class AnimalController : Controller
    {
        private ITransactionManager _transactionManager;
        private IRepository<Animal> _animals;
        private IRepository<Ruleset> _rulesets;

        public AnimalController(ITransactionManager transactionManager, IRepository<Animal> animals, IRepository<Ruleset> rulesets)
        {
            _transactionManager = transactionManager;
            _animals = animals;
            _rulesets = rulesets;
        }

        [Route("{userId}/{animalId}")]
        [HttpGet()]
        public async Task<IActionResult> Get(string userId, string animalId)
        {
            using (var tx = _transactionManager.CreateTransaction())
            {
                var animal = await _animals.ByIdAsync(tx, userId, animalId);
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
                var animalBox = new AnimalBox(_rulesets, _animals);
                await animalBox.SetAnimalAsync(tx, e.OwnerUserId, e.AnimalId);
                bool isSuccess = await animalBox.RunEventsAsync(new[] { e });
                if (isSuccess)
                {
                    await animalBox.CommitAsync();
                    await tx.CommitAsync();
                }
                return Ok();
            }
        }
    }
}
