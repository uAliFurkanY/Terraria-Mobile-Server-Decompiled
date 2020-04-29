namespace Terraria.Modules
{
	public class TileObjectDrawModule
	{
		public int yOffset;

		public bool flipHorizontal;

		public bool flipVertical;

		public int stepDown;

		public TileObjectDrawModule(TileObjectDrawModule copyFrom = null)
		{
			if (copyFrom == null)
			{
				yOffset = 0;
				flipHorizontal = false;
				flipVertical = false;
				stepDown = 0;
			}
			else
			{
				yOffset = copyFrom.yOffset;
				flipHorizontal = copyFrom.flipHorizontal;
				flipVertical = copyFrom.flipVertical;
				stepDown = copyFrom.stepDown;
			}
		}
	}
}
