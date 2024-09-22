using System.Text.Json;
using Photino.NET;
using Smx.Winter.Gui.Models;

namespace Smx.Winter.Gui;



public class PhotinoMessageHandler {
	private JsonSerializerOptions _opts;
	public PhotinoMessageHandler(PhotinoWindow window){
		_opts = new JsonSerializerOptions {
			PropertyNamingPolicy = null
		};

		window.RegisterWebMessageReceivedHandler(HandleMessage);
	}

	public void HandleMessage(object? sender, string message){
		ArgumentNullException.ThrowIfNull(sender);
		if(!(sender is PhotinoWindow window)) throw new InvalidOperationException("Invalid sender");


		var cmd = JsonSerializer.Deserialize<PhotinoCommand>(message);
		if(cmd == null) throw new InvalidDataException("invalid JSON message");
		
		switch(cmd){
			case OpenDirectoryCommand dirCmd:
				window.ShowOpenFolder(dirCmd.Title);
				break;
		}
	}
}
