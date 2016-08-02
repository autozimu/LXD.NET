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

        string[] ids => Client.API.Get(component).ToObject<string[]>();

        public Collection(string component)
        {
            this.component = component;
        }

        public int Count => ids.Length;

        public T this[int index]
        {
            get
            {
                return this[ids[index]];
            }
            set { /* set the specified index to value here */ }
        }

        public T this[string id]
        {
            get
            {
                return Client.API.Get<T>(id);
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

        public void Remove(string id)
        {
            IRestRequest request = new RestRequest($"{component}/{id}", Method.DELETE);
            Client.API.Execute(request);
        }
    }
}
