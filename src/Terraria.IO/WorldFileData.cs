using System;
using System.IO;
using Terraria.Utilities;

namespace Terraria.IO
{
	public class WorldFileData : FileData
	{
		public DateTime CreationTime;

		public int WorldSizeX;

		public int WorldSizeY;

		public bool IsValid = true;

		public string _worldSizeName;

		public bool IsExpertMode;

		public bool HasCorruption = true;

		public bool IsHardMode;

		public string WorldSizeName => _worldSizeName;

		public bool HasCrimson
		{
			get
			{
				return !HasCorruption;
			}
			set
			{
				HasCorruption = !value;
			}
		}

		public WorldFileData()
			: base("World")
		{
		}

		public WorldFileData(string path, bool cloudSave)
			: base("World", path, cloudSave)
		{
		}

		public override void SetAsActive()
		{
			Main.ActiveWorldFileData = this;
		}

		public void SetWorldSize(int x, int y)
		{
			WorldSizeX = x;
			WorldSizeY = y;
			switch (x)
			{
			case 4200:
				_worldSizeName = "Small";
				break;
			case 6400:
				_worldSizeName = "Medium";
				break;
			case 8400:
				_worldSizeName = "Large";
				break;
			default:
				_worldSizeName = "Unknown";
				break;
			}
		}

		public static WorldFileData FromInvalidWorld(string path, bool cloudSave)
		{
			WorldFileData worldFileData = new WorldFileData(path, cloudSave);
			worldFileData.IsExpertMode = false;
			worldFileData.Metadata = FileMetadata.FromCurrentSettings(FileType.World);
			worldFileData.SetWorldSize(1, 1);
			worldFileData.HasCorruption = true;
			worldFileData.IsHardMode = false;
			worldFileData.IsValid = false;
			worldFileData.Name = FileUtilities.GetFileName(path, false);
			if (!cloudSave)
			{
				worldFileData.CreationTime = File.GetCreationTime(path);
			}
			else
			{
				worldFileData.CreationTime = DateTime.Now;
			}
			return worldFileData;
		}

		public override void MoveToCloud()
		{
			if (!base.IsCloudSave)
			{
				string worldPathFromName = Main.GetWorldPathFromName(Name, true);
				if (FileUtilities.MoveToCloud(base.Path, worldPathFromName))
				{
					Main.LocalFavoriteData.ClearEntry(this);
					_isCloudSave = true;
					_path = worldPathFromName;
					Main.CloudFavoritesData.SaveFavorite(this);
				}
			}
		}

		public override void MoveToLocal()
		{
			if (base.IsCloudSave)
			{
				string worldPathFromName = Main.GetWorldPathFromName(Name, false);
				if (FileUtilities.MoveToLocal(base.Path, worldPathFromName))
				{
					Main.CloudFavoritesData.ClearEntry(this);
					_isCloudSave = false;
					_path = worldPathFromName;
					Main.LocalFavoriteData.SaveFavorite(this);
				}
			}
		}
	}
}
