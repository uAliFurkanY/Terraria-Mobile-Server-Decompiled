using System;
using System.Collections.Generic;
using Terraria.GameContent.NetModules;
using Terraria.Localization;
using Terraria.Net;
using Terraria.ObjectData;

namespace Terraria
{
	public class Liquid
	{
		public static int skipCount = 0;

		public static int stuckCount = 0;

		public static int stuckAmount = 0;

		public static int cycles = 10;

		public static int resLiquid = 5000;

		public static int maxLiquid = 5000;

		public static int numLiquid;

		public static bool stuck = false;

		public static bool quickFall = false;

		public static bool quickSettle = false;

		private static int wetCounter;

		public static int panicCounter = 0;

		public static bool panicMode = false;

		public static int panicY = 0;

		public int x;

		public int y;

		public int kill;

		public int delay;

		private static HashSet<int> _netChangeSet = new HashSet<int>();

		private static HashSet<int> _swapNetChangeSet = new HashSet<int>();

		public static void NetSendLiquid(int x, int y)
		{
			lock (_netChangeSet)
			{
				_netChangeSet.Add(((x & 0xFFFF) << 16) | (y & 0xFFFF));
			}
		}

		public static double QuickWater(int verbose = 0, int minY = -1, int maxY = -1)
		{
			Main.tileSolid[379] = true;
			int num = 0;
			if (minY == -1)
			{
				minY = 3;
			}
			if (maxY == -1)
			{
				maxY = Main.maxTilesY - 3;
			}
			for (int num2 = maxY; num2 >= minY; num2--)
			{
				if (verbose > 0)
				{
					float num3 = (float)(maxY - num2) / (float)(maxY - minY + 1);
					num3 /= (float)verbose;
					Main.statusText = string.Concat(Lang.gen[27], " ", (int)(num3 * 100f + 1f), "%");
				}
				else if (verbose < 0)
				{
					float num4 = (float)(maxY - num2) / (float)(maxY - minY + 1);
					num4 /= (float)(-verbose);
					Main.statusText = string.Concat(Lang.gen[18], " ", (int)(num4 * 100f + 1f), "%");
				}
				for (int i = 0; i < 2; i++)
				{
					int num5 = 2;
					int num6 = Main.maxTilesX - 2;
					int num7 = 1;
					if (i == 1)
					{
						num5 = Main.maxTilesX - 2;
						num6 = 2;
						num7 = -1;
					}
					for (int j = num5; j != num6; j += num7)
					{
						Tile tile = Main.tile[j, num2];
						if (tile.liquid <= 0)
						{
							continue;
						}
						int num8 = -num7;
						bool flag = false;
						int num9 = j;
						int num10 = num2;
						byte b = tile.liquidType();
						bool flag2 = tile.lava();
						bool flag3 = tile.honey();
						byte b2 = tile.liquid;
						tile.liquid = 0;
						bool flag4 = true;
						int num11 = 0;
						while (flag4 && num9 > 3 && num9 < Main.maxTilesX - 3 && num10 < Main.maxTilesY - 3)
						{
							flag4 = false;
							while (Main.tile[num9, num10 + 1].liquid == 0 && num10 < Main.maxTilesY - 5 && (!Main.tile[num9, num10 + 1].nactive() || !Main.tileSolid[Main.tile[num9, num10 + 1].type] || Main.tileSolidTop[Main.tile[num9, num10 + 1].type]))
							{
								flag = true;
								num8 = num7;
								num11 = 0;
								flag4 = true;
								num10++;
								if (num10 > WorldGen.waterLine && WorldGen.gen && !flag3)
								{
									b = 1;
								}
							}
							if (Main.tile[num9, num10 + 1].liquid > 0 && Main.tile[num9, num10 + 1].liquid < byte.MaxValue && Main.tile[num9, num10 + 1].liquidType() == b)
							{
								int num12 = 255 - Main.tile[num9, num10 + 1].liquid;
								if (num12 > b2)
								{
									num12 = b2;
								}
								Main.tile[num9, num10 + 1].liquid += (byte)num12;
								b2 = (byte)(b2 - (byte)num12);
								if (b2 <= 0)
								{
									num++;
									break;
								}
							}
							if (num11 == 0)
							{
								if (Main.tile[num9 + num8, num10].liquid == 0 && (!Main.tile[num9 + num8, num10].nactive() || !Main.tileSolid[Main.tile[num9 + num8, num10].type] || Main.tileSolidTop[Main.tile[num9 + num8, num10].type]))
								{
									num11 = num8;
								}
								else if (Main.tile[num9 - num8, num10].liquid == 0 && (!Main.tile[num9 - num8, num10].nactive() || !Main.tileSolid[Main.tile[num9 - num8, num10].type] || Main.tileSolidTop[Main.tile[num9 - num8, num10].type]))
								{
									num11 = -num8;
								}
							}
							if (num11 != 0 && Main.tile[num9 + num11, num10].liquid == 0 && (!Main.tile[num9 + num11, num10].nactive() || !Main.tileSolid[Main.tile[num9 + num11, num10].type] || Main.tileSolidTop[Main.tile[num9 + num11, num10].type]))
							{
								flag4 = true;
								num9 += num11;
							}
							if (flag && !flag4)
							{
								flag = false;
								flag4 = true;
								num8 = -num7;
								num11 = 0;
							}
						}
						if (j != num9 && num2 != num10)
						{
							num++;
						}
						Main.tile[num9, num10].liquid = b2;
						Main.tile[num9, num10].liquidType(b);
						if (Main.tile[num9 - 1, num10].liquid > 0 && Main.tile[num9 - 1, num10].lava() != flag2)
						{
							if (flag2)
							{
								LavaCheck(num9, num10);
							}
							else
							{
								LavaCheck(num9 - 1, num10);
							}
						}
						else if (Main.tile[num9 + 1, num10].liquid > 0 && Main.tile[num9 + 1, num10].lava() != flag2)
						{
							if (flag2)
							{
								LavaCheck(num9, num10);
							}
							else
							{
								LavaCheck(num9 + 1, num10);
							}
						}
						else if (Main.tile[num9, num10 - 1].liquid > 0 && Main.tile[num9, num10 - 1].lava() != flag2)
						{
							if (flag2)
							{
								LavaCheck(num9, num10);
							}
							else
							{
								LavaCheck(num9, num10 - 1);
							}
						}
						else if (Main.tile[num9, num10 + 1].liquid > 0 && Main.tile[num9, num10 + 1].lava() != flag2)
						{
							if (flag2)
							{
								LavaCheck(num9, num10);
							}
							else
							{
								LavaCheck(num9, num10 + 1);
							}
						}
						if (Main.tile[num9, num10].liquid <= 0)
						{
							continue;
						}
						if (Main.tile[num9 - 1, num10].liquid > 0 && Main.tile[num9 - 1, num10].honey() != flag3)
						{
							if (flag3)
							{
								HoneyCheck(num9, num10);
							}
							else
							{
								HoneyCheck(num9 - 1, num10);
							}
						}
						else if (Main.tile[num9 + 1, num10].liquid > 0 && Main.tile[num9 + 1, num10].honey() != flag3)
						{
							if (flag3)
							{
								HoneyCheck(num9, num10);
							}
							else
							{
								HoneyCheck(num9 + 1, num10);
							}
						}
						else if (Main.tile[num9, num10 - 1].liquid > 0 && Main.tile[num9, num10 - 1].honey() != flag3)
						{
							if (flag3)
							{
								HoneyCheck(num9, num10);
							}
							else
							{
								HoneyCheck(num9, num10 - 1);
							}
						}
						else if (Main.tile[num9, num10 + 1].liquid > 0 && Main.tile[num9, num10 + 1].honey() != flag3)
						{
							if (flag3)
							{
								HoneyCheck(num9, num10);
							}
							else
							{
								HoneyCheck(num9, num10 + 1);
							}
						}
					}
				}
			}
			return num;
		}

