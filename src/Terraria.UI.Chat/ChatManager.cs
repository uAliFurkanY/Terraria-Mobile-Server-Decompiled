using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria.GameContent.UI.Chat;

namespace Terraria.UI.Chat
{
	public static class ChatManager
	{
		public static class Regexes
		{
			public static readonly Regex Format = new Regex("(?<!\\\\)\\[(?<tag>[a-zA-Z]{1,10})(\\/(?<options>[^:]+))?:(?<text>.+?)(?<!\\\\)\\]", RegexOptions.Compiled);
		}

		private static ConcurrentDictionary<string, ITagHandler> _handlers = new ConcurrentDictionary<string, ITagHandler>();

		public static readonly Vector2[] ShadowDirections = new Vector2[4]
		{
			-Vector2.UnitX,
			Vector2.UnitX,
			-Vector2.UnitY,
			Vector2.UnitY
		};

		public static Color WaveColor(Color color)
		{
			float num = (float)(int)Main.mouseTextColor / 255f;
			color = Color.Lerp(color, Color.Black, 1f - num);
			color.A = Main.mouseTextColor;
			return color;
		}

		public static void ConvertNormalSnippets(TextSnippet[] snippets)
		{
			for (int i = 0; i < snippets.Length; i++)
			{
				TextSnippet textSnippet = snippets[i];
				if (snippets[i].GetType() == typeof(TextSnippet))
				{
					PlainTagHandler.PlainSnippet plainSnippet = (PlainTagHandler.PlainSnippet)(snippets[i] = new PlainTagHandler.PlainSnippet(textSnippet.Text, textSnippet.Color, textSnippet.Scale));
				}
			}
		}

		public static void Register<T>(params string[] names) where T : ITagHandler, new()
		{
			T val = new T();
			for (int i = 0; i < names.Length; i++)
			{
				_handlers[names[i].ToLower()] = val;
			}
		}

		private static ITagHandler GetHandler(string tagName)
		{
			string key = tagName.ToLower();
			if (_handlers.ContainsKey(key))
			{
				return _handlers[key];
			}
			return null;
		}

		public static TextSnippet[] ParseMessage(string text, Color baseColor)
		{
			MatchCollection matchCollection = Regexes.Format.Matches(text);
			List<TextSnippet> list = new List<TextSnippet>();
			int num = 0;
			foreach (Match item in matchCollection)
			{
				if (item.Index > num)
				{
					list.Add(new TextSnippet(text.Substring(num, item.Index - num), baseColor));
				}
				num = item.Index + item.Length;
				string value = item.Groups["tag"].Value;
				string value2 = item.Groups["text"].Value;
				string value3 = item.Groups["options"].Value;
				ITagHandler handler = GetHandler(value);
				if (handler != null)
				{
					list.Add(handler.Parse(value2, baseColor, value3));
					list[list.Count - 1].TextOriginal = item.ToString();
				}
				else
				{
					list.Add(new TextSnippet(value2, baseColor));
				}
			}
			if (text.Length > num)
			{
				list.Add(new TextSnippet(text.Substring(num, text.Length - num), baseColor));
			}
			return list.ToArray();
		}

		public static bool AddChatText(SpriteFont font, string text, Vector2 baseScale)
		{
			int num = 470;
			num = Main.screenWidth - 330;
			if (GetStringSize(font, Main.chatText + text, baseScale).X > (float)num)
			{
				return false;
			}
			Main.chatText += text;
			return true;
		}

		public static Vector2 GetStringSize(SpriteFont font, string text, Vector2 baseScale, float maxWidth = -1f)
		{
			TextSnippet[] snippets = ParseMessage(text, Color.White);
			return GetStringSize(font, snippets, baseScale, maxWidth);
		}

