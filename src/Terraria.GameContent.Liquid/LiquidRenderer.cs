using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics;

namespace Terraria.GameContent.Liquid
{
	internal class LiquidRenderer
	{
		private struct LiquidCache
		{
			public float LiquidLevel;

			public float VisibleLiquidLevel;

			public float Opacity;

			public bool IsSolid;

			public bool IsHalfBrick;

			public bool HasLiquid;

			public bool HasVisibleLiquid;

			public bool HasWall;

			public Point FrameOffset;

			public bool HasLeftEdge;

			public bool HasRightEdge;

			public bool HasTopEdge;

			public bool HasBottomEdge;

			public float LeftWall;

			public float RightWall;

			public float BottomWall;

			public float TopWall;

			public float VisibleLeftWall;

			public float VisibleRightWall;

			public float VisibleBottomWall;

			public float VisibleTopWall;

			public byte Type;

			public byte VisibleType;
		}

		private struct LiquidDrawCache
		{
			public Rectangle SourceRectangle;

			public Vector2 LiquidOffset;

			public bool IsVisible;

			public float Opacity;

			public byte Type;

			public bool IsSurfaceLiquid;

			public bool HasWall;
		}

		private const int ANIMATION_FRAME_COUNT = 16;

		private const int CACHE_PADDING = 2;

		private const int CACHE_PADDING_2 = 4;

		public const float MIN_LIQUID_SIZE = 0.25f;

		private static int[] WATERFALL_LENGTH = new int[3]
		{
			10,
			3,
			2
		};

		private static readonly float[] DEFAULT_OPACITY = new float[3]
		{
			0.6f,
			0.95f,
			0.95f
		};

		private static readonly Tile EMPTY_TILE = new Tile();

		public static LiquidRenderer Instance = new LiquidRenderer();

		private Tile[,] _tiles = Main.tile;

		private Texture2D[] _liquidTextures = new Texture2D[12];

		private LiquidCache[] _cache = new LiquidCache[41616];

		private LiquidDrawCache[] _drawCache = new LiquidDrawCache[40000];

		private int _animationFrame;

		private Rectangle _drawArea;

		private Random _random = new Random();

		public LiquidRenderer()
		{
			for (int i = 0; i < _liquidTextures.Length; i++)
			{
				_liquidTextures[i] = TextureManager.Load("Images/Misc/water_" + i);
			}
		}

