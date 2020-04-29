using System;

namespace Terraria.Enums
{
	[Flags]
	public enum AnchorType
	{
		None = 0x0,
		SolidTile = 0x1,
		SolidWithTop = 0x2,
		Table = 0x4,
		SolidSide = 0x8,
		Tree = 0x10,
		AlternateTile = 0x20,
		EmptyTile = 0x40,
		SolidBottom = 0x80
	}
}
