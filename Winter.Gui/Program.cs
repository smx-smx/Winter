using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Security.Principal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Photino.Blazor;
using Photino.NET;
using PhotinoNET;
using PhotinoNET.Server;
using Radzen;
using Smx.Winter.Gui.Services;
using Smx.Winter.Gui.WebControllers;
using Swashbuckle.AspNetCore.SwaggerGen;
using Windows.Win32;

namespace Smx.Winter.Gui;

class Program
{
    private const bool UseDevServerIfDebug = true;

    private static WebApplication CreateApiServer(PhotinoWindow mainWindow, string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Winter API", Version = "v1" });
            // Use method name as operationId
            c.CustomOperationIds(apiDesc =>
            {
                return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
            });
        });

        builder.Services.AddSingleton<CbsSessionsRepository>();

        WinterFacade.BuildServices(builder.Services);

        const string apiCorsPolicy = "ApiCorsPolicy";

        builder.Services.AddCors(opts =>
        {
            opts.AddPolicy(apiCorsPolicy, b =>
            {
                b.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services.AddSingleton<PhotinoWindow>(mainWindow);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        var facade = new WinterFacade(app.Services);
        facade.Initialize();

        app.UseCors(apiCorsPolicy);
        app.MapControllers();
        app.UseStaticFiles();
        return app;
    }

    private const string TRUSTED_INSTALLER_SID = "S-1–5–80–956008885–3418522649–1831038044–1853292631–2271478464";

    private static void SetEnv_TrustedInstaller(string[] args)
    {
        var winDir = Environment.GetEnvironmentVariable("WinDir");
        if (winDir == null) throw new InvalidOperationException();
        var bootDrive = Path.GetPathRoot(winDir);
        if (bootDrive == null) throw new InvalidOperationException();

        var tmpPath = Path.Combine(bootDrive, "TEMP", "TrustedInstaller");
        var tmpPath_AppData = Path.Combine(tmpPath, "AppData");
        var tmpPath_AppData_Roaming = Path.Combine(tmpPath_AppData, "Roaming");
        var tmpPath_AppData_Local = Path.Combine(tmpPath_AppData, "Local");
        var tmpPath_Desktop = Path.Combine(tmpPath, "Desktop");

        Directory.CreateDirectory(tmpPath);
        Directory.CreateDirectory(tmpPath_AppData_Roaming);
        Directory.CreateDirectory(tmpPath_AppData_Local);
        Directory.CreateDirectory(tmpPath_Desktop);

        Console.WriteLine("USERPROFILE: " + Environment.GetEnvironmentVariable("USERPROFILE"));
        Console.WriteLine("HOMEPATH: " + Environment.GetEnvironmentVariable("HOMEPATH"));
        Console.WriteLine("APPDATA: " + Environment.GetEnvironmentVariable("APPDATA"));
        Console.WriteLine("LOCALAPPDATA: " + Environment.GetEnvironmentVariable("LOCALAPPDATA"));

        Environment.SetEnvironmentVariable("USERPROFILE", tmpPath);
        Environment.SetEnvironmentVariable("HOMEPATH", tmpPath);
        Environment.SetEnvironmentVariable("APPDATA", tmpPath_AppData_Roaming);
        Environment.SetEnvironmentVariable("LOCALAPPDATA", tmpPath_AppData_Local);

        var identity = WindowsIdentity.GetCurrent();
        Console.WriteLine($"User: {identity.Name}");
        Console.WriteLine($"SID: {identity.User?.Value}");
        if (identity.User == null || !identity.IsSystem)
        {
            var thisProc = Process.GetCurrentProcess();
            if (thisProc == null) throw new InvalidOperationException();
            var mainMod = thisProc.MainModule;
            if (mainMod == null) throw new InvalidOperationException();

            var self = Process.GetCurrentProcess()?.MainModule?.FileName;
            if (self == null) throw new InvalidOperationException();
            var argv = new List<string>
            {
                self
            };
            argv.AddRange(args);
            Thread.Sleep(1000);
            var proc = new ElevationService().RunAsTrustedInstaller(argv.ToArray());
            proc.WaitForExit();
            Environment.Exit(0);
        }
        Console.WriteLine("Running as TrustedInstaller");
    }

    [STAThread]
    public static void Main(string[] args)
    {
#if false
        if (WindowsIdentity.GetCurrent().IsSystem)
        {
            if (!Debugger.IsAttached)
            {
                if (Debugger.Launch())
                {
                    while (!Debugger.IsAttached) Thread.Sleep(200);
                }
            }
        }
#endif

        SetEnv_TrustedInstaller(args);

        var cts = new CancellationTokenSource();

        var builder = PhotinoBlazorAppBuilder.CreateDefault(args);
        builder.Services.AddLogging();
        builder.RootComponents.Add<App>("app");

        builder.Services.AddRadzenComponents();
        builder.Services.AddRadzenQueryStringThemeService();
        var app = builder.Build();

        app.MainWindow
            .SetIconFile("favicon.ico")
            .SetTitle("Winter");

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        app.Run();

        Console.WriteLine("Closing");
        cts.Cancel();
    }
}
