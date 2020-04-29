using Microsoft.Xna.Framework;
using System;

namespace Terraria
{
	public static class Minecart
	{
		private enum TrackState
		{
			NoTrack = -1,
			AboveTrack,
			OnTrack,
			BelowTrack,
			AboveFront,
			AboveBack,
			OnFront,
			OnBack
		}

		private const int TotalFrames = 36;

		public const int LeftDownDecoration = 36;

		public const int RightDownDecoration = 37;

		public const int BouncyBumperDecoration = 38;

		public const int RegularBumperDecoration = 39;

		public const int Flag_OnTrack = 0;

		public const int Flag_BouncyBumper = 1;

		public const int Flag_UsedRamp = 2;

		public const int Flag_HitSwitch = 3;

		public const int Flag_BoostLeft = 4;

		public const int Flag_BoostRight = 5;

		private const int NoConnection = -1;

		private const int TopConnection = 0;

		private const int MiddleConnection = 1;

		private const int BottomConnection = 2;

		private const int BumperEnd = -1;

		private const int BouncyEnd = -2;

		private const int RampEnd = -3;

		private const int OpenEnd = -4;

		public const float BoosterSpeed = 4f;

		private const int Type_Normal = 0;

		private const int Type_Pressure = 1;

		private const int Type_Booster = 2;

		private const float MinecartTextureWidth = 50f;

		private static Vector2 _trackMagnetOffset = new Vector2(25f, 26f);

		private static int[] _leftSideConnection;

		private static int[] _rightSideConnection;

		private static int[] _trackType;

		private static bool[] _boostLeft;

		private static Vector2[] _texturePosition;

		private static short _firstPressureFrame;

		private static short _firstLeftBoostFrame;

		private static short _firstRightBoostFrame;

		private static int[][] _trackSwitchOptions;

		private static int[][] _tileHeight;

