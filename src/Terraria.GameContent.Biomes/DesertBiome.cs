using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	internal class DesertBiome : MicroBiome
	{
		private struct Hub
		{
			public Vector2 Position;

			public Hub(Vector2 position)
			{
				Position = position;
			}

			public Hub(float x, float y)
			{
				Position = new Vector2(x, y);
			}
		}

		private class Cluster : List<Hub>
		{
		}

		private class ClusterGroup : List<Cluster>
		{
			public int Width;

			public int Height;

			private void SearchForCluster(bool[,] hubMap, List<Point> pointCluster, int x, int y, int level = 2)
			{
				pointCluster.Add(new Point(x, y));
				hubMap[x, y] = false;
				level--;
				if (level != -1)
				{
					if (x > 0 && hubMap[x - 1, y])
					{
						SearchForCluster(hubMap, pointCluster, x - 1, y, level);
					}
					if (x < hubMap.GetLength(0) - 1 && hubMap[x + 1, y])
					{
						SearchForCluster(hubMap, pointCluster, x + 1, y, level);
					}
					if (y > 0 && hubMap[x, y - 1])
					{
						SearchForCluster(hubMap, pointCluster, x, y - 1, level);
					}
					if (y < hubMap.GetLength(1) - 1 && hubMap[x, y + 1])
					{
						SearchForCluster(hubMap, pointCluster, x, y + 1, level);
					}
				}
			}

			private void AttemptClaim(int x, int y, int[,] clusterIndexMap, List<List<Point>> pointClusters, int index)
			{
				int num = clusterIndexMap[x, y];
				if (num != -1 && num != index)
				{
					int num2 = (WorldGen.genRand.Next(2) == 0) ? (-1) : index;
					List<Point> list = pointClusters[num];
					foreach (Point item in list)
					{
						clusterIndexMap[item.X, item.Y] = num2;
					}
				}
			}

			public void Generate(int width, int height)
			{
				Width = width;
				Height = height;
				Clear();
				bool[,] array = new bool[width, height];
				int num = (width >> 1) - 1;
				int num2 = (height >> 1) - 1;
				int num3 = (num + 1) * (num + 1);
				Point point = new Point(num, num2);
				for (int i = point.Y - num2; i <= point.Y + num2; i++)
				{
					float num4 = (float)num / (float)num2 * (float)(i - point.Y);
					int num5 = Math.Min(num, (int)Math.Sqrt((float)num3 - num4 * num4));
					for (int j = point.X - num5; j <= point.X + num5; j++)
					{
						array[j, i] = (WorldGen.genRand.Next(2) == 0);
					}
				}
				List<List<Point>> list = new List<List<Point>>();
				for (int k = 0; k < array.GetLength(0); k++)
				{
					for (int l = 0; l < array.GetLength(1); l++)
					{
						if (array[k, l] && WorldGen.genRand.Next(2) == 0)
						{
							List<Point> list2 = new List<Point>();
							SearchForCluster(array, list2, k, l);
							if (list2.Count > 2)
							{
								list.Add(list2);
							}
						}
					}
				}
				int[,] array2 = new int[array.GetLength(0), array.GetLength(1)];
				for (int m = 0; m < array2.GetLength(0); m++)
				{
					for (int n = 0; n < array2.GetLength(1); n++)
					{
						array2[m, n] = -1;
					}
				}
				for (int num6 = 0; num6 < list.Count; num6++)
				{
					foreach (Point item in list[num6])
					{
						array2[item.X, item.Y] = num6;
					}
				}
				for (int num7 = 0; num7 < list.Count; num7++)
				{
					List<Point> list3 = list[num7];
					foreach (Point item2 in list3)
					{
						int x = item2.X;
						int y = item2.Y;
						if (array2[x, y] == -1)
						{
							break;
						}
						int index = array2[x, y];
						if (x > 0)
						{
							AttemptClaim(x - 1, y, array2, list, index);
						}
						if (x < array2.GetLength(0) - 1)
						{
							AttemptClaim(x + 1, y, array2, list, index);
						}
						if (y > 0)
						{
							AttemptClaim(x, y - 1, array2, list, index);
						}
						if (y < array2.GetLength(1) - 1)
						{
							AttemptClaim(x, y + 1, array2, list, index);
						}
					}
				}
				foreach (List<Point> item3 in list)
				{
					item3.Clear();
				}
				for (int num8 = 0; num8 < array2.GetLength(0); num8++)
				{
					for (int num9 = 0; num9 < array2.GetLength(1); num9++)
					{
						if (array2[num8, num9] != -1)
						{
							list[array2[num8, num9]].Add(new Point(num8, num9));
						}
					}
				}
				foreach (List<Point> item4 in list)
				{
					if (item4.Count < 4)
					{
						item4.Clear();
					}
				}
				foreach (List<Point> item5 in list)
				{
					Cluster cluster = new Cluster();
					if (item5.Count > 0)
					{
						foreach (Point item6 in item5)
						{
							cluster.Add(new Hub((float)item6.X + (WorldGen.genRand.NextFloat() - 0.5f) * 0.5f, (float)item6.Y + (WorldGen.genRand.NextFloat() - 0.5f) * 0.5f));
						}
						Add(cluster);
					}
				}
			}
		}

		private void PlaceSand(ClusterGroup clusters, Point start, Vector2 scale)
		{
			int num = (int)(scale.X * (float)clusters.Width);
			int num2 = (int)(scale.Y * (float)clusters.Height);
			int num3 = 5;
			int num4 = start.Y + (num2 >> 1);
			float num5 = 0f;
			short[] array = new short[num + num3 * 2];
			for (int i = -num3; i < num + num3; i++)
			{
				for (int j = 150; j < num4; j++)
				{
					if (WorldGen.SolidOrSlopedTile(i + start.X, j))
					{
						num5 += (float)(j - 1);
						array[i + num3] = (short)(j - 1);
						break;
					}
				}
			}
			float num6 = num5 / (float)(num + num3 * 2);
			int num7 = 0;
			for (int k = -num3; k < num + num3; k++)
			{
				float value = Math.Abs((float)(k + num3) / (float)(num + num3 * 2)) * 2f - 1f;
				value = MathHelper.Clamp(value, -1f, 1f);
				if (k % 3 == 0)
				{
					num7 = Utils.Clamp(num7 + GenBase._random.Next(-1, 2), -10, 10);
				}
				float num8 = (float)Math.Sqrt(1f - value * value * value * value);
				int num9 = num4 - (int)(num8 * ((float)num4 - num6)) + num7;
				int val = num4 - (int)(((float)num4 - num6) * (num8 - 0.15f / (float)Math.Sqrt(Math.Max(0.01, (double)Math.Abs(8f * value) - 0.1)) + 0.25f));
				val = Math.Min(num4, val);
				int num10 = (int)(70f - Utils.SmoothStep(0.5f, 0.8f, Math.Abs(value)) * 70f);
				if (num9 - array[k + num3] < num10)
				{
					for (int l = 0; l < num10; l++)
					{
						int num11 = k + start.X;
						int num12 = l + num9 - 70;
						GenBase._tiles[num11, num12].active(false);
						GenBase._tiles[num11, num12].wall = 0;
					}
					array[k + num3] = (short)Utils.Clamp(num10 + num9 - 70, num9, array[k + num3]);
				}
				for (int num13 = num4 - 1; num13 >= num9; num13--)
				{
					int num14 = k + start.X;
					int num15 = num13;
					Tile tile = GenBase._tiles[num14, num15];
					tile.liquid = 0;
					Tile testTile = GenBase._tiles[num14, num15 + 1];
					Tile testTile2 = GenBase._tiles[num14, num15 + 2];
					tile.type = (ushort)((WorldGen.SolidTile(testTile) && WorldGen.SolidTile(testTile2)) ? 53 : 397);
					if (num13 > num9 + 5)
					{
						tile.wall = 187;
					}
					tile.active(true);
					if (tile.wall != 187)
					{
						tile.wall = 0;
					}
					if (num13 < val)
					{
						if (num13 > num9 + 5)
						{
							tile.wall = 187;
						}
						tile.active(false);
					}
					WorldGen.SquareWallFrame(num14, num15);
				}
			}
		}

		private void PlaceClusters(ClusterGroup clusters, Point start, Vector2 scale)
		{
			int num = (int)(scale.X * (float)clusters.Width);
			int num2 = (int)(scale.Y * (float)clusters.Height);
			Vector2 value = new Vector2(num, num2);
			Vector2 value2 = new Vector2(clusters.Width, clusters.Height);
			for (int i = -20; i < num + 20; i++)
			{
				for (int j = -20; j < num2 + 20; j++)
				{
					float num3 = 0f;
					int num4 = -1;
					float num5 = 0f;
					int num6 = i + start.X;
					int num7 = j + start.Y;
					Vector2 value3 = new Vector2(i, j) / value * value2;
					float num8 = (new Vector2(i, j) / value * 2f - Vector2.One).Length();
					for (int k = 0; k < clusters.Count; k++)
					{
						Cluster cluster = clusters[k];
						if (Math.Abs(cluster[0].Position.X - value3.X) > 10f || Math.Abs(cluster[0].Position.Y - value3.Y) > 10f)
						{
							continue;
						}
						float num9 = 0f;
						foreach (Hub item in cluster)
						{
							num9 += 1f / Vector2.DistanceSquared(item.Position, value3);
						}
						if (num9 > num3)
						{
							if (num3 > num5)
							{
								num5 = num3;
							}
							num3 = num9;
							num4 = k;
						}
						else if (num9 > num5)
						{
							num5 = num9;
						}
					}
					float num10 = num3 + num5;
					Tile tile = GenBase._tiles[num6, num7];
					bool flag = num8 >= 0.8f;
					if (num10 > 3.5f)
					{
						tile.ClearEverything();
						tile.wall = 187;
						tile.liquid = 0;
						if (num4 % 15 == 2)
						{
							tile.ResetToType(404);
							tile.wall = 187;
							tile.active(true);
						}
						Tile.SmoothSlope(num6, num7);
					}
					else if (num10 > 1.8f)
					{
						tile.wall = 187;
						if (!flag || tile.active())
						{
							tile.ResetToType(396);
							tile.wall = 187;
							tile.active(true);
							Tile.SmoothSlope(num6, num7);
						}
						tile.liquid = 0;
					}
					else if (num10 > 0.7f || !flag)
					{
						if (!flag || tile.active())
						{
							tile.ResetToType(397);
							tile.active(true);
							Tile.SmoothSlope(num6, num7);
						}
						tile.liquid = 0;
						tile.wall = 216;
					}
					else
					{
						if (!(num10 > 0.25f))
						{
							continue;
						}
						float num11 = (num10 - 0.25f) / 0.45f;
						if (GenBase._random.NextFloat() < num11)
						{
							if (tile.active())
							{
								tile.ResetToType(397);
								tile.active(true);
								Tile.SmoothSlope(num6, num7);
								tile.wall = 216;
							}
							tile.liquid = 0;
							tile.wall = 187;
						}
					}
				}
			}
		}

		private void AddTileVariance(ClusterGroup clusters, Point start, Vector2 scale)
		{
			int num = (int)(scale.X * (float)clusters.Width);
			int num2 = (int)(scale.Y * (float)clusters.Height);
			for (int i = -20; i < num + 20; i++)
			{
				for (int j = -20; j < num2 + 20; j++)
				{
					int num3 = i + start.X;
					int num4 = j + start.Y;
					Tile tile = GenBase._tiles[num3, num4];
					Tile testTile = GenBase._tiles[num3, num4 + 1];
					Tile testTile2 = GenBase._tiles[num3, num4 + 2];
					if (tile.type == 53 && (!WorldGen.SolidTile(testTile) || !WorldGen.SolidTile(testTile2)))
					{
						tile.type = 397;
					}
				}
			}
			for (int k = -20; k < num + 20; k++)
			{
				for (int l = -20; l < num2 + 20; l++)
				{
					int num5 = k + start.X;
					int num6 = l + start.Y;
					Tile tile2 = GenBase._tiles[num5, num6];
					if (!tile2.active() || tile2.type != 396)
					{
						continue;
					}
					bool flag = true;
					for (int num7 = -1; num7 >= -3; num7--)
					{
						if (GenBase._tiles[num5, num6 + num7].active())
						{
							flag = false;
							break;
						}
					}
					bool flag2 = true;
					for (int m = 1; m <= 3; m++)
					{
						if (GenBase._tiles[num5, num6 + m].active())
						{
							flag2 = false;
							break;
						}
					}
					if ((flag ^ flag2) && GenBase._random.Next(5) == 0)
					{
						WorldGen.PlaceTile(num5, num6 + ((!flag) ? 1 : (-1)), 165, true, true);
					}
					else if (flag && GenBase._random.Next(5) == 0)
					{
						WorldGen.PlaceTile(num5, num6 - 1, 187, true, true, -1, 29 + GenBase._random.Next(6));
					}
				}
			}
		}

		private bool FindStart(Point origin, Vector2 scale, int xHubCount, int yHubCount, out Point start)
		{
			start = new Point(0, 0);
			int num = (int)(scale.X * (float)xHubCount);
			int height = (int)(scale.Y * (float)yHubCount);
			origin.X -= num >> 1;
			int num2 = 220;
			for (int i = -20; i < num + 20; i++)
			{
				for (int j = 220; j < Main.maxTilesY; j++)
				{
					if (!WorldGen.SolidTile(i + origin.X, j))
					{
						continue;
					}
					ushort type = GenBase._tiles[i + origin.X, j].type;
					if (type == 59 || type == 60)
					{
						return false;
					}
					if (j > num2)
					{
						num2 = j;
					}
					break;
				}
			}
			WorldGen.UndergroundDesertLocation = new Rectangle(origin.X, num2, num, height);
			start = new Point(origin.X, num2);
			return true;
		}

		public override bool Place(Point origin, StructureMap structures)
		{
			float num = (float)Main.maxTilesX / 4200f;
			int num2 = (int)(80f * num);
			int num3 = (int)((GenBase._random.NextFloat() + 1f) * 80f * num);
			Vector2 scale = new Vector2(4f, 2f);
			Point start;
			if (!FindStart(origin, scale, num2, num3, out start))
			{
				return false;
			}
			ClusterGroup clusterGroup = new ClusterGroup();
			clusterGroup.Generate(num2, num3);
			PlaceSand(clusterGroup, start, scale);
			PlaceClusters(clusterGroup, start, scale);
			AddTileVariance(clusterGroup, start, scale);
			int num4 = (int)(scale.X * (float)clusterGroup.Width);
			int num5 = (int)(scale.Y * (float)clusterGroup.Height);
			for (int i = -20; i < num4 + 20; i++)
			{
				for (int j = -20; j < num5 + 20; j++)
				{
					if (i + start.X > 0 && i + start.X < Main.maxTilesX - 1 && j + start.Y > 0 && j + start.Y < Main.maxTilesY - 1)
					{
						WorldGen.SquareWallFrame(i + start.X, j + start.Y);
						WorldUtils.TileFrame(i + start.X, j + start.Y, true);
					}
				}
			}
			return true;
		}
	}
}
