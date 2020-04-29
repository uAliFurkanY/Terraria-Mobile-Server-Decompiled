namespace Terraria.Graphics.Capture
{
	public class CaptureBiome
	{
		public enum TileColorStyle
		{
			Normal,
			Jungle,
			Crimson,
			Corrupt,
			Mushroom
		}

		public static CaptureBiome[] Biomes = new CaptureBiome[12]
		{
			new CaptureBiome(0, 0, 0),
			null,
			new CaptureBiome(1, 2, 2, TileColorStyle.Corrupt),
			new CaptureBiome(3, 0, 3, TileColorStyle.Jungle),
			new CaptureBiome(6, 2, 4),
			new CaptureBiome(7, 4, 5),
			new CaptureBiome(2, 1, 6),
			new CaptureBiome(9, 6, 7, TileColorStyle.Mushroom),
			new CaptureBiome(0, 0, 8),
			null,
			new CaptureBiome(8, 5, 10, TileColorStyle.Crimson),
			null
		};

		public readonly int WaterStyle;

		public readonly int BackgroundIndex;

		public readonly int BackgroundIndex2;

		public readonly TileColorStyle TileColor;

		public CaptureBiome(int backgroundIndex, int backgroundIndex2, int waterStyle, TileColorStyle tileColorStyle = TileColorStyle.Normal)
		{
			BackgroundIndex = backgroundIndex;
			BackgroundIndex2 = backgroundIndex2;
			WaterStyle = waterStyle;
			TileColor = tileColorStyle;
		}
	}
}
