using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photino.NET;
using Swashbuckle.AspNetCore.Annotations;

namespace Smx.Winter.Gui.WebControllers;

[ApiController]
[Route("api/[controller]")]
public class PhotinoController : ControllerBase
{
    private readonly ILogger<PhotinoController> _logger;
    private readonly PhotinoWindow _mainWindow;

    public PhotinoController(
        PhotinoWindow mainWindow,
        ILogger<PhotinoController> logger
    )
    {
        _logger = logger;
        _mainWindow = mainWindow;
    }

    [HttpPost]
    public string? GetRootPath([FromBody] string path)
    {
        return Path.GetPathRoot(path);
    }

    [HttpGet]
    public string? ShowOpenFolder(
        [FromQuery] string title
    )
    {
        var res = _mainWindow.ShowOpenFolder(title);
        return res.FirstOrDefault();
    }
}