		private unsafe void InternalUpdate(Rectangle drawArea)
		{
			Rectangle rectangle = new Rectangle(drawArea.X - 2, drawArea.Y - 2, drawArea.Width + 4, drawArea.Height + 4);
			_drawArea = drawArea;
			Tile tile = null;
			fixed (LiquidCache* ptr = &_cache[0])
			{
				LiquidCache* ptr2 = ptr;
				int num = rectangle.Height * 2 + 2;
				ptr2 = ptr;
				for (int i = rectangle.X; i < rectangle.X + rectangle.Width; i++)
				{
					for (int j = rectangle.Y; j < rectangle.Y + rectangle.Height; j++)
					{
						tile = _tiles[i, j];
						if (tile == null)
						{
							tile = EMPTY_TILE;
						}
						ptr2->LiquidLevel = (float)(int)tile.liquid / 255f;
						ptr2->IsHalfBrick = (tile.halfBrick() && ptr2[-1].HasLiquid);
						ptr2->IsSolid = (WorldGen.SolidOrSlopedTile(tile) && !ptr2->IsHalfBrick);
						ptr2->HasLiquid = (tile.liquid != 0);
						ptr2->VisibleLiquidLevel = 0f;
						ptr2->HasWall = (tile.wall != 0);
						ptr2->Type = tile.liquidType();
						if (ptr2->IsHalfBrick && !ptr2->HasLiquid)
						{
							ptr2->Type = ptr2[-1].Type;
						}
						ptr2++;
					}
				}
				ptr2 = ptr;
				float num2 = 0f;
				ptr2 += num;
				for (int k = 2; k < rectangle.Width - 2; k++)
				{
					for (int l = 2; l < rectangle.Height - 2; l++)
					{
						num2 = 0f;
						if (ptr2->IsHalfBrick && ptr2[-1].HasLiquid)
						{
							num2 = 1f;
						}
						else if (!ptr2->HasLiquid)
						{
							LiquidCache liquidCache = ptr2[-rectangle.Height];
							LiquidCache liquidCache2 = ptr2[rectangle.Height];
							LiquidCache liquidCache3 = ptr2[-1];
							LiquidCache liquidCache4 = ptr2[1];
							if (liquidCache.HasLiquid && liquidCache2.HasLiquid && liquidCache.Type == liquidCache2.Type)
							{
								num2 = liquidCache.LiquidLevel + liquidCache2.LiquidLevel;
								ptr2->Type = liquidCache.Type;
							}
							if (liquidCache3.HasLiquid && liquidCache4.HasLiquid && liquidCache3.Type == liquidCache4.Type)
							{
								num2 = Math.Max(num2, liquidCache3.LiquidLevel + liquidCache4.LiquidLevel);
								ptr2->Type = liquidCache3.Type;
							}
							num2 *= 0.5f;
						}
						else
						{
							num2 = ptr2->LiquidLevel;
						}
						ptr2->VisibleLiquidLevel = num2;
						ptr2->HasVisibleLiquid = (num2 != 0f);
						ptr2++;
					}
					ptr2 += 4;
				}
				ptr2 = ptr;
				for (int m = 0; m < rectangle.Width; m++)
				{
					for (int n = 0; n < rectangle.Height - 10; n++)
					{
						if (ptr2->HasVisibleLiquid && !ptr2->IsSolid)
						{
							ptr2->Opacity = 1f;
							ptr2->VisibleType = ptr2->Type;
							float num3 = 1f / (float)(WATERFALL_LENGTH[ptr2->Type] + 1);
							float num4 = 1f;
							for (int num5 = 1; num5 <= WATERFALL_LENGTH[ptr2->Type]; num5++)
							{
								num4 -= num3;
								if (ptr2[num5].IsSolid)
								{
									break;
								}
								ptr2[num5].VisibleLiquidLevel = Math.Max(ptr2[num5].VisibleLiquidLevel, ptr2->VisibleLiquidLevel * num4);
								ptr2[num5].Opacity = num4;
								ptr2[num5].VisibleType = ptr2->Type;
							}
						}
						if (ptr2->IsSolid)
						{
							ptr2->VisibleLiquidLevel = 1f;
							ptr2->HasVisibleLiquid = false;
						}
						else
						{
							ptr2->HasVisibleLiquid = (ptr2->VisibleLiquidLevel != 0f);
						}
						ptr2++;
					}
					ptr2 += 10;
				}
				ptr2 = ptr;
				ptr2 += num;
				for (int num6 = 2; num6 < rectangle.Width - 2; num6++)
				{
					for (int num7 = 2; num7 < rectangle.Height - 2; num7++)
					{
						if (!ptr2->HasVisibleLiquid || ptr2->IsSolid)
						{
							ptr2->HasLeftEdge = false;
							ptr2->HasTopEdge = false;
							ptr2->HasRightEdge = false;
							ptr2->HasBottomEdge = false;
						}
						else
						{
							LiquidCache liquidCache = ptr2[-1];
							LiquidCache liquidCache2 = ptr2[1];
							LiquidCache liquidCache3 = ptr2[-rectangle.Height];
							LiquidCache liquidCache4 = ptr2[rectangle.Height];
							float num8 = 0f;
							float num9 = 1f;
							float num10 = 0f;
							float num11 = 1f;
							float visibleLiquidLevel = ptr2->VisibleLiquidLevel;
							if (!liquidCache.HasVisibleLiquid)
							{
								num10 += liquidCache2.VisibleLiquidLevel * (1f - visibleLiquidLevel);
							}
							if (!liquidCache2.HasVisibleLiquid && !liquidCache2.IsSolid && !liquidCache2.IsHalfBrick)
							{
								num11 -= liquidCache.VisibleLiquidLevel * (1f - visibleLiquidLevel);
							}
							if (!liquidCache3.HasVisibleLiquid && !liquidCache3.IsSolid && !liquidCache3.IsHalfBrick)
							{
								num8 += liquidCache4.VisibleLiquidLevel * (1f - visibleLiquidLevel);
							}
							if (!liquidCache4.HasVisibleLiquid && !liquidCache4.IsSolid && !liquidCache4.IsHalfBrick)
							{
								num9 -= liquidCache3.VisibleLiquidLevel * (1f - visibleLiquidLevel);
							}
							ptr2->LeftWall = num8;
							ptr2->RightWall = num9;
							ptr2->BottomWall = num11;
							ptr2->TopWall = num10;
							Point zero = Point.Zero;
							ptr2->HasTopEdge = ((!liquidCache.HasVisibleLiquid && !liquidCache.IsSolid) || num10 != 0f);
							ptr2->HasBottomEdge = ((!liquidCache2.HasVisibleLiquid && !liquidCache2.IsSolid) || num11 != 1f);
							ptr2->HasLeftEdge = ((!liquidCache3.HasVisibleLiquid && !liquidCache3.IsSolid) || num8 != 0f);
							ptr2->HasRightEdge = ((!liquidCache4.HasVisibleLiquid && !liquidCache4.IsSolid) || num9 != 1f);
							if (!ptr2->HasLeftEdge)
							{
								if (ptr2->HasRightEdge)
								{
									zero.X += 32;
								}
								else
								{
									zero.X += 16;
								}
							}
							if (ptr2->HasLeftEdge && ptr2->HasRightEdge)
							{
								zero.X = 16;
								zero.Y += 32;
								if (ptr2->HasTopEdge)
								{
									zero.Y = 16;
								}
							}
							else if (!ptr2->HasTopEdge)
							{
								if (!ptr2->HasLeftEdge && !ptr2->HasRightEdge)
								{
									zero.Y += 48;
								}
								else
								{
									zero.Y += 16;
								}
							}
							if (zero.Y == 16 && (ptr2->HasLeftEdge ^ ptr2->HasRightEdge) && num7 + rectangle.Y % 2 == 0)
							{
								zero.Y += 16;
							}
							ptr2->FrameOffset = zero;
						}
						ptr2++;
					}
					ptr2 += 4;
				}
				ptr2 = ptr;
				ptr2 += num;
				for (int num12 = 2; num12 < rectangle.Width - 2; num12++)
				{
					for (int num13 = 2; num13 < rectangle.Height - 2; num13++)
					{
						if (ptr2->HasVisibleLiquid)
						{
							LiquidCache liquidCache = ptr2[-1];
							LiquidCache liquidCache2 = ptr2[1];
							LiquidCache liquidCache3 = ptr2[-rectangle.Height];
							LiquidCache liquidCache4 = ptr2[rectangle.Height];
							ptr2->VisibleLeftWall = ptr2->LeftWall;
							ptr2->VisibleRightWall = ptr2->RightWall;
							ptr2->VisibleTopWall = ptr2->TopWall;
							ptr2->VisibleBottomWall = ptr2->BottomWall;
							if (liquidCache.HasVisibleLiquid && liquidCache2.HasVisibleLiquid)
							{
								if (ptr2->HasLeftEdge)
								{
									ptr2->VisibleLeftWall = (ptr2->LeftWall * 2f + liquidCache.LeftWall + liquidCache2.LeftWall) * 0.25f;
								}
								if (ptr2->HasRightEdge)
								{
									ptr2->VisibleRightWall = (ptr2->RightWall * 2f + liquidCache.RightWall + liquidCache2.RightWall) * 0.25f;
								}
							}
							if (liquidCache3.HasVisibleLiquid && liquidCache4.HasVisibleLiquid)
							{
								if (ptr2->HasTopEdge)
								{
									ptr2->VisibleTopWall = (ptr2->TopWall * 2f + liquidCache3.TopWall + liquidCache4.TopWall) * 0.25f;
								}
								if (ptr2->HasBottomEdge)
								{
									ptr2->VisibleBottomWall = (ptr2->BottomWall * 2f + liquidCache3.BottomWall + liquidCache4.BottomWall) * 0.25f;
								}
							}
						}
						ptr2++;
					}
					ptr2 += 4;
				}
				ptr2 = ptr;
				ptr2 += num;
				for (int num14 = 2; num14 < rectangle.Width - 2; num14++)
				{
					for (int num15 = 2; num15 < rectangle.Height - 2; num15++)
					{
						if (ptr2->HasLiquid)
						{
							LiquidCache liquidCache = ptr2[-1];
							LiquidCache liquidCache2 = ptr2[1];
							LiquidCache liquidCache3 = ptr2[-rectangle.Height];
							LiquidCache liquidCache4 = ptr2[rectangle.Height];
							if (ptr2->HasTopEdge && !ptr2->HasBottomEdge && (ptr2->HasLeftEdge ^ ptr2->HasRightEdge))
							{
								if (ptr2->HasRightEdge)
								{
									ptr2->VisibleRightWall = liquidCache2.VisibleRightWall;
									ptr2->VisibleTopWall = liquidCache3.VisibleTopWall;
								}
								else
								{
									ptr2->VisibleLeftWall = liquidCache2.VisibleLeftWall;
									ptr2->VisibleTopWall = liquidCache4.VisibleTopWall;
								}
							}
							else if (liquidCache2.FrameOffset.X == 16 && liquidCache2.FrameOffset.Y == 32)
							{
								if (ptr2->VisibleLeftWall > 0.5f)
								{
									ptr2->VisibleLeftWall = 0f;
									ptr2->FrameOffset = new Point(0, 0);
								}
								else if (ptr2->VisibleRightWall < 0.5f)
								{
									ptr2->VisibleRightWall = 1f;
									ptr2->FrameOffset = new Point(32, 0);
								}
							}
						}
						ptr2++;
					}
					ptr2 += 4;
				}
				ptr2 = ptr;
				ptr2 += num;
				for (int num16 = 2; num16 < rectangle.Width - 2; num16++)
				{
					for (int num17 = 2; num17 < rectangle.Height - 2; num17++)
					{
						if (ptr2->HasLiquid)
						{
							LiquidCache liquidCache = ptr2[-1];
							LiquidCache liquidCache2 = ptr2[1];
							LiquidCache liquidCache3 = ptr2[-rectangle.Height];
							LiquidCache liquidCache4 = ptr2[rectangle.Height];
							if (!ptr2->HasBottomEdge && !ptr2->HasLeftEdge && !ptr2->HasTopEdge && !ptr2->HasRightEdge)
							{
								if (liquidCache3.HasTopEdge && liquidCache.HasLeftEdge)
								{
									ptr2->FrameOffset.X = Math.Max(4, (int)(16f - liquidCache.VisibleLeftWall * 16f)) - 4;
									ptr2->FrameOffset.Y = 48 + Math.Max(4, (int)(16f - liquidCache3.VisibleTopWall * 16f)) - 4;
									ptr2->VisibleLeftWall = 0f;
									ptr2->VisibleTopWall = 0f;
									ptr2->VisibleRightWall = 1f;
									ptr2->VisibleBottomWall = 1f;
								}
								else if (liquidCache4.HasTopEdge && liquidCache.HasRightEdge)
								{
									ptr2->FrameOffset.X = 32 - Math.Min(16, (int)(liquidCache.VisibleRightWall * 16f) - 4);
									ptr2->FrameOffset.Y = 48 + Math.Max(4, (int)(16f - liquidCache4.VisibleTopWall * 16f)) - 4;
									ptr2->VisibleLeftWall = 0f;
									ptr2->VisibleTopWall = 0f;
									ptr2->VisibleRightWall = 1f;
									ptr2->VisibleBottomWall = 1f;
								}
							}
						}
						ptr2++;
					}
					ptr2 += 4;
				}
				ptr2 = ptr;
				ptr2 += num;
				fixed (LiquidDrawCache* ptr3 = &_drawCache[0])
				{
					LiquidDrawCache* ptr4 = ptr3;
					for (int num18 = 2; num18 < rectangle.Width - 2; num18++)
					{
						for (int num19 = 2; num19 < rectangle.Height - 2; num19++)
						{
							if (ptr2->HasVisibleLiquid)
							{
								float num20 = Math.Min(0.75f, ptr2->VisibleLeftWall);
								float num21 = Math.Max(0.25f, ptr2->VisibleRightWall);
								float num22 = Math.Min(0.75f, ptr2->VisibleTopWall);
								float num23 = Math.Max(0.25f, ptr2->VisibleBottomWall);
								if (ptr2->IsHalfBrick && num23 > 0.5f)
								{
									num23 = 0.5f;
								}
								ptr4->IsVisible = (ptr2->HasWall || !ptr2->IsHalfBrick || !ptr2->HasLiquid);
								ptr4->SourceRectangle = new Rectangle((int)(16f - num21 * 16f) + ptr2->FrameOffset.X, (int)(16f - num23 * 16f) + ptr2->FrameOffset.Y, (int)Math.Ceiling((num21 - num20) * 16f), (int)Math.Ceiling((num23 - num22) * 16f));
								ptr4->IsSurfaceLiquid = (ptr2->FrameOffset.X == 16 && ptr2->FrameOffset.Y == 0 && (double)(num19 + rectangle.Y) > Main.worldSurface - 40.0);
								ptr4->Opacity = ptr2->Opacity;
								ptr4->LiquidOffset = new Vector2((float)Math.Floor(num20 * 16f), (float)Math.Floor(num22 * 16f));
								ptr4->Type = ptr2->VisibleType;
								ptr4->HasWall = ptr2->HasWall;
							}
							else
							{
								ptr4->IsVisible = false;
							}
							ptr2++;
							ptr4++;
						}
						ptr2 += 4;
					}
				}
				ptr2 = ptr;
				for (int num24 = rectangle.X; num24 < rectangle.X + rectangle.Width; num24++)
				{
					for (int num25 = rectangle.Y; num25 < rectangle.Y + rectangle.Height; num25++)
					{
						if (ptr2->VisibleType == 1 && ptr2->HasVisibleLiquid && Dust.lavaBubbles < 200)
						{
							if (_random.Next(700) == 0)
							{
								Dust.NewDust(new Vector2(num24 * 16, num25 * 16), 16, 16, 35, 0f, 0f, 0, Color.White);
							}
							if (_random.Next(350) == 0)
							{
								int num26 = Dust.NewDust(new Vector2(num24 * 16, num25 * 16), 16, 8, 35, 0f, 0f, 50, Color.White, 1.5f);
								Main.dust[num26].velocity *= 0.8f;
								Main.dust[num26].velocity.X *= 2f;
								Main.dust[num26].velocity.Y -= (float)_random.Next(1, 7) * 0.1f;
								if (_random.Next(10) == 0)
								{
									Main.dust[num26].velocity.Y *= _random.Next(2, 5);
								}
								Main.dust[num26].noGravity = true;
							}
						}
						ptr2++;
					}
				}
			}
		}

