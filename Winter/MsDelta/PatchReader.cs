using Smx.SharpIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smx.Winter.MsDelta
{
    internal class PatchReader
    {
        private MFile patch;

        public PatchReader(MFile patch)
        {
            this.patch = patch;
        }

        public void Read()
        {
            var sr = new SpanStream(patch);
            var magic = sr.ReadString(4, Encoding.ASCII);
            if (magic == "DCM\x01")
            {
                magic = sr.ReadString(4, Encoding.ASCII);
            }
            if (magic != "PA30")
            {
                throw new NotSupportedException();
            }
            long fileTime = (long)sr.ReadUInt64();
            var time = DateTime.FromFileTime(fileTime);
            Console.WriteLine(time);
        }
    }
}
