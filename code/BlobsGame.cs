using Sandbox;
using Sandbox.Menu;
using Conna.LobbyNet;
using System;
using Sandbox.UI;

namespace Conna.Blobs;

public static class BlobsGame
{
	public static SceneWorld World { get; private set; }

	private static bool IsRunning { get; set; }

	internal static void OnButtonEvent( ButtonEvent e )
	{
		InputSystem.OnButtonEvent( e );
	}

	public static void Start( ILobby lobby )
	{
		Quit();
		OnStart( lobby );
		Log.Info( "BlobsGame Started" );
		IsRunning = true;
	}

	public static void Quit()
	{
		if ( !IsRunning ) return;
		Log.Info( "BlobsGame Quit" );
		OnQuit();
	}

	private static void OnStart( ILobby lobby )
	{
		World = new SceneWorld();

		Network.Initialize( lobby );
		Network.OnTick += OnNetworkTick;
		Network.OnPlayerConnect += OnPlayerConnect;

		if ( Network.IsHost )
		{
			var entity = EntitySystem.Create<ModelEntity>();
			entity.ModelName.Value = "models/citizen_props/crate01.vmdl";
			entity.GiveControl( Network.Host );
		}
	}

	private static void OnPlayerConnect( LobbyNet.Player player )
	{
		var entity = EntitySystem.Create<ModelEntity>();
		entity.ModelName.Value = "models/citizen_props/crate01.vmdl";
		entity.GiveControl( player );
	}

	private static void OnQuit()
	{
		World?.Delete();

		Network.Disconnect();
		Network.OnTick -= OnNetworkTick;
	}

	private static void OnNetworkTick()
	{
		
	}
}
