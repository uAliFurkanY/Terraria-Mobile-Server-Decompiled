using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Terraria.UI.Chat;

namespace Terraria.Graphics.Capture
{
	internal class CaptureInterface
	{
		public static class Settings
		{
			public static bool PackImage = true;

			public static bool IncludeEntities = true;

			public static bool TransparentBackground = false;

			public static int BiomeChoice = 0;

			public static int ScreenAnchor = 0;

			public static Color MarkedAreaColor = new Color(0.8f, 0.8f, 0.8f, 0f) * 0.3f;
		}

		private abstract class CaptureInterfaceMode
		{
			public bool Selected;

			public abstract void Update();

			public abstract void Draw(SpriteBatch sb);

			public abstract void ToggleActive(bool tickedOn);

			public abstract bool UsingMap();
		}

		private class ModeEdgeSelection : CaptureInterfaceMode
		{
			public override void Update()
			{
				if (Selected)
				{
					Vector2 mouse = new Vector2(Main.mouseX, Main.mouseY);
					EdgePlacement(mouse);
				}
			}

			public override void Draw(SpriteBatch sb)
			{
				if (Selected)
				{
					DrawMarkedArea(sb);
					DrawCursors(sb);
				}
			}

			public override void ToggleActive(bool tickedOn)
			{
			}

			public override bool UsingMap()
			{
				return true;
			}

			private void EdgePlacement(Vector2 mouse)
			{
				if (JustActivated)
				{
					return;
				}
				Point result;
				if (!Main.mapFullscreen)
				{
					if (Main.mouseLeft)
					{
						EdgeAPinned = true;
						EdgeA = Main.MouseWorld.ToTileCoordinates();
					}
					if (Main.mouseRight)
					{
						EdgeBPinned = true;
						EdgeB = Main.MouseWorld.ToTileCoordinates();
					}
				}
				else if (GetMapCoords((int)mouse.X, (int)mouse.Y, 0, out result))
				{
					if (Main.mouseLeft)
					{
						EdgeAPinned = true;
						EdgeA = result;
					}
					if (Main.mouseRight)
					{
						EdgeBPinned = true;
						EdgeB = result;
					}
				}
				ConstraintPoints();
			}

			private void DrawMarkedArea(SpriteBatch sb)
			{
				if (!EdgeAPinned || !EdgeBPinned)
				{
					return;
				}
				int num = Math.Min(EdgeA.X, EdgeB.X);
				int num2 = Math.Min(EdgeA.Y, EdgeB.Y);
				int num3 = Math.Abs(EdgeA.X - EdgeB.X);
				int num4 = Math.Abs(EdgeA.Y - EdgeB.Y);
				if (!Main.mapFullscreen)
				{
					Rectangle value = Main.ReverseGravitySupport(new Rectangle(num * 16, num2 * 16, (num3 + 1) * 16, (num4 + 1) * 16));
					Rectangle value2 = Main.ReverseGravitySupport(new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth + 1, Main.screenHeight + 1));
					Rectangle result;
					Rectangle.Intersect(ref value2, ref value, out result);
					if (result.Width != 0 && result.Height != 0)
					{
						result.Offset(-value2.X, -value2.Y);
						sb.Draw(Main.magicPixel, result, Settings.MarkedAreaColor);
						for (int i = 0; i < 2; i++)
						{
							sb.Draw(Main.magicPixel, new Rectangle(result.X, result.Y + ((i == 1) ? result.Height : (-2)), result.Width, 2), Color.White);
							sb.Draw(Main.magicPixel, new Rectangle(result.X + ((i == 1) ? result.Width : (-2)), result.Y, 2, result.Height), Color.White);
						}
					}
					return;
				}
				Point result2;
				GetMapCoords(num, num2, 1, out result2);
				Point result3;
				GetMapCoords(num + num3 + 1, num2 + num4 + 1, 1, out result3);
				Rectangle value3 = new Rectangle(result2.X, result2.Y, result3.X - result2.X, result3.Y - result2.Y);
				Rectangle value4 = new Rectangle(0, 0, Main.screenWidth + 1, Main.screenHeight + 1);
				Rectangle result4;
				Rectangle.Intersect(ref value4, ref value3, out result4);
				if (result4.Width != 0 && result4.Height != 0)
				{
					result4.Offset(-value4.X, -value4.Y);
					sb.Draw(Main.magicPixel, result4, Settings.MarkedAreaColor);
					for (int j = 0; j < 2; j++)
					{
						sb.Draw(Main.magicPixel, new Rectangle(result4.X, result4.Y + ((j == 1) ? result4.Height : (-2)), result4.Width, 2), Color.White);
						sb.Draw(Main.magicPixel, new Rectangle(result4.X + ((j == 1) ? result4.Width : (-2)), result4.Y, 2, result4.Height), Color.White);
					}
				}
			}

