using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Terraria.Graphics.Effects
{
	internal abstract class CustomSky : GameEffect
	{
		public abstract void Update();

		public abstract void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth);

		public abstract bool IsActive();

		public abstract void Reset();

		public virtual Color OnTileColor(Color inColor)
		{
			return inColor;
		}

		public virtual float GetCloudAlpha()
		{
			return 1f;
		}
	}
}
