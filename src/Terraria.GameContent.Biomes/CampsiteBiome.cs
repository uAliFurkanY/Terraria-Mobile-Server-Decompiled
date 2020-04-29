using Microsoft.Xna.Framework;
using Terraria.World.Generation;

namespace Terraria.GameContent.Biomes
{
	internal class CampsiteBiome : MicroBiome
	{
		public override bool Place(Point origin, StructureMap structures)
		{
			Ref<int> @ref = new Ref<int>(0);
			Ref<int> ref2 = new Ref<int>(0);
			WorldUtils.Gen(origin, new Shapes.Circle(10), Actions.Chain(new Actions.Scanner(ref2), new Modifiers.IsSolid(), new Actions.Scanner(@ref)));
			if (@ref.Value < ref2.Value - 5)
			{
				return false;
			}
			int num = GenBase._random.Next(6, 10);
			int num2 = GenBase._random.Next(5);
			if (!structures.CanPlace(new Rectangle(origin.X - num, origin.Y - num, num * 2, num * 2)))
			{
				return false;
			}
			ShapeData data = new ShapeData();
			Point origin2 = origin;
			Shapes.Slime shape = new Shapes.Slime(num);
			GenAction[] array = new GenAction[6]
			{
				new Modifiers.Blotches(num2, num2, num2, 1).Output(data),
				new Modifiers.Offset(0, -2),
				new Modifiers.OnlyTiles(53),
				new Actions.SetTile(397, true),
				null,
				null
			};
			byte[] types = new byte[1];
			array[4] = new Modifiers.OnlyWalls(types);
			array[5] = new Actions.PlaceWall(16);
			WorldUtils.Gen(origin2, shape, Actions.Chain(array));
			Point origin3 = origin;
			ModShapes.All shape2 = new ModShapes.All(data);
			GenAction[] array2 = new GenAction[5]
			{
				new Actions.ClearTile(),
				new Actions.SetLiquid(0, 0),
				new Actions.SetFrames(true),
				null,
				null
			};
			byte[] types2 = new byte[1];
			array2[3] = new Modifiers.OnlyWalls(types2);
			array2[4] = new Actions.PlaceWall(16);
			WorldUtils.Gen(origin3, shape2, Actions.Chain(array2));
			Point result;
			if (!WorldUtils.Find(origin, Searches.Chain(new Searches.Down(10), new Conditions.IsSolid()), out result))
			{
				return false;
			}
			int num3 = result.Y - 1;
			bool flag = GenBase._random.Next() % 2 == 0;
			if (GenBase._random.Next() % 10 != 0)
			{
				int num4 = GenBase._random.Next(1, 4);
				int num5 = flag ? 4 : (-(num >> 1));
				for (int i = 0; i < num4; i++)
				{
					int num6 = GenBase._random.Next(1, 3);
					for (int j = 0; j < num6; j++)
					{
						WorldGen.PlaceTile(origin.X + num5 - i, num3 - j, 331);
					}
				}
			}
			int num7 = (num - 3) * ((!flag) ? 1 : (-1));
			if (GenBase._random.Next() % 10 != 0)
			{
				WorldGen.PlaceTile(origin.X + num7, num3, 186);
			}
			if (GenBase._random.Next() % 10 != 0)
			{
				WorldGen.PlaceTile(origin.X, num3, 215, true);
				if (GenBase._tiles[origin.X, num3].active() && GenBase._tiles[origin.X, num3].type == 215)
				{
					GenBase._tiles[origin.X, num3].frameY += 36;
					GenBase._tiles[origin.X - 1, num3].frameY += 36;
					GenBase._tiles[origin.X + 1, num3].frameY += 36;
					GenBase._tiles[origin.X, num3 - 1].frameY += 36;
					GenBase._tiles[origin.X - 1, num3 - 1].frameY += 36;
					GenBase._tiles[origin.X + 1, num3 - 1].frameY += 36;
				}
			}
			structures.AddStructure(new Rectangle(origin.X - num, origin.Y - num, num * 2, num * 2), 4);
			return true;
		}
	}
}
