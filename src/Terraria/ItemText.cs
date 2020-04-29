using Microsoft.Xna.Framework;
using Terraria.Localization;

namespace Terraria
{
	public class ItemText
	{
		public Vector2 position;

		public Vector2 velocity;

		public float alpha;

		public int alphaDir = 1;

		public string name;

		public int stack;

		public float scale = 1f;

		public float rotation;

		public Color color;

		public bool active;

		public int lifeTime;

		public static int activeTime = 60;

		public static int numActive;

		public bool NoStack;

		public bool coinText;

		public int coinValue;

		public bool expert;

		public static void NewText(Item newItem, int stack, bool noStack = false, bool longText = false)
		{
			bool flag = newItem.type >= 71 && newItem.type <= 74;
			if (!Main.showItemText || newItem.Name == null || !newItem.active || Main.netMode == 2)
			{
				return;
			}
			for (int i = 0; i < 20; i++)
			{
				if (!Main.itemText[i].active || (!(Main.itemText[i].name == newItem.AffixName()) && (!flag || !Main.itemText[i].coinText)) || Main.itemText[i].NoStack || noStack)
				{
					continue;
				}
				string text = newItem.Name + " (" + (Main.itemText[i].stack + stack) + ")";
				string text2 = newItem.Name;
				if (Main.itemText[i].stack > 1)
				{
					object obj = text2;
					text2 = string.Concat(obj, " (", Main.itemText[i].stack, ")");
				}
				Vector2 vector = Main.fontMouseText.MeasureString(text2);
				vector = Main.fontMouseText.MeasureString(text);
				if (Main.itemText[i].lifeTime < 0)
				{
					Main.itemText[i].scale = 1f;
				}
				if (Main.itemText[i].lifeTime < 60)
				{
					Main.itemText[i].lifeTime = 60;
				}
				if (flag && Main.itemText[i].coinText)
				{
					int num = 0;
					if (newItem.type == 71)
					{
						num += newItem.stack;
					}
					else if (newItem.type == 72)
					{
						num += 100 * newItem.stack;
					}
					else if (newItem.type == 73)
					{
						num += 10000 * newItem.stack;
					}
					else if (newItem.type == 74)
					{
						num += 1000000 * newItem.stack;
					}
					Main.itemText[i].coinValue += num;
					text = ValueToName(Main.itemText[i].coinValue);
					vector = Main.fontMouseText.MeasureString(text);
					Main.itemText[i].name = text;
					if (Main.itemText[i].coinValue >= 1000000)
					{
						if (Main.itemText[i].lifeTime < 300)
						{
							Main.itemText[i].lifeTime = 300;
						}
						Main.itemText[i].color = new Color(220, 220, 198);
					}
					else if (Main.itemText[i].coinValue >= 10000)
					{
						if (Main.itemText[i].lifeTime < 240)
						{
							Main.itemText[i].lifeTime = 240;
						}
						Main.itemText[i].color = new Color(224, 201, 92);
					}
					else if (Main.itemText[i].coinValue >= 100)
					{
						if (Main.itemText[i].lifeTime < 180)
						{
							Main.itemText[i].lifeTime = 180;
						}
						Main.itemText[i].color = new Color(181, 192, 193);
					}
					else if (Main.itemText[i].coinValue >= 1)
					{
						if (Main.itemText[i].lifeTime < 120)
						{
							Main.itemText[i].lifeTime = 120;
						}
						Main.itemText[i].color = new Color(246, 138, 96);
					}
				}
				Main.itemText[i].stack += stack;
				Main.itemText[i].scale = 0f;
				Main.itemText[i].rotation = 0f;
				Main.itemText[i].position.X = newItem.position.X + (float)newItem.width * 0.5f - vector.X * 0.5f;
				Main.itemText[i].position.Y = newItem.position.Y + (float)newItem.height * 0.25f - vector.Y * 0.5f;
				Main.itemText[i].velocity.Y = -7f;
				if (Main.itemText[i].coinText)
				{
					Main.itemText[i].stack = 1;
				}
				return;
			}
			int num2 = -1;
			for (int j = 0; j < 20; j++)
			{
				if (!Main.itemText[j].active)
				{
					num2 = j;
					break;
				}
			}
			if (num2 == -1)
			{
				double num3 = Main.bottomWorld;
				for (int k = 0; k < 20; k++)
				{
					if (num3 > (double)Main.itemText[k].position.Y)
					{
						num2 = k;
						num3 = Main.itemText[k].position.Y;
					}
				}
			}
			if (num2 < 0)
			{
				return;
			}
			string text3 = newItem.AffixName();
			if (stack > 1)
			{
				object obj2 = text3;
				text3 = string.Concat(obj2, " (", stack, ")");
			}
			Vector2 vector2 = Main.fontMouseText.MeasureString(text3);
			Main.itemText[num2].alpha = 1f;
			Main.itemText[num2].alphaDir = -1;
			Main.itemText[num2].active = true;
			Main.itemText[num2].scale = 0f;
			Main.itemText[num2].NoStack = noStack;
			Main.itemText[num2].rotation = 0f;
			Main.itemText[num2].position.X = newItem.position.X + (float)newItem.width * 0.5f - vector2.X * 0.5f;
			Main.itemText[num2].position.Y = newItem.position.Y + (float)newItem.height * 0.25f - vector2.Y * 0.5f;
			Main.itemText[num2].color = Color.White;
			if (newItem.rare == 1)
			{
				Main.itemText[num2].color = new Color(150, 150, 255);
			}
			else if (newItem.rare == 2)
			{
				Main.itemText[num2].color = new Color(150, 255, 150);
			}
			else if (newItem.rare == 3)
			{
				Main.itemText[num2].color = new Color(255, 200, 150);
			}
			else if (newItem.rare == 4)
			{
				Main.itemText[num2].color = new Color(255, 150, 150);
			}
			else if (newItem.rare == 5)
			{
				Main.itemText[num2].color = new Color(255, 150, 255);
			}
			else if (newItem.rare == -11)
			{
				Main.itemText[num2].color = new Color(255, 175, 0);
			}
			else if (newItem.rare == -1)
			{
				Main.itemText[num2].color = new Color(130, 130, 130);
			}
			else if (newItem.rare == 6)
			{
				Main.itemText[num2].color = new Color(210, 160, 255);
			}
			else if (newItem.rare == 7)
			{
				Main.itemText[num2].color = new Color(150, 255, 10);
			}
			else if (newItem.rare == 8)
			{
				Main.itemText[num2].color = new Color(255, 255, 10);
			}
			else if (newItem.rare == 9)
			{
				Main.itemText[num2].color = new Color(5, 200, 255);
			}
			else if (newItem.rare == 10)
			{
				Main.itemText[num2].color = new Color(255, 40, 100);
			}
			else if (newItem.rare >= 11)
			{
				Main.itemText[num2].color = new Color(180, 40, 255);
			}
			Main.itemText[num2].expert = newItem.expert;
			Main.itemText[num2].name = newItem.AffixName();
			Main.itemText[num2].stack = stack;
			Main.itemText[num2].velocity.Y = -7f;
			Main.itemText[num2].lifeTime = 60;
			if (longText)
			{
				Main.itemText[num2].lifeTime *= 5;
			}
			Main.itemText[num2].coinValue = 0;
			Main.itemText[num2].coinText = (newItem.type >= 71 && newItem.type <= 74);
			if (!Main.itemText[num2].coinText)
			{
				return;
			}
			if (newItem.type == 71)
			{
				Main.itemText[num2].coinValue += Main.itemText[num2].stack;
			}
			else if (newItem.type == 72)
			{
				Main.itemText[num2].coinValue += 100 * Main.itemText[num2].stack;
			}
			else if (newItem.type == 73)
			{
				Main.itemText[num2].coinValue += 10000 * Main.itemText[num2].stack;
			}
			else if (newItem.type == 74)
			{
				Main.itemText[num2].coinValue += 1000000 * Main.itemText[num2].stack;
			}
			Main.itemText[num2].ValueToName();
			Main.itemText[num2].stack = 1;
			int num4 = num2;
			if (Main.itemText[num4].coinValue >= 1000000)
			{
				if (Main.itemText[num4].lifeTime < 300)
				{
					Main.itemText[num4].lifeTime = 300;
				}
				Main.itemText[num4].color = new Color(220, 220, 198);
			}
			else if (Main.itemText[num4].coinValue >= 10000)
			{
				if (Main.itemText[num4].lifeTime < 240)
				{
					Main.itemText[num4].lifeTime = 240;
				}
				Main.itemText[num4].color = new Color(224, 201, 92);
			}
			else if (Main.itemText[num4].coinValue >= 100)
			{
				if (Main.itemText[num4].lifeTime < 180)
				{
					Main.itemText[num4].lifeTime = 180;
				}
				Main.itemText[num4].color = new Color(181, 192, 193);
			}
			else if (Main.itemText[num4].coinValue >= 1)
			{
				if (Main.itemText[num4].lifeTime < 120)
				{
					Main.itemText[num4].lifeTime = 120;
				}
				Main.itemText[num4].color = new Color(246, 138, 96);
			}
		}

