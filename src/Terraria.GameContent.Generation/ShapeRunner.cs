using Microsoft.Xna.Framework;
using System;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	internal class ShapeRunner : GenShape
	{
		private float _startStrength;

		private int _steps;

		private Vector2 _startVelocity;

		public ShapeRunner(float strength, int steps, Vector2 velocity)
		{
			_startStrength = strength;
			_steps = steps;
			_startVelocity = velocity;
		}

		public override bool Perform(Point origin, GenAction action)
		{
			float num = _steps;
			float num2 = _steps;
			double num3 = _startStrength;
			Vector2 vector = new Vector2(origin.X, origin.Y);
			Vector2 vector2 = (_startVelocity == Vector2.Zero) ? Utils.RandomVector2(GenBase._random, -1f, 1f) : _startVelocity;
			while (num > 0f && num3 > 0.0)
			{
				num3 = _startStrength * (num / num2);
				num -= 1f;
				int num4 = Math.Max(1, (int)((double)vector.X - num3 * 0.5));
				int num5 = Math.Max(1, (int)((double)vector.Y - num3 * 0.5));
				int num6 = Math.Min(GenBase._worldWidth, (int)((double)vector.X + num3 * 0.5));
				int num7 = Math.Min(GenBase._worldHeight, (int)((double)vector.Y + num3 * 0.5));
				for (int i = num4; i < num6; i++)
				{
					for (int j = num5; j < num7; j++)
					{
						if (!((double)(Math.Abs((float)i - vector.X) + Math.Abs((float)j - vector.Y)) >= num3 * 0.5 * (1.0 + (double)GenBase._random.Next(-10, 11) * 0.015)))
						{
							UnitApply(action, origin, i, j);
						}
					}
				}
				int num8 = (int)(num3 / 50.0) + 1;
				num -= (float)num8;
				vector += vector2;
				for (int k = 0; k < num8; k++)
				{
					vector += vector2;
					vector2 += Utils.RandomVector2(GenBase._random, -0.5f, 0.5f);
				}
				vector2 += Utils.RandomVector2(GenBase._random, -0.5f, 0.5f);
				vector2 = Vector2.Clamp(vector2, -Vector2.One, Vector2.One);
			}
			return true;
		}
	}
}
