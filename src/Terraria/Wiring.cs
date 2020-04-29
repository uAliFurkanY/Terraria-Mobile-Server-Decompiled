using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;

namespace Terraria
{
	public static class Wiring
	{
		private const int MaxPump = 20;

		private const int MaxMech = 1000;

		public static bool running;

		private static Dictionary<Point16, bool> _wireSkip;

		private static DoubleStack<Point16> _wireList;

		private static Dictionary<Point16, byte> _toProcess;

		private static Vector2[] _teleport;

		private static int[] _inPumpX;

		private static int[] _inPumpY;

		private static int _numInPump;

		private static int[] _outPumpX;

		private static int[] _outPumpY;

		private static int _numOutPump;

		private static int[] _mechX;

		private static int[] _mechY;

		private static int _numMechs;

		private static int[] _mechTime;

		public static void Initialize()
		{
			_wireSkip = new Dictionary<Point16, bool>();
			_wireList = new DoubleStack<Point16>();
			_toProcess = new Dictionary<Point16, byte>();
			_inPumpX = new int[20];
			_inPumpY = new int[20];
			_outPumpX = new int[20];
			_outPumpY = new int[20];
			_teleport = new Vector2[2];
			_mechX = new int[1000];
			_mechY = new int[1000];
			_mechTime = new int[1000];
		}

		public static void SkipWire(int x, int y)
		{
			_wireSkip[new Point16(x, y)] = true;
		}

		public static void SkipWire(Point16 point)
		{
			_wireSkip[point] = true;
		}

		public static void UpdateMech()
		{
			for (int num = _numMechs - 1; num >= 0; num--)
			{
				_mechTime[num]--;
				if (Main.tile[_mechX[num], _mechY[num]].active() && Main.tile[_mechX[num], _mechY[num]].type == 144)
				{
					if (Main.tile[_mechX[num], _mechY[num]].frameY == 0)
					{
						_mechTime[num] = 0;
					}
					else
					{
						int num2 = Main.tile[_mechX[num], _mechY[num]].frameX / 18;
						switch (num2)
						{
						case 0:
							num2 = 60;
							break;
						case 1:
							num2 = 180;
							break;
						case 2:
							num2 = 300;
							break;
						}
						if (Math.IEEERemainder(_mechTime[num], num2) == 0.0)
						{
							_mechTime[num] = 18000;
							TripWire(_mechX[num], _mechY[num], 1, 1);
						}
					}
				}
				if (_mechTime[num] <= 0)
				{
					if (Main.tile[_mechX[num], _mechY[num]].active() && Main.tile[_mechX[num], _mechY[num]].type == 144)
					{
						Main.tile[_mechX[num], _mechY[num]].frameY = 0;
						NetMessage.SendTileSquare(-1, _mechX[num], _mechY[num], 1);
					}
					if (Main.tile[_mechX[num], _mechY[num]].active() && Main.tile[_mechX[num], _mechY[num]].type == 411)
					{
						Tile tile = Main.tile[_mechX[num], _mechY[num]];
						int num3 = tile.frameX % 36 / 18;
						int num4 = tile.frameY % 36 / 18;
						int num5 = _mechX[num] - num3;
						int num6 = _mechY[num] - num4;
						int num7 = 36;
						if (Main.tile[num5, num6].frameX >= 36)
						{
							num7 = -36;
						}
						for (int i = num5; i < num5 + 2; i++)
						{
							for (int j = num6; j < num6 + 2; j++)
							{
								Main.tile[i, j].frameX = (short)(Main.tile[i, j].frameX + num7);
							}
						}
						NetMessage.SendTileSquare(-1, num5, num6, 2);
					}
					for (int k = num; k < _numMechs; k++)
					{
						_mechX[k] = _mechX[k + 1];
						_mechY[k] = _mechY[k + 1];
						_mechTime[k] = _mechTime[k + 1];
					}
					_numMechs--;
				}
			}
		}

		public static void HitSwitch(int i, int j)
		{
			if (Main.tile[i, j] == null)
			{
				return;
			}
			if (Main.tile[i, j].type == 135 || Main.tile[i, j].type == 314)
			{
				Main.PlaySound(28, i * 16, j * 16, 0);
				TripWire(i, j, 1, 1);
			}
			else if (Main.tile[i, j].type == 136)
			{
				if (Main.tile[i, j].frameY == 0)
				{
					Main.tile[i, j].frameY = 18;
				}
				else
				{
					Main.tile[i, j].frameY = 0;
				}
				Main.PlaySound(28, i * 16, j * 16, 0);
				TripWire(i, j, 1, 1);
			}
			else if (Main.tile[i, j].type == 144)
			{
				if (Main.tile[i, j].frameY == 0)
				{
					Main.tile[i, j].frameY = 18;
					if (Main.netMode != 1)
					{
						CheckMech(i, j, 18000);
					}
				}
				else
				{
					Main.tile[i, j].frameY = 0;
				}
				Main.PlaySound(28, i * 16, j * 16, 0);
			}
			else
			{
				if (Main.tile[i, j].type != 132 && Main.tile[i, j].type != 411)
				{
					return;
				}
				short num = 36;
				int num2 = Main.tile[i, j].frameX / 18 * -1;
				int num3 = Main.tile[i, j].frameY / 18 * -1;
				num2 %= 4;
				if (num2 < -1)
				{
					num2 += 2;
					num = -36;
				}
				num2 += i;
				num3 += j;
				if (Main.netMode != 1 && Main.tile[num2, num3].type == 411)
				{
					CheckMech(num2, num3, 60);
				}
				for (int k = num2; k < num2 + 2; k++)
				{
					for (int l = num3; l < num3 + 2; l++)
					{
						if (Main.tile[k, l].type == 132 || Main.tile[k, l].type == 411)
						{
							Main.tile[k, l].frameX += num;
						}
					}
				}
				WorldGen.TileFrame(num2, num3);
				Main.PlaySound(28, i * 16, j * 16, 0);
				TripWire(num2, num3, 2, 2);
			}
		}

