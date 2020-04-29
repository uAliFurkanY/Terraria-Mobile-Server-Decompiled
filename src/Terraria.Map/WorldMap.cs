using System;
using System.IO;
using Terraria.IO;
using Terraria.Social;
using Terraria.Utilities;

namespace Terraria.Map
{
	public class WorldMap
	{
		public readonly int MaxWidth;

		public readonly int MaxHeight;

		private MapTile[,] _tiles;

		public MapTile this[int x, int y] => _tiles[x, y];

		public WorldMap(int maxWidth, int maxHeight)
		{
			MaxWidth = maxWidth;
			MaxHeight = maxHeight;
			_tiles = new MapTile[MaxWidth, MaxHeight];
		}

		public void ConsumeUpdate(int x, int y)
		{
			_tiles[x, y].IsChanged = false;
		}

		public void Update(int x, int y, byte light)
		{
			_tiles[x, y] = MapHelper.CreateMapTile(x, y, light);
		}

		public void SetTile(int x, int y, ref MapTile tile)
		{
			_tiles[x, y] = tile;
		}

		public bool IsRevealed(int x, int y)
		{
			return _tiles[x, y].Light > 0;
		}

		public bool UpdateLighting(int x, int y, byte light)
		{
			MapTile other = _tiles[x, y];
			MapTile mapTile = MapHelper.CreateMapTile(x, y, Math.Max(other.Light, light));
			if (mapTile.Equals(ref other))
			{
				return false;
			}
			_tiles[x, y] = mapTile;
			return true;
		}

		public bool UpdateType(int x, int y)
		{
			MapTile mapTile = MapHelper.CreateMapTile(x, y, _tiles[x, y].Light);
			if (mapTile.Equals(ref _tiles[x, y]))
			{
				return false;
			}
			_tiles[x, y] = mapTile;
			return true;
		}

		public void UnlockMapSection(int sectionX, int sectionY)
		{
		}

		public void Load()
		{
			bool ısCloudSave = Main.ActivePlayerFileData.IsCloudSave;
			if ((!ısCloudSave || SocialAPI.Cloud != null) && Main.mapEnabled)
			{
				string text = Main.playerPathName.Substring(0, Main.playerPathName.Length - 4);
				string text2 = text + Path.DirectorySeparatorChar + Main.worldID + ".map";
				if (!FileUtilities.Exists(text2, ısCloudSave))
				{
					Main.MapFileMetadata = FileMetadata.FromCurrentSettings(FileType.Map);
				}
				else
				{
					using (MemoryStream input = new MemoryStream(FileUtilities.ReadAllBytes(text2, ısCloudSave)))
					{
						using (BinaryReader binaryReader = new BinaryReader(input))
						{
							try
							{
								int num = binaryReader.ReadInt32();
								if (num <= Main.maxSupportSaveRelease)
								{
									if (num <= 91)
									{
										MapHelper.LoadMapVersion1(binaryReader, num);
									}
									else
									{
										MapHelper.LoadMapVersion2(binaryReader, num);
									}
									Main.clearMap = true;
									Main.loadMap = true;
									Main.loadMapLock = true;
									Main.refreshMap = false;
								}
							}
							catch (Exception value)
							{
								using (StreamWriter streamWriter = new StreamWriter("client-crashlog.txt", true))
								{
									streamWriter.WriteLine(DateTime.Now);
									streamWriter.WriteLine(value);
									streamWriter.WriteLine("");
								}
								if (!ısCloudSave)
								{
									File.Copy(text2, text2 + ".bad", true);
								}
								Clear();
							}
						}
					}
				}
			}
		}

		public void Save()
		{
			MapHelper.SaveMap();
		}

		public void Clear()
		{
			for (int i = 0; i < MaxWidth; i++)
			{
				for (int j = 0; j < MaxHeight; j++)
				{
					_tiles[i, j].Clear();
				}
			}
		}
	}
}
