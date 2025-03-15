using Smx.Winter.Cbs.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter.Cbs.Enumerators
{
    public class CbsIdentityEnumerable : ICbsEnumerable<ICbsIdentity>, IEnumerable<ICbsIdentity>
    {
        private readonly IEnumCbsIdentity _enumerator;

        public CbsIdentityEnumerable(IEnumCbsIdentity enumerator)
        {
            _enumerator = enumerator;
        }

        public IEnumerator<ICbsIdentity> GetEnumerator()
        {
            return new CbsEnumerable<CbsIdentityEnumerable, ICbsIdentity>(this).GetEnumerator();
        }

        public void Next(uint celt, out ICbsIdentity item, out uint fetched)
        {
            _enumerator.Next(celt, out item, out fetched);
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
