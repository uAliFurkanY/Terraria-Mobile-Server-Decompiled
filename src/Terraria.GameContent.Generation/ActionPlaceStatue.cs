using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.World.Generation;

namespace Terraria.GameContent.Generation
{
	internal class ActionPlaceStatue : GenAction
	{
		private int _statueIndex;

		public ActionPlaceStatue(int index = -1)
		{
			_statueIndex = index;
		}

		public override bool Apply(Point origin, int x, int y, params object[] args)
		{
			Point16 point = (_statueIndex != -1) ? WorldGen.statueList[_statueIndex] : WorldGen.statueList[GenBase._random.Next(2, WorldGen.statueList.Length)];
			WorldGen.PlaceTile(x, y, point.X, true, false, -1, point.Y);
			return UnitApply(origin, x, y, args);
		}
	}
}
