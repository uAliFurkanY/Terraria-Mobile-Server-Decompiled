using Microsoft.Xna.Framework;
using System;

namespace Terraria.World.Generation
{
	internal static class Shapes
	{
		public class Circle : GenShape
		{
			private int _verticalRadius;

			private int _horizontalRadius;

			public Circle(int radius)
			{
				_verticalRadius = radius;
				_horizontalRadius = radius;
			}

			public Circle(int horizontalRadius, int verticalRadius)
			{
				_horizontalRadius = horizontalRadius;
				_verticalRadius = verticalRadius;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				int num = (_horizontalRadius + 1) * (_horizontalRadius + 1);
				for (int i = origin.Y - _verticalRadius; i <= origin.Y + _verticalRadius; i++)
				{
					float num2 = (float)_horizontalRadius / (float)_verticalRadius * (float)(i - origin.Y);
					int num3 = Math.Min(_horizontalRadius, (int)Math.Sqrt((float)num - num2 * num2));
					for (int j = origin.X - num3; j <= origin.X + num3; j++)
					{
						if (!UnitApply(action, origin, j, i) && _quitOnFail)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public class HalfCircle : GenShape
		{
			private int _radius;

			public HalfCircle(int radius)
			{
				_radius = radius;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				int num = (_radius + 1) * (_radius + 1);
				for (int i = origin.Y - _radius; i <= origin.Y; i++)
				{
					int num2 = Math.Min(_radius, (int)Math.Sqrt(num - (i - origin.Y) * (i - origin.Y)));
					for (int j = origin.X - num2; j <= origin.X + num2; j++)
					{
						if (!UnitApply(action, origin, j, i) && _quitOnFail)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public class Slime : GenShape
		{
			private int _radius;

			private float _xScale;

			private float _yScale;

			public Slime(int radius)
			{
				_radius = radius;
				_xScale = 1f;
				_yScale = 1f;
			}

			public Slime(int radius, float xScale, float yScale)
			{
				_radius = radius;
				_xScale = xScale;
				_yScale = yScale;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				float num = _radius;
				int num2 = (_radius + 1) * (_radius + 1);
				for (int i = origin.Y - (int)(num * _yScale); i <= origin.Y; i++)
				{
					float num3 = (float)(i - origin.Y) / _yScale;
					int num4 = (int)Math.Min((float)_radius * _xScale, _xScale * (float)Math.Sqrt((float)num2 - num3 * num3));
					for (int j = origin.X - num4; j <= origin.X + num4; j++)
					{
						if (!UnitApply(action, origin, j, i) && _quitOnFail)
						{
							return false;
						}
					}
				}
				for (int k = origin.Y + 1; k <= origin.Y + (int)(num * _yScale * 0.5f) - 1; k++)
				{
					float num5 = (float)(k - origin.Y) * (2f / _yScale);
					int num6 = (int)Math.Min((float)_radius * _xScale, _xScale * (float)Math.Sqrt((float)num2 - num5 * num5));
					for (int l = origin.X - num6; l <= origin.X + num6; l++)
					{
						if (!UnitApply(action, origin, l, k) && _quitOnFail)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public class Rectangle : GenShape
		{
			private int _width;

			private int _height;

			public Rectangle(int width, int height)
			{
				_width = width;
				_height = height;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				for (int i = origin.X; i < origin.X + _width; i++)
				{
					for (int j = origin.Y; j < origin.Y + _height; j++)
					{
						if (!UnitApply(action, origin, i, j) && _quitOnFail)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		public class Tail : GenShape
		{
			private float _width;

			private Vector2 _endOffset;

			public Tail(float width, Vector2 endOffset)
			{
				_width = width * 16f;
				_endOffset = endOffset * 16f;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				Vector2 vector = new Vector2(origin.X << 4, origin.Y << 4);
				return Utils.PlotTileTale(vector, vector + _endOffset, _width, (int x, int y) => UnitApply(action, origin, x, y) || !_quitOnFail);
			}
		}

		public class Mound : GenShape
		{
			private int _halfWidth;

			private int _height;

			public Mound(int halfWidth, int height)
			{
				_halfWidth = halfWidth;
				_height = height;
			}

			public override bool Perform(Point origin, GenAction action)
			{
				float num = _halfWidth;
				for (int i = -_halfWidth; i <= _halfWidth; i++)
				{
					int num2 = Math.Min(_height, (int)((0f - (float)(_height + 1) / (num * num)) * ((float)i + num) * ((float)i - num)));
					for (int j = 0; j < num2; j++)
					{
						if (!UnitApply(action, origin, i + origin.X, origin.Y - j) && _quitOnFail)
						{
							return false;
						}
					}
				}
				return true;
			}
		}
	}
}
