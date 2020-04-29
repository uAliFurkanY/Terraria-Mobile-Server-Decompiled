using System;
using Terraria.DataStructures;

namespace Terraria.Modules
{
	public class TileObjectCoordinatesModule
	{
		public int width;

		public int[] heights;

		public int padding;

		public Point16 paddingFix;

		public int styleWidth;

		public int styleHeight;

		public bool calculated;

		public TileObjectCoordinatesModule(TileObjectCoordinatesModule copyFrom = null, int[] drawHeight = null)
		{
			if (copyFrom == null)
			{
				width = 0;
				padding = 0;
				paddingFix = Point16.Zero;
				styleWidth = 0;
				styleHeight = 0;
				calculated = false;
				heights = drawHeight;
				return;
			}
			width = copyFrom.width;
			padding = copyFrom.padding;
			paddingFix = copyFrom.paddingFix;
			styleWidth = copyFrom.styleWidth;
			styleHeight = copyFrom.styleHeight;
			calculated = copyFrom.calculated;
			if (drawHeight == null)
			{
				if (copyFrom.heights == null)
				{
					heights = null;
					return;
				}
				heights = new int[copyFrom.heights.Length];
				Array.Copy(copyFrom.heights, heights, heights.Length);
			}
			else
			{
				heights = drawHeight;
			}
		}
	}
}
