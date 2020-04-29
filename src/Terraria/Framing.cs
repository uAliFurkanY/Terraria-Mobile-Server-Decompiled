using Terraria.DataStructures;

namespace Terraria
{
	public class Framing
	{
		private struct BlockStyle
		{
			public bool top;

			public bool bottom;

			public bool left;

			public bool right;

			public BlockStyle(bool up, bool down, bool left, bool right)
			{
				top = up;
				bottom = down;
				this.left = left;
				this.right = right;
			}

			public void Clear()
			{
				top = (bottom = (left = (right = false)));
			}
		}

		private static Point16[][] selfFrame8WayLookup;

		private static Point16[][] wallFrameLookup;

		private static Point16 frameSize8Way;

		private static Point16 wallFrameSize;

		private static BlockStyle[] blockStyleLookup;

		private static int[][] phlebasTileFrameNumberLookup;

		private static int[][] lazureTileFrameNumberLookup;

		private static int[][] centerWallFrameLookup;

		public static void Initialize()
		{
			selfFrame8WayLookup = new Point16[256][];
			frameSize8Way = new Point16(18, 18);
			Add8WayLookup(0, 9, 3, 10, 3, 11, 3);
			Add8WayLookup(1, 6, 3, 7, 3, 8, 3);
			Add8WayLookup(2, 12, 0, 12, 1, 12, 2);
			Add8WayLookup(3, 15, 2);
			Add8WayLookup(4, 9, 0, 9, 1, 9, 2);
			Add8WayLookup(5, 13, 2);
			Add8WayLookup(6, 6, 4, 7, 4, 8, 4);
			Add8WayLookup(7, 14, 2);
			Add8WayLookup(8, 6, 0, 7, 0, 8, 0);
			Add8WayLookup(9, 5, 0, 5, 1, 5, 2);
			Add8WayLookup(10, 15, 0);
			Add8WayLookup(11, 15, 1);
			Add8WayLookup(12, 13, 0);
			Add8WayLookup(13, 13, 1);
			Add8WayLookup(14, 14, 0);
			Add8WayLookup(15, 14, 1);
			Add8WayLookup(19, 1, 4, 3, 4, 5, 4);
			Add8WayLookup(23, 16, 3);
			Add8WayLookup(27, 17, 0);
			Add8WayLookup(31, 13, 4);
			Add8WayLookup(37, 0, 4, 2, 4, 4, 4);
			Add8WayLookup(39, 17, 3);
			Add8WayLookup(45, 16, 0);
			Add8WayLookup(47, 12, 4);
			Add8WayLookup(55, 1, 2, 2, 2, 3, 2);
			Add8WayLookup(63, 6, 2, 7, 2, 8, 2);
			Add8WayLookup(74, 1, 3, 3, 3, 5, 3);
			Add8WayLookup(75, 17, 1);
			Add8WayLookup(78, 16, 2);
			Add8WayLookup(79, 13, 3);
			Add8WayLookup(91, 4, 0, 4, 1, 4, 2);
			Add8WayLookup(95, 11, 0, 11, 1, 11, 2);
			Add8WayLookup(111, 17, 4);
			Add8WayLookup(127, 14, 3);
			Add8WayLookup(140, 0, 3, 2, 3, 4, 3);
			Add8WayLookup(141, 16, 1);
			Add8WayLookup(142, 17, 2);
			Add8WayLookup(143, 12, 3);
			Add8WayLookup(159, 16, 4);
			Add8WayLookup(173, 0, 0, 0, 1, 0, 2);
			Add8WayLookup(175, 10, 0, 10, 1, 10, 2);
			Add8WayLookup(191, 15, 3);
			Add8WayLookup(206, 1, 0, 2, 0, 3, 0);
			Add8WayLookup(207, 6, 1, 7, 1, 8, 1);
			Add8WayLookup(223, 14, 4);
			Add8WayLookup(239, 15, 4);
			Add8WayLookup(255, 1, 1, 2, 1, 3, 1);
			blockStyleLookup = new BlockStyle[6];
			blockStyleLookup[0] = new BlockStyle(true, true, true, true);
			blockStyleLookup[1] = new BlockStyle(false, true, true, true);
			blockStyleLookup[2] = new BlockStyle(false, true, true, false);
			blockStyleLookup[3] = new BlockStyle(false, true, false, true);
			blockStyleLookup[4] = new BlockStyle(true, false, true, false);
			blockStyleLookup[5] = new BlockStyle(true, false, false, true);
			phlebasTileFrameNumberLookup = new int[4][]
			{
				new int[3]
				{
					2,
					4,
					2
				},
				new int[3]
				{
					1,
					3,
					1
				},
				new int[3]
				{
					2,
					2,
					4
				},
				new int[3]
				{
					1,
					1,
					3
				}
			};
			lazureTileFrameNumberLookup = new int[2][]
			{
				new int[2]
				{
					1,
					2
				},
				new int[2]
				{
					3,
					4
				}
			};
			centerWallFrameLookup = new int[3][]
			{
				new int[3]
				{
					2,
					0,
					0
				},
				new int[3]
				{
					0,
					1,
					4
				},
				new int[3]
				{
					0,
					3,
					0
				}
			};
			wallFrameLookup = new Point16[20][];
			wallFrameSize = new Point16(36, 36);
			AddWallFrameLookup(0, 9, 3, 10, 3, 11, 3, 6, 6);
			AddWallFrameLookup(1, 6, 3, 7, 3, 8, 3, 4, 6);
			AddWallFrameLookup(2, 12, 0, 12, 1, 12, 2, 12, 5);
			AddWallFrameLookup(3, 1, 4, 3, 4, 5, 4, 3, 6);
			AddWallFrameLookup(4, 9, 0, 9, 1, 9, 2, 9, 5);
			AddWallFrameLookup(5, 0, 4, 2, 4, 4, 4, 2, 6);
			AddWallFrameLookup(6, 6, 4, 7, 4, 8, 4, 5, 6);
			AddWallFrameLookup(7, 1, 2, 2, 2, 3, 2, 3, 5);
			AddWallFrameLookup(8, 6, 0, 7, 0, 8, 0, 6, 5);
			AddWallFrameLookup(9, 5, 0, 5, 1, 5, 2, 5, 5);
			AddWallFrameLookup(10, 1, 3, 3, 3, 5, 3, 1, 6);
			AddWallFrameLookup(11, 4, 0, 4, 1, 4, 2, 4, 5);
			AddWallFrameLookup(12, 0, 3, 2, 3, 4, 3, 0, 6);
			AddWallFrameLookup(13, 0, 0, 0, 1, 0, 2, 0, 5);
			AddWallFrameLookup(14, 1, 0, 2, 0, 3, 0, 1, 6);
			AddWallFrameLookup(15, 1, 1, 2, 1, 3, 1, 2, 5);
			AddWallFrameLookup(16, 6, 1, 7, 1, 8, 1, 7, 5);
			AddWallFrameLookup(17, 6, 2, 7, 2, 8, 2, 8, 5);
			AddWallFrameLookup(18, 10, 0, 10, 1, 10, 2, 10, 5);
			AddWallFrameLookup(19, 11, 0, 11, 1, 11, 2, 11, 5);
		}