			private void DrawCursors(SpriteBatch sb)
			{
				float num = 1f / Main.cursorScale;
				float num2 = 0.8f / num;
				Vector2 vector = Main.screenPosition + new Vector2(30f);
				Vector2 max = vector + new Vector2(Main.screenWidth, Main.screenHeight) - new Vector2(60f);
				if (Main.mapFullscreen)
				{
					vector -= Main.screenPosition;
					max -= Main.screenPosition;
				}
				Vector3 vector2 = Main.rgbToHsl(Main.cursorColor);
				Color color = Main.hslToRgb((vector2.X + 0.33f) % 1f, vector2.Y, vector2.Z);
				Color color2 = Main.hslToRgb((vector2.X - 0.33f) % 1f, vector2.Y, vector2.Z);
				color = (color2 = Color.White);
				bool flag = Main.player[Main.myPlayer].gravDir == -1f;
				if (!EdgeAPinned)
				{
					Utils.DrawCursorSingle(sb, color, 3.926991f, Main.cursorScale * num * num2, new Vector2((float)Main.mouseX - 5f + 12f, (float)Main.mouseY + 2.5f + 12f), 4);
				}
				else
				{
					int specialMode = 0;
					float num3 = 0f;
					Vector2 zero = Vector2.Zero;
					if (!Main.mapFullscreen)
					{
						Vector2 vector3 = EdgeA.ToVector2() * 16f;
						if (!EdgeBPinned)
						{
							specialMode = 1;
							vector3 += Vector2.One * 8f;
							zero = vector3;
							num3 = (-vector3 + Main.ReverseGravitySupport(new Vector2(Main.mouseX, Main.mouseY)) + Main.screenPosition).ToRotation();
							if (flag)
							{
								num3 = 0f - num3;
							}
							zero = Vector2.Clamp(vector3, vector, max);
							if (zero != vector3)
							{
								num3 = (vector3 - zero).ToRotation();
							}
						}
						else
						{
							Vector2 vector4 = new Vector2((EdgeA.X > EdgeB.X).ToInt() * 16, (EdgeA.Y > EdgeB.Y).ToInt() * 16);
							vector3 += vector4;
							zero = Vector2.Clamp(vector3, vector, max);
							num3 = (EdgeB.ToVector2() * 16f + new Vector2(16f) - vector4 - zero).ToRotation();
							if (zero != vector3)
							{
								num3 = (vector3 - zero).ToRotation();
								specialMode = 1;
							}
							if (flag)
							{
								num3 *= -1f;
							}
						}
						Utils.DrawCursorSingle(sb, color, num3 - (float)Math.PI / 2f, Main.cursorScale * num, Main.ReverseGravitySupport(zero - Main.screenPosition), 4, specialMode);
					}
					else
					{
						Point result = EdgeA;
						if (EdgeBPinned)
						{
							int num4 = (EdgeA.X > EdgeB.X).ToInt();
							int num5 = (EdgeA.Y > EdgeB.Y).ToInt();
							result.X += num4;
							result.Y += num5;
							GetMapCoords(result.X, result.Y, 1, out result);
							Point result2 = EdgeB;
							result2.X += 1 - num4;
							result2.Y += 1 - num5;
							GetMapCoords(result2.X, result2.Y, 1, out result2);
							zero = result.ToVector2();
							zero = Vector2.Clamp(zero, vector, max);
							num3 = (result2.ToVector2() - zero).ToRotation();
						}
						else
						{
							GetMapCoords(result.X, result.Y, 1, out result);
						}
						Utils.DrawCursorSingle(sb, color, num3 - (float)Math.PI / 2f, Main.cursorScale * num, result.ToVector2(), 4);
					}
				}
				if (!EdgeBPinned)
				{
					Utils.DrawCursorSingle(sb, color2, (float)Math.PI / 4f, Main.cursorScale * num * num2, new Vector2((float)Main.mouseX + 2.5f + 12f, (float)Main.mouseY - 5f + 12f), 5);
					return;
				}
				int specialMode2 = 0;
				float num6 = 0f;
				Vector2 zero2 = Vector2.Zero;
				if (!Main.mapFullscreen)
				{
					Vector2 vector5 = EdgeB.ToVector2() * 16f;
					if (!EdgeAPinned)
					{
						specialMode2 = 1;
						vector5 += Vector2.One * 8f;
						zero2 = vector5;
						num6 = (-vector5 + Main.ReverseGravitySupport(new Vector2(Main.mouseX, Main.mouseY)) + Main.screenPosition).ToRotation();
						if (flag)
						{
							num6 = 0f - num6;
						}
						zero2 = Vector2.Clamp(vector5, vector, max);
						if (zero2 != vector5)
						{
							num6 = (vector5 - zero2).ToRotation();
						}
					}
					else
					{
						Vector2 vector6 = new Vector2((EdgeB.X >= EdgeA.X).ToInt() * 16, (EdgeB.Y >= EdgeA.Y).ToInt() * 16);
						vector5 += vector6;
						zero2 = Vector2.Clamp(vector5, vector, max);
						num6 = (EdgeA.ToVector2() * 16f + new Vector2(16f) - vector6 - zero2).ToRotation();
						if (zero2 != vector5)
						{
							num6 = (vector5 - zero2).ToRotation();
							specialMode2 = 1;
						}
						if (flag)
						{
							num6 *= -1f;
						}
					}
					Utils.DrawCursorSingle(sb, color2, num6 - (float)Math.PI / 2f, Main.cursorScale * num, Main.ReverseGravitySupport(zero2 - Main.screenPosition), 5, specialMode2);
				}
				else
				{
					Point result3 = EdgeB;
					if (EdgeAPinned)
					{
						int num7 = (EdgeB.X >= EdgeA.X).ToInt();
						int num8 = (EdgeB.Y >= EdgeA.Y).ToInt();
						result3.X += num7;
						result3.Y += num8;
						GetMapCoords(result3.X, result3.Y, 1, out result3);
						Point result4 = EdgeA;
						result4.X += 1 - num7;
						result4.Y += 1 - num8;
						GetMapCoords(result4.X, result4.Y, 1, out result4);
						zero2 = result3.ToVector2();
						zero2 = Vector2.Clamp(zero2, vector, max);
						num6 = (result4.ToVector2() - zero2).ToRotation();
					}
					else
					{
						GetMapCoords(result3.X, result3.Y, 1, out result3);
					}
					Utils.DrawCursorSingle(sb, color2, num6 - (float)Math.PI / 2f, Main.cursorScale * num, result3.ToVector2(), 5);
				}
			}
		}

		private class ModeDragBounds : CaptureInterfaceMode
		{
			public int currentAim = -1;

			private bool dragging;

			private int caughtEdge = -1;

			private bool inMap;

