using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Smx.Winter;
using Smx.Winter.Gui.Controllers;
using Smx.Winter.Gui.Models;
using Smx.Winter.Gui.Services;

namespace Smx.Winter.Gui.WebControllers;

[ApiController]
[Route("api/[controller]")]
public class WinterController : ControllerBase
{
    private readonly CbsSessionsRepository _sessions;
    private readonly ILogger<WinterController> _logger;

    public WinterController(ILogger<WinterController> logger, CbsSessionsRepository sessions)
    {
        _logger = logger;
        _sessions = sessions;
    }

    [HttpPost("startCbsSession")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult StartCbsSession([FromBody] StartCbsSessionCommand cmd)
    {
        var sess = new WinterSession(cmd);
        _sessions.Add(sess.SessionId, sess);
        return Ok(sess.SessionId.ToString());
    }

    [HttpPost("getComponents")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    public IActionResult GetComponents([FromBody] string guidStr)
    {
        var guid = Guid.Parse(guidStr);
        if (!_sessions.TryGetSession(guid, out var session))
        {
            return NotFound();
        }
        return Ok(session.GetPackages());
    }
}
