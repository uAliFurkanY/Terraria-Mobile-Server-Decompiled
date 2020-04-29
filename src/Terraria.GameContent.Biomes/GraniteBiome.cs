using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	internal class GraniteBiome : MicroBiome
	{
		private struct Magma
		{
			public readonly float Pressure;

			public readonly float Resistance;

			public readonly bool IsActive;

			private Magma(float pressure, float resistance, bool active)
			{
				Pressure = pressure;
				Resistance = resistance;
				IsActive = active;
			}

			public Magma ToFlow()
			{
				return new Magma(Pressure, Resistance, true);
			}

			public static Magma CreateFlow(float pressure, float resistance = 0f)
			{
				return new Magma(pressure, resistance, true);
			}

			public static Magma CreateEmpty(float resistance = 0f)
			{
				return new Magma(0f, resistance, false);
			}
		}

		private const int MAX_MAGMA_ITERATIONS = 300;

		private static Magma[,] _sourceMagmaMap = new Magma[200, 200];

		private static Magma[,] _targetMagmaMap = new Magma[200, 200];

		public override bool Place(Point origin, StructureMap structures)
		{
			if (GenBase._tiles[origin.X, origin.Y].active())
			{
				return false;
			}
			int length = _sourceMagmaMap.GetLength(0);
			int length2 = _sourceMagmaMap.GetLength(1);
			int num = length / 2;
			int num2 = length2 / 2;
			origin.X -= num;
			origin.Y -= num2;
			for (int i = 0; i < length; i++)
			{
				for (int j = 0; j < length2; j++)
				{
					int i2 = i + origin.X;
					int j2 = j + origin.Y;
					_sourceMagmaMap[i, j] = Magma.CreateEmpty(WorldGen.SolidTile(i2, j2) ? 4f : 1f);
					_targetMagmaMap[i, j] = _sourceMagmaMap[i, j];
				}
			}
			int num3 = num;
			int num4 = num;
			int num5 = num2;
			int num6 = num2;
			for (int k = 0; k < 300; k++)
			{
				for (int l = num3; l <= num4; l++)
				{
					for (int m = num5; m <= num6; m++)
					{
						Magma magma = _sourceMagmaMap[l, m];
						if (!magma.IsActive)
						{
							continue;
						}
						float num7 = 0f;
						Vector2 zero = Vector2.Zero;
						for (int n = -1; n <= 1; n++)
						{
							for (int num8 = -1; num8 <= 1; num8++)
							{
								if (n == 0 && num8 == 0)
								{
									continue;
								}
								Vector2 value = new Vector2(n, num8);
								value.Normalize();
								Magma magma2 = _sourceMagmaMap[l + n, m + num8];
								if (magma.Pressure > 0.01f && !magma2.IsActive)
								{
									if (n == -1)
									{
										num3 = Utils.Clamp(l + n, 1, num3);
									}
									else
									{
										num4 = Utils.Clamp(l + n, num4, length - 2);
									}
									if (num8 == -1)
									{
										num5 = Utils.Clamp(m + num8, 1, num5);
									}
									else
									{
										num6 = Utils.Clamp(m + num8, num6, length2 - 2);
									}
									_targetMagmaMap[l + n, m + num8] = magma2.ToFlow();
								}
								float pressure = magma2.Pressure;
								num7 += pressure;
								zero += pressure * value;
							}
						}
						num7 /= 8f;
						if (num7 > magma.Resistance)
						{
							float num9 = zero.Length() / 8f;
							float val = Math.Max(num7 - num9 - magma.Pressure, 0f) + num9 + magma.Pressure * 0.875f - magma.Resistance;
							val = Math.Max(0f, val);
							_targetMagmaMap[l, m] = Magma.CreateFlow(val, Math.Max(0f, magma.Resistance - val * 0.02f));
						}
					}
				}
				if (k < 2)
				{
					_targetMagmaMap[num, num2] = Magma.CreateFlow(25f);
				}
				Utils.Swap(ref _sourceMagmaMap, ref _targetMagmaMap);
			}
			bool flag = origin.Y + num2 > WorldGen.lavaLine - 30;
			bool flag2 = false;
			for (int num10 = -50; num10 < 50; num10++)
			{
				if (flag2)
				{
					break;
				}
				for (int num11 = -50; num11 < 50; num11++)
				{
					if (flag2)
					{
						break;
					}
					if (GenBase._tiles[origin.X + num + num10, origin.Y + num2 + num11].active())
					{
						switch (GenBase._tiles[origin.X + num + num10, origin.Y + num2 + num11].type)
						{
						case 147:
						case 161:
						case 162:
						case 163:
						case 200:
							flag = false;
							flag2 = true;
							break;
						}
					}
				}
			}
			for (int num12 = num3; num12 <= num4; num12++)
			{
				for (int num13 = num5; num13 <= num6; num13++)
				{
					Magma magma3 = _sourceMagmaMap[num12, num13];
					if (!magma3.IsActive)
					{
						continue;
					}
					Tile tile = GenBase._tiles[origin.X + num12, origin.Y + num13];
					float num14 = (float)Math.Sin((float)(origin.Y + num13) * 0.4f) * 0.7f + 1.2f;
					float num15 = 0.2f + 0.5f / (float)Math.Sqrt(Math.Max(0f, magma3.Pressure - magma3.Resistance));
					float val2 = 1f - Math.Max(0f, num14 * num15);
					val2 = Math.Max(val2, magma3.Pressure / 15f);
					if (val2 > 0.35f + (WorldGen.SolidTile(origin.X + num12, origin.Y + num13) ? 0f : 0.5f))
					{
						if (TileID.Sets.Ore[tile.type])
						{
							tile.ResetToType(tile.type);
						}
						else
						{
							tile.ResetToType(368);
						}
						tile.wall = 180;
					}
					else if (magma3.Resistance < 0.01f)
					{
						WorldUtils.ClearTile(origin.X + num12, origin.Y + num13);
						tile.wall = 180;
					}
					if (tile.liquid > 0 && flag)
					{
						tile.liquidType(1);
					}
				}
			}
			List<Point16> list = new List<Point16>();
			for (int num16 = num3; num16 <= num4; num16++)
			{
				for (int num17 = num5; num17 <= num6; num17++)
				{
					Magma magma4 = _sourceMagmaMap[num16, num17];
					if (!magma4.IsActive)
					{
						continue;
					}
					int num18 = 0;
					int num19 = num16 + origin.X;
					int num20 = num17 + origin.Y;
					if (!WorldGen.SolidTile(num19, num20))
					{
						continue;
					}
					for (int num21 = -1; num21 <= 1; num21++)
					{
						for (int num22 = -1; num22 <= 1; num22++)
						{
							if (WorldGen.SolidTile(num19 + num21, num20 + num22))
							{
								num18++;
							}
						}
					}
					if (num18 < 3)
					{
						list.Add(new Point16(num19, num20));
					}
				}
			}
			foreach (Point16 item in list)
			{
				int x = item.X;
				int y = item.Y;
				WorldUtils.ClearTile(x, y, true);
				GenBase._tiles[x, y].wall = 180;
			}
			list.Clear();
			for (int num23 = num3; num23 <= num4; num23++)
			{
				for (int num24 = num5; num24 <= num6; num24++)
				{
					Magma magma5 = _sourceMagmaMap[num23, num24];
					int num25 = num23 + origin.X;
					int num26 = num24 + origin.Y;
					if (!magma5.IsActive)
					{
						continue;
					}
					WorldUtils.TileFrame(num25, num26);
					WorldGen.SquareWallFrame(num25, num26);
					if (GenBase._random.Next(8) == 0 && GenBase._tiles[num25, num26].active())
					{
						if (!GenBase._tiles[num25, num26 + 1].active())
						{
							WorldGen.PlaceTight(num25, num26 + 1, 165);
						}
						if (!GenBase._tiles[num25, num26 - 1].active())
						{
							WorldGen.PlaceTight(num25, num26 - 1, 165);
						}
					}
					if (GenBase._random.Next(2) == 0)
					{
						Tile.SmoothSlope(num25, num26);
					}
				}
			}
			return true;
		}
	}
}