		private static BlockStyle FindBlockStyle(Tile blockTile)
		{
			return blockStyleLookup[blockTile.blockType()];
		}

		public static void Add8WayLookup(int lookup, short point1X, short point1Y, short point2X, short point2Y, short point3X, short point3Y)
		{
			Point16[] array = new Point16[3]
			{
				new Point16(point1X * frameSize8Way.X, point1Y * frameSize8Way.Y),
				new Point16(point2X * frameSize8Way.X, point2Y * frameSize8Way.Y),
				new Point16(point3X * frameSize8Way.X, point3Y * frameSize8Way.Y)
			};
			selfFrame8WayLookup[lookup] = array;
		}

		public static void Add8WayLookup(int lookup, short x, short y)
		{
			Point16[] array = new Point16[3]
			{
				new Point16(x * frameSize8Way.X, y * frameSize8Way.Y),
				new Point16(x * frameSize8Way.X, y * frameSize8Way.Y),
				new Point16(x * frameSize8Way.X, y * frameSize8Way.Y)
			};
			selfFrame8WayLookup[lookup] = array;
		}

		public static void AddWallFrameLookup(int lookup, short point1X, short point1Y, short point2X, short point2Y, short point3X, short point3Y, short point4X, short point4Y)
		{
			Point16[] array = new Point16[4]
			{
				new Point16(point1X * wallFrameSize.X, point1Y * wallFrameSize.Y),
				new Point16(point2X * wallFrameSize.X, point2Y * wallFrameSize.Y),
				new Point16(point3X * wallFrameSize.X, point3Y * wallFrameSize.Y),
				new Point16(point4X * wallFrameSize.X, point4Y * wallFrameSize.Y)
			};
			wallFrameLookup[lookup] = array;
		}

		public static void SelfFrame4Way()
		{
		}

