using Microsoft.Xna.Framework;

namespace Terraria.UI.Chat
{
	public class ChatLine
	{
		public Color color = Color.White;

		public int showTime;

		public string text = "";

		public TextSnippet[] parsedText = new TextSnippet[0];
	}
}