		public void Update()
		{
			Main.tileSolid[379] = true;
			Tile tile = Main.tile[x - 1, y];
			Tile tile2 = Main.tile[x + 1, y];
			Tile tile3 = Main.tile[x, y - 1];
			Tile tile4 = Main.tile[x, y + 1];
			Tile tile5 = Main.tile[x, y];
			if (tile5.nactive() && Main.tileSolid[tile5.type] && !Main.tileSolidTop[tile5.type])
			{
				ushort type = tile5.type;
				int num5 = 10;
				kill = 9;
				return;
			}
			byte liquid = tile5.liquid;
			float num = 0f;
			if (y > Main.maxTilesY - 200 && tile5.liquidType() == 0 && tile5.liquid > 0)
			{
				byte b = 2;
				if (tile5.liquid < b)
				{
					b = tile5.liquid;
				}
				tile5.liquid -= b;
			}
			if (tile5.liquid == 0)
			{
				kill = 9;
				return;
			}
			if (tile5.lava())
			{
				LavaCheck(x, y);
				if (!quickFall)
				{
					if (delay < 5)
					{
						delay++;
						return;
					}
					delay = 0;
				}
			}
			else
			{
				if (tile.lava())
				{
					AddWater(x - 1, y);
				}
				if (tile2.lava())
				{
					AddWater(x + 1, y);
				}
				if (tile3.lava())
				{
					AddWater(x, y - 1);
				}
				if (tile4.lava())
				{
					AddWater(x, y + 1);
				}
				if (tile5.honey())
				{
					HoneyCheck(x, y);
					if (!quickFall)
					{
						if (delay < 10)
						{
							delay++;
							return;
						}
						delay = 0;
					}
				}
				else
				{
					if (tile.honey())
					{
						AddWater(x - 1, y);
					}
					if (tile2.honey())
					{
						AddWater(x + 1, y);
					}
					if (tile3.honey())
					{
						AddWater(x, y - 1);
					}
					if (tile4.honey())
					{
						AddWater(x, y + 1);
					}
				}
			}
			if ((!tile4.nactive() || !Main.tileSolid[tile4.type] || Main.tileSolidTop[tile4.type]) && (tile4.liquid <= 0 || tile4.liquidType() == tile5.liquidType()) && tile4.liquid < byte.MaxValue)
			{
				num = 255 - tile4.liquid;
				if (num > (float)(int)tile5.liquid)
				{
					num = (int)tile5.liquid;
				}
				tile5.liquid -= (byte)num;
				tile4.liquid += (byte)num;
				tile4.liquidType(tile5.liquidType());
				AddWater(x, y + 1);
				tile4.skipLiquid(true);
				tile5.skipLiquid(true);
				if (tile5.liquid > 250)
				{
					tile5.liquid = byte.MaxValue;
				}
				else
				{
					AddWater(x - 1, y);
					AddWater(x + 1, y);
				}
			}
			if (tile5.liquid > 0)
			{
				bool flag = true;
				bool flag2 = true;
				bool flag3 = true;
				bool flag4 = true;
				if (tile.nactive() && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type])
				{
					flag = false;
				}
				else if (tile.liquid > 0 && tile.liquidType() != tile5.liquidType())
				{
					flag = false;
				}
				else if (Main.tile[x - 2, y].nactive() && Main.tileSolid[Main.tile[x - 2, y].type] && !Main.tileSolidTop[Main.tile[x - 2, y].type])
				{
					flag3 = false;
				}
				else if (Main.tile[x - 2, y].liquid == 0)
				{
					flag3 = false;
				}
				else if (Main.tile[x - 2, y].liquid > 0 && Main.tile[x - 2, y].liquidType() != tile5.liquidType())
				{
					flag3 = false;
				}
				if (tile2.nactive() && Main.tileSolid[tile2.type] && !Main.tileSolidTop[tile2.type])
				{
					flag2 = false;
				}
				else if (tile2.liquid > 0 && tile2.liquidType() != tile5.liquidType())
				{
					flag2 = false;
				}
				else if (Main.tile[x + 2, y].nactive() && Main.tileSolid[Main.tile[x + 2, y].type] && !Main.tileSolidTop[Main.tile[x + 2, y].type])
				{
					flag4 = false;
				}
				else if (Main.tile[x + 2, y].liquid == 0)
				{
					flag4 = false;
				}
				else if (Main.tile[x + 2, y].liquid > 0 && Main.tile[x + 2, y].liquidType() != tile5.liquidType())
				{
					flag4 = false;
				}
				int num2 = 0;
				if (tile5.liquid < 3)
				{
					num2 = -1;
				}
				if (flag && flag2)
				{
					if (flag3 && flag4)
					{
						bool flag5 = true;
						bool flag6 = true;
						if (Main.tile[x - 3, y].nactive() && Main.tileSolid[Main.tile[x - 3, y].type] && !Main.tileSolidTop[Main.tile[x - 3, y].type])
						{
							flag5 = false;
						}
						else if (Main.tile[x - 3, y].liquid == 0)
						{
							flag5 = false;
						}
						else if (Main.tile[x - 3, y].liquidType() != tile5.liquidType())
						{
							flag5 = false;
						}
						if (Main.tile[x + 3, y].nactive() && Main.tileSolid[Main.tile[x + 3, y].type] && !Main.tileSolidTop[Main.tile[x + 3, y].type])
						{
							flag6 = false;
						}
						else if (Main.tile[x + 3, y].liquid == 0)
						{
							flag6 = false;
						}
						else if (Main.tile[x + 3, y].liquidType() != tile5.liquidType())
						{
							flag6 = false;
						}
						if (flag5 && flag6)
						{
							num = tile.liquid + tile2.liquid + Main.tile[x - 2, y].liquid + Main.tile[x + 2, y].liquid + Main.tile[x - 3, y].liquid + Main.tile[x + 3, y].liquid + tile5.liquid + num2;
							num = (float)Math.Round(num / 7f);
							int num3 = 0;
							tile.liquidType(tile5.liquidType());
							if (tile.liquid != (byte)num)
							{
								tile.liquid = (byte)num;
								AddWater(x - 1, y);
							}
							else
							{
								num3++;
							}
							tile2.liquidType(tile5.liquidType());
							if (tile2.liquid != (byte)num)
							{
								tile2.liquid = (byte)num;
								AddWater(x + 1, y);
							}
							else
							{
								num3++;
							}
							Main.tile[x - 2, y].liquidType(tile5.liquidType());
							if (Main.tile[x - 2, y].liquid != (byte)num)
							{
								Main.tile[x - 2, y].liquid = (byte)num;
								AddWater(x - 2, y);
							}
							else
							{
								num3++;
							}
							Main.tile[x + 2, y].liquidType(tile5.liquidType());
							if (Main.tile[x + 2, y].liquid != (byte)num)
							{
								Main.tile[x + 2, y].liquid = (byte)num;
								AddWater(x + 2, y);
							}
							else
							{
								num3++;
							}
							Main.tile[x - 3, y].liquidType(tile5.liquidType());
							if (Main.tile[x - 3, y].liquid != (byte)num)
							{
								Main.tile[x - 3, y].liquid = (byte)num;
								AddWater(x - 3, y);
							}
							else
							{
								num3++;
							}
							Main.tile[x + 3, y].liquidType(tile5.liquidType());
							if (Main.tile[x + 3, y].liquid != (byte)num)
							{
								Main.tile[x + 3, y].liquid = (byte)num;
								AddWater(x + 3, y);
							}
							else
							{
								num3++;
							}
							if (tile.liquid != (byte)num || tile5.liquid != (byte)num)
							{
								AddWater(x - 1, y);
							}
							if (tile2.liquid != (byte)num || tile5.liquid != (byte)num)
							{
								AddWater(x + 1, y);
							}
							if (Main.tile[x - 2, y].liquid != (byte)num || tile5.liquid != (byte)num)
							{
								AddWater(x - 2, y);
							}
							if (Main.tile[x + 2, y].liquid != (byte)num || tile5.liquid != (byte)num)
							{
								AddWater(x + 2, y);
							}
							if (Main.tile[x - 3, y].liquid != (byte)num || tile5.liquid != (byte)num)
							{
								AddWater(x - 3, y);
							}
							if (Main.tile[x + 3, y].liquid != (byte)num || tile5.liquid != (byte)num)
							{
								AddWater(x + 3, y);
							}
							if (num3 != 6 || tile3.liquid <= 0)
							{
								tile5.liquid = (byte)num;
							}
						}
						else
						{
							int num4 = 0;
							num = tile.liquid + tile2.liquid + Main.tile[x - 2, y].liquid + Main.tile[x + 2, y].liquid + tile5.liquid + num2;
							num = (float)Math.Round(num / 5f);
							tile.liquidType(tile5.liquidType());
							if (tile.liquid != (byte)num)
							{
								tile.liquid = (byte)num;
								AddWater(x - 1, y);
							}
							else
							{
								num4++;
							}
							tile2.liquidType(tile5.liquidType());
							if (tile2.liquid != (byte)num)
							{
								tile2.liquid = (byte)num;
								AddWater(x + 1, y);
							}
							else
							{
								num4++;
							}
							Main.tile[x - 2, y].liquidType(tile5.liquidType());
							if (Main.tile[x - 2, y].liquid != (byte)num)
							{
								Main.tile[x - 2, y].liquid = (byte)num;
								AddWater(x - 2, y);
							}
							else
							{
								num4++;
							}
							Main.tile[x + 2, y].liquidType(tile5.liquidType());
							if (Main.tile[x + 2, y].liquid != (byte)num)
							{
								Main.tile[x + 2, y].liquid = (byte)num;
								AddWater(x + 2, y);
							}
							else
							{
								num4++;
							}
							if (tile.liquid != (byte)num || tile5.liquid != (byte)num)
							{
								AddWater(x - 1, y);
							}
							if (tile2.liquid != (byte)num || tile5.liquid != (byte)num)
							{
								AddWater(x + 1, y);
							}
							if (Main.tile[x - 2, y].liquid != (byte)num || tile5.liquid != (byte)num)
							{
								AddWater(x - 2, y);
							}
							if (Main.tile[x + 2, y].liquid != (byte)num || tile5.liquid != (byte)num)
							{
								AddWater(x + 2, y);
							}
							if (num4 != 4 || tile3.liquid <= 0)
							{
								tile5.liquid = (byte)num;
							}
						}
					}
					else if (flag3)
					{
						num = tile.liquid + tile2.liquid + Main.tile[x - 2, y].liquid + tile5.liquid + num2;
						num = (float)Math.Round((double)(num / 4f) + 0.001);
						tile.liquidType(tile5.liquidType());
						if (tile.liquid != (byte)num || tile5.liquid != (byte)num)
						{
							tile.liquid = (byte)num;
							AddWater(x - 1, y);
						}
						tile2.liquidType(tile5.liquidType());
						if (tile2.liquid != (byte)num || tile5.liquid != (byte)num)
						{
							tile2.liquid = (byte)num;
							AddWater(x + 1, y);
						}
						Main.tile[x - 2, y].liquidType(tile5.liquidType());
						if (Main.tile[x - 2, y].liquid != (byte)num || tile5.liquid != (byte)num)
						{
							Main.tile[x - 2, y].liquid = (byte)num;
							AddWater(x - 2, y);
						}
						tile5.liquid = (byte)num;
					}
					else if (flag4)
					{
						num = tile.liquid + tile2.liquid + Main.tile[x + 2, y].liquid + tile5.liquid + num2;
						num = (float)Math.Round((double)(num / 4f) + 0.001);
						tile.liquidType(tile5.liquidType());
						if (tile.liquid != (byte)num || tile5.liquid != (byte)num)
						{
							tile.liquid = (byte)num;
							AddWater(x - 1, y);
						}
						tile2.liquidType(tile5.liquidType());
						if (tile2.liquid != (byte)num || tile5.liquid != (byte)num)
						{
							tile2.liquid = (byte)num;
							AddWater(x + 1, y);
						}
						Main.tile[x + 2, y].liquidType(tile5.liquidType());
						if (Main.tile[x + 2, y].liquid != (byte)num || tile5.liquid != (byte)num)
						{
							Main.tile[x + 2, y].liquid = (byte)num;
							AddWater(x + 2, y);
						}
						tile5.liquid = (byte)num;
					}
					else
					{
						num = tile.liquid + tile2.liquid + tile5.liquid + num2;
						num = (float)Math.Round((double)(num / 3f) + 0.001);
						tile.liquidType(tile5.liquidType());
						if (tile.liquid != (byte)num)
						{
							tile.liquid = (byte)num;
						}
						if (tile5.liquid != (byte)num || tile.liquid != (byte)num)
						{
							AddWater(x - 1, y);
						}
						tile2.liquidType(tile5.liquidType());
						if (tile2.liquid != (byte)num)
						{
							tile2.liquid = (byte)num;
						}
						if (tile5.liquid != (byte)num || tile2.liquid != (byte)num)
						{
							AddWater(x + 1, y);
						}
						tile5.liquid = (byte)num;
					}
				}
				else if (flag)
				{
					num = tile.liquid + tile5.liquid + num2;
					num = (float)Math.Round((double)(num / 2f) + 0.001);
					if (tile.liquid != (byte)num)
					{
						tile.liquid = (byte)num;
					}
					tile.liquidType(tile5.liquidType());
					if (tile5.liquid != (byte)num || tile.liquid != (byte)num)
					{
						AddWater(x - 1, y);
					}
					tile5.liquid = (byte)num;
				}
				else if (flag2)
				{
					num = tile2.liquid + tile5.liquid + num2;
					num = (float)Math.Round((double)(num / 2f) + 0.001);
					if (tile2.liquid != (byte)num)
					{
						tile2.liquid = (byte)num;
					}
					tile2.liquidType(tile5.liquidType());
					if (tile5.liquid != (byte)num || tile2.liquid != (byte)num)
					{
						AddWater(x + 1, y);
					}
					tile5.liquid = (byte)num;
				}
			}
			if (tile5.liquid != liquid)
			{
				if (tile5.liquid == 254 && liquid == byte.MaxValue)
				{
					tile5.liquid = byte.MaxValue;
					kill++;
				}
				else
				{
					AddWater(x, y - 1);
					kill = 0;
				}
			}
			else
			{
				kill++;
			}
		}

