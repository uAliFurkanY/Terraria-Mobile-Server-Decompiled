using Terraria.DataStructures;
using Terraria.Enums;

namespace Terraria.Modules
{
	public class TileObjectBaseModule
	{
		public int width;

		public int height;

		public Point16 origin;

		public TileObjectDirection direction;

		public int randomRange;

		public bool flattenAnchors;

		public TileObjectBaseModule(TileObjectBaseModule copyFrom = null)
		{
			if (copyFrom == null)
			{
				width = 1;
				height = 1;
				origin = Point16.Zero;
				direction = TileObjectDirection.None;
				randomRange = 0;
				flattenAnchors = false;
			}
			else
			{
				width = copyFrom.width;
				height = copyFrom.height;
				origin = copyFrom.origin;
				direction = copyFrom.direction;
				randomRange = copyFrom.randomRange;
				flattenAnchors = copyFrom.flattenAnchors;
			}
		}
	}
}