		private static string ValueToName(int coinValue)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			string text = "";
			int num5 = coinValue;
			while (num5 > 0)
			{
				if (num5 >= 1000000)
				{
					num5 -= 1000000;
					num++;
				}
				else if (num5 >= 10000)
				{
					num5 -= 10000;
					num2++;
				}
				else if (num5 >= 100)
				{
					num5 -= 100;
					num3++;
				}
				else if (num5 >= 1)
				{
					num5--;
					num4++;
				}
			}
			text = "";
			if (num > 0)
			{
				text = text + num + string.Format(" {0} ", Language.GetTextValue("Currency.Platinum"));
			}
			if (num2 > 0)
			{
				text = text + num2 + string.Format(" {0} ", Language.GetTextValue("Currency.Gold"));
			}
			if (num3 > 0)
			{
				text = text + num3 + string.Format(" {0} ", Language.GetTextValue("Currency.Silver"));
			}
			if (num4 > 0)
			{
				text = text + num4 + string.Format(" {0} ", Language.GetTextValue("Currency.Copper"));
			}
			if (text.Length > 1)
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		private void ValueToName()
		{
			name = ValueToName(coinValue);
		}

		public void Update(int whoAmI)
		{
			if (!active)
			{
				return;
			}
			alpha += (float)alphaDir * 0.01f;
			if ((double)alpha <= 0.7)
			{
				alpha = 0.7f;
				alphaDir = 1;
			}
			if (alpha >= 1f)
			{
				alpha = 1f;
				alphaDir = -1;
			}
			if (expert && expert)
			{
				color = new Color((byte)Main.DiscoR, (byte)Main.DiscoG, (byte)Main.DiscoB, Main.mouseTextColor);
			}
			bool flag = false;
			string text = name;
			if (stack > 1)
			{
				object obj = text;
				text = string.Concat(obj, " (", stack, ")");
			}
			Vector2 vector = Main.fontMouseText.MeasureString(text);
			vector *= scale;
			vector.Y *= 0.8f;
			Rectangle rectangle = new Rectangle((int)(position.X - vector.X / 2f), (int)(position.Y - vector.Y / 2f), (int)vector.X, (int)vector.Y);
			for (int i = 0; i < 20; i++)
			{
				if (!Main.itemText[i].active || i == whoAmI)
				{
					continue;
				}
				string text2 = Main.itemText[i].name;
				if (Main.itemText[i].stack > 1)
				{
					object obj2 = text2;
					text2 = string.Concat(obj2, " (", Main.itemText[i].stack, ")");
				}
				Vector2 vector2 = Main.fontMouseText.MeasureString(text2);
				vector2 *= Main.itemText[i].scale;
				vector2.Y *= 0.8f;
				Rectangle value = new Rectangle((int)(Main.itemText[i].position.X - vector2.X / 2f), (int)(Main.itemText[i].position.Y - vector2.Y / 2f), (int)vector2.X, (int)vector2.Y);
				if (rectangle.Intersects(value) && (position.Y < Main.itemText[i].position.Y || (position.Y == Main.itemText[i].position.Y && whoAmI < i)))
				{
					flag = true;
					int num = numActive;
					if (num > 3)
					{
						num = 3;
					}
					Main.itemText[i].lifeTime = activeTime + 15 * num;
					lifeTime = activeTime + 15 * num;
				}
			}
			if (!flag)
			{
				velocity.Y *= 0.86f;
				if (scale == 1f)
				{
					velocity.Y *= 0.4f;
				}
			}
			else if (velocity.Y > -6f)
			{
				velocity.Y -= 0.2f;
			}
			else
			{
				velocity.Y *= 0.86f;
			}
			velocity.X *= 0.93f;
			position += velocity;
			lifeTime--;
			if (lifeTime <= 0)
			{
				scale -= 0.03f;
				if ((double)scale < 0.1)
				{
					active = false;
				}
				lifeTime = 0;
				return;
			}
			if (scale < 1f)
			{
				scale += 0.1f;
			}
			if (scale > 1f)
			{
				scale = 1f;
			}
		}

		public static void UpdateItemText()
		{
			int num = 0;
			for (int i = 0; i < 20; i++)
			{
				if (Main.itemText[i].active)
				{
					num++;
					Main.itemText[i].Update(i);
				}
			}
			numActive = num;
		}
	}
}