		public static void StartPanic()
		{
			if (!panicMode)
			{
				WorldGen.waterLine = Main.maxTilesY;
				numLiquid = 0;
				LiquidBuffer.numLiquidBuffer = 0;
				panicCounter = 0;
				panicMode = true;
				panicY = Main.maxTilesY - 3;
				if (Main.dedServ)
				{
					Console.WriteLine(Language.GetTextValue("Misc.ForceWaterSettling"));
				}
			}
		}

		public static void UpdateLiquid()
		{
			int netMode = Main.netMode;
			int num8 = 2;
			if (!WorldGen.gen)
			{
				if (!panicMode)
				{
					if (numLiquid + LiquidBuffer.numLiquidBuffer > 4000)
					{
						panicCounter++;
						if (panicCounter > 1800 || numLiquid + LiquidBuffer.numLiquidBuffer > 13500)
						{
							StartPanic();
						}
					}
					else
					{
						panicCounter = 0;
					}
				}
				if (panicMode)
				{
					int num = 0;
					while (panicY >= 3 && num < 5)
					{
						num++;
						QuickWater(0, panicY, panicY);
						panicY--;
						if (panicY >= 3)
						{
							continue;
						}
						Console.WriteLine(Language.GetTextValue("Misc.WaterSettled"));
						panicCounter = 0;
						panicMode = false;
						WorldGen.WaterCheck();
						if (Main.netMode != 2)
						{
							continue;
						}
						for (int i = 0; i < 16; i++)
						{
							for (int j = 0; j < Main.maxSectionsX; j++)
							{
								for (int k = 0; k < Main.maxSectionsY; k++)
								{
									Netplay.Clients[i].TileSections[j, k] = false;
								}
							}
						}
					}
					return;
				}
			}
			if (quickSettle || numLiquid > 2000)
			{
				quickFall = true;
			}
			else
			{
				quickFall = false;
			}
			wetCounter++;
			int num2 = maxLiquid / cycles;
			int num3 = num2 * (wetCounter - 1);
			int num4 = num2 * wetCounter;
			if (wetCounter == cycles)
			{
				num4 = numLiquid;
			}
			if (num4 > numLiquid)
			{
				num4 = numLiquid;
				int netMode2 = Main.netMode;
				wetCounter = cycles;
			}
			if (quickFall)
			{
				for (int l = num3; l < num4; l++)
				{
					Main.liquid[l].delay = 10;
					Main.liquid[l].Update();
					Main.tile[Main.liquid[l].x, Main.liquid[l].y].skipLiquid(false);
				}
			}
			else
			{
				for (int m = num3; m < num4; m++)
				{
					if (!Main.tile[Main.liquid[m].x, Main.liquid[m].y].skipLiquid())
					{
						Main.liquid[m].Update();
					}
					else
					{
						Main.tile[Main.liquid[m].x, Main.liquid[m].y].skipLiquid(false);
					}
				}
			}
			if (wetCounter >= cycles)
			{
				wetCounter = 0;
				for (int num5 = numLiquid - 1; num5 >= 0; num5--)
				{
					if (Main.liquid[num5].kill > 4)
					{
						DelWater(num5);
					}
				}
				int num6 = maxLiquid - (maxLiquid - numLiquid);
				if (num6 > LiquidBuffer.numLiquidBuffer)
				{
					num6 = LiquidBuffer.numLiquidBuffer;
				}
				for (int n = 0; n < num6; n++)
				{
					Main.tile[Main.liquidBuffer[0].x, Main.liquidBuffer[0].y].checkingLiquid(false);
					AddWater(Main.liquidBuffer[0].x, Main.liquidBuffer[0].y);
					LiquidBuffer.DelBuffer(0);
				}
				if (numLiquid > 0 && numLiquid > stuckAmount - 50 && numLiquid < stuckAmount + 50)
				{
					stuckCount++;
					if (stuckCount >= 10000)
					{
						stuck = true;
						for (int num7 = numLiquid - 1; num7 >= 0; num7--)
						{
							DelWater(num7);
						}
						stuck = false;
						stuckCount = 0;
					}
				}
				else
				{
					stuckCount = 0;
					stuckAmount = numLiquid;
				}
			}
			if (Main.netMode == 2 && _netChangeSet.Count > 0)
			{
				Utils.Swap(ref _netChangeSet, ref _swapNetChangeSet);
				NetManager.Instance.Broadcast(NetLiquidModule.Serialize(_swapNetChangeSet));
				_swapNetChangeSet.Clear();
			}
		}

