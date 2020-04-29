using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	internal class UIHeader : UIElement
	{
		private string _text;

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				if (_text != value)
				{
					_text = value;
					Width.Precent = 0f;
					Height.Precent = 0f;
					Recalculate();
				}
			}
		}

		public UIHeader()
		{
			Text = "";
		}

		public UIHeader(string text)
		{
			Text = text;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			spriteBatch.DrawString(Main.fontDeathText, Text, new Vector2(dimensions.X, dimensions.Y), Color.White);
		}
	}
}
