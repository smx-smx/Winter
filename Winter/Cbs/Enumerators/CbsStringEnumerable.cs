using Smx.Winter.Cbs.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter.Cbs.Enumerators
{
    public class CbsStringEnumerable : ICbsEnumerable<string>, IEnumerable<string>
    {
        private readonly IEnumString _enumerator;

        public CbsStringEnumerable(IEnumString enumerator)
        {
            _enumerator = enumerator;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new CbsEnumerable<CbsStringEnumerable, string>(this).GetEnumerator();
        }

        public void Next(uint celt, out string item, out uint fetched)
        {
            _enumerator.RemoteNext(celt, out item, out fetched);
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        public void Skip(uint celt)
        {
            _enumerator.Skip(celt);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
