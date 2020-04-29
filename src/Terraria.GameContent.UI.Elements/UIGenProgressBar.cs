using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	internal class UIGenProgressBar : UIElement
	{
		private Texture2D _texInnerDirt;

		private Texture2D _texOuterCrimson;

		private Texture2D _texOuterCorrupt;

		private Texture2D _texOuterLower;

		private float _visualOverallProgress;

		private float _targetOverallProgress;

		private float _visualCurrentProgress;

		private float _targetCurrentProgress;

		public UIGenProgressBar()
		{
			if (Main.netMode != 2)
			{
				_texInnerDirt = TextureManager.Load("Images/UI/WorldGen/Outer Dirt");
				_texOuterCorrupt = TextureManager.Load("Images/UI/WorldGen/Outer Corrupt");
				_texOuterCrimson = TextureManager.Load("Images/UI/WorldGen/Outer Crimson");
				_texOuterLower = TextureManager.Load("Images/UI/WorldGen/Outer Lower");
			}
			Recalculate();
		}

		public override void Recalculate()
		{
			Width.Precent = 0f;
			Height.Precent = 0f;
			Width.Pixels = 612f;
			Height.Pixels = 70f;
			base.Recalculate();
		}

		public void SetProgress(float overallProgress, float currentProgress)
		{
			_targetCurrentProgress = currentProgress;
			_targetOverallProgress = overallProgress;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			_visualOverallProgress = _targetOverallProgress;
			_visualCurrentProgress = _targetCurrentProgress;
			CalculatedStyle dimensions = GetDimensions();
			int completedWidth = (int)(_visualOverallProgress * 504f);
			int completedWidth2 = (int)(_visualCurrentProgress * 504f);
			Vector2 value = new Vector2(dimensions.X, dimensions.Y);
			Color color = default(Color);
			color.PackedValue = (WorldGen.crimson ? 4286836223u : 4283888223u);
			DrawFilling2(spriteBatch, value + new Vector2(20f, 40f), 16, completedWidth, 564, color, Color.Lerp(color, Color.Black, 0.5f), new Color(48, 48, 48));
			color.PackedValue = 4290947159u;
			DrawFilling2(spriteBatch, value + new Vector2(50f, 60f), 8, completedWidth2, 504, color, Color.Lerp(color, Color.Black, 0.5f), new Color(33, 33, 33));
			Rectangle r = GetDimensions().ToRectangle();
			r.X -= 8;
			spriteBatch.Draw(WorldGen.crimson ? _texOuterCrimson : _texOuterCorrupt, r.TopLeft(), Color.White);
			spriteBatch.Draw(_texOuterLower, r.TopLeft() + new Vector2(44f, 60f), Color.White);
		}

		private void DrawFilling(SpriteBatch spritebatch, Texture2D tex, Texture2D texShadow, Vector2 topLeft, int completedWidth, int totalWidth, Color separator, Color empty)
		{
			if (completedWidth % 2 != 0)
			{
				completedWidth--;
			}
			Vector2 position = topLeft + completedWidth * Vector2.UnitX;
			int num = completedWidth;
			Rectangle value = tex.Frame();
			while (num > 0)
			{
				if (value.Width > num)
				{
					value.X += value.Width - num;
					value.Width = num;
				}
				spritebatch.Draw(tex, position, value, Color.White, 0f, new Vector2(value.Width, 0f), 1f, SpriteEffects.None, 0f);
				position.X -= value.Width;
				num -= value.Width;
			}
			if (texShadow != null)
			{
				spritebatch.Draw(texShadow, topLeft, new Rectangle(0, 0, completedWidth, texShadow.Height), Color.White);
			}
			spritebatch.Draw(Main.magicPixel, new Rectangle((int)topLeft.X + completedWidth, (int)topLeft.Y, totalWidth - completedWidth, tex.Height), new Rectangle(0, 0, 1, 1), empty);
			spritebatch.Draw(Main.magicPixel, new Rectangle((int)topLeft.X + completedWidth - 2, (int)topLeft.Y, 2, tex.Height), new Rectangle(0, 0, 1, 1), separator);
		}

		private void DrawFilling2(SpriteBatch spritebatch, Vector2 topLeft, int height, int completedWidth, int totalWidth, Color filled, Color separator, Color empty)
		{
			if (completedWidth % 2 != 0)
			{
				completedWidth--;
			}
			spritebatch.Draw(Main.magicPixel, new Rectangle((int)topLeft.X, (int)topLeft.Y, completedWidth, height), new Rectangle(0, 0, 1, 1), filled);
			spritebatch.Draw(Main.magicPixel, new Rectangle((int)topLeft.X + completedWidth, (int)topLeft.Y, totalWidth - completedWidth, height), new Rectangle(0, 0, 1, 1), empty);
			spritebatch.Draw(Main.magicPixel, new Rectangle((int)topLeft.X + completedWidth - 2, (int)topLeft.Y, 2, height), new Rectangle(0, 0, 1, 1), separator);
		}
	}
}
