using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	internal class ShapeBranch : GenShape
	{
		private Point _offset;

		private List<Point> _endPoints;

		public ShapeBranch()
		{
			_offset = new Point(10, -5);
		}

		public ShapeBranch(Point offset)
		{
			_offset = offset;
		}

		public ShapeBranch(double angle, double distance)
		{
			_offset = new Point((int)(Math.Cos(angle) * distance), (int)(Math.Sin(angle) * distance));
		}

		private bool PerformSegment(Point origin, GenAction action, Point start, Point end, int size)
		{
			size = Math.Max(1, size);
			for (int i = -(size >> 1); i < size - (size >> 1); i++)
			{
				for (int j = -(size >> 1); j < size - (size >> 1); j++)
				{
					if (!Utils.PlotLine(new Point(start.X + i, start.Y + j), end, (int tileX, int tileY) => UnitApply(action, origin, tileX, tileY) || !_quitOnFail, false))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override bool Perform(Point origin, GenAction action)
		{
			float num = new Vector2(_offset.X, _offset.Y).Length();
			int num2 = (int)(num / 6f);
			if (_endPoints != null)
			{
				_endPoints.Add(new Point(origin.X + _offset.X, origin.Y + _offset.Y));
			}
			if (!PerformSegment(origin, action, origin, new Point(origin.X + _offset.X, origin.Y + _offset.Y), num2))
			{
				return false;
			}
			int num3 = (int)(num / 8f);
			for (int i = 0; i < num3; i++)
			{
				float num4 = ((float)i + 1f) / ((float)num3 + 1f);
				Point point = new Point((int)(num4 * (float)_offset.X), (int)(num4 * (float)_offset.Y));
				Vector2 spinningpoint = new Vector2(_offset.X - point.X, _offset.Y - point.Y);
				spinningpoint = spinningpoint.RotatedBy(((float)GenBase._random.NextDouble() * 0.5f + 1f) * (float)((GenBase._random.Next(2) != 0) ? 1 : (-1))) * 0.75f;
				Point point2 = new Point((int)spinningpoint.X + point.X, (int)spinningpoint.Y + point.Y);
				if (_endPoints != null)
				{
					_endPoints.Add(new Point(point2.X + origin.X, point2.Y + origin.Y));
				}
				if (!PerformSegment(origin, action, new Point(point.X + origin.X, point.Y + origin.Y), new Point(point2.X + origin.X, point2.Y + origin.Y), num2 - 1))
				{
					return false;
				}
			}
			return true;
		}

		public ShapeBranch OutputEndpoints(List<Point> endpoints)
		{
			_endPoints = endpoints;
			return this;
		}
	}
}
