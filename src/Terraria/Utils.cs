using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Terraria.DataStructures;

namespace Terraria
{
	public static class Utils
	{
		public delegate bool PerLinePoint(int x, int y);

		public delegate void LaserLineFraming(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distanceCovered, out Rectangle frame, out Vector2 origin, out Color color);

		public const long MaxCoins = 999999999L;

		private const ulong RANDOM_MULTIPLIER = 25214903917uL;

		private const ulong RANDOM_ADD = 11uL;

		private const ulong RANDOM_MASK = 281474976710655uL;

		public static Dictionary<SpriteFont, float[]> charLengths = new Dictionary<SpriteFont, float[]>();

		public static float SmoothStep(float min, float max, float x)
		{
			return MathHelper.Clamp((x - min) / (max - min), 0f, 1f);
		}

		public static Dictionary<string, string> ParseArguements(string[] args)
		{
			string text = null;
			string text2 = "";
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].Length == 0)
				{
					continue;
				}
				if (args[i][0] == '-' || args[i][0] == '+')
				{
					if (text != null)
					{
						dictionary.Add(text.ToLower(), text2);
						text2 = "";
					}
					text = args[i];
					text2 = "";
				}
				else
				{
					if (text2 != "")
					{
						text2 += " ";
					}
					text2 += args[i];
				}
			}
			if (text != null)
			{
				dictionary.Add(text.ToLower(), text2);
				text2 = "";
			}
			return dictionary;
		}

		public static void Swap<T>(ref T t1, ref T t2)
		{
			T val = t1;
			t1 = t2;
			t2 = val;
		}

		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			if (value.CompareTo(max) > 0)
			{
				return max;
			}
			if (value.CompareTo(min) < 0)
			{
				return min;
			}
			return value;
		}

		public static string[] FixArgs(string[] brokenArgs)
		{
			ArrayList arrayList = new ArrayList();
			string text = "";
			for (int i = 0; i < brokenArgs.Length; i++)
			{
				if (brokenArgs[i].StartsWith("-"))
				{
					if (text != "")
					{
						arrayList.Add(text);
						text = "";
					}
					else
					{
						arrayList.Add("");
					}
					arrayList.Add(brokenArgs[i]);
				}
				else
				{
					if (text != "")
					{
						text += " ";
					}
					text += brokenArgs[i];
				}
			}
			arrayList.Add(text);
			string[] array = new string[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		public static string[] WordwrapString(string text, SpriteFont font, int maxWidth, int maxLines, out int lineAmount)
		{
			string[] array = new string[maxLines];
			int num = 0;
			List<string> list = new List<string>(text.Split('\n'));
			List<string> list2 = new List<string>(list[0].Split(' '));
			for (int i = 1; i < list.Count; i++)
			{
				list2.Add("\n");
				list2.AddRange(list[i].Split(' '));
			}
			bool flag = true;
			while (list2.Count > 0)
			{
				string text2 = list2[0];
				string str = " ";
				if (list2.Count == 1)
				{
					str = "";
				}
				if (text2 == "\n")
				{
					string[] array2;
					string[] array3 = array2 = array;
					int num2 = num++;
					IntPtr ıntPtr = (IntPtr)num2;
					array3[num2] = array2[(long)ıntPtr] + text2;
					if (num >= maxLines)
					{
						break;
					}
					list2.RemoveAt(0);
				}
				else if (flag)
				{
					if (font.MeasureString(text2).X > (float)maxWidth)
					{
						string arg = string.Concat(text2[0]);
						int num3 = 1;
						while (font.MeasureString(arg + text2[num3] + '-').X <= (float)maxWidth)
						{
							arg += text2[num3++];
						}
						arg += '-';
						array[num++] = arg + " ";
						if (num >= maxLines)
						{
							break;
						}
						list2.RemoveAt(0);
						list2.Insert(0, text2.Substring(num3));
					}
					else
					{
						string[] array4;
						string[] array5 = array4 = array;
						int num4 = num;
						IntPtr ıntPtr2 = (IntPtr)num4;
						array5[num4] = array4[(long)ıntPtr2] + text2 + str;
						flag = false;
						list2.RemoveAt(0);
					}
				}
				else if (font.MeasureString(array[num] + text2).X > (float)maxWidth)
				{
					num++;
					if (num >= maxLines)
					{
						break;
					}
					flag = true;
				}
				else
				{
					string[] array6;
					string[] array7 = array6 = array;
					int num5 = num;
					IntPtr ıntPtr3 = (IntPtr)num5;
					array7[num5] = array6[(long)ıntPtr3] + text2 + str;
					flag = false;
					list2.RemoveAt(0);
				}
			}
			lineAmount = num;
			if (lineAmount == maxLines)
			{
				lineAmount--;
			}
			return array;
		}

		public static Rectangle CenteredRectangle(Vector2 center, Vector2 size)
		{
			return new Rectangle((int)(center.X - size.X / 2f), (int)(center.Y - size.Y / 2f), (int)size.X, (int)size.Y);
		}

		public static Vector2 Vector2FromElipse(Vector2 angleVector, Vector2 elipseSizes)
		{
			if (elipseSizes == Vector2.Zero)
			{
				return Vector2.Zero;
			}
			if (angleVector == Vector2.Zero)
			{
				return Vector2.Zero;
			}
			angleVector.Normalize();
			Vector2 value = Vector2.Normalize(elipseSizes);
			value = Vector2.One / value;
			angleVector *= value;
			angleVector.Normalize();
			return angleVector * elipseSizes / 2f;
		}

		public static bool FloatIntersect(float r1StartX, float r1StartY, float r1Width, float r1Height, float r2StartX, float r2StartY, float r2Width, float r2Height)
		{
			if (r1StartX > r2StartX + r2Width || r1StartY > r2StartY + r2Height || r1StartX + r1Width < r2StartX || r1StartY + r1Height < r2StartY)
			{
				return false;
			}
			return true;
		}

		public static long CoinsCount(out bool overFlowing, Item[] inv, params int[] ignoreSlots)
		{
			List<int> list = new List<int>(ignoreSlots);
			long num = 0L;
			for (int i = 0; i < inv.Length; i++)
			{
				if (!list.Contains(i))
				{
					switch (inv[i].type)
					{
					case 71:
						num += inv[i].stack;
						break;
					case 72:
						num += inv[i].stack * 100;
						break;
					case 73:
						num += inv[i].stack * 10000;
						break;
					case 74:
						num += inv[i].stack * 1000000;
						break;
					}
					if (num >= 999999999)
					{
						overFlowing = true;
						return 999999999L;
					}
				}
			}
			overFlowing = false;
			return num;
		}

		public static int[] CoinsSplit(long count)
		{
			int[] array = new int[4];
			long num = 0L;
			long num2 = 1000000L;
			for (int num3 = 3; num3 >= 0; num3--)
			{
				array[num3] = (int)((count - num) / num2);
				num += array[num3] * num2;
				num2 /= 100;
			}
			return array;
		}

		public static long CoinsCombineStacks(out bool overFlowing, params long[] coinCounts)
		{
			long num = 0L;
			foreach (long num2 in coinCounts)
			{
				num += num2;
				if (num >= 999999999)
				{
					overFlowing = true;
					return 999999999L;
				}
			}
			overFlowing = false;
			return num;
		}

		public static byte[] ToByteArray(this string str)
		{
			byte[] array = new byte[str.Length * 2];
			Buffer.BlockCopy(str.ToCharArray(), 0, array, 0, array.Length);
			return array;
		}

		public static float NextFloat(this Random r)
		{
			return (float)r.NextDouble();
		}

		public static Rectangle Frame(this Texture2D tex, int horizontalFrames = 1, int verticalFrames = 1, int frameX = 0, int frameY = 0)
		{
			int num = tex.Width / horizontalFrames;
			int num2 = tex.Height / verticalFrames;
			return new Rectangle(num * frameX, num2 * frameY, num, num2);
		}

		public static Vector2 OriginFlip(this Rectangle rect, Vector2 origin, SpriteEffects effects)
		{
			if (effects.HasFlag(SpriteEffects.FlipHorizontally))
			{
				origin.X = (float)rect.Width - origin.X;
			}
			if (effects.HasFlag(SpriteEffects.FlipVertically))
			{
				origin.Y = (float)rect.Height - origin.Y;
			}
			return origin;
		}

		public static Vector2 Size(this Texture2D tex)
		{
			return new Vector2(tex.Width, tex.Height);
		}

		public static void WriteRGB(this BinaryWriter bb, Color c)
		{
			bb.Write(c.R);
			bb.Write(c.G);
			bb.Write(c.B);
		}

		public static void WriteVector2(this BinaryWriter bb, Vector2 v)
		{
			bb.Write(v.X);
			bb.Write(v.Y);
		}

		public static Color ReadRGB(this BinaryReader bb)
		{
			return new Color(bb.ReadByte(), bb.ReadByte(), bb.ReadByte());
		}

		public static Vector2 ReadVector2(this BinaryReader bb)
		{
			return new Vector2(bb.ReadSingle(), bb.ReadSingle());
		}

		public static Vector2 Left(this Rectangle r)
		{
			return new Vector2(r.X, r.Y + r.Height / 2);
		}

		public static Vector2 Right(this Rectangle r)
		{
			return new Vector2(r.X + r.Width, r.Y + r.Height / 2);
		}

		public static Vector2 Top(this Rectangle r)
		{
			return new Vector2(r.X + r.Width / 2, r.Y);
		}

		public static Vector2 Bottom(this Rectangle r)
		{
			return new Vector2(r.X + r.Width / 2, r.Y + r.Height);
		}

		public static Vector2 TopLeft(this Rectangle r)
		{
			return new Vector2(r.X, r.Y);
		}

		public static Vector2 TopRight(this Rectangle r)
		{
			return new Vector2(r.X + r.Width, r.Y);
		}

		public static Vector2 BottomLeft(this Rectangle r)
		{
			return new Vector2(r.X, r.Y + r.Height);
		}

		public static Vector2 BottomRight(this Rectangle r)
		{
			return new Vector2(r.X + r.Width, r.Y + r.Height);
		}

		public static Vector2 Center(this Rectangle r)
		{
			return new Vector2(r.X + r.Width / 2, r.Y + r.Height / 2);
		}

		public static Vector2 Size(this Rectangle r)
		{
			return new Vector2(r.Width, r.Height);
		}

		public static float Distance(this Rectangle r, Vector2 point)
		{
			if (FloatIntersect(r.Left, r.Top, r.Width, r.Height, point.X, point.Y, 0f, 0f))
			{
				return 0f;
			}
			if (point.X >= (float)r.Left && point.X <= (float)r.Right)
			{
				if (point.Y < (float)r.Top)
				{
					return (float)r.Top - point.Y;
				}
				return point.Y - (float)r.Bottom;
			}
			if (point.Y >= (float)r.Top && point.Y <= (float)r.Bottom)
			{
				if (point.X < (float)r.Left)
				{
					return (float)r.Left - point.X;
				}
				return point.X - (float)r.Right;
			}
			if (point.X < (float)r.Left)
			{
				if (point.Y < (float)r.Top)
				{
					return Vector2.Distance(point, r.TopLeft());
				}
				return Vector2.Distance(point, r.BottomLeft());
			}
			if (point.Y < (float)r.Top)
			{
				return Vector2.Distance(point, r.TopRight());
			}
			return Vector2.Distance(point, r.BottomRight());
		}

		public static float ToRotation(this Vector2 v)
		{
			return (float)Math.Atan2(v.Y, v.X);
		}

		public static Vector2 ToRotationVector2(this float f)
		{
			return new Vector2((float)Math.Cos(f), (float)Math.Sin(f));
		}

		public static Vector2 RotatedBy(this Vector2 spinningpoint, double radians, Vector2 center = default(Vector2))
		{
			float num = (float)Math.Cos(radians);
			float num2 = (float)Math.Sin(radians);
			Vector2 vector = spinningpoint - center;
			Vector2 result = center;
			result.X += vector.X * num - vector.Y * num2;
			result.Y += vector.X * num2 + vector.Y * num;
			return result;
		}

		public static Vector2 RotatedByRandom(this Vector2 spinninpoint, double maxRadians)
		{
			return spinninpoint.RotatedBy(Main.rand.NextDouble() * maxRadians - Main.rand.NextDouble() * maxRadians);
		}

		public static Vector2 Floor(this Vector2 vec)
		{
			vec.X = (int)vec.X;
			vec.Y = (int)vec.Y;
			return vec;
		}

		public static bool HasNaNs(this Vector2 vec)
		{
			if (!float.IsNaN(vec.X))
			{
				return float.IsNaN(vec.Y);
			}
			return true;
		}

		public static bool Between(this Vector2 vec, Vector2 minimum, Vector2 maximum)
		{
			if (vec.X >= minimum.X && vec.X <= maximum.X && vec.Y >= minimum.Y)
			{
				return vec.Y <= maximum.Y;
			}
			return false;
		}

		public static Vector2 ToVector2(this Point p)
		{
			return new Vector2(p.X, p.Y);
		}

		public static Point16 ToTileCoordinates16(this Vector2 vec)
		{
			return new Point16((int)vec.X >> 4, (int)vec.Y >> 4);
		}

		public static Point ToTileCoordinates(this Vector2 vec)
		{
			return new Point((int)vec.X >> 4, (int)vec.Y >> 4);
		}

		public static Point ToPoint(this Vector2 v)
		{
			return new Point((int)v.X, (int)v.Y);
		}

		public static Vector2 XY(this Vector4 vec)
		{
			return new Vector2(vec.X, vec.Y);
		}

		public static Vector2 ZW(this Vector4 vec)
		{
			return new Vector2(vec.Z, vec.W);
		}

		public static Vector3 XZW(this Vector4 vec)
		{
			return new Vector3(vec.X, vec.Z, vec.W);
		}

		public static Vector3 YZW(this Vector4 vec)
		{
			return new Vector3(vec.Y, vec.Z, vec.W);
		}

		public static Color MultiplyRGB(this Color firstColor, Color secondColor)
		{
			return new Color((byte)((float)(firstColor.R * secondColor.R) / 255f), (byte)((float)(firstColor.G * secondColor.G) / 255f), (byte)((float)(firstColor.B * secondColor.B) / 255f));
		}

		public static Color MultiplyRGBA(this Color firstColor, Color secondColor)
		{
			return new Color((byte)((float)(firstColor.R * secondColor.R) / 255f), (byte)((float)(firstColor.G * secondColor.G) / 255f), (byte)((float)(firstColor.B * secondColor.B) / 255f), (byte)((float)(firstColor.A * secondColor.A) / 255f));
		}

		public static string Hex3(this Color color)
		{
			return (color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2")).ToLower();
		}

		public static string Hex4(this Color color)
		{
			return (color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2") + color.A.ToString("X2")).ToLower();
		}

		public static int ToDirectionInt(this bool value)
		{
			if (!value)
			{
				return -1;
			}
			return 1;
		}

		public static int ToInt(this bool value)
		{
			if (!value)
			{
				return 0;
			}
			return 1;
		}

		public static float AngleLerp(this float curAngle, float targetAngle, float amount)
		{
			float angle;
			if (targetAngle < curAngle)
			{
				float num = targetAngle + (float)Math.PI * 2f;
				angle = ((num - curAngle > curAngle - targetAngle) ? MathHelper.Lerp(curAngle, targetAngle, amount) : MathHelper.Lerp(curAngle, num, amount));
			}
			else
			{
				if (!(targetAngle > curAngle))
				{
					return curAngle;
				}
				float num = targetAngle - (float)Math.PI * 2f;
				angle = ((targetAngle - curAngle > curAngle - num) ? MathHelper.Lerp(curAngle, num, amount) : MathHelper.Lerp(curAngle, targetAngle, amount));
			}
			return MathHelper.WrapAngle(angle);
		}

		public static float AngleTowards(this float curAngle, float targetAngle, float maxChange)
		{
			curAngle = MathHelper.WrapAngle(curAngle);
			targetAngle = MathHelper.WrapAngle(targetAngle);
			if (curAngle < targetAngle)
			{
				if (targetAngle - curAngle > (float)Math.PI)
				{
					curAngle += (float)Math.PI * 2f;
				}
			}
			else if (curAngle - targetAngle > (float)Math.PI)
			{
				curAngle -= (float)Math.PI * 2f;
			}
			curAngle += MathHelper.Clamp(targetAngle - curAngle, 0f - maxChange, maxChange);
			return MathHelper.WrapAngle(curAngle);
		}

		public static bool deepCompare(this int[] firstArray, int[] secondArray)
		{
			if (firstArray == null && secondArray == null)
			{
				return true;
			}
			if (firstArray != null && secondArray != null)
			{
				if (firstArray.Length != secondArray.Length)
				{
					return false;
				}
				for (int i = 0; i < firstArray.Length; i++)
				{
					if (firstArray[i] != secondArray[i])
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public static bool PlotLine(Point16 p0, Point16 p1, PerLinePoint plot, bool jump = true)
		{
			return PlotLine(p0.X, p0.Y, p1.X, p1.Y, plot, jump);
		}

		public static bool PlotLine(Point p0, Point p1, PerLinePoint plot, bool jump = true)
		{
			return PlotLine(p0.X, p0.Y, p1.X, p1.Y, plot, jump);
		}

		private static bool PlotLine(int x0, int y0, int x1, int y1, PerLinePoint plot, bool jump = true)
		{
			if (x0 == x1 && y0 == y1)
			{
				return plot(x0, y0);
			}
			bool flag = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
			if (flag)
			{
				Swap(ref x0, ref y0);
				Swap(ref x1, ref y1);
			}
			int num = Math.Abs(x1 - x0);
			int num2 = Math.Abs(y1 - y0);
			int num3 = num / 2;
			int num4 = y0;
			int num5 = (x0 < x1) ? 1 : (-1);
			int num6 = (y0 < y1) ? 1 : (-1);
			for (int i = x0; i != x1; i += num5)
			{
				if (flag)
				{
					if (!plot(num4, i))
					{
						return false;
					}
				}
				else if (!plot(i, num4))
				{
					return false;
				}
				num3 -= num2;
				if (num3 >= 0)
				{
					continue;
				}
				num4 += num6;
				if (!jump)
				{
					if (flag)
					{
						if (!plot(num4, i))
						{
							return false;
						}
					}
					else if (!plot(i, num4))
					{
						return false;
					}
				}
				num3 += num;
			}
			return true;
		}

		public static int RandomNext(ref ulong seed, int bits)
		{
			seed = RandomNextSeed(seed);
			return (int)(seed >> 48 - bits);
		}

		public static ulong RandomNextSeed(ulong seed)
		{
			return (seed * 25214903917L + 11) & 0xFFFFFFFFFFFFL;
		}

		public static float RandomFloat(ref ulong seed)
		{
			return (float)RandomNext(ref seed, 24) / 16777216f;
		}

		public static int RandomInt(ref ulong seed, int max)
		{
			if ((max & -max) == max)
			{
				return (int)((long)max * (long)RandomNext(ref seed, 31) >> 31);
			}
			int num;
			int num2;
			do
			{
				num = RandomNext(ref seed, 31);
				num2 = num % max;
			}
			while (num - num2 + (max - 1) < 0);
			return num2;
		}

		public static int RandomInt(ref ulong seed, int min, int max)
		{
			return RandomInt(ref seed, max - min) + min;
		}

		public static bool PlotTileLine(Vector2 start, Vector2 end, float width, PerLinePoint plot)
		{
			float scaleFactor = width / 2f;
			Vector2 value = end - start;
			Vector2 vector = value / value.Length();
			Vector2 value2 = new Vector2(0f - vector.Y, vector.X) * scaleFactor;
			Point point = (start - value2).ToTileCoordinates();
			Point point2 = (start + value2).ToTileCoordinates();
			Point point3 = start.ToTileCoordinates();
			Point point4 = end.ToTileCoordinates();
			Point lineMinOffset = new Point(point.X - point3.X, point.Y - point3.Y);
			Point lineMaxOffset = new Point(point2.X - point3.X, point2.Y - point3.Y);
			return PlotLine(point3.X, point3.Y, point4.X, point4.Y, (int x, int y) => PlotLine(x + lineMinOffset.X, y + lineMinOffset.Y, x + lineMaxOffset.X, y + lineMaxOffset.Y, plot, false));
		}

		public static bool PlotTileTale(Vector2 start, Vector2 end, float width, PerLinePoint plot)
		{
			float halfWidth = width / 2f;
			Vector2 value = end - start;
			Vector2 vector = value / value.Length();
			Vector2 perpOffset = new Vector2(0f - vector.Y, vector.X);
			Point pointStart = start.ToTileCoordinates();
			Point point = end.ToTileCoordinates();
			int length = 0;
			PlotLine(pointStart.X, pointStart.Y, point.X, point.Y, delegate
			{
				length++;
				return true;
			});
			length--;
			int curLength = 0;
			return PlotLine(pointStart.X, pointStart.Y, point.X, point.Y, delegate(int x, int y)
			{
				float scaleFactor = 1f - (float)curLength / (float)length;
				curLength++;
				Point point2 = (start - perpOffset * halfWidth * scaleFactor).ToTileCoordinates();
				Point point3 = (start + perpOffset * halfWidth * scaleFactor).ToTileCoordinates();
				Point point4 = new Point(point2.X - pointStart.X, point2.Y - pointStart.Y);
				Point point5 = new Point(point3.X - pointStart.X, point3.Y - pointStart.Y);
				return PlotLine(x + point4.X, y + point4.Y, x + point5.X, y + point5.Y, plot, false);
			});
		}

		public static int RandomConsecutive(double random, int odds)
		{
			return (int)Math.Log(1.0 - random, 1.0 / (double)odds);
		}

		public static Vector2 RandomVector2(Random random, float min, float max)
		{
			return new Vector2((max - min) * (float)random.NextDouble() + min, (max - min) * (float)random.NextDouble() + min);
		}

		public static T SelectRandom<T>(Random random, params T[] choices)
		{
			return choices[random.Next(choices.Length)];
		}

		public static void DrawBorderStringFourWay(SpriteBatch sb, SpriteFont font, string text, float x, float y, Color textColor, Color borderColor, Vector2 origin, float scale = 1f)
		{
			Color color = borderColor;
			Vector2 zero = Vector2.Zero;
			for (int i = 0; i < 5; i++)
			{
				switch (i)
				{
				case 0:
					zero.X = x - 2f;
					zero.Y = y;
					break;
				case 1:
					zero.X = x + 2f;
					zero.Y = y;
					break;
				case 2:
					zero.X = x;
					zero.Y = y - 2f;
					break;
				case 3:
					zero.X = x;
					zero.Y = y + 2f;
					break;
				default:
					zero.X = x;
					zero.Y = y;
					color = textColor;
					break;
				}
				sb.DrawString(font, text, zero, color, 0f, origin, scale, SpriteEffects.None, 0f);
			}
		}

		public static Vector2 DrawBorderString(SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1f, float anchorx = 0f, float anchory = 0f, int stringLimit = -1)
		{
			if (stringLimit != -1 && text.Length > stringLimit)
			{
				text.Substring(0, stringLimit);
			}
			SpriteFont fontMouseText = Main.fontMouseText;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					sb.DrawString(fontMouseText, text, pos + new Vector2(i, j), Color.Black, 0f, new Vector2(anchorx, anchory) * fontMouseText.MeasureString(text), scale, SpriteEffects.None, 0f);
				}
			}
			sb.DrawString(fontMouseText, text, pos, color, 0f, new Vector2(anchorx, anchory) * fontMouseText.MeasureString(text), scale, SpriteEffects.None, 0f);
			return fontMouseText.MeasureString(text) * scale;
		}

		public static Vector2 DrawBorderStringBig(SpriteBatch sb, string text, Vector2 pos, Color color, float scale = 1f, float anchorx = 0f, float anchory = 0f, int stringLimit = -1)
		{
			if (stringLimit != -1 && text.Length > stringLimit)
			{
				text.Substring(0, stringLimit);
			}
			SpriteFont fontDeathText = Main.fontDeathText;
			for (int i = -1; i < 2; i++)
			{
				for (int j = -1; j < 2; j++)
				{
					sb.DrawString(fontDeathText, text, pos + new Vector2(i, j), Color.Black, 0f, new Vector2(anchorx, anchory) * fontDeathText.MeasureString(text), scale, SpriteEffects.None, 0f);
				}
			}
			sb.DrawString(fontDeathText, text, pos, color, 0f, new Vector2(anchorx, anchory) * fontDeathText.MeasureString(text), scale, SpriteEffects.None, 0f);
			return fontDeathText.MeasureString(text) * scale;
		}

		public static void DrawInvBG(SpriteBatch sb, Rectangle R, Color c = default(Color))
		{
			DrawInvBG(sb, R.X, R.Y, R.Width, R.Height, c);
		}

		public static void DrawInvBG(SpriteBatch sb, float x, float y, float w, float h, Color c = default(Color))
		{
			DrawInvBG(sb, (int)x, (int)y, (int)w, (int)h, c);
		}

		public static void DrawInvBG(SpriteBatch sb, int x, int y, int w, int h, Color c = default(Color))
		{
			if (c == default(Color))
			{
				c = new Color(63, 65, 151, 255) * 0.785f;
			}
			Texture2D inventoryBack13Texture = Main.inventoryBack13Texture;
			if (w < 20)
			{
				w = 20;
			}
			if (h < 20)
			{
				h = 20;
			}
			sb.Draw(inventoryBack13Texture, new Rectangle(x, y, 10, 10), new Rectangle(0, 0, 10, 10), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + 10, y, w - 20, 10), new Rectangle(10, 0, 10, 10), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + w - 10, y, 10, 10), new Rectangle(inventoryBack13Texture.Width - 10, 0, 10, 10), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x, y + 10, 10, h - 20), new Rectangle(0, 10, 10, 10), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + 10, y + 10, w - 20, h - 20), new Rectangle(10, 10, 10, 10), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + w - 10, y + 10, 10, h - 20), new Rectangle(inventoryBack13Texture.Width - 10, 10, 10, 10), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x, y + h - 10, 10, 10), new Rectangle(0, inventoryBack13Texture.Height - 10, 10, 10), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + 10, y + h - 10, w - 20, 10), new Rectangle(10, inventoryBack13Texture.Height - 10, 10, 10), c);
			sb.Draw(inventoryBack13Texture, new Rectangle(x + w - 10, y + h - 10, 10, 10), new Rectangle(inventoryBack13Texture.Width - 10, inventoryBack13Texture.Height - 10, 10, 10), c);
		}

		public static void DrawLaser(SpriteBatch sb, Texture2D tex, Vector2 start, Vector2 end, Vector2 scale, LaserLineFraming framing)
		{
			Vector2 vector = start;
			Vector2 vector2 = Vector2.Normalize(end - start);
			float num = (end - start).Length();
			float rotation = vector2.ToRotation() - (float)Math.PI / 2f;
			if (vector2.HasNaNs())
			{
				return;
			}
			float distanceCovered;
			Rectangle frame;
			Vector2 origin;
			Color color;
			framing(0, vector, num, default(Rectangle), out distanceCovered, out frame, out origin, out color);
			sb.Draw(tex, vector, frame, color, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
			num -= distanceCovered * scale.Y;
			vector += vector2 * ((float)frame.Height - origin.Y) * scale.Y;
			if (num > 0f)
			{
				float num2 = 0f;
				while (num2 + 1f < num)
				{
					framing(1, vector, num - num2, frame, out distanceCovered, out frame, out origin, out color);
					if (num - num2 < (float)frame.Height)
					{
						distanceCovered *= (num - num2) / (float)frame.Height;
						frame.Height = (int)(num - num2);
					}
					sb.Draw(tex, vector, frame, color, rotation, origin, scale, SpriteEffects.None, 0f);
					num2 += distanceCovered * scale.Y;
					vector += vector2 * distanceCovered * scale.Y;
				}
			}
			framing(2, vector, num, default(Rectangle), out distanceCovered, out frame, out origin, out color);
			sb.Draw(tex, vector, frame, color, rotation, origin, scale, SpriteEffects.None, 0f);
		}

		public static void DrawLine(SpriteBatch spriteBatch, Point start, Point end, Color color)
		{
			DrawLine(spriteBatch, new Vector2(start.X << 4, start.Y << 4), new Vector2(end.X << 4, end.Y << 4), color);
		}

		public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
		{
			float num = Vector2.Distance(start, end);
			Vector2 vector = (end - start) / num;
			Vector2 value = start;
			Vector2 screenPosition = Main.screenPosition;
			float rotation = vector.ToRotation();
			for (float num2 = 0f; num2 <= num; num2 += 4f)
			{
				float num3 = num2 / num;
				spriteBatch.Draw(Main.blackTileTexture, value - screenPosition, null, new Color(new Vector4(num3, num3, num3, 1f) * color.ToVector4()), rotation, Vector2.Zero, 0.25f, SpriteEffects.None, 0f);
				value = start + num2 * vector;
			}
		}

		public static void DrawRect(SpriteBatch spriteBatch, Rectangle rect, Color color)
		{
			DrawRect(spriteBatch, new Point(rect.X, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height), color);
		}

		public static void DrawRect(SpriteBatch spriteBatch, Point start, Point end, Color color)
		{
			DrawRect(spriteBatch, new Vector2(start.X << 4, start.Y << 4), new Vector2((end.X << 4) - 4, (end.Y << 4) - 4), color);
		}

		public static void DrawRect(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
		{
			DrawLine(spriteBatch, start, new Vector2(start.X, end.Y), color);
			DrawLine(spriteBatch, start, new Vector2(end.X, start.Y), color);
			DrawLine(spriteBatch, end, new Vector2(start.X, end.Y), color);
			DrawLine(spriteBatch, end, new Vector2(end.X, start.Y), color);
		}

		public static void DrawRect(SpriteBatch spriteBatch, Vector2 topLeft, Vector2 topRight, Vector2 bottomRight, Vector2 bottomLeft, Color color)
		{
			DrawLine(spriteBatch, topLeft, topRight, color);
			DrawLine(spriteBatch, topRight, bottomRight, color);
			DrawLine(spriteBatch, bottomRight, bottomLeft, color);
			DrawLine(spriteBatch, bottomLeft, topLeft, color);
		}

		public static void DrawCursorSingle(SpriteBatch sb, Color color, float rot = float.NaN, float scale = 1f, Vector2 manualPosition = default(Vector2), int cursorSlot = 0, int specialMode = 0)
		{
			bool flag = false;
			bool flag2 = true;
			bool flag3 = true;
			Vector2 origin = Vector2.Zero;
			Vector2 value = new Vector2(Main.mouseX, Main.mouseY);
			if (manualPosition != Vector2.Zero)
			{
				value = manualPosition;
			}
			if (float.IsNaN(rot))
			{
				rot = 0f;
			}
			else
			{
				flag = true;
				rot -= (float)Math.PI * 3f / 4f;
			}
			if (cursorSlot == 4 || cursorSlot == 5)
			{
				flag2 = false;
				origin = new Vector2(8f);
				if (flag && specialMode == 0)
				{
					float num = rot;
					if (num < 0f)
					{
						num += (float)Math.PI * 2f;
					}
					for (float num2 = 0f; num2 < 4f; num2 += 1f)
					{
						if (Math.Abs(num - (float)Math.PI / 2f * num2) <= (float)Math.PI / 4f)
						{
							rot = (float)Math.PI / 2f * num2;
							break;
						}
					}
				}
			}
			Vector2 value2 = Vector2.One;
			if ((Main.ThickMouse && cursorSlot == 0) || cursorSlot == 1)
			{
				value2 = Main.DrawThickCursor(cursorSlot == 1);
			}
			if (flag2)
			{
				sb.Draw(Main.cursorTextures[cursorSlot], value + value2 + Vector2.One, null, color.MultiplyRGB(new Color(0.2f, 0.2f, 0.2f, 0.5f)), rot, origin, scale * 1.1f, SpriteEffects.None, 0f);
			}
			if (flag3)
			{
				sb.Draw(Main.cursorTextures[cursorSlot], value + value2, null, color, rot, origin, scale, SpriteEffects.None, 0f);
			}
		}
	}
}
