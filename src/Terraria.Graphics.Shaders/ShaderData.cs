using Microsoft.Xna.Framework.Graphics;

namespace Terraria.Graphics.Shaders
{
	public class ShaderData
	{
		protected Effect _shader;

		protected string _passName;

		protected EffectPass _effectPass;

		public ShaderData(Effect shader, string passName)
		{
			_passName = passName;
			_shader = shader;
			if (shader != null && passName != null)
			{
				_effectPass = shader.CurrentTechnique.Passes[passName];
			}
		}

		public void SwapProgram(string passName)
		{
			_passName = passName;
			if (passName != null)
			{
				_effectPass = _shader.CurrentTechnique.Passes[passName];
			}
		}

		public virtual void Apply()
		{
			_effectPass.Apply();
		}
	}
}
