using Smx.Winter;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Registry;

namespace RegBatch
{
    public class Params
    {
        public string SearchText { get; private set; }
        public byte[] SearchTextUtf8 { get; private set; }
        public byte[] SearchTextUnicode { get; private set; }

        public Params(string searchText)
        {
            SearchText = searchText;
            SearchTextUtf8 = new UTF8Encoding(false).GetBytes(SearchText);
            SearchTextUnicode = new UnicodeEncoding(false, false).GetBytes(SearchText);
        }
    }

    public class Context
    {
        public Stats Stats { get; set; } = new Stats();
        public required Params Params { get; set; }
    }

    public class Stats
    {

        private int _nTasks;
        private int _doneTasks;

        private int _numByteArrays;
        private int _numDwords;
        private int _numQwords;
        private int _numStrings;
        private int _numCollections;
        private int _numKeys;
        private int _numValues;

        public int NumByteArrays => _numCollections;
        public int NumDwords => _numDwords;
        public int NumQwords => _numQwords;
        public int NumStrings => _numStrings;
        public int NumCollections => _numCollections;
        public int NumKeys => _numKeys;
        public int NumValues => _numValues;

        public int DoneTasks => _doneTasks;
        public int TotalTasks => _nTasks;

        public delegate void KeyMatchHandler(ManagedRegistryKey key, string subKey);
        public delegate void ValueMatchHandler(ManagedRegistryKey key, string valueName, REG_VALUE_TYPE? valueType, object? value);

        public event KeyMatchHandler? OnKeyMatch;
        public event ValueMatchHandler? OnValueMatch;

        public void MarkKey(ManagedRegistryKey key, string subKey)
        {
            Interlocked.Increment(ref _numKeys);
            OnKeyMatch?.Invoke(key, subKey);
        }

        public void MarkNewTask()
        {
            Interlocked.Increment(ref _nTasks);
        }

        internal void MarkDoneTask()
        {
            Interlocked.Increment(ref _doneTasks);
        }

        internal void MarkByteArray(ManagedRegistryKey key, string valueName, REG_VALUE_TYPE valueType, object value)
        {
            Interlocked.Increment(ref _numByteArrays);
            OnValueMatch?.Invoke(key, valueName, valueType, value);
        }
        internal void MarkDword(ManagedRegistryKey key, string valueName, REG_VALUE_TYPE valueType, object value)
        {
            Interlocked.Increment(ref _numDwords);
            OnValueMatch?.Invoke(key, valueName, valueType, value);
        }
        internal void MarkQword(ManagedRegistryKey key, string valueName, REG_VALUE_TYPE valueType, object value)
        {
            Interlocked.Increment(ref _numQwords);
            OnValueMatch?.Invoke(key, valueName, valueType, value);
        }
        internal void MarkString(ManagedRegistryKey key, string valueName, REG_VALUE_TYPE valueType, object value)
        {
            Interlocked.Increment(ref _numStrings);
            OnValueMatch?.Invoke(key, valueName, valueType, value);
        }
        internal void MarkCollection(ManagedRegistryKey key, string valueName, REG_VALUE_TYPE valueType, object value)
        {
            Interlocked.Increment(ref _numCollections);
            OnValueMatch?.Invoke(key, valueName, valueType, value);
        }
        internal void MarkValue(ManagedRegistryKey key, string valueName)
        {
            Interlocked.Increment(ref _numValues);
            OnValueMatch?.Invoke(key, valueName, null, null);
        }
    }

    public class Program
    {
        bool TryTake(IEnumerator<string> it, [MaybeNullWhen(false)] out string arg)
        {
            if (!it.MoveNext())
            {
                arg = null;
                return false;
            }

            arg = it.Current;
            return true;
        }

        public Program()
        {
            Util.EnablePrivilege(PInvoke.SE_BACKUP_NAME);
            Util.EnablePrivilege(PInvoke.SE_RESTORE_NAME);
        }

        [DoesNotReturn]
        private void UsageExit(TextWriter os)
        {
            os.WriteLine("Usage: <mode> <...args>");
            Environment.Exit(1);
        }

        [DoesNotReturn]
        private void UsageExit()
        {
            UsageExit(Console.Error);
        }

        private ManagedRegistryKey? OpenKey(ManagedRegistryKey key, string subKey, SafeHandle? token = null)
        {
            try
            {
                if (token != null)
                {
                    return ElevationService.RunAsToken(token, () =>
                    {
                        return key.OpenChildKey(subKey);
                    });
                } else
                {
                    return key.OpenChildKey(subKey);
                }
            } catch (Win32Exception ex)
            {
                return null;
            }
        }