		private static bool CheckMech(int i, int j, int time)
		{
			for (int k = 0; k < _numMechs; k++)
			{
				if (_mechX[k] == i && _mechY[k] == j)
				{
					return false;
				}
			}
			if (_numMechs < 999)
			{
				_mechX[_numMechs] = i;
				_mechY[_numMechs] = j;
				_mechTime[_numMechs] = time;
				_numMechs++;
				return true;
			}
			return false;
		}

		private static void XferWater()
		{
			for (int i = 0; i < _numInPump; i++)
			{
				int num = _inPumpX[i];
				int num2 = _inPumpY[i];
				int liquid = Main.tile[num, num2].liquid;
				if (liquid <= 0)
				{
					continue;
				}
				bool flag = Main.tile[num, num2].lava();
				bool flag2 = Main.tile[num, num2].honey();
				for (int j = 0; j < _numOutPump; j++)
				{
					int num3 = _outPumpX[j];
					int num4 = _outPumpY[j];
					int liquid2 = Main.tile[num3, num4].liquid;
					if (liquid2 >= 255)
					{
						continue;
					}
					bool flag3 = Main.tile[num3, num4].lava();
					bool flag4 = Main.tile[num3, num4].honey();
					if (liquid2 == 0)
					{
						flag3 = flag;
						flag4 = flag2;
					}
					if (flag == flag3 && flag2 == flag4)
					{
						int num5 = liquid;
						if (num5 + liquid2 > 255)
						{
							num5 = 255 - liquid2;
						}
						Main.tile[num3, num4].liquid += (byte)num5;
						Main.tile[num, num2].liquid -= (byte)num5;
						liquid = Main.tile[num, num2].liquid;
						Main.tile[num3, num4].lava(flag);
						Main.tile[num3, num4].honey(flag2);
						WorldGen.SquareTileFrame(num3, num4);
						if (Main.tile[num, num2].liquid == 0)
						{
							Main.tile[num, num2].lava(false);
							WorldGen.SquareTileFrame(num, num2);
							break;
						}
					}
				}
				WorldGen.SquareTileFrame(num, num2);
			}
		}

		private static void TripWire(int left, int top, int width, int height)
		{
			if (Main.netMode == 1)
			{
				return;
			}
			running = true;
			if (_wireList.Count != 0)
			{
				_wireList.Clear(true);
			}
			for (int i = left; i < left + width; i++)
			{
				for (int j = top; j < top + height; j++)
				{
					Point16 back = new Point16(i, j);
					Tile tile = Main.tile[i, j];
					if (tile != null && tile.wire())
					{
						_wireList.PushBack(back);
					}
				}
			}
			Vector2[] array = new Vector2[6];
			_teleport[0].X = -1f;
			_teleport[0].Y = -1f;
			_teleport[1].X = -1f;
			_teleport[1].Y = -1f;
			if (_wireList.Count > 0)
			{
				_numInPump = 0;
				_numOutPump = 0;
				HitWire(_wireList, 1);
				if (_numInPump > 0 && _numOutPump > 0)
				{
					XferWater();
				}
			}
			for (int k = left; k < left + width; k++)
			{
				for (int l = top; l < top + height; l++)
				{
					Point16 back = new Point16(k, l);
					Tile tile2 = Main.tile[k, l];
					if (tile2 != null && tile2.wire2())
					{
						_wireList.PushBack(back);
					}
				}
			}
			array[0] = _teleport[0];
			array[1] = _teleport[1];
			_teleport[0].X = -1f;
			_teleport[0].Y = -1f;
			_teleport[1].X = -1f;
			_teleport[1].Y = -1f;
			if (_wireList.Count > 0)
			{
				_numInPump = 0;
				_numOutPump = 0;
				HitWire(_wireList, 2);
				if (_numInPump > 0 && _numOutPump > 0)
				{
					XferWater();
				}
			}
			array[2] = _teleport[0];
			array[3] = _teleport[1];
			_teleport[0].X = -1f;
			_teleport[0].Y = -1f;
			_teleport[1].X = -1f;
			_teleport[1].Y = -1f;
			for (int m = left; m < left + width; m++)
			{
				for (int n = top; n < top + height; n++)
				{
					Point16 back = new Point16(m, n);
					Tile tile3 = Main.tile[m, n];
					if (tile3 != null && tile3.wire3())
					{
						_wireList.PushBack(back);
					}
				}
			}
			if (_wireList.Count > 0)
			{
				_numInPump = 0;
				_numOutPump = 0;
				HitWire(_wireList, 3);
				if (_numInPump > 0 && _numOutPump > 0)
				{
					XferWater();
				}
			}
			array[4] = _teleport[0];
			array[5] = _teleport[1];
			for (int num = 0; num < 5; num += 2)
			{
				_teleport[0] = array[num];
				_teleport[1] = array[num + 1];
				if (_teleport[0].X >= 0f && _teleport[1].X >= 0f)
				{
					Teleport();
				}
			}
		}

