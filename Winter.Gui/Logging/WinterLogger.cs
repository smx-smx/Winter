using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Smx.Winter.Gui.Logging;

public sealed class WinterLogEntry
{
    public DateTime Timestamp { get; init; }
    public LogLevel Level { get; init; }
    public string Category { get; init; } = "";
    public string Message { get; init; } = "";
    public string? Exception { get; init; }
}

public sealed class WinterLogStore
{
    private const int MaxEntries = 2000;
    private readonly ConcurrentQueue<WinterLogEntry> _entries = new();

    public event Action? OnNewEntry;

    public IReadOnlyCollection<WinterLogEntry> Entries => _entries;

    internal void Add(WinterLogEntry entry)
    {
        _entries.Enqueue(entry);
        while (_entries.Count > MaxEntries)
            _entries.TryDequeue(out _);
        OnNewEntry?.Invoke();
    }

    public void Clear()
    {
        while (_entries.TryDequeue(out _)) { }
    }
}

[ProviderAlias("Winter")]
public sealed class WinterFileLoggerProvider : ILoggerProvider
{
    private readonly string _logDir;
    private readonly WinterLogStore _logStore;
    private readonly string _logFilePath;
    private readonly StreamWriter _writer;
    private readonly object _lock = new();
    private bool _disposed;

    public WinterLogStore Store => _logStore;

    public WinterFileLoggerProvider(string? logDir = null)
    {
        _logDir = logDir ?? Path.GetDirectoryName(
            System.Reflection.Assembly.GetEntryAssembly()?.Location
            ?? Environment.ProcessPath
            ?? AppContext.BaseDirectory) ?? ".";
        Directory.CreateDirectory(_logDir);

        var date = DateTime.Now.ToString("yyyy-MM-dd");
        _logFilePath = Path.Combine(_logDir, $"winter-{date}.log");
        _logStore = new WinterLogStore();

        _writer = new StreamWriter(_logFilePath, true) { AutoFlush = false };
    }

    public string LogDirectory => _logDir;
    public string LogFilePath => _logFilePath;

    public ILogger CreateLogger(string categoryName)
        => new WinterLogger(categoryName, this);

    internal void Write(LogLevel logLevel, string categoryName, string message, Exception? exception)
    {
        var entry = new WinterLogEntry
        {
            Timestamp = DateTime.Now,
            Level = logLevel,
            Category = categoryName,
            Message = message,
            Exception = exception?.ToString()
        };
        _logStore.Add(entry);

        var levelStr = logLevel switch
        {
            LogLevel.Trace => "TRCE",
            LogLevel.Debug => "DBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERRO",
            LogLevel.Critical => "CRIT",
            _ => "????"
        };
        var line = $"{entry.Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{levelStr}] {categoryName}: {message}";
        if (entry.Exception != null)
            line += $"\n{entry.Exception}";

        lock (_lock)
        {
            try
            {
                _writer.WriteLine(line);
            }
            catch
            {
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        try
        {
            lock (_lock)
            {
                _writer.Flush();
                _writer.Dispose();
            }
        }
        catch { }
    }
}

internal sealed class WinterLogger : ILogger
{
    private readonly string _categoryName;
    private readonly WinterFileLoggerProvider _provider;

    public WinterLogger(string categoryName, WinterFileLoggerProvider provider)
    {
        _categoryName = categoryName;
        _provider = provider;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        // Also write to debug output for IDE visibility
        System.Diagnostics.Debug.WriteLine($"[{logLevel}] {_categoryName}: {message}");
        if (exception != null)
            System.Diagnostics.Debug.WriteLine(exception.ToString());
        _provider.Write(logLevel, _categoryName, message, exception);
    }
}
