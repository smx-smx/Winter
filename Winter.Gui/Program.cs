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
using Photino.NET;
using PhotinoNET;
using PhotinoNET.Server;
using Smx.Winter.Gui.Services;
using Smx.Winter.Gui.WebControllers;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Smx.Winter.Gui;

class Program
{
    private const bool UseDevServerIfDebug = true;

    private static WebApplication CreateApiServer(PhotinoWindow mainWindow, string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Winter API", Version = "v1" });
            // Use method name as operationId
            c.CustomOperationIds(apiDesc => {
                return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
            });
        });

        builder.Services.AddSingleton<CbsSessionsRepository>();

        WinterFacade.BuildServices(builder.Services);

        const string apiCorsPolicy = "ApiCorsPolicy";

        builder.Services.AddCors(opts => {
            opts.AddPolicy(apiCorsPolicy, b => {
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

    private static void SetEnv_TrustedInstaller(){
        var winDir = Environment.GetEnvironmentVariable("WinDir");
        if(winDir == null) throw new InvalidOperationException();
        var bootDrive = Path.GetPathRoot(winDir);
        if(bootDrive == null) throw new InvalidOperationException();

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
        if(identity.User == null || identity.User.Value != TRUSTED_INSTALLER_SID){
            var thisProc = Process.GetCurrentProcess();
            if(thisProc == null) throw new InvalidOperationException();
            var mainMod = thisProc.MainModule;
            if(mainMod == null) throw new InvalidOperationException();

            new ElevationService().ImpersonateTrustedInstaller();
        }
        Console.WriteLine("Running as TrustedInstaller");
    }

    [STAThread]
    public static void Main(string[] args)
    {
        string windowTitle;
        var useDevServer = false;
        var isDebug = false;
        var devServer = new DevServer();
        var baseUrl = string.Empty;

#if DEBUG
        // mark a flag, that we are currently in debug mode.
        isDebug = true;
#endif

        var cts = new CancellationTokenSource();


        if (isDebug && UseDevServerIfDebug)
        {
            // if we are in debug mode, and the user wants to start the dev server, save a flag
            useDevServer = true;
        }

        if (useDevServer)
        {
            windowTitle = "My Application (Debug)";
            new Task(async () =>
            {
                try {
                    await devServer.Start();
                } catch(OperationCanceledException){
                    Console.WriteLine("DevServer stopped");
                }
            }, cts.Token).Start();

            // wait until we were able to read the dev-server url from the stdout of the npm run
            // so that we know were Photino should navigate.
            devServer.WaitUntilReady();
        } else {
            windowTitle = "My Application (Release)";
        }

        SetEnv_TrustedInstaller();

        if(!useDevServer)
        {
            // in release mode, we will need to create static file server, so that we can serve the static file different then index.html
            PhotinoServer
                .CreateStaticFileServer(args, out baseUrl)
                .RunAsync();
        }

        var window = new PhotinoWindow()
            .SetTitle(windowTitle)
            .SetUseOsDefaultSize(false)
            .SetSize(new Size(600, 400))
            .Center()
            .SetTopMost(true)
            .SetIconFile("wwwroot/favicon.ico")
            .SetResizable(true);


        var aspnetTask = new Task(async () => {
            WebApplication? webapp = default;
            try {
                webapp = CreateApiServer(window, args);
                await webapp.RunAsync(cts.Token);
            } catch(OperationCanceledException){
                Console.WriteLine("ApiServer stopped");
            }
        }, cts.Token);

        var shutdown = () => {
            Console.WriteLine("Closing");
            cts.Cancel();

            if (useDevServer)
            {
                // if the dev server was used, make sure to terminate it correctly, so 
                // that the used ports etc are freed
                devServer.Stop();
            }

            aspnetTask.Wait();
        };

        var handler = new PhotinoMessageHandler(window);

        if (useDevServer)
        {
            window.Load(devServer.GetUrl());
        }
        else
        {
            window.Load(baseUrl + "/index.html");
        }

        aspnetTask.Start();
        window.WaitForClose(); // Starts the application event loop
        shutdown();
    }
}