		public static void AddWater(int x, int y)
		{
			Tile tile = Main.tile[x, y];
			if (Main.tile[x, y] == null || tile.checkingLiquid() || x >= Main.maxTilesX - 5 || y >= Main.maxTilesY - 5 || x < 5 || y < 5 || tile.liquid == 0)
			{
				return;
			}
			if (numLiquid >= maxLiquid - 1)
			{
				LiquidBuffer.AddBuffer(x, y);
				return;
			}
			tile.checkingLiquid(true);
			Main.liquid[numLiquid].kill = 0;
			Main.liquid[numLiquid].x = x;
			Main.liquid[numLiquid].y = y;
			Main.liquid[numLiquid].delay = 0;
			tile.skipLiquid(false);
			numLiquid++;
			if (Main.netMode == 2)
			{
				NetSendLiquid(x, y);
			}
			if (!tile.active() || WorldGen.gen)
			{
				return;
			}
			bool flag = false;
			if (tile.lava())
			{
				if (TileObjectData.CheckLavaDeath(tile))
				{
					flag = true;
				}
			}
			else if (TileObjectData.CheckWaterDeath(tile))
			{
				flag = true;
			}
			if (flag)
			{
				WorldGen.KillTile(x, y);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(17, -1, -1, "", 0, x, y);
				}
			}
		}

		public static void LavaCheck(int x, int y)
		{
			Tile tile = Main.tile[x - 1, y];
			Tile tile2 = Main.tile[x + 1, y];
			Tile tile3 = Main.tile[x, y - 1];
			Tile tile4 = Main.tile[x, y + 1];
			Tile tile5 = Main.tile[x, y];
			if ((tile.liquid > 0 && !tile.lava()) || (tile2.liquid > 0 && !tile2.lava()) || (tile3.liquid > 0 && !tile3.lava()))
			{
				int num = 0;
				int type = 56;
				if (!tile.lava())
				{
					num += tile.liquid;
					tile.liquid = 0;
				}
				if (!tile2.lava())
				{
					num += tile2.liquid;
					tile2.liquid = 0;
				}
				if (!tile3.lava())
				{
					num += tile3.liquid;
					tile3.liquid = 0;
				}
				if (tile.honey() || tile2.honey() || tile3.honey())
				{
					type = 230;
				}
				if (num < 24)
				{
					return;
				}
				if (tile5.active() && Main.tileObsidianKill[tile5.type])
				{
					WorldGen.KillTile(x, y);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(17, -1, -1, "", 0, x, y);
					}
				}
				if (!tile5.active())
				{
					tile5.liquid = 0;
					tile5.lava(false);
					WorldGen.PlaceTile(x, y, type, true, true);
					WorldGen.SquareTileFrame(x, y);
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, x - 1, y - 1, 3);
					}
				}
			}
			else
			{
				if (tile4.liquid <= 0 || tile4.lava())
				{
					return;
				}
				if (Main.tileCut[tile4.type])
				{
					WorldGen.KillTile(x, y + 1);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(17, -1, -1, "", 0, x, y + 1);
					}
				}
				else if (tile4.active() && Main.tileObsidianKill[tile4.type])
				{
					WorldGen.KillTile(x, y + 1);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(17, -1, -1, "", 0, x, y + 1);
					}
				}
				if (tile4.active())
				{
					return;
				}
				if (tile5.liquid < 24)
				{
					tile5.liquid = 0;
					tile5.liquidType(0);
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, x - 1, y, 3);
					}
					return;
				}
				int type2 = 56;
				if (tile4.honey())
				{
					type2 = 230;
				}
				tile5.liquid = 0;
				tile5.lava(false);
				tile4.liquid = 0;
				WorldGen.PlaceTile(x, y + 1, type2, true, true);
				WorldGen.SquareTileFrame(x, y + 1);
				if (Main.netMode == 2)
				{
					NetMessage.SendTileSquare(-1, x - 1, y, 3);
				}
			}
		}

		public static void HoneyCheck(int x, int y)
		{
			Tile tile = Main.tile[x - 1, y];
			Tile tile2 = Main.tile[x + 1, y];
			Tile tile3 = Main.tile[x, y - 1];
			Tile tile4 = Main.tile[x, y + 1];
			Tile tile5 = Main.tile[x, y];
			if ((tile.liquid > 0 && tile.liquidType() == 0) || (tile2.liquid > 0 && tile2.liquidType() == 0) || (tile3.liquid > 0 && tile3.liquidType() == 0))
			{
				int num = 0;
				if (tile.liquidType() == 0)
				{
					num += tile.liquid;
					tile.liquid = 0;
				}
				if (tile2.liquidType() == 0)
				{
					num += tile2.liquid;
					tile2.liquid = 0;
				}
				if (tile3.liquidType() == 0)
				{
					num += tile3.liquid;
					tile3.liquid = 0;
				}
				if (num < 32)
				{
					return;
				}
				if (tile5.active() && Main.tileObsidianKill[tile5.type])
				{
					WorldGen.KillTile(x, y);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(17, -1, -1, "", 0, x, y);
					}
				}
				if (!tile5.active())
				{
					tile5.liquid = 0;
					tile5.liquidType(0);
					WorldGen.PlaceTile(x, y, 229, true, true);
					WorldGen.SquareTileFrame(x, y);
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, x - 1, y - 1, 3);
					}
				}
			}
			else
			{
				if (tile4.liquid <= 0 || tile4.liquidType() != 0)
				{
					return;
				}
				if (Main.tileCut[tile4.type])
				{
					WorldGen.KillTile(x, y + 1);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(17, -1, -1, "", 0, x, y + 1);
					}
				}
				else if (tile4.active() && Main.tileObsidianKill[tile4.type])
				{
					WorldGen.KillTile(x, y + 1);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(17, -1, -1, "", 0, x, y + 1);
					}
				}
				if (tile4.active())
				{
					return;
				}
				if (tile5.liquid < 32)
				{
					tile5.liquid = 0;
					tile5.liquidType(0);
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, x - 1, y, 3);
					}
					return;
				}
				tile5.liquid = 0;
				tile5.liquidType(0);
				tile4.liquid = 0;
				tile4.liquidType(0);
				WorldGen.PlaceTile(x, y + 1, 229, true, true);
				WorldGen.SquareTileFrame(x, y + 1);
				if (Main.netMode == 2)
				{
					NetMessage.SendTileSquare(-1, x - 1, y, 3);
				}
			}
		}

		public static void DelWater(int l)
		{
			int num = Main.liquid[l].x;
			int num2 = Main.liquid[l].y;
			Tile tile = Main.tile[num - 1, num2];
			Tile tile2 = Main.tile[num + 1, num2];
			Tile tile3 = Main.tile[num, num2 + 1];
			Tile tile4 = Main.tile[num, num2];
			byte b = 2;
			if (tile4.liquid < b)
			{
				tile4.liquid = 0;
				if (tile.liquid < b)
				{
					tile.liquid = 0;
				}
				else
				{
					AddWater(num - 1, num2);
				}
				if (tile2.liquid < b)
				{
					tile2.liquid = 0;
				}
				else
				{
					AddWater(num + 1, num2);
				}
			}
			else if (tile4.liquid < 20)
			{
				if ((tile.liquid < tile4.liquid && (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type])) || (tile2.liquid < tile4.liquid && (!tile2.nactive() || !Main.tileSolid[tile2.type] || Main.tileSolidTop[tile2.type])) || (tile3.liquid < byte.MaxValue && (!tile3.nactive() || !Main.tileSolid[tile3.type] || Main.tileSolidTop[tile3.type])))
				{
					tile4.liquid = 0;
				}
			}
			else if (tile3.liquid < byte.MaxValue && (!tile3.nactive() || !Main.tileSolid[tile3.type] || Main.tileSolidTop[tile3.type]) && !stuck)
			{
				Main.liquid[l].kill = 0;
				return;
			}
			if (tile4.liquid < 250 && Main.tile[num, num2 - 1].liquid > 0)
			{
				AddWater(num, num2 - 1);
			}
			if (tile4.liquid == 0)
			{
				tile4.liquidType(0);
			}
			else
			{
				if ((tile2.liquid > 0 && Main.tile[num + 1, num2 + 1].liquid < 250 && !Main.tile[num + 1, num2 + 1].active()) || (tile.liquid > 0 && Main.tile[num - 1, num2 + 1].liquid < 250 && !Main.tile[num - 1, num2 + 1].active()))
				{
					AddWater(num - 1, num2);
					AddWater(num + 1, num2);
				}
				if (tile4.lava())
				{
					LavaCheck(num, num2);
					for (int i = num - 1; i <= num + 1; i++)
					{
						for (int j = num2 - 1; j <= num2 + 1; j++)
						{
							Tile tile5 = Main.tile[i, j];
							if (!tile5.active())
							{
								continue;
							}
							if (tile5.type == 2 || tile5.type == 23 || tile5.type == 109 || tile5.type == 199)
							{
								tile5.type = 0;
								WorldGen.SquareTileFrame(i, j);
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, num, num2, 3);
								}
							}
							else if (tile5.type == 60 || tile5.type == 70)
							{
								tile5.type = 59;
								WorldGen.SquareTileFrame(i, j);
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, num, num2, 3);
								}
							}
						}
					}
				}
				else if (tile4.honey())
				{
					HoneyCheck(num, num2);
				}
			}
			if (Main.netMode == 2)
			{
				NetSendLiquid(num, num2);
			}
			numLiquid--;
			Main.tile[Main.liquid[l].x, Main.liquid[l].y].checkingLiquid(false);
			Main.liquid[l].x = Main.liquid[numLiquid].x;
			Main.liquid[l].y = Main.liquid[numLiquid].y;
			Main.liquid[l].kill = Main.liquid[numLiquid].kill;
			if (Main.tileAlch[tile4.type])
			{
				WorldGen.CheckAlch(num, num2);
			}
		}
	}
}
