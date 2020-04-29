using Microsoft.Xna.Framework;
using System;
using Terraria.GameContent;

namespace Terraria
{
	public class Gore
	{
		public static int goreTime = 600;

		public Vector2 position;

		public Vector2 velocity;

		public float rotation;

		public float scale;

		public int alpha;

		public int type;

		public float light;

		public bool active;

		public bool sticky = true;

		public int timeLeft = goreTime;

		public bool behindTiles;

		public byte frame;

		public byte frameCounter;

		public byte numFrames = 1;

		public void Update()
		{
			if (Main.netMode == 2 || !active)
			{
				return;
			}
			if (type >= 276 && type <= 282)
			{
				velocity.X *= 0.98f;
				velocity.Y *= 0.98f;
				if (velocity.Y < scale)
				{
					velocity.Y += 0.05f;
				}
				if ((double)velocity.Y > 0.1)
				{
					if (velocity.X > 0f)
					{
						rotation += 0.01f;
					}
					else
					{
						rotation -= 0.01f;
					}
				}
			}
			if (type >= 570 && type <= 572)
			{
				scale -= 0.001f;
				if ((double)scale <= 0.01)
				{
					scale = 0.01f;
					goreTime = 0;
				}
				sticky = false;
				rotation = velocity.X * 0.1f;
			}
			else if (type >= 706 && type <= 717)
			{
				if ((double)position.Y < Main.worldSurface * 16.0 + 8.0)
				{
					alpha = 0;
				}
				else
				{
					alpha = 100;
				}
				int num = 4;
				frameCounter++;
				if (frame <= 4)
				{
					int num2 = (int)(position.X / 16f);
					int num3 = (int)(position.Y / 16f) - 1;
					if (WorldGen.InWorld(num2, num3) && !Main.tile[num2, num3].active())
					{
						active = false;
					}
					if (frame == 0)
					{
						num = 24 + Main.rand.Next(256);
					}
					if (frame == 1)
					{
						num = 24 + Main.rand.Next(256);
					}
					if (frame == 2)
					{
						num = 24 + Main.rand.Next(256);
					}
					if (frame == 3)
					{
						num = 24 + Main.rand.Next(96);
					}
					if (frame == 5)
					{
						num = 16 + Main.rand.Next(64);
					}
					if (type == 716)
					{
						num *= 2;
					}
					if (type == 717)
					{
						num *= 4;
					}
					if (frameCounter >= num)
					{
						frameCounter = 0;
						frame++;
						if (frame == 5)
						{
							int num4 = NewGore(position, velocity, type);
							Main.gore[num4].frame = 9;
							Main.gore[num4].velocity *= 0f;
						}
					}
				}
				else if (frame <= 6)
				{
					num = 8;
					if (type == 716)
					{
						num *= 2;
					}
					if (type == 717)
					{
						num *= 3;
					}
					if (frameCounter >= num)
					{
						frameCounter = 0;
						frame++;
						if (frame == 7)
						{
							active = false;
						}
					}
				}
				else if (frame <= 9)
				{
					num = 6;
					if (type == 716)
					{
						num = (int)((double)num * 1.5);
						velocity.Y += 0.175f;
					}
					else if (type == 717)
					{
						num *= 2;
						velocity.Y += 0.15f;
					}
					else
					{
						velocity.Y += 0.2f;
					}
					if ((double)velocity.Y < 0.5)
					{
						velocity.Y = 0.5f;
					}
					if (velocity.Y > 12f)
					{
						velocity.Y = 12f;
					}
					if (frameCounter >= num)
					{
						frameCounter = 0;
						frame++;
					}
					if (frame > 9)
					{
						frame = 7;
					}
				}
				else
				{
					if (type == 716)
					{
						num *= 2;
					}
					else if (type == 717)
					{
						num *= 6;
					}
					velocity.Y += 0.1f;
					if (frameCounter >= num)
					{
						frameCounter = 0;
						frame++;
					}
					velocity *= 0f;
					if (frame > 14)
					{
						active = false;
					}
				}
			}
			else if (type == 11 || type == 12 || type == 13 || type == 61 || type == 62 || type == 63 || type == 99 || type == 220 || type == 221 || type == 222 || (type >= 375 && type <= 377) || (type >= 435 && type <= 437) || (type >= 861 && type <= 862))
			{
				velocity.Y *= 0.98f;
				velocity.X *= 0.98f;
				scale -= 0.007f;
				if ((double)scale < 0.1)
				{
					scale = 0.1f;
					alpha = 255;
				}
			}
			else if (type == 16 || type == 17)
			{
				velocity.Y *= 0.98f;
				velocity.X *= 0.98f;
				scale -= 0.01f;
				if ((double)scale < 0.1)
				{
					scale = 0.1f;
					alpha = 255;
				}
			}
			else if (type == 331)
			{
				alpha += 5;
				velocity.Y *= 0.95f;
				velocity.X *= 0.95f;
				rotation = velocity.X * 0.1f;
			}
			else if (type != 860 && type != 892 && type != 893 && (type < 825 || type > 827) && (type < 411 || type > 430))
			{
				velocity.Y += 0.2f;
			}
			rotation += velocity.X * 0.1f;
			if (type >= 580 && type <= 582)
			{
				rotation = 0f;
				velocity.X *= 0.95f;
			}
			if (type >= 825 && type <= 827)
			{
				if (timeLeft < 60)
				{
					alpha += Main.rand.Next(1, 7);
				}
				else if (alpha > 100)
				{
					alpha -= Main.rand.Next(1, 4);
				}
				if (alpha < 0)
				{
					alpha = 0;
				}
				if (alpha > 255)
				{
					timeLeft = 0;
				}
				velocity.X = (velocity.X * 50f + Main.windSpeed * 2f + (float)Main.rand.Next(-10, 11) * 0.1f) / 51f;
				float num5 = 0f;
				if (velocity.X < 0f)
				{
					num5 = velocity.X * 0.2f;
				}
				velocity.Y = (velocity.Y * 50f + -0.35f + num5 + (float)Main.rand.Next(-10, 11) * 0.2f) / 51f;
				rotation = velocity.X * 0.6f;
				float num6 = -1f;
				if (Main.goreLoaded[type])
				{
					Rectangle rectangle = new Rectangle((int)position.X, (int)position.Y, (int)((float)Main.goreTexture[type].Width * scale), (int)((float)Main.goreTexture[type].Height * scale));
					for (int i = 0; i < 16; i++)
					{
						if (Main.player[i].active && !Main.player[i].dead)
						{
							Rectangle value = new Rectangle((int)Main.player[i].position.X, (int)Main.player[i].position.Y, Main.player[i].width, Main.player[i].height);
							if (rectangle.Intersects(value))
							{
								timeLeft = 0;
								num6 = Main.player[i].velocity.Length();
								break;
							}
						}
					}
				}
				if (timeLeft > 0)
				{
					if (Main.rand.Next(2) == 0)
					{
						timeLeft--;
					}
					if (Main.rand.Next(50) == 0)
					{
						timeLeft -= 5;
					}
					if (Main.rand.Next(100) == 0)
					{
						timeLeft -= 10;
					}
				}
				else
				{
					alpha = 255;
					if (Main.goreLoaded[type] && num6 != -1f)
					{
						float num7 = (float)Main.goreTexture[type].Width * scale * 0.8f;
						float x = position.X;
						float y = position.Y;
						float num8 = (float)Main.goreTexture[type].Width * scale;
						float num9 = (float)Main.goreTexture[type].Height * scale;
						int num10 = 31;
						for (int j = 0; (float)j < num7; j++)
						{
							int num11 = Dust.NewDust(new Vector2(x, y), (int)num8, (int)num9, num10);
							Main.dust[num11].velocity *= (1f + num6) / 3f;
							Main.dust[num11].noGravity = true;
							Main.dust[num11].alpha = 100;
							Main.dust[num11].scale = scale;
						}
					}
				}
			}
			if (type >= 411 && type <= 430)
			{
				alpha = 50;
				velocity.X = (velocity.X * 50f + Main.windSpeed * 2f + (float)Main.rand.Next(-10, 11) * 0.1f) / 51f;
				velocity.Y = (velocity.Y * 50f + -0.25f + (float)Main.rand.Next(-10, 11) * 0.2f) / 51f;
				rotation = velocity.X * 0.3f;
				if (Main.goreLoaded[type])
				{
					Rectangle rectangle2 = new Rectangle((int)position.X, (int)position.Y, (int)((float)Main.goreTexture[type].Width * scale), (int)((float)Main.goreTexture[type].Height * scale));
					for (int k = 0; k < 16; k++)
					{
						if (Main.player[k].active && !Main.player[k].dead)
						{
							Rectangle value2 = new Rectangle((int)Main.player[k].position.X, (int)Main.player[k].position.Y, Main.player[k].width, Main.player[k].height);
							if (rectangle2.Intersects(value2))
							{
								timeLeft = 0;
							}
						}
					}
					if (Collision.SolidCollision(position, (int)((float)Main.goreTexture[type].Width * scale), (int)((float)Main.goreTexture[type].Height * scale)))
					{
						timeLeft = 0;
					}
				}
				if (timeLeft > 0)
				{
					if (Main.rand.Next(2) == 0)
					{
						timeLeft--;
					}
					if (Main.rand.Next(50) == 0)
					{
						timeLeft -= 5;
					}
					if (Main.rand.Next(100) == 0)
					{
						timeLeft -= 10;
					}
				}
				else
				{
					alpha = 255;
					if (Main.goreLoaded[type])
					{
						float num12 = (float)Main.goreTexture[type].Width * scale * 0.8f;
						float x2 = position.X;
						float y2 = position.Y;
						float num13 = (float)Main.goreTexture[type].Width * scale;
						float num14 = (float)Main.goreTexture[type].Height * scale;
						int num15 = 176;
						if (type >= 416 && type <= 420)
						{
							num15 = 177;
						}
						if (type >= 421 && type <= 425)
						{
							num15 = 178;
						}
						if (type >= 426 && type <= 430)
						{
							num15 = 179;
						}
						for (int l = 0; (float)l < num12; l++)
						{
							int num16 = Dust.NewDust(new Vector2(x2, y2), (int)num13, (int)num14, num15);
							Main.dust[num16].noGravity = true;
							Main.dust[num16].alpha = 100;
							Main.dust[num16].scale = scale;
						}
					}
				}
			}
			else if (type != 860 && type != 892 && type != 893)
			{
				if (type >= 706 && type <= 717)
				{
					if (type == 716)
					{
						float num17 = 1f;
						float num18 = 1f;
						float num19 = 1f;
						float num20 = 0.6f;
						num20 = ((frame == 0) ? (num20 * 0.1f) : ((frame == 1) ? (num20 * 0.2f) : ((frame == 2) ? (num20 * 0.3f) : ((frame == 3) ? (num20 * 0.4f) : ((frame == 4) ? (num20 * 0.5f) : ((frame == 5) ? (num20 * 0.4f) : ((frame == 6) ? (num20 * 0.2f) : ((frame <= 9) ? (num20 * 0.5f) : ((frame == 10) ? (num20 * 0.5f) : ((frame == 11) ? (num20 * 0.4f) : ((frame == 12) ? (num20 * 0.3f) : ((frame == 13) ? (num20 * 0.2f) : ((frame != 14) ? 0f : (num20 * 0.1f))))))))))))));
						num17 = 1f * num20;
						num18 = 0.5f * num20;
						num19 = 0.1f * num20;
						Lighting.AddLight(position + new Vector2(8f, 8f), num17, num18, num19);
					}
					Vector2 value3 = velocity;
					velocity = Collision.TileCollision(position, velocity, 16, 14);
					if (velocity != value3)
					{
						if (frame < 10)
						{
							frame = 10;
							frameCounter = 0;
							if (type != 716 && type != 717)
							{
								Main.PlaySound(39, (int)position.X + 8, (int)position.Y + 8, Main.rand.Next(2));
							}
						}
					}
					else if (Collision.WetCollision(position + velocity, 16, 14))
					{
						if (frame < 10)
						{
							frame = 10;
							frameCounter = 0;
							if (type != 716 && type != 717)
							{
								Main.PlaySound(39, (int)position.X + 8, (int)position.Y + 8, 2);
							}
						}
						int num21 = (int)(position.X + 8f) / 16;
						int num22 = (int)(position.Y + 14f) / 16;
						if (Main.tile[num21, num22] != null && Main.tile[num21, num22].liquid > 0)
						{
							velocity *= 0f;
							position.Y = num22 * 16 - (int)Main.tile[num21, num22].liquid / 16;
						}
					}
				}
				else if (sticky)
				{
					int num23 = 32;
					if (Main.goreLoaded[type])
					{
						num23 = Main.goreTexture[type].Width;
						if (Main.goreTexture[type].Height < num23)
						{
							num23 = Main.goreTexture[type].Height;
						}
					}
					num23 = (int)((float)num23 * 0.9f);
					velocity = Collision.TileCollision(position, velocity, (int)((float)num23 * scale), (int)((float)num23 * scale));
					if (velocity.Y == 0f)
					{
						velocity.X *= 0.97f;
						if ((double)velocity.X > -0.01 && (double)velocity.X < 0.01)
						{
							velocity.X = 0f;
						}
					}
					if (timeLeft > 0)
					{
						timeLeft--;
					}
					else
					{
						alpha++;
					}
				}
				else
				{
					alpha += 2;
				}
			}
			if (type == 860 || type == 892 || type == 893)
			{
				if (velocity.Y < 0f)
				{
					Vector2 vector = new Vector2(velocity.X, 0.6f);
					int num24 = 32;
					if (Main.goreLoaded[type])
					{
						num24 = Main.goreTexture[type].Width;
						if (Main.goreTexture[type].Height < num24)
						{
							num24 = Main.goreTexture[type].Height;
						}
					}
					num24 = (int)((float)num24 * 0.9f);
					vector = Collision.TileCollision(position, vector, (int)((float)num24 * scale), (int)((float)num24 * scale));
					vector.X *= 0.97f;
					if ((double)vector.X > -0.01 && (double)vector.X < 0.01)
					{
						vector.X = 0f;
					}
					if (timeLeft > 0)
					{
						timeLeft--;
					}
					else
					{
						alpha++;
					}
					velocity.X = vector.X;
				}
				else
				{
					velocity.Y += (float)Math.PI / 60f;
					Vector2 vector2 = new Vector2(Vector2.UnitY.RotatedBy(velocity.Y).X * 2f, Math.Abs(Vector2.UnitY.RotatedBy(velocity.Y).Y) * 3f);
					vector2 *= 2f;
					int num25 = 32;
					if (Main.goreLoaded[type])
					{
						num25 = Main.goreTexture[type].Width;
						if (Main.goreTexture[type].Height < num25)
						{
							num25 = Main.goreTexture[type].Height;
						}
					}
					Vector2 value4 = vector2;
					vector2 = Collision.TileCollision(position, vector2, (int)((float)num25 * scale), (int)((float)num25 * scale));
					if (vector2 != value4)
					{
						velocity.Y = -1f;
					}
					position += vector2;
					rotation = vector2.ToRotation() + (float)Math.PI;
					if (timeLeft > 0)
					{
						timeLeft--;
					}
					else
					{
						alpha++;
					}
				}
			}
			else
			{
				position += velocity;
			}
			if (alpha >= 255)
			{
				active = false;
			}
			if (light > 0f)
			{
				float num26 = light * scale;
				float num27 = light * scale;
				float num28 = light * scale;
				if (type == 16)
				{
					num28 *= 0.3f;
					num27 *= 0.8f;
				}
				else if (type == 17)
				{
					num27 *= 0.6f;
					num26 *= 0.3f;
				}
				if (Main.goreLoaded[type])
				{
					Lighting.AddLight((int)((position.X + (float)Main.goreTexture[type].Width * scale / 2f) / 16f), (int)((position.Y + (float)Main.goreTexture[type].Height * scale / 2f) / 16f), num26, num27, num28);
				}
				else
				{
					Lighting.AddLight((int)((position.X + 32f * scale / 2f) / 16f), (int)((position.Y + 32f * scale / 2f) / 16f), num26, num27, num28);
				}
			}
		}

