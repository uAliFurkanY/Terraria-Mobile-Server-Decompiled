using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Terraria
{
	public class TargetDummy
	{
		public const int MaxDummies = 1000;

		public static TargetDummy[] dummies = new TargetDummy[1000];

		public short x;

		public short y;

		public int npc;

		public int whoAmI;

		public static void UpdateDummies()
		{
			Dictionary<int, Rectangle> dictionary = new Dictionary<int, Rectangle>();
			bool flag = false;
			Rectangle value = new Rectangle(0, 0, 32, 48);
			value.Inflate(1600, 1600);
			int num = value.X;
			int num2 = value.Y;
			for (int i = 0; i < 1000; i++)
			{
				if (dummies[i] == null)
				{
					continue;
				}
				dummies[i].whoAmI = i;
				if (dummies[i].npc != -1)
				{
					if (!Main.npc[dummies[i].npc].active || Main.npc[dummies[i].npc].type != 488 || Main.npc[dummies[i].npc].ai[0] != (float)dummies[i].x || Main.npc[dummies[i].npc].ai[1] != (float)dummies[i].y)
					{
						dummies[i].Deactivate();
					}
					continue;
				}
				if (!flag)
				{
					for (int j = 0; j < 16; j++)
					{
						if (Main.player[j].active)
						{
							dictionary[j] = Main.player[j].getRect();
						}
					}
					flag = true;
				}
				value.X = dummies[i].x * 16 + num;
				value.Y = dummies[i].y * 16 + num2;
				bool flag2 = false;
				foreach (KeyValuePair<int, Rectangle> item in dictionary)
				{
					if (item.Value.Intersects(value))
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					dummies[i].Activate();
				}
			}
		}

		public TargetDummy(int x, int y)
		{
			this.x = (short)x;
			this.y = (short)y;
			npc = -1;
		}

		public static int Find(int x, int y)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (dummies[i] != null && dummies[i].x == x && dummies[i].y == y)
				{
					return i;
				}
			}
			return -1;
		}

		public static int Place(int x, int y)
		{
			int num = -1;
			for (int i = 0; i < 1000; i++)
			{
				if (dummies[i] == null)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return num;
			}
			dummies[num] = new TargetDummy(x, y);
			return num;
		}

		public static void Kill(int x, int y)
		{
			for (int i = 0; i < 1000; i++)
			{
				TargetDummy targetDummy = dummies[i];
				if (targetDummy != null && targetDummy.x == x && targetDummy.y == y)
				{
					dummies[i] = null;
				}
			}
		}

		public static int Hook_AfterPlacement(int x, int y, int type = 21, int style = 0, int direction = 1)
		{
			if (Main.netMode == 1)
			{
				NetMessage.SendTileSquare(Main.myPlayer, x - 1, y - 1, 3);
				NetMessage.SendData(87, -1, -1, "", x - 1, y - 2);
				return -1;
			}
			return Place(x - 1, y - 2);
		}

		public void Activate()
		{
			int num = NPC.NewNPC(x * 16 + 16, y * 16 + 48, 488, 100);
			Main.npc[num].ai[0] = x;
			Main.npc[num].ai[1] = y;
			Main.npc[num].netUpdate = true;
			npc = num;
			if (Main.netMode != 1)
			{
				NetMessage.SendData(86, -1, -1, "", whoAmI, x, y);
			}
		}

		public void Deactivate()
		{
			if (npc != -1)
			{
				Main.npc[npc].active = false;
			}
			npc = -1;
			if (Main.netMode != 1)
			{
				NetMessage.SendData(86, -1, -1, "", whoAmI, x, y);
			}
		}

		public override string ToString()
		{
			return x + "x  " + y + "y npc: " + npc;
		}
	}
}