		public static void Initialize()
		{
			_rightSideConnection = new int[36];
			_leftSideConnection = new int[36];
			_trackType = new int[36];
			_boostLeft = new bool[36];
			_texturePosition = new Vector2[40];
			_tileHeight = new int[36][];
			for (int i = 0; i < 36; i++)
			{
				int[] array = new int[8];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = 5;
				}
				_tileHeight[i] = array;
			}
			int num = 0;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = -1;
			_tileHeight[num][0] = -4;
			_tileHeight[num][7] = -4;
			_texturePosition[num] = new Vector2(0f, 0f);
			num++;
			_leftSideConnection[num] = 1;
			_rightSideConnection[num] = 1;
			_texturePosition[num] = new Vector2(1f, 0f);
			num++;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = 1;
			for (int k = 0; k < 4; k++)
			{
				_tileHeight[num][k] = -1;
			}
			_texturePosition[num] = new Vector2(2f, 1f);
			num++;
			_leftSideConnection[num] = 1;
			_rightSideConnection[num] = -1;
			for (int l = 4; l < 8; l++)
			{
				_tileHeight[num][l] = -1;
			}
			_texturePosition[num] = new Vector2(3f, 1f);
			num++;
			_leftSideConnection[num] = 2;
			_rightSideConnection[num] = 1;
			_tileHeight[num][0] = 1;
			_tileHeight[num][1] = 2;
			_tileHeight[num][2] = 3;
			_tileHeight[num][3] = 3;
			_tileHeight[num][4] = 4;
			_tileHeight[num][5] = 4;
			_texturePosition[num] = new Vector2(0f, 2f);
			num++;
			_leftSideConnection[num] = 1;
			_rightSideConnection[num] = 2;
			_tileHeight[num][2] = 4;
			_tileHeight[num][3] = 4;
			_tileHeight[num][4] = 3;
			_tileHeight[num][5] = 3;
			_tileHeight[num][6] = 2;
			_tileHeight[num][7] = 1;
			_texturePosition[num] = new Vector2(1f, 2f);
			num++;
			_leftSideConnection[num] = 1;
			_rightSideConnection[num] = 0;
			_tileHeight[num][4] = 6;
			_tileHeight[num][5] = 6;
			_tileHeight[num][6] = 7;
			_tileHeight[num][7] = 8;
			_texturePosition[num] = new Vector2(0f, 1f);
			num++;
			_leftSideConnection[num] = 0;
			_rightSideConnection[num] = 1;
			_tileHeight[num][0] = 8;
			_tileHeight[num][1] = 7;
			_tileHeight[num][2] = 6;
			_tileHeight[num][3] = 6;
			_texturePosition[num] = new Vector2(1f, 1f);
			num++;
			_leftSideConnection[num] = 0;
			_rightSideConnection[num] = 2;
			for (int m = 0; m < 8; m++)
			{
				_tileHeight[num][m] = 8 - m;
			}
			_texturePosition[num] = new Vector2(0f, 3f);
			num++;
			_leftSideConnection[num] = 2;
			_rightSideConnection[num] = 0;
			for (int n = 0; n < 8; n++)
			{
				_tileHeight[num][n] = n + 1;
			}
			_texturePosition[num] = new Vector2(1f, 3f);
			num++;
			_leftSideConnection[num] = 2;
			_rightSideConnection[num] = -1;
			_tileHeight[num][0] = 1;
			_tileHeight[num][1] = 2;
			for (int num2 = 2; num2 < 8; num2++)
			{
				_tileHeight[num][num2] = -1;
			}
			_texturePosition[num] = new Vector2(4f, 1f);
			num++;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = 2;
			_tileHeight[num][6] = 2;
			_tileHeight[num][7] = 1;
			for (int num3 = 0; num3 < 6; num3++)
			{
				_tileHeight[num][num3] = -1;
			}
			_texturePosition[num] = new Vector2(5f, 1f);
			num++;
			_leftSideConnection[num] = 0;
			_rightSideConnection[num] = -1;
			_tileHeight[num][0] = 8;
			_tileHeight[num][1] = 7;
			_tileHeight[num][2] = 6;
			for (int num4 = 3; num4 < 8; num4++)
			{
				_tileHeight[num][num4] = -1;
			}
			_texturePosition[num] = new Vector2(6f, 1f);
			num++;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = 0;
			_tileHeight[num][5] = 6;
			_tileHeight[num][6] = 7;
			_tileHeight[num][7] = 8;
			for (int num5 = 0; num5 < 5; num5++)
			{
				_tileHeight[num][num5] = -1;
			}
			_texturePosition[num] = new Vector2(7f, 1f);
			num++;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = 1;
			_tileHeight[num][0] = -4;
			_texturePosition[num] = new Vector2(2f, 0f);
			num++;
			_leftSideConnection[num] = 1;
			_rightSideConnection[num] = -1;
			_tileHeight[num][7] = -4;
			_texturePosition[num] = new Vector2(3f, 0f);
			num++;
			_leftSideConnection[num] = 2;
			_rightSideConnection[num] = -1;
			for (int num6 = 0; num6 < 6; num6++)
			{
				_tileHeight[num][num6] = num6 + 1;
			}
			_tileHeight[num][6] = -3;
			_tileHeight[num][7] = -3;
			_texturePosition[num] = new Vector2(4f, 0f);
			num++;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = 2;
			_tileHeight[num][0] = -3;
			_tileHeight[num][1] = -3;
			for (int num7 = 2; num7 < 8; num7++)
			{
				_tileHeight[num][num7] = 8 - num7;
			}
			_texturePosition[num] = new Vector2(5f, 0f);
			num++;
			_leftSideConnection[num] = 0;
			_rightSideConnection[num] = -1;
			for (int num8 = 0; num8 < 6; num8++)
			{
				_tileHeight[num][num8] = 8 - num8;
			}
			_tileHeight[num][6] = -3;
			_tileHeight[num][7] = -3;
			_texturePosition[num] = new Vector2(6f, 0f);
			num++;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = 0;
			_tileHeight[num][0] = -3;
			_tileHeight[num][1] = -3;
			for (int num9 = 2; num9 < 8; num9++)
			{
				_tileHeight[num][num9] = num9 + 1;
			}
			_texturePosition[num] = new Vector2(7f, 0f);
			num++;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = -1;
			_tileHeight[num][0] = -4;
			_tileHeight[num][7] = -4;
			_trackType[num] = 1;
			_texturePosition[num] = new Vector2(0f, 4f);
			num++;
			_leftSideConnection[num] = 1;
			_rightSideConnection[num] = 1;
			_trackType[num] = 1;
			_texturePosition[num] = new Vector2(1f, 4f);
			num++;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = 1;
			_tileHeight[num][0] = -4;
			_trackType[num] = 1;
			_texturePosition[num] = new Vector2(0f, 5f);
			num++;
			_leftSideConnection[num] = 1;
			_rightSideConnection[num] = -1;
			_tileHeight[num][7] = -4;
			_trackType[num] = 1;
			_texturePosition[num] = new Vector2(1f, 5f);
			num++;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = 1;
			for (int num10 = 0; num10 < 6; num10++)
			{
				_tileHeight[num][num10] = -2;
			}
			_texturePosition[num] = new Vector2(2f, 2f);
			num++;
			_leftSideConnection[num] = 1;
			_rightSideConnection[num] = -1;
			for (int num11 = 2; num11 < 8; num11++)
			{
				_tileHeight[num][num11] = -2;
			}
			_texturePosition[num] = new Vector2(3f, 2f);
			num++;
			_leftSideConnection[num] = 2;
			_rightSideConnection[num] = -1;
			_tileHeight[num][0] = 1;
			_tileHeight[num][1] = 2;
			for (int num12 = 2; num12 < 8; num12++)
			{
				_tileHeight[num][num12] = -2;
			}
			_texturePosition[num] = new Vector2(4f, 2f);
			num++;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = 2;
			_tileHeight[num][6] = 2;
			_tileHeight[num][7] = 1;
			for (int num13 = 0; num13 < 6; num13++)
			{
				_tileHeight[num][num13] = -2;
			}
			_texturePosition[num] = new Vector2(5f, 2f);
			num++;
			_leftSideConnection[num] = 0;
			_rightSideConnection[num] = -1;
			_tileHeight[num][0] = 8;
			_tileHeight[num][1] = 7;
			_tileHeight[num][2] = 6;
			for (int num14 = 3; num14 < 8; num14++)
			{
				_tileHeight[num][num14] = -2;
			}
			_texturePosition[num] = new Vector2(6f, 2f);
			num++;
			_leftSideConnection[num] = -1;
			_rightSideConnection[num] = 0;
			_tileHeight[num][5] = 6;
			_tileHeight[num][6] = 7;
			_tileHeight[num][7] = 8;
			for (int num15 = 0; num15 < 5; num15++)
			{
				_tileHeight[num][num15] = -2;
			}
			_texturePosition[num] = new Vector2(7f, 2f);
			num++;
			_leftSideConnection[num] = 1;
			_rightSideConnection[num] = 1;
			_trackType[num] = 2;
			_boostLeft[num] = false;
			_texturePosition[num] = new Vector2(2f, 3f);
			num++;
			_leftSideConnection[num] = 1;
			_rightSideConnection[num] = 1;
			_trackType[num] = 2;
			_boostLeft[num] = true;
			_texturePosition[num] = new Vector2(3f, 3f);
			num++;
			_leftSideConnection[num] = 0;
			_rightSideConnection[num] = 2;
			for (int num16 = 0; num16 < 8; num16++)
			{
				_tileHeight[num][num16] = 8 - num16;
			}
			_trackType[num] = 2;
			_boostLeft[num] = false;
			_texturePosition[num] = new Vector2(4f, 3f);
			num++;
			_leftSideConnection[num] = 2;
			_rightSideConnection[num] = 0;
			for (int num17 = 0; num17 < 8; num17++)
			{
				_tileHeight[num][num17] = num17 + 1;
			}
			_trackType[num] = 2;
			_boostLeft[num] = true;
			_texturePosition[num] = new Vector2(5f, 3f);
			num++;
			_leftSideConnection[num] = 0;
			_rightSideConnection[num] = 2;
			for (int num18 = 0; num18 < 8; num18++)
			{
				_tileHeight[num][num18] = 8 - num18;
			}
			_trackType[num] = 2;
			_boostLeft[num] = true;
			_texturePosition[num] = new Vector2(6f, 3f);
			num++;
			_leftSideConnection[num] = 2;
			_rightSideConnection[num] = 0;
			for (int num19 = 0; num19 < 8; num19++)
			{
				_tileHeight[num][num19] = num19 + 1;
			}
			_trackType[num] = 2;
			_boostLeft[num] = false;
			_texturePosition[num] = new Vector2(7f, 3f);
			num++;
			_texturePosition[36] = new Vector2(0f, 6f);
			_texturePosition[37] = new Vector2(1f, 6f);
			_texturePosition[39] = new Vector2(0f, 7f);
			_texturePosition[38] = new Vector2(1f, 7f);
			for (int num20 = 0; num20 < _texturePosition.Length; num20++)
			{
				_texturePosition[num20] *= 18f;
			}
			for (int num21 = 0; num21 < _tileHeight.Length; num21++)
			{
				int[] array2 = _tileHeight[num21];
				for (int num22 = 0; num22 < array2.Length; num22++)
				{
					if (array2[num22] >= 0)
					{
						array2[num22] = (8 - array2[num22]) * 2;
					}
				}
			}
			int[] array3 = new int[36];
			_trackSwitchOptions = new int[64][];
			for (int num23 = 0; num23 < 64; num23++)
			{
				int num24 = 0;
				for (int num25 = 1; num25 < 256; num25 <<= 1)
				{
					if ((num23 & num25) == num25)
					{
						num24++;
					}
				}
				int num26 = 0;
				for (int num27 = 0; num27 < 36; num27++)
				{
					array3[num27] = -1;
					int num28 = 0;
					switch (_leftSideConnection[num27])
					{
					case 0:
						num28 |= 1;
						break;
					case 1:
						num28 |= 2;
						break;
					case 2:
						num28 |= 4;
						break;
					}
					switch (_rightSideConnection[num27])
					{
					case 0:
						num28 |= 8;
						break;
					case 1:
						num28 |= 0x10;
						break;
					case 2:
						num28 |= 0x20;
						break;
					}
					if (num24 < 2)
					{
						if (num23 != num28)
						{
							continue;
						}
					}
					else if (num28 == 0 || (num23 & num28) != num28)
					{
						continue;
					}
					array3[num27] = num27;
					num26++;
				}
				if (num26 == 0)
				{
					continue;
				}
				int[] array4 = new int[num26];
				int num29 = 0;
				for (int num30 = 0; num30 < 36; num30++)
				{
					if (array3[num30] != -1)
					{
						array4[num29] = array3[num30];
						num29++;
					}
				}
				_trackSwitchOptions[num23] = array4;
			}
			_firstPressureFrame = -1;
			_firstLeftBoostFrame = -1;
			_firstRightBoostFrame = -1;
			for (int num31 = 0; num31 < _trackType.Length; num31++)
			{
				switch (_trackType[num31])
				{
				case 1:
					if (_firstPressureFrame == -1)
					{
						_firstPressureFrame = (short)num31;
					}
					break;
				case 2:
					if (_boostLeft[num31])
					{
						if (_firstLeftBoostFrame == -1)
						{
							_firstLeftBoostFrame = (short)num31;
						}
					}
					else if (_firstRightBoostFrame == -1)
					{
						_firstRightBoostFrame = (short)num31;
					}
					break;
				}
			}
		}