        private void CreateWorkItem(Context ctx, ManagedRegistryKey key)
        {
            if (key.CachedInfo.NumberOfSubKeys > 50)
            {
                ctx.Stats.MarkNewTask();
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    ProcessKey(ctx, key);
                    ctx.Stats.MarkDoneTask();
                });
            } else
            {
                ProcessKey(ctx, key);
            }
        }

        private void ProcessKey(Context theCtx, ManagedRegistryKey key)
        {
            var ctx = theCtx.Stats;
            var para = theCtx.Params;

            ctx.MarkNewTask();
            var enumVals = Task.Run(() =>
            {
                Parallel.ForEach(key.ValueNames, (valueName) =>
                {
                    if (valueName.Contains(para.SearchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ctx.MarkValue(key, valueName);
                    }

                    var value = key.GetValue(valueName, out var valueType);
                    switch (value)
                    {
                        case byte[] bytes:
                            bool isUtf8 = false;
                            bool isUnicode = false;
                            if((isUtf8=bytes.AsSpan().IndexOf(para.SearchTextUtf8) >= 0)
                            || (isUnicode=bytes.AsSpan().IndexOf(para.SearchTextUnicode) >= 0))
                            {
                                ctx.MarkByteArray(key, valueName, valueType, value);
                            }
                            break;
                        case uint dword:
                            if (false)
                            {
                                ctx.MarkDword(key, valueName, valueType, value);
                            }
                            break;
                        case ulong qword:
                            if (false)
                            {
                                ctx.MarkQword(key, valueName, valueType, value);
                            }
                            break;
                        case string str:
                            if (str.Contains(para.SearchText, StringComparison.InvariantCultureIgnoreCase))
                            {
                                ctx.MarkString(key, valueName, valueType, value);
                            }
                            break;
                        case ICollection<string> values:
                            string? found = null;
                            foreach(var v in values)
                            {
                                if(v.Contains(para.SearchText, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    found = v;
                                    break;
                                }
                            }
                            if (found != null)
                            {
                                ctx.MarkCollection(key, valueName, valueType, found);
                            }
                            break;
                    }
                });
                ctx.MarkDoneTask();
            });

            ctx.MarkNewTask();
            var enumKeys = Task.Run(() =>
            {
                Parallel.ForEach(key.KeyNames, (keyName) =>
                {
                    if(keyName.Contains(para.SearchText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        ctx.MarkKey(key, keyName);
                    }

                    try
                    {
                        var child = key.OpenChildKey(keyName);
                        CreateWorkItem(theCtx, child);
                    } catch (Win32Exception ex)
                    {
                        bool fatal = true;
                        switch ((WIN32_ERROR)ex.NativeErrorCode)
                        {
                            case WIN32_ERROR.ERROR_FILE_NOT_FOUND:
                                fatal = false;
                                break;
                            case WIN32_ERROR.ERROR_ACCESS_DENIED:
                                Console.Error.WriteLine($">> WARN: Skipping {key.Path}\\{keyName}");
                                fatal = false;
                                break;
                        }
                        if (fatal) throw;
                    }
                });
                ctx.MarkDoneTask();
            });
        }

        private void Traverse(Queue<ManagedRegistryKey> keys, string text)
        {
            var ctx = new Context
            {
                Params = new Params(text)
            };

            using var logFile = new StreamWriter(new FileStream("log.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read));
            logFile.BaseStream.SetLength(0);

            var lockObj = new object();

            ctx.Stats.OnKeyMatch += (key, keyName) =>
            {
                lock (lockObj)
                {
                    //Console.WriteLine($"Hit: {key.Path}\\{keyName}");
                    //Console.ReadLine();
                }

            };
            ctx.Stats.OnValueMatch += (key, valueName, valueType, value) =>
            {
                lock (lockObj)
                {
                    if (value is byte[]) return;
                    if (valueType is null) return;
                    if(value is string str)
                    {
                        Console.WriteLine($"Hit: {key.Path}\\{valueName} -> {value}");
                        //var newValue = ...
                        //key.SetValue(valueName, valueType.Value, newValue);
                    }
                }
            };

            while (keys.Count > 0)
            {
                var key = keys.Dequeue();
                CreateWorkItem(ctx, key);
            }

            var sw = Stopwatch.StartNew();
            var incrementalCount = 0;
            var stats = ctx.Stats;
            while (stats.DoneTasks < stats.TotalTasks)
            {
                if (sw.ElapsedMilliseconds >= 1000)
                {
                    sw.Restart();
                    var snapshot = ctx.Stats.DoneTasks;
                    var totalTasks = ctx.Stats.TotalTasks;
                    Console.WriteLine($"{snapshot} / {totalTasks} ({snapshot - incrementalCount} per sec)");
                    Console.WriteLine($"K: {stats.NumKeys}, " +
                        $"V: {stats.NumValues}, B: {stats.NumByteArrays}, D: {stats.NumDwords}, " +
                        $"Q: {stats.NumQwords}, S: {stats.NumStrings}, C: {stats.NumCollections}");
                    incrementalCount = snapshot;
                }
            }

            logFile.Flush();
        }

        private void RunFind(IEnumerator<string> it)
        {
            if (!TryTake(it, out var text))
            {
                UsageExit();
            }

            // we skip the following virtual views to avoid traversing twice:
            // - HKEY_CURRENT_USER -> HKEY_USERS\<SID>
            // - HKEY_CURRENT_CONFIG -> HKLM\SYSTEM\CurrentControlSet\Hardware Profiles\Current
            // - HKEY_CLASSES_ROOT -> [HKLM|HKCU]\Software\Classes

            var queue = new Queue<ManagedRegistryKey>([
                ManagedRegistryKey.Open("HKEY_LOCAL_MACHINE"),
                ManagedRegistryKey.Open("HKEY_USERS")
            ]);

            Traverse(queue, text);
        }

        public void Run(IEnumerable<string> args)
        {
            var it = args.GetEnumerator();
            if (!TryTake(it, out var mode))
            {
                UsageExit();
                return;
            }

            switch (mode)
            {
                case "--find":
                    RunFind(it);
                    break;
            }
        }

        static void Main(string[] args)
        {
            new Program().Run(args);
        }
    }
}
