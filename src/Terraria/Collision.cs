using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Terraria
{
	public class Collision
	{
		public static bool stair = false;

		public static bool stairFall = false;

		public static bool honey = false;

		public static bool sloping = false;

		public static bool landMine = false;

		public static bool up = false;

		public static bool down = false;

		public static float Epsilon = (float)Math.E;

		public static Vector2[] CheckLinevLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			if (a1.Equals(a2) && b1.Equals(b2))
			{
				if (a1.Equals(b1))
				{
					return new Vector2[1]
					{
						a1
					};
				}
				return new Vector2[0];
			}
			if (b1.Equals(b2))
			{
				if (PointOnLine(b1, a1, a2))
				{
					return new Vector2[1]
					{
						b1
					};
				}
				return new Vector2[0];
			}
			if (a1.Equals(a2))
			{
				if (PointOnLine(a1, b1, b2))
				{
					return new Vector2[1]
					{
						a1
					};
				}
				return new Vector2[0];
			}
			float num = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
			float num2 = (a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X);
			float num3 = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);
			if (!(0f - Epsilon < num3) || !(num3 < Epsilon))
			{
				float num4 = num / num3;
				float num5 = num2 / num3;
				if (0f <= num4 && num4 <= 1f && 0f <= num5 && num5 <= 1f)
				{
					return new Vector2[1]
					{
						new Vector2(a1.X + num4 * (a2.X - a1.X), a1.Y + num4 * (a2.Y - a1.Y))
					};
				}
				return new Vector2[0];
			}
			if ((0f - Epsilon < num && num < Epsilon) || (0f - Epsilon < num2 && num2 < Epsilon))
			{
				if (a1.Equals(a2))
				{
					return OneDimensionalIntersection(b1, b2, a1, a2);
				}
				return OneDimensionalIntersection(a1, a2, b1, b2);
			}
			return new Vector2[0];
		}

		private static double DistFromSeg(Vector2 p, Vector2 q0, Vector2 q1, double radius, ref float u)
		{
			double num = q1.X - q0.X;
			double num2 = q1.Y - q0.Y;
			double num3 = q0.X - p.X;
			double num4 = q0.Y - p.Y;
			double num5 = Math.Sqrt(num * num + num2 * num2);
			if (num5 < (double)Epsilon)
			{
				throw new Exception("Expected line segment, not point.");
			}
			double num6 = Math.Abs(num * num4 - num3 * num2);
			return num6 / num5;
		}

		private static bool PointOnLine(Vector2 p, Vector2 a1, Vector2 a2)
		{
			float u = 0f;
			double num = DistFromSeg(p, a1, a2, Epsilon, ref u);
			return num < (double)Epsilon;
		}

		private static Vector2[] OneDimensionalIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
		{
			float num = a2.X - a1.X;
			float num2 = a2.Y - a1.Y;
			float relativePoint;
			float relativePoint2;
			if (Math.Abs(num) > Math.Abs(num2))
			{
				relativePoint = (b1.X - a1.X) / num;
				relativePoint2 = (b2.X - a1.X) / num;
			}
			else
			{
				relativePoint = (b1.Y - a1.Y) / num2;
				relativePoint2 = (b2.Y - a1.Y) / num2;
			}
			List<Vector2> list = new List<Vector2>();
			float[] array = FindOverlapPoints(relativePoint, relativePoint2);
			float[] array2 = array;
			foreach (float num3 in array2)
			{
				float x = a2.X * num3 + a1.X * (1f - num3);
				float y = a2.Y * num3 + a1.Y * (1f - num3);
				list.Add(new Vector2(x, y));
			}
			return list.ToArray();
		}

		private static float[] FindOverlapPoints(float relativePoint1, float relativePoint2)
		{
			float val = Math.Min(relativePoint1, relativePoint2);
			float val2 = Math.Max(relativePoint1, relativePoint2);
			float num = Math.Max(0f, val);
			float num2 = Math.Min(1f, val2);
			if (num > num2)
			{
				return new float[0];
			}
			if (num == num2)
			{
				return new float[1]
				{
					num
				};
			}
			return new float[2]
			{
				num,
				num2
			};
		}

		public static bool CheckAABBvAABBCollision(Vector2 position1, Vector2 dimensions1, Vector2 position2, Vector2 dimensions2)
		{
			if (position1.X < position2.X + dimensions2.X && position1.Y < position2.Y + dimensions2.Y && position1.X + dimensions1.X > position2.X)
			{
				return position1.Y + dimensions1.Y > position2.Y;
			}
			return false;
		}

		public static bool CheckAABBvLineCollision(Vector2 objectPosition, Vector2 objectDimensions, Vector2 lineStart, Vector2 lineEnd, float lineWidth, ref float collisionPoint)
		{
			float num = lineWidth * 0.5f;
			Vector2 position = lineStart;
			Vector2 dimensions = lineEnd - lineStart;
			if (dimensions.X > 0f)
			{
				dimensions.X += lineWidth;
				position.X -= num;
			}
			else
			{
				position.X += dimensions.X - num;
				dimensions.X = 0f - dimensions.X + lineWidth;
			}
			if (dimensions.Y > 0f)
			{
				dimensions.Y += lineWidth;
				position.Y -= num;
			}
			else
			{
				position.Y += dimensions.Y - num;
				dimensions.Y = 0f - dimensions.Y + lineWidth;
			}
			if (!CheckAABBvAABBCollision(objectPosition, objectDimensions, position, dimensions))
			{
				return false;
			}
			Vector2 vector = objectPosition - lineStart;
			Vector2 spinningpoint = vector + objectDimensions;
			Vector2 spinningpoint2 = new Vector2(vector.X, spinningpoint.Y);
			Vector2 spinningpoint3 = new Vector2(spinningpoint.X, vector.Y);
			Vector2 vector2 = lineEnd - lineStart;
			float num2 = vector2.Length();
			float num3 = (float)Math.Atan2(vector2.Y, vector2.X);
			Vector2[] array = new Vector2[4]
			{
				vector.RotatedBy(0f - num3),
				spinningpoint3.RotatedBy(0f - num3),
				spinningpoint.RotatedBy(0f - num3),
				spinningpoint2.RotatedBy(0f - num3)
			};
			collisionPoint = num2;
			bool result = false;
			for (int i = 0; i < array.Length; i++)
			{
				if (Math.Abs(array[i].Y) < num && array[i].X < collisionPoint && array[i].X >= 0f)
				{
					collisionPoint = array[i].X;
					result = true;
				}
			}
			Vector2 value = new Vector2(0f, num);
			Vector2 value2 = new Vector2(num2, num);
			Vector2 value3 = new Vector2(0f, 0f - num);
			Vector2 value4 = new Vector2(num2, 0f - num);
			for (int j = 0; j < array.Length; j++)
			{
				int num4 = (j + 1) % array.Length;
				Vector2 vector3 = value2 - value;
				Vector2 vector4 = array[num4] - array[j];
				float num5 = vector3.X * vector4.Y - vector3.Y * vector4.X;
				if (num5 != 0f)
				{
					Vector2 vector5 = array[j] - value;
					float num6 = (vector5.X * vector4.Y - vector5.Y * vector4.X) / num5;
					if (num6 >= 0f && num6 <= 1f)
					{
						float num7 = (vector5.X * vector3.Y - vector5.Y * vector3.X) / num5;
						if (num7 >= 0f && num7 <= 1f)
						{
							result = true;
							collisionPoint = Math.Min(collisionPoint, value.X + num6 * vector3.X);
						}
					}
				}
				vector3 = value4 - value3;
				num5 = vector3.X * vector4.Y - vector3.Y * vector4.X;
				if (num5 == 0f)
				{
					continue;
				}
				Vector2 vector6 = array[j] - value3;
				float num8 = (vector6.X * vector4.Y - vector6.Y * vector4.X) / num5;
				if (num8 >= 0f && num8 <= 1f)
				{
					float num9 = (vector6.X * vector3.Y - vector6.Y * vector3.X) / num5;
					if (num9 >= 0f && num9 <= 1f)
					{
						result = true;
						collisionPoint = Math.Min(collisionPoint, value3.X + num8 * vector3.X);
					}
				}
			}
			return result;
		}

		public static bool CanHit(Vector2 Position1, int Width1, int Height1, Vector2 Position2, int Width2, int Height2)
		{
			int num = (int)((Position1.X + (float)(Width1 / 2)) / 16f);
			int num2 = (int)((Position1.Y + (float)(Height1 / 2)) / 16f);
			int num3 = (int)((Position2.X + (float)(Width2 / 2)) / 16f);
			int num4 = (int)((Position2.Y + (float)(Height2 / 2)) / 16f);
			if (num <= 1)
			{
				num = 1;
			}
			if (num >= Main.maxTilesX)
			{
				num = Main.maxTilesX - 1;
			}
			if (num3 <= 1)
			{
				num3 = 1;
			}
			if (num3 >= Main.maxTilesX)
			{
				num3 = Main.maxTilesX - 1;
			}
			if (num2 <= 1)
			{
				num2 = 1;
			}
			if (num2 >= Main.maxTilesY)
			{
				num2 = Main.maxTilesY - 1;
			}
			if (num4 <= 1)
			{
				num4 = 1;
			}
			if (num4 >= Main.maxTilesY)
			{
				num4 = Main.maxTilesY - 1;
			}
			try
			{
				do
				{
					int num5 = Math.Abs(num - num3);
					int num6 = Math.Abs(num2 - num4);
					if (num == num3 && num2 == num4)
					{
						return true;
					}
					if (num5 > num6)
					{
						num = ((num >= num3) ? (num - 1) : (num + 1));
						if (Main.tile[num, num2 - 1] == null)
						{
							return false;
						}
						if (Main.tile[num, num2 + 1] == null)
						{
							return false;
						}
						if (!Main.tile[num, num2 - 1].inActive() && Main.tile[num, num2 - 1].active() && Main.tileSolid[Main.tile[num, num2 - 1].type] && !Main.tileSolidTop[Main.tile[num, num2 - 1].type] && Main.tile[num, num2 - 1].slope() == 0 && !Main.tile[num, num2 - 1].halfBrick() && !Main.tile[num, num2 + 1].inActive() && Main.tile[num, num2 + 1].active() && Main.tileSolid[Main.tile[num, num2 + 1].type] && !Main.tileSolidTop[Main.tile[num, num2 + 1].type] && Main.tile[num, num2 + 1].slope() == 0 && !Main.tile[num, num2 + 1].halfBrick())
						{
							return false;
						}
					}
					else
					{
						num2 = ((num2 >= num4) ? (num2 - 1) : (num2 + 1));
						if (Main.tile[num - 1, num2] == null)
						{
							return false;
						}
						if (Main.tile[num + 1, num2] == null)
						{
							return false;
						}
						if (!Main.tile[num - 1, num2].inActive() && Main.tile[num - 1, num2].active() && Main.tileSolid[Main.tile[num - 1, num2].type] && !Main.tileSolidTop[Main.tile[num - 1, num2].type] && Main.tile[num - 1, num2].slope() == 0 && !Main.tile[num - 1, num2].halfBrick() && !Main.tile[num + 1, num2].inActive() && Main.tile[num + 1, num2].active() && Main.tileSolid[Main.tile[num + 1, num2].type] && !Main.tileSolidTop[Main.tile[num + 1, num2].type] && Main.tile[num + 1, num2].slope() == 0 && !Main.tile[num + 1, num2].halfBrick())
						{
							return false;
						}
					}
					if (Main.tile[num, num2] == null)
					{
						return false;
					}
				}
				while (Main.tile[num, num2].inActive() || !Main.tile[num, num2].active() || !Main.tileSolid[Main.tile[num, num2].type] || Main.tileSolidTop[Main.tile[num, num2].type]);
				return false;
			}
			catch
			{
				return false;
			}
		}

		public static bool CanHitWithCheck(Vector2 Position1, int Width1, int Height1, Vector2 Position2, int Width2, int Height2, Utils.PerLinePoint check)
		{
			int num = (int)((Position1.X + (float)(Width1 / 2)) / 16f);
			int num2 = (int)((Position1.Y + (float)(Height1 / 2)) / 16f);
			int num3 = (int)((Position2.X + (float)(Width2 / 2)) / 16f);
			int num4 = (int)((Position2.Y + (float)(Height2 / 2)) / 16f);
			if (num <= 1)
			{
				num = 1;
			}
			if (num >= Main.maxTilesX)
			{
				num = Main.maxTilesX - 1;
			}
			if (num3 <= 1)
			{
				num3 = 1;
			}
			if (num3 >= Main.maxTilesX)
			{
				num3 = Main.maxTilesX - 1;
			}
			if (num2 <= 1)
			{
				num2 = 1;
			}
			if (num2 >= Main.maxTilesY)
			{
				num2 = Main.maxTilesY - 1;
			}
			if (num4 <= 1)
			{
				num4 = 1;
			}
			if (num4 >= Main.maxTilesY)
			{
				num4 = Main.maxTilesY - 1;
			}
			try
			{
				do
				{
					int num5 = Math.Abs(num - num3);
					int num6 = Math.Abs(num2 - num4);
					if (num == num3 && num2 == num4)
					{
						return true;
					}
					if (num5 > num6)
					{
						num = ((num >= num3) ? (num - 1) : (num + 1));
						if (Main.tile[num, num2 - 1] == null)
						{
							return false;
						}
						if (Main.tile[num, num2 + 1] == null)
						{
							return false;
						}
						if (!Main.tile[num, num2 - 1].inActive() && Main.tile[num, num2 - 1].active() && Main.tileSolid[Main.tile[num, num2 - 1].type] && !Main.tileSolidTop[Main.tile[num, num2 - 1].type] && Main.tile[num, num2 - 1].slope() == 0 && !Main.tile[num, num2 - 1].halfBrick() && !Main.tile[num, num2 + 1].inActive() && Main.tile[num, num2 + 1].active() && Main.tileSolid[Main.tile[num, num2 + 1].type] && !Main.tileSolidTop[Main.tile[num, num2 + 1].type] && Main.tile[num, num2 + 1].slope() == 0 && !Main.tile[num, num2 + 1].halfBrick())
						{
							return false;
						}
					}
					else
					{
						num2 = ((num2 >= num4) ? (num2 - 1) : (num2 + 1));
						if (Main.tile[num - 1, num2] == null)
						{
							return false;
						}
						if (Main.tile[num + 1, num2] == null)
						{
							return false;
						}
						if (!Main.tile[num - 1, num2].inActive() && Main.tile[num - 1, num2].active() && Main.tileSolid[Main.tile[num - 1, num2].type] && !Main.tileSolidTop[Main.tile[num - 1, num2].type] && Main.tile[num - 1, num2].slope() == 0 && !Main.tile[num - 1, num2].halfBrick() && !Main.tile[num + 1, num2].inActive() && Main.tile[num + 1, num2].active() && Main.tileSolid[Main.tile[num + 1, num2].type] && !Main.tileSolidTop[Main.tile[num + 1, num2].type] && Main.tile[num + 1, num2].slope() == 0 && !Main.tile[num + 1, num2].halfBrick())
						{
							return false;
						}
					}
					if (Main.tile[num, num2] == null)
					{
						return false;
					}
					if (!Main.tile[num, num2].inActive() && Main.tile[num, num2].active() && Main.tileSolid[Main.tile[num, num2].type] && !Main.tileSolidTop[Main.tile[num, num2].type])
					{
						return false;
					}
				}
				while (check(num, num2));
				return false;
			}
			catch
			{
				return false;
			}
		}

		public static bool CanHitLine(Vector2 Position1, int Width1, int Height1, Vector2 Position2, int Width2, int Height2)
		{
			int num = (int)((Position1.X + (float)(Width1 / 2)) / 16f);
			int num2 = (int)((Position1.Y + (float)(Height1 / 2)) / 16f);
			int num3 = (int)((Position2.X + (float)(Width2 / 2)) / 16f);
			int num4 = (int)((Position2.Y + (float)(Height2 / 2)) / 16f);
			if (num <= 1)
			{
				num = 1;
			}
			if (num >= Main.maxTilesX)
			{
				num = Main.maxTilesX - 1;
			}
			if (num3 <= 1)
			{
				num3 = 1;
			}
			if (num3 >= Main.maxTilesX)
			{
				num3 = Main.maxTilesX - 1;
			}
			if (num2 <= 1)
			{
				num2 = 1;
			}
			if (num2 >= Main.maxTilesY)
			{
				num2 = Main.maxTilesY - 1;
			}
			if (num4 <= 1)
			{
				num4 = 1;
			}
			if (num4 >= Main.maxTilesY)
			{
				num4 = Main.maxTilesY - 1;
			}
			float num5 = Math.Abs(num - num3);
			float num6 = Math.Abs(num2 - num4);
			if (num5 == 0f && num6 == 0f)
			{
				return true;
			}
			float num7 = 1f;
			float num8 = 1f;
			if (num5 == 0f || num6 == 0f)
			{
				if (num5 == 0f)
				{
					num7 = 0f;
				}
				if (num6 == 0f)
				{
					num8 = 0f;
				}
			}
			else if (num5 > num6)
			{
				num7 = num5 / num6;
			}
			else
			{
				num8 = num6 / num5;
			}
			float num9 = 0f;
			float num10 = 0f;
			int num11 = 1;
			if (num2 < num4)
			{
				num11 = 2;
			}
			int num12 = (int)num5;
			int num13 = (int)num6;
			int num14 = Math.Sign(num3 - num);
			int num15 = Math.Sign(num4 - num2);
			bool flag = false;
			bool flag2 = false;
			try
			{
				do
				{
					switch (num11)
					{
					case 2:
					{
						num9 += num7;
						int num17 = (int)num9;
						num9 %= 1f;
						for (int j = 0; j < num17; j++)
						{
							if (Main.tile[num, num2 - 1] == null)
							{
								return false;
							}
							if (Main.tile[num, num2] == null)
							{
								return false;
							}
							if (Main.tile[num, num2 + 1] == null)
							{
								return false;
							}
							Tile tile4 = Main.tile[num, num2 - 1];
							Tile tile5 = Main.tile[num, num2 + 1];
							Tile tile6 = Main.tile[num, num2];
							if ((!tile4.inActive() && tile4.active() && Main.tileSolid[tile4.type] && !Main.tileSolidTop[tile4.type]) || (!tile5.inActive() && tile5.active() && Main.tileSolid[tile5.type] && !Main.tileSolidTop[tile5.type]) || (!tile6.inActive() && tile6.active() && Main.tileSolid[tile6.type] && !Main.tileSolidTop[tile6.type]))
							{
								return false;
							}
							if (num12 == 0 && num13 == 0)
							{
								flag = true;
								break;
							}
							num += num14;
							num12--;
							if (num12 == 0 && num13 == 0 && num17 == 1)
							{
								flag2 = true;
							}
						}
						if (num13 != 0)
						{
							num11 = 1;
						}
						break;
					}
					case 1:
					{
						num10 += num8;
						int num16 = (int)num10;
						num10 %= 1f;
						for (int i = 0; i < num16; i++)
						{
							if (Main.tile[num - 1, num2] == null)
							{
								return false;
							}
							if (Main.tile[num, num2] == null)
							{
								return false;
							}
							if (Main.tile[num + 1, num2] == null)
							{
								return false;
							}
							Tile tile = Main.tile[num - 1, num2];
							Tile tile2 = Main.tile[num + 1, num2];
							Tile tile3 = Main.tile[num, num2];
							if ((!tile.inActive() && tile.active() && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type]) || (!tile2.inActive() && tile2.active() && Main.tileSolid[tile2.type] && !Main.tileSolidTop[tile2.type]) || (!tile3.inActive() && tile3.active() && Main.tileSolid[tile3.type] && !Main.tileSolidTop[tile3.type]))
							{
								return false;
							}
							if (num12 == 0 && num13 == 0)
							{
								flag = true;
								break;
							}
							num2 += num15;
							num13--;
							if (num12 == 0 && num13 == 0 && num16 == 1)
							{
								flag2 = true;
							}
						}
						if (num12 != 0)
						{
							num11 = 2;
						}
						break;
					}
					}
					if (Main.tile[num, num2] == null)
					{
						return false;
					}
					Tile tile7 = Main.tile[num, num2];
					if (!tile7.inActive() && tile7.active() && Main.tileSolid[tile7.type] && !Main.tileSolidTop[tile7.type])
					{
						return false;
					}
				}
				while (!flag && !flag2);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool TupleHitLine(int x1, int y1, int x2, int y2, int ignoreX, int ignoreY, List<Tuple<int, int>> ignoreTargets, out Tuple<int, int> col)
		{
			int value = x1;
			int value2 = y1;
			int value3 = x2;
			int value4 = y2;
			value = Utils.Clamp(value, 1, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 1, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 1, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 1, Main.maxTilesY - 1);
			float num = Math.Abs(value - value3);
			float num2 = Math.Abs(value2 - value4);
			if (num == 0f && num2 == 0f)
			{
				col = new Tuple<int, int>(value, value2);
				return true;
			}
			float num3 = 1f;
			float num4 = 1f;
			if (num == 0f || num2 == 0f)
			{
				if (num == 0f)
				{
					num3 = 0f;
				}
				if (num2 == 0f)
				{
					num4 = 0f;
				}
			}
			else if (num > num2)
			{
				num3 = num / num2;
			}
			else
			{
				num4 = num2 / num;
			}
			float num5 = 0f;
			float num6 = 0f;
			int num7 = 1;
			if (value2 < value4)
			{
				num7 = 2;
			}
			int num8 = (int)num;
			int num9 = (int)num2;
			int num10 = Math.Sign(value3 - value);
			int num11 = Math.Sign(value4 - value2);
			bool flag = false;
			bool flag2 = false;
			try
			{
				do
				{
					switch (num7)
					{
					case 2:
					{
						num5 += num3;
						int num13 = (int)num5;
						num5 %= 1f;
						for (int j = 0; j < num13; j++)
						{
							if (Main.tile[value, value2 - 1] == null)
							{
								col = new Tuple<int, int>(value, value2 - 1);
								return false;
							}
							if (Main.tile[value, value2 + 1] == null)
							{
								col = new Tuple<int, int>(value, value2 + 1);
								return false;
							}
							Tile tile4 = Main.tile[value, value2 - 1];
							Tile tile5 = Main.tile[value, value2 + 1];
							Tile tile6 = Main.tile[value, value2];
							if (!ignoreTargets.Contains(new Tuple<int, int>(value, value2)) && !ignoreTargets.Contains(new Tuple<int, int>(value, value2 - 1)) && !ignoreTargets.Contains(new Tuple<int, int>(value, value2 + 1)))
							{
								if (ignoreY != -1 && num11 < 0 && !tile4.inActive() && tile4.active() && Main.tileSolid[tile4.type] && !Main.tileSolidTop[tile4.type])
								{
									col = new Tuple<int, int>(value, value2 - 1);
									return true;
								}
								if (ignoreY != 1 && num11 > 0 && !tile5.inActive() && tile5.active() && Main.tileSolid[tile5.type] && !Main.tileSolidTop[tile5.type])
								{
									col = new Tuple<int, int>(value, value2 + 1);
									return true;
								}
								if (!tile6.inActive() && tile6.active() && Main.tileSolid[tile6.type] && !Main.tileSolidTop[tile6.type])
								{
									col = new Tuple<int, int>(value, value2);
									return true;
								}
							}
							if (num8 == 0 && num9 == 0)
							{
								flag = true;
								break;
							}
							value += num10;
							num8--;
							if (num8 == 0 && num9 == 0 && num13 == 1)
							{
								flag2 = true;
							}
						}
						if (num9 != 0)
						{
							num7 = 1;
						}
						break;
					}
					case 1:
					{
						num6 += num4;
						int num12 = (int)num6;
						num6 %= 1f;
						for (int i = 0; i < num12; i++)
						{
							if (Main.tile[value - 1, value2] == null)
							{
								col = new Tuple<int, int>(value - 1, value2);
								return false;
							}
							if (Main.tile[value + 1, value2] == null)
							{
								col = new Tuple<int, int>(value + 1, value2);
								return false;
							}
							Tile tile = Main.tile[value - 1, value2];
							Tile tile2 = Main.tile[value + 1, value2];
							Tile tile3 = Main.tile[value, value2];
							if (!ignoreTargets.Contains(new Tuple<int, int>(value, value2)) && !ignoreTargets.Contains(new Tuple<int, int>(value - 1, value2)) && !ignoreTargets.Contains(new Tuple<int, int>(value + 1, value2)))
							{
								if (ignoreX != -1 && num10 < 0 && !tile.inActive() && tile.active() && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type])
								{
									col = new Tuple<int, int>(value - 1, value2);
									return true;
								}
								if (ignoreX != 1 && num10 > 0 && !tile2.inActive() && tile2.active() && Main.tileSolid[tile2.type] && !Main.tileSolidTop[tile2.type])
								{
									col = new Tuple<int, int>(value + 1, value2);
									return true;
								}
								if (!tile3.inActive() && tile3.active() && Main.tileSolid[tile3.type] && !Main.tileSolidTop[tile3.type])
								{
									col = new Tuple<int, int>(value, value2);
									return true;
								}
							}
							if (num8 == 0 && num9 == 0)
							{
								flag = true;
								break;
							}
							value2 += num11;
							num9--;
							if (num8 == 0 && num9 == 0 && num12 == 1)
							{
								flag2 = true;
							}
						}
						if (num8 != 0)
						{
							num7 = 2;
						}
						break;
					}
					}
					if (Main.tile[value, value2] == null)
					{
						col = new Tuple<int, int>(value, value2);
						return false;
					}
					Tile tile7 = Main.tile[value, value2];
					if (!ignoreTargets.Contains(new Tuple<int, int>(value, value2)) && !tile7.inActive() && tile7.active() && Main.tileSolid[tile7.type] && !Main.tileSolidTop[tile7.type])
					{
						col = new Tuple<int, int>(value, value2);
						return true;
					}
				}
				while (!flag && !flag2);
				col = new Tuple<int, int>(value, value2);
				return true;
			}
			catch
			{
				col = new Tuple<int, int>(x1, y1);
				return false;
			}
		}

		public static Tuple<int, int> TupleHitLineWall(int x1, int y1, int x2, int y2)
		{
			int num = x1;
			int num2 = y1;
			int num3 = x2;
			int num4 = y2;
			if (num <= 1)
			{
				num = 1;
			}
			if (num >= Main.maxTilesX)
			{
				num = Main.maxTilesX - 1;
			}
			if (num3 <= 1)
			{
				num3 = 1;
			}
			if (num3 >= Main.maxTilesX)
			{
				num3 = Main.maxTilesX - 1;
			}
			if (num2 <= 1)
			{
				num2 = 1;
			}
			if (num2 >= Main.maxTilesY)
			{
				num2 = Main.maxTilesY - 1;
			}
			if (num4 <= 1)
			{
				num4 = 1;
			}
			if (num4 >= Main.maxTilesY)
			{
				num4 = Main.maxTilesY - 1;
			}
			float num5 = Math.Abs(num - num3);
			float num6 = Math.Abs(num2 - num4);
			if (num5 == 0f && num6 == 0f)
			{
				return new Tuple<int, int>(num, num2);
			}
			float num7 = 1f;
			float num8 = 1f;
			if (num5 == 0f || num6 == 0f)
			{
				if (num5 == 0f)
				{
					num7 = 0f;
				}
				if (num6 == 0f)
				{
					num8 = 0f;
				}
			}
			else if (num5 > num6)
			{
				num7 = num5 / num6;
			}
			else
			{
				num8 = num6 / num5;
			}
			float num9 = 0f;
			float num10 = 0f;
			int num11 = 1;
			if (num2 < num4)
			{
				num11 = 2;
			}
			int num12 = (int)num5;
			int num13 = (int)num6;
			int num14 = Math.Sign(num3 - num);
			int num15 = Math.Sign(num4 - num2);
			bool flag = false;
			bool flag2 = false;
			try
			{
				do
				{
					switch (num11)
					{
					case 2:
					{
						num9 += num7;
						int num17 = (int)num9;
						num9 %= 1f;
						for (int j = 0; j < num17; j++)
						{
							Tile tile = Main.tile[num, num2];
							if (HitWallSubstep(num, num2))
							{
								return new Tuple<int, int>(num, num2);
							}
							if (num12 == 0 && num13 == 0)
							{
								flag = true;
								break;
							}
							num += num14;
							num12--;
							if (num12 == 0 && num13 == 0 && num17 == 1)
							{
								flag2 = true;
							}
						}
						if (num13 != 0)
						{
							num11 = 1;
						}
						break;
					}
					case 1:
					{
						num10 += num8;
						int num16 = (int)num10;
						num10 %= 1f;
						for (int i = 0; i < num16; i++)
						{
							Tile tile2 = Main.tile[num, num2];
							if (HitWallSubstep(num, num2))
							{
								return new Tuple<int, int>(num, num2);
							}
							if (num12 == 0 && num13 == 0)
							{
								flag = true;
								break;
							}
							num2 += num15;
							num13--;
							if (num12 == 0 && num13 == 0 && num16 == 1)
							{
								flag2 = true;
							}
						}
						if (num12 != 0)
						{
							num11 = 2;
						}
						break;
					}
					}
					if (Main.tile[num, num2] == null)
					{
						return new Tuple<int, int>(-1, -1);
					}
					Tile tile3 = Main.tile[num, num2];
					if (HitWallSubstep(num, num2))
					{
						return new Tuple<int, int>(num, num2);
					}
				}
				while (!flag && !flag2);
				return new Tuple<int, int>(num, num2);
			}
			catch
			{
				return new Tuple<int, int>(-1, -1);
			}
		}

		public static bool HitWallSubstep(int x, int y)
		{
			if (Main.tile[x, y].wall == 0)
			{
				return false;
			}
			bool flag = false;
			if (Main.wallHouse[Main.tile[x, y].wall])
			{
				flag = true;
			}
			if (!flag)
			{
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						if ((i != 0 || j != 0) && Main.tile[x + i, y + j].wall == 0)
						{
							flag = true;
						}
					}
				}
			}
			if (Main.tile[x, y].active() && flag)
			{
				bool flag2 = true;
				for (int k = -1; k < 2; k++)
				{
					for (int l = -1; l < 2; l++)
					{
						if (k != 0 || l != 0)
						{
							Tile tile = Main.tile[x + k, y + l];
							if (!tile.active() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type])
							{
								flag2 = false;
							}
						}
					}
				}
				if (flag2)
				{
					flag = false;
				}
			}
			return flag;
		}

		public static bool EmptyTile(int i, int j, bool ignoreTiles = false)
		{
			Rectangle rectangle = new Rectangle(i * 16, j * 16, 16, 16);
			if (Main.tile[i, j].active() && !ignoreTiles)
			{
				return false;
			}
			for (int k = 0; k < 16; k++)
			{
				if (Main.player[k].active && rectangle.Intersects(new Rectangle((int)Main.player[k].position.X, (int)Main.player[k].position.Y, Main.player[k].width, Main.player[k].height)))
				{
					return false;
				}
			}
			for (int l = 0; l < 200; l++)
			{
				if (Main.npc[l].active && rectangle.Intersects(new Rectangle((int)Main.npc[l].position.X, (int)Main.npc[l].position.Y, Main.npc[l].width, Main.npc[l].height)))
				{
					return false;
				}
			}
			return true;
		}

		public static bool DrownCollision(Vector2 Position, int Width, int Height, float gravDir = -1f)
		{
			Vector2 vector = new Vector2(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
			int num = 10;
			int num2 = 12;
			if (num > Width)
			{
				num = Width;
			}
			if (num2 > Height)
			{
				num2 = Height;
			}
			vector = new Vector2(vector.X - (float)(num / 2), Position.Y + -2f);
			if (gravDir == -1f)
			{
				vector.Y += Height / 2 - 6;
			}
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			value = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			int num3 = (gravDir == 1f) ? value3 : (value4 - 1);
			Vector2 vector2 = default(Vector2);
			for (int i = value; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile != null && tile.liquid > 0 && !tile.lava() && (j != num3 || !tile.active() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]))
					{
						vector2.X = i * 16;
						vector2.Y = j * 16;
						int num4 = 16;
						float num5 = 256 - Main.tile[i, j].liquid;
						num5 /= 32f;
						vector2.Y += num5 * 2f;
						num4 -= (int)(num5 * 2f);
						if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num4)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool WetCollision(Vector2 Position, int Width, int Height)
		{
			honey = false;
			Vector2 vector = new Vector2(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
			int num = 10;
			int num2 = Height / 2;
			if (num > Width)
			{
				num = Width;
			}
			if (num2 > Height)
			{
				num2 = Height;
			}
			vector = new Vector2(vector.X - (float)(num / 2), vector.Y - (float)(num2 / 2));
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			value = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			Vector2 vector2 = default(Vector2);
			for (int i = value; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					if (Main.tile[i, j] == null)
					{
						continue;
					}
					if (Main.tile[i, j].liquid > 0)
					{
						vector2.X = i * 16;
						vector2.Y = j * 16;
						int num3 = 16;
						float num4 = 256 - Main.tile[i, j].liquid;
						num4 /= 32f;
						vector2.Y += num4 * 2f;
						num3 -= (int)(num4 * 2f);
						if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num3)
						{
							if (Main.tile[i, j].honey())
							{
								honey = true;
							}
							return true;
						}
					}
					else
					{
						if (!Main.tile[i, j].active() || Main.tile[i, j].slope() == 0 || j <= 0 || Main.tile[i, j - 1] == null || Main.tile[i, j - 1].liquid <= 0)
						{
							continue;
						}
						vector2.X = i * 16;
						vector2.Y = j * 16;
						int num5 = 16;
						if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num5)
						{
							if (Main.tile[i, j - 1].honey())
							{
								honey = true;
							}
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool LavaCollision(Vector2 Position, int Width, int Height)
		{
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			value = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			Vector2 vector = default(Vector2);
			for (int i = value; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].liquid > 0 && Main.tile[i, j].lava())
					{
						vector.X = i * 16;
						vector.Y = j * 16;
						int num = 16;
						float num2 = 256 - Main.tile[i, j].liquid;
						num2 /= 32f;
						vector.Y += num2 * 2f;
						num -= (int)(num2 * 2f);
						if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + (float)num)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static Vector4 WalkDownSlope(Vector2 Position, Vector2 Velocity, int Width, int Height, float gravity = 0f)
		{
			if (Velocity.Y != gravity)
			{
				return new Vector4(Position, Velocity.X, Velocity.Y);
			}
			Vector2 vector = Position;
			int value = (int)(vector.X / 16f);
			int value2 = (int)((vector.X + (float)Width) / 16f);
			int value3 = (int)((Position.Y + (float)Height + 4f) / 16f);
			value = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 3);
			float num = (value3 + 3) * 16;
			int num2 = -1;
			int num3 = -1;
			int num4 = 1;
			if (Velocity.X < 0f)
			{
				num4 = 2;
			}
			for (int i = value; i <= value2; i++)
			{
				for (int j = value3; j <= value3 + 1; j++)
				{
					if (Main.tile[i, j] == null)
					{
						Main.tile[i, j] = new Tile();
					}
					if (!Main.tile[i, j].nactive() || (!Main.tileSolid[Main.tile[i, j].type] && !Main.tileSolidTop[Main.tile[i, j].type]))
					{
						continue;
					}
					int num5 = j * 16;
					if (Main.tile[i, j].halfBrick())
					{
						num5 += 8;
					}
					if (!new Rectangle(i * 16, j * 16 - 17, 16, 16).Intersects(new Rectangle((int)Position.X, (int)Position.Y, Width, Height)) || !((float)num5 <= num))
					{
						continue;
					}
					if (num == (float)num5)
					{
						if (Main.tile[i, j].slope() == 0)
						{
							continue;
						}
						if (num2 != -1 && num3 != -1 && Main.tile[num2, num3] != null && Main.tile[num2, num3].slope() != 0)
						{
							if (Main.tile[i, j].slope() == num4)
							{
								num = num5;
								num2 = i;
								num3 = j;
							}
						}
						else
						{
							num = num5;
							num2 = i;
							num3 = j;
						}
					}
					else
					{
						num = num5;
						num2 = i;
						num3 = j;
					}
				}
			}
			int num6 = num2;
			int num7 = num3;
			if (num2 != -1 && num3 != -1 && Main.tile[num6, num7] != null && Main.tile[num6, num7].slope() > 0)
			{
				int num8 = Main.tile[num6, num7].slope();
				Vector2 vector2 = default(Vector2);
				vector2.X = num6 * 16;
				vector2.Y = num7 * 16;
				switch (num8)
				{
				case 2:
				{
					float num9 = vector2.X + 16f - (Position.X + (float)Width);
					if (Position.Y + (float)Height >= vector2.Y + num9 && Velocity.X < 0f)
					{
						Velocity.Y += Math.Abs(Velocity.X);
					}
					break;
				}
				case 1:
				{
					float num9 = Position.X - vector2.X;
					if (Position.Y + (float)Height >= vector2.Y + num9 && Velocity.X > 0f)
					{
						Velocity.Y += Math.Abs(Velocity.X);
					}
					break;
				}
				}
			}
			return new Vector4(Position, Velocity.X, Velocity.Y);
		}

		public static Vector4 SlopeCollision_Yor(Vector2 Position, Vector2 Velocity, int Width, int Height, float gravity = 0f, bool fall = false)
		{
			stair = false;
			stairFall = false;
			bool[] array = new bool[5];
			float y = Position.Y;
			float y2 = Position.Y;
			sloping = false;
			Vector2 vector = Position;
			Vector2 vector2 = Position;
			Vector2 vector3 = Velocity;
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			value = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			int num = 1;
			int num2 = 1;
			int num3 = value2;
			int num4 = value4;
			if (Velocity.X < 0f)
			{
				num = -1;
				num3 = value;
			}
			if (Velocity.Y < 0f)
			{
				num2 = -1;
				num4 = value3;
			}
			int num5 = (int)(Position.X + (float)(Width / 2)) / 16;
			int num6 = (int)(Position.Y + (float)(Height / 2)) / 16;
			Vector2 vector4 = default(Vector2);
			for (int i = num5; i != num3; i += num)
			{
				for (int j = num6; j != num4; j += num2)
				{
					if (Main.tile[i, j] == null || !Main.tile[i, j].active() || Main.tile[i, j].inActive() || (!Main.tileSolid[Main.tile[i, j].type] && (!Main.tileSolidTop[Main.tile[i, j].type] || Main.tile[i, j].frameY != 0)))
					{
						continue;
					}
					vector4.X = i * 16;
					vector4.Y = j * 16;
					int num7 = 16;
					if (Main.tile[i, j].halfBrick())
					{
						vector4.Y += 8f;
						num7 -= 8;
					}
					if (!(Position.X + (float)Width > vector4.X) || !(Position.X < vector4.X + 16f) || !(Position.Y + (float)Height > vector4.Y) || !(Position.Y < vector4.Y + (float)num7))
					{
						continue;
					}
					bool flag = true;
					if (Main.tile[i, j].slope() > 0)
					{
						if (Main.tile[i, j].slope() > 2)
						{
							if (Main.tile[i, j].slope() == 3 && vector.Y + Math.Abs(Velocity.X) + 1f >= vector4.Y && vector.X >= vector4.X)
							{
								flag = true;
							}
							if (Main.tile[i, j].slope() == 4 && vector.Y + Math.Abs(Velocity.X) + 1f >= vector4.Y && vector.X + (float)Width <= vector4.X + 16f)
							{
								flag = true;
							}
						}
						else
						{
							if (Main.tile[i, j].slope() == 1 && vector.Y + (float)Height - Math.Abs(Velocity.X) - 1f <= vector4.Y + (float)num7 && vector.X >= vector4.X)
							{
								flag = true;
							}
							if (Main.tile[i, j].slope() == 2 && vector.Y + (float)Height - Math.Abs(Velocity.X) - 1f <= vector4.Y + (float)num7 && vector.X + (float)Width <= vector4.X + 16f)
							{
								flag = true;
							}
						}
					}
					if (Main.tile[i, j].type == 19)
					{
						if (Velocity.Y < 0f)
						{
							flag = false;
						}
						if (Position.Y + (float)Height < (float)(j * 16) || Position.Y + (float)Height - (1f + Math.Abs(Velocity.X)) > (float)(j * 16 + 16))
						{
							flag = false;
						}
					}
					if (!flag)
					{
						continue;
					}
					bool flag2 = false;
					if (fall && Main.tile[i, j].type == 19)
					{
						flag2 = true;
					}
					int num8 = Main.tile[i, j].slope();
					vector4.X = i * 16;
					vector4.Y = j * 16;
					if (!(Position.X + (float)Width > vector4.X) || !(Position.X < vector4.X + 16f) || !(Position.Y + (float)Height > vector4.Y) || !(Position.Y < vector4.Y + 16f))
					{
						continue;
					}
					float num9 = 0f;
					if (num8 == 3 || num8 == 4)
					{
						if (num8 == 3)
						{
							num9 = Position.X - vector4.X;
						}
						if (num8 == 4)
						{
							num9 = vector4.X + 16f - (Position.X + (float)Width);
						}
						if (num9 >= 0f)
						{
							if (Position.Y <= vector4.Y + 16f - num9)
							{
								float num10 = vector4.Y + 16f - vector.Y - num9;
								if (Position.Y + num10 > y2)
								{
									vector2.Y += num10;
									y2 = vector2.Y;
									if (vector3.Y < 0.0101f)
									{
										vector3.Y = 0.0101f;
									}
									array[num8] = true;
								}
							}
						}
						else if (Position.Y > vector4.Y)
						{
							float num11 = vector4.Y + 16f;
							if (vector2.Y < num11)
							{
								vector2.Y = num11;
								if (vector3.Y < 0.0101f)
								{
									vector3.Y = 0.0101f;
								}
							}
						}
					}
					if (num8 != 1 && num8 != 2)
					{
						continue;
					}
					if (num8 == 1)
					{
						num9 = Position.X - vector4.X;
					}
					if (num8 == 2)
					{
						num9 = vector4.X + 16f - (Position.X + (float)Width);
					}
					if (num9 >= 0f)
					{
						if (!(Position.Y + (float)Height >= vector4.Y + num9))
						{
							continue;
						}
						float num12 = vector4.Y - (vector.Y + (float)Height) + num9;
						if (!(Position.Y + num12 < y))
						{
							continue;
						}
						if (flag2)
						{
							stairFall = true;
							continue;
						}
						if (Main.tile[i, j].type == 19)
						{
							stair = true;
						}
						else
						{
							stair = false;
						}
						vector2.Y += num12;
						y = vector2.Y;
						if (vector3.Y > 0f)
						{
							vector3.Y = 0f;
						}
						array[num8] = true;
						continue;
					}
					if (Main.tile[i, j].type == 19 && !(Position.Y + (float)Height - 4f - Math.Abs(Velocity.X) <= vector4.Y))
					{
						if (flag2)
						{
							stairFall = true;
						}
						continue;
					}
					float num13 = vector4.Y - (float)Height;
					if (!(vector2.Y > num13))
					{
						continue;
					}
					if (flag2)
					{
						stairFall = true;
						continue;
					}
					if (Main.tile[i, j].type == 19)
					{
						stair = true;
					}
					else
					{
						stair = false;
					}
					vector2.Y = num13;
					if (vector3.Y > 0f)
					{
						vector3.Y = 0f;
					}
				}
			}
			Vector2 position = Position;
			Vector2 velocity = vector2 - Position;
			Vector2 vector5 = TileCollision(position, velocity, Width, Height);
			if (vector5.Y > velocity.Y)
			{
				float num14 = velocity.Y - vector5.Y;
				vector2.Y = Position.Y + vector5.Y;
				if (array[1])
				{
					vector2.X = Position.X - num14;
				}
				if (array[2])
				{
					vector2.X = Position.X + num14;
				}
				vector3.X = 0f;
				vector3.Y = 0f;
				up = false;
			}
			else if (vector5.Y < velocity.Y)
			{
				float num15 = vector5.Y - velocity.Y;
				vector2.Y = Position.Y + vector5.Y;
				if (array[3])
				{
					vector2.X = Position.X - num15;
				}
				if (array[4])
				{
					vector2.X = Position.X + num15;
				}
				vector3.X = 0f;
				vector3.Y = 0f;
			}
			return new Vector4(vector2, vector3.X, vector3.Y);
		}

		public static Vector4 SlopeCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, float gravity = 0f, bool fall = false)
		{
			stair = false;
			stairFall = false;
			bool[] array = new bool[5];
			float y = Position.Y;
			float y2 = Position.Y;
			sloping = false;
			Vector2 vector = Position;
			Vector2 vector2 = Position;
			Vector2 vector3 = Velocity;
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			value = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			Vector2 vector4 = default(Vector2);
			for (int i = value; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					if (Main.tile[i, j] == null || !Main.tile[i, j].active() || Main.tile[i, j].inActive() || (!Main.tileSolid[Main.tile[i, j].type] && (!Main.tileSolidTop[Main.tile[i, j].type] || Main.tile[i, j].frameY != 0)))
					{
						continue;
					}
					vector4.X = i * 16;
					vector4.Y = j * 16;
					int num = 16;
					if (Main.tile[i, j].halfBrick())
					{
						vector4.Y += 8f;
						num -= 8;
					}
					if (!(Position.X + (float)Width > vector4.X) || !(Position.X < vector4.X + 16f) || !(Position.Y + (float)Height > vector4.Y) || !(Position.Y < vector4.Y + (float)num))
					{
						continue;
					}
					bool flag = true;
					if (Main.tile[i, j].slope() > 0)
					{
						if (Main.tile[i, j].slope() > 2)
						{
							if (Main.tile[i, j].slope() == 3 && vector.Y + Math.Abs(Velocity.X) + 1f >= vector4.Y && vector.X >= vector4.X)
							{
								flag = true;
							}
							if (Main.tile[i, j].slope() == 4 && vector.Y + Math.Abs(Velocity.X) + 1f >= vector4.Y && vector.X + (float)Width <= vector4.X + 16f)
							{
								flag = true;
							}
						}
						else
						{
							if (Main.tile[i, j].slope() == 1 && vector.Y + (float)Height - Math.Abs(Velocity.X) - 1f <= vector4.Y + (float)num && vector.X >= vector4.X)
							{
								flag = true;
							}
							if (Main.tile[i, j].slope() == 2 && vector.Y + (float)Height - Math.Abs(Velocity.X) - 1f <= vector4.Y + (float)num && vector.X + (float)Width <= vector4.X + 16f)
							{
								flag = true;
							}
						}
					}
					if (Main.tile[i, j].type == 19)
					{
						if (Velocity.Y < 0f)
						{
							flag = false;
						}
						if (Position.Y + (float)Height < (float)(j * 16) || Position.Y + (float)Height - (1f + Math.Abs(Velocity.X)) > (float)(j * 16 + 16))
						{
							flag = false;
						}
					}
					if (!flag)
					{
						continue;
					}
					bool flag2 = false;
					if (fall && Main.tile[i, j].type == 19)
					{
						flag2 = true;
					}
					int num2 = Main.tile[i, j].slope();
					vector4.X = i * 16;
					vector4.Y = j * 16;
					if (!(Position.X + (float)Width > vector4.X) || !(Position.X < vector4.X + 16f) || !(Position.Y + (float)Height > vector4.Y) || !(Position.Y < vector4.Y + 16f))
					{
						continue;
					}
					float num3 = 0f;
					if (num2 == 3 || num2 == 4)
					{
						if (num2 == 3)
						{
							num3 = Position.X - vector4.X;
						}
						if (num2 == 4)
						{
							num3 = vector4.X + 16f - (Position.X + (float)Width);
						}
						if (num3 >= 0f)
						{
							if (Position.Y <= vector4.Y + 16f - num3)
							{
								float num4 = vector4.Y + 16f - vector.Y - num3;
								if (Position.Y + num4 > y2)
								{
									vector2.Y = Position.Y + num4;
									y2 = vector2.Y;
									if (vector3.Y < 0.0101f)
									{
										vector3.Y = 0.0101f;
									}
									array[num2] = true;
								}
							}
						}
						else if (Position.Y > vector4.Y)
						{
							float num5 = vector4.Y + 16f;
							if (vector2.Y < num5)
							{
								vector2.Y = num5;
								if (vector3.Y < 0.0101f)
								{
									vector3.Y = 0.0101f;
								}
							}
						}
					}
					if (num2 != 1 && num2 != 2)
					{
						continue;
					}
					if (num2 == 1)
					{
						num3 = Position.X - vector4.X;
					}
					if (num2 == 2)
					{
						num3 = vector4.X + 16f - (Position.X + (float)Width);
					}
					if (num3 >= 0f)
					{
						if (!(Position.Y + (float)Height >= vector4.Y + num3))
						{
							continue;
						}
						float num6 = vector4.Y - (vector.Y + (float)Height) + num3;
						if (!(Position.Y + num6 < y))
						{
							continue;
						}
						if (flag2)
						{
							stairFall = true;
							continue;
						}
						if (Main.tile[i, j].type == 19)
						{
							stair = true;
						}
						else
						{
							stair = false;
						}
						vector2.Y = Position.Y + num6;
						y = vector2.Y;
						if (vector3.Y > 0f)
						{
							vector3.Y = 0f;
						}
						array[num2] = true;
						continue;
					}
					if (Main.tile[i, j].type == 19 && !(Position.Y + (float)Height - 4f - Math.Abs(Velocity.X) <= vector4.Y))
					{
						if (flag2)
						{
							stairFall = true;
						}
						continue;
					}
					float num7 = vector4.Y - (float)Height;
					if (!(vector2.Y > num7))
					{
						continue;
					}
					if (flag2)
					{
						stairFall = true;
						continue;
					}
					if (Main.tile[i, j].type == 19)
					{
						stair = true;
					}
					else
					{
						stair = false;
					}
					vector2.Y = num7;
					if (vector3.Y > 0f)
					{
						vector3.Y = 0f;
					}
				}
			}
			Vector2 position = Position;
			Vector2 velocity = vector2 - Position;
			Vector2 vector5 = TileCollision(position, velocity, Width, Height);
			if (vector5.Y > velocity.Y)
			{
				float num8 = velocity.Y - vector5.Y;
				vector2.Y = Position.Y + vector5.Y;
				if (array[1])
				{
					vector2.X = Position.X - num8;
				}
				if (array[2])
				{
					vector2.X = Position.X + num8;
				}
				vector3.X = 0f;
				vector3.Y = 0f;
				up = false;
			}
			else if (vector5.Y < velocity.Y)
			{
				float num9 = vector5.Y - velocity.Y;
				vector2.Y = Position.Y + vector5.Y;
				if (array[3])
				{
					vector2.X = Position.X - num9;
				}
				if (array[4])
				{
					vector2.X = Position.X + num9;
				}
				vector3.X = 0f;
				vector3.Y = 0f;
			}
			return new Vector4(vector2, vector3.X, vector3.Y);
		}

		public static Vector2 noSlopeCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough = false, bool fall2 = false)
		{
			up = false;
			down = false;
			Vector2 result = Velocity;
			Vector2 vector = Velocity;
			Vector2 vector2 = Position + Velocity;
			Vector2 vector3 = Position;
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			value = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			float num5 = (value4 + 3) * 16;
			Vector2 vector4 = default(Vector2);
			for (int i = value; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					if (Main.tile[i, j] == null || !Main.tile[i, j].active() || (!Main.tileSolid[Main.tile[i, j].type] && (!Main.tileSolidTop[Main.tile[i, j].type] || Main.tile[i, j].frameY != 0)))
					{
						continue;
					}
					vector4.X = i * 16;
					vector4.Y = j * 16;
					int num6 = 16;
					if (Main.tile[i, j].halfBrick())
					{
						vector4.Y += 8f;
						num6 -= 8;
					}
					if (!(vector2.X + (float)Width > vector4.X) || !(vector2.X < vector4.X + 16f) || !(vector2.Y + (float)Height > vector4.Y) || !(vector2.Y < vector4.Y + (float)num6))
					{
						continue;
					}
					if (vector3.Y + (float)Height <= vector4.Y)
					{
						down = true;
						if ((!Main.tileSolidTop[Main.tile[i, j].type] || !fallThrough || (!(Velocity.Y <= 1f) && !fall2)) && num5 > vector4.Y)
						{
							num3 = i;
							num4 = j;
							if (num6 < 16)
							{
								num4++;
							}
							if (num3 != num)
							{
								result.Y = vector4.Y - (vector3.Y + (float)Height);
								num5 = vector4.Y;
							}
						}
					}
					else if (vector3.X + (float)Width <= vector4.X && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						num = i;
						num2 = j;
						if (num2 != num4)
						{
							result.X = vector4.X - (vector3.X + (float)Width);
						}
						if (num3 == num)
						{
							result.Y = vector.Y;
						}
					}
					else if (vector3.X >= vector4.X + 16f && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						num = i;
						num2 = j;
						if (num2 != num4)
						{
							result.X = vector4.X + 16f - vector3.X;
						}
						if (num3 == num)
						{
							result.Y = vector.Y;
						}
					}
					else if (vector3.Y >= vector4.Y + (float)num6 && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						up = true;
						num3 = i;
						num4 = j;
						result.Y = vector4.Y + (float)num6 - vector3.Y + 0.01f;
						if (num4 == num2)
						{
							result.X = vector.X;
						}
					}
				}
			}
			return result;
		}

		public static Vector2 TileCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough = false, bool fall2 = false, int gravDir = 1)
		{
			up = false;
			down = false;
			Vector2 result = Velocity;
			Vector2 vector = Velocity;
			Vector2 vector2 = Position + Velocity;
			Vector2 vector3 = Position;
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			value = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			float num5 = (value4 + 3) * 16;
			Vector2 vector4 = default(Vector2);
			for (int i = value; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					if (Main.tile[i, j] == null || !Main.tile[i, j].active() || Main.tile[i, j].inActive() || (!Main.tileSolid[Main.tile[i, j].type] && (!Main.tileSolidTop[Main.tile[i, j].type] || Main.tile[i, j].frameY != 0)))
					{
						continue;
					}
					vector4.X = i * 16;
					vector4.Y = j * 16;
					int num6 = 16;
					if (Main.tile[i, j].halfBrick())
					{
						vector4.Y += 8f;
						num6 -= 8;
					}
					if (!(vector2.X + (float)Width > vector4.X) || !(vector2.X < vector4.X + 16f) || !(vector2.Y + (float)Height > vector4.Y) || !(vector2.Y < vector4.Y + (float)num6))
					{
						continue;
					}
					bool flag = false;
					bool flag2 = false;
					if (Main.tile[i, j].slope() > 2)
					{
						if (Main.tile[i, j].slope() == 3 && vector3.Y + Math.Abs(Velocity.X) >= vector4.Y && vector3.X >= vector4.X)
						{
							flag2 = true;
						}
						if (Main.tile[i, j].slope() == 4 && vector3.Y + Math.Abs(Velocity.X) >= vector4.Y && vector3.X + (float)Width <= vector4.X + 16f)
						{
							flag2 = true;
						}
					}
					else if (Main.tile[i, j].slope() > 0)
					{
						flag = true;
						if (Main.tile[i, j].slope() == 1 && vector3.Y + (float)Height - Math.Abs(Velocity.X) <= vector4.Y + (float)num6 && vector3.X >= vector4.X)
						{
							flag2 = true;
						}
						if (Main.tile[i, j].slope() == 2 && vector3.Y + (float)Height - Math.Abs(Velocity.X) <= vector4.Y + (float)num6 && vector3.X + (float)Width <= vector4.X + 16f)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						continue;
					}
					if (vector3.Y + (float)Height <= vector4.Y)
					{
						down = true;
						if ((!Main.tileSolidTop[Main.tile[i, j].type] || !fallThrough || (!(Velocity.Y <= 1f) && !fall2)) && num5 > vector4.Y)
						{
							num3 = i;
							num4 = j;
							if (num6 < 16)
							{
								num4++;
							}
							if (num3 != num && !flag)
							{
								result.Y = vector4.Y - (vector3.Y + (float)Height) + ((gravDir == -1) ? (-0.01f) : 0f);
								num5 = vector4.Y;
							}
						}
					}
					else if (vector3.X + (float)Width <= vector4.X && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						if (Main.tile[i - 1, j] == null)
						{
							Main.tile[i - 1, j] = new Tile();
						}
						if (Main.tile[i - 1, j].slope() != 2 && Main.tile[i - 1, j].slope() != 4)
						{
							num = i;
							num2 = j;
							if (num2 != num4)
							{
								result.X = vector4.X - (vector3.X + (float)Width);
							}
							if (num3 == num)
							{
								result.Y = vector.Y;
							}
						}
					}
					else if (vector3.X >= vector4.X + 16f && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						if (Main.tile[i + 1, j] == null)
						{
							Main.tile[i + 1, j] = new Tile();
						}
						if (Main.tile[i + 1, j].slope() != 1 && Main.tile[i + 1, j].slope() != 3)
						{
							num = i;
							num2 = j;
							if (num2 != num4)
							{
								result.X = vector4.X + 16f - vector3.X;
							}
							if (num3 == num)
							{
								result.Y = vector.Y;
							}
						}
					}
					else if (vector3.Y >= vector4.Y + (float)num6 && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						up = true;
						num3 = i;
						num4 = j;
						result.Y = vector4.Y + (float)num6 - vector3.Y + ((gravDir == 1) ? 0.01f : 0f);
						if (num4 == num2)
						{
							result.X = vector.X;
						}
					}
				}
			}
			return result;
		}

		public static bool IsClearSpotHack(Vector2 position, float testMagnitude, int Width, int Height, bool fallThrough = false, bool fall2 = false, int gravDir = 1, bool checkCardinals = true, bool checkSlopes = false)
		{
			if (checkCardinals)
			{
				Vector2 vector = Vector2.UnitX * testMagnitude;
				if (TileCollision(position - vector, vector, Width, Height, fallThrough, fall2, gravDir) != vector)
				{
					return false;
				}
				vector = -Vector2.UnitX * testMagnitude;
				if (TileCollision(position - vector, vector, Width, Height, fallThrough, fall2, gravDir) != vector)
				{
					return false;
				}
				vector = Vector2.UnitY * testMagnitude;
				if (TileCollision(position - vector, vector, Width, Height, fallThrough, fall2, gravDir) != vector)
				{
					return false;
				}
				vector = -Vector2.UnitY * testMagnitude;
				if (TileCollision(position - vector, vector, Width, Height, fallThrough, fall2, gravDir) != vector)
				{
					return false;
				}
			}
			if (checkSlopes)
			{
				Vector2 vector = Vector2.UnitX * testMagnitude;
				Vector4 value = new Vector4(position, testMagnitude, 0f);
				if (SlopeCollision(position, vector, Width, Height, gravDir, fallThrough) != value)
				{
					return false;
				}
				vector = -Vector2.UnitX * testMagnitude;
				value = new Vector4(position, 0f - testMagnitude, 0f);
				if (SlopeCollision(position, vector, Width, Height, gravDir, fallThrough) != value)
				{
					return false;
				}
				vector = Vector2.UnitY * testMagnitude;
				value = new Vector4(position, 0f, testMagnitude);
				if (SlopeCollision(position, vector, Width, Height, gravDir, fallThrough) != value)
				{
					return false;
				}
				vector = -Vector2.UnitY * testMagnitude;
				value = new Vector4(position, 0f, 0f - testMagnitude);
				if (SlopeCollision(position, vector, Width, Height, gravDir, fallThrough) != value)
				{
					return false;
				}
			}
			return true;
		}

		public static List<Point> FindCollisionTile(int Direction, Vector2 position, float testMagnitude, int Width, int Height, bool fallThrough = false, bool fall2 = false, int gravDir = 1, bool checkCardinals = true, bool checkSlopes = false)
		{
			List<Point> list = new List<Point>();
			Vector2 vector3;
			Vector2 vector;
			float num;
			float num2;
			int width;
			Vector2 vector2;
			Vector4 vec;
			int y;
			float num3;
			float num4;
			int height;
			switch (Direction)
			{
			case 1:
				vector3 = -Vector2.UnitX * testMagnitude;
				goto IL_0045;
			case 0:
				vector3 = Vector2.UnitX * testMagnitude;
				goto IL_0045;
			case 3:
				vector = -Vector2.UnitY * testMagnitude;
				goto IL_02ab;
			case 2:
				{
					vector = Vector2.UnitY * testMagnitude;
					goto IL_02ab;
				}
				IL_02ab:
				vector2 = vector;
				vec = new Vector4(position, vector2.X, vector2.Y);
				y = (int)(position.Y + (float)((Direction == 2) ? Height : 0)) / 16;
				num = Math.Min(16f - position.X % 16f, Width);
				num2 = num;
				if (checkCardinals && TileCollision(position - vector2, vector2, (int)num, Height, fallThrough, fall2, gravDir) != vector2)
				{
					list.Add(new Point((int)position.X / 16, y));
				}
				else if (checkSlopes && SlopeCollision(position, vector2, (int)num, Height, gravDir, fallThrough).YZW() != vec.YZW())
				{
					list.Add(new Point((int)position.X / 16, y));
				}
				for (; num2 + 16f <= (float)(Width - 16); num2 += 16f)
				{
					if (checkCardinals && TileCollision(position - vector2 + Vector2.UnitX * num2, vector2, 16, Height, fallThrough, fall2, gravDir) != vector2)
					{
						list.Add(new Point((int)(position.X + num2) / 16, y));
					}
					else if (checkSlopes && SlopeCollision(position + Vector2.UnitX * num2, vector2, 16, Height, gravDir, fallThrough).YZW() != vec.YZW())
					{
						list.Add(new Point((int)(position.X + num2) / 16, y));
					}
				}
				width = Width - (int)num2;
				if (checkCardinals && TileCollision(position - vector2 + Vector2.UnitX * num2, vector2, width, Height, fallThrough, fall2, gravDir) != vector2)
				{
					list.Add(new Point((int)(position.X + num2) / 16, y));
				}
				else if (checkSlopes && SlopeCollision(position + Vector2.UnitX * num2, vector2, width, Height, gravDir, fallThrough).YZW() != vec.YZW())
				{
					list.Add(new Point((int)(position.X + num2) / 16, y));
				}
				break;
				IL_0045:
				vector2 = vector3;
				vec = new Vector4(position, vector2.X, vector2.Y);
				y = (int)(position.X + (float)((Direction == 0) ? Width : 0)) / 16;
				num3 = Math.Min(16f - position.Y % 16f, Height);
				num4 = num3;
				if (checkCardinals && TileCollision(position - vector2, vector2, Width, (int)num3, fallThrough, fall2, gravDir) != vector2)
				{
					list.Add(new Point(y, (int)position.Y / 16));
				}
				else if (checkSlopes && SlopeCollision(position, vector2, Width, (int)num3, gravDir, fallThrough).XZW() != vec.XZW())
				{
					list.Add(new Point(y, (int)position.Y / 16));
				}
				for (; num4 + 16f <= (float)(Height - 16); num4 += 16f)
				{
					if (checkCardinals && TileCollision(position - vector2 + Vector2.UnitY * num4, vector2, Width, 16, fallThrough, fall2, gravDir) != vector2)
					{
						list.Add(new Point(y, (int)(position.Y + num4) / 16));
					}
					else if (checkSlopes && SlopeCollision(position + Vector2.UnitY * num4, vector2, Width, 16, gravDir, fallThrough).XZW() != vec.XZW())
					{
						list.Add(new Point(y, (int)(position.Y + num4) / 16));
					}
				}
				height = Height - (int)num4;
				if (checkCardinals && TileCollision(position - vector2 + Vector2.UnitY * num4, vector2, Width, height, fallThrough, fall2, gravDir) != vector2)
				{
					list.Add(new Point(y, (int)(position.Y + num4) / 16));
				}
				else if (checkSlopes && SlopeCollision(position + Vector2.UnitY * num4, vector2, Width, height, gravDir, fallThrough).XZW() != vec.XZW())
				{
					list.Add(new Point(y, (int)(position.Y + num4) / 16));
				}
				break;
			}
			return list;
		}

		public static bool FindCollisionDirection(out int Direction, Vector2 position, int Width, int Height, bool fallThrough = false, bool fall2 = false, int gravDir = 1)
		{
			Vector2 vector = Vector2.UnitX * 16f;
			if (TileCollision(position - vector, vector, Width, Height, fallThrough, fall2, gravDir) != vector)
			{
				Direction = 0;
				return true;
			}
			vector = -Vector2.UnitX * 16f;
			if (TileCollision(position - vector, vector, Width, Height, fallThrough, fall2, gravDir) != vector)
			{
				Direction = 1;
				return true;
			}
			vector = Vector2.UnitY * 16f;
			if (TileCollision(position - vector, vector, Width, Height, fallThrough, fall2, gravDir) != vector)
			{
				Direction = 2;
				return true;
			}
			vector = -Vector2.UnitY * 16f;
			if (TileCollision(position - vector, vector, Width, Height, fallThrough, fall2, gravDir) != vector)
			{
				Direction = 3;
				return true;
			}
			Direction = -1;
			return false;
		}

		public static bool SolidCollision(Vector2 Position, int Width, int Height)
		{
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			value = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			Vector2 vector = default(Vector2);
			for (int i = value; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					if (Main.tile[i, j] != null && !Main.tile[i, j].inActive() && Main.tile[i, j].active() && Main.tileSolid[Main.tile[i, j].type] && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						vector.X = i * 16;
						vector.Y = j * 16;
						int num = 16;
						if (Main.tile[i, j].halfBrick())
						{
							vector.Y += 8f;
							num -= 8;
						}
						if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + (float)num)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static Vector2 WaterCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough = false, bool fall2 = false, bool lavaWalk = true)
		{
			Vector2 result = Velocity;
			Vector2 vector = Position + Velocity;
			Vector2 vector2 = Position;
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			value = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			Vector2 vector3 = default(Vector2);
			for (int i = value; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].liquid > 0 && Main.tile[i, j - 1].liquid == 0 && (!Main.tile[i, j].lava() || lavaWalk))
					{
						int num = (int)Main.tile[i, j].liquid / 32 * 2 + 2;
						vector3.X = i * 16;
						vector3.Y = j * 16 + 16 - num;
						if (vector.X + (float)Width > vector3.X && vector.X < vector3.X + 16f && vector.Y + (float)Height > vector3.Y && vector.Y < vector3.Y + (float)num && vector2.Y + (float)Height <= vector3.Y && !fallThrough)
						{
							result.Y = vector3.Y - (vector2.Y + (float)Height);
						}
					}
				}
			}
			return result;
		}

		public static Vector2 AnyCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool evenActuated = false)
		{
			Vector2 result = Velocity;
			Vector2 vector = Velocity;
			Vector2 vector2 = Position + Velocity;
			Vector2 vector3 = Position;
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			int num8 = -1;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 > Main.maxTilesX)
			{
				num2 = Main.maxTilesX;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 > Main.maxTilesY)
			{
				num4 = Main.maxTilesY;
			}
			Vector2 vector4 = default(Vector2);
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] == null || !Main.tile[i, j].active() || (!evenActuated && Main.tile[i, j].inActive()))
					{
						continue;
					}
					vector4.X = i * 16;
					vector4.Y = j * 16;
					int num9 = 16;
					if (Main.tile[i, j].halfBrick())
					{
						vector4.Y += 8f;
						num9 -= 8;
					}
					if (!(vector2.X + (float)Width > vector4.X) || !(vector2.X < vector4.X + 16f) || !(vector2.Y + (float)Height > vector4.Y) || !(vector2.Y < vector4.Y + (float)num9))
					{
						continue;
					}
					if (vector3.Y + (float)Height <= vector4.Y)
					{
						num7 = i;
						num8 = j;
						if (num7 != num5)
						{
							result.Y = vector4.Y - (vector3.Y + (float)Height);
						}
					}
					else if (vector3.X + (float)Width <= vector4.X && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						num5 = i;
						num6 = j;
						if (num6 != num8)
						{
							result.X = vector4.X - (vector3.X + (float)Width);
						}
						if (num7 == num5)
						{
							result.Y = vector.Y;
						}
					}
					else if (vector3.X >= vector4.X + 16f && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						num5 = i;
						num6 = j;
						if (num6 != num8)
						{
							result.X = vector4.X + 16f - vector3.X;
						}
						if (num7 == num5)
						{
							result.Y = vector.Y;
						}
					}
					else if (vector3.Y >= vector4.Y + (float)num9 && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						num7 = i;
						num8 = j;
						result.Y = vector4.Y + (float)num9 - vector3.Y + 0.01f;
						if (num8 == num6)
						{
							result.X = vector.X + 0.01f;
						}
					}
				}
			}
			return result;
		}

		public static void HitTiles(Vector2 Position, Vector2 Velocity, int Width, int Height)
		{
			Vector2 vector = Position + Velocity;
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 > Main.maxTilesX)
			{
				num2 = Main.maxTilesX;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 > Main.maxTilesY)
			{
				num4 = Main.maxTilesY;
			}
			Vector2 vector2 = default(Vector2);
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] != null && !Main.tile[i, j].inActive() && Main.tile[i, j].active() && (Main.tileSolid[Main.tile[i, j].type] || (Main.tileSolidTop[Main.tile[i, j].type] && Main.tile[i, j].frameY == 0)))
					{
						vector2.X = i * 16;
						vector2.Y = j * 16;
						int num5 = 16;
						if (Main.tile[i, j].halfBrick())
						{
							vector2.Y += 8f;
							num5 -= 8;
						}
						if (vector.X + (float)Width >= vector2.X && vector.X <= vector2.X + 16f && vector.Y + (float)Height >= vector2.Y && vector.Y <= vector2.Y + (float)num5)
						{
							WorldGen.KillTile(i, j, true, true);
						}
					}
				}
			}
		}

		public static Vector2 HurtTiles(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fireImmune = false)
		{
			Vector2 vector = Position;
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 > Main.maxTilesX)
			{
				num2 = Main.maxTilesX;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 > Main.maxTilesY)
			{
				num4 = Main.maxTilesY;
			}
			Vector2 vector2 = default(Vector2);
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] == null || Main.tile[i, j].slope() != 0 || Main.tile[i, j].inActive() || !Main.tile[i, j].active() || (Main.tile[i, j].type != 32 && Main.tile[i, j].type != 37 && Main.tile[i, j].type != 48 && Main.tile[i, j].type != 232 && Main.tile[i, j].type != 53 && Main.tile[i, j].type != 57 && Main.tile[i, j].type != 58 && Main.tile[i, j].type != 69 && Main.tile[i, j].type != 76 && Main.tile[i, j].type != 112 && Main.tile[i, j].type != 116 && Main.tile[i, j].type != 123 && Main.tile[i, j].type != 224 && Main.tile[i, j].type != 234 && Main.tile[i, j].type != 352))
					{
						continue;
					}
					vector2.X = i * 16;
					vector2.Y = j * 16;
					int num5 = 0;
					int type = Main.tile[i, j].type;
					int num6 = 16;
					if (Main.tile[i, j].halfBrick())
					{
						vector2.Y += 8f;
						num6 -= 8;
					}
					if (type == 32 || type == 69 || type == 80 || type == 352 || (type == 80 && Main.expertMode))
					{
						if (!(vector.X + (float)Width > vector2.X) || !(vector.X < vector2.X + 16f) || !(vector.Y + (float)Height > vector2.Y) || !(vector.Y < vector2.Y + (float)num6 + 0.011f))
						{
							continue;
						}
						int num7 = 1;
						if (vector.X + (float)(Width / 2) < vector2.X + 8f)
						{
							num7 = -1;
						}
						num5 = 10;
						switch (type)
						{
						case 69:
							num5 = 17;
							break;
						case 80:
							num5 = 6;
							break;
						}
						if (type == 32 || type == 69 || type == 352)
						{
							WorldGen.KillTile(i, j);
							if (Main.netMode == 1 && !Main.tile[i, j].active() && Main.netMode == 1)
							{
								NetMessage.SendData(17, -1, -1, "", 4, i, j);
							}
						}
						return new Vector2(num7, num5);
					}
					if (type == 53 || type == 112 || type == 116 || type == 123 || type == 224 || type == 234)
					{
						if (vector.X + (float)Width - 2f >= vector2.X && vector.X + 2f <= vector2.X + 16f && vector.Y + (float)Height - 2f >= vector2.Y && vector.Y + 2f <= vector2.Y + (float)num6)
						{
							int num8 = 1;
							if (vector.X + (float)(Width / 2) < vector2.X + 8f)
							{
								num8 = -1;
							}
							num5 = 15;
							return new Vector2(num8, num5);
						}
					}
					else if (vector.X + (float)Width >= vector2.X && vector.X <= vector2.X + 16f && vector.Y + (float)Height >= vector2.Y && vector.Y <= vector2.Y + (float)num6 + 0.011f)
					{
						int num9 = 1;
						if (vector.X + (float)(Width / 2) < vector2.X + 8f)
						{
							num9 = -1;
						}
						if (!fireImmune && (type == 37 || type == 58 || type == 76))
						{
							num5 = 20;
						}
						if (type == 48)
						{
							num5 = 40;
						}
						if (type == 232)
						{
							num5 = 60;
						}
						return new Vector2(num9, num5);
					}
				}
			}
			return default(Vector2);
		}

		public static bool SwitchTiles(Vector2 Position, int Width, int Height, Vector2 oldPosition, int objType)
		{
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 > Main.maxTilesX)
			{
				num2 = Main.maxTilesX;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 > Main.maxTilesY)
			{
				num4 = Main.maxTilesY;
			}
			Vector2 vector = default(Vector2);
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] == null || !Main.tile[i, j].active() || (Main.tile[i, j].type != 135 && Main.tile[i, j].type != 210))
					{
						continue;
					}
					vector.X = i * 16;
					vector.Y = j * 16 + 12;
					if (!(Position.X + (float)Width > vector.X) || !(Position.X < vector.X + 16f) || !(Position.Y + (float)Height > vector.Y) || !((double)Position.Y < (double)vector.Y + 4.01))
					{
						continue;
					}
					if (Main.tile[i, j].type == 210)
					{
						WorldGen.ExplodeMine(i, j);
					}
					else
					{
						if (oldPosition.X + (float)Width > vector.X && oldPosition.X < vector.X + 16f && oldPosition.Y + (float)Height > vector.Y && (double)oldPosition.Y < (double)vector.Y + 16.01)
						{
							continue;
						}
						int num5 = Main.tile[i, j].frameY / 18;
						bool flag = true;
						if ((num5 == 4 || num5 == 2 || num5 == 3 || num5 == 6) && objType != 1)
						{
							flag = false;
						}
						if (num5 == 5 && objType == 1)
						{
							flag = false;
						}
						if (flag)
						{
							if (objType == 1)
							{
								NetMessage.SendData(13, -1, -1, "", Main.myPlayer);
							}
							Wiring.HitSwitch(i, j);
							NetMessage.SendData(59, -1, -1, "", i, j);
							return true;
						}
					}
				}
			}
			return false;
		}

		public static Vector2 StickyTiles(Vector2 Position, Vector2 Velocity, int Width, int Height)
		{
			Vector2 vector = Position;
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			if (num < 0)
			{
				num = 0;
			}
			if (num2 > Main.maxTilesX)
			{
				num2 = Main.maxTilesX;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 > Main.maxTilesY)
			{
				num4 = Main.maxTilesY;
			}
			Vector2 vector2 = default(Vector2);
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] == null || !Main.tile[i, j].active() || Main.tile[i, j].inActive())
					{
						continue;
					}
					if (Main.tile[i, j].type == 51)
					{
						int num5 = 0;
						vector2.X = i * 16;
						vector2.Y = j * 16;
						if (vector.X + (float)Width > vector2.X - (float)num5 && vector.X < vector2.X + 16f + (float)num5 && vector.Y + (float)Height > vector2.Y && (double)vector.Y < (double)vector2.Y + 16.01)
						{
							if (Main.tile[i, j].type == 51 && (double)(Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) > 0.7 && Main.rand.Next(30) == 0)
							{
								Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, 30);
							}
							return new Vector2(i, j);
						}
					}
					else
					{
						if (Main.tile[i, j].type != 229 || Main.tile[i, j].slope() != 0)
						{
							continue;
						}
						int num6 = 1;
						vector2.X = i * 16;
						vector2.Y = j * 16;
						float num7 = 16.01f;
						if (Main.tile[i, j].halfBrick())
						{
							vector2.Y += 8f;
							num7 -= 8f;
						}
						if (vector.X + (float)Width > vector2.X - (float)num6 && vector.X < vector2.X + 16f + (float)num6 && vector.Y + (float)Height > vector2.Y && vector.Y < vector2.Y + num7)
						{
							if (Main.tile[i, j].type == 51 && (double)(Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) > 0.7 && Main.rand.Next(30) == 0)
							{
								Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, 30);
							}
							return new Vector2(i, j);
						}
					}
				}
			}
			return new Vector2(-1f, -1f);
		}

		public static bool SolidTilesVersatile(int startX, int endX, int startY, int endY)
		{
			if (startX > endX)
			{
				Utils.Swap(ref startX, ref endX);
			}
			if (startY > endY)
			{
				Utils.Swap(ref startY, ref endY);
			}
			return SolidTiles(startX, endX, startY, endY);
		}

		public static bool SolidTiles(int startX, int endX, int startY, int endY)
		{
			if (startX < 0)
			{
				return true;
			}
			if (endX >= Main.maxTilesX)
			{
				return true;
			}
			if (startY < 0)
			{
				return true;
			}
			if (endY >= Main.maxTilesY)
			{
				return true;
			}
			for (int i = startX; i < endX + 1; i++)
			{
				for (int j = startY; j < endY + 1; j++)
				{
					if (Main.tile[i, j] == null)
					{
						return false;
					}
					if (Main.tile[i, j].active() && !Main.tile[i, j].inActive() && Main.tileSolid[Main.tile[i, j].type] && !Main.tileSolidTop[Main.tile[i, j].type])
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void StepDown(ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir = 1, bool waterWalk = false)
		{
			Vector2 vector = position;
			vector.X += velocity.X;
			vector.Y = (float)Math.Floor((vector.Y + (float)height) / 16f) * 16f - (float)height;
			bool flag = false;
			int num = (int)(vector.X / 16f);
			int num2 = (int)((vector.X + (float)width) / 16f);
			int num3 = (int)((vector.Y + (float)height + 4f) / 16f);
			int num4 = height / 16 + ((height % 16 != 0) ? 1 : 0);
			float num5 = (num3 + num4) * 16;
			float num6 = Main.bottomWorld / 16f - 42f;
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num3 + 1; j++)
				{
					if (!WorldGen.InWorld(i, j, 1))
					{
						continue;
					}
					if (Main.tile[i, j] == null)
					{
						Main.tile[i, j] = new Tile();
					}
					if (Main.tile[i, j - 1] == null)
					{
						Main.tile[i, j - 1] = new Tile();
					}
					if (Main.tile[i, j].topSlope())
					{
						flag = true;
					}
					if (waterWalk && Main.tile[i, j].liquid > 0 && Main.tile[i, j - 1].liquid == 0)
					{
						int num7 = (int)Main.tile[i, j].liquid / 32 * 2 + 2;
						int num8 = j * 16 + 16 - num7;
						if (new Rectangle(i * 16, j * 16 - 17, 16, 16).Intersects(new Rectangle((int)position.X, (int)position.Y, width, height)) && (float)num8 < num5)
						{
							num5 = num8;
						}
					}
					if ((float)j >= num6 || (Main.tile[i, j].nactive() && (Main.tileSolid[Main.tile[i, j].type] || Main.tileSolidTop[Main.tile[i, j].type])))
					{
						int num9 = j * 16;
						if (Main.tile[i, j].halfBrick())
						{
							num9 += 8;
						}
						if (Utils.FloatIntersect(i * 16, j * 16 - 17, 16f, 16f, position.X, position.Y, width, height) && (float)num9 < num5)
						{
							num5 = num9;
						}
					}
				}
			}
			float num10 = num5 - (position.Y + (float)height);
			if (num10 > 7f && num10 < 17f && !flag)
			{
				stepSpeed = 1.5f;
				if (num10 > 9f)
				{
					stepSpeed = 2.5f;
				}
				gfxOffY += position.Y + (float)height - num5;
				position.Y = num5 - (float)height;
			}
		}

		public static void StepUp(ref Vector2 position, ref Vector2 velocity, int width, int height, ref float stepSpeed, ref float gfxOffY, int gravDir = 1, bool holdsMatching = false, int specialChecksMode = 0)
		{
			int num = 0;
			if (velocity.X < 0f)
			{
				num = -1;
			}
			if (velocity.X > 0f)
			{
				num = 1;
			}
			Vector2 vector = position;
			vector.X += velocity.X;
			int num2 = (int)((vector.X + (float)(width / 2) + (float)((width / 2 + 1) * num)) / 16f);
			int num3 = (int)(((double)vector.Y + 0.1) / 16.0);
			if (gravDir == 1)
			{
				num3 = (int)((vector.Y + (float)height - 1f) / 16f);
			}
			int num4 = height / 16 + ((height % 16 != 0) ? 1 : 0);
			bool flag = true;
			bool flag2 = true;
			if (Main.tile[num2, num3] == null)
			{
				return;
			}
			for (int i = 1; i < num4 + 2; i++)
			{
				if (!WorldGen.InWorld(num2, num3 - i * gravDir) || Main.tile[num2, num3 - i * gravDir] == null)
				{
					return;
				}
			}
			if (!WorldGen.InWorld(num2 - num, num3 - num4 * gravDir) || Main.tile[num2 - num, num3 - num4 * gravDir] == null)
			{
				return;
			}
			Tile tile;
			for (int j = 2; j < num4 + 1; j++)
			{
				if (!WorldGen.InWorld(num2, num3 - j * gravDir) || Main.tile[num2, num3 - j * gravDir] == null)
				{
					return;
				}
				tile = Main.tile[num2, num3 - j * gravDir];
				flag = (flag && (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]));
			}
			tile = Main.tile[num2 - num, num3 - num4 * gravDir];
			flag2 = (flag2 && (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]));
			bool flag3 = true;
			bool flag4 = true;
			bool flag5 = true;
			if (gravDir == 1)
			{
				if (Main.tile[num2, num3 - gravDir] == null || Main.tile[num2, num3 - (num4 + 1) * gravDir] == null)
				{
					return;
				}
				tile = Main.tile[num2, num3 - gravDir];
				Tile tile2 = Main.tile[num2, num3 - (num4 + 1) * gravDir];
				flag3 = (flag3 && (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type] || (tile.slope() == 1 && position.X + (float)(width / 2) > (float)(num2 * 16)) || (tile.slope() == 2 && position.X + (float)(width / 2) < (float)(num2 * 16 + 16)) || (tile.halfBrick() && (!tile2.nactive() || !Main.tileSolid[tile2.type] || Main.tileSolidTop[tile2.type]))));
				tile = Main.tile[num2, num3];
				tile2 = Main.tile[num2, num3 - 1];
				if (specialChecksMode == 1)
				{
					flag5 = (tile.type != 16 && tile.type != 18 && tile.type != 134);
				}
				flag4 = (flag4 && ((tile.nactive() && (!tile.topSlope() || (tile.slope() == 1 && position.X + (float)(width / 2) < (float)(num2 * 16)) || (tile.slope() == 2 && position.X + (float)(width / 2) > (float)(num2 * 16 + 16))) && (!tile.topSlope() || position.Y + (float)height > (float)(num3 * 16)) && ((Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type]) || (holdsMatching && ((Main.tileSolidTop[tile.type] && tile.frameY == 0) || tile.type == 19) && (!Main.tileSolid[tile2.type] || !tile2.nactive()) && flag5))) || (tile2.halfBrick() && tile2.nactive())));
				flag4 &= (!Main.tileSolidTop[tile.type] || !Main.tileSolidTop[tile2.type]);
			}
			else
			{
				tile = Main.tile[num2, num3 - gravDir];
				Tile tile2 = Main.tile[num2, num3 - (num4 + 1) * gravDir];
				flag3 = (flag3 && (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type] || tile.slope() != 0 || (tile.halfBrick() && (!tile2.nactive() || !Main.tileSolid[tile2.type] || Main.tileSolidTop[tile2.type]))));
				tile = Main.tile[num2, num3];
				tile2 = Main.tile[num2, num3 + 1];
				flag4 = (flag4 && ((tile.nactive() && ((Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type]) || (holdsMatching && Main.tileSolidTop[tile.type] && tile.frameY == 0 && (!Main.tileSolid[tile2.type] || !tile2.nactive())))) || (tile2.halfBrick() && tile2.nactive())));
			}
			if (!((float)(num2 * 16) < vector.X + (float)width) || !((float)(num2 * 16 + 16) > vector.X))
			{
				return;
			}
			if (gravDir == 1)
			{
				if (!flag4 || !flag3 || !flag || !flag2)
				{
					return;
				}
				float num5 = num3 * 16;
				if (Main.tile[num2, num3 - 1].halfBrick())
				{
					num5 -= 8f;
				}
				else if (Main.tile[num2, num3].halfBrick())
				{
					num5 += 8f;
				}
				if (!(num5 < vector.Y + (float)height))
				{
					return;
				}
				float num6 = vector.Y + (float)height - num5;
				if ((double)num6 <= 16.1)
				{
					gfxOffY += position.Y + (float)height - num5;
					position.Y = num5 - (float)height;
					if (num6 < 9f)
					{
						stepSpeed = 1f;
					}
					else
					{
						stepSpeed = 2f;
					}
				}
			}
			else
			{
				if (!flag4 || !flag3 || !flag || !flag2 || Main.tile[num2, num3].bottomSlope())
				{
					return;
				}
				float num7 = num3 * 16 + 16;
				if (!(num7 > vector.Y))
				{
					return;
				}
				float num8 = num7 - vector.Y;
				if ((double)num8 <= 16.1)
				{
					gfxOffY -= num7 - position.Y;
					position.Y = num7;
					velocity.Y = 0f;
					if (num8 < 9f)
					{
						stepSpeed = 1f;
					}
					else
					{
						stepSpeed = 2f;
					}
				}
			}
		}

		public static bool InTileBounds(int x, int y, int lx, int ly, int hx, int hy)
		{
			if (x < lx || x > hx || y < ly || y > hy)
			{
				return false;
			}
			return true;
		}

		public static float GetTileRotation(Vector2 position)
		{
			float num = position.Y % 16f;
			int num2 = (int)(position.X / 16f);
			int num3 = (int)(position.Y / 16f);
			Tile tile = Main.tile[num2, num3];
			bool flag = false;
			for (int num4 = 2; num4 >= 0; num4--)
			{
				if (tile.active())
				{
					if (Main.tileSolid[tile.type])
					{
						int num5 = tile.blockType();
						if (tile.type != 19)
						{
							switch (num5)
							{
							case 1:
								return 0f;
							case 2:
								return (float)Math.PI / 4f;
							case 3:
								return -(float)Math.PI / 4f;
							default:
								return 0f;
							}
						}
						int num6 = tile.frameX / 18;
						if (((num6 >= 0 && num6 <= 7) || (num6 >= 12 && num6 <= 16)) && (num == 0f || flag))
						{
							return 0f;
						}
						switch (num6)
						{
						case 8:
						case 19:
						case 21:
						case 23:
							return -(float)Math.PI / 4f;
						case 10:
						case 20:
						case 22:
						case 24:
							return (float)Math.PI / 4f;
						case 25:
						case 26:
							if (!flag)
							{
								switch (num5)
								{
								case 2:
									return (float)Math.PI / 4f;
								case 3:
									return -(float)Math.PI / 4f;
								}
								break;
							}
							return 0f;
						}
					}
					else if (Main.tileSolidTop[tile.type] && tile.frameY == 0 && flag)
					{
						return 0f;
					}
				}
				num3++;
				tile = Main.tile[num2, num3];
				flag = true;
			}
			return 0f;
		}
	}
}