		public static int NewGore(Vector2 Position, Vector2 Velocity, int Type, float Scale = 1f)
		{
			if (Main.netMode == 2)
			{
				return 500;
			}
			if (Main.gamePaused)
			{
				return 500;
			}
			if (Main.rand == null)
			{
				Main.rand = new Random();
			}
			int num = 500;
			for (int i = 0; i < 500; i++)
			{
				if (!Main.gore[i].active)
				{
					num = i;
					break;
				}
			}
			if (num == 500)
			{
				return num;
			}
			Main.gore[num].numFrames = 1;
			Main.gore[num].frame = 0;
			Main.gore[num].frameCounter = 0;
			Main.gore[num].behindTiles = false;
			Main.gore[num].light = 0f;
			Main.gore[num].position = Position;
			Main.gore[num].velocity = Velocity;
			Main.gore[num].velocity.Y -= (float)Main.rand.Next(10, 31) * 0.1f;
			Main.gore[num].velocity.X += (float)Main.rand.Next(-20, 21) * 0.1f;
			Main.gore[num].type = Type;
			Main.gore[num].active = true;
			Main.gore[num].alpha = 0;
			Main.gore[num].rotation = 0f;
			Main.gore[num].scale = Scale;
			if (!ChildSafety.Disabled && ChildSafety.DangerousGore(Type))
			{
				Main.gore[num].type = Main.rand.Next(11, 14);
				Main.gore[num].scale = Main.rand.NextFloat() * 0.5f + 0.5f;
				Main.gore[num].velocity /= 2f;
			}
			if (goreTime == 0 || Type == 11 || Type == 12 || Type == 13 || Type == 16 || Type == 17 || Type == 61 || Type == 62 || Type == 63 || Type == 99 || Type == 220 || Type == 221 || Type == 222 || Type == 435 || Type == 436 || Type == 437 || (Type >= 861 && Type <= 862))
			{
				Main.gore[num].sticky = false;
			}
			else if (Type >= 375 && Type <= 377)
			{
				Main.gore[num].sticky = false;
				Main.gore[num].alpha = 100;
			}
			else
			{
				Main.gore[num].sticky = true;
				Main.gore[num].timeLeft = goreTime;
			}
			if (Type >= 706 && Type <= 717)
			{
				Main.gore[num].numFrames = 15;
				Main.gore[num].behindTiles = true;
				Main.gore[num].timeLeft = goreTime * 3;
			}
			if (Type == 16 || Type == 17)
			{
				Main.gore[num].alpha = 100;
				Main.gore[num].scale = 0.7f;
				Main.gore[num].light = 1f;
			}
			if (Type >= 570 && Type <= 572)
			{
				Main.gore[num].velocity = Velocity;
			}
			if (Type == 860 || Type == 892 || Type == 893)
			{
				Main.gore[num].velocity = new Vector2((Main.rand.NextFloat() - 0.5f) * 3f, Main.rand.NextFloat() * ((float)Math.PI * 2f));
			}
			if (Type >= 411 && Type <= 430 && Main.goreLoaded[Type])
			{
				Main.gore[num].position.X = Position.X - (float)(Main.goreTexture[Type].Width / 2) * Scale;
				Main.gore[num].position.Y = Position.Y - (float)Main.goreTexture[Type].Height * Scale;
				Main.gore[num].velocity.Y *= (float)Main.rand.Next(90, 150) * 0.01f;
				Main.gore[num].velocity.X *= (float)Main.rand.Next(40, 90) * 0.01f;
				int num2 = Main.rand.Next(4) * 5;
				Main.gore[num].type += num2;
				Main.gore[num].timeLeft = Main.rand.Next(goreTime / 2, goreTime * 2);
			}
			if (Type >= 825 && Type <= 827)
			{
				Main.gore[num].sticky = false;
				if (Main.goreLoaded[Type])
				{
					Main.gore[num].alpha = 150;
					Main.gore[num].velocity = Velocity;
					Main.gore[num].position.X = Position.X - (float)(Main.goreTexture[Type].Width / 2) * Scale;
					Main.gore[num].position.Y = Position.Y - (float)Main.goreTexture[Type].Height * Scale / 2f;
					Main.gore[num].timeLeft = Main.rand.Next(goreTime / 2, goreTime + 1);
				}
			}
			return num;
		}

		public Color GetAlpha(Color newColor)
		{
			float num = (float)(255 - alpha) / 255f;
			int r;
			int g;
			int b;
			if (type == 16 || type == 17)
			{
				r = newColor.R;
				g = newColor.G;
				b = newColor.B;
			}
			else
			{
				if (type == 716)
				{
					return new Color(255, 255, 255, 200);
				}
				if (type >= 570 && type <= 572)
				{
					byte b2 = (byte)(255 - alpha);
					return new Color(b2, b2, b2, (int)b2 / 2);
				}
				if (type == 331)
				{
					return new Color(255, 255, 255, 50);
				}
				r = (int)((float)(int)newColor.R * num);
				g = (int)((float)(int)newColor.G * num);
				b = (int)((float)(int)newColor.B * num);
			}
			int num2 = newColor.A - alpha;
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > 255)
			{
				num2 = 255;
			}
			return new Color(r, g, b, num2);
		}
	}
}
