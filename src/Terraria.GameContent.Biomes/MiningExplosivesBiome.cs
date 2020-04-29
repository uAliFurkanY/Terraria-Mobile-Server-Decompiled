using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	internal class MiningExplosivesBiome : MicroBiome
	{
		public override bool Place(Point origin, StructureMap structures)
		{
			if (WorldGen.SolidTile(origin.X, origin.Y))
			{
				return false;
			}
			ushort type = Utils.SelectRandom<ushort>(GenBase._random, (ushort)((WorldGen.goldBar == 19) ? 8 : 169), (ushort)((WorldGen.silverBar == 21) ? 9 : 168), (ushort)((WorldGen.ironBar == 22) ? 6 : 167), (ushort)((WorldGen.copperBar == 20) ? 7 : 166));
			double num = GenBase._random.NextDouble() * 2.0 - 1.0;
			if (!WorldUtils.Find(origin, Searches.Chain((num > 0.0) ? ((GenSearch)new Searches.Right(40)) : ((GenSearch)new Searches.Left(40)), new Conditions.IsSolid()), out origin))
			{
				return false;
			}
			if (!WorldUtils.Find(origin, Searches.Chain(new Searches.Down(80), new Conditions.IsSolid()), out origin))
			{
				return false;
			}
			ShapeData shapeData = new ShapeData();
			Ref<int> @ref = new Ref<int>(0);
			Ref<int> ref2 = new Ref<int>(0);
			WorldUtils.Gen(origin, new ShapeRunner(10f, 20, new Vector2((float)num, 1f)).Output(shapeData), Actions.Chain(new Modifiers.Blotches(), new Actions.Scanner(@ref), new Modifiers.IsSolid(), new Actions.Scanner(ref2)));
			if (ref2.Value < @ref.Value / 2)
			{
				return false;
			}
			Rectangle area = new Rectangle(origin.X - 15, origin.Y - 10, 30, 20);
			if (!structures.CanPlace(area))
			{
				return false;
			}
			WorldUtils.Gen(origin, new ModShapes.All(shapeData), new Actions.SetTile(type, true));
			WorldUtils.Gen(new Point(origin.X - (int)(num * -5.0), origin.Y - 5), new Shapes.Circle(5), Actions.Chain(new Modifiers.Blotches(), new Actions.ClearTile(true)));
			bool flag = true;
			Point result;
			flag &= WorldUtils.Find(new Point(origin.X - ((num > 0.0) ? 3 : (-3)), origin.Y - 3), Searches.Chain(new Searches.Down(10), new Conditions.IsSolid()), out result);
			Point result2;
			if (!(flag & WorldUtils.Find(new Point(origin.X - ((num > 0.0) ? (-3) : 3), origin.Y - 3), Searches.Chain(new Searches.Down(10), new Conditions.IsSolid()), out result2)))
			{
				return false;
			}
			result.Y--;
			result2.Y--;
			Tile tile = GenBase._tiles[result.X, result.Y + 1];
			tile.slope(0);
			tile.halfBrick(false);
			for (int i = -1; i <= 1; i++)
			{
				WorldUtils.ClearTile(result2.X + i, result2.Y);
				Tile tile2 = GenBase._tiles[result2.X + i, result2.Y + 1];
				if (!WorldGen.SolidOrSlopedTile(tile2))
				{
					tile2.ResetToType(1);
					tile2.active(true);
				}
				tile2.slope(0);
				tile2.halfBrick(false);
				WorldUtils.TileFrame(result2.X + i, result2.Y + 1, true);
			}
			WorldGen.PlaceTile(result.X, result.Y, 141);
			WorldGen.PlaceTile(result2.X, result2.Y, 411, true, true);
			WorldUtils.WireLine(result, result2);
			structures.AddStructure(area, 5);
			return true;
		}
	}
}
