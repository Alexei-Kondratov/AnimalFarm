using System;
using System.Collections.Generic;

namespace AnimalFarm.Model.Tests.Builders
{
    public class AnimalActionBuilder : NestedBuilder<AnimalAction, RulesetBuilder>
    {
        public AnimalActionBuilder(AnimalAction target, RulesetBuilder parent)
            : base(target, parent)
        {
        }

        public AnimalActionBuilder HavingAttributeEffect(string attributeId, decimal effectValue)
        {
            if (_target.AttributeEffects == null)
                _target.AttributeEffects = new Dictionary<string, decimal>();

            _target.AttributeEffects.Add(attributeId, effectValue);
            return this;
        }
    }
}
