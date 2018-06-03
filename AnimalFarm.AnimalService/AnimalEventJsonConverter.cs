using AnimalFarm.Model.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace AnimalFarm.AnimalService
{
    internal class AnimalEventJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(AnimalEvent).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObj = JObject.Load(reader);
            string typeName = (string)jObj[nameof(AnimalEvent.EventType)];

            AnimalEvent result;
            switch (typeName)
            {
                case "Interact":
                    result = new AnimalActionEvent();
                    break;
                case "Create":
                    result = new CreateAnimalEvent();
                    break;
                default:
                    throw new NotImplementedException();
            }

            serializer.Populate(jObj.CreateReader(), result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}