		public static BitsByte TrackCollision(ref Vector2 Position, ref Vector2 Velocity, ref Vector2 lastBoost, int Width, int Height, bool followDown, bool followUp, int fallStart, bool trackOnly, Action<Vector2> MinecartDust)
		{
			if (followDown && followUp)
			{
				followDown = false;
				followUp = false;
			}
			Vector2 vector = new Vector2((float)(Width / 2) - 25f, Height / 2);
			Vector2 value = Position + new Vector2((float)(Width / 2) - 25f, Height / 2);
			Vector2 vector2 = value + _trackMagnetOffset;
			Vector2 value2 = Velocity;
			float num = value2.Length();
			value2.Normalize();
			Vector2 vector3 = vector2;
			Tile tile = null;
			bool flag = false;
			bool flag2 = true;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			TrackState trackState = TrackState.NoTrack;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			Vector2 vector4 = Vector2.Zero;
			Vector2 vector5 = Vector2.Zero;
			BitsByte result = default(BitsByte);
			while (true)
			{
				int num5 = (int)(vector3.X / 16f);
				int num6 = (int)(vector3.Y / 16f);
				int num7 = (int)vector3.X % 16 / 2;
				if (flag2)
				{
					num4 = num7;
				}
				bool flag7 = num7 != num4;
				if ((trackState == TrackState.OnBack || trackState == TrackState.OnTrack || trackState == TrackState.OnFront) && num5 != num2)
				{
					int num8 = (trackState != TrackState.OnBack) ? tile.FrontTrack() : tile.BackTrack();
					switch ((!(value2.X < 0f)) ? _rightSideConnection[num8] : _leftSideConnection[num8])
					{
					case 0:
						num6--;
						vector3.Y -= 2f;
						break;
					case 2:
						num6++;
						vector3.Y += 2f;
						break;
					}
				}
				TrackState trackState2 = TrackState.NoTrack;
				bool flag8 = false;
				if (num5 != num2 || num6 != num3)
				{
					if (flag2)
					{
						flag2 = false;
					}
					else
					{
						flag8 = true;
					}
					tile = Main.tile[num5, num6];
					if (tile == null)
					{
						tile = new Tile();
						Main.tile[num5, num6] = tile;
					}
					flag = ((tile.nactive() && tile.type == 314) ? true : false);
				}
				if (flag)
				{
					TrackState trackState3 = TrackState.NoTrack;
					int num9 = tile.FrontTrack();
					int num10 = tile.BackTrack();
					int num11 = _tileHeight[num9][num7];
					switch (num11)
					{
					case -4:
						if (trackState == TrackState.OnFront)
						{
							if (trackOnly)
							{
								vector3 -= vector5;
								num = 0f;
								trackState2 = TrackState.OnFront;
								flag6 = true;
							}
							else
							{
								trackState2 = TrackState.NoTrack;
								flag5 = true;
							}
						}
						break;
					case -1:
						if (trackState == TrackState.OnFront)
						{
							vector3 -= vector5;
							num = 0f;
							trackState2 = TrackState.OnFront;
							flag6 = true;
						}
						break;
					case -2:
						if (trackState != TrackState.OnFront)
						{
							break;
						}
						if (trackOnly)
						{
							vector3 -= vector5;
							num = 0f;
							trackState2 = TrackState.OnFront;
							flag6 = true;
							break;
						}
						if (value2.X < 0f)
						{
							float num14 = (float)(num5 * 16 + (num7 + 1) * 2) - vector3.X;
							vector3.X += num14;
							num += num14 / value2.X;
						}
						value2.X = 0f - value2.X;
						result[1] = true;
						trackState2 = TrackState.OnFront;
						break;
					case -3:
						if (trackState == TrackState.OnFront)
						{
							trackState = TrackState.NoTrack;
							vector4 = Vector2.Transform(matrix: (Velocity.X > 0f) ? ((_leftSideConnection[num9] != 2) ? Matrix.CreateRotationZ((float)Math.PI / 4f) : Matrix.CreateRotationZ(-(float)Math.PI / 4f)) : ((_rightSideConnection[num9] != 2) ? Matrix.CreateRotationZ(-(float)Math.PI / 4f) : Matrix.CreateRotationZ((float)Math.PI / 4f)), position: new Vector2(Velocity.X, 0f));
							vector4.X = Velocity.X;
							flag4 = true;
							num = 0f;
						}
						break;
					default:
					{
						float num12 = num6 * 16 + num11;
						if (num5 != num2 && trackState == TrackState.NoTrack && vector3.Y > num12 && vector3.Y - num12 < 2f)
						{
							flag8 = false;
							trackState = TrackState.AboveFront;
						}
						TrackState trackState4 = (!(vector3.Y < num12)) ? ((!(vector3.Y > num12)) ? TrackState.OnTrack : TrackState.BelowTrack) : TrackState.AboveTrack;
						if (num10 != -1)
						{
							float num13 = num6 * 16 + _tileHeight[num10][num7];
							trackState3 = ((!(vector3.Y < num13)) ? ((!(vector3.Y > num13)) ? TrackState.OnTrack : TrackState.BelowTrack) : TrackState.AboveTrack);
						}
						switch (trackState4)
						{
						case TrackState.OnTrack:
							trackState2 = ((trackState3 == TrackState.OnTrack) ? TrackState.OnTrack : TrackState.OnFront);
							break;
						case TrackState.AboveTrack:
							switch (trackState3)
							{
							case TrackState.OnTrack:
								trackState2 = TrackState.OnBack;
								break;
							case TrackState.BelowTrack:
								trackState2 = TrackState.AboveFront;
								break;
							case TrackState.AboveTrack:
								trackState2 = TrackState.AboveTrack;
								break;
							default:
								trackState2 = TrackState.AboveFront;
								break;
							}
							break;
						case TrackState.BelowTrack:
							switch (trackState3)
							{
							case TrackState.OnTrack:
								trackState2 = TrackState.OnBack;
								break;
							case TrackState.AboveTrack:
								trackState2 = TrackState.AboveBack;
								break;
							case TrackState.BelowTrack:
								trackState2 = TrackState.BelowTrack;
								break;
							default:
								trackState2 = TrackState.BelowTrack;
								break;
							}
							break;
						}
						break;
					}
					}
				}
				if (!flag8)
				{
					if (trackState != trackState2)
					{
						bool flag9 = false;
						if (flag7 || value2.Y > 0f)
						{
							switch (trackState)
							{
							case TrackState.AboveTrack:
								switch (trackState2)
								{
								case TrackState.AboveFront:
									trackState2 = TrackState.OnBack;
									break;
								case TrackState.AboveBack:
									trackState2 = TrackState.OnFront;
									break;
								case TrackState.AboveTrack:
									trackState2 = TrackState.OnTrack;
									break;
								}
								break;
							case TrackState.AboveFront:
							{
								TrackState trackState5 = trackState2;
								if (trackState5 == TrackState.BelowTrack)
								{
									trackState2 = TrackState.OnFront;
								}
								break;
							}
							case TrackState.AboveBack:
							{
								TrackState trackState6 = trackState2;
								if (trackState6 == TrackState.BelowTrack)
								{
									trackState2 = TrackState.OnBack;
								}
								break;
							}
							case TrackState.OnFront:
								trackState2 = TrackState.OnFront;
								flag9 = true;
								break;
							case TrackState.OnBack:
								trackState2 = TrackState.OnBack;
								flag9 = true;
								break;
							case TrackState.OnTrack:
							{
								int num15 = _tileHeight[tile.FrontTrack()][num7];
								int num16 = _tileHeight[tile.BackTrack()][num7];
								trackState2 = (followDown ? ((num15 >= num16) ? TrackState.OnFront : TrackState.OnBack) : ((!followUp) ? TrackState.OnFront : ((num15 >= num16) ? TrackState.OnBack : TrackState.OnFront)));
								flag9 = true;
								break;
							}
							}
							int num17 = -1;
							switch (trackState2)
							{
							case TrackState.OnTrack:
							case TrackState.OnFront:
								num17 = tile.FrontTrack();
								break;
							case TrackState.OnBack:
								num17 = tile.BackTrack();
								break;
							}
							if (num17 != -1)
							{
								if (!flag9 && Velocity.Y > Player.defaultGravity)
								{
									int num18 = (int)(Position.Y / 16f);
									if (fallStart < num18 - 1)
									{
										Main.PlaySound(2, (int)Position.X + Width / 2, (int)Position.Y + Height / 2, 53);
										WheelSparks(MinecartDust, Position, Width, Height, 10);
									}
								}
								if (trackState == TrackState.AboveFront && _trackType[num17] == 1)
								{
									flag3 = true;
								}
								value2.Y = 0f;
								vector3.Y = num6 * 16 + _tileHeight[num17][num7];
							}
						}
					}
				}
				else if (trackState2 == TrackState.OnFront || trackState2 == TrackState.OnBack || trackState2 == TrackState.OnTrack)
				{
					if (flag && _trackType[tile.FrontTrack()] == 1)
					{
						flag3 = true;
					}
					value2.Y = 0f;
				}
				if (trackState2 == TrackState.OnFront)
				{
					int num19 = tile.FrontTrack();
					if (_trackType[num19] == 2 && lastBoost.X == 0f && lastBoost.Y == 0f)
					{
						lastBoost = new Vector2(num5, num6);
						if (_boostLeft[num19])
						{
							result[4] = true;
						}
						else
						{
							result[5] = true;
						}
					}
				}
				num4 = num7;
				trackState = trackState2;
				num2 = num5;
				num3 = num6;
				if (num > 0f)
				{
					float num20 = vector3.X % 2f;
					float num21 = vector3.Y % 2f;
					float num22 = 3f;
					float num23 = 3f;
					if (value2.X < 0f)
					{
						num22 = num20 + 0.125f;
					}
					else if (value2.X > 0f)
					{
						num22 = 2f - num20;
					}
					if (value2.Y < 0f)
					{
						num23 = num21 + 0.125f;
					}
					else if (value2.Y > 0f)
					{
						num23 = 2f - num21;
					}
					if (num22 == 3f && num23 == 3f)
					{
						break;
					}
					float num24 = Math.Abs(num22 / value2.X);
					float num25 = Math.Abs(num23 / value2.Y);
					float num26 = (num24 < num25) ? num24 : num25;
					if (num26 > num)
					{
						vector5 = value2 * num;
						num = 0f;
					}
					else
					{
						vector5 = value2 * num26;
						num -= num26;
					}
					vector3 += vector5;
					continue;
				}
				if (lastBoost.X != (float)num2 || lastBoost.Y != (float)num3)
				{
					lastBoost = Vector2.Zero;
				}
				break;
			}
			if (flag3)
			{
				result[3] = true;
			}
			if (flag5)
			{
				Velocity.X = vector3.X - vector2.X;
				Velocity.Y = Player.defaultGravity;
			}
			else if (flag4)
			{
				result[2] = true;
				Velocity = vector4;
			}
			else if (result[1])
			{
				Velocity.X = 0f - Velocity.X;
				Position.X = vector3.X - _trackMagnetOffset.X - vector.X - Velocity.X;
				if (value2.Y == 0f)
				{
					Velocity.Y = 0f;
				}
			}
			else
			{
				if (flag6)
				{
					Velocity.X = vector3.X - vector2.X;
				}
				if (value2.Y == 0f)
				{
					Velocity.Y = 0f;
				}
			}
			Position.Y += vector3.Y - vector2.Y - Velocity.Y;
			Position.Y = (float)Math.Round(Position.Y, 2);
			switch (trackState)
			{
			case TrackState.OnTrack:
			case TrackState.OnFront:
			case TrackState.OnBack:
				result[0] = true;
				break;
			}
			return result;
		}

