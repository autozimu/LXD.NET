using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace lxd
{
    public class Collection<T> : IEnumerable<T>
    {
        string component;
        API api;

        string[] ids => api.Get(component).ToObject<string[]>();

        public Collection(string component, API api)
        {
            this.component = component;
            this.api = api;
        }

        public T this[int index]
        {
            get
            {
                return this[ids[index]];
            }
            set { /* set the specified index to value here */ }
        }

        public T this[string index]
        {
            get
            {
                return api.Get<T>(index);
            }
            set { /* set the specified index to value here */ }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (string id in ids)
            {
                yield return this[id];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
