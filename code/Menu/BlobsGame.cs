using Sandbox;
using Sandbox.Menu;
using Conna.LobbyNet;
using System;
using Sandbox.UI;

namespace Conna.Blobs;

public partial class BlobsGame
{
	public static BlobsGame Current { get; private set; }

	private static SceneWorld World { get; set; }
	private static SceneCamera Camera { get; set; }

	private TimeUntil NextRandomEntity;
	private LobbyNet.Entity TestEntity;

	internal static void OnButtonEvent( ButtonEvent e )
	{
		Log.Info( e.Button + " " + e.Pressed );

		if ( Network.IsHost && e.Button == "a" )
		{
			Current.TestEntity.Position -= Vector3.Left * 8f;
			Current.TestEntity.SomeInteger.SetValue( Current.TestEntity.SomeInteger.Value + 1 );
		}
	}

	public static BlobsGame Start( ILobby lobby )
	{
		Quit();
		Current = new BlobsGame();
		Current.OnStart( lobby );
		Log.Info( "BlobsGame Started" );
		return Current;
	}

	public static void Quit()
	{
		if ( Current == null ) return;
		Log.Info( "BlobsGame Quit" );
		Current.OnQuit();
		Current = null;
	}

	private void OnStart( ILobby lobby )
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
			Log.Info( "Made: " + TestEntity + " with id: " + TestEntity.NetworkIdent );
		}

		EventSystem.Subscribe<TestEvent>( OnTestEvent );
	}

	private void OnNetworkTick()
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

	private void OnTestEvent( TestEvent evnt )
	{
		Log.Info( "We got: " + evnt.Message + " from " + evnt.Sender.Name );
	}

	private void OnQuit()
	{
		World?.Delete();
		Camera = null;

		Network.Disconnect();
	}
}