		public static Vector2 GetStringSize(SpriteFont font, TextSnippet[] snippets, Vector2 baseScale, float maxWidth = -1f)
		{
			Vector2 vec = new Vector2(Main.mouseX, Main.mouseY);
			Vector2 zero = Vector2.Zero;
			Vector2 vector = zero;
			Vector2 result = vector;
			float x = font.MeasureString(" ").X;
			float num = 1f;
			float num2 = 0f;
			foreach (TextSnippet textSnippet in snippets)
			{
				textSnippet.Update();
				num = textSnippet.Scale;
				Vector2 size;
				if (textSnippet.UniqueDraw(true, out size, null))
				{
					vector.X += size.X * baseScale.X * num;
					result.X = Math.Max(result.X, vector.X);
					result.Y = Math.Max(result.Y, vector.Y + size.Y);
					continue;
				}
				string[] array = textSnippet.Text.Split('\n');
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split(' ');
					for (int k = 0; k < array3.Length; k++)
					{
						if (k != 0)
						{
							vector.X += x * baseScale.X * num;
						}
						if (maxWidth > 0f)
						{
							float num3 = font.MeasureString(array3[k]).X * baseScale.X * num;
							if (vector.X - zero.X + num3 > maxWidth)
							{
								vector.X = zero.X;
								vector.Y += (float)font.LineSpacing * num2 * baseScale.Y;
								result.Y = Math.Max(result.Y, vector.Y);
								num2 = 0f;
							}
						}
						if (num2 < num)
						{
							num2 = num;
						}
						Vector2 value = font.MeasureString(array3[k]);
						vec.Between(vector, vector + value);
						vector.X += value.X * baseScale.X * num;
						result.X = Math.Max(result.X, vector.X);
						result.Y = Math.Max(result.Y, vector.Y + value.Y);
					}
					if (array.Length > 1)
					{
						vector.X = zero.X;
						vector.Y += (float)font.LineSpacing * num2 * baseScale.Y;
						result.Y = Math.Max(result.Y, vector.Y);
						num2 = 0f;
					}
				}
			}
			return result;
		}