		public static bool FrameTrack(int i, int j, bool pound, bool mute = false)
		{
			int num = 0;
			Tile tile = Main.tile[i, j];
			if (tile == null)
			{
				tile = new Tile();
				Main.tile[i, j] = tile;
			}
			if (mute && tile.type != 314)
			{
				return false;
			}
			if (Main.tile[i - 1, j - 1] != null && Main.tile[i - 1, j - 1].type == 314)
			{
				num++;
			}
			if (Main.tile[i - 1, j] != null && Main.tile[i - 1, j].type == 314)
			{
				num += 2;
			}
			if (Main.tile[i - 1, j + 1] != null && Main.tile[i - 1, j + 1].type == 314)
			{
				num += 4;
			}
			if (Main.tile[i + 1, j - 1] != null && Main.tile[i + 1, j - 1].type == 314)
			{
				num += 8;
			}
			if (Main.tile[i + 1, j] != null && Main.tile[i + 1, j].type == 314)
			{
				num += 16;
			}
			if (Main.tile[i + 1, j + 1] != null && Main.tile[i + 1, j + 1].type == 314)
			{
				num += 32;
			}
			int num2 = tile.FrontTrack();
			int num3 = tile.BackTrack();
			if (_trackType == null)
			{
				return false;
			}
			int num4 = (num2 >= 0 && num2 < _trackType.Length) ? _trackType[num2] : 0;
			int num5 = -1;
			int num6 = -1;
			int[] array = _trackSwitchOptions[num];
			if (array == null)
			{
				if (pound)
				{
					return false;
				}
				tile.FrontTrack(0);
				tile.BackTrack(-1);
				return false;
			}
			if (!pound)
			{
				int num7 = -1;
				int num8 = -1;
				bool flag = false;
				for (int k = 0; k < array.Length; k++)
				{
					int num9 = array[k];
					if (num3 == array[k])
					{
						num6 = k;
					}
					if (_trackType[num9] != num4)
					{
						continue;
					}
					if (_leftSideConnection[num9] == -1 || _rightSideConnection[num9] == -1)
					{
						if (num2 == array[k])
						{
							num5 = k;
							flag = true;
						}
						if (num7 == -1)
						{
							num7 = k;
						}
					}
					else
					{
						if (num2 == array[k])
						{
							num5 = k;
							flag = false;
						}
						if (num8 == -1)
						{
							num8 = k;
						}
					}
				}
				if (num8 != -1)
				{
					if (num5 == -1 || flag)
					{
						num5 = num8;
					}
				}
				else
				{
					if (num5 == -1)
					{
						switch (num4)
						{
						case 2:
							return false;
						case 1:
							return false;
						}
						num5 = num7;
					}
					num6 = -1;
				}
			}
			else
			{
				for (int l = 0; l < array.Length; l++)
				{
					if (num2 == array[l])
					{
						num5 = l;
					}
					if (num3 == array[l])
					{
						num6 = l;
					}
				}
				int num10 = 0;
				int num11 = 0;
				for (int m = 0; m < array.Length; m++)
				{
					if (_trackType[array[m]] == num4)
					{
						if (_leftSideConnection[array[m]] == -1 || _rightSideConnection[array[m]] == -1)
						{
							num11++;
						}
						else
						{
							num10++;
						}
					}
				}
				if (num10 < 2 && num11 < 2)
				{
					return false;
				}
				bool flag2 = num10 == 0;
				bool flag3 = false;
				if (!flag2)
				{
					while (!flag3)
					{
						num6++;
						if (num6 >= array.Length)
						{
							num6 = -1;
							break;
						}
						if ((_leftSideConnection[array[num6]] != _leftSideConnection[array[num5]] || _rightSideConnection[array[num6]] != _rightSideConnection[array[num5]]) && _trackType[array[num6]] == num4 && _leftSideConnection[array[num6]] != -1 && _rightSideConnection[array[num6]] != -1)
						{
							flag3 = true;
						}
					}
				}
				if (!flag3)
				{
					do
					{
						num5++;
						if (num5 >= array.Length)
						{
							num5 = -1;
							do
							{
								num5++;
							}
							while (_trackType[array[num5]] != num4 || (_leftSideConnection[array[num5]] == -1 || _rightSideConnection[array[num5]] == -1) != flag2);
							break;
						}
					}
					while (_trackType[array[num5]] != num4 || (_leftSideConnection[array[num5]] == -1 || _rightSideConnection[array[num5]] == -1) != flag2);
				}
			}
			bool flag4 = false;
			switch (num5)
			{
			case -2:
				if (tile.FrontTrack() != _firstPressureFrame)
				{
					flag4 = true;
				}
				break;
			case -1:
				if (tile.FrontTrack() != 0)
				{
					flag4 = true;
				}
				break;
			default:
				if (tile.FrontTrack() != array[num5])
				{
					flag4 = true;
				}
				break;
			}
			if (num6 == -1)
			{
				if (tile.BackTrack() != -1)
				{
					flag4 = true;
				}
			}
			else if (tile.BackTrack() != array[num6])
			{
				flag4 = true;
			}
			switch (num5)
			{
			case -2:
				tile.FrontTrack(_firstPressureFrame);
				break;
			case -1:
				tile.FrontTrack(0);
				break;
			default:
				tile.FrontTrack((short)array[num5]);
				break;
			}
			if (num6 == -1)
			{
				tile.BackTrack(-1);
			}
			else
			{
				tile.BackTrack((short)array[num6]);
			}
			if (pound && flag4 && !mute)
			{
				WorldGen.KillTile(i, j, true);
			}
			return true;
		}

