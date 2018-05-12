using System;
using System.Collections.Generic;

namespace AnimalFarm.Model.Tests.Builders
{
    public class AnimalTypeBuilder : NestedBuilder<AnimalType, RulesetBuilder>
    {
        public AnimalTypeBuilder(AnimalType target, RulesetBuilder parent)
            : base(target, parent)
        {
        }

        public AnimalTypeBuilder HavingAttribute(string attributeId,
            decimal initialValue = 0, decimal minValue = 0, decimal maxValue = 100, decimal ratio = 10)
        {
            if (_target.Attributes == null)
                _target.Attributes = new Dictionary<string, AnimalTypeAttribute>();

            _target.Attributes.Add(attributeId, new AnimalTypeAttribute
            {
                InitialValue = initialValue,
                MinValue = minValue,
                MaxValue = maxValue,
                RatioPerMinute = ratio
            });
            return this;
        }
    }
}
