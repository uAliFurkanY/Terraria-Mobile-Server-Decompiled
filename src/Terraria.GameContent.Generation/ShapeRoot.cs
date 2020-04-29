using Microsoft.Xna.Framework;
using System;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	internal class ShapeRoot : GenShape
	{
		private float _angle;

		private float _startingSize;

		private float _endingSize;

		private float _distance;

		public ShapeRoot(float angle, float distance = 10f, float startingSize = 4f, float endingSize = 1f)
		{
			_angle = angle;
			_distance = distance;
			_startingSize = startingSize;
			_endingSize = endingSize;
		}

		private bool DoRoot(Point origin, GenAction action, float angle, float distance, float startingSize)
		{
			float num = origin.X;
			float num2 = origin.Y;
			for (float num3 = 0f; num3 < distance * 0.85f; num3 += 1f)
			{
				float num4 = num3 / distance;
				float num5 = MathHelper.Lerp(startingSize, _endingSize, num4);
				num += (float)Math.Cos(angle);
				num2 += (float)Math.Sin(angle);
				angle += GenBase._random.NextFloat() - 0.5f + GenBase._random.NextFloat() * (_angle - (float)Math.PI / 2f) * 0.1f * (1f - num4);
				angle = angle * 0.4f + 0.45f * MathHelper.Clamp(angle, _angle - 2f * (1f - 0.5f * num4), _angle + 2f * (1f - 0.5f * num4)) + MathHelper.Lerp(_angle, (float)Math.PI / 2f, num4) * 0.15f;
				for (int i = 0; i < (int)num5; i++)
				{
					for (int j = 0; j < (int)num5; j++)
					{
						if (!UnitApply(action, origin, (int)num + i, (int)num2 + j) && _quitOnFail)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public override bool Perform(Point origin, GenAction action)
		{
			return DoRoot(origin, action, _angle, _distance, _startingSize);
		}
	}
}
