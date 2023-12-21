using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter
{
    internal class ComponentFactory
    {
        private readonly WindowsSystem windows;
        private readonly ComponentStoreService componentStore;

        public ComponentFactory(WindowsSystem windows, ComponentStoreService componentStore)
        {
            this.windows = windows;
            this.componentStore = componentStore;
        }

        private static string? GetComponentIdFromFile(string filePath)
        {
            var dirSep = Path.DirectorySeparatorChar;
            return Util.GetHardLinks(filePath)
                .FirstOrDefault(
                    l => l.Contains($"{dirSep}WinSxS{dirSep}",
                    StringComparison.InvariantCultureIgnoreCase));
        }


        public Component GetFromManifest(string manifestPath)
        {
            //componentStore.TraverseDeployments
            return null;
        }

        public Component GetFromFile(string filePath)
        {
            var id = GetComponentIdFromFile(filePath);
            return new Component(id);
        }
    }
}
