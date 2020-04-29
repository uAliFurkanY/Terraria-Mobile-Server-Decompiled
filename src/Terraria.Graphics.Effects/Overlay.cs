using Microsoft.Xna.Framework.Graphics;

namespace Terraria.Graphics.Effects
{
	internal abstract class Overlay : GameEffect
	{
		public OverlayMode Mode = OverlayMode.Inactive;

		public Overlay(EffectPriority priority)
		{
			_priority = priority;
		}

		public abstract void Draw(SpriteBatch spriteBatch);
	}
}
