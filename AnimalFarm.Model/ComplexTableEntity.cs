using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AnimalFarm.Model
{
    /// <summary>
    /// Implementation of the TableEntity that serializes all complex properties in Json format.
    /// </summary>
    public abstract class ComplexTableEntity : TableEntity
    {
        private IEnumerable<PropertyInfo> GetExcludedProperties(IDictionary<string, EntityProperty> entityProperties)
        {
            var ignoreList = new[] { nameof(ETag), nameof(PartitionKey), nameof(RowKey), nameof(Timestamp) };

            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            return properties.Where(p => 
                !entityProperties.ContainsKey(p.Name) && !ignoreList.Contains(p.Name));
        }

        private IEnumerable<PropertyInfo> GetNotReadProperties(IDictionary<string, EntityProperty> entityProperties)
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            return properties.Where(p =>
                entityProperties.ContainsKey(p.Name) && p.GetValue(this) == null);
        }

        public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            var results = base.WriteEntity(operationContext);

            foreach (PropertyInfo property in GetExcludedProperties(results))
            {
                var value = property.GetValue(this);
                results.Add(property.Name, new EntityProperty(JsonConvert.SerializeObject(value)));
            }

            return results;
        }

        public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            base.ReadEntity(properties, operationContext);

            foreach (PropertyInfo property in GetNotReadProperties(properties))
            {
                var serializedValue = properties[property.Name].StringValue;
                var value = JsonConvert.DeserializeObject(serializedValue, property.PropertyType);
                property.SetValue(this, value);
            }
        }
    }
}
