using Humanizer;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lxd
{
    class PascalCasePropertyNamesContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName) => propertyName.Underscore();
    }
}