		public static bool GetOnTrack(int tileX, int tileY, ref Vector2 Position, int Width, int Height)
		{
			Tile tile = Main.tile[tileX, tileY];
			if (tile.type != 314)
			{
				return false;
			}
			Vector2 value = new Vector2((float)(Width / 2) - 25f, Height / 2);
			Vector2 value2 = Position + value;
			Vector2 value3 = value2 + _trackMagnetOffset;
			int num = (int)value3.X % 16 / 2;
			int num2 = -1;
			int num3 = 0;
			for (int i = num; i < 8; i++)
			{
				num3 = _tileHeight[tile.frameX][i];
				if (num3 >= 0)
				{
					num2 = i;
					break;
				}
			}
			if (num2 == -1)
			{
				for (int num4 = num - 1; num4 >= 0; num4--)
				{
					num3 = _tileHeight[tile.frameX][num4];
					if (num3 >= 0)
					{
						num2 = num4;
						break;
					}
				}
			}
			if (num2 == -1)
			{
				return false;
			}
			value3.X = tileX * 16 + num2 * 2;
			value3.Y = tileY * 16 + num3;
			value3 -= _trackMagnetOffset;
			value3 = (Position = value3 - value);
			return true;
		}

		public static bool OnTrack(Vector2 Position, int Width, int Height)
		{
			Vector2 value = Position + new Vector2((float)(Width / 2) - 25f, Height / 2);
			Vector2 vector = value + _trackMagnetOffset;
			int num = (int)(vector.X / 16f);
			int num2 = (int)(vector.Y / 16f);
			return Main.tile[num, num2].type == 314;
		}

