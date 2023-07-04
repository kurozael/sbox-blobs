using Sandbox;
using Conna.LobbyNet;

namespace Conna.Blobs;

public class ModelEntity : LobbyNet.Entity
{
	public SyncString ModelName { get; set; } = new( string.Empty );
	public SceneModel SceneObject { get; private set; }

	public ModelEntity() : base()
	{
		ModelName.OnChanged += OnModelNameChanged;
	}

	public override void OnSpawn()
	{
		UpdateSceneObject();
		base.OnSpawn();
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
			SceneObject.Update( Network.FixedDeltaTime );
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
		Log.Info( "We Made The Entity And Add It To: " + BlobsGame.World );

		if ( !SceneObject.IsValid() )
		{
			SceneObject = new SceneModel( BlobsGame.World, ModelName.Value, new Transform( Position, Rotation ) );
		}

		SceneObject.Model = Model.Load( ModelName.Value );
	}

	private void OnModelNameChanged()
	{
		UpdateSceneObject();
	}
}
