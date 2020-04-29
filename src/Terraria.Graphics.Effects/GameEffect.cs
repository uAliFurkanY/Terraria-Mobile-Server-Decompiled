using Microsoft.Xna.Framework;

namespace Terraria.Graphics.Effects
{
	internal abstract class GameEffect
	{
		public float Opacity;

		protected bool _isLoaded;

		protected EffectPriority _priority;

		public bool IsLoaded => _isLoaded;

		public EffectPriority Priority => _priority;

		public void Load()
		{
			if (!_isLoaded)
			{
				_isLoaded = true;
				OnLoad();
			}
		}

		public virtual void OnLoad()
		{
		}

		internal abstract void Activate(Vector2 position, params object[] args);

		internal abstract void Deactivate(params object[] args);
	}
}