			public override void Update()
			{
				if (Selected && !JustActivated)
				{
					Vector2 mouse = new Vector2(Main.mouseX, Main.mouseY);
					DragBounds(mouse);
				}
			}

			public override void Draw(SpriteBatch sb)
			{
				if (Selected)
				{
					DrawMarkedArea(sb);
				}
			}

			public override void ToggleActive(bool tickedOn)
			{
				if (!tickedOn)
				{
					currentAim = -1;
				}
			}

			public override bool UsingMap()
			{
				return caughtEdge != -1;
			}

			private void DragBounds(Vector2 mouse)
			{
				if (!EdgeAPinned || !EdgeBPinned)
				{
					bool flag = false;
					if (Main.mouseLeft)
					{
						flag = true;
					}
					if (flag)
					{
						bool flag2 = true;
						Point result;
						if (!Main.mapFullscreen)
						{
							result = (Main.screenPosition + mouse).ToTileCoordinates();
						}
						else
						{
							flag2 = GetMapCoords((int)mouse.X, (int)mouse.Y, 0, out result);
						}
						if (flag2)
						{
							if (!EdgeAPinned)
							{
								EdgeAPinned = true;
								EdgeA = result;
							}
							if (!EdgeBPinned)
							{
								EdgeBPinned = true;
								EdgeB = result;
							}
						}
						currentAim = 3;
						caughtEdge = 1;
					}
				}
				int num = Math.Min(EdgeA.X, EdgeB.X);
				int num2 = Math.Min(EdgeA.Y, EdgeB.Y);
				int num3 = Math.Abs(EdgeA.X - EdgeB.X);
				int num4 = Math.Abs(EdgeA.Y - EdgeB.Y);
				bool value = Main.player[Main.myPlayer].gravDir == -1f;
				int num5 = 1 - value.ToInt();
				int num6 = value.ToInt();
				Rectangle value2;
				Rectangle value3;
				Rectangle result2;
				if (!Main.mapFullscreen)
				{
					value2 = Main.ReverseGravitySupport(new Rectangle(num * 16, num2 * 16, (num3 + 1) * 16, (num4 + 1) * 16));
					value3 = Main.ReverseGravitySupport(new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth + 1, Main.screenHeight + 1));
					Rectangle.Intersect(ref value3, ref value2, out result2);
					if (result2.Width == 0 || result2.Height == 0)
					{
						return;
					}
					result2.Offset(-value3.X, -value3.Y);
				}
				else
				{
					Point result3;
					GetMapCoords(num, num2, 1, out result3);
					Point result4;
					GetMapCoords(num + num3 + 1, num2 + num4 + 1, 1, out result4);
					value2 = new Rectangle(result3.X, result3.Y, result4.X - result3.X, result4.Y - result3.Y);
					value3 = new Rectangle(0, 0, Main.screenWidth + 1, Main.screenHeight + 1);
					Rectangle.Intersect(ref value3, ref value2, out result2);
					if (result2.Width == 0 || result2.Height == 0)
					{
						return;
					}
					result2.Offset(-value3.X, -value3.Y);
				}
				dragging = false;
				if (!Main.mouseLeft)
				{
					currentAim = -1;
				}
				if (currentAim != -1)
				{
					dragging = true;
					Point point = default(Point);
					if (!Main.mapFullscreen)
					{
						point = Main.MouseWorld.ToTileCoordinates();
					}
					else
					{
						Point result5;
						if (!GetMapCoords((int)mouse.X, (int)mouse.Y, 0, out result5))
						{
							return;
						}
						point = result5;
					}
					switch (currentAim)
					{
					case 0:
					case 1:
						if (caughtEdge == 0)
						{
							EdgeA.Y = point.Y;
						}
						if (caughtEdge == 1)
						{
							EdgeB.Y = point.Y;
						}
						break;
					case 2:
					case 3:
						if (caughtEdge == 0)
						{
							EdgeA.X = point.X;
						}
						if (caughtEdge == 1)
						{
							EdgeB.X = point.X;
						}
						break;
					}
				}
				else
				{
					caughtEdge = -1;
					Rectangle drawbox = value2;
					drawbox.Offset(-value3.X, -value3.Y);
					inMap = drawbox.Contains(mouse.ToPoint());
					for (int i = 0; i < 4; i++)
					{
						Rectangle bound = GetBound(drawbox, i);
						bound.Inflate(8, 8);
						if (!bound.Contains(mouse.ToPoint()))
						{
							continue;
						}
						currentAim = i;
						switch (i)
						{
						case 0:
							if (EdgeA.Y < EdgeB.Y)
							{
								caughtEdge = num6;
							}
							else
							{
								caughtEdge = num5;
							}
							break;
						case 1:
							if (EdgeA.Y >= EdgeB.Y)
							{
								caughtEdge = num6;
							}
							else
							{
								caughtEdge = num5;
							}
							break;
						case 2:
							if (EdgeA.X < EdgeB.X)
							{
								caughtEdge = 0;
							}
							else
							{
								caughtEdge = 1;
							}
							break;
						case 3:
							if (EdgeA.X >= EdgeB.X)
							{
								caughtEdge = 0;
							}
							else
							{
								caughtEdge = 1;
							}
							break;
						}
						break;
					}
				}
				ConstraintPoints();
			}

			private Rectangle GetBound(Rectangle drawbox, int boundIndex)
			{
				switch (boundIndex)
				{
				case 0:
					return new Rectangle(drawbox.X, drawbox.Y - 2, drawbox.Width, 2);
				case 1:
					return new Rectangle(drawbox.X, drawbox.Y + drawbox.Height, drawbox.Width, 2);
				case 2:
					return new Rectangle(drawbox.X - 2, drawbox.Y, 2, drawbox.Height);
				case 3:
					return new Rectangle(drawbox.X + drawbox.Width, drawbox.Y, 2, drawbox.Height);
				default:
					return Rectangle.Empty;
				}
			}

