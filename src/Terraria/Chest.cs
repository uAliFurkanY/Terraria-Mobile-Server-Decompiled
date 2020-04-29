using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.ObjectData;

namespace Terraria
{
	public class Chest
	{
		public const int maxChestTypes = 52;

		public const int maxDresserTypes = 28;

		public const int maxItems = 40;

		public const int MaxNameLength = 20;

		public static int[] chestTypeToIcon = new int[52];

		public static int[] chestItemSpawn = new int[52];

		public static int[] dresserTypeToIcon = new int[28];

		public static int[] dresserItemSpawn = new int[28];

		public Item[] item;

		public int x;

		public int y;

		public bool bankChest;

		public string name;

		public int frameCounter;

		public int frame;

		public Chest(bool bank = false)
		{
			item = new Item[40];
			bankChest = bank;
			name = string.Empty;
		}

		public override string ToString()
		{
			int num = 0;
			for (int i = 0; i < item.Length; i++)
			{
				if (item[i].stack > 0)
				{
					num++;
				}
			}
			return $"{{X: {x}, Y: {y}, Count: {num}}}";
		}

		public static void Initialize()
		{
			chestTypeToIcon[0] = (chestItemSpawn[0] = 48);
			chestTypeToIcon[1] = (chestItemSpawn[1] = 306);
			chestTypeToIcon[2] = 327;
			chestItemSpawn[2] = 306;
			chestTypeToIcon[3] = (chestItemSpawn[3] = 328);
			chestTypeToIcon[4] = 329;
			chestItemSpawn[4] = 328;
			chestTypeToIcon[5] = (chestItemSpawn[5] = 343);
			chestTypeToIcon[6] = (chestItemSpawn[6] = 348);
			chestTypeToIcon[7] = (chestItemSpawn[7] = 625);
			chestTypeToIcon[8] = (chestItemSpawn[8] = 626);
			chestTypeToIcon[9] = (chestItemSpawn[9] = 627);
			chestTypeToIcon[10] = (chestItemSpawn[10] = 680);
			chestTypeToIcon[11] = (chestItemSpawn[11] = 681);
			chestTypeToIcon[12] = (chestItemSpawn[12] = 831);
			chestTypeToIcon[13] = (chestItemSpawn[13] = 838);
			chestTypeToIcon[14] = (chestItemSpawn[14] = 914);
			chestTypeToIcon[15] = (chestItemSpawn[15] = 952);
			chestTypeToIcon[16] = (chestItemSpawn[16] = 1142);
			chestTypeToIcon[17] = (chestItemSpawn[17] = 1298);
			chestTypeToIcon[18] = (chestItemSpawn[18] = 1528);
			chestTypeToIcon[19] = (chestItemSpawn[19] = 1529);
			chestTypeToIcon[20] = (chestItemSpawn[20] = 1530);
			chestTypeToIcon[21] = (chestItemSpawn[21] = 1531);
			chestTypeToIcon[22] = (chestItemSpawn[22] = 1532);
			chestTypeToIcon[23] = 1533;
			chestItemSpawn[23] = 1528;
			chestTypeToIcon[24] = 1534;
			chestItemSpawn[24] = 1529;
			chestTypeToIcon[25] = 1535;
			chestItemSpawn[25] = 1530;
			chestTypeToIcon[26] = 1536;
			chestItemSpawn[26] = 1531;
			chestTypeToIcon[27] = 1537;
			chestItemSpawn[27] = 1532;
			chestTypeToIcon[28] = (chestItemSpawn[28] = 2230);
			chestTypeToIcon[29] = (chestItemSpawn[29] = 2249);
			chestTypeToIcon[30] = (chestItemSpawn[30] = 2250);
			chestTypeToIcon[31] = (chestItemSpawn[31] = 2526);
			chestTypeToIcon[32] = (chestItemSpawn[32] = 2544);
			chestTypeToIcon[33] = (chestItemSpawn[33] = 2559);
			chestTypeToIcon[34] = (chestItemSpawn[34] = 2574);
			chestTypeToIcon[35] = (chestItemSpawn[35] = 2612);
			chestTypeToIcon[36] = 327;
			chestItemSpawn[36] = 2612;
			chestTypeToIcon[37] = (chestItemSpawn[37] = 2613);
			chestTypeToIcon[38] = 327;
			chestItemSpawn[38] = 2613;
			chestTypeToIcon[39] = (chestItemSpawn[39] = 2614);
			chestTypeToIcon[40] = 327;
			chestItemSpawn[40] = 2614;
			chestTypeToIcon[41] = (chestItemSpawn[41] = 2615);
			chestTypeToIcon[42] = (chestItemSpawn[42] = 2616);
			chestTypeToIcon[43] = (chestItemSpawn[43] = 2617);
			chestTypeToIcon[44] = (chestItemSpawn[44] = 2618);
			chestTypeToIcon[45] = (chestItemSpawn[45] = 2619);
			chestTypeToIcon[46] = (chestItemSpawn[46] = 2620);
			chestTypeToIcon[47] = (chestItemSpawn[47] = 2748);
			chestTypeToIcon[48] = (chestItemSpawn[48] = 2814);
			chestTypeToIcon[49] = (chestItemSpawn[49] = 3180);
			chestTypeToIcon[50] = (chestItemSpawn[50] = 3125);
			chestTypeToIcon[51] = (chestItemSpawn[51] = 3181);
			dresserTypeToIcon[0] = (dresserItemSpawn[0] = 334);
			dresserTypeToIcon[1] = (dresserItemSpawn[1] = 647);
			dresserTypeToIcon[2] = (dresserItemSpawn[2] = 648);
			dresserTypeToIcon[3] = (dresserItemSpawn[3] = 649);
			dresserTypeToIcon[4] = (dresserItemSpawn[4] = 918);
			dresserTypeToIcon[5] = (dresserItemSpawn[5] = 2386);
			dresserTypeToIcon[6] = (dresserItemSpawn[6] = 2387);
			dresserTypeToIcon[7] = (dresserItemSpawn[7] = 2388);
			dresserTypeToIcon[8] = (dresserItemSpawn[8] = 2389);
			dresserTypeToIcon[9] = (dresserItemSpawn[9] = 2390);
			dresserTypeToIcon[10] = (dresserItemSpawn[10] = 2391);
			dresserTypeToIcon[11] = (dresserItemSpawn[11] = 2392);
			dresserTypeToIcon[12] = (dresserItemSpawn[12] = 2393);
			dresserTypeToIcon[13] = (dresserItemSpawn[13] = 2394);
			dresserTypeToIcon[14] = (dresserItemSpawn[14] = 2395);
			dresserTypeToIcon[15] = (dresserItemSpawn[15] = 2396);
			dresserTypeToIcon[16] = (dresserItemSpawn[16] = 2529);
			dresserTypeToIcon[17] = (dresserItemSpawn[17] = 2545);
			dresserTypeToIcon[18] = (dresserItemSpawn[18] = 2562);
			dresserTypeToIcon[19] = (dresserItemSpawn[19] = 2577);
			dresserTypeToIcon[20] = (dresserItemSpawn[20] = 2637);
			dresserTypeToIcon[21] = (dresserItemSpawn[21] = 2638);
			dresserTypeToIcon[22] = (dresserItemSpawn[22] = 2639);
			dresserTypeToIcon[23] = (dresserItemSpawn[23] = 2640);
			dresserTypeToIcon[24] = (dresserItemSpawn[24] = 2816);
			dresserTypeToIcon[25] = (dresserItemSpawn[25] = 3132);
			dresserTypeToIcon[26] = (dresserItemSpawn[26] = 3134);
			dresserTypeToIcon[27] = (dresserItemSpawn[27] = 3133);
		}

