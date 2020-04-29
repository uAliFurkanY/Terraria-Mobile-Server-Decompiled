using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	internal class UIImage : UIElement
	{
		private Texture2D _texture;

		public UIImage(Texture2D texture)
		{
			_texture = texture;
			Width.Set(_texture.Width, 0f);
			Height.Set(_texture.Height, 0f);
		}

		public void SetImage(Texture2D texture)
		{
			_texture = texture;
			Width.Set(_texture.Width, 0f);
			Height.Set(_texture.Height, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(position: GetDimensions().Position(), texture: _texture, color: Color.White);
		}
	}
}
