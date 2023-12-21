using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter
{
    public class WindowsSystem
    {
        public string SystemRoot
        {
            get => ManagedRegistryKey.Open(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion").GetValue<string>("SystemRoot");
        }
    }
}
