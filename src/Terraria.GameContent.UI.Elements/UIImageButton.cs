using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	internal class UIImageButton : UIElement
	{
		private Texture2D _texture;

		private float _visibilityActive = 1f;

		private float _visibilityInactive = 0.4f;

		public UIImageButton(Texture2D texture)
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
			spriteBatch.Draw(position: GetDimensions().Position(), texture: _texture, color: Color.White * (base.IsMouseHovering ? _visibilityActive : _visibilityInactive));
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			Main.PlaySound(12);
		}

		public void SetVisibility(float whenActive, float whenInactive)
		{
			_visibilityActive = MathHelper.Clamp(whenActive, 0f, 1f);
			_visibilityInactive = MathHelper.Clamp(whenInactive, 0f, 1f);
		}
	}
}
