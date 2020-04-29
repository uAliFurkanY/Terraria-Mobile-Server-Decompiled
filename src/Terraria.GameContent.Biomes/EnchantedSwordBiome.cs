using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	internal class EnchantedSwordBiome : MicroBiome
	{
		public override bool Place(Point origin, StructureMap structures)
		{
			Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
			WorldUtils.Gen(new Point(origin.X - 25, origin.Y - 25), new Shapes.Rectangle(50, 50), new Actions.TileScanner(0, 1).Output(dictionary));
			int num = dictionary[0] + dictionary[1];
			if (num < 1250)
			{
				return false;
			}
			Point result;
			bool flag = WorldUtils.Find(origin, Searches.Chain(new Searches.Up(1000), new Conditions.IsSolid().AreaOr(1, 50).Not()), out result);
			Point result2;
			if (WorldUtils.Find(origin, Searches.Chain(new Searches.Up(origin.Y - result.Y), new Conditions.IsTile(53)), out result2))
			{
				return false;
			}
			if (!flag)
			{
				return false;
			}
			result.Y += 50;
			ShapeData shapeData = new ShapeData();
			ShapeData shapeData2 = new ShapeData();
			Point point = new Point(origin.X, origin.Y + 20);
			Point point2 = new Point(origin.X, origin.Y + 30);
			float num2 = 0.8f + GenBase._random.NextFloat() * 0.5f;
			if (!structures.CanPlace(new Rectangle(point.X - (int)(20f * num2), point.Y - 20, (int)(40f * num2), 40)))
			{
				return false;
			}
			if (!structures.CanPlace(new Rectangle(origin.X, result.Y + 10, 1, origin.Y - result.Y - 9), 2))
			{
				return false;
			}
			WorldUtils.Gen(point, new Shapes.Slime(20, num2, 1f), Actions.Chain(new Modifiers.Blotches(2, 0.4), new Actions.ClearTile(true).Output(shapeData)));
			WorldUtils.Gen(point2, new Shapes.Mound(14, 14), Actions.Chain(new Modifiers.Blotches(2, 1, 0.8), new Actions.SetTile(0), new Actions.SetFrames(true).Output(shapeData2)));
			shapeData.Subtract(shapeData2, point, point2);
			WorldUtils.Gen(point, new ModShapes.InnerOutline(shapeData), Actions.Chain(new Actions.SetTile(2), new Actions.SetFrames(true)));
			WorldUtils.Gen(point, new ModShapes.All(shapeData), Actions.Chain(new Modifiers.RectangleMask(-40, 40, 0, 40), new Modifiers.IsEmpty(), new Actions.SetLiquid()));
			WorldUtils.Gen(point, new ModShapes.All(shapeData), Actions.Chain(new Actions.PlaceWall(68), new Modifiers.OnlyTiles(2), new Modifiers.Offset(0, 1), new ActionVines(3, 5)));
			ShapeData data = new ShapeData();
			WorldUtils.Gen(new Point(origin.X, result.Y + 10), new Shapes.Rectangle(1, origin.Y - result.Y - 9), Actions.Chain(new Modifiers.Blotches(2, 0.2), new Actions.ClearTile().Output(data), new Modifiers.Expand(1), new Modifiers.OnlyTiles(53), new Actions.SetTile(397).Output(data)));
			WorldUtils.Gen(new Point(origin.X, result.Y + 10), new ModShapes.All(data), new Actions.SetFrames(true));
			if (GenBase._random.Next(3) == 0)
			{
				WorldGen.PlaceTile(point2.X, point2.Y - 15, 187, true, false, -1, 17);
			}
			else
			{
				WorldGen.PlaceTile(point2.X, point2.Y - 15, 186, true, false, -1, 15);
			}
			WorldUtils.Gen(point2, new ModShapes.All(shapeData2), Actions.Chain(new Modifiers.Offset(0, -1), new Modifiers.OnlyTiles(2), new Modifiers.Offset(0, -1), new ActionGrass()));
			structures.AddStructure(new Rectangle(point.X - (int)(20f * num2), point.Y - 20, (int)(40f * num2), 40), 4);
			return true;
		}
	}
}
