using System;

namespace Terraria
{
	public class HitTile
	{
		public class HitTileObject
		{
			public int X;

			public int Y;

			public int damage;

			public int type;

			public int timeToLive;

			public int crackStyle;

			public HitTileObject()
			{
				Clear();
			}

			public void Clear()
			{
				X = 0;
				Y = 0;
				damage = 0;
				type = 0;
				timeToLive = 0;
				if (rand == null)
				{
					rand = new Random((int)DateTime.Now.Ticks);
				}
				for (crackStyle = rand.Next(4); crackStyle == lastCrack; crackStyle = rand.Next(4))
				{
				}
				lastCrack = crackStyle;
			}
		}

		internal const int UNUSED = 0;

		internal const int TILE = 1;

		internal const int WALL = 2;

		internal const int MAX_HITTILES = 20;

		internal const int TIMETOLIVE = 60;

		private static Random rand;

		private static int lastCrack = -1;

		public HitTileObject[] data;

		private int[] order;

		private int bufferLocation;

		public HitTile()
		{
			rand = new Random();
			data = new HitTileObject[21];
			order = new int[21];
			for (int i = 0; i <= 20; i++)
			{
				data[i] = new HitTileObject();
				order[i] = i;
			}
			bufferLocation = 0;
		}

		public int HitObject(int x, int y, int hitType)
		{
			HitTileObject hitTileObject;
			for (int i = 0; i <= 20; i++)
			{
				int num = order[i];
				hitTileObject = data[num];
				if (hitTileObject.type == hitType)
				{
					if (hitTileObject.X == x && hitTileObject.Y == y)
					{
						return num;
					}
				}
				else if (i != 0 && hitTileObject.type == 0)
				{
					break;
				}
			}
			hitTileObject = data[bufferLocation];
			hitTileObject.X = x;
			hitTileObject.Y = y;
			hitTileObject.type = hitType;
			return bufferLocation;
		}

		public void UpdatePosition(int tileId, int x, int y)
		{
			if (tileId >= 0 && tileId <= 20)
			{
				HitTileObject hitTileObject = data[tileId];
				hitTileObject.X = x;
				hitTileObject.Y = y;
			}
		}

		public int AddDamage(int tileId, int damageAmount, bool updateAmount = true)
		{
			if (tileId < 0 || tileId > 20)
			{
				return 0;
			}
			if (tileId == bufferLocation && damageAmount == 0)
			{
				return 0;
			}
			HitTileObject hitTileObject = data[tileId];
			if (!updateAmount)
			{
				return hitTileObject.damage + damageAmount;
			}
			hitTileObject.damage += damageAmount;
			hitTileObject.timeToLive = 60;
			if (tileId == bufferLocation)
			{
				bufferLocation = order[20];
				data[bufferLocation].Clear();
				for (int num = 20; num > 0; num--)
				{
					order[num] = order[num - 1];
				}
				order[0] = bufferLocation;
			}
			else
			{
				int num;
				for (num = 0; num <= 20 && order[num] != tileId; num++)
				{
				}
				while (num > 1)
				{
					int num2 = order[num - 1];
					order[num - 1] = order[num];
					order[num] = num2;
					num--;
				}
				order[1] = tileId;
			}
			return hitTileObject.damage;
		}

		public void Clear(int tileId)
		{
			if (tileId >= 0 && tileId <= 20)
			{
				data[tileId].Clear();
				int i;
				for (i = 0; i < 20 && order[i] != tileId; i++)
				{
				}
				for (; i < 20; i++)
				{
					order[i] = order[i + 1];
				}
				order[20] = tileId;
			}
		}

		public void Prune()
		{
			bool flag = false;
			for (int i = 0; i <= 20; i++)
			{
				HitTileObject hitTileObject = data[i];
				if (hitTileObject.type == 0)
				{
					continue;
				}
				Tile tile = Main.tile[hitTileObject.X, hitTileObject.Y];
				if (hitTileObject.timeToLive <= 1)
				{
					hitTileObject.Clear();
					flag = true;
					continue;
				}
				hitTileObject.timeToLive--;
				if ((double)hitTileObject.timeToLive < 12.0)
				{
					hitTileObject.damage -= 10;
				}
				else if ((double)hitTileObject.timeToLive < 24.0)
				{
					hitTileObject.damage -= 7;
				}
				else if ((double)hitTileObject.timeToLive < 36.0)
				{
					hitTileObject.damage -= 5;
				}
				else if ((double)hitTileObject.timeToLive < 48.0)
				{
					hitTileObject.damage -= 2;
				}
				if (hitTileObject.damage < 0)
				{
					hitTileObject.Clear();
					flag = true;
				}
				else if (hitTileObject.type == 1)
				{
					if (!tile.active())
					{
						hitTileObject.Clear();
						flag = true;
					}
				}
				else if (tile.wall == 0)
				{
					hitTileObject.Clear();
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			int num = 1;
			while (flag)
			{
				flag = false;
				for (int j = num; j < 20; j++)
				{
					if (data[order[j]].type == 0 && data[order[j + 1]].type != 0)
					{
						int num2 = order[j];
						order[j] = order[j + 1];
						order[j + 1] = num2;
						flag = true;
					}
				}
			}
		}
	}
}
