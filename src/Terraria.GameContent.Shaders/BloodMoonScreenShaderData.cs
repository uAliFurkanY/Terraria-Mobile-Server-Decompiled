using Terraria.Graphics.Shaders;

namespace Terraria.GameContent.Shaders
{
	internal class BloodMoonScreenShaderData : ScreenShaderData
	{
		public BloodMoonScreenShaderData(string passName)
			: base(passName)
		{
		}

		public override void Apply()
		{
			float num = 1f - Utils.SmoothStep((float)Main.worldSurface + 50f, (float)Main.rockLayer + 100f, (Main.screenPosition.Y + (float)(Main.screenHeight / 2)) / 16f);
			UseOpacity(num * 0.75f);
			base.Apply();
		}
	}
}
