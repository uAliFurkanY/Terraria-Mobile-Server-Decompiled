using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	internal class CaveHouseBiome : MicroBiome
	{
		private class BuildData
		{
			public delegate void ProcessRoomMethod(Rectangle room);

			public static BuildData Snow = CreateSnowData();

			public static BuildData Jungle = CreateJungleData();

			public static BuildData Default = CreateDefaultData();

			public static BuildData Granite = CreateGraniteData();

			public static BuildData Marble = CreateMarbleData();

			public static BuildData Mushroom = CreateMushroomData();

			public static BuildData Desert = CreateDesertData();

			public ushort Tile;

			public byte Wall;

			public int PlatformStyle;

			public int DoorStyle;

			public int TableStyle;

			public int WorkbenchStyle;

			public int PianoStyle;

			public int BookcaseStyle;

			public int ChairStyle;

			public int ChestStyle;

			public ProcessRoomMethod ProcessRoom;

			public static BuildData CreateSnowData()
			{
				BuildData buildData = new BuildData();
				buildData.Tile = 321;
				buildData.Wall = 149;
				buildData.DoorStyle = 30;
				buildData.PlatformStyle = 19;
				buildData.TableStyle = 28;
				buildData.WorkbenchStyle = 23;
				buildData.PianoStyle = 23;
				buildData.BookcaseStyle = 25;
				buildData.ChairStyle = 30;
				buildData.ChestStyle = 11;
				buildData.ProcessRoom = AgeSnowRoom;
				return buildData;
			}

			public static BuildData CreateDesertData()
			{
				BuildData buildData = new BuildData();
				buildData.Tile = 396;
				buildData.Wall = 187;
				buildData.PlatformStyle = 0;
				buildData.DoorStyle = 0;
				buildData.TableStyle = 0;
				buildData.WorkbenchStyle = 0;
				buildData.PianoStyle = 0;
				buildData.BookcaseStyle = 0;
				buildData.ChairStyle = 0;
				buildData.ChestStyle = 1;
				buildData.ProcessRoom = AgeDesertRoom;
				return buildData;
			}

			public static BuildData CreateJungleData()
			{
				BuildData buildData = new BuildData();
				buildData.Tile = 158;
				buildData.Wall = 42;
				buildData.PlatformStyle = 2;
				buildData.DoorStyle = 2;
				buildData.TableStyle = 2;
				buildData.WorkbenchStyle = 2;
				buildData.PianoStyle = 2;
				buildData.BookcaseStyle = 12;
				buildData.ChairStyle = 3;
				buildData.ChestStyle = 8;
				buildData.ProcessRoom = AgeJungleRoom;
				return buildData;
			}

			public static BuildData CreateGraniteData()
			{
				BuildData buildData = new BuildData();
				buildData.Tile = 369;
				buildData.Wall = 181;
				buildData.PlatformStyle = 28;
				buildData.DoorStyle = 34;
				buildData.TableStyle = 33;
				buildData.WorkbenchStyle = 29;
				buildData.PianoStyle = 28;
				buildData.BookcaseStyle = 30;
				buildData.ChairStyle = 34;
				buildData.ChestStyle = 50;
				buildData.ProcessRoom = AgeGraniteRoom;
				return buildData;
			}

			public static BuildData CreateMarbleData()
			{
				BuildData buildData = new BuildData();
				buildData.Tile = 357;
				buildData.Wall = 179;
				buildData.PlatformStyle = 29;
				buildData.DoorStyle = 35;
				buildData.TableStyle = 34;
				buildData.WorkbenchStyle = 30;
				buildData.PianoStyle = 29;
				buildData.BookcaseStyle = 31;
				buildData.ChairStyle = 35;
				buildData.ChestStyle = 51;
				buildData.ProcessRoom = AgeMarbleRoom;
				return buildData;
			}

			public static BuildData CreateMushroomData()
			{
				BuildData buildData = new BuildData();
				buildData.Tile = 190;
				buildData.Wall = 74;
				buildData.PlatformStyle = 18;
				buildData.DoorStyle = 6;
				buildData.TableStyle = 27;
				buildData.WorkbenchStyle = 7;
				buildData.PianoStyle = 22;
				buildData.BookcaseStyle = 24;
				buildData.ChairStyle = 9;
				buildData.ChestStyle = 32;
				buildData.ProcessRoom = AgeMushroomRoom;
				return buildData;
			}

			public static BuildData CreateDefaultData()
			{
				BuildData buildData = new BuildData();
				buildData.Tile = 30;
				buildData.Wall = 27;
				buildData.PlatformStyle = 0;
				buildData.DoorStyle = 0;
				buildData.TableStyle = 0;
				buildData.WorkbenchStyle = 0;
				buildData.PianoStyle = 0;
				buildData.BookcaseStyle = 0;
				buildData.ChairStyle = 0;
				buildData.ChestStyle = 1;
				buildData.ProcessRoom = AgeDefaultRoom;
				return buildData;
			}
		}

		private const int VERTICAL_EXIT_WIDTH = 3;

		private static readonly bool[] _blacklistedTiles = TileID.Sets.Factory.CreateBoolSet(true, 225, 41, 43, 44, 226, 203, 112, 25, 151);

		private int _sharpenerCount;

		private int _extractinatorCount;

		private Rectangle GetRoom(Point origin)
		{
			Point result;
			bool flag = WorldUtils.Find(origin, Searches.Chain(new Searches.Left(25), new Conditions.IsSolid()), out result);
			Point result2;
			bool flag2 = WorldUtils.Find(origin, Searches.Chain(new Searches.Right(25), new Conditions.IsSolid()), out result2);
			if (!flag)
			{
				result = new Point(origin.X - 25, origin.Y);
			}
			if (!flag2)
			{
				result2 = new Point(origin.X + 25, origin.Y);
			}
			Rectangle result3 = new Rectangle(origin.X, origin.Y, 0, 0);
			if (origin.X - result.X > result2.X - origin.X)
			{
				result3.X = result.X;
				result3.Width = Utils.Clamp(result2.X - result.X, 15, 30);
			}
			else
			{
				result3.Width = Utils.Clamp(result2.X - result.X, 15, 30);
				result3.X = result2.X - result3.Width;
			}
			Point result4;
			bool flag3 = WorldUtils.Find(result, Searches.Chain(new Searches.Up(10), new Conditions.IsSolid()), out result4);
			Point result5;
			bool flag4 = WorldUtils.Find(result2, Searches.Chain(new Searches.Up(10), new Conditions.IsSolid()), out result5);
			if (!flag3)
			{
				result4 = new Point(origin.X, origin.Y - 10);
			}
			if (!flag4)
			{
				result5 = new Point(origin.X, origin.Y - 10);
			}
			result3.Height = Utils.Clamp(Math.Max(origin.Y - result4.Y, origin.Y - result5.Y), 8, 12);
			result3.Y -= result3.Height;
			return result3;
		}

		private float RoomSolidPrecentage(Rectangle room)
		{
			float num = room.Width * room.Height;
			Ref<int> @ref = new Ref<int>(0);
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.IsSolid(), new Actions.Count(@ref)));
			return (float)@ref.Value / num;
		}

		private bool FindVerticalExit(Rectangle wall, bool isUp, out int exitX)
		{
			Point result;
			bool result2 = WorldUtils.Find(new Point(wall.X + wall.Width - 3, wall.Y + (isUp ? (-5) : 0)), Searches.Chain(new Searches.Left(wall.Width - 3), new Conditions.IsSolid().Not().AreaOr(3, 5)), out result);
			exitX = result.X;
			return result2;
		}

		private bool FindSideExit(Rectangle wall, bool isLeft, out int exitY)
		{
			Point result;
			bool result2 = WorldUtils.Find(new Point(wall.X + (isLeft ? (-4) : 0), wall.Y + wall.Height - 3), Searches.Chain(new Searches.Up(wall.Height - 3), new Conditions.IsSolid().Not().AreaOr(4, 3)), out result);
			exitY = result.Y;
			return result2;
		}

		private int SortBiomeResults(Tuple<BuildData, int> item1, Tuple<BuildData, int> item2)
		{
			return item2.Item2.CompareTo(item1.Item2);
		}

		public override bool Place(Point origin, StructureMap structures)
		{
			Point result;
			if (!WorldUtils.Find(origin, Searches.Chain(new Searches.Down(200), new Conditions.IsSolid()), out result) || result == origin)
			{
				return false;
			}
			Rectangle room = GetRoom(result);
			Rectangle rectangle = GetRoom(new Point(room.Center.X, room.Y + 1));
			Rectangle rectangle2 = GetRoom(new Point(room.Center.X, room.Y + room.Height + 10));
			rectangle2.Y = room.Y + room.Height - 1;
			float num = RoomSolidPrecentage(rectangle);
			float num2 = RoomSolidPrecentage(rectangle2);
			room.Y += 3;
			rectangle.Y += 3;
			rectangle2.Y += 3;
			List<Rectangle> list = new List<Rectangle>();
			if (GenBase._random.NextFloat() > num + 0.2f)
			{
				list.Add(rectangle);
			}
			else
			{
				rectangle = room;
			}
			list.Add(room);
			if (GenBase._random.NextFloat() > num2 + 0.2f)
			{
				list.Add(rectangle2);
			}
			else
			{
				rectangle2 = room;
			}
			Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
			foreach (Rectangle item in list)
			{
				WorldUtils.Gen(new Point(item.X - 5, item.Y - 5), new Shapes.Rectangle(item.Width + 10, item.Height + 10), new Actions.TileScanner(0, 59, 147, 1, 161, 53, 396, 397, 368, 367, 60, 70).Output(dictionary));
			}
			List<Tuple<BuildData, int>> list2 = new List<Tuple<BuildData, int>>();
			list2.Add(Tuple.Create(BuildData.Default, dictionary[0] + dictionary[1]));
			list2.Add(Tuple.Create(BuildData.Jungle, dictionary[59] + dictionary[60] * 10));
			list2.Add(Tuple.Create(BuildData.Mushroom, dictionary[59] + dictionary[70] * 10));
			list2.Add(Tuple.Create(BuildData.Snow, dictionary[147] + dictionary[161]));
			list2.Add(Tuple.Create(BuildData.Desert, dictionary[397] + dictionary[396] + dictionary[53]));
			list2.Add(Tuple.Create(BuildData.Granite, dictionary[368]));
			list2.Add(Tuple.Create(BuildData.Marble, dictionary[367]));
			list2.Sort(SortBiomeResults);
			BuildData ıtem = list2[0].Item1;
			foreach (Rectangle item2 in list)
			{
				Point result2;
				if (ıtem != BuildData.Granite && WorldUtils.Find(new Point(item2.X - 2, item2.Y - 2), Searches.Chain(new Searches.Rectangle(item2.Width + 4, item2.Height + 4).RequireAll(false), new Conditions.HasLava()), out result2))
				{
					return false;
				}
				if (!structures.CanPlace(item2, _blacklistedTiles, 5))
				{
					return false;
				}
			}
			int num3 = room.X;
			int num4 = room.X + room.Width - 1;
			List<Rectangle> list3 = new List<Rectangle>();
			foreach (Rectangle item3 in list)
			{
				num3 = Math.Min(num3, item3.X);
				num4 = Math.Max(num4, item3.X + item3.Width - 1);
			}
			int num5 = 6;
			while (num5 > 4 && (num4 - num3) % num5 != 0)
			{
				num5--;
			}
			for (int i = num3; i <= num4; i += num5)
			{
				for (int j = 0; j < list.Count; j++)
				{
					Rectangle rectangle3 = list[j];
					if (i < rectangle3.X || i >= rectangle3.X + rectangle3.Width)
					{
						continue;
					}
					int num6 = rectangle3.Y + rectangle3.Height;
					int num7 = 50;
					for (int k = j + 1; k < list.Count; k++)
					{
						if (i >= list[k].X && i < list[k].X + list[k].Width)
						{
							num7 = Math.Min(num7, list[k].Y - num6);
						}
					}
					if (num7 > 0)
					{
						Point result3;
						bool flag = WorldUtils.Find(new Point(i, num6), Searches.Chain(new Searches.Down(num7), new Conditions.IsSolid()), out result3);
						if (num7 < 50)
						{
							flag = true;
							result3 = new Point(i, num6 + num7);
						}
						if (flag)
						{
							list3.Add(new Rectangle(i, num6, 1, result3.Y - num6));
						}
					}
				}
			}
			List<Point> list4 = new List<Point>();
			foreach (Rectangle item4 in list)
			{
				int exitY;
				if (FindSideExit(new Rectangle(item4.X + item4.Width, item4.Y + 1, 1, item4.Height - 2), false, out exitY))
				{
					list4.Add(new Point(item4.X + item4.Width - 1, exitY));
				}
				if (FindSideExit(new Rectangle(item4.X, item4.Y + 1, 1, item4.Height - 2), true, out exitY))
				{
					list4.Add(new Point(item4.X, exitY));
				}
			}
			List<Tuple<Point, Point>> list5 = new List<Tuple<Point, Point>>();
			for (int l = 1; l < list.Count; l++)
			{
				Rectangle rectangle4 = list[l];
				Rectangle rectangle5 = list[l - 1];
				int num8 = rectangle5.X - rectangle4.X;
				int num9 = rectangle4.X + rectangle4.Width - (rectangle5.X + rectangle5.Width);
				if (num8 > num9)
				{
					list5.Add(new Tuple<Point, Point>(new Point(rectangle4.X + rectangle4.Width - 1, rectangle4.Y + 1), new Point(rectangle4.X + rectangle4.Width - rectangle4.Height + 1, rectangle4.Y + rectangle4.Height - 1)));
				}
				else
				{
					list5.Add(new Tuple<Point, Point>(new Point(rectangle4.X, rectangle4.Y + 1), new Point(rectangle4.X + rectangle4.Height - 1, rectangle4.Y + rectangle4.Height - 1)));
				}
			}
			List<Point> list6 = new List<Point>();
			int exitX;
			if (FindVerticalExit(new Rectangle(rectangle.X + 2, rectangle.Y, rectangle.Width - 4, 1), true, out exitX))
			{
				list6.Add(new Point(exitX, rectangle.Y));
			}
			if (FindVerticalExit(new Rectangle(rectangle2.X + 2, rectangle2.Y + rectangle2.Height - 1, rectangle2.Width - 4, 1), false, out exitX))
			{
				list6.Add(new Point(exitX, rectangle2.Y + rectangle2.Height - 1));
			}
			foreach (Rectangle item5 in list)
			{
				WorldUtils.Gen(new Point(item5.X, item5.Y), new Shapes.Rectangle(item5.Width, item5.Height), Actions.Chain(new Actions.SetTile(ıtem.Tile), new Actions.SetFrames(true)));
				WorldUtils.Gen(new Point(item5.X + 1, item5.Y + 1), new Shapes.Rectangle(item5.Width - 2, item5.Height - 2), Actions.Chain(new Actions.ClearTile(true), new Actions.PlaceWall(ıtem.Wall)));
				structures.AddStructure(item5, 8);
			}
			foreach (Tuple<Point, Point> item6 in list5)
			{
				Point ıtem2 = item6.Item1;
				Point ıtem3 = item6.Item2;
				int num10 = (ıtem3.X > ıtem2.X) ? 1 : (-1);
				ShapeData shapeData = new ShapeData();
				for (int m = 0; m < ıtem3.Y - ıtem2.Y; m++)
				{
					shapeData.Add(num10 * (m + 1), m);
				}
				WorldUtils.Gen(ıtem2, new ModShapes.All(shapeData), Actions.Chain(new Actions.PlaceTile(19, ıtem.PlatformStyle), new Actions.SetSlope((num10 == 1) ? 1 : 2), new Actions.SetFrames(true)));
				WorldUtils.Gen(new Point(ıtem2.X + ((num10 == 1) ? 1 : (-4)), ıtem2.Y - 1), new Shapes.Rectangle(4, 1), Actions.Chain(new Actions.Clear(), new Actions.PlaceWall(ıtem.Wall), new Actions.PlaceTile(19, ıtem.PlatformStyle), new Actions.SetFrames(true)));
			}
			foreach (Point item7 in list4)
			{
				WorldUtils.Gen(item7, new Shapes.Rectangle(1, 3), new Actions.ClearTile(true));
				WorldGen.PlaceTile(item7.X, item7.Y, 10, true, true, -1, ıtem.DoorStyle);
			}
			foreach (Point item8 in list6)
			{
				WorldUtils.Gen(item8, new Shapes.Rectangle(3, 1), Actions.Chain(new Actions.ClearMetadata(), new Actions.PlaceTile(19, ıtem.PlatformStyle), new Actions.SetFrames(true)));
			}
			foreach (Rectangle item9 in list3)
			{
				if (item9.Height > 1 && GenBase._tiles[item9.X, item9.Y - 1].type != 19)
				{
					WorldUtils.Gen(new Point(item9.X, item9.Y), new Shapes.Rectangle(item9.Width, item9.Height), Actions.Chain(new Actions.SetTile(124), new Actions.SetFrames(true)));
					Tile tile = GenBase._tiles[item9.X, item9.Y + item9.Height];
					tile.slope(0);
					tile.halfBrick(false);
				}
			}
			Point[] choices = new Point[7]
			{
				new Point(14, ıtem.TableStyle),
				new Point(16, 0),
				new Point(18, ıtem.WorkbenchStyle),
				new Point(86, 0),
				new Point(87, ıtem.PianoStyle),
				new Point(94, 0),
				new Point(101, ıtem.BookcaseStyle)
			};
			foreach (Rectangle item10 in list)
			{
				int num11 = item10.Width / 8;
				int num12 = item10.Width / (num11 + 1);
				int num13 = GenBase._random.Next(2);
				for (int n = 0; n < num11; n++)
				{
					int num14 = (n + 1) * num12 + item10.X;
					switch (n + num13 % 2)
					{
					case 0:
					{
						int num15 = item10.Y + Math.Min(item10.Height / 2, item10.Height - 5);
						Vector2 vector = WorldGen.randHousePicture();
						int type = (int)vector.X;
						int style = (int)vector.Y;
						if (!WorldGen.nearPicture(num14, num15))
						{
							WorldGen.PlaceTile(num14, num15, type, true, false, -1, style);
						}
						break;
					}
					case 1:
					{
						int num15 = item10.Y + 1;
						WorldGen.PlaceTile(num14, num15, 34, true, false, -1, GenBase._random.Next(6));
						for (int num16 = -1; num16 < 2; num16++)
						{
							for (int num17 = 0; num17 < 3; num17++)
							{
								GenBase._tiles[num16 + num14, num17 + num15].frameX += 54;
							}
						}
						break;
					}
					}
				}
				int num18 = item10.Width / 8 + 3;
				WorldGen.SetupStatueList();
				while (num18 > 0)
				{
					int num19 = GenBase._random.Next(item10.Width - 3) + 1 + item10.X;
					int num20 = item10.Y + item10.Height - 2;
					switch (GenBase._random.Next(4))
					{
					case 0:
						WorldGen.PlaceSmallPile(num19, num20, GenBase._random.Next(31, 34), 1, 185);
						break;
					case 1:
						WorldGen.PlaceTile(num19, num20, 186, true, false, -1, GenBase._random.Next(22, 26));
						break;
					case 2:
					{
						int num21 = GenBase._random.Next(2, WorldGen.statueList.Length);
						WorldGen.PlaceTile(num19, num20, WorldGen.statueList[num21].X, true, false, -1, WorldGen.statueList[num21].Y);
						if (WorldGen.StatuesWithTraps.Contains(num21))
						{
							WorldGen.PlaceStatueTrap(num19, num20);
						}
						break;
					}
					case 3:
					{
						Point point = Utils.SelectRandom(GenBase._random, choices);
						WorldGen.PlaceTile(num19, num20, point.X, true, false, -1, point.Y);
						break;
					}
					}
					num18--;
				}
			}
			foreach (Rectangle item11 in list)
			{
				ıtem.ProcessRoom(item11);
			}
			bool flag2 = false;
			foreach (Rectangle item12 in list)
			{
				int num22 = item12.Height - 1 + item12.Y;
				int style2 = (num22 > (int)Main.worldSurface) ? ıtem.ChestStyle : 0;
				for (int num23 = 0; num23 < 10; num23++)
				{
					int i2 = GenBase._random.Next(2, item12.Width - 2) + item12.X;
					if (flag2 = WorldGen.AddBuriedChest(i2, num22, 0, false, style2))
					{
						break;
					}
				}
				if (flag2)
				{
					break;
				}
				for (int num24 = item12.X + 2; num24 <= item12.X + item12.Width - 2; num24++)
				{
					if (flag2 = WorldGen.AddBuriedChest(num24, num22, 0, false, style2))
					{
						break;
					}
				}
				if (flag2)
				{
					break;
				}
			}
			if (!flag2)
			{
				foreach (Rectangle item13 in list)
				{
					int num25 = item13.Y - 1;
					int style3 = (num25 > (int)Main.worldSurface) ? ıtem.ChestStyle : 0;
					for (int num26 = 0; num26 < 10; num26++)
					{
						int i3 = GenBase._random.Next(2, item13.Width - 2) + item13.X;
						if (flag2 = WorldGen.AddBuriedChest(i3, num25, 0, false, style3))
						{
							break;
						}
					}
					if (flag2)
					{
						break;
					}
					for (int num27 = item13.X + 2; num27 <= item13.X + item13.Width - 2; num27++)
					{
						if (flag2 = WorldGen.AddBuriedChest(num27, num25, 0, false, style3))
						{
							break;
						}
					}
					if (flag2)
					{
						break;
					}
				}
			}
			if (!flag2)
			{
				for (int num28 = 0; num28 < 1000; num28++)
				{
					int i4 = GenBase._random.Next(list[0].X - 30, list[0].X + 30);
					int num29 = GenBase._random.Next(list[0].Y - 30, list[0].Y + 30);
					int style4 = (num29 > (int)Main.worldSurface) ? ıtem.ChestStyle : 0;
					if (flag2 = WorldGen.AddBuriedChest(i4, num29, 0, false, style4))
					{
						break;
					}
				}
			}
			if (ıtem == BuildData.Jungle && _sharpenerCount < GenBase._random.Next(2, 5))
			{
				bool flag3 = false;
				foreach (Rectangle item14 in list)
				{
					int num30 = item14.Height - 2 + item14.Y;
					for (int num31 = 0; num31 < 10; num31++)
					{
						int num32 = GenBase._random.Next(2, item14.Width - 2) + item14.X;
						WorldGen.PlaceTile(num32, num30, 377, true, true);
						if (flag3 = (GenBase._tiles[num32, num30].active() && GenBase._tiles[num32, num30].type == 377))
						{
							break;
						}
					}
					if (flag3)
					{
						break;
					}
					for (int num33 = item14.X + 2; num33 <= item14.X + item14.Width - 2; num33++)
					{
						if (flag3 = WorldGen.PlaceTile(num33, num30, 377, true, true))
						{
							break;
						}
					}
					if (flag3)
					{
						break;
					}
				}
				if (flag3)
				{
					_sharpenerCount++;
				}
			}
			if (ıtem == BuildData.Desert && _extractinatorCount < GenBase._random.Next(2, 5))
			{
				bool flag4 = false;
				foreach (Rectangle item15 in list)
				{
					int num34 = item15.Height - 2 + item15.Y;
					for (int num35 = 0; num35 < 10; num35++)
					{
						int num36 = GenBase._random.Next(2, item15.Width - 2) + item15.X;
						WorldGen.PlaceTile(num36, num34, 219, true, true);
						if (flag4 = (GenBase._tiles[num36, num34].active() && GenBase._tiles[num36, num34].type == 219))
						{
							break;
						}
					}
					if (flag4)
					{
						break;
					}
					for (int num37 = item15.X + 2; num37 <= item15.X + item15.Width - 2; num37++)
					{
						if (flag4 = WorldGen.PlaceTile(num37, num34, 219, true, true))
						{
							break;
						}
					}
					if (flag4)
					{
						break;
					}
				}
				if (flag4)
				{
					_extractinatorCount++;
				}
			}
			return true;
		}

		public override void Reset()
		{
			_sharpenerCount = 0;
			_extractinatorCount = 0;
		}

		internal static void AgeDefaultRoom(Rectangle room)
		{
			for (int i = 0; i < room.Width * room.Height / 16; i++)
			{
				int x = GenBase._random.Next(1, room.Width - 1) + room.X;
				int y = GenBase._random.Next(1, room.Height - 1) + room.Y;
				WorldUtils.Gen(new Point(x, y), new Shapes.Rectangle(2, 2), Actions.Chain(new Modifiers.Dither(), new Modifiers.Blotches(2, 2.0), new Modifiers.IsEmpty(), new Actions.SetTile(51, true)));
			}
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.85000002384185791), new Modifiers.Blotches(), new Modifiers.OnlyWalls(BuildData.Default.Wall), ((double)room.Y > Main.worldSurface) ? ((GenAction)new Actions.ClearWall(true)) : ((GenAction)new Actions.PlaceWall(2))));
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.949999988079071), new Modifiers.OnlyTiles(30, 321, 158), new Actions.ClearTile(true)));
		}

		internal static void AgeSnowRoom(Rectangle room)
		{
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.60000002384185791), new Modifiers.Blotches(2, 0.60000002384185791), new Modifiers.OnlyTiles(BuildData.Snow.Tile), new Actions.SetTile(161, true), new Modifiers.Dither(0.8), new Actions.SetTile(147, true)));
			WorldUtils.Gen(new Point(room.X + 1, room.Y), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(), new Modifiers.OnlyTiles(161), new Modifiers.Offset(0, 1), new ActionStalagtite()));
			WorldUtils.Gen(new Point(room.X + 1, room.Y + room.Height - 1), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(), new Modifiers.OnlyTiles(161), new Modifiers.Offset(0, 1), new ActionStalagtite()));
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.85000002384185791), new Modifiers.Blotches(2, 0.8), ((double)room.Y > Main.worldSurface) ? ((GenAction)new Actions.ClearWall(true)) : ((GenAction)new Actions.PlaceWall(40))));
		}

		internal static void AgeDesertRoom(Rectangle room)
		{
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.800000011920929), new Modifiers.Blotches(2, 0.20000000298023224), new Modifiers.OnlyTiles(BuildData.Desert.Tile), new Actions.SetTile(396, true), new Modifiers.Dither(), new Actions.SetTile(397, true)));
			WorldUtils.Gen(new Point(room.X + 1, room.Y), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(), new Modifiers.OnlyTiles(397, 396), new Modifiers.Offset(0, 1), new ActionStalagtite()));
			WorldUtils.Gen(new Point(room.X + 1, room.Y + room.Height - 1), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(), new Modifiers.OnlyTiles(397, 396), new Modifiers.Offset(0, 1), new ActionStalagtite()));
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.800000011920929), new Modifiers.Blotches(), new Modifiers.OnlyWalls(BuildData.Desert.Wall), new Actions.PlaceWall(216)));
		}

		internal static void AgeGraniteRoom(Rectangle room)
		{
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.60000002384185791), new Modifiers.Blotches(2, 0.60000002384185791), new Modifiers.OnlyTiles(BuildData.Granite.Tile), new Actions.SetTile(368, true)));
			WorldUtils.Gen(new Point(room.X + 1, room.Y), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(0.800000011920929), new Modifiers.OnlyTiles(368), new Modifiers.Offset(0, 1), new ActionStalagtite()));
			WorldUtils.Gen(new Point(room.X + 1, room.Y + room.Height - 1), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(0.800000011920929), new Modifiers.OnlyTiles(368), new Modifiers.Offset(0, 1), new ActionStalagtite()));
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.85000002384185791), new Modifiers.Blotches(), new Actions.PlaceWall(180)));
		}

		internal static void AgeMarbleRoom(Rectangle room)
		{
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.60000002384185791), new Modifiers.Blotches(2, 0.60000002384185791), new Modifiers.OnlyTiles(BuildData.Marble.Tile), new Actions.SetTile(367, true)));
			WorldUtils.Gen(new Point(room.X + 1, room.Y), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(0.800000011920929), new Modifiers.OnlyTiles(367), new Modifiers.Offset(0, 1), new ActionStalagtite()));
			WorldUtils.Gen(new Point(room.X + 1, room.Y + room.Height - 1), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(0.800000011920929), new Modifiers.OnlyTiles(367), new Modifiers.Offset(0, 1), new ActionStalagtite()));
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.85000002384185791), new Modifiers.Blotches(), new Actions.PlaceWall(178)));
		}

		internal static void AgeMushroomRoom(Rectangle room)
		{
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.699999988079071), new Modifiers.Blotches(2, 0.5), new Modifiers.OnlyTiles(BuildData.Mushroom.Tile), new Actions.SetTile(70, true)));
			WorldUtils.Gen(new Point(room.X + 1, room.Y), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(0.60000002384185791), new Modifiers.OnlyTiles(70), new Modifiers.Offset(0, -1), new Actions.SetTile(71)));
			WorldUtils.Gen(new Point(room.X + 1, room.Y + room.Height - 1), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(0.60000002384185791), new Modifiers.OnlyTiles(70), new Modifiers.Offset(0, -1), new Actions.SetTile(71)));
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.85000002384185791), new Modifiers.Blotches(), new Actions.ClearWall()));
		}

		internal static void AgeJungleRoom(Rectangle room)
		{
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.60000002384185791), new Modifiers.Blotches(2, 0.60000002384185791), new Modifiers.OnlyTiles(BuildData.Jungle.Tile), new Actions.SetTile(60, true), new Modifiers.Dither(0.800000011920929), new Actions.SetTile(59, true)));
			WorldUtils.Gen(new Point(room.X + 1, room.Y), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(), new Modifiers.OnlyTiles(60), new Modifiers.Offset(0, 1), new ActionVines(3, room.Height, 62)));
			WorldUtils.Gen(new Point(room.X + 1, room.Y + room.Height - 1), new Shapes.Rectangle(room.Width - 2, 1), Actions.Chain(new Modifiers.Dither(), new Modifiers.OnlyTiles(60), new Modifiers.Offset(0, 1), new ActionVines(3, room.Height, 62)));
			WorldUtils.Gen(new Point(room.X, room.Y), new Shapes.Rectangle(room.Width, room.Height), Actions.Chain(new Modifiers.Dither(0.85000002384185791), new Modifiers.Blotches(), new Actions.PlaceWall(64)));
		}
	}
}