		public static float TrackRotation(ref float rotation, Vector2 Position, int Width, int Height, bool followDown, bool followUp, Action<Vector2> MinecartDust)
		{
			Vector2 Position2 = Position;
			Vector2 Position3 = Position;
			Vector2 lastBoost = Vector2.Zero;
			Vector2 Velocity = new Vector2(-12f, 0f);
			TrackCollision(ref Position2, ref Velocity, ref lastBoost, Width, Height, followDown, followUp, 0, true, MinecartDust);
			Position2 += Velocity;
			Velocity = new Vector2(12f, 0f);
			TrackCollision(ref Position3, ref Velocity, ref lastBoost, Width, Height, followDown, followUp, 0, true, MinecartDust);
			Position3 += Velocity;
			float num = Position3.Y - Position2.Y;
			float num2 = Position3.X - Position2.X;
			float num3 = num / num2;
			float num4 = Position2.Y + (Position.X - Position2.X) * num3;
			float num5 = (Position.X - (float)(int)Position.X) * num3;
			rotation = (float)Math.Atan2(num, num2);
			return num4 - Position.Y + num5;
		}

		public static void HitTrackSwitch(Vector2 Position, int Width, int Height)
		{
			new Vector2((float)(Width / 2) - 25f, Height / 2);
			Vector2 value = Position + new Vector2((float)(Width / 2) - 25f, Height / 2);
			Vector2 vector = value + _trackMagnetOffset;
			int num = (int)(vector.X / 16f);
			int num2 = (int)(vector.Y / 16f);
			Wiring.HitSwitch(num, num2);
			NetMessage.SendData(59, -1, -1, "", num, num2);
		}

