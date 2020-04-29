using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;
using Terraria.Graphics.Effects;

namespace Terraria.GameContent.Skies
{
	internal class SlimeSky : CustomSky
	{
		private struct Slime
		{
			private const int MAX_FRAMES = 4;

			private const int FRAME_RATE = 6;

			private Texture2D _texture;

			public Vector2 Position;

			public float Depth;

			public int FrameHeight;

			public int FrameWidth;

			public float Speed;

			public bool Active;

			private int _frame;

			public Texture2D Texture
			{
				get
				{
					return _texture;
				}
				set
				{
					_texture = value;
					FrameWidth = value.Width;
					FrameHeight = value.Height / 4;
				}
			}

			public int Frame
			{
				get
				{
					return _frame;
				}
				set
				{
					_frame = value % 24;
				}
			}

			public Rectangle GetSourceRectangle()
			{
				return new Rectangle(0, _frame / 6 * FrameHeight, FrameWidth, FrameHeight);
			}
		}

		private Texture2D[] _textures;

		private Slime[] _slimes;

		private Random _random = new Random();

		private int _slimesRemaining;

		private bool _isActive;

		private bool _isLeaving;

		public override void OnLoad()
		{
			_textures = new Texture2D[4];
			for (int i = 0; i < 4; i++)
			{
				_textures[i] = TextureManager.Load("Images/Misc/Sky_Slime_" + (i + 1));
			}
			GenerateSlimes();
		}

		private void GenerateSlimes()
		{
			_slimes = new Slime[Main.maxTilesY / 6];
			for (int i = 0; i < _slimes.Length; i++)
			{
				int num = (int)((double)Main.screenPosition.Y * 0.7 - (double)Main.screenHeight);
				int minValue = (int)((double)num - Main.worldSurface * 16.0);
				_slimes[i].Position = new Vector2(_random.Next(0, Main.maxTilesX) * 16, _random.Next(minValue, num));
				_slimes[i].Speed = 5f + 3f * (float)_random.NextDouble();
				_slimes[i].Depth = (float)i / (float)_slimes.Length * 1.75f + 1.6f;
				_slimes[i].Texture = _textures[_random.Next(2)];
				if (_random.Next(60) == 0)
				{
					_slimes[i].Texture = _textures[3];
					_slimes[i].Speed = 6f + 3f * (float)_random.NextDouble();
					_slimes[i].Depth += 0.5f;
				}
				else if (_random.Next(30) == 0)
				{
					_slimes[i].Texture = _textures[2];
					_slimes[i].Speed = 6f + 2f * (float)_random.NextDouble();
				}
				_slimes[i].Active = true;
			}
			_slimesRemaining = _slimes.Length;
		}

		public override void Update()
		{
			if (Main.gamePaused || !Main.hasFocus)
			{
				return;
			}
			for (int i = 0; i < _slimes.Length; i++)
			{
				if (!_slimes[i].Active)
				{
					continue;
				}
				_slimes[i].Frame++;
				_slimes[i].Position.Y += _slimes[i].Speed;
				if (!((double)_slimes[i].Position.Y > Main.worldSurface * 16.0))
				{
					continue;
				}
				if (!_isLeaving)
				{
					_slimes[i].Depth = (float)i / (float)_slimes.Length * 1.75f + 1.6f;
					_slimes[i].Position = new Vector2(_random.Next(0, Main.maxTilesX) * 16, -100f);
					_slimes[i].Texture = _textures[_random.Next(2)];
					_slimes[i].Speed = 5f + 3f * (float)_random.NextDouble();
					if (_random.Next(60) == 0)
					{
						_slimes[i].Texture = _textures[3];
						_slimes[i].Speed = 6f + 3f * (float)_random.NextDouble();
						_slimes[i].Depth += 0.5f;
					}
					else if (_random.Next(30) == 0)
					{
						_slimes[i].Texture = _textures[2];
						_slimes[i].Speed = 6f + 2f * (float)_random.NextDouble();
					}
				}
				else
				{
					_slimes[i].Active = false;
					_slimesRemaining--;
				}
			}
			if (_slimesRemaining == 0)
			{
				_isActive = false;
			}
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (Main.screenPosition.Y > 10000f || Main.gameMenu)
			{
				return;
			}
			int num = -1;
			int num2 = 0;
			for (int i = 0; i < _slimes.Length; i++)
			{
				float depth = _slimes[i].Depth;
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
			Vector2 value = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
			Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
			for (int j = num; j < num2; j++)
			{
				if (_slimes[j].Active)
				{
					Color color = new Color(Main.bgColor.ToVector4() * 0.9f + new Vector4(0.1f)) * 0.8f;
					float num3 = 1f;
					if (_slimes[j].Depth > 3f)
					{
						num3 = 0.6f;
					}
					else if ((double)_slimes[j].Depth > 2.5)
					{
						num3 = 0.7f;
					}
					else if (_slimes[j].Depth > 2f)
					{
						num3 = 0.8f;
					}
					else if ((double)_slimes[j].Depth > 1.5)
					{
						num3 = 0.9f;
					}
					num3 *= 0.8f;
					color = new Color((int)((float)(int)color.R * num3), (int)((float)(int)color.G * num3), (int)((float)(int)color.B * num3), (int)((float)(int)color.A * num3));
					Vector2 value2 = new Vector2(1f / _slimes[j].Depth, 0.9f / _slimes[j].Depth);
					Vector2 vector = _slimes[j].Position;
					vector = (vector - value) * value2 + value - Main.screenPosition;
					vector.X = (vector.X + 500f) % 4000f;
					if (vector.X < 0f)
					{
						vector.X += 4000f;
					}
					vector.X -= 500f;
					if (rectangle.Contains((int)vector.X, (int)vector.Y))
					{
						spriteBatch.Draw(_slimes[j].Texture, vector, _slimes[j].GetSourceRectangle(), color, 0f, Vector2.Zero, value2.X * 2f, SpriteEffects.None, 0f);
					}
				}
			}
		}

		internal override void Activate(Vector2 position, params object[] args)
		{
			GenerateSlimes();
			_isActive = true;
			_isLeaving = false;
		}

		internal override void Deactivate(params object[] args)
		{
			_isLeaving = true;
		}

		public override void Reset()
		{
			_isActive = false;
		}

		public override bool IsActive()
		{
			return _isActive;
		}
	}
}
