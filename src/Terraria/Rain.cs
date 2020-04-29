using Microsoft.Xna.Framework;
using System;

namespace Terraria
{
	public class Rain
	{
		public Vector2 position;

		public Vector2 velocity;

		public float scale;

		public float rotation;

		public int alpha;

		public bool active;

		public byte type;

		public static void MakeRain()
		{
			if ((double)Main.screenPosition.Y > Main.worldSurface * 16.0 || Main.gameMenu)
			{
				return;
			}
			float num = (float)Main.screenWidth / 1920f;
			num *= 25f;
			num *= 0.25f + 1f * Main.cloudAlpha;
			Vector2 vector = default(Vector2);
			for (int i = 0; (float)i < num; i++)
			{
				int num2 = 600;
				if (Main.player[Main.myPlayer].velocity.Y < 0f)
				{
					num2 += (int)(Math.Abs(Main.player[Main.myPlayer].velocity.Y) * 30f);
				}
				vector.X = Main.rand.Next((int)Main.screenPosition.X - num2, (int)Main.screenPosition.X + Main.screenWidth + num2);
				vector.Y = Main.screenPosition.Y - (float)Main.rand.Next(20, 100);
				vector.X -= Main.windSpeed * 15f * 40f;
				vector.X += Main.player[Main.myPlayer].velocity.X * 40f;
				if (vector.X < 0f)
				{
					vector.X = 0f;
				}
				if (vector.X > (float)((Main.maxTilesX - 1) * 16))
				{
					vector.X = (Main.maxTilesX - 1) * 16;
				}
				int num3 = (int)vector.X / 16;
				int num4 = (int)vector.Y / 16;
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num3 > Main.maxTilesX - 1)
				{
					num3 = Main.maxTilesX - 1;
				}
				if (Main.gameMenu || (!WorldGen.SolidTile(num3, num4) && Main.tile[num3, num4].wall <= 0))
				{
					NewRain(Velocity: new Vector2(Main.windSpeed * 12f, 14f), Position: vector);
				}
			}
		}

		public void Update()
		{
			position += velocity;
			if (!Collision.SolidCollision(position, 2, 2) && !(position.Y > Main.screenPosition.Y + (float)Main.screenHeight + 100f) && !Collision.WetCollision(position, 2, 2))
			{
				return;
			}
			active = false;
			if ((float)Main.rand.Next(100) < Main.gfxQuality * 100f)
			{
				int num = 154;
				if (type == 3 || type == 4 || type == 5)
				{
					num = 218;
				}
				int num2 = Dust.NewDust(position - velocity, 2, 2, num);
				Main.dust[num2].position.X -= 2f;
				Main.dust[num2].alpha = 38;
				Main.dust[num2].velocity *= 0.1f;
				Main.dust[num2].velocity += -velocity * 0.025f;
				Main.dust[num2].scale = 0.75f;
			}
		}

		public static int NewRain(Vector2 Position, Vector2 Velocity)
		{
			int num = -1;
			int num2 = (int)((float)Main.maxRain * Main.cloudAlpha);
			if (num2 > Main.maxRain)
			{
				num2 = Main.maxRain;
			}
			float num3 = (1f + Main.gfxQuality) / 2f;
			if ((double)num3 < 0.9)
			{
				num2 = (int)((float)num2 * num3);
			}
			float num4 = 800 - Main.snowTiles;
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			num4 /= 800f;
			num2 = (int)((float)num2 * num4);
			for (int i = 0; i < num2; i++)
			{
				if (!Main.rain[i].active)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return Main.maxRain;
			}
			Rain rain = Main.rain[num];
			rain.active = true;
			rain.position = Position;
			rain.scale = 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
			rain.velocity = Velocity * rain.scale;
			rain.rotation = (float)Math.Atan2(rain.velocity.X, 0f - rain.velocity.Y);
			rain.type = (byte)Main.rand.Next(3);
			if (Main.bloodMoon)
			{
				rain.type += 3;
			}
			return num;
		}
	}
}
