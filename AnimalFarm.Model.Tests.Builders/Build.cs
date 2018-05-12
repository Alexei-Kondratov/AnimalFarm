using System;
using System.Collections.Generic;
using System.Text;

namespace AnimalFarm.Model.Tests.Builders
{
    public static class Build
    {
        public static RulesetBuilder Ruleset(string id = null)
        {
            return new RulesetBuilder(id);
        }
    }
}
