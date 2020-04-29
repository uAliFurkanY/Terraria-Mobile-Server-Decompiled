using Microsoft.Xna.Framework;

namespace Terraria
{
	public class Tile
	{
		public const int Type_Solid = 0;

		public const int Type_Halfbrick = 1;

		public const int Type_SlopeDownRight = 2;

		public const int Type_SlopeDownLeft = 3;

		public const int Type_SlopeUpRight = 4;

		public const int Type_SlopeUpLeft = 5;

		public const int Liquid_Water = 0;

		public const int Liquid_Lava = 1;

		public const int Liquid_Honey = 2;

		public ushort type;

		public byte wall;

		public byte liquid;

		public short sTileHeader;

		public byte bTileHeader;

		public byte bTileHeader2;

		public byte bTileHeader3;

		public short frameX;

		public short frameY;

		public int collisionType
		{
			get
			{
				if (!active())
				{
					return 0;
				}
				if (halfBrick())
				{
					return 2;
				}
				if (slope() > 0)
				{
					return 2 + slope();
				}
				if (Main.tileSolid[type] && !Main.tileSolidTop[type])
				{
					return 1;
				}
				return -1;
			}
		}

		public Tile()
		{
			type = 0;
			wall = 0;
			liquid = 0;
			sTileHeader = 0;
			bTileHeader = 0;
			bTileHeader2 = 0;
			bTileHeader3 = 0;
			frameX = 0;
			frameY = 0;
		}