		public static void SelfFrame8Way(int i, int j, Tile centerTile, bool resetFrame)
		{
			if (!centerTile.active())
			{
				return;
			}
			ushort type = centerTile.type;
			BlockStyle blockStyle = FindBlockStyle(centerTile);
			int num = 0;
			BlockStyle blockStyle2 = default(BlockStyle);
			if (blockStyle.top)
			{
				Tile tileSafely = GetTileSafely(i, j - 1);
				if (tileSafely.active() && tileSafely.type == type)
				{
					blockStyle2 = FindBlockStyle(tileSafely);
					if (blockStyle2.bottom)
					{
						num |= 1;
					}
					else
					{
						blockStyle2.Clear();
					}
				}
			}
			BlockStyle blockStyle3 = default(BlockStyle);
			if (blockStyle.left)
			{
				Tile tileSafely2 = GetTileSafely(i - 1, j);
				if (tileSafely2.active() && tileSafely2.type == type)
				{
					blockStyle3 = FindBlockStyle(tileSafely2);
					if (blockStyle3.right)
					{
						num |= 2;
					}
					else
					{
						blockStyle3.Clear();
					}
				}
			}
			BlockStyle blockStyle4 = default(BlockStyle);
			if (blockStyle.right)
			{
				Tile tileSafely3 = GetTileSafely(i + 1, j);
				if (tileSafely3.active() && tileSafely3.type == type)
				{
					blockStyle4 = FindBlockStyle(tileSafely3);
					if (blockStyle4.left)
					{
						num |= 4;
					}
					else
					{
						blockStyle4.Clear();
					}
				}
			}
			BlockStyle blockStyle5 = default(BlockStyle);
			if (blockStyle.bottom)
			{
				Tile tileSafely4 = GetTileSafely(i, j + 1);
				if (tileSafely4.active() && tileSafely4.type == type)
				{
					blockStyle5 = FindBlockStyle(tileSafely4);
					if (blockStyle5.top)
					{
						num |= 8;
					}
					else
					{
						blockStyle5.Clear();
					}
				}
			}
			if (blockStyle2.left && blockStyle3.top)
			{
				Tile tileSafely5 = GetTileSafely(i - 1, j - 1);
				if (tileSafely5.active() && tileSafely5.type == type)
				{
					BlockStyle blockStyle6 = FindBlockStyle(tileSafely5);
					if (blockStyle6.right && blockStyle6.bottom)
					{
						num |= 0x10;
					}
				}
			}
			if (blockStyle2.right && blockStyle4.top)
			{
				Tile tileSafely6 = GetTileSafely(i + 1, j - 1);
				if (tileSafely6.active() && tileSafely6.type == type)
				{
					BlockStyle blockStyle7 = FindBlockStyle(tileSafely6);
					if (blockStyle7.left && blockStyle7.bottom)
					{
						num |= 0x20;
					}
				}
			}
			if (blockStyle5.left && blockStyle3.bottom)
			{
				Tile tileSafely7 = GetTileSafely(i - 1, j + 1);
				if (tileSafely7.active() && tileSafely7.type == type)
				{
					BlockStyle blockStyle8 = FindBlockStyle(tileSafely7);
					if (blockStyle8.right && blockStyle8.top)
					{
						num |= 0x40;
					}
				}
			}
			if (blockStyle5.right && blockStyle4.bottom)
			{
				Tile tileSafely8 = GetTileSafely(i + 1, j + 1);
				if (tileSafely8.active() && tileSafely8.type == type)
				{
					BlockStyle blockStyle9 = FindBlockStyle(tileSafely8);
					if (blockStyle9.left && blockStyle9.top)
					{
						num |= 0x80;
					}
				}
			}
			if (resetFrame)
			{
				centerTile.frameNumber((byte)WorldGen.genRand.Next(0, 3));
			}
			Point16 point = selfFrame8WayLookup[num][centerTile.frameNumber()];
			centerTile.frameX = point.X;
			centerTile.frameY = point.Y;
		}

		public static void WallFrame(int i, int j, bool resetFrame = false)
		{
			if (i <= 0 || j <= 0 || i >= Main.maxTilesX - 1 || j >= Main.maxTilesY - 1 || Main.tile[i, j] == null)
			{
				return;
			}
			WorldGen.UpdateMapTile(i, j);
			Tile tile = Main.tile[i, j];
			if (tile.wall == 0)
			{
				tile.wallColor(0);
				return;
			}
			int num = 0;
			Tile tile2 = Main.tile[i, j - 1];
			if (tile2 != null && (tile2.wall > 0 || (tile2.active() && tile2.type == 54)))
			{
				num = 1;
			}
			tile2 = Main.tile[i - 1, j];
			if (tile2 != null && (tile2.wall > 0 || (tile2.active() && tile2.type == 54)))
			{
				num |= 2;
			}
			tile2 = Main.tile[i + 1, j];
			if (tile2 != null && (tile2.wall > 0 || (tile2.active() && tile2.type == 54)))
			{
				num |= 4;
			}
			tile2 = Main.tile[i, j + 1];
			if (tile2 != null && (tile2.wall > 0 || (tile2.active() && tile2.type == 54)))
			{
				num |= 8;
			}
			int num2 = 0;
			if (Main.wallLargeFrames[tile.wall] == 1)
			{
				num2 = phlebasTileFrameNumberLookup[j % 4][i % 3] - 1;
				tile.wallFrameNumber((byte)num2);
			}
			else if (Main.wallLargeFrames[tile.wall] == 2)
			{
				num2 = lazureTileFrameNumberLookup[i % 2][j % 2] - 1;
				tile.wallFrameNumber((byte)num2);
			}
			else if (resetFrame)
			{
				num2 = WorldGen.genRand.Next(0, 3);
				tile.wallFrameNumber((byte)num2);
			}
			else
			{
				num2 = tile.wallFrameNumber();
			}
			if (num == 15)
			{
				num += centerWallFrameLookup[i % 3][j % 3];
			}
			Point16 point = wallFrameLookup[num][num2];
			tile.wallFrameX(point.X);
			tile.wallFrameY(point.Y);
		}

		public static Tile GetTileSafely(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			if (tile == null)
			{
				tile = new Tile();
				Main.tile[i, j] = tile;
			}
			return tile;
		}
	}
}