		public static void FlipSwitchTrack(int i, int j)
		{
			Tile tileTrack = Main.tile[i, j];
			short num = tileTrack.FrontTrack();
			if (num == -1)
			{
				return;
			}
			switch (_trackType[num])
			{
			case 1:
				break;
			case 0:
				if (tileTrack.BackTrack() != -1)
				{
					tileTrack.FrontTrack(tileTrack.BackTrack());
					tileTrack.BackTrack(num);
					NetMessage.SendTileSquare(-1, i, j, 1);
				}
				break;
			case 2:
				FrameTrack(i, j, true, true);
				NetMessage.SendTileSquare(-1, i, j, 1);
				break;
			}
		}

		public static void TrackColors(int i, int j, Tile trackTile, out int frontColor, out int backColor)
		{
			if (trackTile.type == 314)
			{
				frontColor = trackTile.color();
				backColor = frontColor;
				if (trackTile.frameY == -1)
				{
					return;
				}
				int num = _leftSideConnection[trackTile.frameX];
				int num2 = _rightSideConnection[trackTile.frameX];
				int num3 = _leftSideConnection[trackTile.frameY];
				int num4 = _rightSideConnection[trackTile.frameY];
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				for (int k = 0; k < 4; k++)
				{
					int num9;
					switch (k)
					{
					default:
						num9 = num;
						break;
					case 1:
						num9 = num2;
						break;
					case 2:
						num9 = num3;
						break;
					case 3:
						num9 = num4;
						break;
					}
					int num10;
					switch (num9)
					{
					case 0:
						num10 = -1;
						break;
					case 1:
						num10 = 0;
						break;
					case 2:
						num10 = 1;
						break;
					default:
						num10 = 0;
						break;
					}
					Tile tile = (k % 2 != 0) ? Main.tile[i + 1, j + num10] : Main.tile[i - 1, j + num10];
					int num11 = (tile != null && tile.active() && tile.type == 314) ? tile.color() : 0;
					switch (k)
					{
					default:
						num5 = num11;
						break;
					case 1:
						num6 = num11;
						break;
					case 2:
						num7 = num11;
						break;
					case 3:
						num8 = num11;
						break;
					}
				}
				if (num == num3)
				{
					if (num6 != 0)
					{
						frontColor = num6;
					}
					else if (num5 != 0)
					{
						frontColor = num5;
					}
					if (num8 != 0)
					{
						backColor = num8;
					}
					else if (num7 != 0)
					{
						backColor = num7;
					}
					return;
				}
				if (num2 == num4)
				{
					if (num5 != 0)
					{
						frontColor = num5;
					}
					else if (num6 != 0)
					{
						frontColor = num6;
					}
					if (num7 != 0)
					{
						backColor = num7;
					}
					else if (num8 != 0)
					{
						backColor = num8;
					}
					return;
				}
				if (num6 == 0)
				{
					if (num5 != 0)
					{
						frontColor = num5;
					}
				}
				else if (num5 != 0)
				{
					frontColor = ((num2 <= num) ? num6 : num5);
				}
				if (num8 == 0)
				{
					if (num7 != 0)
					{
						backColor = num7;
					}
				}
				else if (num7 != 0)
				{
					backColor = ((num4 <= num3) ? num8 : num7);
				}
			}
			else
			{
				frontColor = 0;
				backColor = 0;
			}
		}

