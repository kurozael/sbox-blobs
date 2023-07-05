using Sandbox;
using Conna.LobbyNet;
using System;

namespace Conna.Blobs;

public class PlayerEntity : ModelEntity
{
	[LobbyNet.Input] public Vector3 MoveDirection { get; set; }
	[LobbyNet.Input] public Rotation LookDirection { get; set; }
	[LobbyNet.Input] public bool IsRunning { get; set; }
	[State] public Vector3 Velocity { get; set; }
	private bool WasHoldingSpace { get; set; }

	public override void BuildInput()
	{
		var direction = Vector3.Zero;

		if ( InputSystem.IsKeyDown( "a" ) )
			direction.y = 1f;
		else if ( InputSystem.IsKeyDown( "d" ) )
			direction.y = -1f;

		if ( InputSystem.IsKeyDown( "w" ) )
			direction.x = 1f;
		else if ( InputSystem.IsKeyDown( "s" ) )
			direction.x = -1f;

		var cameraRay = SceneRenderer.CurrentCamera.GetRay( Mouse.Position );
		var plane = new Plane( Vector3.Zero, Vector3.Up );
		var hitPosition = plane.Trace( cameraRay );

		if ( hitPosition.HasValue )
		{
			var aimDirection = (hitPosition.Value - Position).Normal.WithZ( 0f );
			LookDirection = Rotation.LookAt( aimDirection );
		}

		if ( WasHoldingSpace && !InputSystem.IsKeyDown( "space" ) )
		{
			CallRpc( "MyRpc", true, 1, 0, this );
		}

		IsRunning = InputSystem.IsKeyDown( "lshift" );
		MoveDirection = direction;
		WasHoldingSpace = InputSystem.IsKeyDown( "space" );
	}


	[Rpc( "MyRpc" )]
	public void RemoteCall( LobbyNet.Player player, bool a, int b, int c, LobbyNet.Entity d )
	{
		Log.Info( $"{player.Name} did a remote call {a}, {b}, {c}, {d}" );
	}

	public override void Simulate( bool isFirstTime )
	{
		var moveSpeed = IsRunning ? 70f : 24f;
		Velocity = Velocity.AddClamped( MoveDirection * moveSpeed, moveSpeed );
		Rotation = LookDirection;
		Position += Velocity * Network.FixedDeltaTime;
		Velocity *= 0.9f;
	}

	public override void Tick()
	{
		var forward = Rotation.Forward.Dot( Velocity );
		var sideward = Rotation.Right.Dot( Velocity );
		var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

		SceneObject.SetAnimParameter( "move_direction", angle );
		SceneObject.SetAnimParameter( "move_speed", Velocity.Length );
		SceneObject.SetAnimParameter( "move_groundspeed", Velocity.WithZ( 0 ).Length );
		SceneObject.SetAnimParameter( "move_y", sideward );
		SceneObject.SetAnimParameter( "move_x", forward );
		SceneObject.SetAnimParameter( "move_z", Velocity.z );

		SceneObject.SetAnimParameter( "wish_direction", angle );
		SceneObject.SetAnimParameter( "wish_speed", Velocity.Length );
		SceneObject.SetAnimParameter( "wish_groundspeed", Velocity.WithZ( 0 ).Length );
		SceneObject.SetAnimParameter( "wish_y", sideward );
		SceneObject.SetAnimParameter( "wish_x", forward );
		SceneObject.SetAnimParameter( "wish_z", Velocity.z );

		SceneObject.SetAnimParameter( "b_grounded", true );

		base.Tick();
	}
}