		public Tile(Tile copy)
		{
			if (copy == null)
			{
				type = 0;
				wall = 0;
				liquid = 0;
				sTileHeader = 0;
				bTileHeader = 0;
				bTileHeader2 = 0;
				bTileHeader3 = 0;
				frameX = 0;
				frameY = 0;
			}
			else
			{
				type = copy.type;
				wall = copy.wall;
				liquid = copy.liquid;
				sTileHeader = copy.sTileHeader;
				bTileHeader = copy.bTileHeader;
				bTileHeader2 = copy.bTileHeader2;
				bTileHeader3 = copy.bTileHeader3;
				frameX = copy.frameX;
				frameY = copy.frameY;
			}
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public void ClearEverything()
		{
			type = 0;
			wall = 0;
			liquid = 0;
			sTileHeader = 0;
			bTileHeader = 0;
			bTileHeader2 = 0;
			bTileHeader3 = 0;
			frameX = 0;
			frameY = 0;
		}

		public void ClearTile()
		{
			slope(0);
			halfBrick(false);
			active(false);
		}

		public void CopyFrom(Tile from)
		{
			type = from.type;
			wall = from.wall;
			liquid = from.liquid;
			sTileHeader = from.sTileHeader;
			bTileHeader = from.bTileHeader;
			bTileHeader2 = from.bTileHeader2;
			bTileHeader3 = from.bTileHeader3;
			frameX = from.frameX;
			frameY = from.frameY;
		}

		public bool isTheSameAs(Tile compTile)
		{
			if (compTile == null)
			{
				return false;
			}
			if (sTileHeader != compTile.sTileHeader)
			{
				return false;
			}
			if (active())
			{
				if (type != compTile.type)
				{
					return false;
				}
				if (Main.tileFrameImportant[type] && (frameX != compTile.frameX || frameY != compTile.frameY))
				{
					return false;
				}
			}
			if (wall != compTile.wall || liquid != compTile.liquid)
			{
				return false;
			}
			if (compTile.liquid == 0)
			{
				if (wallColor() != compTile.wallColor())
				{
					return false;
				}
			}
			else if (bTileHeader != compTile.bTileHeader)
			{
				return false;
			}
			return true;
		}

		public int wallFrameX()
		{
			return (bTileHeader2 & 0xF) * 36;
		}

		public void wallFrameX(int wallFrameX)
		{
			bTileHeader2 = (byte)((bTileHeader2 & 0xF0) | ((wallFrameX / 36) & 0xF));
		}

		public int wallFrameY()
		{
			return (bTileHeader3 & 7) * 36;
		}

		public void wallFrameY(int wallFrameY)
		{
			bTileHeader3 = (byte)((bTileHeader3 & 0xF8) | ((wallFrameY / 36) & 7));
		}

		public byte frameNumber()
		{
			return (byte)((bTileHeader2 & 0x30) >> 4);
		}

		public void frameNumber(byte frameNumber)
		{
			bTileHeader2 = (byte)((bTileHeader2 & 0xCF) | ((frameNumber & 3) << 4));
		}

		public byte wallFrameNumber()
		{
			return (byte)((bTileHeader2 & 0xC0) >> 6);
		}

		public void wallFrameNumber(byte wallFrameNumber)
		{
			bTileHeader2 = (byte)((bTileHeader2 & 0x3F) | ((wallFrameNumber & 3) << 6));
		}

		public bool topSlope()
		{
			byte b = slope();
			if (b != 1)
			{
				return b == 2;
			}
			return true;
		}

		public bool bottomSlope()
		{
			byte b = slope();
			if (b != 3)
			{
				return b == 4;
			}
			return true;
		}

		public bool leftSlope()
		{
			byte b = slope();
			if (b != 2)
			{
				return b == 4;
			}
			return true;
		}

		public bool rightSlope()
		{
			byte b = slope();
			if (b != 1)
			{
				return b == 3;
			}
			return true;
		}

		public byte slope()
		{
			return (byte)((sTileHeader & 0x7000) >> 12);
		}

		public bool HasSameSlope(Tile tile)
		{
			return (sTileHeader & 0x7400) == (tile.sTileHeader & 0x7400);
		}

		public void slope(byte slope)
		{
			sTileHeader = (short)((sTileHeader & 0x8FFF) | ((slope & 7) << 12));
		}

		public int blockType()
		{
			if (halfBrick())
			{
				return 1;
			}
			int num = slope();
			if (num > 0)
			{
				num++;
			}
			return num;
		}

		public byte color()
		{
			return (byte)(sTileHeader & 0x1F);
		}

		public void color(byte color)
		{
			if (color > 30)
			{
				color = 30;
			}
			sTileHeader = (short)((sTileHeader & 0xFFE0) | color);
		}

		public byte wallColor()
		{
			return (byte)(bTileHeader & 0x1F);
		}

		public void wallColor(byte wallColor)
		{
			if (wallColor > 30)
			{
				wallColor = 30;
			}
			bTileHeader = (byte)((bTileHeader & 0xE0) | wallColor);
		}

		public bool lava()
		{
			return (bTileHeader & 0x20) == 32;
		}

		public void lava(bool lava)
		{
			if (lava)
			{
				bTileHeader = (byte)((bTileHeader & 0x9F) | 0x20);
			}
			else
			{
				bTileHeader &= 223;
			}
		}

		public bool honey()
		{
			return (bTileHeader & 0x40) == 64;
		}

		public void honey(bool honey)
		{
			if (honey)
			{
				bTileHeader = (byte)((bTileHeader & 0x9F) | 0x40);
			}
			else
			{
				bTileHeader &= 191;
			}
		}

		public void liquidType(int liquidType)
		{
			switch (liquidType)
			{
			case 0:
				bTileHeader &= 159;
				break;
			case 1:
				lava(true);
				break;
			case 2:
				honey(true);
				break;
			}
		}

		public byte liquidType()
		{
			return (byte)((bTileHeader & 0x60) >> 5);
		}

		public bool checkingLiquid()
		{
			return (bTileHeader3 & 8) == 8;
		}

		public void checkingLiquid(bool checkingLiquid)
		{
			if (checkingLiquid)
			{
				bTileHeader3 |= 8;
			}
			else
			{
				bTileHeader3 &= 247;
			}
		}

		public bool skipLiquid()
		{
			return (bTileHeader3 & 0x10) == 16;
		}

		public void skipLiquid(bool skipLiquid)
		{
			if (skipLiquid)
			{
				bTileHeader3 |= 16;
			}
			else
			{
				bTileHeader3 &= 239;
			}
		}

		public bool wire()
		{
			return (sTileHeader & 0x80) == 128;
		}

		public void wire(bool wire)
		{
			if (wire)
			{
				sTileHeader |= 128;
			}
			else
			{
				sTileHeader = (short)(sTileHeader & 0xFF7F);
			}
		}

		public bool wire2()
		{
			return (sTileHeader & 0x100) == 256;
		}

		public void wire2(bool wire2)
		{
			if (wire2)
			{
				sTileHeader |= 256;
			}
			else
			{
				sTileHeader = (short)(sTileHeader & 0xFEFF);
			}
		}

		public bool wire3()
		{
			return (sTileHeader & 0x200) == 512;
		}

		public void wire3(bool wire3)
		{
			if (wire3)
			{
				sTileHeader |= 512;
			}
			else
			{
				sTileHeader = (short)(sTileHeader & 0xFDFF);
			}
		}

		public bool halfBrick()
		{
			return (sTileHeader & 0x400) == 1024;
		}

		public void halfBrick(bool halfBrick)
		{
			if (halfBrick)
			{
				sTileHeader |= 1024;
			}
			else
			{
				sTileHeader = (short)(sTileHeader & 0xFBFF);
			}
		}

		public bool actuator()
		{
			return (sTileHeader & 0x800) == 2048;
		}

		public void actuator(bool actuator)
		{
			if (actuator)
			{
				sTileHeader |= 2048;
			}
			else
			{
				sTileHeader = (short)(sTileHeader & 0xF7FF);
			}
		}

		public bool nactive()
		{
			int num = sTileHeader & 0x60;
			if (num == 32)
			{
				return true;
			}
			return false;
		}

		public bool inActive()
		{
			return (sTileHeader & 0x40) == 64;
		}

		public void inActive(bool inActive)
		{
			if (inActive)
			{
				sTileHeader |= 64;
			}
			else
			{
				sTileHeader = (short)(sTileHeader & 0xFFBF);
			}
		}

		public bool active()
		{
			return (sTileHeader & 0x20) == 32;
		}

		public void active(bool active)
		{
			if (active)
			{
				sTileHeader |= 32;
			}
			else
			{
				sTileHeader = (short)(sTileHeader & 0xFFDF);
			}
		}

		public void ResetToType(ushort type)
		{
			liquid = 0;
			sTileHeader = 32;
			bTileHeader = 0;
			bTileHeader2 = 0;
			bTileHeader3 = 0;
			frameX = 0;
			frameY = 0;
			this.type = type;
		}

		public Color actColor(Color oldColor)
		{
			if (!inActive())
			{
				return oldColor;
			}
			double num = 0.4;
			return new Color((byte)(num * (double)(int)oldColor.R), (byte)(num * (double)(int)oldColor.G), (byte)(num * (double)(int)oldColor.B), oldColor.A);
		}

		public static void SmoothSlope(int x, int y, bool applyToNeighbors = true)
		{
			if (applyToNeighbors)
			{
				SmoothSlope(x + 1, y, false);
				SmoothSlope(x - 1, y, false);
				SmoothSlope(x, y + 1, false);
				SmoothSlope(x, y - 1, false);
			}
			Tile tile = Main.tile[x, y];
			if (!WorldGen.SolidOrSlopedTile(x, y))
			{
				return;
			}
			bool flag = !WorldGen.TileEmpty(x, y - 1);
			bool flag2 = !WorldGen.SolidOrSlopedTile(x, y - 1) && flag;
			bool flag3 = WorldGen.SolidOrSlopedTile(x, y + 1);
			bool flag4 = WorldGen.SolidOrSlopedTile(x - 1, y);
			bool flag5 = WorldGen.SolidOrSlopedTile(x + 1, y);
			switch (((flag ? 1 : 0) << 3) | ((flag3 ? 1 : 0) << 2) | ((flag4 ? 1 : 0) << 1) | (flag5 ? 1 : 0))
			{
			case 10:
				if (!flag2)
				{
					tile.halfBrick(false);
					tile.slope(3);
				}
				break;
			case 9:
				if (!flag2)
				{
					tile.halfBrick(false);
					tile.slope(4);
				}
				break;
			case 6:
				tile.halfBrick(false);
				tile.slope(1);
				break;
			case 5:
				tile.halfBrick(false);
				tile.slope(2);
				break;
			case 4:
				tile.slope(0);
				tile.halfBrick(true);
				break;
			default:
				tile.halfBrick(false);
				tile.slope(0);
				break;
			}
		}

		internal void ClearMetadata()
		{
			liquid = 0;
			sTileHeader = 0;
			bTileHeader = 0;
			bTileHeader2 = 0;
			bTileHeader3 = 0;
			frameX = 0;
			frameY = 0;
		}
	}
}
