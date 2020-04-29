using Microsoft.Xna.Framework;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	internal class ActionStalagtite : GenAction
	{
		public override bool Apply(Point origin, int x, int y, params object[] args)
		{
			WorldGen.PlaceTight(x, y, 165);
			return UnitApply(origin, x, y, args);
		}
	}
}
