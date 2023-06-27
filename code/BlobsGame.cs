using Sandbox;
using Sandbox.Menu;
using Conna.LobbyNet;
using System;
using Sandbox.UI;

namespace Conna.Blobs;

public static class BlobsGame
{
	private static SceneWorld World { get; set; }
	private static SceneCamera Camera { get; set; }

	private static TimeUntil NextRandomEntity;
	private static LobbyNet.Entity TestEntity;
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
		Network.Initialize( lobby );
		Network.OnTick += OnNetworkTick;

		World = new SceneWorld();
		World.AmbientLightColor = Color.White;

		Camera = new SceneCamera();
		Camera.World = World;

		var obj = new SceneModel( World, "models/sbox_props/aircon_unit_large/aircon_unit_large_128x64_a.vmdl", new Transform() );
		obj.Position = Vector3.Zero;

		Camera.AmbientLightColor = Color.White;
		Camera.Position = obj.Position + Vector3.Up * 300f;
		Camera.Rotation = Rotation.LookAt( Vector3.Down );

		if ( Network.IsHost )
		{
			TestEntity = EntitySystem.Create<LobbyNet.Entity>();
			TestEntity.GiveControl( Network.Host );
			Log.Info( "Made: " + TestEntity + " with id: " + TestEntity.NetworkIdent );
		}

		EventSystem.Subscribe<TestEvent>( OnTestEvent );
	}

	private static void OnQuit()
	{
		World?.Delete();
		Camera = null;

		Network.Disconnect();
		Network.OnTick -= OnNetworkTick;
	}

	private static void OnNetworkTick()
	{
		if ( NextRandomEntity )
		{
			if ( Network.IsHost )
			{
				Log.Info( "We're the host" );
				var evnt = new TestEvent();
				evnt.Message = "What's up, dawg " + Game.Random.Int( 1, 10 );
				EventSystem.Send( evnt );
			}

			NextRandomEntity = 10f;
		}
	}

	private static void OnTestEvent( TestEvent evnt )
	{
		Log.Info( "We got: " + evnt.Message + " from " + evnt.Sender.Name );
	}
}