		public static bool DrawLeftDecoration(int frameID)
		{
			if (frameID < 0 || frameID >= 36)
			{
				return false;
			}
			return _leftSideConnection[frameID] == 2;
		}

		public static bool DrawRightDecoration(int frameID)
		{
			if (frameID < 0 || frameID >= 36)
			{
				return false;
			}
			return _rightSideConnection[frameID] == 2;
		}

		public static bool DrawBumper(int frameID)
		{
			if (frameID < 0 || frameID >= 36)
			{
				return false;
			}
			if (_tileHeight[frameID][0] != -1)
			{
				return _tileHeight[frameID][7] == -1;
			}
			return true;
		}

		public static bool DrawBouncyBumper(int frameID)
		{
			if (frameID < 0 || frameID >= 36)
			{
				return false;
			}
			if (_tileHeight[frameID][0] != -2)
			{
				return _tileHeight[frameID][7] == -2;
			}
			return true;
		}

		public static void PlaceTrack(Tile trackCache, int style)
		{
			trackCache.active(true);
			trackCache.type = 314;
			trackCache.frameY = -1;
			switch (style)
			{
			case 0:
				trackCache.frameX = -1;
				break;
			case 1:
				trackCache.frameX = _firstPressureFrame;
				break;
			case 2:
				trackCache.frameX = _firstLeftBoostFrame;
				break;
			case 3:
				trackCache.frameX = _firstRightBoostFrame;
				break;
			}
		}

		public static int GetTrackItem(Tile trackCache)
		{
			switch (_trackType[trackCache.frameX])
			{
			case 0:
				return 2340;
			case 1:
				return 2492;
			case 2:
				return 2739;
			default:
				return 0;
			}
		}

		public static Rectangle GetSourceRect(int frameID, int animationFrame = 0)
		{
			if (frameID < 0 || frameID >= 40)
			{
				return new Rectangle(0, 0, 0, 0);
			}
			Vector2 vector = _texturePosition[frameID];
			Rectangle result = new Rectangle((int)vector.X, (int)vector.Y, 16, 16);
			if (frameID < 36 && _trackType[frameID] == 2)
			{
				result.Y += 18 * animationFrame;
			}
			return result;
		}

		public static void WheelSparks(Action<Vector2> DustAction, Vector2 Position, int Width, int Height, int sparkCount)
		{
			Vector2 value = new Vector2((float)(Width / 2) - 25f, Height / 2);
			Vector2 value2 = Position + value;
			Vector2 obj = value2 + _trackMagnetOffset;
			for (int i = 0; i < sparkCount; i++)
			{
				DustAction(obj);
			}
		}

		private static short FrontTrack(this Tile tileTrack)
		{
			return tileTrack.frameX;
		}

		private static void FrontTrack(this Tile tileTrack, short trackID)
		{
			tileTrack.frameX = trackID;
		}

		private static short BackTrack(this Tile tileTrack)
		{
			return tileTrack.frameY;
		}

		private static void BackTrack(this Tile tileTrack, short trackID)
		{
			tileTrack.frameY = trackID;
		}
	}
}