			public void DrawMarkedArea(SpriteBatch sb)
			{
				if (!EdgeAPinned || !EdgeBPinned)
				{
					return;
				}
				int num = Math.Min(EdgeA.X, EdgeB.X);
				int num2 = Math.Min(EdgeA.Y, EdgeB.Y);
				int num3 = Math.Abs(EdgeA.X - EdgeB.X);
				int num4 = Math.Abs(EdgeA.Y - EdgeB.Y);
				Rectangle result;
				if (!Main.mapFullscreen)
				{
					Rectangle value = Main.ReverseGravitySupport(new Rectangle(num * 16, num2 * 16, (num3 + 1) * 16, (num4 + 1) * 16));
					Rectangle value2 = Main.ReverseGravitySupport(new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth + 1, Main.screenHeight + 1));
					Rectangle.Intersect(ref value2, ref value, out result);
					if (result.Width == 0 || result.Height == 0)
					{
						return;
					}
					result.Offset(-value2.X, -value2.Y);
				}
				else
				{
					Point result2;
					GetMapCoords(num, num2, 1, out result2);
					Point result3;
					GetMapCoords(num + num3 + 1, num2 + num4 + 1, 1, out result3);
					Rectangle value = new Rectangle(result2.X, result2.Y, result3.X - result2.X, result3.Y - result2.Y);
					Rectangle value2 = new Rectangle(0, 0, Main.screenWidth + 1, Main.screenHeight + 1);
					Rectangle.Intersect(ref value2, ref value, out result);
					if (result.Width == 0 || result.Height == 0)
					{
						return;
					}
					result.Offset(-value2.X, -value2.Y);
				}
				sb.Draw(Main.magicPixel, result, Settings.MarkedAreaColor);
				Rectangle rectangle = Rectangle.Empty;
				for (int i = 0; i < 2; i++)
				{
					if (currentAim == i)
					{
						rectangle = new Rectangle(result.X, result.Y + ((i == 1) ? result.Height : (-2)), result.Width, 2);
					}
					else
					{
						DrawBound(sb, new Rectangle(result.X, result.Y + ((i == 1) ? result.Height : (-2)), result.Width, 2), 0);
					}
					if (currentAim != i + 2)
					{
						DrawBound(sb, new Rectangle(result.X + ((i == 1) ? result.Width : (-2)), result.Y, 2, result.Height), 0);
					}
					else
					{
						rectangle = new Rectangle(result.X + ((i == 1) ? result.Width : (-2)), result.Y, 2, result.Height);
					}
				}
				if (rectangle != Rectangle.Empty)
				{
					DrawBound(sb, rectangle, 1 + dragging.ToInt());
				}
			}

			private void DrawBound(SpriteBatch sb, Rectangle r, int mode)
			{
				switch (mode)
				{
				case 0:
					sb.Draw(Main.magicPixel, r, Color.Silver);
					break;
				case 1:
					sb.Draw(destinationRectangle: new Rectangle(r.X - 2, r.Y, r.Width + 4, r.Height), texture: Main.magicPixel, color: Color.White);
					sb.Draw(destinationRectangle: new Rectangle(r.X, r.Y - 2, r.Width, r.Height + 4), texture: Main.magicPixel, color: Color.White);
					sb.Draw(Main.magicPixel, r, Color.White);
					break;
				case 2:
					sb.Draw(destinationRectangle: new Rectangle(r.X - 2, r.Y, r.Width + 4, r.Height), texture: Main.magicPixel, color: Color.Gold);
					sb.Draw(destinationRectangle: new Rectangle(r.X, r.Y - 2, r.Width, r.Height + 4), texture: Main.magicPixel, color: Color.Gold);
					sb.Draw(Main.magicPixel, r, Color.Gold);
					break;
				}
			}
		}

		private class ModeChangeSettings : CaptureInterfaceMode
		{
			private const int ButtonsCount = 7;

			private int hoveredButton = -1;

			private bool inUI;

			private Rectangle GetRect()
			{
				Rectangle result = new Rectangle(0, 0, 224, 170);
				if (Settings.ScreenAnchor == 0)
				{
					result.X = 227 - result.Width / 2;
					result.Y = 80;
					int i = 0;
					for (Player player = Main.player[Main.myPlayer]; i < player.buffTime.Length && player.buffTime[i] > 0; i++)
					{
					}
					int num = i / 11;
					num += ((i % 11 >= 3) ? 1 : 0);
					result.Y += 48 * num;
				}
				return result;
			}

			private void ButtonDraw(int button, ref string key, ref string value)
			{
				switch (button)
				{
				case 3:
				case 4:
				case 5:
					break;
				case 0:
					key = Lang.inter[74].Value;
					value = Lang.inter[73 - Settings.PackImage.ToInt()].Value;
					break;
				case 1:
					key = Lang.inter[75].Value;
					value = Lang.inter[73 - Settings.IncludeEntities.ToInt()].Value;
					break;
				case 2:
					key = Lang.inter[76].Value;
					value = Lang.inter[73 - (!Settings.TransparentBackground).ToInt()].Value;
					break;
				case 6:
					key = "      " + Lang.menu[86];
					value = "";
					break;
				}
			}

			private void PressButton(int button)
			{
				switch (button)
				{
				case 3:
				case 4:
				case 5:
					break;
				case 0:
					Settings.PackImage = !Settings.PackImage;
					break;
				case 1:
					Settings.IncludeEntities = !Settings.IncludeEntities;
					break;
				case 2:
					Settings.TransparentBackground = !Settings.TransparentBackground;
					break;
				case 6:
					Settings.PackImage = false;
					Settings.IncludeEntities = true;
					Settings.TransparentBackground = false;
					Settings.BiomeChoice = 0;
					break;
				}
			}

			private void DrawWaterChoices(SpriteBatch spritebatch, Point start, Point mouse)
			{
				Rectangle r = new Rectangle(0, 0, 20, 20);
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 5; j++)
					{
						if (i == 1 && j == 3)
						{
							continue;
						}
						int index = j + i * 5;
						r.X = start.X + 24 * j + 12 * i;
						r.Y = start.Y + 24 * i;
						if (i == 1 && j == 4)
						{
							r.X -= 24;
						}
						int num = 0;
						if (r.Contains(mouse))
						{
							if (Main.mouseLeft && Main.mouseLeftRelease)
							{
								Settings.BiomeChoice = BiomeWater(index);
							}
							num++;
						}
						if (Settings.BiomeChoice == BiomeWater(index))
						{
							num += 2;
						}
						Texture2D texture = Main.liquidTexture[BiomeWater(index)];
						int x = (int)Main.wFrame * 18;
						Color white = Color.White;
						float num2 = 1f;
						if (num < 2)
						{
							num2 *= 0.5f;
						}
						if (num % 2 == 1)
						{
							spritebatch.Draw(Main.magicPixel, r.TopLeft(), new Rectangle(0, 0, 1, 1), Color.Gold, 0f, Vector2.Zero, new Vector2(20f), SpriteEffects.None, 0f);
						}
						else
						{
							spritebatch.Draw(Main.magicPixel, r.TopLeft(), new Rectangle(0, 0, 1, 1), Color.White * num2, 0f, Vector2.Zero, new Vector2(20f), SpriteEffects.None, 0f);
						}
						spritebatch.Draw(texture, r.TopLeft() + new Vector2(2f), new Rectangle(x, 0, 16, 16), Color.White * num2);
					}
				}
			}

			private int BiomeWater(int index)
			{
				switch (index)
				{
				case 0:
					return 0;
				case 1:
					return 2;
				case 2:
					return 3;
				case 3:
					return 4;
				case 4:
					return 5;
				case 5:
					return 6;
				case 6:
					return 7;
				case 7:
					return 8;
				case 8:
					return 9;
				case 9:
					return 10;
				default:
					return 0;
				}
			}

			public override void Update()
			{
				if (!Selected || JustActivated)
				{
					return;
				}
				Point value = new Point(Main.mouseX, Main.mouseY);
				hoveredButton = -1;
				Rectangle rect = GetRect();
				inUI = rect.Contains(value);
				rect.Inflate(-20, -20);
				rect.Height = 16;
				int y = rect.Y;
				for (int i = 0; i < 7; i++)
				{
					rect.Y = y + i * 20;
					if (rect.Contains(value))
					{
						hoveredButton = i;
						break;
					}
				}
				if (Main.mouseLeft && Main.mouseLeftRelease && hoveredButton != -1)
				{
					PressButton(hoveredButton);
				}
			}

			public override void Draw(SpriteBatch sb)
			{
				if (!Selected)
				{
					return;
				}
				((ModeDragBounds)Modes[1]).currentAim = -1;
				((ModeDragBounds)Modes[1]).DrawMarkedArea(sb);
				Rectangle rect = GetRect();
				Utils.DrawInvBG(sb, rect, new Color(63, 65, 151, 255) * 0.485f);
				for (int i = 0; i < 7; i++)
				{
					string key = "";
					string value = "";
					ButtonDraw(i, ref key, ref value);
					Color baseColor = Color.White;
					if (i == hoveredButton)
					{
						baseColor = Color.Gold;
					}
					ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontItemStack, key, rect.TopLeft() + new Vector2(20f, 20 + 20 * i), baseColor, 0f, Vector2.Zero, Vector2.One);
					ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontItemStack, value, rect.TopRight() + new Vector2(-20f, 20 + 20 * i), baseColor, 0f, Main.fontItemStack.MeasureString(value) * Vector2.UnitX, Vector2.One);
				}
				DrawWaterChoices(sb, (rect.TopLeft() + new Vector2(rect.Width / 2 - 58, 90f)).ToPoint(), Main.MouseScreen.ToPoint());
			}

			public override void ToggleActive(bool tickedOn)
			{
				if (tickedOn)
				{
					hoveredButton = -1;
				}
			}

			public override bool UsingMap()
			{
				return inUI;
			}
		}

		private const Keys KeyToggleActive = Keys.F1;

		private const float CameraMaxFrame = 5f;

		private const float CameraMaxWait = 60f;

		private static Dictionary<int, CaptureInterfaceMode> Modes = FillModes();

		public bool Active;

		public static bool JustActivated = false;

		private bool KeyToggleActiveHeld;

		public int SelectedMode;

		public int HoveredMode;

		public static bool EdgeAPinned = false;

		public static bool EdgeBPinned = false;

		public static Point EdgeA = default(Point);

		public static Point EdgeB = default(Point);

		public static bool CameraLock = false;

		private static float CameraFrame = 0f;

		private static float CameraWaiting = 0f;

		private static CaptureSettings CameraSettings;

		private static Dictionary<int, CaptureInterfaceMode> FillModes()
		{
			Dictionary<int, CaptureInterfaceMode> dictionary = new Dictionary<int, CaptureInterfaceMode>();
			dictionary.Add(0, new ModeEdgeSelection());
			dictionary.Add(1, new ModeDragBounds());
			dictionary.Add(2, new ModeChangeSettings());
			return dictionary;
		}

		public static Rectangle GetArea()
		{
			int x = Math.Min(EdgeA.X, EdgeB.X);
			int y = Math.Min(EdgeA.Y, EdgeB.Y);
			int num = Math.Abs(EdgeA.X - EdgeB.X);
			int num2 = Math.Abs(EdgeA.Y - EdgeB.Y);
			return new Rectangle(x, y, num + 1, num2 + 1);
		}

		public void Update()
		{
			UpdateCamera();
			if (CameraLock)
			{
				return;
			}
			bool flag = Main.keyState.IsKeyDown(Keys.F1);
			if (flag && !KeyToggleActiveHeld && (Main.mouseItem.type == 0 || Active) && !Main.CaptureModeDisabled)
			{
				ToggleCamera(!Active);
			}
			KeyToggleActiveHeld = flag;
			if (Active)
			{
				Main.blockMouse = true;
				if (JustActivated && Main.mouseLeftRelease && !Main.mouseLeft)
				{
					JustActivated = false;
				}
				Vector2 mouse = new Vector2(Main.mouseX, Main.mouseY);
				if (!UpdateButtons(mouse) || !Main.mouseLeft)
				{
					foreach (KeyValuePair<int, CaptureInterfaceMode> mode in Modes)
					{
						mode.Value.Selected = (mode.Key == SelectedMode);
						mode.Value.Update();
					}
				}
			}
		}

		public void Draw(SpriteBatch sb)
		{
			if (Active)
			{
				foreach (CaptureInterfaceMode value in Modes.Values)
				{
					value.Draw(sb);
				}
				Main.mouseText = false;
				Main.instance.GUIBarsDraw();
				DrawButtons(sb);
				Main.instance.DrawMouseOver();
				Utils.DrawBorderStringBig(sb, Lang.inter[81].Value, new Vector2((float)Main.screenWidth * 0.5f, 100f), Color.White, 1f, 0.5f, 0.5f);
				Utils.DrawCursorSingle(sb, Main.cursorColor, float.NaN, Main.cursorScale);
				DrawCameraLock(sb);
			}
		}

		public void ToggleCamera(bool On = true)
		{
			if (!CameraLock)
			{
				bool active = Active;
				Active = (Modes.ContainsKey(SelectedMode) && On);
				if (active != Active)
				{
					Main.PlaySound(12);
				}
				foreach (KeyValuePair<int, CaptureInterfaceMode> mode in Modes)
				{
					mode.Value.ToggleActive(Active && mode.Key == SelectedMode);
				}
				if (On && !active)
				{
					JustActivated = true;
				}
			}
		}

		private bool UpdateButtons(Vector2 mouse)
		{
			HoveredMode = -1;
			bool flag = !Main.graphics.IsFullScreen;
			int num = 9;
			for (int i = 0; i < num; i++)
			{
				if (new Rectangle(24 + 46 * i, 24, 42, 42).Contains(mouse.ToPoint()))
				{
					HoveredMode = i;
					bool flag2 = Main.mouseLeft && Main.mouseLeftRelease;
					int num2 = 0;
					if (i == num2++ && flag2)
					{
						CaptureSettings captureSettings = new CaptureSettings();
						Point point = Main.screenPosition.ToTileCoordinates();
						Point point2 = (Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight)).ToTileCoordinates();
						captureSettings.Area = new Rectangle(point.X, point.Y, point2.X - point.X + 1, point2.Y - point.Y + 1);
						captureSettings.Biome = CaptureBiome.Biomes[Settings.BiomeChoice];
						captureSettings.CaptureBackground = !Settings.TransparentBackground;
						captureSettings.CaptureEntities = Settings.IncludeEntities;
						captureSettings.UseScaling = Settings.PackImage;
						captureSettings.CaptureMech = Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].mech;
						StartCamera(captureSettings);
					}
					if (i == num2++ && flag2 && EdgeAPinned && EdgeBPinned)
					{
						CaptureSettings captureSettings2 = new CaptureSettings();
						captureSettings2.Area = GetArea();
						captureSettings2.Biome = CaptureBiome.Biomes[Settings.BiomeChoice];
						captureSettings2.CaptureBackground = !Settings.TransparentBackground;
						captureSettings2.CaptureEntities = Settings.IncludeEntities;
						captureSettings2.UseScaling = Settings.PackImage;
						captureSettings2.CaptureMech = Main.player[Main.myPlayer].inventory[Main.player[Main.myPlayer].selectedItem].mech;
						StartCamera(captureSettings2);
					}
					if (i == num2++ && flag2 && SelectedMode != 0)
					{
						SelectedMode = 0;
						ToggleCamera();
					}
					if (i == num2++ && flag2 && SelectedMode != 1)
					{
						SelectedMode = 1;
						ToggleCamera();
					}
					if (i == num2++ && flag2)
					{
						ResetFocus();
					}
					if (i == num2++ && flag2 && Main.mapEnabled)
					{
						Main.mapFullscreen = !Main.mapFullscreen;
					}
					if (i == num2++ && flag2 && SelectedMode != 2)
					{
						SelectedMode = 2;
						ToggleCamera();
					}
					if (i == num2++ && flag2 && flag)
					{
						string fileName = Main.SavePath + Path.DirectorySeparatorChar + "Captures" + Path.DirectorySeparatorChar;
						Process.Start(fileName);
					}
					if (i == num2++ && flag2)
					{
						ToggleCamera(false);
						Main.blockMouse = true;
						Main.mouseLeftRelease = false;
					}
					return true;
				}
			}
			return false;
		}

		private void DrawButtons(SpriteBatch sb)
		{
			new Vector2(Main.mouseX, Main.mouseY);
			int num = 9;
			for (int i = 0; i < num; i++)
			{
				Texture2D texture2D = Main.inventoryBackTexture;
				float num2 = 0.8f;
				Vector2 vector = new Vector2(24 + 46 * i, 24f);
				Color color = Main.inventoryBack * 0.8f;
				if (SelectedMode == 0 && i == 2)
				{
					texture2D = Main.inventoryBack14Texture;
				}
				else if (SelectedMode == 1 && i == 3)
				{
					texture2D = Main.inventoryBack14Texture;
				}
				else if (SelectedMode == 2 && i == 6)
				{
					texture2D = Main.inventoryBack14Texture;
				}
				else if (i >= 2 && i <= 3)
				{
					texture2D = Main.inventoryBack2Texture;
				}
				sb.Draw(texture2D, vector, null, color, 0f, default(Vector2), num2, SpriteEffects.None, 0f);
				switch (i)
				{
				case 0:
					texture2D = Main.cameraTexture[7];
					break;
				case 1:
					texture2D = Main.cameraTexture[0];
					break;
				case 2:
				case 3:
				case 4:
					texture2D = Main.cameraTexture[i];
					break;
				case 5:
					texture2D = (Main.mapFullscreen ? Main.mapIconTexture[0] : Main.mapIconTexture[4]);
					break;
				case 6:
					texture2D = Main.cameraTexture[1];
					break;
				case 7:
					texture2D = Main.cameraTexture[6];
					break;
				case 8:
					texture2D = Main.cameraTexture[5];
					break;
				}
				sb.Draw(texture2D, vector + new Vector2(26f) * num2, null, Color.White, 0f, texture2D.Size() / 2f, 1f, SpriteEffects.None, 0f);
				bool flag = false;
				switch (i)
				{
				case 1:
					if (!EdgeAPinned || !EdgeBPinned)
					{
						flag = true;
					}
					break;
				case 7:
					if (Main.graphics.IsFullScreen)
					{
						flag = true;
					}
					break;
				case 5:
					if (!Main.mapEnabled)
					{
						flag = true;
					}
					break;
				}
				if (flag)
				{
					sb.Draw(Main.cdTexture, vector + new Vector2(26f) * num2, null, Color.White * 0.65f, 0f, Main.cdTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
				}
			}
			string text = "";
			switch (HoveredMode)
			{
			case 0:
				text = Lang.inter[111].Value;
				break;
			case 1:
				text = Lang.inter[67].Value;
				break;
			case 2:
				text = Lang.inter[69].Value;
				break;
			case 3:
				text = Lang.inter[70].Value;
				break;
			case 4:
				text = Lang.inter[78].Value;
				break;
			case 5:
				text = (Main.mapFullscreen ? Lang.inter[109].Value : Lang.inter[108].Value);
				break;
			case 6:
				text = Lang.inter[68].Value;
				break;
			case 7:
				text = Lang.inter[110].Value;
				break;
			case 8:
				text = Lang.inter[71].Value;
				break;
			default:
				text = "???";
				break;
			case -1:
				break;
			}
			switch (HoveredMode)
			{
			case 1:
				if (!EdgeAPinned || !EdgeBPinned)
				{
					text = text + "\n" + Lang.inter[112];
				}
				break;
			case 7:
				if (Main.graphics.IsFullScreen)
				{
					text = text + "\n" + Lang.inter[113];
				}
				break;
			case 5:
				if (!Main.mapEnabled)
				{
					text = text + "\n" + Lang.inter[114];
				}
				break;
			}
			if (text != "")
			{
				Main.instance.MouseText(text, 0, 0);
			}
		}

		private static bool GetMapCoords(int PinX, int PinY, int Goal, out Point result)
		{
			if (!Main.mapFullscreen)
			{
				result = new Point(-1, -1);
				return false;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 2f;
			int num16 = Main.maxTilesX / Main.textureMaxWidth;
			int num17 = Main.maxTilesY / Main.textureMaxHeight;
			float num4 = 10f;
			float num5 = 10f;
			float num6 = Main.maxTilesX - 10;
			float num7 = Main.maxTilesY - 10;
			num = 200f;
			num2 = 300f;
			num3 = Main.mapFullscreenScale;
			float num8 = (float)Main.screenWidth / (float)Main.maxTilesX * 0.8f;
			if (Main.mapFullscreenScale < num8)
			{
				Main.mapFullscreenScale = num8;
			}
			if (Main.mapFullscreenScale > 16f)
			{
				Main.mapFullscreenScale = 16f;
			}
			num3 = Main.mapFullscreenScale;
			if (Main.mapFullscreenPos.X < num4)
			{
				Main.mapFullscreenPos.X = num4;
			}
			if (Main.mapFullscreenPos.X > num6)
			{
				Main.mapFullscreenPos.X = num6;
			}
			if (Main.mapFullscreenPos.Y < num5)
			{
				Main.mapFullscreenPos.Y = num5;
			}
			if (Main.mapFullscreenPos.Y > num7)
			{
				Main.mapFullscreenPos.Y = num7;
			}
			float x = Main.mapFullscreenPos.X;
			float y = Main.mapFullscreenPos.Y;
			x *= num3;
			y *= num3;
			num = 0f - x + (float)(Main.screenWidth / 2);
			num2 = 0f - y + (float)(Main.screenHeight / 2);
			num += num4 * num3;
			num2 += num5 * num3;
			float num9 = Main.maxTilesX / 840;
			num9 *= Main.mapFullscreenScale;
			float num10 = num;
			float num11 = num2;
			float num12 = Main.mapTexture.Width;
			float num13 = Main.mapTexture.Height;
			if (Main.maxTilesX == 8400)
			{
				num9 *= 0.999f;
				num10 -= 40.6f * num9;
				num11 = num2 - 5f * num9;
				num12 -= 8.045f;
				num12 *= num9;
				num13 += 0.12f;
				num13 *= num9;
				if ((double)num9 < 1.2)
				{
					num13 += 1f;
				}
			}
			else if (Main.maxTilesX == 6400)
			{
				num9 *= 1.09f;
				num10 -= 38.8f * num9;
				num11 = num2 - 3.85f * num9;
				num12 -= 13.6f;
				num12 *= num9;
				num13 -= 6.92f;
				num13 *= num9;
				if ((double)num9 < 1.2)
				{
					num13 += 2f;
				}
			}
			else if (Main.maxTilesX == 6300)
			{
				num9 *= 1.09f;
				num10 -= 39.8f * num9;
				num11 = num2 - 4.08f * num9;
				num12 -= 26.69f;
				num12 *= num9;
				num13 -= 6.92f;
				num13 *= num9;
				if ((double)num9 < 1.2)
				{
					num13 += 2f;
				}
			}
			else if (Main.maxTilesX == 4200)
			{
				num9 *= 0.998f;
				num10 -= 37.3f * num9;
				num11 -= 1.7f * num9;
				num12 -= 16f;
				num12 *= num9;
				num13 -= 8.31f;
				num13 *= num9;
			}
			switch (Goal)
			{
			case 0:
			{
				int num14 = (int)((0f - num + (float)PinX) / num3 + num4);
				int num15 = (int)((0f - num2 + (float)PinY) / num3 + num5);
				bool flag = false;
				if ((float)num14 < num4)
				{
					flag = true;
				}
				if ((float)num14 >= num6)
				{
					flag = true;
				}
				if ((float)num15 < num5)
				{
					flag = true;
				}
				if ((float)num15 >= num7)
				{
					flag = true;
				}
				if (!flag)
				{
					result = new Point(num14, num15);
					return true;
				}
				result = new Point(-1, -1);
				return false;
			}
			case 1:
			{
				Vector2 value = new Vector2(num, num2);
				Vector2 value2 = new Vector2(PinX, PinY) * num3 - new Vector2(10f * num3);
				result = (value + value2).ToPoint();
				return true;
			}
			default:
				result = new Point(-1, -1);
				return false;
			}
		}

		private static void ConstraintPoints()
		{
			int offScreenTiles = Lighting.offScreenTiles;
			if (EdgeAPinned)
			{
				PointWorldClamp(ref EdgeA, offScreenTiles);
			}
			if (EdgeBPinned)
			{
				PointWorldClamp(ref EdgeB, offScreenTiles);
			}
		}

		private static void PointWorldClamp(ref Point point, int fluff)
		{
			if (point.X < fluff)
			{
				point.X = fluff;
			}
			if (point.X > Main.maxTilesX - 1 - fluff)
			{
				point.X = Main.maxTilesX - 1 - fluff;
			}
			if (point.Y < fluff)
			{
				point.Y = fluff;
			}
			if (point.Y > Main.maxTilesY - 1 - fluff)
			{
				point.Y = Main.maxTilesY - 1 - fluff;
			}
		}

		public bool UsingMap()
		{
			if (CameraLock)
			{
				return true;
			}
			return Modes[SelectedMode].UsingMap();
		}

		public static void ResetFocus()
		{
			EdgeAPinned = false;
			EdgeBPinned = false;
			EdgeA = new Point(-1, -1);
			EdgeB = new Point(-1, -1);
		}

		public void Scrolling()
		{
			int num = (Main.mouseState.ScrollWheelValue - Main.oldMouseWheel) / 120;
			num %= 30;
			if (num < 0)
			{
				num += 30;
			}
			int selectedMode = SelectedMode;
			SelectedMode -= num;
			while (SelectedMode < 0)
			{
				SelectedMode += 2;
			}
			while (SelectedMode > 2)
			{
				SelectedMode -= 2;
			}
			if (SelectedMode != selectedMode)
			{
				Main.PlaySound(12);
			}
		}

		private void UpdateCamera()
		{
			if (CameraLock && CameraFrame == 4f)
			{
				CaptureManager.Instance.Capture(CameraSettings);
			}
			CameraFrame += CameraLock.ToDirectionInt();
			if (CameraFrame < 0f)
			{
				CameraFrame = 0f;
			}
			if (CameraFrame > 5f)
			{
				CameraFrame = 5f;
			}
			if (CameraFrame == 5f)
			{
				CameraWaiting += 1f;
			}
			if (CameraWaiting > 60f)
			{
				CameraWaiting = 60f;
			}
		}

		private void DrawCameraLock(SpriteBatch sb)
		{
			if (CameraFrame == 0f)
			{
				return;
			}
			sb.Draw(Main.magicPixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Rectangle(0, 0, 1, 1), Color.Black * (CameraFrame / 5f));
			if (CameraFrame != 5f)
			{
				return;
			}
			float num = CameraWaiting - 60f + 5f;
			if (!(num <= 0f))
			{
				num /= 5f;
				float num2 = CaptureManager.Instance.GetProgress() * 100f;
				if (num2 > 100f)
				{
					num2 = 100f;
				}
				string text = num2.ToString("##") + " ";
				string text2 = "/ 100%";
				Vector2 vector = Main.fontDeathText.MeasureString(text);
				Vector2 vector2 = Main.fontDeathText.MeasureString(text2);
				Vector2 value = new Vector2(0f - vector.X, (0f - vector.Y) / 2f);
				Vector2 value2 = new Vector2(0f, (0f - vector2.Y) / 2f);
				ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontDeathText, text, new Vector2(Main.screenWidth, Main.screenHeight) / 2f + value, Color.White * num, 0f, Vector2.Zero, Vector2.One);
				ChatManager.DrawColorCodedStringWithShadow(sb, Main.fontDeathText, text2, new Vector2(Main.screenWidth, Main.screenHeight) / 2f + value2, Color.White * num, 0f, Vector2.Zero, Vector2.One);
			}
		}

		public static void StartCamera(CaptureSettings settings)
		{
			Main.PlaySound(40);
			CameraSettings = settings;
			CameraLock = true;
			CameraWaiting = 0f;
		}

		public static void EndCamera()
		{
			CameraLock = false;
		}
	}
}