		public unsafe void InternalDraw(SpriteBatch spriteBatch, Vector2 drawOffset, int waterStyle, float globalAlpha, bool isBackgroundDraw)
		{
			Rectangle drawArea = _drawArea;
			fixed (LiquidDrawCache* ptr = &_drawCache[0])
			{
				LiquidDrawCache* ptr2 = ptr;
				for (int i = drawArea.X; i < drawArea.X + drawArea.Width; i++)
				{
					for (int j = drawArea.Y; j < drawArea.Y + drawArea.Height; j++)
					{
						if (ptr2->IsVisible)
						{
							Rectangle sourceRectangle = ptr2->SourceRectangle;
							if (ptr2->IsSurfaceLiquid)
							{
								sourceRectangle.Y = 1280;
							}
							else
							{
								sourceRectangle.Y += _animationFrame * 80;
							}
							Vector2 liquidOffset = ptr2->LiquidOffset;
							int num = ptr2->Type;
							switch (num)
							{
							case 0:
								num = waterStyle;
								break;
							case 2:
								num = 11;
								break;
							}
							VertexColors vertices;
							Lighting.GetColor4Slice_New(i, j, out vertices);
							float num2 = ptr2->Opacity * Math.Min(1f, isBackgroundDraw ? 1f : (globalAlpha * DEFAULT_OPACITY[ptr2->Type]));
							vertices.BottomLeftColor *= num2;
							vertices.BottomRightColor *= num2;
							vertices.TopLeftColor *= num2;
							vertices.TopRightColor *= num2;
							Main.tileBatch.Draw(_liquidTextures[num], new Vector2(i << 4, j << 4) + drawOffset + liquidOffset, sourceRectangle, vertices, Vector2.Zero, 1f, SpriteEffects.None);
						}
						ptr2++;
					}
				}
			}
		}

		public bool HasFullWater(int x, int y)
		{
			x -= _drawArea.X;
			y -= _drawArea.Y;
			int num = x * _drawArea.Height + y;
			if (num >= 0 && num < _drawCache.Length)
			{
				if (_drawCache[num].IsVisible)
				{
					return !_drawCache[num].IsSurfaceLiquid;
				}
				return false;
			}
			return true;
		}

		public void Update(Rectangle drawArea)
		{
			InternalUpdate(drawArea);
			_animationFrame = (_animationFrame + 1) % 16;
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 drawOffset, int waterStyle, float alpha, bool isBackgroundDraw)
		{
			InternalDraw(spriteBatch, drawOffset, waterStyle, alpha, isBackgroundDraw);
		}
	}
}
