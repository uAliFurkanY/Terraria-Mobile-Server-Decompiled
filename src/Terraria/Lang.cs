using System;
using System.Linq;
using System.Reflection;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace Terraria
{
	public class Lang
	{
		public static int lang = 0;

		public static LocalizedText[] misc = new LocalizedText[105];

		public static LocalizedText[] menu = new LocalizedText[253];

		public static LocalizedText[] gen = new LocalizedText[82];

		public static LocalizedText[] inter = new LocalizedText[118];

		public static LocalizedText[] tip = new LocalizedText[60];

		public static LocalizedText[] mp = new LocalizedText[23];

		public static LocalizedText[] dt = new LocalizedText[4];

		public static LocalizedText[] chestType = new LocalizedText[52];

		public static LocalizedText[] dresserType = new LocalizedText[28];

		public static LocalizedText[] mobile = new LocalizedText[120];

		public static string the;

		private static LocalizedText[] _itemNameCache = new LocalizedText[3602];

		private static LocalizedText[] _negativeItemNameCache = new LocalizedText[24];

		private static LocalizedText[] _projectileNameCache = new LocalizedText[651];

		private static LocalizedText[] _npcNameCache = new LocalizedText[540];

		private static LocalizedText[] _negativeNpcNameCache = new LocalizedText[65];

		private static LocalizedText[] _buffNameCache = new LocalizedText[191];

		private static LocalizedText[] _buffDescriptionCache = new LocalizedText[191];

		public static LocalizedText[] prefix = new LocalizedText[84];

		public static LocalizedText[] _mapLegendCache;

		private static ItemTooltip[] _itemTooltipCache = new ItemTooltip[3602];

		private static ItemTooltip[] _negativeItemTooltipCache = new ItemTooltip[24];

		public static object CreateDialogSubstitutionObject(NPC npc = null)
		{
			return new
			{
				Nurse = NPC.GetFirstNPCNameOrNull(18),
				Merchant = NPC.GetFirstNPCNameOrNull(17),
				ArmsDealer = NPC.GetFirstNPCNameOrNull(19),
				Dryad = NPC.GetFirstNPCNameOrNull(20),
				Demolitionist = NPC.GetFirstNPCNameOrNull(38),
				Clothier = NPC.GetFirstNPCNameOrNull(54),
				Guide = NPC.GetFirstNPCNameOrNull(22),
				Wizard = NPC.GetFirstNPCNameOrNull(108),
				GoblinTinkerer = NPC.GetFirstNPCNameOrNull(107),
				Mechanic = NPC.GetFirstNPCNameOrNull(124),
				Truffle = NPC.GetFirstNPCNameOrNull(160),
				Steampunker = NPC.GetFirstNPCNameOrNull(178),
				DyeTrader = NPC.GetFirstNPCNameOrNull(207),
				PartyGirl = NPC.GetFirstNPCNameOrNull(208),
				Cyborg = NPC.GetFirstNPCNameOrNull(209),
				Painter = NPC.GetFirstNPCNameOrNull(227),
				WitchDoctor = NPC.GetFirstNPCNameOrNull(228),
				Pirate = NPC.GetFirstNPCNameOrNull(229),
				Stylist = NPC.GetFirstNPCNameOrNull(353),
				TravelingMerchant = NPC.GetFirstNPCNameOrNull(368),
				Angler = NPC.GetFirstNPCNameOrNull(369),
				WorldName = Main.worldName,
				Day = Main.dayTime,
				BloodMoon = Main.bloodMoon,
				MoonLordDefeated = NPC.downedMoonlord,
				HardMode = Main.hardMode,
				Homeless = (npc?.homeless ?? false),
				InventoryKey = Main.cInv,
				PlayerName = Main.player[Main.myPlayer].name,
				AnglerCompletedQuestsCount = Main.player[Main.myPlayer].anglerQuestsFinished
			};
		}

		public static string dialog(int l, bool english = false)
		{
			return Language.GetTextValueWith("LegacyDialog." + l, CreateDialogSubstitutionObject());
		}

		public static string evilGood()
		{
			if (lang <= 1)
			{
				string text = "";
				int tGood = WorldGen.tGood;
				int tEvil = WorldGen.tEvil;
				int tBlood = WorldGen.tBlood;
				if (tGood > 0 && tEvil > 0 && tBlood > 0)
				{
					text = Main.worldName + " is " + tGood + "% hallow, " + tEvil + "% corrupt, and " + tBlood + "% crimson.";
				}
				else if (tGood > 0 && tEvil > 0)
				{
					text = Main.worldName + " is " + tGood + "% hallow and " + tEvil + "% corrupt.";
				}
				else if (tGood > 0 && tBlood > 0)
				{
					text = Main.worldName + " is " + tGood + "% hallow and " + tBlood + "% crimson.";
				}
				else if (tEvil > 0 && tBlood > 0)
				{
					text = Main.worldName + " is " + tEvil + "% corrupt and " + tBlood + "% crimson.";
				}
				else if (tEvil > 0)
				{
					text = Main.worldName + " is " + tEvil + "% corrupt.";
				}
				else if (tBlood > 0)
				{
					text = Main.worldName + " is " + tBlood + "% crimson.";
				}
				else
				{
					if (tGood <= 0)
					{
						return Main.worldName + " is completely pure. You have done an amazing job!";
					}
					text = Main.worldName + " is " + tGood + "% hallow.";
				}
				if ((double)tGood * 1.2 >= (double)(tEvil + tBlood) && (double)tGood * 0.8 <= (double)(tEvil + tBlood))
				{
					return text + " The world is in balance.";
				}
				if (tGood >= tEvil + tBlood)
				{
					return text + " We are living in a fairy tale.";
				}
				if (tEvil + tBlood > tGood + 20)
				{
					return text + " Things are grim indeed...";
				}
				if (tEvil + tBlood > 10)
				{
					return text + " You have a lot of work to do.";
				}
				return text + " You are so close!";
			}
			if (lang == 2)
			{
				string text2 = "";
				text2 = ((WorldGen.tGood == 0) ? (Main.worldName + " ist zu " + WorldGen.tEvil + "% verderbt.") : ((WorldGen.tEvil != 0) ? (Main.worldName + " ist zu " + WorldGen.tGood + "% gesegnet und zu " + WorldGen.tEvil + "% verderbt.") : (Main.worldName + " ist zu " + WorldGen.tGood + "% gesegnet.")));
				if (WorldGen.tGood > WorldGen.tEvil)
				{
					return text2 + " Gute Arbeit, weiter so!";
				}
				if (WorldGen.tEvil > WorldGen.tGood && WorldGen.tEvil > 20)
				{
					return text2 + " Es steht nicht gut.";
				}
				return text2 + " Streng dich mehr an!";
			}
			if (lang == 3)
			{
				string text3 = "";
				text3 = ((WorldGen.tGood == 0) ? (Main.worldName + " è distrutto al " + WorldGen.tEvil + "%.") : ((WorldGen.tEvil != 0) ? (Main.worldName + " è santo al " + WorldGen.tGood + "% e distrutto al " + WorldGen.tEvil + "%.") : (Main.worldName + " è santo al " + WorldGen.tGood + "%.")));
				if (WorldGen.tGood > WorldGen.tEvil)
				{
					return text3 + " Continua così!";
				}
				if (WorldGen.tEvil > WorldGen.tGood && WorldGen.tEvil > 20)
				{
					return text3 + " Le cose vanno male.";
				}
				return text3 + " Dovresti impegnarti di più.";
			}
			if (lang == 4)
			{
				string text4 = "";
				text4 = ((WorldGen.tGood == 0) ? (Main.worldName + " est corrompu à " + WorldGen.tEvil + "%.") : ((WorldGen.tEvil != 0) ? (Main.worldName + " est purifié à " + WorldGen.tGood + "% et corrompu à " + WorldGen.tEvil + "%.") : (Main.worldName + " est purifié à " + WorldGen.tGood + "%.")));
				if (WorldGen.tGood > WorldGen.tEvil)
				{
					return text4 + " Continuez comme ça.";
				}
				if (WorldGen.tEvil > WorldGen.tGood && WorldGen.tEvil > 20)
				{
					return text4 + " En effet, c'est pas la joie.";
				}
				return text4 + " Essayez encore.";
			}
			if (lang == 5)
			{
				string text5 = "";
				text5 = ((WorldGen.tGood == 0) ? (Main.worldName + " ha sido corrompido por " + WorldGen.tEvil + "%.") : ((WorldGen.tEvil != 0) ? (Main.worldName + " ha sido bendecido por " + WorldGen.tGood + "% y corrompido por " + WorldGen.tEvil + "%.") : (Main.worldName + " ha sido bendecido por " + WorldGen.tGood + "%.")));
				if (WorldGen.tGood > WorldGen.tEvil)
				{
					return text5 + " ¡Sigue haciéndolo bien!";
				}
				if (WorldGen.tEvil > WorldGen.tGood && WorldGen.tEvil > 20)
				{
					return text5 + " Es bastante desalentador.";
				}
				return text5 + " Deberías esforzarte más.";
			}
			return "";
		}

		public static string title()
		{
			int num = Main.rand.Next(16);
			if (lang <= 1)
			{
				switch (Main.rand.Next(53))
				{
				case 0:
					return "Terraria: Dig Peon, Dig!";
				case 1:
					return "Terraria: Epic Dirt";
				case 2:
					return "Terraria: Adaman-TIGHT!";
				case 3:
					return "Terraria: Sand is Overpowered";
				case 4:
					return "Terraria Part 3: The Return of the Guide";
				case 5:
					return "Terraria: A Bunnies Tale";
				case 6:
					return "Terraria: Dr. Bones and The Temple of Blood Moon";
				case 7:
					return "Terraria: Slimeassic Park";
				case 8:
					return "Terraria: The Grass is Greener on This Side";
				case 9:
					return "Terraria: Small Blocks, Not for Children Under the Age of 5";
				case 10:
					return "Terraria: Digger T' Blocks";
				case 11:
					return "Terraria: There is No Cow Layer";
				case 12:
					return "Terraria: Suspicous Looking Eyeballs";
				case 13:
					return "Terraria: Purple Grass!";
				case 14:
					return "Terraria: No one Dug Behind!";
				case 15:
					return "Terraria: The Water Fall Of Content!";
				case 16:
					return "Terraria: Earthbound";
				case 17:
					return "Terraria: Dig Dug Ain't Got Nuthin on Me";
				case 18:
					return "Terraria: Ore's Well That Ends Well";
				case 19:
					return "Terraria: Judgement Clay";
				case 20:
					return "Terraria: Terrestrial Trouble";
				case 21:
					return "Terraria: Obsessive-Compulsive Discovery Simulator";
				case 22:
					return "Terraria: Red Dev Redemption";
				case 23:
					return "Terraria: Rise of the Slimes";
				case 24:
					return "Terraria: Now with more things to kill you!";
				case 25:
					return "Terraria: Rumors of the Guides' death were greatly exaggerated";
				case 26:
					return "Terraria: I Pity the Tools...";
				case 27:
					return "Terraria: A spelunker says 'What'?";
				case 28:
					return "Terraria: So then I said 'Something about a PC update....'";
				case 29:
					return "Terraria: May the blocks be with you";
				case 30:
					return "Terraria: Better than life";
				case 31:
					return "Terraria: Terraria: Terraria:";
				case 32:
					return "Terraria: Now in 1D";
				case 33:
					return "Terraria: Coming soon to a computer near you";
				case 34:
					return "Terraria: Dividing by zero";
				case 35:
					return "Terraria: Now with SOUND";
				case 36:
					return "Terraria: Press alt-f4";
				case 37:
					return "Terraria: I Pity the Tools";
				case 38:
					return "Terraria: You sand bro?";
				case 39:
					return "Terraria: A good day to dig hard";
				case 40:
					return "Terraria: Can You Re-Dig-It?";
				case 41:
					return "Terraria: I don't know that-- aaaaa!";
				case 42:
					return "Terraria: What's that purple spiked thing?";
				case 43:
					return "Terraria: I wanna be the guide";
				case 44:
					return "Terraria: Cthulhu is mad... and is missing an eye!";
				case 45:
					return "Terraria: NOT THE BEES!!!";
				case 46:
					return "Terraria: Legend of Maxx";
				case 47:
					return "Terraria: Cult of Cenx";
				case 48:
					return "Terraria 2: Electric Boogaloo";
				case 49:
					return "Terraria: Also try Minecraft!";
				case 50:
					return "Terraria: Also try Edge of Space!";
				case 51:
					return "Terraria: I just wanna know where the gold at?";
				default:
					return "Terraria: Shut Up and Dig Gaiden!";
				}
			}
			if (lang == 2)
			{
				switch (num)
				{
				case 0:
					return "Terraria: Nun grab schon, Bauer, grab!";
				case 1:
					return "Terraria: Epischer Dreck";
				case 2:
					return "Terraria: Huhu, Leute!";
				case 3:
					return "Terraria: Sand is overpowered!";
				case 4:
					return "Terraria Teil 3: Die Rueckkehr des Fremdenfuehrers";
				case 5:
					return "Terraria: Geschichte eines verderbten Haeschens";
				case 6:
					return "Terraria: Dr. Bones und der Tempel des Blutmondes";
				case 7:
					return "Terraria: Schleimassic Park";
				case 8:
					return "Terraria: Das Gras ist auf dieser Seite gruener";
				case 9:
					return "Terraria: Kleine Bloecke, nicht fuer Kinder unter 5";
				case 10:
					return "Terraria: Der Block des Ausgraebers";
				case 11:
					return "Terraria: Hier gibt's auch kein Kuh-Level!";
				case 12:
					return "Terraria: Verdaechtig ausschauende Augaepfel";
				case 13:
					return "Terraria: Violettes Gras!";
				case 14:
					return "Terraria: Houston, wir haben ein Problem gehabt!";
				default:
					return "Terraria: Grab mit deiner Hand, nicht mit dem Mund!";
				}
			}
			if (lang == 3)
			{
				switch (num)
				{
				case 0:
					return "Terraria: Scava contadino, scava!";
				case 1:
					return "Terraria: Terra epica";
				case 2:
					return "Terraria: Ehi, ragazzi!";
				case 3:
					return "Terraria: La sabbia è strapotente";
				case 4:
					return "Terraria: Il ritorno della guida";
				case 5:
					return "Terraria: Coda di coniglio";
				case 6:
					return "Terraria: Dottor Ossa e il tempio della luna di sangue";
				case 7:
					return "Terraria: Slimeassic Park";
				case 8:
					return "Terraria: L'erba è più verde da questo lato";
				case 9:
					return "Terraria: Piccoli blocchi, non per bambini al di sotto di 5 anni";
				case 10:
					return "Terraria:  Il blocco dello scavatore";
				case 11:
					return "Terraria: No mucche, no party";
				case 12:
					return "Terraria: Bulbi oculari diffidenti";
				case 13:
					return "Terraria: Erba viola!";
				case 14:
					return "Terraria: Houston, abbiamo un problema!";
				default:
					return "Terraria: Zitto e scava, 'azzo!";
				}
			}
			if (lang == 4)
			{
				switch (num)
				{
				case 0:
					return "Terraria : Creuse et fais pas cette mine !";
				case 1:
					return "Terraria : Bain de boue";
				case 2:
					return "Terraria : Salut la compagnie !";
				case 3:
					return "Terraria : Le canon à sable, c'est vraiment grosbill";
				case 4:
					return "Terraria, 3e partie : Le retour du guide";
				case 5:
					return "Terraria : Des lapins pas si crétins";
				case 6:
					return "Terraria : Dr Bones et le temple de la lune de sang";
				case 7:
					return "Terraria: Slimeassic Park";
				case 8:
					return "Terraria : L'herbe est plus verte dans le pré du voisin";
				case 9:
					return "Terraria : Petits blocs interdits aux enfants de moins de 5 ans";
				case 10:
					return "Terraria : Des mineurs gonflés à bloc ! ";
				case 11:
					return "Terraria : Strates aux sphères";
				case 12:
					return "Terraria : L'œil observateur suspicieux";
				case 13:
					return "Terraria  : Silence, ça pousse !";
				case 14:
					return "Terraria : Houston, nous avons un problème !";
				default:
					return "Terraria : J'fais des trous, des p'tis trous...";
				}
			}
			if (lang == 5)
			{
				switch (num)
				{
				case 0:
					return "Terraria: ¡Cava, peón, cava!";
				case 1:
					return "Terraria: Terreno épico";
				case 2:
					return "Terraria: ¡Hola, tíos!";
				case 3:
					return "Terraria: El poder de la arena";
				case 4:
					return "Terraria parte 3: El regreso del Guía";
				case 5:
					return "Terraria: Un cuento de conejitos";
				case 6:
					return "Terraria: El Dr. Látigo y el Templo de la Luna Sangrienta";
				case 7:
					return "Terraria: Babosic Park";
				case 8:
					return "Terraria: Mi césped es más verde que el del vecino";
				case 9:
					return "Terraria: Bloques de construcción (no apto para menores de 5 años)";
				case 10:
					return "Terraria: Buscador de bloques";
				case 11:
					return "Terraria: Nada de niveles ocultos";
				case 12:
					return "Terraria: Ojos de aspecto sospechoso";
				case 13:
					return "Terraria: ¡Césped morado!";
				case 14:
					return "Terraria: ¡No abandonamos ningún agujero!";
				default:
					return "Terraria: ¡Cállate y cava un universo paralelo!";
				}
			}
			return "";
		}

		public static string DyeTraderQuestChat(bool gotDye = false)
		{
			object obj = CreateDialogSubstitutionObject();
			LocalizedText[] array = Language.FindAll(CreateDialogFilter(gotDye ? "DyeTraderSpecialText.HasPlant" : "DyeTraderSpecialText.NoPlant", obj));
			return array[Main.rand.Next(array.Length)].FormatWith(obj);
		}

		public static LanguageSearchFilter CreateDialogFilter(string startsWith, object substitutions)
		{
			return (string key, LocalizedText text) => key.StartsWith(startsWith) && text.CanFormatWith(substitutions);
		}

		public static string AnglerQuestChat(bool gotFish = false)
		{
			object obj = CreateDialogSubstitutionObject();
			if (gotFish)
			{
				return Language.SelectRandom(CreateDialogFilter("AnglerQuestText.TurnIn_", obj)).FormatWith(obj);
			}
			if (Main.anglerQuestFinished)
			{
				return Language.SelectRandom(CreateDialogFilter("AnglerQuestText.NoQuest_", obj)).FormatWith(obj);
			}
			int num = Main.npcChatCornerItem = Main.anglerQuestItemNetIDs[Main.anglerQuest];
			return Language.GetTextValueWith("AnglerQuestText.Quest_" + Main.itemName[num].Replace(" ", ""), obj);
		}

		private static void FillNameCacheArray<IdClass, IdType>(string category, string[] nameCache, bool leaveMissingEntriesBlank = false) where IdType : IConvertible
		{
			for (int i = 0; i < nameCache.Length; i++)
			{
				nameCache[i] = LocalizedText.Empty.Value;
			}
			(from f in typeof(IdClass).GetFields(BindingFlags.Static | BindingFlags.Public)
				where f.FieldType == typeof(IdType)
				select f).ToList().ForEach(delegate(FieldInfo field)
			{
				long num = Convert.ToInt64((IdType)field.GetValue(null));
				if (num >= 0 && num < nameCache.Length)
				{
					nameCache[num] = ((!leaveMissingEntriesBlank || Language.Exists(category + "." + field.Name)) ? Language.GetText(category + "." + field.Name).Value : LocalizedText.Empty.Value);
				}
				else if (field.Name == "None")
				{
					nameCache[num] = LocalizedText.Empty.Value;
				}
			});
		}

		private static void FillNameCacheArray<IdClass, IdType>(string category, LocalizedText[] nameCache, bool leaveMissingEntriesBlank = false) where IdType : IConvertible
		{
			for (int i = 0; i < nameCache.Length; i++)
			{
				nameCache[i] = LocalizedText.Empty;
			}
			(from f in typeof(IdClass).GetFields(BindingFlags.Static | BindingFlags.Public)
				where f.FieldType == typeof(IdType)
				select f).ToList().ForEach(delegate(FieldInfo field)
			{
				long num = Convert.ToInt64((IdType)field.GetValue(null));
				if (num >= 0 && num < nameCache.Length)
				{
					nameCache[num] = ((!leaveMissingEntriesBlank || Language.Exists(category + "." + field.Name)) ? Language.GetText(category + "." + field.Name) : LocalizedText.Empty);
				}
				else if (field.Name == "None")
				{
					nameCache[num] = LocalizedText.Empty;
				}
			});
		}

		public static string GetBuffName(int id)
		{
			return _buffNameCache[id].Value;
		}

		public static string GetBuffDescription(int id)
		{
			return _buffDescriptionCache[id].Value;
		}

		public static string GetItemNameValue(int id)
		{
			return GetItemName(id).Value;
		}

		public static string GetMapObjectName(int id)
		{
			if (_mapLegendCache != null)
			{
				return _mapLegendCache[id].Value;
			}
			return string.Empty;
		}

		public static ItemTooltip GetTooltip(int itemId)
		{
			if (itemId > 0 && itemId < 3602)
			{
				return _itemTooltipCache[itemId];
			}
			if (itemId < 0 && -itemId - 1 < _negativeItemTooltipCache.Length)
			{
				return _negativeItemTooltipCache[-itemId - 1];
			}
			return null;
		}

		public static LocalizedText GetProjectileName(int type)
		{
			if (type >= 0 && type < _projectileNameCache.Length && _projectileNameCache[type] != null)
			{
				return _projectileNameCache[type];
			}
			return LocalizedText.Empty;
		}

		public static LocalizedText GetItemName(int id)
		{
			if (id > 0 && id < 3602 && _itemNameCache[id] != null)
			{
				return _itemNameCache[id];
			}
			if (id < 0 && -id - 1 < _negativeItemNameCache.Length)
			{
				return _negativeItemNameCache[-id - 1];
			}
			return LocalizedText.Empty;
		}

		public static void InitializeLegacyLocalization()
		{
			FillNameCacheArray<PrefixID, int>("Prefix", prefix);
			for (int i = 0; i < gen.Length; i++)
			{
				gen[i] = Language.GetText("LegacyWorldGen." + i);
			}
			for (int j = 0; j < menu.Length; j++)
			{
				menu[j] = Language.GetText("LegacyMenu." + j);
			}
			for (int k = 0; k < inter.Length; k++)
			{
				inter[k] = Language.GetText("LegacyInterface." + k);
			}
			for (int l = 0; l < misc.Length; l++)
			{
				misc[l] = Language.GetText("LegacyMisc." + l);
			}
			for (int m = 0; m < mp.Length; m++)
			{
				mp[m] = Language.GetText("LegacyMultiplayer." + m);
			}
			for (int n = 0; n < tip.Length; n++)
			{
				tip[n] = Language.GetText("LegacyTooltip." + n);
			}
			for (int num = 0; num < chestType.Length; num++)
			{
				chestType[num] = Language.GetText("LegacyChestType." + num);
			}
			for (int num2 = 0; num2 < dresserType.Length; num2++)
			{
				dresserType[num2] = Language.GetText("LegacyDresserType." + num2);
			}
			FillNameCacheArray<ItemID, short>("ItemName", _itemNameCache);
			FillNameCacheArray<MobileLangID, short>("Mobile", mobile);
			for (int num3 = 0; num3 < _negativeItemNameCache.Length; num3++)
			{
				_negativeItemNameCache[num3] = LocalizedText.Empty;
			}
			_negativeItemNameCache[23] = Language.GetText("ItemName.YellowPhasesaber");
			_negativeItemNameCache[22] = Language.GetText("ItemName.WhitePhasesaber");
			_negativeItemNameCache[21] = Language.GetText("ItemName.PurplePhasesaber");
			_negativeItemNameCache[20] = Language.GetText("ItemName.GreenPhasesaber");
			_negativeItemNameCache[19] = Language.GetText("ItemName.RedPhasesaber");
			_negativeItemNameCache[18] = Language.GetText("ItemName.BluePhasesaber");
			FillNameCacheArray<ProjectileID, short>("ProjectileName", _projectileNameCache);
			FillNameCacheArray<NPCID, short>("NPCName", _npcNameCache);
			FillNameCacheArray<BuffID, int>("BuffName", _buffNameCache);
			FillNameCacheArray<BuffID, int>("BuffDescription", _buffDescriptionCache);
			for (int num4 = -65; num4 < 0; num4++)
			{
				_negativeNpcNameCache[-num4 - 1] = _npcNameCache[NPCID.FromNetId(num4)];
			}
			_negativeNpcNameCache[0] = Language.GetText("NPCName.Slimeling");
			_negativeNpcNameCache[1] = Language.GetText("NPCName.Slimer2");
			_negativeNpcNameCache[2] = Language.GetText("NPCName.GreenSlime");
			_negativeNpcNameCache[3] = Language.GetText("NPCName.Pinky");
			_negativeNpcNameCache[4] = Language.GetText("NPCName.BabySlime");
			_negativeNpcNameCache[5] = Language.GetText("NPCName.BlackSlime");
			_negativeNpcNameCache[6] = Language.GetText("NPCName.PurpleSlime");
			_negativeNpcNameCache[7] = Language.GetText("NPCName.RedSlime");
			_negativeNpcNameCache[8] = Language.GetText("NPCName.YellowSlime");
			_negativeNpcNameCache[9] = Language.GetText("NPCName.JungleSlime");
			_negativeNpcNameCache[53] = Language.GetText("NPCName.SmallRainZombie");
			_negativeNpcNameCache[54] = Language.GetText("NPCName.BigRainZombie");
			ItemTooltip.AddGlobalProcessor(delegate(string tooltip)
			{
				if (tooltip.Contains("<right>"))
				{
					tooltip = tooltip.Replace("<right>", Language.GetTextValue("Controls.RightClick"));
				}
				return tooltip;
			});
			for (int num5 = 0; num5 < _itemTooltipCache.Length; num5++)
			{
				_itemTooltipCache[num5] = ItemTooltip.None;
			}
			for (int num6 = 0; num6 < _negativeItemTooltipCache.Length; num6++)
			{
				_negativeItemTooltipCache[num6] = ItemTooltip.None;
			}
			(from f in typeof(ItemID).GetFields(BindingFlags.Static | BindingFlags.Public)
				where f.FieldType == typeof(short)
				select f).ToList().ForEach(delegate(FieldInfo field)
			{
				short num8 = (short)field.GetValue(null);
				if (num8 > 0 && num8 < _itemTooltipCache.Length)
				{
					_itemTooltipCache[num8] = ItemTooltip.FromLanguageKey("ItemTooltip." + field.Name);
				}
			});
			_negativeItemTooltipCache[23] = ItemTooltip.FromLanguageKey("ItemTooltip.YellowPhasesaber");
			_negativeItemTooltipCache[22] = ItemTooltip.FromLanguageKey("ItemTooltip.WhitePhasesaber");
			_negativeItemTooltipCache[21] = ItemTooltip.FromLanguageKey("ItemTooltip.PurplePhasesaber");
			_negativeItemTooltipCache[20] = ItemTooltip.FromLanguageKey("ItemTooltip.GreenPhasesaber");
			_negativeItemTooltipCache[19] = ItemTooltip.FromLanguageKey("ItemTooltip.RedPhasesaber");
			_negativeItemTooltipCache[18] = ItemTooltip.FromLanguageKey("ItemTooltip.BluePhasesaber");
			for (int num7 = 0; num7 < Recipe.numRecipes; num7++)
			{
				Main.recipe[num7].createItem.RebuildTooltip();
			}
		}

		public static void BuildMapAtlas()
		{
		}

		public static void setLang(bool english = false)
		{
			int language = lang;
			if (lang <= 1 || english)
			{
				language = 1;
			}
			LanguageManager.Instance.SetLanguage(language);
			InitializeLegacyLocalization();
			BuildMapAtlas();
			Main.chTitle = true;
		}

		public static string GetNPCNameValue(int netID)
		{
			return GetNPCName(netID).Value;
		}

		public static string GetInvasionWaveText(int wave, params short[] npcIds)
		{
			string[] array = new string[npcIds.Length + 1];
			for (int i = 0; i < npcIds.Length; i++)
			{
				array[i + 1] = GetNPCNameValue(npcIds[i]);
			}
			switch (wave)
			{
			case -1:
				array[0] = Language.GetTextValue("Game.FinalWave");
				break;
			case 1:
				array[0] = Language.GetTextValue("Game.FirstWave");
				break;
			default:
				array[0] = Language.GetTextValue("Game.Wave", wave);
				break;
			}
			return Language.GetTextValue("Game.InvasionWave_Type" + npcIds.Length, array);
		}

		public static LocalizedText GetNPCName(int netID)
		{
			if (netID > 0 && netID < 540)
			{
				return _npcNameCache[netID];
			}
			if (netID < 0 && -netID - 1 < _negativeNpcNameCache.Length)
			{
				return _negativeNpcNameCache[-netID - 1];
			}
			return LocalizedText.Empty;
		}

		public static string deathMsg(string deadPlayerName, int plr = -1, int npc = -1, int proj = -1, int other = -1)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string arg = string.Empty;
			string text3 = string.Empty;
			if (proj >= 0)
			{
				text = Main.projectile[proj].Name;
			}
			if (npc >= 0)
			{
				text2 = Main.npc[npc].GivenOrTypeName;
			}
			if (plr >= 0 && plr < 16)
			{
				arg = Main.player[plr].name;
			}
			if (plr >= 0)
			{
				text3 = Main.player[plr].inventory[Main.player[plr].selectedItem].Name;
			}
			bool flag = text != string.Empty;
			bool flag2 = plr >= 0 && plr < 16;
			bool flag3 = text2 != string.Empty;
			string result = string.Empty;
			string textValue = Language.GetTextValue(Language.RandomFromCategory("DeathTextGeneric").Key, deadPlayerName, Main.worldName);
			if (flag2)
			{
				result = Language.GetTextValue("DeathSource.Player", textValue, arg, flag ? text : text3);
			}
			else if (flag3)
			{
				result = Language.GetTextValue("DeathSource.NPC", textValue, text2);
			}
			else if (flag)
			{
				result = Language.GetTextValue("DeathSource.Projectile", textValue, text);
			}
			else
			{
				switch (other)
				{
				case 0:
					result = Language.GetTextValue("DeathText.Fell_" + (Main.rand.Next(2) + 1), deadPlayerName);
					break;
				case 1:
					result = Language.GetTextValue("DeathText.Drowned_" + (Main.rand.Next(4) + 1), deadPlayerName);
					break;
				case 2:
					result = Language.GetTextValue("DeathText.Lava_" + (Main.rand.Next(4) + 1), deadPlayerName);
					break;
				case 3:
					result = Language.GetTextValue("DeathText.Default", textValue);
					break;
				case 4:
					result = Language.GetTextValue("DeathText.Slain", deadPlayerName);
					break;
				case 5:
					result = Language.GetTextValue("DeathText.Petrified_" + (Main.rand.Next(4) + 1), deadPlayerName);
					break;
				case 6:
					result = Language.GetTextValue("DeathText.Stabbed", deadPlayerName);
					break;
				case 7:
					result = Language.GetTextValue("DeathText.Suffocated", deadPlayerName);
					break;
				case 8:
					result = Language.GetTextValue("DeathText.Burned", deadPlayerName);
					break;
				case 9:
					result = Language.GetTextValue("DeathText.Poisoned", deadPlayerName);
					break;
				case 10:
					result = Language.GetTextValue("DeathText.Electrocuted", deadPlayerName);
					break;
				case 11:
					result = Language.GetTextValue("DeathText.TriedToEscape", deadPlayerName);
					break;
				case 12:
					result = Language.GetTextValue("DeathText.WasLicked", deadPlayerName);
					break;
				case 13:
					result = Language.GetTextValue("DeathText.Teleport_1", deadPlayerName);
					break;
				case 14:
					result = Language.GetTextValue("DeathText.Teleport_2_Male", deadPlayerName);
					break;
				case 15:
					result = Language.GetTextValue("DeathText.Teleport_2_Female", deadPlayerName);
					break;
				case 16:
					result = Language.GetTextValue("DeathText.Inferno", deadPlayerName);
					break;
				case 254:
					result = string.Empty;
					break;
				case 255:
					result = Language.GetTextValue("DeathText.Slain", deadPlayerName);
					break;
				}
			}
			return result;
		}
	}
}
