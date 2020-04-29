using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace Terraria.Graphics.Shaders
{
	public class HairShaderData : ShaderData
	{
		protected Vector3 _uColor = Vector3.One;

		protected Vector3 _uSecondaryColor = Vector3.One;

		protected float _uSaturation = 1f;

		protected float _uOpacity = 1f;

		protected Ref<Texture2D> _uImage;

		protected bool _shaderDisabled;

		public bool ShaderDisabled => _shaderDisabled;

		public HairShaderData(Effect shader, string passName)
			: base(shader, passName)
		{
		}

		public virtual void Apply(Player player, DrawData? drawData = null)
		{
			if (!_shaderDisabled)
			{
				_shader.Parameters["uColor"].SetValue(_uColor);
				_shader.Parameters["uSaturation"].SetValue(_uSaturation);
				_shader.Parameters["uSecondaryColor"].SetValue(_uSecondaryColor);
				_shader.Parameters["uTime"].SetValue(Main.GlobalTime);
				_shader.Parameters["uOpacity"].SetValue(_uOpacity);
				if (drawData.HasValue)
				{
					DrawData value = drawData.Value;
					Vector4 value2 = new Vector4(value.sourceRect.Value.X, value.sourceRect.Value.Y, value.sourceRect.Value.Width, value.sourceRect.Value.Height);
					_shader.Parameters["uSourceRect"].SetValue(value2);
					_shader.Parameters["uWorldPosition"].SetValue(Main.screenPosition + value.position);
					_shader.Parameters["uImageSize0"].SetValue(new Vector2(value.texture.Width, value.texture.Height));
				}
				else
				{
					_shader.Parameters["uSourceRect"].SetValue(new Vector4(0f, 0f, 4f, 4f));
				}
				if (_uImage != null)
				{
					Main.graphics.GraphicsDevice.Textures[1] = _uImage.Value;
					_shader.Parameters["uImageSize1"].SetValue(new Vector2(_uImage.Value.Width, _uImage.Value.Height));
				}
				if (player != null)
				{
					_shader.Parameters["uDirection"].SetValue((float)player.direction);
				}
				Apply();
			}
		}

		public virtual Color GetColor(Player player, Color lightColor)
		{
			return new Color(lightColor.ToVector4() * player.hairColor.ToVector4());
		}

		public HairShaderData UseColor(float r, float g, float b)
		{
			return UseColor(new Vector3(r, g, b));
		}

		public HairShaderData UseColor(Color color)
		{
			return UseColor(color.ToVector3());
		}

		public HairShaderData UseColor(Vector3 color)
		{
			_uColor = color;
			return this;
		}

		public HairShaderData UseImage(string path)
		{
			_uImage = TextureManager.Retrieve(path);
			return this;
		}

		public HairShaderData UseOpacity(float alpha)
		{
			_uOpacity = alpha;
			return this;
		}

		public HairShaderData UseSecondaryColor(float r, float g, float b)
		{
			return UseSecondaryColor(new Vector3(r, g, b));
		}

		public HairShaderData UseSecondaryColor(Color color)
		{
			return UseSecondaryColor(color.ToVector3());
		}

		public HairShaderData UseSecondaryColor(Vector3 color)
		{
			_uSecondaryColor = color;
			return this;
		}

		public HairShaderData UseSaturation(float saturation)
		{
			_uSaturation = saturation;
			return this;
		}
	}
}
