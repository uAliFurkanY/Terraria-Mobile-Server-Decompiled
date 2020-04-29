using System.IO;
using Terraria.DataStructures;

namespace Terraria.GameContent.Tile_Entities
{
	internal class TEItemFrame : TileEntity
	{
		public Item item;

		public TEItemFrame()
		{
			item = new Item();
		}

		public static int Place(int x, int y)
		{
			TEItemFrame tEItemFrame = new TEItemFrame();
			tEItemFrame.Position = new Point16(x, y);
			tEItemFrame.ID = TileEntity.AssignNewID();
			tEItemFrame.type = 1;
			TileEntity.ByID[tEItemFrame.ID] = tEItemFrame;
			TileEntity.ByPosition[tEItemFrame.Position] = tEItemFrame;
			return tEItemFrame.ID;
		}

		public static int Hook_AfterPlacement(int x, int y, int type = 21, int style = 0, int direction = 1)
		{
			if (Main.netMode == 1)
			{
				NetMessage.SendTileSquare(Main.myPlayer, x, y, 2);
				NetMessage.SendData(87, -1, -1, "", x, y, 1f);
				return -1;
			}
			return Place(x, y);
		}

		public static void Kill(int x, int y)
		{
			TileEntity value;
			if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out value) && value.type == 1)
			{
				TileEntity.ByID.Remove(value.ID);
				TileEntity.ByPosition.Remove(new Point16(x, y));
			}
		}

		public static int Find(int x, int y)
		{
			TileEntity value;
			if (TileEntity.ByPosition.TryGetValue(new Point16(x, y), out value) && value.type == 1)
			{
				return value.ID;
			}
			return -1;
		}

		public static bool ValidTile(int x, int y)
		{
			if (!Main.tile[x, y].active() || Main.tile[x, y].type != 395 || Main.tile[x, y].frameY != 0 || Main.tile[x, y].frameX % 36 != 0)
			{
				return false;
			}
			return true;
		}

		public override void WriteExtraData(BinaryWriter writer)
		{
			writer.Write((short)item.netID);
			writer.Write(item.prefix);
			writer.Write((short)item.stack);
		}

		public override void ReadExtraData(BinaryReader reader)
		{
			item = new Item();
			item.netDefaults(reader.ReadInt16());
			item.Prefix(reader.ReadByte());
			item.stack = reader.ReadInt16();
		}

		public override string ToString()
		{
			return Position.X + "x  " + Position.Y + "y item: " + item.ToString();
		}

		public void DropItem()
		{
			if (Main.netMode != 1)
			{
				Item.NewItem(Position.X * 16, Position.Y * 16, 32, 32, item.netID, 1, false, item.prefix);
			}
			item = new Item();
		}

		public static void TryPlacing(int x, int y, int netid, int prefix, int stack)
		{
			int num = Find(x, y);
			if (num == -1)
			{
				int num2 = Item.NewItem(x * 16, y * 16, 32, 32, 1);
				Main.item[num2].netDefaults(netid);
				Main.item[num2].Prefix(prefix);
				Main.item[num2].stack = stack;
				NetMessage.SendData(21, -1, -1, "", num2);
				return;
			}
			TEItemFrame tEItemFrame = (TEItemFrame)TileEntity.ByID[num];
			if (tEItemFrame.item.stack > 0)
			{
				tEItemFrame.DropItem();
			}
			tEItemFrame.item = new Item();
			tEItemFrame.item.netDefaults(netid);
			tEItemFrame.item.Prefix(prefix);
			tEItemFrame.item.stack = stack;
			NetMessage.SendData(86, -1, -1, "", tEItemFrame.ID, x, y);
		}
	}
}
