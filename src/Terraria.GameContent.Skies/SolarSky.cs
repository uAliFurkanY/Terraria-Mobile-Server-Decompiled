using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;
using Terraria.Graphics.Effects;

namespace Terraria.GameContent.Skies
{
	internal class SolarSky : CustomSky
	{
		private struct Meteor
		{
			public Vector2 Position;

			public float Depth;

			public int FrameCounter;

			public float Scale;

			public float StartX;
		}

		private Random _random = new Random();

		private Texture2D _planetTexture;

		private Texture2D _bgTexture;

		private Texture2D _meteorTexture;

		private bool _isActive;

		private Meteor[] _meteors;

		private float _fadeOpacity;

		public override void OnLoad()
		{
			_planetTexture = TextureManager.Load("Images/Misc/SolarSky/Planet");
			_bgTexture = TextureManager.Load("Images/Misc/SolarSky/Background");
			_meteorTexture = TextureManager.Load("Images/Misc/SolarSky/Meteor");
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
			float num = 20f;
			for (int i = 0; i < _meteors.Length; i++)
			{
				_meteors[i].Position.X -= num;
				_meteors[i].Position.Y += num;
				if ((double)_meteors[i].Position.Y > Main.worldSurface * 16.0)
				{
					_meteors[i].Position.X = _meteors[i].StartX;
					_meteors[i].Position.Y = -10000f;
				}
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
			for (int i = 0; i < _meteors.Length; i++)
			{
				float depth = _meteors[i].Depth;
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
				Vector2 value4 = new Vector2(1f / _meteors[j].Depth, 0.9f / _meteors[j].Depth);
				Vector2 position = (_meteors[j].Position - value3) * value4 + value3 - Main.screenPosition;
				int num3 = _meteors[j].FrameCounter / 3;
				_meteors[j].FrameCounter = (_meteors[j].FrameCounter + 1) % 12;
				if (rectangle.Contains((int)position.X, (int)position.Y))
				{
					spriteBatch.Draw(_meteorTexture, position, new Rectangle(0, num3 * (_meteorTexture.Height / 4), _meteorTexture.Width, _meteorTexture.Height / 4), Color.White * scale * _fadeOpacity, 0f, Vector2.Zero, value4.X * 5f * _meteors[j].Scale, SpriteEffects.None, 0f);
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
			_meteors = new Meteor[150];
			for (int i = 0; i < _meteors.Length; i++)
			{
				float num = (float)i / (float)_meteors.Length;
				_meteors[i].Position.X = num * ((float)Main.maxTilesX * 16f) + _random.NextFloat() * 40f - 20f;
				_meteors[i].Position.Y = _random.NextFloat() * (0f - ((float)Main.worldSurface * 16f + 10000f)) - 10000f;
				if (_random.Next(3) != 0)
				{
					_meteors[i].Depth = _random.NextFloat() * 3f + 1.8f;
				}
				else
				{
					_meteors[i].Depth = _random.NextFloat() * 5f + 4.8f;
				}
				_meteors[i].FrameCounter = _random.Next(12);
				_meteors[i].Scale = _random.NextFloat() * 0.5f + 1f;
				_meteors[i].StartX = _meteors[i].Position.X;
			}
			Array.Sort(_meteors, SortMethod);
		}

		private int SortMethod(Meteor meteor1, Meteor meteor2)
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
