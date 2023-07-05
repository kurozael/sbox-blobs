using Sandbox;
using Sandbox.Menu;
using Conna.LobbyNet;
using System;
using Sandbox.UI;
using System.Linq;

namespace Conna.Blobs;

public static class BlobsGame
{
	public static SceneWorld World { get; private set; }
	public static SceneCamera Camera { get; internal set; }

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
		Network.OnPlayerDisconnect += OnPlayerDisconnect;

		if ( Network.IsHost )
		{
			var entity = EntitySystem.Create<PlayerEntity>();
			entity.ModelName.Value = "models/citizen/citizen.vmdl";
			entity.GiveControl( Network.Host );
		}
	}

	private static void OnPlayerDisconnect( LobbyNet.Player player )
	{
		var entities = EntitySystem.All.Where( e => e.IsController( player ) );

		foreach ( var e in entities )
		{
			EntitySystem.Destroy( e );
		}
	}

	private static void OnPlayerConnect( LobbyNet.Player player )
	{
		var entity = EntitySystem.Create<PlayerEntity>();
		entity.ModelName.Value = "models/citizen/citizen.vmdl";
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
		if ( Network.LocalPlayer.IsValid() )
		{
			var controlledEntity = EntitySystem.All.FirstOrDefault( e => e.IsController( Network.LocalPlayer ) );

			if ( controlledEntity.IsValid() )
			{
				Camera.Position = Camera.Position.LerpTo( controlledEntity.Position + Vector3.Up * 500f, Network.FixedDeltaTime * 4f );
			}
		}
	}
}
