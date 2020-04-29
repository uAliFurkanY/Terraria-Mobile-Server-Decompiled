using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terraria.DataStructures
{
	public class DrawAnimationVertical : DrawAnimation
	{
		public DrawAnimationVertical(int ticksperframe, int frameCount)
		{
			Frame = 0;
			FrameCounter = 0;
			FrameCount = frameCount;
			TicksPerFrame = ticksperframe;
		}

		public override void Update()
		{
			if (++FrameCounter >= TicksPerFrame)
			{
				FrameCounter = 0;
				if (++Frame >= FrameCount)
				{
					Frame = 0;
				}
			}
		}

		public override Rectangle GetFrame(Texture2D texture)
		{
			return texture.Frame(1, FrameCount, 0, Frame);
		}
	}
}