		private static void HitWire(DoubleStack<Point16> next, int wireType)
		{
			for (int i = 0; i < next.Count; i++)
			{
				Point16 point = next.PopFront();
				SkipWire(point);
				_toProcess.Add(point, 4);
				next.PushBack(point);
			}
			while (next.Count > 0)
			{
				Point16 key = next.PopFront();
				int x = key.X;
				int y = key.Y;
				if (!_wireSkip.ContainsKey(key))
				{
					HitWireSingle(x, y);
				}
				for (int j = 0; j < 4; j++)
				{
					int num;
					int num2;
					switch (j)
					{
					case 0:
						num = x;
						num2 = y + 1;
						break;
					case 1:
						num = x;
						num2 = y - 1;
						break;
					case 2:
						num = x + 1;
						num2 = y;
						break;
					case 3:
						num = x - 1;
						num2 = y;
						break;
					default:
						num = x;
						num2 = y + 1;
						break;
					}
					if (num < 2 || num >= Main.maxTilesX - 2 || num2 < 2 || num2 >= Main.maxTilesY - 2)
					{
						continue;
					}
					Tile tile = Main.tile[num, num2];
					if (tile == null)
					{
						continue;
					}
					bool flag;
					switch (wireType)
					{
					case 1:
						flag = tile.wire();
						break;
					case 2:
						flag = tile.wire2();
						break;
					case 3:
						flag = tile.wire3();
						break;
					default:
						flag = false;
						break;
					}
					if (!flag)
					{
						continue;
					}
					Point16 point2 = new Point16(num, num2);
					byte value;
					if (_toProcess.TryGetValue(point2, out value))
					{
						value = (byte)(value - 1);
						if (value == 0)
						{
							_toProcess.Remove(point2);
						}
						else
						{
							_toProcess[point2] = value;
						}
					}
					else
					{
						next.PushBack(point2);
						_toProcess.Add(point2, 3);
					}
				}
			}
			_wireSkip.Clear();
			_toProcess.Clear();
			running = false;
		}

