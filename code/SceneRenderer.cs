using Sandbox;
using Sandbox.Menu;
using Conna.LobbyNet;
using System;
using Sandbox.UI;

namespace Conna.Blobs;

public class SceneRenderer : ScenePanel
{
	public override void Tick()
	{
		World = BlobsGame.World;

		Camera.AmbientLightColor = Color.White;
		Camera.BackgroundColor = Color.Black;
		Camera.Position = Vector3.Up * 300f;
		Camera.Rotation = Rotation.LookAt( Vector3.Down );
		Camera.FieldOfView = 60f;

		base.Tick();
	}
}
