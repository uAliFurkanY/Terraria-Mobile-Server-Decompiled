using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terraria.Graphics.Capture
{
	internal class CaptureManager
	{
		public static CaptureManager Instance = new CaptureManager();

		private CaptureInterface _interface;

		private CaptureCamera _camera;

		public bool IsCapturing => false;

		public bool Active
		{
			get
			{
				return _interface.Active;
			}
			set
			{
				if (!Main.CaptureModeDisabled && _interface.Active != value)
				{
					_interface.ToggleCamera(value);
				}
			}
		}

		public bool UsingMap
		{
			get
			{
				if (!Active)
				{
					return false;
				}
				return _interface.UsingMap();
			}
		}

		public CaptureManager()
		{
			_interface = new CaptureInterface();
		}

		public void Scrolling()
		{
			_interface.Scrolling();
		}

		public void Update()
		{
			_interface.Update();
		}

		public void Draw(SpriteBatch sb)
		{
			_interface.Draw(sb);
		}

		public float GetProgress()
		{
			return _camera.GetProgress();
		}

		public void Capture()
		{
			CaptureSettings captureSettings = new CaptureSettings();
			captureSettings.Area = new Rectangle(2660, 100, 1000, 1000);
			captureSettings.UseScaling = false;
			Capture(captureSettings);
		}

		public void Capture(CaptureSettings settings)
		{
			_camera.Capture(settings);
		}

		public void DrawTick()
		{
			_camera.DrawTick();
		}
	}
}
