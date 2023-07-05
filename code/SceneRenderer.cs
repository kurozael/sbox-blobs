using Sandbox;
using Sandbox.Menu;
using Conna.LobbyNet;
using System;
using Sandbox.UI;
using System.Linq;

namespace Conna.Blobs;

public class SceneRenderer : ScenePanel
{
	public SceneRenderer()
	{
		BlobsGame.Camera = Camera;

		Camera.Position = Vector3.Up * 500f;
		Camera.AmbientLightColor = Color.White;
		Camera.BackgroundColor = Color.Black;
		Camera.Rotation = Rotation.LookAt( Vector3.Down );
		Camera.FieldOfView = 60f;
	}

	public override void Tick()
	{
		World = BlobsGame.World;
		base.Tick();
	}
}
