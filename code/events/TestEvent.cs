using Conna.LobbyNet;
using Sandbox;

namespace Conna.Blobs;

public class TestEvent : LobbyNet.Event
{
	public string Message { get; set; }

	public override void Write( ref ByteStream stream )
	{
		stream.Write( Message );
	}

	public override void Read( ref ByteStream stream )
	{
		Message = stream.Read<string>();
	}
}
