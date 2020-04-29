using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace Terraria.Localization
{
	public class LanguageManager
	{
		public static LanguageManager Instance = new LanguageManager();

		private readonly Dictionary<string, LocalizedText> _localizedTexts = new Dictionary<string, LocalizedText>();

		private readonly Dictionary<string, List<string>> _categoryGroupedKeys = new Dictionary<string, List<string>>();

		private GameCulture _fallbackCulture = GameCulture.DefaultCulture;

		public GameCulture ActiveCulture
		{
			get;
			private set;
		}

		public event LanguageChangeCallback OnLanguageChanging;

		public event LanguageChangeCallback OnLanguageChanged;

		private LanguageManager()
		{
			_localizedTexts[""] = LocalizedText.Empty;
		}

		public int GetCategorySize(string name)
		{
			if (_categoryGroupedKeys.ContainsKey(name))
			{
				return _categoryGroupedKeys[name].Count;
			}
			return 0;
		}

		public void SetLanguage(int legacyId)
		{
			GameCulture language = GameCulture.FromLegacyId(legacyId);
			SetLanguage(language);
		}

		public void SetLanguage(string cultureName)
		{
			GameCulture language = GameCulture.FromName(cultureName);
			SetLanguage(language);
		}

		private void SetAllTextValuesToKeys()
		{
			foreach (KeyValuePair<string, LocalizedText> localizedText in _localizedTexts)
			{
				localizedText.Value.SetValue(localizedText.Key);
			}
		}

		public void SetLanguage(GameCulture culture)
		{
			if (ActiveCulture != culture)
			{
				if (culture != _fallbackCulture && ActiveCulture != _fallbackCulture)
				{
					SetAllTextValuesToKeys();
					LoadLanguage(_fallbackCulture);
				}
				LoadLanguage(culture);
				ActiveCulture = culture;
				Thread.CurrentThread.CurrentCulture = culture.CultureInfo;
				Thread.CurrentThread.CurrentUICulture = culture.CultureInfo;
				if (this.OnLanguageChanged != null)
				{
					this.OnLanguageChanged(this);
				}
				SpriteFont fontMouseText = Main.fontMouseText;
			}
		}

		private void LoadLanguage(GameCulture culture)
		{
			LoadFilesForCulture(culture);
			if (this.OnLanguageChanging != null)
			{
				this.OnLanguageChanging(this);
			}
			ProcessCopyCommandsInTexts();
		}

		private void LoadFilesForCulture(GameCulture culture)
		{
			LoadLanguageFromFileText(File.ReadAllText("Localisation/" + culture.Name + "/" + culture.Name + ".json"));
			LoadLanguageFromFileText(File.ReadAllText("Localisation/" + culture.Name + "/" + culture.Name + ".Game.json"));
			LoadLanguageFromFileText(File.ReadAllText("Localisation/" + culture.Name + "/" + culture.Name + ".Items.json"));
			LoadLanguageFromFileText(File.ReadAllText("Localisation/" + culture.Name + "/" + culture.Name + ".Legacy.json"));
			LoadLanguageFromFileText(File.ReadAllText("Localisation/" + culture.Name + "/" + culture.Name + ".NPCs.json"));
			LoadLanguageFromFileText(File.ReadAllText("Localisation/" + culture.Name + "/" + culture.Name + ".Projectiles.json"));
			LoadLanguageFromFileText(File.ReadAllText("Localisation/" + culture.Name + "/" + culture.Name + ".Town.json"));
			LoadLanguageFromFileText(File.ReadAllText("Localisation/" + culture.Name + "/" + culture.Name + ".Mobile.json"));
		}

		private static string ReadEmbeddedResource(string path)
		{
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
			{
				using (StreamReader streamReader = new StreamReader(stream))
				{
					return streamReader.ReadToEnd();
				}
			}
		}

		private void ProcessCopyCommandsInTexts()
		{
			Regex regex = new Regex("{\\$(\\w+\\.\\w+)}", RegexOptions.Compiled);
			foreach (KeyValuePair<string, LocalizedText> localizedText in _localizedTexts)
			{
				LocalizedText value = localizedText.Value;
				for (int i = 0; i < 100; i++)
				{
					string text = regex.Replace(value.Value, (Match match) => GetTextValue(match.Groups[1].ToString()));
					if (text == value.Value)
					{
						break;
					}
					value.SetValue(text);
				}
			}
		}

		public void LoadLanguageFromFileText(string fileText)
		{
			Dictionary<string, Dictionary<string, string>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(fileText);
			foreach (KeyValuePair<string, Dictionary<string, string>> item in dictionary)
			{
				string key2 = item.Key;
				foreach (KeyValuePair<string, string> item2 in item.Value)
				{
					string key = item.Key + "." + item2.Key;
					if (_localizedTexts.ContainsKey(key))
					{
						_localizedTexts[key].SetValue(item2.Value);
					}
					else
					{
						_localizedTexts.Add(key, new LocalizedText(key, item2.Value));
						if (!_categoryGroupedKeys.ContainsKey(item.Key))
						{
							_categoryGroupedKeys.Add(item.Key, new List<string>());
						}
						_categoryGroupedKeys[item.Key].Add(item2.Key);
					}
				}
			}
		}

		private static string WriteCategoryStart(string category)
		{
			return "\t\"" + category + "\": {\r\n";
		}

		private static string WriteCategoryEnd(bool moreValues)
		{
			return "\t}" + (moreValues ? "," : "") + "\r\n";
		}

		private static string WriteLocalisedString(string ident, string value, bool moreValues)
		{
			value = value.Replace("\n", "\\n");
			value = value.Replace("\"", "\\\"");
			return "\t\t\"" + ident + "\": \"" + value + "\"" + (moreValues ? "," : "") + "\r\n";
		}

		[Conditional("DEBUG")]
		private void ValidateAllCharactersContainedInFont(SpriteFont font)
		{
			if (font != null)
			{
				foreach (LocalizedText value2 in _localizedTexts.Values)
				{
					string value = value2.Value;
					foreach (char c in value)
					{
						char.IsWhiteSpace(c);
					}
				}
			}
		}

		public LocalizedText[] FindAll(Regex regex)
		{
			int num = 0;
			foreach (KeyValuePair<string, LocalizedText> localizedText in _localizedTexts)
			{
				if (regex.IsMatch(localizedText.Key))
				{
					num++;
				}
			}
			LocalizedText[] array = new LocalizedText[num];
			int num2 = 0;
			foreach (KeyValuePair<string, LocalizedText> localizedText2 in _localizedTexts)
			{
				if (regex.IsMatch(localizedText2.Key))
				{
					array[num2] = localizedText2.Value;
					num2++;
				}
			}
			return array;
		}

		public LocalizedText[] FindAll(LanguageSearchFilter filter)
		{
			LinkedList<LocalizedText> linkedList = new LinkedList<LocalizedText>();
			foreach (KeyValuePair<string, LocalizedText> localizedText in _localizedTexts)
			{
				if (filter(localizedText.Key, localizedText.Value))
				{
					linkedList.AddLast(localizedText.Value);
				}
			}
			return linkedList.ToArray();
		}

		public LocalizedText SelectRandom(LanguageSearchFilter filter, Random random = null)
		{
			int num = 0;
			foreach (KeyValuePair<string, LocalizedText> localizedText in _localizedTexts)
			{
				if (filter(localizedText.Key, localizedText.Value))
				{
					num++;
				}
			}
			int num2 = (random ?? Main.rand).Next(num);
			foreach (KeyValuePair<string, LocalizedText> localizedText2 in _localizedTexts)
			{
				if (filter(localizedText2.Key, localizedText2.Value) && --num == num2)
				{
					return localizedText2.Value;
				}
			}
			return LocalizedText.Empty;
		}

		public LocalizedText RandomFromCategory(string categoryName, Random random = null)
		{
			if (!_categoryGroupedKeys.ContainsKey(categoryName))
			{
				return new LocalizedText(categoryName + ".RANDOM", categoryName + ".RANDOM");
			}
			List<string> list = _categoryGroupedKeys[categoryName];
			return GetText(categoryName + "." + list[(random ?? Main.rand).Next(list.Count)]);
		}

		public bool Exists(string key)
		{
			return _localizedTexts.ContainsKey(key);
		}

		public LocalizedText GetText(string key)
		{
			if (!_localizedTexts.ContainsKey(key))
			{
				return new LocalizedText(key, key);
			}
			return _localizedTexts[key];
		}

		public string GetTextValue(string key)
		{
			if (_localizedTexts.ContainsKey(key))
			{
				return _localizedTexts[key].Value;
			}
			return key;
		}

		public string GetTextValue(string key, object arg0)
		{
			if (_localizedTexts.ContainsKey(key))
			{
				return _localizedTexts[key].Format(arg0);
			}
			return key;
		}

		public string GetTextValue(string key, object arg0, object arg1)
		{
			if (_localizedTexts.ContainsKey(key))
			{
				return _localizedTexts[key].Format(arg0, arg1);
			}
			return key;
		}

		public string GetTextValue(string key, object arg0, object arg1, object arg2)
		{
			if (_localizedTexts.ContainsKey(key))
			{
				return _localizedTexts[key].Format(arg0, arg1, arg2);
			}
			return key;
		}

		public string GetTextValue(string key, params object[] args)
		{
			if (_localizedTexts.ContainsKey(key))
			{
				return _localizedTexts[key].Format(args);
			}
			return key;
		}

		public void SetFallbackCulture(GameCulture culture)
		{
			_fallbackCulture = culture;
		}
	}
}
