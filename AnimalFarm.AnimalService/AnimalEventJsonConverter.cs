using AnimalFarm.Model.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace AnimalFarm.AnimalService
{
    public class AnimalEventJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(AnimalEvent).IsAssignableFrom(objectType);
        }

        private AnimalEvent InstantiateByTypeName(string eventTypeName)
        {
            switch (eventTypeName)
            {
                case "interact":
                    return new AnimalActionEvent();
                case "create":
                    return new CreateAnimalEvent();
                default:
                    throw new NotSupportedException(eventTypeName);
            }
        }
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObj = JObject.Load(reader);
            bool isEventTypeProperty(JProperty p) 
                => String.Equals(p.Name, nameof(AnimalEvent.EventType), StringComparison.InvariantCultureIgnoreCase);

            string typeName = ((string)jObj.Children<JProperty>().First(isEventTypeProperty).Value).ToLower();
            AnimalEvent result = InstantiateByTypeName(typeName);
            serializer.Populate(jObj.CreateReader(), result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}
