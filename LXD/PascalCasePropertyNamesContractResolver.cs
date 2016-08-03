using Humanizer;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LXD
{
    class PascalCasePropertyNamesContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            if (char.IsLower(propertyName.First()))
            {
                // Property already have a customized name.
                return propertyName;
            }

            return propertyName.Underscore();
        }
    }
}
