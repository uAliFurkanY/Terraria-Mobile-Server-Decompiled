using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;
using Terraria.Graphics.Effects;

namespace Terraria.GameContent.Skies
{
	internal class StardustSky : CustomSky
	{
		private struct Star
		{
			public Vector2 Position;

			public float Depth;

			public int TextureIndex;

			public float SinOffset;

			public float AlphaFrequency;

			public float AlphaAmplitude;
		}

		private Random _random = new Random();

		private Texture2D _planetTexture;

		private Texture2D _bgTexture;

		private Texture2D[] _starTextures;

		private bool _isActive;

		private Star[] _stars;

		private float _fadeOpacity;

		public override void OnLoad()
		{
			_planetTexture = TextureManager.Load("Images/Misc/StarDustSky/Planet");
			_bgTexture = TextureManager.Load("Images/Misc/StarDustSky/Background");
			_starTextures = new Texture2D[2];
			for (int i = 0; i < _starTextures.Length; i++)
			{
				_starTextures[i] = TextureManager.Load("Images/Misc/StarDustSky/Star " + i);
			}
		}

		public override void Update()
		{
			if (_isActive)
			{
				_fadeOpacity = Math.Min(1f, 0.01f + _fadeOpacity);
			}
			else
			{
				_fadeOpacity = Math.Max(0f, _fadeOpacity - 0.01f);
			}
		}

		public override Color OnTileColor(Color inColor)
		{
			Vector4 value = inColor.ToVector4();
			return new Color(Vector4.Lerp(value, Vector4.One, _fadeOpacity * 0.5f));
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
			{
				spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * _fadeOpacity);
				spriteBatch.Draw(_bgTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - (double)Main.screenPosition.Y - 2400.0) * 0.10000000149011612)), Main.screenWidth, Main.screenHeight), Color.White * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * _fadeOpacity));
				Vector2 value = new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
				Vector2 value2 = 0.01f * (new Vector2((float)Main.maxTilesX * 8f, (float)Main.worldSurface / 2f) - Main.screenPosition);
				spriteBatch.Draw(_planetTexture, value + new Vector2(-200f, -200f) + value2, null, Color.White * 0.9f * _fadeOpacity, 0f, new Vector2(_planetTexture.Width >> 1, _planetTexture.Height >> 1), 1f, SpriteEffects.None, 1f);
			}
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < _stars.Length; i++)
			{
				float depth = _stars[i].Depth;
				if (num == -1 && depth < maxDepth)
				{
					num = i;
				}
				if (depth <= minDepth)
				{
					break;
				}
				num2 = i;
			}
			if (num == -1)
			{
				return;
			}
			float scale = Math.Min(1f, (Main.screenPosition.Y - 1000f) / 1000f);
			Vector2 value3 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
			Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
			for (int j = num; j < num2; j++)
			{
				Vector2 value4 = new Vector2(1f / _stars[j].Depth, 1.1f / _stars[j].Depth);
				Vector2 position = (_stars[j].Position - value3) * value4 + value3 - Main.screenPosition;
				if (rectangle.Contains((int)position.X, (int)position.Y))
				{
					float value5 = (float)Math.Sin(_stars[j].AlphaFrequency * Main.GlobalTime + _stars[j].SinOffset) * _stars[j].AlphaAmplitude + _stars[j].AlphaAmplitude;
					float num3 = (float)Math.Sin(_stars[j].AlphaFrequency * Main.GlobalTime * 5f + _stars[j].SinOffset) * 0.1f - 0.1f;
					value5 = MathHelper.Clamp(value5, 0f, 1f);
					Texture2D texture2D = _starTextures[_stars[j].TextureIndex];
					spriteBatch.Draw(texture2D, position, null, Color.White * scale * value5 * 0.8f * (1f - num3) * _fadeOpacity, 0f, new Vector2(texture2D.Width >> 1, texture2D.Height >> 1), (value4.X * 0.5f + 0.5f) * (value5 * 0.3f + 0.7f), SpriteEffects.None, 0f);
				}
			}
		}

		public override float GetCloudAlpha()
		{
			return (1f - _fadeOpacity) * 0.3f + 0.7f;
		}

		internal override void Activate(Vector2 position, params object[] args)
		{
			_fadeOpacity = 0.002f;
			_isActive = true;
			int num = 200;
			int num2 = 10;
			_stars = new Star[num * num2];
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				float num4 = (float)i / (float)num;
				for (int j = 0; j < num2; j++)
				{
					float num5 = (float)j / (float)num2;
					_stars[num3].Position.X = num4 * (float)Main.maxTilesX * 16f;
					_stars[num3].Position.Y = num5 * ((float)Main.worldSurface * 16f + 2000f) - 1000f;
					_stars[num3].Depth = _random.NextFloat() * 8f + 1.5f;
					_stars[num3].TextureIndex = _random.Next(_starTextures.Length);
					_stars[num3].SinOffset = _random.NextFloat() * 6.28f;
					_stars[num3].AlphaAmplitude = _random.NextFloat() * 5f;
					_stars[num3].AlphaFrequency = _random.NextFloat() + 1f;
					num3++;
				}
			}
			Array.Sort(_stars, SortMethod);
		}

		private int SortMethod(Star meteor1, Star meteor2)
		{
			return meteor2.Depth.CompareTo(meteor1.Depth);
		}

		internal override void Deactivate(params object[] args)
		{
			_isActive = false;
		}

		public override void Reset()
		{
			_isActive = false;
		}

		public override bool IsActive()
		{
			if (!_isActive)
			{
				return _fadeOpacity > 0.001f;
			}
			return true;
		}
	}
}
