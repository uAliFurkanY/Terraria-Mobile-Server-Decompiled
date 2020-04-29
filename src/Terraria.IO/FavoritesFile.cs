using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Terraria.Utilities;

namespace Terraria.IO
{
	public class FavoritesFile
	{
		public readonly string Path;

		public readonly bool IsCloudSave;

		private Dictionary<string, Dictionary<string, bool>> _data = new Dictionary<string, Dictionary<string, bool>>();

		public FavoritesFile(string path, bool isCloud)
		{
			Path = path;
			IsCloudSave = isCloud;
		}

		public void SaveFavorite(FileData fileData)
		{
			if (!_data.ContainsKey(fileData.Type))
			{
				_data.Add(fileData.Type, new Dictionary<string, bool>());
			}
			_data[fileData.Type][fileData.GetFileName()] = fileData.IsFavorite;
			Save();
		}

		public void ClearEntry(FileData fileData)
		{
			if (_data.ContainsKey(fileData.Type))
			{
				_data[fileData.Type].Remove(fileData.GetFileName());
				Save();
			}
		}

		public bool IsFavorite(FileData fileData)
		{
			if (!_data.ContainsKey(fileData.Type))
			{
				return false;
			}
			string fileName = fileData.GetFileName();
			bool value;
			if (_data[fileData.Type].TryGetValue(fileName, out value))
			{
				return value;
			}
			return false;
		}

		public void Save()
		{
			FileUtilities.WriteAllBytes(Path, Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(_data, Formatting.Indented)), IsCloudSave);
		}

		public void Load()
		{
			if (!FileUtilities.Exists(Path, IsCloudSave))
			{
				_data.Clear();
				return;
			}
			string @string = Encoding.ASCII.GetString(FileUtilities.ReadAllBytes(Path, IsCloudSave));
			_data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, bool>>>(@string);
			if (_data == null)
			{
				_data = new Dictionary<string, Dictionary<string, bool>>();
			}
		}
	}
}
