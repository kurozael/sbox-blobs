using Sandbox;
using Conna.LobbyNet;

namespace Conna.Blobs;

public class ModelEntity : LobbyNet.Entity
{
	public SyncString ModelName { get; set; } = new( string.Empty );

	[LobbyNet.Input] public bool IsLeftPressed { get; set; }
	[LobbyNet.Input] public bool IsRightPressed { get; set; }

	private SceneModel SceneObject { get; set; }

	public ModelEntity() : base()
	{
		ModelName.OnChanged += OnModelNameChanged;
	}

	public override void OnSpawn()
	{
		UpdateSceneObject();
		base.OnSpawn();
	}

	public override void BuildInput()
	{
		IsLeftPressed = InputSystem.IsKeyDown( "a" );
		IsRightPressed = InputSystem.IsKeyDown( "d" );
	}

	public override void Simulate( bool isFirstTime )
	{
		if ( IsLeftPressed )
		{
			Position += Vector3.Left * Network.FixedDeltaTime * 4f;
		}

		if ( IsRightPressed )
		{
			Position += Vector3.Right * Network.FixedDeltaTime * 4f;
		}
	}

	public override void OnDestroy()
	{
		SceneObject?.Delete();
		SceneObject = null;

		base.OnDestroy();
	}

	public override void Tick()
	{
		if ( SceneObject.IsValid() )
		{
			SceneObject.Transform = Transform;

			// Conna: I don't understand why I have to do this for scale to work.
			var tx = SceneObject.Transform;
			tx.Scale = Transform.Scale;
			SceneObject.Transform = tx;
		}
		
		base.Tick();
	}

	private void UpdateSceneObject()
	{
		if ( string.IsNullOrEmpty( ModelName.Value ) )
		{
			SceneObject?.Delete();
			SceneObject = null;

			return;
		}

		Log.Info( "Model Name Changed To: " + ModelName.Value );

		if ( !SceneObject.IsValid() )
		{
			SceneObject = new SceneModel( BlobsGame.World, ModelName.Value, Transform );
		}

		SceneObject.Model = Model.Load( ModelName.Value );
	}

	private void OnModelNameChanged()
	{
		UpdateSceneObject();
	}
}
