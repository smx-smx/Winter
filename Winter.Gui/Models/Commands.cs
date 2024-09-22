using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Smx.Winter.Gui.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "CommandType")]
[JsonDerivedType(typeof(StartCbsSessionCommand), "StartCbsSession")]
[JsonDerivedType(typeof(OpenDirectoryCommand), "OpenDirectory")]
public class PhotinoCommand {}

public class StartCbsSessionCommand : PhotinoCommand {
    public required string BootDrive { get; set; }
    public required string WinDir { get; set; }
}

public class OpenDirectoryCommand : PhotinoCommand {
    public string Title { get; set; } = string.Empty;
}
