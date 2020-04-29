using System.Collections.Generic;
using System.IO;
using Terraria.Net;

namespace Terraria.GameContent.NetModules
{
	internal class NetLiquidModule : NetModule
	{
		public static NetPacket Serialize(HashSet<int> changes)
		{
			NetPacket result = NetModule.CreatePacket<NetLiquidModule>(changes.Count * 6 + 2);
			result.Writer.Write((ushort)changes.Count);
			foreach (int change in changes)
			{
				int num = (change >> 16) & 0xFFFF;
				int num2 = change & 0xFFFF;
				result.Writer.Write(change);
				result.Writer.Write(Main.tile[num, num2].liquid);
				result.Writer.Write(Main.tile[num, num2].liquidType());
			}
			return result;
		}

		public override bool Deserialize(BinaryReader reader, int userId)
		{
			int num = reader.ReadUInt16();
			for (int i = 0; i < num; i++)
			{
				int num2 = reader.ReadInt32();
				byte liquid = reader.ReadByte();
				byte liquidType = reader.ReadByte();
				int num3 = (num2 >> 16) & 0xFFFF;
				int num4 = num2 & 0xFFFF;
				Tile tile = Main.tile[num3, num4];
				if (tile != null)
				{
					tile.liquid = liquid;
					tile.liquidType(liquidType);
				}
			}
			return true;
		}
	}
}
