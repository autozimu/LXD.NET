using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace LXD
{
    public class Collection<T> : IEnumerable<T>
    {
        string component;

        public string[] Ids => Client.API.Get(component).SelectToken("metadata").ToObject<string[]>();

        public Collection(string component)
        {
            this.component = component;
        }

        public int Count => Ids.Length;

        public T this[int index]
        {
            get
            {
                return this[Ids[index]];
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
            foreach (string id in Ids)
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
