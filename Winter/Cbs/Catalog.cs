using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter.Cbs
{
    internal class Catalog : IDisposable
    {
        private readonly ComponentStoreService store;
        private readonly ManagedRegistryKey key;

        public string Thumbprint => key.Name;

        private IEnumerable<Component> GetComponents()
        {
            return Enumerable.Empty<Component>();
        }

        public IEnumerable<Component> Components => GetComponents();

        public Catalog(ComponentStoreService store, ManagedRegistryKey key)
        {
            this.store = store;
            this.key = key;
        }

        public void Dispose()
        {
            key.Dispose();
        }
    }
}
