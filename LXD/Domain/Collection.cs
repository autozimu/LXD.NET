using System.Collections.Generic;
using System.Collections;
using System.Diagnostics.Contracts;
using RestSharp;

namespace LXD.Domain
{
    public class Collection<T> : RemoteObject, IEnumerable<T>
    {
        string component;

        public string[] IDs => API.Get<string[]>(component);

        public Collection(API API, string component)
        {
            Contract.Requires(API != null);
            Contract.Requires(component != null);

            this.API = API;
            this.component = component;
        }

        public int Count => IDs.Length;

        public T this[int index]
        {
            get
            {
                Contract.Requires(index >= 0);

                return this[IDs[index]];
            }
            set { /* set the specified index to value here */ }
        }

        public T this[string id]
        {
            get
            {
                return API.Get<T>(id);
            }
            set { /* set the specified index to value here */ }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (string id in IDs)
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
            API.Execute(request);
        }
    }
}
