using Microsoft.Xna.Framework;
using System.Globalization;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI.Chat
{
	internal class ColorTagHandler : ITagHandler
	{
		TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
		{
			TextSnippet textSnippet = new TextSnippet(text);
			int result;
			if (!int.TryParse(options, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result))
			{
				return textSnippet;
			}
			textSnippet.Color = new Color((result >> 16) & 0xFF, (result >> 8) & 0xFF, result & 0xFF);
			return textSnippet;
		}
	}
}
