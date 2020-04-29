using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	internal class TrackGenerator
	{
		private struct TrackHistory
		{
			public short X;

			public short Y;

			public byte YDirection;

			public TrackHistory(int x, int y, int yDirection)
			{
				X = (short)x;
				Y = (short)y;
				YDirection = (byte)yDirection;
			}

			public TrackHistory(short x, short y, byte yDirection)
			{
				X = x;
				Y = y;
				YDirection = yDirection;
			}
		}

		private const int TOTAL_TILE_IGNORES = 150;

		private const int PLAYER_HEIGHT = 6;

		private const int MAX_RETRIES = 400;

		private const int MAX_SMOOTH_DISTANCE = 15;

		private const int MAX_ITERATIONS = 1000000;

		private static readonly byte[] INVALID_WALLS = new byte[13]
		{
			7,
			94,
			95,
			8,
			98,
			99,
			9,
			96,
			97,
			3,
			83,
			87,
			86
		};

		private TrackHistory[] _historyCache = new TrackHistory[2048];

		public void Generate(int trackCount, int minimumLength)
		{
			Random random = new Random();
			int num = trackCount;
			while (num > 0)
			{
				int x = random.Next(150, Main.maxTilesX - 150);
				int i = random.Next((int)Main.worldSurface + 25, Main.maxTilesY - 200);
				if (IsLocationEmpty(x, i))
				{
					for (; IsLocationEmpty(x, i + 1); i++)
					{
					}
					if (FindPath(x, i, minimumLength))
					{
						num--;
					}
				}
			}
		}

		private bool IsLocationEmpty(int x, int y)
		{
			if (y > Main.maxTilesY - 200 || x < 0 || y < (int)Main.worldSurface || x > Main.maxTilesX - 5)
			{
				return false;
			}
			for (int i = 0; i < 6; i++)
			{
				if (WorldGen.SolidTile(x, y - i))
				{
					return false;
				}
			}
			return true;
		}

		private bool CanTrackBePlaced(int x, int y)
		{
			if (y > Main.maxTilesY - 200 || x < 0 || y < (int)Main.worldSurface || x > Main.maxTilesX - 5)
			{
				return false;
			}
			byte wall = Main.tile[x, y].wall;
			for (int i = 0; i < INVALID_WALLS.Length; i++)
			{
				if (wall == INVALID_WALLS[i])
				{
					return false;
				}
			}
			for (int j = -1; j <= 1; j++)
			{
				if (Main.tile[x + j, y].active() && (Main.tile[x + j, y].type == 314 || !TileID.Sets.GeneralPlacementTiles[Main.tile[x + j, y].type]))
				{
					return false;
				}
			}
			return true;
		}

		private void SmoothTrack(TrackHistory[] history, int length)
		{
			int num = length - 1;
			bool flag = false;
			for (int num2 = length - 1; num2 >= 0; num2--)
			{
				if (flag)
				{
					num = Math.Min(num2 + 15, num);
					if (history[num2].Y >= history[num].Y)
					{
						for (int i = num2 + 1; history[i].Y > history[num2].Y; i++)
						{
							history[i].Y = history[num2].Y;
						}
						if (history[num2].Y == history[num].Y)
						{
							flag = false;
						}
					}
				}
				else if (history[num2].Y > history[num].Y)
				{
					flag = true;
				}
				else
				{
					num = num2;
				}
			}
		}

		public bool FindPath(int x, int y, int minimumLength, bool debugMode = false)
		{
			TrackHistory[] historyCache = _historyCache;
			int num = 0;
			Tile[,] tile = Main.tile;
			bool flag = true;
			int num2 = (new Random().Next(2) == 0) ? 1 : (-1);
			if (debugMode)
			{
				num2 = Main.player[Main.myPlayer].direction;
			}
			int num3 = 1;
			int num4 = 0;
			int num5 = 400;
			bool flag2 = false;
			int num6 = 150;
			int num7 = 0;
			int num8 = 1000000;
			while (num8 > 0 && flag && num < historyCache.Length - 1)
			{
				num8--;
				historyCache[num] = new TrackHistory(x, y, num3);
				bool flag3 = false;
				int num9 = 1;
				if (num > minimumLength >> 1)
				{
					num9 = -1;
				}
				else if (num > (minimumLength >> 1) - 5)
				{
					num9 = 0;
				}
				if (flag2)
				{
					int num10 = 0;
					int num11 = num6;
					bool flag4 = false;
					for (int num12 = Math.Min(1, num3 + 1); num12 >= Math.Max(-1, num3 - 1); num12--)
					{
						int i;
						for (i = 0; i <= num6; i++)
						{
							if (IsLocationEmpty(x + (i + 1) * num2, y + (i + 1) * num12 * num9))
							{
								flag4 = true;
								break;
							}
						}
						if (i < num11)
						{
							num11 = i;
							num10 = num12;
						}
					}
					if (flag4)
					{
						num3 = num10;
						for (int j = 0; j < num11 - 1; j++)
						{
							num++;
							x += num2;
							y += num3 * num9;
							historyCache[num] = new TrackHistory(x, y, num3);
							num7 = num;
						}
						x += num2;
						y += num3 * num9;
						num4 = num + 1;
						flag2 = false;
					}
					num6 -= num11;
					if (num6 < 0)
					{
						flag = false;
					}
				}
				else
				{
					for (int num13 = Math.Min(1, num3 + 1); num13 >= Math.Max(-1, num3 - 1); num13--)
					{
						if (IsLocationEmpty(x + num2, y + num13 * num9))
						{
							num3 = num13;
							flag3 = true;
							x += num2;
							y += num3 * num9;
							num4 = num + 1;
							break;
						}
					}
					if (!flag3)
					{
						while (num > num7 && y == historyCache[num].Y)
						{
							num--;
						}
						x = historyCache[num].X;
						y = historyCache[num].Y;
						num3 = historyCache[num].YDirection - 1;
						num5--;
						if (num5 <= 0)
						{
							num = num4;
							x = historyCache[num].X;
							y = historyCache[num].Y;
							num3 = historyCache[num].YDirection;
							flag2 = true;
							num5 = 200;
						}
						num--;
					}
				}
				num++;
			}
			if (num4 > minimumLength || debugMode)
			{
				SmoothTrack(historyCache, num4);
				if (!debugMode)
				{
					for (int k = 0; k < num4; k++)
					{
						for (int l = -1; l < 7; l++)
						{
							if (!CanTrackBePlaced(historyCache[k].X, historyCache[k].Y - l))
							{
								return false;
							}
						}
					}
				}
				for (int m = 0; m < num4; m++)
				{
					TrackHistory trackHistory = historyCache[m];
					for (int n = 0; n < 6; n++)
					{
						Main.tile[trackHistory.X, trackHistory.Y - n].active(false);
					}
				}
				for (int num14 = 0; num14 < num4; num14++)
				{
					TrackHistory trackHistory2 = historyCache[num14];
					Tile.SmoothSlope(trackHistory2.X, trackHistory2.Y + 1);
					Tile.SmoothSlope(trackHistory2.X, trackHistory2.Y - 6);
					bool wire = Main.tile[trackHistory2.X, trackHistory2.Y].wire();
					Main.tile[trackHistory2.X, trackHistory2.Y].ResetToType(314);
					Main.tile[trackHistory2.X, trackHistory2.Y].wire(wire);
					if (num14 == 0)
					{
						continue;
					}
					for (int num15 = 0; num15 < 6; num15++)
					{
						WorldUtils.TileFrame(historyCache[num14 - 1].X, historyCache[num14 - 1].Y - num15, true);
					}
					if (num14 == num4 - 1)
					{
						for (int num16 = 0; num16 < 6; num16++)
						{
							WorldUtils.TileFrame(trackHistory2.X, trackHistory2.Y - num16, true);
						}
					}
				}
				return true;
			}
			return false;
		}

		public static void Run(int trackCount = 30, int minimumLength = 250)
		{
			new TrackGenerator().Generate(trackCount, minimumLength);
		}

		public static void Run(Point start)
		{
			new TrackGenerator().FindPath(start.X, start.Y, 250, true);
		}
	}
}
