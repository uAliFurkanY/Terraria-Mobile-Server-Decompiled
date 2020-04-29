using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	internal class UITextPanel : UIPanel
	{
		private string _text = "";

		private float _textScale = 1f;

		private Vector2 _textSize = Vector2.Zero;

		private bool _isLarge;

		public UITextPanel(string text, float textScale = 1f, bool large = false)
		{
			SetText(text, textScale, large);
		}

		public override void Recalculate()
		{
			SetText(_text, _textScale, _isLarge);
			base.Recalculate();
		}

		public void SetText(string text, float textScale, bool large)
		{
			SpriteFont spriteFont = large ? Main.fontDeathText : Main.fontMouseText;
			Vector2 textSize = new Vector2(spriteFont.MeasureString(text).X, large ? 32f : 16f) * textScale;
			_text = text;
			_textScale = textScale;
			_textSize = textSize;
			_isLarge = large;
			MinWidth.Set(textSize.X + PaddingLeft + PaddingRight, 0f);
			MinHeight.Set(textSize.Y + PaddingTop + PaddingBottom, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle ınnerDimensions = GetInnerDimensions();
			Vector2 pos = ınnerDimensions.Position();
			if (_isLarge)
			{
				pos.Y -= 10f * _textScale;
			}
			else
			{
				pos.Y -= 2f * _textScale;
			}
			pos.X += (ınnerDimensions.Width - _textSize.X) * 0.5f;
			if (_isLarge)
			{
				Utils.DrawBorderStringBig(spriteBatch, _text, pos, Color.White, _textScale);
			}
			else
			{
				Utils.DrawBorderString(spriteBatch, _text, pos, Color.White, _textScale);
			}
		}
	}
}
