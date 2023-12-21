using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Smx.Winter
{
    public static class Extensions
    {
        public static Span<T> AsSpan<T>(this nint ptr, int numElements)
        {
            Span<T> span;
            unsafe
            {
                span = new Span<T>(ptr.ToPointer(), numElements);
            }
            return span;
        }

        public static void WithSavedPosition(this Stream stream, Action action)
        {
            WithSavedPosition<object?>(stream, (_) =>
            {
                action();
                return null;
            });
        }

        public static void WithSavedPosition<T>(this Stream stream, Action<Stream> action)
        {
            WithSavedPosition<object?>(stream, (_) =>
            {
                action(stream);
                return null;
            });
        }

        public static T WithSavedPosition<T>(this Stream stream, Func<T> action)
        {
            return WithSavedPosition(stream, (_) =>
            {
                return action();
            });
        }

        public static T WithSavedPosition<T>(this Stream stream, Func<Stream, T> action)
        {
            var pos = stream.Position;
            var ret = action(stream);
            stream.Position = pos;
            return ret;
        }

        public static Memory<byte> ReadToEnd(this Stream stream)
        {
            var buf = new byte[stream.Length - stream.Position];
            stream.Read(buf);
            return buf;
        }
    }
}
