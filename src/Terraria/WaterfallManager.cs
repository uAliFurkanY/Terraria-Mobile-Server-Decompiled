using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Terraria
{
	public class WaterfallManager
	{
		public struct WaterfallData
		{
			public int x;

			public int y;

			public int type;

			public int stopAtStep;
		}

		private const int minWet = 160;

		private const int maxCount = 200;

		private const int maxLength = 100;

		private const int maxTypes = 22;

		private int qualityMax;

		private int currentMax;

		private WaterfallData[] waterfalls;

		public Texture2D[] waterfallTexture = new Texture2D[22];

		private int wFallFrCounter;

		private int regularFrame;

		private int wFallFrCounter2;

		private int slowFrame;

		private int rainFrameCounter;

		private int rainFrameForeground;

		private int rainFrameBackground;

		private int findWaterfallCount;

		private int waterfallDist = 100;

		public WaterfallManager()
		{
			waterfalls = new WaterfallData[200];
		}

		public void LoadContent(ContentManager manager)
		{
			for (int i = 0; i < 22; i++)
			{
				waterfallTexture[i] = manager.Load<Texture2D>("Images" + Path.DirectorySeparatorChar + "Waterfall_" + i);
			}
		}

		public bool CheckForWaterfall(int i, int j)
		{
			for (int k = 0; k < currentMax; k++)
			{
				if (waterfalls[k].x == i && waterfalls[k].y == j)
				{
					return true;
				}
			}
			return false;
		}

		public void FindWaterfalls(bool forced = false)
		{
			findWaterfallCount++;
			if (findWaterfallCount < 30 && !forced)
			{
				return;
			}
			findWaterfallCount = 0;
			waterfallDist = (int)(75f * Main.gfxQuality) + 25;
			qualityMax = (int)(175f * Main.gfxQuality) + 25;
			currentMax = 0;
			int num = (int)(Main.screenPosition.X / 16f - 1f);
			int num2 = (int)((Main.screenPosition.X + (float)Main.screenWidth) / 16f) + 2;
			int num3 = (int)(Main.screenPosition.Y / 16f - 1f);
			int num4 = (int)((Main.screenPosition.Y + (float)Main.screenHeight) / 16f) + 2;
			num -= waterfallDist;
			num2 += waterfallDist;
			num3 -= waterfallDist;
			num4 += 20;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 > Main.maxTilesX)
			{
				num2 = Main.maxTilesX;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 > Main.maxTilesY)
			{
				num4 = Main.maxTilesY;
			}
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile == null)
					{
						tile = new Tile();
						Main.tile[i, j] = tile;
					}
					if (!tile.active())
					{
						continue;
					}
					if (tile.halfBrick())
					{
						Tile tile2 = Main.tile[i, j - 1];
						if (tile2 == null)
						{
							tile2 = new Tile();
							Main.tile[i, j - 1] = tile2;
						}
						if (tile2.liquid < 16 || WorldGen.SolidTile(tile2))
						{
							Tile tile3 = Main.tile[i + 1, j];
							if (tile3 == null)
							{
								tile3 = new Tile();
								Main.tile[i - 1, j] = tile3;
							}
							Tile tile4 = Main.tile[i - 1, j];
							if (tile4 == null)
							{
								tile4 = new Tile();
								Main.tile[i + 1, j] = tile4;
							}
							if ((tile3.liquid > 160 || tile4.liquid > 160) && ((tile3.liquid == 0 && !WorldGen.SolidTile(tile3) && tile3.slope() == 0) || (tile4.liquid == 0 && !WorldGen.SolidTile(tile4) && tile4.slope() == 0)) && currentMax < qualityMax)
							{
								waterfalls[currentMax].type = 0;
								if (tile2.lava() || tile4.lava() || tile3.lava())
								{
									waterfalls[currentMax].type = 1;
								}
								else if (tile2.honey() || tile4.honey() || tile3.honey())
								{
									waterfalls[currentMax].type = 14;
								}
								else
								{
									waterfalls[currentMax].type = 0;
								}
								waterfalls[currentMax].x = i;
								waterfalls[currentMax].y = j;
								currentMax++;
							}
						}
					}
					if (tile.type == 196)
					{
						Tile tile5 = Main.tile[i, j + 1];
						if (tile5 == null)
						{
							tile5 = new Tile();
							Main.tile[i, j + 1] = tile5;
						}
						if (!WorldGen.SolidTile(tile5) && tile5.slope() == 0 && currentMax < qualityMax)
						{
							waterfalls[currentMax].type = 11;
							waterfalls[currentMax].x = i;
							waterfalls[currentMax].y = j + 1;
							currentMax++;
						}
					}
				}
			}
		}

		public void UpdateFrame()
		{
			wFallFrCounter++;
			if (wFallFrCounter > 2)
			{
				wFallFrCounter = 0;
				regularFrame++;
				if (regularFrame > 15)
				{
					regularFrame = 0;
				}
			}
			wFallFrCounter2++;
			if (wFallFrCounter2 > 6)
			{
				wFallFrCounter2 = 0;
				slowFrame++;
				if (slowFrame > 15)
				{
					slowFrame = 0;
				}
			}
			rainFrameCounter++;
			if (rainFrameCounter <= 0)
			{
				return;
			}
			rainFrameForeground++;
			if (rainFrameForeground > 7)
			{
				rainFrameForeground -= 8;
			}
			if (rainFrameCounter > 2)
			{
				rainFrameCounter = 0;
				rainFrameBackground--;
				if (rainFrameBackground < 0)
				{
					rainFrameBackground = 7;
				}
			}
		}

		private void DrawWaterfall(SpriteBatch spriteBatch, int Style = 0, float Alpha = 1f)
		{
			float num = 0f;
			float num2 = 99999f;
			float num3 = 99999f;
			int num4 = -1;
			int num5 = -1;
			float num6 = 0f;
			float num7 = 99999f;
			float num8 = 99999f;
			int num9 = -1;
			int num10 = -1;
			for (int i = 0; i < currentMax; i++)
			{
				int num11 = 0;
				int num12 = waterfalls[i].type;
				int num13 = waterfalls[i].x;
				int num14 = waterfalls[i].y;
				int num15 = 0;
				int num16 = 0;
				int num17 = 0;
				int num18 = 0;
				int num19 = 0;
				int num20 = 0;
				int num21;
				int num22;
				if (num12 == 1 || num12 == 14)
				{
					if (Main.drewLava || waterfalls[i].stopAtStep == 0)
					{
						continue;
					}
					num21 = 32 * slowFrame;
				}
				else
				{
					switch (num12)
					{
					case 11:
					{
						if (Main.drewLava)
						{
							continue;
						}
						num22 = waterfallDist / 4;
						if (waterfalls[i].stopAtStep > num22)
						{
							waterfalls[i].stopAtStep = num22;
						}
						if (waterfalls[i].stopAtStep == 0 || (float)(num14 + num22) < Main.screenPosition.Y / 16f || (float)num13 < Main.screenPosition.X / 16f - 1f || (float)num13 > (Main.screenPosition.X + (float)Main.screenWidth) / 16f + 1f)
						{
							continue;
						}
						int num23;
						int num24;
						if (num13 % 2 == 0)
						{
							num23 = rainFrameForeground + 3;
							if (num23 > 7)
							{
								num23 -= 8;
							}
							num24 = rainFrameBackground + 2;
							if (num24 > 7)
							{
								num24 -= 8;
							}
						}
						else
						{
							num23 = rainFrameForeground;
							num24 = rainFrameBackground;
						}
						Rectangle value = new Rectangle(num24 * 18, 0, 16, 16);
						Rectangle value2 = new Rectangle(num23 * 18, 0, 16, 16);
						Vector2 origin = new Vector2(8f, 8f);
						Vector2 position = (num14 % 2 != 0) ? (new Vector2(num13 * 16 + 8, num14 * 16 + 8) - Main.screenPosition) : (new Vector2(num13 * 16 + 9, num14 * 16 + 8) - Main.screenPosition);
						bool flag = false;
						for (int j = 0; j < num22; j++)
						{
							Color color = Lighting.GetColor(num13, num14);
							float num25 = 0.6f;
							float num26 = 0.3f;
							if (j > num22 - 8)
							{
								float num27 = (float)(num22 - j) / 8f;
								num25 *= num27;
								num26 *= num27;
							}
							Color color2 = color * num25;
							Color color3 = color * num26;
							spriteBatch.Draw(waterfallTexture[12], position, value, color3, 0f, origin, 1f, SpriteEffects.None, 0f);
							spriteBatch.Draw(waterfallTexture[11], position, value2, color2, 0f, origin, 1f, SpriteEffects.None, 0f);
							if (flag)
							{
								break;
							}
							num14++;
							Tile tile = Main.tile[num13, num14];
							if (WorldGen.SolidTile(tile))
							{
								flag = true;
							}
							if (tile.liquid > 0)
							{
								int num28 = (int)(16f * ((float)(int)tile.liquid / 255f)) & 0xFE;
								if (num28 >= 15)
								{
									break;
								}
								value2.Height -= num28;
								value.Height -= num28;
							}
							if (num14 % 2 == 0)
							{
								position.X += 1f;
							}
							else
							{
								position.X -= 1f;
							}
							position.Y += 16f;
						}
						waterfalls[i].stopAtStep = 0;
						continue;
					}
					case 0:
						num12 = Style;
						break;
					case 2:
						if (Main.drewLava)
						{
							continue;
						}
						break;
					}
					num21 = 32 * regularFrame;
				}
				int num29 = 0;
				num22 = waterfallDist;
				Color color4 = Color.White;
				for (int k = 0; k < num22; k++)
				{
					if (num29 >= 2)
					{
						continue;
					}
					switch (num12)
					{
					case 1:
					{
						float num30 = 0.55f;
						num30 += (float)(270 - Main.mouseTextColor) / 900f;
						num30 *= 0.4f;
						float r = num30;
						float g = num30 * 0.3f;
						float b = num30 * 0.1f;
						Lighting.AddLight(num13, num14, r, g, b);
						break;
					}
					case 2:
					{
						float r = (float)Main.DiscoR / 255f;
						float g = (float)Main.DiscoG / 255f;
						float b = (float)Main.DiscoB / 255f;
						r *= 0.2f;
						g *= 0.2f;
						b *= 0.2f;
						Lighting.AddLight(num13, num14, r, g, b);
						break;
					}
					case 15:
					{
						float r = 0f;
						float g = 0f;
						float b = 0.2f;
						Lighting.AddLight(num13, num14, r, g, b);
						break;
					}
					case 16:
					{
						float r = 0f;
						float g = 0.2f;
						float b = 0f;
						Lighting.AddLight(num13, num14, r, g, b);
						break;
					}
					case 17:
					{
						float r = 0f;
						float g = 0f;
						float b = 0.2f;
						Lighting.AddLight(num13, num14, r, g, b);
						break;
					}
					case 18:
					{
						float r = 0f;
						float g = 0.2f;
						float b = 0f;
						Lighting.AddLight(num13, num14, r, g, b);
						break;
					}
					case 19:
					{
						float r = 0.2f;
						float g = 0f;
						float b = 0f;
						Lighting.AddLight(num13, num14, r, g, b);
						break;
					}
					case 20:
						Lighting.AddLight(num13, num14, 0.2f, 0.2f, 0.2f);
						break;
					case 21:
					{
						float r = 0.2f;
						float g = 0f;
						float b = 0f;
						Lighting.AddLight(num13, num14, r, g, b);
						break;
					}
					}
					int num31 = 0;
					int num32 = 0;
					int num33 = 0;
					Tile tile2 = Main.tile[num13, num14];
					if (tile2 == null)
					{
						tile2 = new Tile();
						Main.tile[num13, num14] = tile2;
					}
					Tile tile3 = Main.tile[num13 - 1, num14];
					if (tile3 == null)
					{
						tile3 = new Tile();
						Main.tile[num13 - 1, num14] = tile3;
					}
					Tile tile4 = Main.tile[num13, num14 + 1];
					if (tile4 == null)
					{
						tile4 = new Tile();
						Main.tile[num13, num14 + 1] = tile4;
					}
					Tile tile5 = Main.tile[num13 + 1, num14];
					if (tile5 == null)
					{
						tile5 = new Tile();
						Main.tile[num13 + 1, num14] = tile5;
					}
					num33 = (int)tile2.liquid / 16;
					int num34 = 0;
					int num35 = num18;
					if (tile4.topSlope())
					{
						if (tile4.slope() == 1)
						{
							num34 = 1;
							num31 = 1;
							num17 = 1;
							num18 = num17;
						}
						else
						{
							num34 = -1;
							num31 = -1;
							num17 = -1;
							num18 = num17;
						}
						num32 = 1;
					}
					else if ((((!WorldGen.SolidTile(tile4) && !tile4.bottomSlope()) || tile4.type == 162) && !tile2.halfBrick()) || (!tile4.active() && !tile2.halfBrick()))
					{
						num29 = 0;
						num32 = 1;
						num31 = 0;
					}
					else if ((WorldGen.SolidTile(tile3) || tile3.topSlope() || tile3.liquid > 0) && !WorldGen.SolidTile(tile5) && tile5.liquid == 0)
					{
						if (num17 == -1)
						{
							num29++;
						}
						num31 = 1;
						num32 = 0;
						num17 = 1;
					}
					else if ((WorldGen.SolidTile(tile5) || tile5.topSlope() || tile5.liquid > 0) && !WorldGen.SolidTile(tile3) && tile3.liquid == 0)
					{
						if (num17 == 1)
						{
							num29++;
						}
						num31 = -1;
						num32 = 0;
						num17 = -1;
					}
					else if (((!WorldGen.SolidTile(tile5) && !tile2.topSlope()) || tile5.liquid == 0) && !WorldGen.SolidTile(tile3) && !tile2.topSlope() && tile3.liquid == 0)
					{
						num32 = 0;
						num31 = num17;
					}
					else
					{
						num29++;
						num32 = 0;
						num31 = 0;
					}
					if (num29 >= 2)
					{
						num17 *= -1;
						num31 *= -1;
					}
					int num36 = -1;
					if (num12 != 1 && num12 != 14)
					{
						if (tile4.active())
						{
							num36 = tile4.type;
						}
						if (tile2.active())
						{
							num36 = tile2.type;
						}
					}
					switch (num36)
					{
					case 160:
						num12 = 2;
						break;
					case 262:
					case 263:
					case 264:
					case 265:
					case 266:
					case 267:
					case 268:
						num12 = 15 + num36 - 262;
						break;
					}
					if (WorldGen.SolidTile(tile4) && !tile2.halfBrick())
					{
						num11 = 8;
					}
					else if (num16 != 0)
					{
						num11 = 0;
					}
					Color color5 = Lighting.GetColor(num13, num14);
					Color color6 = color5;
					float num37;
					switch (num12)
					{
					case 1:
						num37 = 1f;
						break;
					case 14:
						num37 = 0.8f;
						break;
					default:
						num37 = ((tile2.wall != 0 || !((double)num14 < Main.worldSurface)) ? (0.6f * Alpha) : Alpha);
						break;
					}
					if (k > num22 - 10)
					{
						num37 *= (float)(num22 - k) / 10f;
					}
					float num38 = (float)(int)color5.R * num37;
					float num39 = (float)(int)color5.G * num37;
					float num40 = (float)(int)color5.B * num37;
					float num41 = (float)(int)color5.A * num37;
					switch (num12)
					{
					case 1:
						if (num38 < 190f * num37)
						{
							num38 = 190f * num37;
						}
						if (num39 < 190f * num37)
						{
							num39 = 190f * num37;
						}
						if (num40 < 190f * num37)
						{
							num40 = 190f * num37;
						}
						break;
					case 2:
						num38 = (float)Main.DiscoR * num37;
						num39 = (float)Main.DiscoG * num37;
						num40 = (float)Main.DiscoB * num37;
						break;
					case 15:
					case 16:
					case 17:
					case 18:
					case 19:
					case 20:
					case 21:
						num38 = 255f * num37;
						num39 = 255f * num37;
						num40 = 255f * num37;
						break;
					}
					color5 = new Color((int)num38, (int)num39, (int)num40, (int)num41);
					if (num12 == 1)
					{
						float num42 = Math.Abs((float)(num13 * 16 + 8) - (Main.screenPosition.X + (float)(Main.screenWidth / 2)));
						float num43 = Math.Abs((float)(num14 * 16 + 8) - (Main.screenPosition.Y + (float)(Main.screenHeight / 2)));
						if (num42 < (float)(Main.screenWidth * 2) && num43 < (float)(Main.screenHeight * 2))
						{
							float num44 = (float)Math.Sqrt(num42 * num42 + num43 * num43);
							float num45 = 1f - num44 / ((float)Main.screenWidth * 0.75f);
							if (num45 > 0f)
							{
								num6 += num45;
							}
						}
						if (num42 < num7)
						{
							num7 = num42;
							num9 = num13 * 16 + 8;
						}
						if (num43 < num8)
						{
							num8 = num42;
							num10 = num14 * 16 + 8;
						}
					}
					else if (num12 != 1 && num12 != 14 && num12 != 11 && num12 != 12)
					{
						float num46 = Math.Abs((float)(num13 * 16 + 8) - (Main.screenPosition.X + (float)(Main.screenWidth / 2)));
						float num47 = Math.Abs((float)(num14 * 16 + 8) - (Main.screenPosition.Y + (float)(Main.screenHeight / 2)));
						if (num46 < (float)(Main.screenWidth * 2) && num47 < (float)(Main.screenHeight * 2))
						{
							float num48 = (float)Math.Sqrt(num46 * num46 + num47 * num47);
							float num49 = 1f - num48 / ((float)Main.screenWidth * 0.75f);
							if (num49 > 0f)
							{
								num += num49;
							}
						}
						if (num46 < num2)
						{
							num2 = num46;
							num4 = num13 * 16 + 8;
						}
						if (num47 < num3)
						{
							num3 = num46;
							num5 = num14 * 16 + 8;
						}
					}
					if (k > 50 && (color6.R > 20 || color6.B > 20 || color6.G > 20))
					{
						float num50 = (int)color6.R;
						if ((float)(int)color6.G > num50)
						{
							num50 = (int)color6.G;
						}
						if ((float)(int)color6.B > num50)
						{
							num50 = (int)color6.B;
						}
						if ((float)Main.rand.Next(20000) < num50 / 30f)
						{
							int num51 = Dust.NewDust(new Vector2(num13 * 16 - num17 * 7, num14 * 16 + 6), 10, 8, 43, 0f, 0f, 254, Color.White, 0.5f);
							Main.dust[num51].velocity *= 0f;
						}
					}
					if (num15 == 0 && num34 != 0 && num16 == 1 && num17 != num18)
					{
						num34 = 0;
						num17 = num18;
						color5 = Color.White;
						if (num17 == 1)
						{
							spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16 - 16, num14 * 16 + 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 16 - num33), color5, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
						}
						else
						{
							spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16 - 16, num14 * 16 + 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 8), color5, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
						}
					}
					if (num19 != 0 && num31 == 0 && num32 == 1)
					{
						if (num17 == 1)
						{
							if (num20 != num12)
							{
								spriteBatch.Draw(waterfallTexture[num20], new Vector2(num13 * 16, num14 * 16 + num11 + 8) - Main.screenPosition, new Rectangle(num21, 0, 16, 16 - num33 - 8), color4, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
							}
							else
							{
								spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16, num14 * 16 + num11 + 8) - Main.screenPosition, new Rectangle(num21, 0, 16, 16 - num33 - 8), color5, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0f);
							}
						}
						else
						{
							spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16, num14 * 16 + num11 + 8) - Main.screenPosition, new Rectangle(num21, 0, 16, 16 - num33 - 8), color5, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
						}
					}
					if (num11 == 8 && num16 == 1 && num19 == 0)
					{
						if (num18 == -1)
						{
							if (num20 != num12)
							{
								spriteBatch.Draw(waterfallTexture[num20], new Vector2(num13 * 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 8), color4, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
							}
							else
							{
								spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 8), color5, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
							}
						}
						else if (num20 != num12)
						{
							spriteBatch.Draw(waterfallTexture[num20], new Vector2(num13 * 16 - 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 8), color4, 0f, default(Vector2), 1f, SpriteEffects.FlipHorizontally, 0f);
						}
						else
						{
							spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16 - 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 8), color5, 0f, default(Vector2), 1f, SpriteEffects.FlipHorizontally, 0f);
						}
					}
					if (num34 != 0 && num15 == 0)
					{
						if (num35 == 1)
						{
							if (num20 != num12)
							{
								spriteBatch.Draw(waterfallTexture[num20], new Vector2(num13 * 16 - 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 16 - num33), color4, 0f, default(Vector2), 1f, SpriteEffects.FlipHorizontally, 0f);
							}
							else
							{
								spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16 - 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 16 - num33), color5, 0f, default(Vector2), 1f, SpriteEffects.FlipHorizontally, 0f);
							}
						}
						else if (num20 != num12)
						{
							spriteBatch.Draw(waterfallTexture[num20], new Vector2(num13 * 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 16 - num33), color4, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
						}
						else
						{
							spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 16 - num33), color5, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
						}
					}
					if (num32 == 1 && num34 == 0 && num19 == 0)
					{
						if (num17 == -1)
						{
							if (num16 == 0)
							{
								spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16, num14 * 16 + num11) - Main.screenPosition, new Rectangle(num21, 0, 16, 16 - num33), color5, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
							}
							else if (num20 != num12)
							{
								spriteBatch.Draw(waterfallTexture[num20], new Vector2(num13 * 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 16 - num33), color4, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
							}
							else
							{
								spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 16 - num33), color5, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
							}
						}
						else if (num16 == 0)
						{
							spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16, num14 * 16 + num11) - Main.screenPosition, new Rectangle(num21, 0, 16, 16 - num33), color5, 0f, default(Vector2), 1f, SpriteEffects.FlipHorizontally, 0f);
						}
						else if (num20 != num12)
						{
							spriteBatch.Draw(waterfallTexture[num20], new Vector2(num13 * 16 - 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 16 - num33), color4, 0f, default(Vector2), 1f, SpriteEffects.FlipHorizontally, 0f);
						}
						else
						{
							spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16 - 16, num14 * 16) - Main.screenPosition, new Rectangle(num21, 24, 32, 16 - num33), color5, 0f, default(Vector2), 1f, SpriteEffects.FlipHorizontally, 0f);
						}
					}
					else
					{
						switch (num31)
						{
						case 1:
							if (Main.tile[num13, num14].liquid > 0 && !Main.tile[num13, num14].halfBrick())
							{
								break;
							}
							if (num34 == 1)
							{
								for (int m = 0; m < 8; m++)
								{
									int num55 = m * 2;
									int num56 = 14 - m * 2;
									int num57 = num55;
									num11 = 8;
									if (num15 == 0 && m < 2)
									{
										num57 = 4;
									}
									spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16 + num55, num14 * 16 + num11 + num57) - Main.screenPosition, new Rectangle(16 + num21 + num56, 0, 2, 16 - num11), color5, 0f, default(Vector2), 1f, SpriteEffects.FlipHorizontally, 0f);
								}
							}
							else
							{
								spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16, num14 * 16 + num11) - Main.screenPosition, new Rectangle(16 + num21, 0, 16, 16), color5, 0f, default(Vector2), 1f, SpriteEffects.FlipHorizontally, 0f);
							}
							break;
						case -1:
							if (Main.tile[num13, num14].liquid > 0 && !Main.tile[num13, num14].halfBrick())
							{
								break;
							}
							if (num34 == -1)
							{
								for (int l = 0; l < 8; l++)
								{
									int num52 = l * 2;
									int num53 = l * 2;
									int num54 = 14 - l * 2;
									num11 = 8;
									if (num15 == 0 && l > 5)
									{
										num54 = 4;
									}
									spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16 + num52, num14 * 16 + num11 + num54) - Main.screenPosition, new Rectangle(16 + num21 + num53, 0, 2, 16 - num11), color5, 0f, default(Vector2), 1f, SpriteEffects.FlipHorizontally, 0f);
								}
							}
							else
							{
								spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16, num14 * 16 + num11) - Main.screenPosition, new Rectangle(16 + num21, 0, 16, 16), color5, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
							}
							break;
						case 0:
							if (num32 == 0)
							{
								if (Main.tile[num13, num14].liquid <= 0 || Main.tile[num13, num14].halfBrick())
								{
									spriteBatch.Draw(waterfallTexture[num12], new Vector2(num13 * 16, num14 * 16 + num11) - Main.screenPosition, new Rectangle(16 + num21, 0, 16, 16), color5, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
								}
								k = 1000;
							}
							break;
						}
					}
					if (tile2.liquid > 0 && !tile2.halfBrick())
					{
						k = 1000;
					}
					num16 = num32;
					num18 = num17;
					num15 = num31;
					num13 += num31;
					num14 += num32;
					num19 = num34;
					color4 = color5;
					if (num20 != num12)
					{
						num20 = num12;
					}
					if ((tile3.active() && (tile3.type == 189 || tile3.type == 196)) || (tile5.active() && (tile5.type == 189 || tile5.type == 196)) || (tile4.active() && (tile4.type == 189 || tile4.type == 196)))
					{
						num22 = (int)((float)(40 * (Main.maxTilesX / 4200)) * Main.gfxQuality);
					}
				}
			}
			Main.ambientWaterfallX = num4;
			Main.ambientWaterfallY = num5;
			Main.ambientWaterfallStrength = num;
			Main.ambientLavafallX = num9;
			Main.ambientLavafallY = num10;
			Main.ambientLavafallStrength = num6;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < currentMax; i++)
			{
				waterfalls[i].stopAtStep = waterfallDist;
			}
			Main.drewLava = false;
			if (Main.liquidAlpha[0] > 0f)
			{
				DrawWaterfall(spriteBatch, 0, Main.liquidAlpha[0]);
			}
			if (Main.liquidAlpha[2] > 0f)
			{
				DrawWaterfall(spriteBatch, 3, Main.liquidAlpha[2]);
			}
			if (Main.liquidAlpha[3] > 0f)
			{
				DrawWaterfall(spriteBatch, 4, Main.liquidAlpha[3]);
			}
			if (Main.liquidAlpha[4] > 0f)
			{
				DrawWaterfall(spriteBatch, 5, Main.liquidAlpha[4]);
			}
			if (Main.liquidAlpha[5] > 0f)
			{
				DrawWaterfall(spriteBatch, 6, Main.liquidAlpha[5]);
			}
			if (Main.liquidAlpha[6] > 0f)
			{
				DrawWaterfall(spriteBatch, 7, Main.liquidAlpha[6]);
			}
			if (Main.liquidAlpha[7] > 0f)
			{
				DrawWaterfall(spriteBatch, 8, Main.liquidAlpha[7]);
			}
			if (Main.liquidAlpha[8] > 0f)
			{
				DrawWaterfall(spriteBatch, 9, Main.liquidAlpha[8]);
			}
			if (Main.liquidAlpha[9] > 0f)
			{
				DrawWaterfall(spriteBatch, 10, Main.liquidAlpha[9]);
			}
			if (Main.liquidAlpha[10] > 0f)
			{
				DrawWaterfall(spriteBatch, 13, Main.liquidAlpha[10]);
			}
		}
	}
}