		private static bool IsPlayerInChest(int i)
		{
			for (int j = 0; j < 16; j++)
			{
				if (Main.player[j].chest == i)
				{
					return true;
				}
			}
			return false;
		}

		public static bool isLocked(int x, int y)
		{
			if (Main.tile[x, y] == null)
			{
				return true;
			}
			if ((Main.tile[x, y].frameX >= 72 && Main.tile[x, y].frameX <= 106) || (Main.tile[x, y].frameX >= 144 && Main.tile[x, y].frameX <= 178) || (Main.tile[x, y].frameX >= 828 && Main.tile[x, y].frameX <= 1006) || (Main.tile[x, y].frameX >= 1296 && Main.tile[x, y].frameX <= 1330) || (Main.tile[x, y].frameX >= 1368 && Main.tile[x, y].frameX <= 1402) || (Main.tile[x, y].frameX >= 1440 && Main.tile[x, y].frameX <= 1474))
			{
				return true;
			}
			return false;
		}

		public static void ServerPlaceItem(int plr, int slot)
		{
			Main.player[plr].inventory[slot] = PutItemInNearbyChest(Main.player[plr].inventory[slot], Main.player[plr].Center);
			NetMessage.SendData(5, -1, -1, "", plr, slot, (int)Main.player[plr].inventory[slot].prefix);
		}

		public static Item PutItemInNearbyChest(Item item, Vector2 position)
		{
			if (Main.netMode == 1)
			{
				return item;
			}
			for (int i = 0; i < 1000; i++)
			{
				bool flag = false;
				bool flag2 = false;
				if (Main.chest[i] == null || IsPlayerInChest(i) || isLocked(Main.chest[i].x, Main.chest[i].y))
				{
					continue;
				}
				Vector2 value = new Vector2(Main.chest[i].x * 16 + 16, Main.chest[i].y * 16 + 16);
				if (!((value - position).Length() < 200f))
				{
					continue;
				}
				for (int j = 0; j < Main.chest[i].item.Length; j++)
				{
					if (Main.chest[i].item[j].type > 0 && Main.chest[i].item[j].stack > 0)
					{
						if (!item.IsTheSameAs(Main.chest[i].item[j]))
						{
							continue;
						}
						flag = true;
						int num = Main.chest[i].item[j].maxStack - Main.chest[i].item[j].stack;
						if (num > 0)
						{
							if (num > item.stack)
							{
								num = item.stack;
							}
							item.stack -= num;
							Main.chest[i].item[j].stack += num;
							if (item.stack <= 0)
							{
								item.SetDefaults();
								return item;
							}
						}
					}
					else
					{
						flag2 = true;
					}
				}
				if (!flag || !flag2 || item.stack <= 0)
				{
					continue;
				}
				for (int k = 0; k < Main.chest[i].item.Length; k++)
				{
					if (Main.chest[i].item[k].type == 0 || Main.chest[i].item[k].stack == 0)
					{
						Main.chest[i].item[k] = item.Clone();
						item.SetDefaults();
						return item;
					}
				}
			}
			return item;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public static bool Unlock(int X, int Y)
		{
			if (Main.tile[X, Y] == null)
			{
				return false;
			}
			short num;
			int type;
			switch (Main.tile[X, Y].frameX / 36)
			{
			case 2:
				num = 36;
				type = 11;
				AchievementsHelper.NotifyProgressionEvent(19);
				break;
			case 4:
				num = 36;
				type = 11;
				break;
			case 23:
			case 24:
			case 25:
			case 26:
			case 27:
				if (!NPC.downedPlantBoss)
				{
					return false;
				}
				num = 180;
				type = 11;
				AchievementsHelper.NotifyProgressionEvent(20);
				break;
			case 36:
			case 38:
			case 40:
				num = 36;
				type = 11;
				break;
			default:
				return false;
			}
			Main.PlaySound(22, X * 16, Y * 16);
			for (int i = X; i <= X + 1; i++)
			{
				for (int j = Y; j <= Y + 1; j++)
				{
					Main.tile[i, j].frameX -= num;
					for (int k = 0; k < 4; k++)
					{
						Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, type);
					}
				}
			}
			return true;
		}