		private static void HitWireSingle(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int type = tile.type;
			if (tile.active() && type >= 255 && type <= 268)
			{
				if (type >= 262)
				{
					tile.type -= 7;
				}
				else
				{
					tile.type += 7;
				}
				NetMessage.SendTileSquare(-1, i, j, 1);
			}
			if (tile.actuator() && (type != 226 || !((double)j > Main.worldSurface) || NPC.downedPlantBoss))
			{
				if (tile.inActive())
				{
					ReActive(i, j);
				}
				else
				{
					DeActive(i, j);
				}
			}
			if (!tile.active())
			{
				return;
			}
			switch (type)
			{
			case 144:
				HitSwitch(i, j);
				WorldGen.SquareTileFrame(i, j);
				NetMessage.SendTileSquare(-1, i, j, 1);
				break;
			case 406:
			{
				int num32 = tile.frameX % 54 / 18;
				int num33 = tile.frameY % 54 / 18;
				int num34 = i - num32;
				int num35 = j - num33;
				int num36 = 54;
				if (Main.tile[num34, num35].frameY >= 108)
				{
					num36 = -108;
				}
				for (int num37 = num34; num37 < num34 + 3; num37++)
				{
					for (int num38 = num35; num38 < num35 + 3; num38++)
					{
						SkipWire(num37, num38);
						Main.tile[num37, num38].frameY = (short)(Main.tile[num37, num38].frameY + num36);
					}
				}
				NetMessage.SendTileSquare(-1, num34 + 1, num35 + 1, 3);
				break;
			}
			case 411:
			{
				int num40 = tile.frameX % 36 / 18;
				int num41 = tile.frameY % 36 / 18;
				int num42 = i - num40;
				int num43 = j - num41;
				int num44 = 36;
				if (Main.tile[num42, num43].frameX >= 36)
				{
					num44 = -36;
				}
				for (int num45 = num42; num45 < num42 + 2; num45++)
				{
					for (int num46 = num43; num46 < num43 + 2; num46++)
					{
						SkipWire(num45, num46);
						Main.tile[num45, num46].frameX = (short)(Main.tile[num45, num46].frameX + num44);
					}
				}
				NetMessage.SendTileSquare(-1, num42, num43, 2);
				break;
			}
			case 405:
			{
				int num73 = tile.frameX % 54 / 18;
				int num74 = tile.frameY % 36 / 18;
				int num75 = i - num73;
				int num76 = j - num74;
				int num77 = 54;
				if (Main.tile[num75, num76].frameX >= 54)
				{
					num77 = -54;
				}
				for (int num78 = num75; num78 < num75 + 3; num78++)
				{
					for (int num79 = num76; num79 < num76 + 2; num79++)
					{
						SkipWire(num78, num79);
						Main.tile[num78, num79].frameX = (short)(Main.tile[num78, num79].frameX + num77);
					}
				}
				NetMessage.SendTileSquare(-1, num75 + 1, num76 + 1, 3);
				break;
			}
			case 215:
			{
				int num65 = tile.frameX % 54 / 18;
				int num66 = tile.frameY % 36 / 18;
				int num67 = i - num65;
				int num68 = j - num66;
				int num69 = 36;
				if (Main.tile[num67, num68].frameY >= 36)
				{
					num69 = -36;
				}
				for (int num70 = num67; num70 < num67 + 3; num70++)
				{
					for (int num71 = num68; num71 < num68 + 2; num71++)
					{
						SkipWire(num70, num71);
						Main.tile[num70, num71].frameY = (short)(Main.tile[num70, num71].frameY + num69);
					}
				}
				NetMessage.SendTileSquare(-1, num67 + 1, num68 + 1, 3);
				break;
			}
			case 130:
				if (Main.tile[i, j - 1] == null || !Main.tile[i, j - 1].active() || (Main.tile[i, j - 1].type != 21 && Main.tile[i, j - 1].type != 88))
				{
					tile.type = 131;
					WorldGen.SquareTileFrame(i, j);
					NetMessage.SendTileSquare(-1, i, j, 1);
				}
				break;
			case 131:
				tile.type = 130;
				WorldGen.SquareTileFrame(i, j);
				NetMessage.SendTileSquare(-1, i, j, 1);
				break;
			case 386:
			case 387:
			{
				bool value = type == 387;
				int num12 = WorldGen.ShiftTrapdoor(i, j, true).ToInt();
				if (num12 == 0)
				{
					num12 = -WorldGen.ShiftTrapdoor(i, j, false).ToInt();
				}
				if (num12 != 0)
				{
					NetMessage.SendData(19, -1, -1, "", 2 + value.ToInt(), i, j, num12);
				}
				break;
			}
			case 388:
			case 389:
			{
				bool flag2 = type == 389;
				WorldGen.ShiftTallGate(i, j, flag2);
				NetMessage.SendData(19, -1, -1, "", 4 + flag2.ToInt(), i, j);
				break;
			}
			case 11:
				if (WorldGen.CloseDoor(i, j, true))
				{
					NetMessage.SendData(19, -1, -1, "", 1, i, j);
				}
				break;
			case 10:
			{
				int num72 = 1;
				if (Main.rand.Next(2) == 0)
				{
					num72 = -1;
				}
				if (!WorldGen.OpenDoor(i, j, num72))
				{
					if (WorldGen.OpenDoor(i, j, -num72))
					{
						NetMessage.SendData(19, -1, -1, "", 0, i, j, -num72);
					}
				}
				else
				{
					NetMessage.SendData(19, -1, -1, "", 0, i, j, num72);
				}
				break;
			}
			case 216:
				WorldGen.LaunchRocket(i, j);
				SkipWire(i, j);
				break;
			case 335:
			{
				int num60 = j - tile.frameY / 18;
				int num61 = i - tile.frameX / 18;
				SkipWire(num61, num60);
				SkipWire(num61, num60 + 1);
				SkipWire(num61 + 1, num60);
				SkipWire(num61 + 1, num60 + 1);
				if (CheckMech(num61, num60, 30))
				{
					WorldGen.LaunchRocketSmall(num61, num60);
				}
				break;
			}
			case 338:
			{
				int num7 = j - tile.frameY / 18;
				int num8 = i - tile.frameX / 18;
				SkipWire(num8, num7);
				SkipWire(num8, num7 + 1);
				if (!CheckMech(num8, num7, 30))
				{
					break;
				}
				bool flag = false;
				for (int m = 0; m < 1000; m++)
				{
					if (Main.projectile[m].active && Main.projectile[m].aiStyle == 73 && Main.projectile[m].ai[0] == (float)num8 && Main.projectile[m].ai[1] == (float)num7)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Projectile.NewProjectile(num8 * 16 + 8, num7 * 16 + 2, 0f, 0f, 419 + Main.rand.Next(4), 0, 0f, Main.myPlayer, num8, num7);
				}
				break;
			}
			case 235:
			{
				int num39 = i - tile.frameX / 18;
				if (tile.wall == 87 && (double)j > Main.worldSurface && !NPC.downedPlantBoss)
				{
					break;
				}
				if (_teleport[0].X == -1f)
				{
					_teleport[0].X = num39;
					_teleport[0].Y = j;
					if (tile.halfBrick())
					{
						_teleport[0].Y += 0.5f;
					}
				}
				else if (_teleport[0].X != (float)num39 || _teleport[0].Y != (float)j)
				{
					_teleport[1].X = num39;
					_teleport[1].Y = j;
					if (tile.halfBrick())
					{
						_teleport[1].Y += 0.5f;
					}
				}
				break;
			}
			case 4:
				if (tile.frameX < 66)
				{
					tile.frameX += 66;
				}
				else
				{
					tile.frameX -= 66;
				}
				NetMessage.SendTileSquare(-1, i, j, 1);
				break;
			case 149:
				if (tile.frameX < 54)
				{
					tile.frameX += 54;
				}
				else
				{
					tile.frameX -= 54;
				}
				NetMessage.SendTileSquare(-1, i, j, 1);
				break;
			case 244:
			{
				int num47;
				for (num47 = tile.frameX / 18; num47 >= 3; num47 -= 3)
				{
				}
				int num48;
				for (num48 = tile.frameY / 18; num48 >= 3; num48 -= 3)
				{
				}
				int num49 = i - num47;
				int num50 = j - num48;
				int num51 = 54;
				if (Main.tile[num49, num50].frameX >= 54)
				{
					num51 = -54;
				}
				for (int num52 = num49; num52 < num49 + 3; num52++)
				{
					for (int num53 = num50; num53 < num50 + 2; num53++)
					{
						SkipWire(num52, num53);
						Main.tile[num52, num53].frameX = (short)(Main.tile[num52, num53].frameX + num51);
					}
				}
				NetMessage.SendTileSquare(-1, num49 + 1, num50 + 1, 3);
				break;
			}
			case 42:
			{
				int num9;
				for (num9 = tile.frameY / 18; num9 >= 2; num9 -= 2)
				{
				}
				int num10 = j - num9;
				short num11 = 18;
				if (tile.frameX > 0)
				{
					num11 = -18;
				}
				Main.tile[i, num10].frameX += num11;
				Main.tile[i, num10 + 1].frameX += num11;
				SkipWire(i, num10);
				SkipWire(i, num10 + 1);
				NetMessage.SendTileSquare(-1, i, j, 2);
				break;
			}
			case 93:
			{
				int num4;
				for (num4 = tile.frameY / 18; num4 >= 3; num4 -= 3)
				{
				}
				num4 = j - num4;
				short num5 = 18;
				if (tile.frameX > 0)
				{
					num5 = -18;
				}
				Main.tile[i, num4].frameX += num5;
				Main.tile[i, num4 + 1].frameX += num5;
				Main.tile[i, num4 + 2].frameX += num5;
				SkipWire(i, num4);
				SkipWire(i, num4 + 1);
				SkipWire(i, num4 + 2);
				NetMessage.SendTileSquare(-1, i, num4 + 1, 3);
				break;
			}
			case 95:
			case 100:
			case 126:
			case 173:
			{
				int num62;
				for (num62 = tile.frameY / 18; num62 >= 2; num62 -= 2)
				{
				}
				num62 = j - num62;
				int num63 = tile.frameX / 18;
				if (num63 > 1)
				{
					num63 -= 2;
				}
				num63 = i - num63;
				short num64 = 36;
				if (Main.tile[num63, num62].frameX > 0)
				{
					num64 = -36;
				}
				Main.tile[num63, num62].frameX += num64;
				Main.tile[num63, num62 + 1].frameX += num64;
				Main.tile[num63 + 1, num62].frameX += num64;
				Main.tile[num63 + 1, num62 + 1].frameX += num64;
				SkipWire(num63, num62);
				SkipWire(num63 + 1, num62);
				SkipWire(num63, num62 + 1);
				SkipWire(num63 + 1, num62 + 1);
				NetMessage.SendTileSquare(-1, num63, num62, 3);
				break;
			}
			case 34:
			{
				int num13;
				for (num13 = tile.frameY / 18; num13 >= 3; num13 -= 3)
				{
				}
				int num14 = j - num13;
				int num15 = tile.frameX / 18;
				if (num15 > 2)
				{
					num15 -= 3;
				}
				num15 = i - num15;
				short num16 = 54;
				if (Main.tile[num15, num14].frameX > 0)
				{
					num16 = -54;
				}
				for (int n = num15; n < num15 + 3; n++)
				{
					for (int num17 = num14; num17 < num14 + 3; num17++)
					{
						Main.tile[n, num17].frameX += num16;
						SkipWire(n, num17);
					}
				}
				NetMessage.SendTileSquare(-1, num15 + 1, num14 + 1, 3);
				break;
			}
			case 314:
				if (CheckMech(i, j, 5))
				{
					Minecart.FlipSwitchTrack(i, j);
				}
				break;
			case 33:
			case 174:
			{
				short num6 = 18;
				if (tile.frameX > 0)
				{
					num6 = -18;
				}
				tile.frameX += num6;
				NetMessage.SendTileSquare(-1, i, j, 3);
				break;
			}
			case 92:
			{
				int num88 = j - tile.frameY / 18;
				short num89 = 18;
				if (tile.frameX > 0)
				{
					num89 = -18;
				}
				for (int num90 = num88; num90 < num88 + 6; num90++)
				{
					Main.tile[i, num90].frameX += num89;
					SkipWire(i, num90);
				}
				NetMessage.SendTileSquare(-1, i, num88 + 3, 7);
				break;
			}
			case 137:
			{
				int num80 = tile.frameY / 18;
				Vector2 vector = Vector2.Zero;
				float speedX = 0f;
				float speedY = 0f;
				int num81 = 0;
				int damage = 0;
				switch (num80)
				{
				case 0:
					if (CheckMech(i, j, 200))
					{
						int num86 = -1;
						if (tile.frameX != 0)
						{
							num86 = 1;
						}
						speedX = 12 * num86;
						damage = 20;
						num81 = 98;
						vector = new Vector2(i * 16 + 8, j * 16 + 7);
						vector.X += 10 * num86;
						vector.Y += 2f;
					}
					break;
				case 1:
					if (CheckMech(i, j, 200))
					{
						int num82 = -1;
						if (tile.frameX != 0)
						{
							num82 = 1;
						}
						speedX = 12 * num82;
						damage = 40;
						num81 = 184;
						vector = new Vector2(i * 16 + 8, j * 16 + 7);
						vector.X += 10 * num82;
						vector.Y += 2f;
					}
					break;
				case 2:
					if (CheckMech(i, j, 200))
					{
						int num87 = -1;
						if (tile.frameX != 0)
						{
							num87 = 1;
						}
						speedX = 5 * num87;
						damage = 40;
						num81 = 187;
						vector = new Vector2(i * 16 + 8, j * 16 + 7);
						vector.X += 10 * num87;
						vector.Y += 2f;
					}
					break;
				case 3:
				{
					if (!CheckMech(i, j, 300))
					{
						break;
					}
					num81 = 185;
					int num83 = 200;
					for (int num84 = 0; num84 < 1000; num84++)
					{
						if (Main.projectile[num84].active && Main.projectile[num84].type == num81)
						{
							float num85 = (new Vector2(i * 16 + 8, j * 18 + 8) - Main.projectile[num84].Center).Length();
							num83 = ((!(num85 < 50f)) ? ((!(num85 < 100f)) ? ((!(num85 < 200f)) ? ((!(num85 < 300f)) ? ((!(num85 < 400f)) ? ((!(num85 < 500f)) ? ((!(num85 < 700f)) ? ((!(num85 < 900f)) ? ((!(num85 < 1200f)) ? (num83 - 1) : (num83 - 2)) : (num83 - 3)) : (num83 - 4)) : (num83 - 5)) : (num83 - 6)) : (num83 - 8)) : (num83 - 10)) : (num83 - 15)) : (num83 - 50));
						}
					}
					if (num83 > 0)
					{
						speedX = (float)Main.rand.Next(-20, 21) * 0.05f;
						speedY = 4f + (float)Main.rand.Next(0, 21) * 0.05f;
						damage = 40;
						vector = new Vector2(i * 16 + 8, j * 16 + 16);
						vector.Y += 6f;
						Projectile.NewProjectile((int)vector.X, (int)vector.Y, speedX, speedY, num81, damage, 2f, Main.myPlayer);
					}
					break;
				}
				case 4:
					if (CheckMech(i, j, 90))
					{
						speedX = 0f;
						speedY = 8f;
						damage = 60;
						num81 = 186;
						vector = new Vector2(i * 16 + 8, j * 16 + 16);
						vector.Y += 10f;
					}
					break;
				}
				if (num81 != 0)
				{
					Projectile.NewProjectile((int)vector.X, (int)vector.Y, speedX, speedY, num81, damage, 2f, Main.myPlayer);
				}
				break;
			}
			case 35:
			case 139:
				WorldGen.SwitchMB(i, j);
				break;
			case 207:
				WorldGen.SwitchFountain(i, j);
				break;
			case 410:
				WorldGen.SwitchMonolith(i, j);
				break;
			case 141:
				WorldGen.KillTile(i, j, false, false, true);
				NetMessage.SendTileSquare(-1, i, j, 1);
				Projectile.NewProjectile(i * 16 + 8, j * 16 + 8, 0f, 0f, 108, 500, 10f, Main.myPlayer);
				break;
			case 210:
				WorldGen.ExplodeMine(i, j);
				break;
			case 142:
			case 143:
			{
				int num54 = j - tile.frameY / 18;
				int num55 = tile.frameX / 18;
				if (num55 > 1)
				{
					num55 -= 2;
				}
				num55 = i - num55;
				SkipWire(num55, num54);
				SkipWire(num55, num54 + 1);
				SkipWire(num55 + 1, num54);
				SkipWire(num55 + 1, num54 + 1);
				if (type == 142)
				{
					for (int num56 = 0; num56 < 4; num56++)
					{
						if (_numInPump >= 19)
						{
							break;
						}
						int num57;
						int num58;
						switch (num56)
						{
						case 0:
							num57 = num55;
							num58 = num54 + 1;
							break;
						case 1:
							num57 = num55 + 1;
							num58 = num54 + 1;
							break;
						case 2:
							num57 = num55;
							num58 = num54;
							break;
						default:
							num57 = num55 + 1;
							num58 = num54;
							break;
						}
						_inPumpX[_numInPump] = num57;
						_inPumpY[_numInPump] = num58;
						_numInPump++;
					}
					break;
				}
				for (int num59 = 0; num59 < 4; num59++)
				{
					if (_numOutPump >= 19)
					{
						break;
					}
					int num57;
					int num58;
					switch (num59)
					{
					case 0:
						num57 = num55;
						num58 = num54 + 1;
						break;
					case 1:
						num57 = num55 + 1;
						num58 = num54 + 1;
						break;
					case 2:
						num57 = num55;
						num58 = num54;
						break;
					default:
						num57 = num55 + 1;
						num58 = num54;
						break;
					}
					_outPumpX[_numOutPump] = num57;
					_outPumpY[_numOutPump] = num58;
					_numOutPump++;
				}
				break;
			}
			case 105:
			{
				int num18 = j - tile.frameY / 18;
				int num19 = tile.frameX / 18;
				int num20 = 0;
				while (num19 >= 2)
				{
					num19 -= 2;
					num20++;
				}
				num19 = i - num19;
				SkipWire(num19, num18);
				SkipWire(num19, num18 + 1);
				SkipWire(num19, num18 + 2);
				SkipWire(num19 + 1, num18);
				SkipWire(num19 + 1, num18 + 1);
				SkipWire(num19 + 1, num18 + 2);
				int num21 = num19 * 16 + 16;
				int num22 = (num18 + 3) * 16;
				int num23 = -1;
				switch (num20)
				{
				case 4:
					if (CheckMech(num19, num18, 30) && NPC.MechSpawn(num21, num22, 1))
					{
						num23 = NPC.NewNPC(num21, num22 - 12, 1);
					}
					break;
				case 7:
					if (CheckMech(num19, num18, 30) && NPC.MechSpawn(num21, num22, 49))
					{
						num23 = NPC.NewNPC(num21 - 4, num22 - 6, 49);
					}
					break;
				case 8:
					if (CheckMech(num19, num18, 30) && NPC.MechSpawn(num21, num22, 55))
					{
						num23 = NPC.NewNPC(num21, num22 - 12, 55);
					}
					break;
				case 9:
					if (CheckMech(num19, num18, 30) && NPC.MechSpawn(num21, num22, 46))
					{
						num23 = NPC.NewNPC(num21, num22 - 12, 46);
					}
					break;
				case 10:
					if (CheckMech(num19, num18, 30) && NPC.MechSpawn(num21, num22, 21))
					{
						num23 = NPC.NewNPC(num21, num22, 21);
					}
					break;
				case 18:
					if (CheckMech(num19, num18, 30) && NPC.MechSpawn(num21, num22, 67))
					{
						num23 = NPC.NewNPC(num21, num22 - 12, 67);
					}
					break;
				case 23:
					if (CheckMech(num19, num18, 30) && NPC.MechSpawn(num21, num22, 63))
					{
						num23 = NPC.NewNPC(num21, num22 - 12, 63);
					}
					break;
				case 27:
					if (CheckMech(num19, num18, 30) && NPC.MechSpawn(num21, num22, 85))
					{
						num23 = NPC.NewNPC(num21 - 9, num22, 85);
					}
					break;
				case 28:
					if (CheckMech(num19, num18, 30) && NPC.MechSpawn(num21, num22, 74))
					{
						num23 = NPC.NewNPC(num21, num22 - 12, 74);
					}
					break;
				case 34:
				{
					for (int num27 = 0; num27 < 2; num27++)
					{
						for (int num28 = 0; num28 < 3; num28++)
						{
							Tile tile2 = Main.tile[num19 + num27, num18 + num28];
							tile2.type = 349;
							tile2.frameX = (short)(num27 * 18 + 216);
							tile2.frameY = (short)(num28 * 18);
						}
					}
					Animation.NewTemporaryAnimation(0, 349, num19, num18);
					if (Main.netMode == 2)
					{
						NetMessage.SendTileRange(-1, num19, num18, 2, 3);
					}
					break;
				}
				case 42:
					if (CheckMech(num19, num18, 30) && NPC.MechSpawn(num21, num22, 58))
					{
						num23 = NPC.NewNPC(num21, num22 - 12, 58);
					}
					break;
				case 37:
					if (CheckMech(num19, num18, 600) && Item.MechSpawn(num21, num22, 58) && Item.MechSpawn(num21, num22, 1734) && Item.MechSpawn(num21, num22, 1867))
					{
						Item.NewItem(num21, num22 - 16, 0, 0, 58);
					}
					break;
				case 50:
					if (CheckMech(num19, num18, 30) && NPC.MechSpawn(num21, num22, 65) && !Collision.SolidTiles(num19 - 2, num19 + 3, num18, num18 + 2))
					{
						num23 = NPC.NewNPC(num21, num22 - 12, 65);
					}
					break;
				case 2:
					if (CheckMech(num19, num18, 600) && Item.MechSpawn(num21, num22, 184) && Item.MechSpawn(num21, num22, 1735) && Item.MechSpawn(num21, num22, 1868))
					{
						Item.NewItem(num21, num22 - 16, 0, 0, 184);
					}
					break;
				case 17:
					if (CheckMech(num19, num18, 600) && Item.MechSpawn(num21, num22, 166))
					{
						Item.NewItem(num21, num22 - 20, 0, 0, 166);
					}
					break;
				case 40:
				{
					if (!CheckMech(num19, num18, 300))
					{
						break;
					}
					int[] array2 = new int[10];
					int num29 = 0;
					for (int num30 = 0; num30 < 200; num30++)
					{
						if (Main.npc[num30].active && (Main.npc[num30].type == 17 || Main.npc[num30].type == 19 || Main.npc[num30].type == 22 || Main.npc[num30].type == 38 || Main.npc[num30].type == 54 || Main.npc[num30].type == 107 || Main.npc[num30].type == 108 || Main.npc[num30].type == 142 || Main.npc[num30].type == 160 || Main.npc[num30].type == 207 || Main.npc[num30].type == 209 || Main.npc[num30].type == 227 || Main.npc[num30].type == 228 || Main.npc[num30].type == 229 || Main.npc[num30].type == 358 || Main.npc[num30].type == 369))
						{
							array2[num29] = num30;
							num29++;
							if (num29 >= 9)
							{
								break;
							}
						}
					}
					if (num29 > 0)
					{
						int num31 = array2[Main.rand.Next(num29)];
						Main.npc[num31].position.X = num21 - Main.npc[num31].width / 2;
						Main.npc[num31].position.Y = num22 - Main.npc[num31].height - 1;
						NetMessage.SendData(23, -1, -1, "", num31);
					}
					break;
				}
				case 41:
				{
					if (!CheckMech(num19, num18, 300))
					{
						break;
					}
					int[] array = new int[10];
					int num24 = 0;
					for (int num25 = 0; num25 < 200; num25++)
					{
						if (Main.npc[num25].active && (Main.npc[num25].type == 18 || Main.npc[num25].type == 20 || Main.npc[num25].type == 124 || Main.npc[num25].type == 178 || Main.npc[num25].type == 208 || Main.npc[num25].type == 353))
						{
							array[num24] = num25;
							num24++;
							if (num24 >= 9)
							{
								break;
							}
						}
					}
					if (num24 > 0)
					{
						int num26 = array[Main.rand.Next(num24)];
						Main.npc[num26].position.X = num21 - Main.npc[num26].width / 2;
						Main.npc[num26].position.Y = num22 - Main.npc[num26].height - 1;
						NetMessage.SendData(23, -1, -1, "", num26);
					}
					break;
				}
				}
				if (num23 >= 0)
				{
					Main.npc[num23].value = 0f;
					Main.npc[num23].npcSlots = 0f;
				}
				break;
			}
			case 349:
			{
				int num = j - tile.frameY / 18;
				int num2;
				for (num2 = tile.frameX / 18; num2 >= 2; num2 -= 2)
				{
				}
				num2 = i - num2;
				SkipWire(num2, num);
				SkipWire(num2, num + 1);
				SkipWire(num2, num + 2);
				SkipWire(num2 + 1, num);
				SkipWire(num2 + 1, num + 1);
				SkipWire(num2 + 1, num + 2);
				short num3 = (short)((Main.tile[num2, num].frameX != 0) ? (-216) : 216);
				for (int k = 0; k < 2; k++)
				{
					for (int l = 0; l < 3; l++)
					{
						Main.tile[num2 + k, num + l].frameX += num3;
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendTileRange(-1, num2, num, 2, 3);
				}
				Animation.NewTemporaryAnimation((num3 <= 0) ? 1 : 0, 349, num2, num);
				break;
			}
			}
		}

		private static void Teleport()
		{
			if (_teleport[0].X < _teleport[1].X + 3f && _teleport[0].X > _teleport[1].X - 3f && _teleport[0].Y > _teleport[1].Y - 3f && _teleport[0].Y < _teleport[1].Y)
			{
				return;
			}
			Rectangle[] array = new Rectangle[2];
			array[0].X = (int)(_teleport[0].X * 16f);
			array[0].Width = 48;
			array[0].Height = 48;
			array[0].Y = (int)(_teleport[0].Y * 16f - (float)array[0].Height);
			array[1].X = (int)(_teleport[1].X * 16f);
			array[1].Width = 48;
			array[1].Height = 48;
			array[1].Y = (int)(_teleport[1].Y * 16f - (float)array[1].Height);
			for (int i = 0; i < 2; i++)
			{
				Vector2 value = new Vector2(array[1].X - array[0].X, array[1].Y - array[0].Y);
				if (i == 1)
				{
					value = new Vector2(array[0].X - array[1].X, array[0].Y - array[1].Y);
				}
				for (int j = 0; j < 16; j++)
				{
					if (Main.player[j].active && !Main.player[j].dead && !Main.player[j].teleporting && array[i].Intersects(Main.player[j].getRect()))
					{
						Vector2 vector = Main.player[j].position + value;
						Main.player[j].teleporting = true;
						if (Main.netMode == 2)
						{
							RemoteClient.CheckSection(j, vector);
						}
						Main.player[j].Teleport(vector);
						if (Main.netMode == 2)
						{
							NetMessage.SendData(65, -1, -1, "", 0, j, vector.X, vector.Y);
						}
					}
				}
				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active && !Main.npc[k].teleporting && Main.npc[k].lifeMax > 5 && !Main.npc[k].boss && !Main.npc[k].noTileCollide && array[i].Intersects(Main.npc[k].getRect()))
					{
						Main.npc[k].teleporting = true;
						Main.npc[k].Teleport(Main.npc[k].position + value);
					}
				}
			}
			for (int l = 0; l < 16; l++)
			{
				Main.player[l].teleporting = false;
			}
			for (int m = 0; m < 200; m++)
			{
				Main.npc[m].teleporting = false;
			}
		}

		private static void DeActive(int i, int j)
		{
			if (!Main.tile[i, j].active())
			{
				return;
			}
			bool flag = Main.tileSolid[Main.tile[i, j].type] && !TileID.Sets.NotReallySolid[Main.tile[i, j].type];
			switch (Main.tile[i, j].type)
			{
			case 314:
			case 386:
			case 387:
			case 388:
			case 389:
				flag = false;
				break;
			}
			if (flag && (!Main.tile[i, j - 1].active() || (Main.tile[i, j - 1].type != 5 && Main.tile[i, j - 1].type != 21 && Main.tile[i, j - 1].type != 26 && Main.tile[i, j - 1].type != 77 && Main.tile[i, j - 1].type != 72)))
			{
				Main.tile[i, j].inActive(true);
				WorldGen.SquareTileFrame(i, j, false);
				if (Main.netMode != 1)
				{
					NetMessage.SendTileSquare(-1, i, j, 1);
				}
			}
		}

		private static void ReActive(int i, int j)
		{
			Main.tile[i, j].inActive(false);
			WorldGen.SquareTileFrame(i, j, false);
			if (Main.netMode != 1)
			{
				NetMessage.SendTileSquare(-1, i, j, 1);
			}
		}
	}
}
