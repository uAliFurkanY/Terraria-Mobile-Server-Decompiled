using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Achievements;
using Terraria.Graphics;

namespace Terraria.UI
{
	public class AchievementCompleteUI
	{
		private class DrawCache
		{
			private const int _iconSize = 64;

			private const int _iconSizeWithSpace = 66;

			private const int _iconsPerRow = 8;

			public Achievement theAchievement;

			public int IconIndex;

			public Rectangle Frame;

			public string Title;

			public int TimeLeft;

			public float Scale
			{
				get
				{
					if (TimeLeft < 30)
					{
						return MathHelper.Lerp(0f, 1f, (float)TimeLeft / 30f);
					}
					if (TimeLeft > 285)
					{
						return MathHelper.Lerp(1f, 0f, ((float)TimeLeft - 285f) / 15f);
					}
					return 1f;
				}
			}

			public float Alpha
			{
				get
				{
					float scale = Scale;
					if (scale <= 0.5f)
					{
						return 0f;
					}
					return (scale - 0.5f) / 0.5f;
				}
			}

			public void Update()
			{
				TimeLeft--;
				if (TimeLeft < 0)
				{
					TimeLeft = 0;
				}
			}

			public DrawCache(Achievement achievement)
			{
				theAchievement = achievement;
				Title = achievement.FriendlyName.Value;
				int num = IconIndex = Main.Achievements.GetIconIndex(achievement.Name);
				Frame = new Rectangle(num % 8 * 66, num / 8 * 66, 64, 64);
				TimeLeft = 300;
			}

			public void ApplyHeight(ref Vector2 v)
			{
				v.Y -= 50f * Alpha;
			}
		}

		private static Texture2D AchievementsTexture;

		private static Texture2D AchievementsTextureBorder;

		private static List<DrawCache> caches = new List<DrawCache>();

		public static void LoadContent()
		{
			AchievementsTexture = TextureManager.Load("Images/UI/Achievements");
			AchievementsTextureBorder = TextureManager.Load("Images/UI/Achievement_Borders");
		}

		public static void Initialize()
		{
			Main.Achievements.OnAchievementCompleted += AddCompleted;
		}

		public static void Draw(SpriteBatch sb)
		{
			Vector2 center = new Vector2(Main.screenWidth / 2, Main.screenHeight - 40);
			foreach (DrawCache cach in caches)
			{
				DrawAchievement(sb, ref center, cach);
				if (center.Y < -100f)
				{
					break;
				}
			}
		}

		public static void AddCompleted(Achievement achievement)
		{
			if (Main.netMode != 2)
			{
				caches.Add(new DrawCache(achievement));
			}
		}

		public static void Clear()
		{
			caches.Clear();
		}

		public static void Update()
		{
			foreach (DrawCache cach in caches)
			{
				cach.Update();
			}
			for (int i = 0; i < caches.Count; i++)
			{
				if (caches[i].TimeLeft == 0)
				{
					caches.Remove(caches[i]);
					i--;
				}
			}
		}

		private static void DrawAchievement(SpriteBatch sb, ref Vector2 center, DrawCache ach)
		{
			float alpha = ach.Alpha;
			if (alpha > 0f)
			{
				string title = ach.Title;
				Vector2 center2 = center;
				Vector2 value = Main.fontItemStack.MeasureString(title);
				float num = ach.Scale * 1.1f;
				Rectangle r = Utils.CenteredRectangle(center2, (value + new Vector2(58f, 10f)) * num);
				Vector2 mouseScreen = Main.MouseScreen;
				bool flag = r.Contains(mouseScreen.ToPoint());
				Color c = flag ? (new Color(64, 109, 164) * 0.75f) : (new Color(64, 109, 164) * 0.5f);
				Utils.DrawInvBG(sb, r, c);
				float num2 = num * 0.3f;
				Color value2 = new Color(Main.mouseTextColor, Main.mouseTextColor, (int)Main.mouseTextColor / 5, Main.mouseTextColor);
				Vector2 vector = r.Right() - Vector2.UnitX * num * (12f + num2 * (float)ach.Frame.Width);
				sb.Draw(AchievementsTexture, vector, ach.Frame, Color.White * alpha, 0f, new Vector2(0f, ach.Frame.Height / 2), num2, SpriteEffects.None, 0f);
				sb.Draw(AchievementsTextureBorder, vector, null, Color.White * alpha, 0f, new Vector2(0f, ach.Frame.Height / 2), num2, SpriteEffects.None, 0f);
				Utils.DrawBorderString(sb, title, vector - Vector2.UnitX * 10f, value2 * alpha, num * 0.9f, 1f, 0.4f);
				if (flag)
				{
					Main.player[Main.myPlayer].mouseInterface = true;
					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						AchievementsUI.OpenAndGoto(ach.theAchievement);
						ach.TimeLeft = 0;
					}
				}
			}
			ach.ApplyHeight(ref center);
		}
	}
}
