using Smx.Winter.Cbs.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter.Cbs
{
    public interface ICbsEnumerable<Titem>
    {
        void Next(uint celt, out Titem item, out uint fetched);
        void Skip(uint celt);
        void Reset();
    }

    public class CbsEnumerable<TEnum, TItem> : IEnumerable<TItem>
        where TEnum : ICbsEnumerable<TItem>
    {
        private readonly TEnum _list;

        public CbsEnumerable(TEnum list)
        {
            _list = list;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            for (; ; )
            {
                _list.Next(1, out var item, out var fetched);
                if (fetched == 0)
                {
                    yield break;
                } else
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