		public static void DrawColorCodedStringShadow(SpriteBatch spriteBatch, SpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
		{
			for (int i = 0; i < ShadowDirections.Length; i++)
			{
				int hoveredSnippet;
				DrawColorCodedString(spriteBatch, font, snippets, position + ShadowDirections[i] * spread, baseColor, rotation, origin, baseScale, out hoveredSnippet, maxWidth, true);
			}
		}

		public static Vector2 DrawColorCodedString(SpriteBatch spriteBatch, SpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth, bool ignoreColors = false)
		{
			int num = -1;
			Vector2 vec = new Vector2(Main.mouseX, Main.mouseY);
			Vector2 vector = position;
			Vector2 result = vector;
			float x = font.MeasureString(" ").X;
			Color color = baseColor;
			float num2 = 1f;
			float num3 = 0f;
			for (int i = 0; i < snippets.Length; i++)
			{
				TextSnippet textSnippet = snippets[i];
				textSnippet.Update();
				if (!ignoreColors)
				{
					color = textSnippet.GetVisibleColor();
				}
				num2 = textSnippet.Scale;
				Vector2 size;
				if (textSnippet.UniqueDraw(false, out size, spriteBatch, vector, color, num2))
				{
					if (vec.Between(vector, vector + size))
					{
						num = i;
					}
					vector.X += size.X * baseScale.X * num2;
					result.X = Math.Max(result.X, vector.X);
					continue;
				}
				string[] array = textSnippet.Text.Split('\n');
				string[] array2 = array;
				foreach (string text in array2)
				{
					string[] array3 = text.Split(' ');
					for (int k = 0; k < array3.Length; k++)
					{
						if (k != 0)
						{
							vector.X += x * baseScale.X * num2;
						}
						if (maxWidth > 0f)
						{
							float num4 = font.MeasureString(array3[k]).X * baseScale.X * num2;
							if (vector.X - position.X + num4 > maxWidth)
							{
								vector.X = position.X;
								vector.Y += (float)font.LineSpacing * num3 * baseScale.Y;
								result.Y = Math.Max(result.Y, vector.Y);
								num3 = 0f;
							}
						}
						if (num3 < num2)
						{
							num3 = num2;
						}
						spriteBatch.DrawString(font, array3[k], vector, color, rotation, origin, baseScale * textSnippet.Scale * num2, SpriteEffects.None, 0f);
						Vector2 value = font.MeasureString(array3[k]);
						if (vec.Between(vector, vector + value))
						{
							num = i;
						}
						vector.X += value.X * baseScale.X * num2;
						result.X = Math.Max(result.X, vector.X);
					}
					if (array.Length > 1)
					{
						vector.Y += (float)font.LineSpacing * num3 * baseScale.Y;
						vector.X = position.X;
						result.Y = Math.Max(result.Y, vector.Y);
						num3 = 0f;
					}
				}
			}
			hoveredSnippet = num;
			return result;
		}

		public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, TextSnippet[] snippets, Vector2 position, float rotation, Vector2 origin, Vector2 baseScale, out int hoveredSnippet, float maxWidth = -1f, float spread = 2f)
		{
			DrawColorCodedStringShadow(spriteBatch, font, snippets, position, Color.Black, rotation, origin, baseScale, maxWidth, spread);
			return DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, rotation, origin, baseScale, out hoveredSnippet, maxWidth);
		}

		public static void DrawColorCodedStringShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
		{
			for (int i = 0; i < ShadowDirections.Length; i++)
			{
				DrawColorCodedString(spriteBatch, font, text, position + ShadowDirections[i] * spread, baseColor, rotation, origin, baseScale, maxWidth, true);
			}
		}

		public static Vector2 DrawColorCodedString(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, bool ignoreColors = false)
		{
			Vector2 vector = position;
			Vector2 result = vector;
			string[] array = text.Split('\n');
			float x = font.MeasureString(" ").X;
			Color color = baseColor;
			float num = 1f;
			float num2 = 0f;
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string[] array3 = text2.Split(':');
				string[] array4 = array3;
				foreach (string text3 in array4)
				{
					if (text3.StartsWith("sss"))
					{
						if (text3.StartsWith("sss1"))
						{
							if (!ignoreColors)
							{
								color = Color.Red;
							}
						}
						else if (text3.StartsWith("sss2"))
						{
							if (!ignoreColors)
							{
								color = Color.Blue;
							}
						}
						else if (text3.StartsWith("sssr") && !ignoreColors)
						{
							color = Color.White;
						}
						continue;
					}
					string[] array5 = text3.Split(' ');
					for (int k = 0; k < array5.Length; k++)
					{
						if (k != 0)
						{
							vector.X += x * baseScale.X * num;
						}
						if (maxWidth > 0f)
						{
							float num3 = font.MeasureString(array5[k]).X * baseScale.X * num;
							if (vector.X - position.X + num3 > maxWidth)
							{
								vector.X = position.X;
								vector.Y += (float)font.LineSpacing * num2 * baseScale.Y;
								result.Y = Math.Max(result.Y, vector.Y);
								num2 = 0f;
							}
						}
						if (num2 < num)
						{
							num2 = num;
						}
						spriteBatch.DrawString(font, array5[k], vector, color, rotation, origin, baseScale * num, SpriteEffects.None, 0f);
						vector.X += font.MeasureString(array5[k]).X * baseScale.X * num;
						result.X = Math.Max(result.X, vector.X);
					}
				}
				vector.X = position.X;
				vector.Y += (float)font.LineSpacing * num2 * baseScale.Y;
				result.Y = Math.Max(result.Y, vector.Y);
				num2 = 0f;
			}
			return result;
		}

		public static Vector2 DrawColorCodedStringWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth = -1f, float spread = 2f)
		{
			TextSnippet[] snippets = ParseMessage(text, baseColor);
			ConvertNormalSnippets(snippets);
			DrawColorCodedStringShadow(spriteBatch, font, snippets, position, Color.Black, rotation, origin, baseScale, maxWidth, spread);
			int hoveredSnippet;
			return DrawColorCodedString(spriteBatch, font, snippets, position, Color.White, rotation, origin, baseScale, out hoveredSnippet, maxWidth);
		}
	}
}
