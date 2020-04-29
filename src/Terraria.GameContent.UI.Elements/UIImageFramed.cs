using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	internal class UIImageFramed : UIElement
	{
		private Texture2D _texture;

		private Rectangle _frame;

		public UIImageFramed(Texture2D texture, Rectangle frame)
		{
			_texture = texture;
			_frame = frame;
			Width.Set(_frame.Width, 0f);
			Height.Set(_frame.Height, 0f);
		}

		public void SetImage(Texture2D texture, Rectangle frame)
		{
			_texture = texture;
			_frame = frame;
			Width.Set(_frame.Width, 0f);
			Height.Set(_frame.Height, 0f);
		}

		public void SetFrame(Rectangle frame)
		{
			_frame = frame;
			Width.Set(_frame.Width, 0f);
			Height.Set(_frame.Height, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(position: GetDimensions().Position(), texture: _texture, sourceRectangle: _frame, color: Color.White);
		}
	}
}
