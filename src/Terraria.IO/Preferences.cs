using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Terraria.IO
{
	public class Preferences
	{
		private Dictionary<string, object> _data = new Dictionary<string, object>();

		private readonly string _path;

		private readonly JsonSerializerSettings _serializerSettings;

		public readonly bool UseBson;

		private readonly object _lock = new object();

		public bool AutoSave;

		public event Action<Preferences> OnSave;

		public event Action<Preferences> OnLoad;

		public Preferences(string path, bool parseAllTypes = false, bool useBson = false)
		{
			_path = path;
			UseBson = useBson;
			if (parseAllTypes)
			{
				_serializerSettings = new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto,
					MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
				};
			}
			else
			{
				_serializerSettings = new JsonSerializerSettings();
			}
		}

		public bool Load()
		{
			lock (_lock)
			{
				if (File.Exists(_path))
				{
					try
					{
						if (!UseBson)
						{
							string value = File.ReadAllText(_path);
							_data = JsonConvert.DeserializeObject<Dictionary<string, object>>(value, _serializerSettings);
						}
						else
						{
							using (FileStream stream = File.OpenRead(_path))
							{
								using (BsonReader reader = new BsonReader(stream))
								{
									JsonSerializer jsonSerializer = JsonSerializer.Create(_serializerSettings);
									_data = jsonSerializer.Deserialize<Dictionary<string, object>>(reader);
								}
							}
						}
						if (_data == null)
						{
							_data = new Dictionary<string, object>();
						}
						if (this.OnLoad != null)
						{
							this.OnLoad(this);
						}
						return true;
					}
					catch (Exception)
					{
						return false;
					}
				}
				return false;
			}
		}

		public bool Save(bool createFile = true)
		{
			lock (_lock)
			{
				try
				{
					if (this.OnSave != null)
					{
						this.OnSave(this);
					}
					if (!createFile && !File.Exists(_path))
					{
						return false;
					}
					Directory.GetParent(_path).Create();
					if (!createFile)
					{
						File.SetAttributes(_path, FileAttributes.Normal);
					}
					if (!UseBson)
					{
						File.WriteAllText(_path, JsonConvert.SerializeObject(_data, Formatting.Indented, _serializerSettings));
						File.SetAttributes(_path, FileAttributes.Normal);
					}
					else
					{
						using (FileStream stream = File.Create(_path))
						{
							using (BsonWriter jsonWriter = new BsonWriter(stream))
							{
								File.SetAttributes(_path, FileAttributes.Normal);
								JsonSerializer jsonSerializer = JsonSerializer.Create(_serializerSettings);
								jsonSerializer.Serialize(jsonWriter, _data);
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Unable to write file at: " + _path);
					Console.WriteLine(ex.ToString());
					Monitor.Exit(_lock);
					return false;
				}
				return true;
			}
		}

		public void Put(string name, object value)
		{
			lock (_lock)
			{
				_data[name] = value;
				if (AutoSave)
				{
					Save();
				}
			}
		}

		public T Get<T>(string name, T defaultValue)
		{
			lock (_lock)
			{
				try
				{
					object value;
					if (_data.TryGetValue(name, out value))
					{
						if (value is T)
						{
							return (T)value;
						}
						return (T)Convert.ChangeType(value, typeof(T));
					}
					return defaultValue;
				}
				catch
				{
					return defaultValue;
				}
			}
		}

		public void Get<T>(string name, ref T currentValue)
		{
			currentValue = Get(name, currentValue);
		}
	}
}