		public static int UsingChest(int i)
		{
			if (Main.chest[i] != null)
			{
				for (int j = 0; j < 16; j++)
				{
					if (Main.player[j].active && Main.player[j].chest == i)
					{
						return j;
					}
				}
			}
			return -1;
		}

		public static int FindChest(int X, int Y)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (Main.chest[i] != null && Main.chest[i].x == X && Main.chest[i].y == Y)
				{
					return i;
				}
			}
			return -1;
		}

		public static int FindEmptyChest(int x, int y, int type = 21, int style = 0, int direction = 1)
		{
			int num = -1;
			for (int i = 0; i < 1000; i++)
			{
				Chest chest = Main.chest[i];
				if (chest != null)
				{
					if (chest.x == x && chest.y == y)
					{
						return -1;
					}
				}
				else if (num == -1)
				{
					num = i;
				}
			}
			return num;
		}

		public static bool NearOtherChests(int x, int y)
		{
			for (int i = x - 25; i < x + 25; i++)
			{
				for (int j = y - 8; j < y + 8; j++)
				{
					Tile tileSafely = Framing.GetTileSafely(i, j);
					if (tileSafely.active() && tileSafely.type == 21)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static int AfterPlacement_Hook(int x, int y, int type = 21, int style = 0, int direction = 1)
		{
			Point16 baseCoords = new Point16(x, y);
			TileObjectData.OriginToTopLeft(type, style, ref baseCoords);
			int num = FindEmptyChest(baseCoords.X, baseCoords.Y);
			if (num == -1)
			{
				return -1;
			}
			if (Main.netMode != 1)
			{
				Chest chest = new Chest();
				chest.x = baseCoords.X;
				chest.y = baseCoords.Y;
				for (int i = 0; i < 40; i++)
				{
					chest.item[i] = new Item();
				}
				Main.chest[num] = chest;
			}
			else if (type == 21)
			{
				NetMessage.SendData(34, -1, -1, "", 0, x, y, style);
			}
			else
			{
				NetMessage.SendData(34, -1, -1, "", 2, x, y, style);
			}
			return num;
		}

		public static int CreateChest(int X, int Y, int id = -1)
		{
			int num = id;
			if (num == -1)
			{
				num = FindEmptyChest(X, Y);
				if (num == -1)
				{
					return -1;
				}
				if (Main.netMode == 1)
				{
					return num;
				}
			}
			Main.chest[num] = new Chest();
			Main.chest[num].x = X;
			Main.chest[num].y = Y;
			for (int i = 0; i < 40; i++)
			{
				Main.chest[num].item[i] = new Item();
			}
			return num;
		}

		public static bool CanDestroyChest(int X, int Y)
		{
			for (int i = 0; i < 1000; i++)
			{
				Chest chest = Main.chest[i];
				if (chest == null || chest.x != X || chest.y != Y)
				{
					continue;
				}
				for (int j = 0; j < 40; j++)
				{
					if (chest.item[j] != null && chest.item[j].type > 0 && chest.item[j].stack > 0)
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		public static bool DestroyChest(int X, int Y)
		{
			for (int i = 0; i < 1000; i++)
			{
				Chest chest = Main.chest[i];
				if (chest == null || chest.x != X || chest.y != Y)
				{
					continue;
				}
				for (int j = 0; j < 40; j++)
				{
					if (chest.item[j] != null && chest.item[j].type > 0 && chest.item[j].stack > 0)
					{
						return false;
					}
				}
				Main.chest[i] = null;
				if (Main.player[Main.myPlayer].chest == i)
				{
					Main.player[Main.myPlayer].chest = -1;
				}
				Recipe.FindRecipes();
				return true;
			}
			return true;
		}

		public static void DestroyChestDirect(int X, int Y, int id)
		{
			if (id >= 0 && id < Main.chest.Length)
			{
				try
				{
					Chest chest = Main.chest[id];
					if (chest != null && chest.x == X && chest.y == Y)
					{
						Main.chest[id] = null;
						if (Main.player[Main.myPlayer].chest == id)
						{
							Main.player[Main.myPlayer].chest = -1;
						}
						Recipe.FindRecipes();
					}
				}
				catch
				{
				}
			}
		}

		public void AddShop(Item newItem)
		{
			int num = 0;
			while (true)
			{
				if (num < 39)
				{
					if (item[num] == null || item[num].type == 0)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			item[num] = newItem.Clone();
			item[num].favorited = false;
			item[num].buyOnce = true;
			if (item[num].value > 0)
			{
				item[num].value = item[num].value / 5;
				if (item[num].value < 1)
				{
					item[num].value = 1;
				}
			}
		}

		public static void SetupTravelShop()
		{
			for (int i = 0; i < 40; i++)
			{
				Main.travelShop[i] = 0;
			}
			int num = Main.rand.Next(4, 7);
			if (Main.rand.Next(4) == 0)
			{
				num++;
			}
			if (Main.rand.Next(8) == 0)
			{
				num++;
			}
			if (Main.rand.Next(16) == 0)
			{
				num++;
			}
			if (Main.rand.Next(32) == 0)
			{
				num++;
			}
			if (Main.expertMode && Main.rand.Next(2) == 0)
			{
				num++;
			}
			int num2 = 0;
			int num3 = 0;
			int[] array = new int[6]
			{
				100,
				200,
				300,
				400,
				500,
				600
			};
			while (num3 < num)
			{
				int num4 = 0;
				if (Main.rand.Next(array[4]) == 0)
				{
					num4 = 3309;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num4 = 3314;
				}
				if (Main.rand.Next(array[5]) == 0)
				{
					num4 = 1987;
				}
				if (Main.rand.Next(array[4]) == 0 && Main.hardMode)
				{
					num4 = 2270;
				}
				if (Main.rand.Next(array[4]) == 0)
				{
					num4 = 2278;
				}
				if (Main.rand.Next(array[4]) == 0)
				{
					num4 = 2271;
				}
				if (Main.rand.Next(array[3]) == 0 && Main.hardMode && NPC.downedPlantBoss)
				{
					num4 = 2223;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num4 = 2272;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num4 = 2219;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num4 = 2276;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num4 = 2284;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num4 = 2285;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num4 = 2286;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num4 = 2287;
				}
				if (Main.rand.Next(array[3]) == 0)
				{
					num4 = 2296;
				}
				if (Main.rand.Next(array[2]) == 0 && WorldGen.shadowOrbSmashed)
				{
					num4 = 2269;
				}
				if (Main.rand.Next(array[2]) == 0)
				{
					num4 = 2177;
				}
				if (Main.rand.Next(array[2]) == 0)
				{
					num4 = 1988;
				}
				if (Main.rand.Next(array[2]) == 0)
				{
					num4 = 2275;
				}
				if (Main.rand.Next(array[2]) == 0)
				{
					num4 = 2279;
				}
				if (Main.rand.Next(array[2]) == 0)
				{
					num4 = 2277;
				}
				if (Main.rand.Next(array[2]) == 0 && NPC.downedBoss1)
				{
					num4 = 3262;
				}
				if (Main.rand.Next(array[2]) == 0 && NPC.downedMechBossAny)
				{
					num4 = 3284;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.hardMode && NPC.downedMoonlord)
				{
					num4 = 3596;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.hardMode && NPC.downedMartians)
				{
					num4 = 2865;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.hardMode && NPC.downedMartians)
				{
					num4 = 2866;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.hardMode && NPC.downedMartians)
				{
					num4 = 2867;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.xMas)
				{
					num4 = 3055;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.xMas)
				{
					num4 = 3056;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.xMas)
				{
					num4 = 3057;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.xMas)
				{
					num4 = 3058;
				}
				if (Main.rand.Next(array[2]) == 0 && Main.xMas)
				{
					num4 = 3059;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num4 = 2214;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num4 = 2215;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num4 = 2216;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num4 = 2217;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num4 = 2273;
				}
				if (Main.rand.Next(array[1]) == 0)
				{
					num4 = 2274;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num4 = 2266;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num4 = 2267;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num4 = 2268;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num4 = 2281 + Main.rand.Next(3);
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num4 = 2258;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num4 = 2242;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num4 = 2260;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num4 = 3119;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num4 = 3118;
				}
				if (Main.rand.Next(array[0]) == 0)
				{
					num4 = 3099;
				}
				if (num4 != 0)
				{
					for (int j = 0; j < 40; j++)
					{
						if (Main.travelShop[j] == num4)
						{
							num4 = 0;
							break;
						}
					}
				}
				if (num4 != 0)
				{
					num3++;
					Main.travelShop[num2] = num4;
					num2++;
					if (num4 == 2260)
					{
						Main.travelShop[num2] = 2261;
						num2++;
						Main.travelShop[num2] = 2262;
						num2++;
					}
				}
			}
		}

		public void SetupShop(int type)
		{
			for (int i = 0; i < 40; i++)
			{
				item[i] = new Item();
			}
			int num = 0;
			switch (type)
			{
			case 1:
			{
				item[num].SetDefaults(88);
				num++;
				item[num].SetDefaults(87);
				num++;
				item[num].SetDefaults(35);
				num++;
				item[num].SetDefaults(1991);
				num++;
				item[num].SetDefaults(3509);
				num++;
				item[num].SetDefaults(3506);
				num++;
				item[num].SetDefaults(8);
				num++;
				item[num].SetDefaults(28);
				num++;
				item[num].SetDefaults(110);
				num++;
				item[num].SetDefaults(40);
				num++;
				item[num].SetDefaults(42);
				num++;
				item[num].SetDefaults(965);
				num++;
				if (Main.player[Main.myPlayer].ZoneSnow)
				{
					item[num].SetDefaults(967);
					num++;
				}
				if (Main.bloodMoon)
				{
					item[num].SetDefaults(279);
					num++;
				}
				if (!Main.dayTime)
				{
					item[num].SetDefaults(282);
					num++;
				}
				if (NPC.downedBoss3)
				{
					item[num].SetDefaults(346);
					num++;
				}
				if (Main.hardMode)
				{
					item[num].SetDefaults(488);
					num++;
				}
				for (int j = 0; j < 58; j++)
				{
					if (Main.player[Main.myPlayer].inventory[j].type == 930)
					{
						item[num].SetDefaults(931);
						num++;
						item[num].SetDefaults(1614);
						num++;
						break;
					}
				}
				item[num].SetDefaults(1786);
				num++;
				if (Main.hardMode)
				{
					item[num].SetDefaults(1348);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(3107))
				{
					item[num].SetDefaults(3108);
					num++;
				}
				if (Main.halloween)
				{
					item[num++].SetDefaults(3242);
					item[num++].SetDefaults(3243);
					item[num++].SetDefaults(3244);
				}
				break;
			}
			case 2:
				item[num].SetDefaults(97);
				num++;
				if (Main.bloodMoon || Main.hardMode)
				{
					item[num].SetDefaults(278);
					num++;
				}
				if ((NPC.downedBoss2 && !Main.dayTime) || Main.hardMode)
				{
					item[num].SetDefaults(47);
					num++;
				}
				item[num].SetDefaults(95);
				num++;
				item[num].SetDefaults(98);
				num++;
				if (!Main.dayTime)
				{
					item[num].SetDefaults(324);
					num++;
				}
				if (Main.hardMode)
				{
					item[num].SetDefaults(534);
					num++;
				}
				if (Main.hardMode)
				{
					item[num].SetDefaults(1432);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(1258))
				{
					item[num].SetDefaults(1261);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(1835))
				{
					item[num].SetDefaults(1836);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(3107))
				{
					item[num].SetDefaults(3108);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(1782))
				{
					item[num].SetDefaults(1783);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(1784))
				{
					item[num].SetDefaults(1785);
					num++;
				}
				if (Main.halloween)
				{
					item[num].SetDefaults(1736);
					num++;
					item[num].SetDefaults(1737);
					num++;
					item[num].SetDefaults(1738);
					num++;
				}
				break;
			case 3:
				if (Main.bloodMoon)
				{
					if (WorldGen.crimson)
					{
						item[num].SetDefaults(2886);
						num++;
						item[num].SetDefaults(2171);
						num++;
					}
					else
					{
						item[num].SetDefaults(67);
						num++;
						item[num].SetDefaults(59);
						num++;
					}
				}
				else
				{
					item[num].SetDefaults(66);
					num++;
					item[num].SetDefaults(62);
					num++;
					item[num].SetDefaults(63);
					num++;
				}
				item[num].SetDefaults(27);
				num++;
				item[num].SetDefaults(114);
				num++;
				item[num].SetDefaults(1828);
				num++;
				item[num].SetDefaults(745);
				num++;
				item[num].SetDefaults(747);
				num++;
				if (Main.hardMode)
				{
					item[num].SetDefaults(746);
					num++;
				}
				if (Main.hardMode)
				{
					item[num].SetDefaults(369);
					num++;
				}
				if (Main.shroomTiles > 50)
				{
					item[num].SetDefaults(194);
					num++;
				}
				if (Main.halloween)
				{
					item[num].SetDefaults(1853);
					num++;
					item[num].SetDefaults(1854);
					num++;
				}
				if (NPC.downedSlimeKing)
				{
					item[num].SetDefaults(3215);
					num++;
				}
				if (NPC.downedQueenBee)
				{
					item[num].SetDefaults(3216);
					num++;
				}
				if (NPC.downedBoss1)
				{
					item[num].SetDefaults(3219);
					num++;
				}
				if (NPC.downedBoss2)
				{
					if (WorldGen.crimson)
					{
						item[num].SetDefaults(3218);
						num++;
					}
					else
					{
						item[num].SetDefaults(3217);
						num++;
					}
				}
				if (NPC.downedBoss3)
				{
					item[num].SetDefaults(3220);
					num++;
					item[num].SetDefaults(3221);
					num++;
				}
				if (Main.hardMode)
				{
					item[num].SetDefaults(3222);
					num++;
				}
				break;
			case 4:
				item[num].SetDefaults(168);
				num++;
				item[num].SetDefaults(166);
				num++;
				item[num].SetDefaults(167);
				num++;
				if (Main.hardMode)
				{
					item[num].SetDefaults(265);
					num++;
				}
				if (Main.hardMode && NPC.downedPlantBoss && NPC.downedPirates)
				{
					item[num].SetDefaults(937);
					num++;
				}
				if (Main.hardMode)
				{
					item[num].SetDefaults(1347);
					num++;
				}
				break;
			case 5:
				item[num].SetDefaults(254);
				num++;
				item[num].SetDefaults(981);
				num++;
				if (Main.dayTime)
				{
					item[num].SetDefaults(242);
					num++;
				}
				if (Main.moonPhase == 0)
				{
					item[num].SetDefaults(245);
					num++;
					item[num].SetDefaults(246);
					num++;
					if (!Main.dayTime)
					{
						item[num++].SetDefaults(1288);
						item[num++].SetDefaults(1289);
					}
				}
				else if (Main.moonPhase == 1)
				{
					item[num].SetDefaults(325);
					num++;
					item[num].SetDefaults(326);
					num++;
				}
				item[num].SetDefaults(269);
				num++;
				item[num].SetDefaults(270);
				num++;
				item[num].SetDefaults(271);
				num++;
				if (NPC.downedClown)
				{
					item[num].SetDefaults(503);
					num++;
					item[num].SetDefaults(504);
					num++;
					item[num].SetDefaults(505);
					num++;
				}
				if (Main.bloodMoon)
				{
					item[num].SetDefaults(322);
					num++;
					if (!Main.dayTime)
					{
						item[num++].SetDefaults(3362);
						item[num++].SetDefaults(3363);
					}
				}
				if (NPC.downedAncientCultist)
				{
					if (Main.dayTime)
					{
						item[num++].SetDefaults(2856);
						item[num++].SetDefaults(2858);
					}
					else
					{
						item[num++].SetDefaults(2857);
						item[num++].SetDefaults(2859);
					}
				}
				if (NPC.AnyNPCs(441))
				{
					item[num++].SetDefaults(3242);
					item[num++].SetDefaults(3243);
					item[num++].SetDefaults(3244);
				}
				if (Main.player[Main.myPlayer].ZoneSnow)
				{
					item[num].SetDefaults(1429);
					num++;
				}
				if (Main.halloween)
				{
					item[num].SetDefaults(1740);
					num++;
				}
				if (Main.hardMode)
				{
					if (Main.moonPhase == 2)
					{
						item[num].SetDefaults(869);
						num++;
					}
					if (Main.moonPhase == 4)
					{
						item[num].SetDefaults(864);
						num++;
						item[num].SetDefaults(865);
						num++;
					}
					if (Main.moonPhase == 6)
					{
						item[num].SetDefaults(873);
						num++;
						item[num].SetDefaults(874);
						num++;
						item[num].SetDefaults(875);
						num++;
					}
				}
				if (NPC.downedFrost)
				{
					item[num].SetDefaults(1275);
					num++;
					item[num].SetDefaults(1276);
					num++;
				}
				if (Main.halloween)
				{
					item[num++].SetDefaults(3246);
					item[num++].SetDefaults(3247);
				}
				break;
			case 6:
				item[num].SetDefaults(128);
				num++;
				item[num].SetDefaults(486);
				num++;
				item[num].SetDefaults(398);
				num++;
				item[num].SetDefaults(84);
				num++;
				item[num].SetDefaults(407);
				num++;
				item[num].SetDefaults(161);
				num++;
				break;
			case 7:
				item[num].SetDefaults(487);
				num++;
				item[num].SetDefaults(496);
				num++;
				item[num].SetDefaults(500);
				num++;
				item[num].SetDefaults(507);
				num++;
				item[num].SetDefaults(508);
				num++;
				item[num].SetDefaults(531);
				num++;
				item[num].SetDefaults(576);
				num++;
				item[num].SetDefaults(3186);
				num++;
				if (Main.halloween)
				{
					item[num].SetDefaults(1739);
					num++;
				}
				break;
			case 8:
				item[num].SetDefaults(509);
				num++;
				item[num].SetDefaults(850);
				num++;
				item[num].SetDefaults(851);
				num++;
				item[num].SetDefaults(510);
				num++;
				item[num].SetDefaults(530);
				num++;
				item[num].SetDefaults(513);
				num++;
				item[num].SetDefaults(538);
				num++;
				item[num].SetDefaults(529);
				num++;
				item[num].SetDefaults(541);
				num++;
				item[num].SetDefaults(542);
				num++;
				item[num].SetDefaults(543);
				num++;
				item[num].SetDefaults(852);
				num++;
				item[num].SetDefaults(853);
				num++;
				item[num].SetDefaults(2739);
				num++;
				item[num].SetDefaults(849);
				num++;
				item[num++].SetDefaults(2799);
				if (NPC.AnyNPCs(369) && Main.hardMode && Main.moonPhase == 3)
				{
					item[num].SetDefaults(2295);
					num++;
				}
				break;
			case 9:
			{
				item[num].SetDefaults(588);
				num++;
				item[num].SetDefaults(589);
				num++;
				item[num].SetDefaults(590);
				num++;
				item[num].SetDefaults(597);
				num++;
				item[num].SetDefaults(598);
				num++;
				item[num].SetDefaults(596);
				num++;
				for (int num2 = 1873; num2 < 1906; num2++)
				{
					item[num].SetDefaults(num2);
					num++;
				}
				break;
			}
			case 10:
				if (NPC.downedMechBossAny)
				{
					item[num].SetDefaults(756);
					num++;
					item[num].SetDefaults(787);
					num++;
				}
				item[num].SetDefaults(868);
				num++;
				if (NPC.downedPlantBoss)
				{
					item[num].SetDefaults(1551);
					num++;
				}
				item[num].SetDefaults(1181);
				num++;
				item[num].SetDefaults(783);
				num++;
				break;
			case 11:
				item[num].SetDefaults(779);
				num++;
				if (Main.moonPhase >= 4)
				{
					item[num].SetDefaults(748);
					num++;
				}
				else
				{
					item[num].SetDefaults(839);
					num++;
					item[num].SetDefaults(840);
					num++;
					item[num].SetDefaults(841);
					num++;
				}
				if (NPC.downedGolemBoss)
				{
					item[num].SetDefaults(948);
					num++;
				}
				item[num].SetDefaults(995);
				num++;
				if (NPC.downedBoss1 && NPC.downedBoss2 && NPC.downedBoss3)
				{
					item[num].SetDefaults(2203);
					num++;
				}
				if (WorldGen.crimson)
				{
					item[num].SetDefaults(2193);
					num++;
				}
				item[num].SetDefaults(1263);
				num++;
				if (Main.eclipse || Main.bloodMoon)
				{
					if (WorldGen.crimson)
					{
						item[num].SetDefaults(784);
						num++;
					}
					else
					{
						item[num].SetDefaults(782);
						num++;
					}
				}
				else if (Main.player[Main.myPlayer].ZoneHoly)
				{
					item[num].SetDefaults(781);
					num++;
				}
				else
				{
					item[num].SetDefaults(780);
					num++;
				}
				if (Main.hardMode)
				{
					item[num].SetDefaults(1344);
					num++;
				}
				if (Main.halloween)
				{
					item[num].SetDefaults(1742);
					num++;
				}
				break;
			case 12:
				item[num].SetDefaults(1037);
				num++;
				item[num].SetDefaults(2874);
				num++;
				item[num].SetDefaults(1120);
				num++;
				if (Main.netMode == 1)
				{
					item[num].SetDefaults(1969);
					num++;
				}
				if (Main.halloween)
				{
					item[num].SetDefaults(3248);
					num++;
					item[num].SetDefaults(1741);
					num++;
				}
				if (Main.moonPhase == 0)
				{
					item[num].SetDefaults(2871);
					num++;
					item[num].SetDefaults(2872);
					num++;
				}
				break;
			case 13:
				item[num].SetDefaults(859);
				num++;
				item[num].SetDefaults(1000);
				num++;
				item[num].SetDefaults(1168);
				num++;
				item[num].SetDefaults(1449);
				num++;
				item[num].SetDefaults(1345);
				num++;
				item[num].SetDefaults(1450);
				num++;
				item[num++].SetDefaults(3253);
				item[num++].SetDefaults(2700);
				item[num++].SetDefaults(2738);
				if (Main.player[Main.myPlayer].HasItem(3548))
				{
					item[num].SetDefaults(3548);
					num++;
				}
				if (NPC.AnyNPCs(229))
				{
					item[num++].SetDefaults(3369);
				}
				if (Main.hardMode)
				{
					item[num].SetDefaults(3214);
					num++;
					item[num].SetDefaults(2868);
					num++;
					item[num].SetDefaults(970);
					num++;
					item[num].SetDefaults(971);
					num++;
					item[num].SetDefaults(972);
					num++;
					item[num].SetDefaults(973);
					num++;
				}
				break;
			case 14:
				item[num].SetDefaults(771);
				num++;
				if (Main.bloodMoon)
				{
					item[num].SetDefaults(772);
					num++;
				}
				if (!Main.dayTime || Main.eclipse)
				{
					item[num].SetDefaults(773);
					num++;
				}
				if (Main.eclipse)
				{
					item[num].SetDefaults(774);
					num++;
				}
				if (Main.hardMode)
				{
					item[num].SetDefaults(760);
					num++;
				}
				if (Main.hardMode)
				{
					item[num].SetDefaults(1346);
					num++;
				}
				if (Main.halloween)
				{
					item[num].SetDefaults(1743);
					num++;
					item[num].SetDefaults(1744);
					num++;
					item[num].SetDefaults(1745);
					num++;
				}
				if (NPC.downedMartians)
				{
					item[num++].SetDefaults(2862);
					item[num++].SetDefaults(3109);
				}
				break;
			case 15:
			{
				item[num].SetDefaults(1071);
				num++;
				item[num].SetDefaults(1072);
				num++;
				item[num].SetDefaults(1100);
				num++;
				for (int k = 1073; k <= 1084; k++)
				{
					item[num].SetDefaults(k);
					num++;
				}
				item[num].SetDefaults(1097);
				num++;
				item[num].SetDefaults(1099);
				num++;
				item[num].SetDefaults(1098);
				num++;
				item[num].SetDefaults(1966);
				num++;
				if (Main.hardMode)
				{
					item[num].SetDefaults(1967);
					num++;
					item[num].SetDefaults(1968);
					num++;
				}
				item[num].SetDefaults(1490);
				num++;
				if (Main.moonPhase <= 1)
				{
					item[num].SetDefaults(1481);
					num++;
				}
				else if (Main.moonPhase <= 3)
				{
					item[num].SetDefaults(1482);
					num++;
				}
				else if (Main.moonPhase <= 5)
				{
					item[num].SetDefaults(1483);
					num++;
				}
				else
				{
					item[num].SetDefaults(1484);
					num++;
				}
				if (Main.player[Main.myPlayer].ZoneCrimson)
				{
					item[num].SetDefaults(1492);
					num++;
				}
				if (Main.player[Main.myPlayer].ZoneCorrupt)
				{
					item[num].SetDefaults(1488);
					num++;
				}
				if (Main.player[Main.myPlayer].ZoneHoly)
				{
					item[num].SetDefaults(1489);
					num++;
				}
				if (Main.player[Main.myPlayer].ZoneJungle)
				{
					item[num].SetDefaults(1486);
					num++;
				}
				if (Main.player[Main.myPlayer].ZoneSnow)
				{
					item[num].SetDefaults(1487);
					num++;
				}
				if (Main.sandTiles > 1000)
				{
					item[num].SetDefaults(1491);
					num++;
				}
				if (Main.bloodMoon)
				{
					item[num].SetDefaults(1493);
					num++;
				}
				if ((double)(Main.player[Main.myPlayer].position.Y / 16f) < Main.worldSurface * 0.34999999403953552)
				{
					item[num].SetDefaults(1485);
					num++;
				}
				if ((double)(Main.player[Main.myPlayer].position.Y / 16f) < Main.worldSurface * 0.34999999403953552 && Main.hardMode)
				{
					item[num].SetDefaults(1494);
					num++;
				}
				if (Main.xMas)
				{
					for (int l = 1948; l <= 1957; l++)
					{
						item[num].SetDefaults(l);
						num++;
					}
				}
				for (int m = 2158; m <= 2160; m++)
				{
					if (num < 39)
					{
						item[num].SetDefaults(m);
					}
					num++;
				}
				for (int n = 2008; n <= 2014; n++)
				{
					if (num < 39)
					{
						item[num].SetDefaults(n);
					}
					num++;
				}
				break;
			}
			case 16:
				item[num].SetDefaults(1430);
				num++;
				item[num].SetDefaults(986);
				num++;
				if (NPC.AnyNPCs(108))
				{
					item[num++].SetDefaults(2999);
				}
				if (Main.hardMode && NPC.downedPlantBoss)
				{
					if (Main.player[Main.myPlayer].HasItem(1157))
					{
						item[num].SetDefaults(1159);
						num++;
						item[num].SetDefaults(1160);
						num++;
						item[num].SetDefaults(1161);
						num++;
						if (!Main.dayTime)
						{
							item[num].SetDefaults(1158);
							num++;
						}
						if (Main.player[Main.myPlayer].ZoneJungle)
						{
							item[num].SetDefaults(1167);
							num++;
						}
					}
					item[num].SetDefaults(1339);
					num++;
				}
				if (Main.hardMode && Main.player[Main.myPlayer].ZoneJungle)
				{
					item[num].SetDefaults(1171);
					num++;
					if (!Main.dayTime)
					{
						item[num].SetDefaults(1162);
						num++;
					}
				}
				item[num].SetDefaults(909);
				num++;
				item[num].SetDefaults(910);
				num++;
				item[num].SetDefaults(940);
				num++;
				item[num].SetDefaults(941);
				num++;
				item[num].SetDefaults(942);
				num++;
				item[num].SetDefaults(943);
				num++;
				item[num].SetDefaults(944);
				num++;
				item[num].SetDefaults(945);
				num++;
				if (Main.player[Main.myPlayer].HasItem(1835))
				{
					item[num].SetDefaults(1836);
					num++;
				}
				if (Main.player[Main.myPlayer].HasItem(1258))
				{
					item[num].SetDefaults(1261);
					num++;
				}
				if (Main.halloween)
				{
					item[num].SetDefaults(1791);
					num++;
				}
				break;
			case 17:
			{
				item[num].SetDefaults(928);
				num++;
				item[num].SetDefaults(929);
				num++;
				item[num].SetDefaults(876);
				num++;
				item[num].SetDefaults(877);
				num++;
				item[num].SetDefaults(878);
				num++;
				item[num].SetDefaults(2434);
				num++;
				int num3 = (int)((Main.screenPosition.X + (float)(Main.screenWidth / 2)) / 16f);
				if ((double)(Main.screenPosition.Y / 16f) < Main.worldSurface + 10.0 && (num3 < 380 || num3 > Main.maxTilesX - 380))
				{
					item[num].SetDefaults(1180);
					num++;
				}
				if (Main.hardMode && NPC.downedMechBossAny && NPC.AnyNPCs(208))
				{
					item[num].SetDefaults(1337);
					num++;
				}
				break;
			}
			case 18:
			{
				item[num].SetDefaults(1990);
				num++;
				item[num].SetDefaults(1979);
				num++;
				if (Main.player[Main.myPlayer].statLifeMax >= 400)
				{
					item[num].SetDefaults(1977);
					num++;
				}
				if (Main.player[Main.myPlayer].statManaMax >= 200)
				{
					item[num].SetDefaults(1978);
					num++;
				}
				long num5 = 0L;
				for (int num6 = 0; num6 < 54; num6++)
				{
					if (Main.player[Main.myPlayer].inventory[num6].type == 71)
					{
						num5 += Main.player[Main.myPlayer].inventory[num6].stack;
					}
					if (Main.player[Main.myPlayer].inventory[num6].type == 72)
					{
						num5 += Main.player[Main.myPlayer].inventory[num6].stack * 100;
					}
					if (Main.player[Main.myPlayer].inventory[num6].type == 73)
					{
						num5 += Main.player[Main.myPlayer].inventory[num6].stack * 10000;
					}
					if (Main.player[Main.myPlayer].inventory[num6].type == 74)
					{
						num5 += Main.player[Main.myPlayer].inventory[num6].stack * 1000000;
					}
				}
				if (num5 >= 1000000)
				{
					item[num].SetDefaults(1980);
					num++;
				}
				if ((Main.moonPhase % 2 == 0 && Main.dayTime) || (Main.moonPhase % 2 == 1 && !Main.dayTime))
				{
					item[num].SetDefaults(1981);
					num++;
				}
				if (Main.player[Main.myPlayer].team != 0)
				{
					item[num].SetDefaults(1982);
					num++;
				}
				if (Main.hardMode)
				{
					item[num].SetDefaults(1983);
					num++;
				}
				if (NPC.AnyNPCs(208))
				{
					item[num].SetDefaults(1984);
					num++;
				}
				if (Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
				{
					item[num].SetDefaults(1985);
					num++;
				}
				if (Main.hardMode && NPC.downedMechBossAny)
				{
					item[num].SetDefaults(1986);
					num++;
				}
				if (Main.hardMode && NPC.downedMartians)
				{
					item[num].SetDefaults(2863);
					num++;
					item[num].SetDefaults(3259);
					num++;
				}
				break;
			}
			case 19:
			{
				for (int num4 = 0; num4 < 40; num4++)
				{
					if (Main.travelShop[num4] != 0)
					{
						item[num].netDefaults(Main.travelShop[num4]);
						num++;
					}
				}
				break;
			}
			case 20:
				if (Main.moonPhase % 2 == 0)
				{
					item[num].SetDefaults(3001);
				}
				else
				{
					item[num].SetDefaults(28);
				}
				num++;
				if (!Main.dayTime || Main.moonPhase == 0)
				{
					item[num].SetDefaults(3002);
				}
				else
				{
					item[num].SetDefaults(282);
				}
				num++;
				if (Main.time % 60.0 * 60.0 * 6.0 <= 10800.0)
				{
					item[num].SetDefaults(3004);
				}
				else
				{
					item[num].SetDefaults(8);
				}
				num++;
				if (Main.moonPhase == 0 || Main.moonPhase == 1 || Main.moonPhase == 4 || Main.moonPhase == 5)
				{
					item[num].SetDefaults(3003);
				}
				else
				{
					item[num].SetDefaults(40);
				}
				num++;
				if (Main.moonPhase % 4 == 0)
				{
					item[num].SetDefaults(3310);
				}
				else if (Main.moonPhase % 4 == 1)
				{
					item[num].SetDefaults(3313);
				}
				else if (Main.moonPhase % 4 == 2)
				{
					item[num].SetDefaults(3312);
				}
				else
				{
					item[num].SetDefaults(3311);
				}
				num++;
				item[num].SetDefaults(166);
				num++;
				item[num].SetDefaults(965);
				num++;
				if (Main.hardMode)
				{
					if (Main.moonPhase < 4)
					{
						item[num].SetDefaults(3316);
					}
					else
					{
						item[num].SetDefaults(3315);
					}
					num++;
					item[num].SetDefaults(3334);
					num++;
					if (Main.bloodMoon)
					{
						item[num].SetDefaults(3258);
						num++;
					}
				}
				if (Main.moonPhase == 0 && !Main.dayTime)
				{
					item[num].SetDefaults(3043);
					num++;
				}
				break;
			}
			if (Main.player[Main.myPlayer].discount)
			{
				for (int num7 = 0; num7 < num; num7++)
				{
					item[num7].value = (int)((float)item[num7].value * 0.8f);
				}
			}
		}

		public static void UpdateChestFrames()
		{
			bool[] array = new bool[1000];
			for (int i = 0; i < 16; i++)
			{
				if (Main.player[i].active && Main.player[i].chest >= 0 && Main.player[i].chest < 1000)
				{
					array[Main.player[i].chest] = true;
				}
			}
			Chest chest = null;
			for (int j = 0; j < 1000; j++)
			{
				chest = Main.chest[j];
				if (chest != null)
				{
					if (array[j])
					{
						chest.frameCounter++;
					}
					else
					{
						chest.frameCounter--;
					}
					if (chest.frameCounter < 0)
					{
						chest.frameCounter = 0;
					}
					if (chest.frameCounter > 10)
					{
						chest.frameCounter = 10;
					}
					if (chest.frameCounter == 0)
					{
						chest.frame = 0;
					}
					else if (chest.frameCounter == 10)
					{
						chest.frame = 2;
					}
					else
					{
						chest.frame = 1;
					}
				}
			}
		}
	}
}
