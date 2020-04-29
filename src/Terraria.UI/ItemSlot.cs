using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.UI.Chat;

namespace Terraria.UI
{
	public class ItemSlot
	{
		public class Options
		{
			public static bool DisableLeftShiftTrashCan = false;

			public static bool HighlightNewItems = true;
		}

		public class Context
		{
			public const int InventoryItem = 0;

			public const int InventoryCoin = 1;

			public const int InventoryAmmo = 2;

			public const int ChestItem = 3;

			public const int BankItem = 4;

			public const int PrefixItem = 5;

			public const int TrashItem = 6;

			public const int GuideItem = 7;

			public const int EquipArmor = 8;

			public const int EquipArmorVanity = 9;

			public const int EquipAccessory = 10;

			public const int EquipAccessoryVanity = 11;

			public const int EquipDye = 12;

			public const int HotbarItem = 13;

			public const int ChatItem = 14;

			public const int ShopItem = 15;

			public const int EquipGrapple = 16;

			public const int EquipMount = 17;

			public const int EquipMinecart = 18;

			public const int EquipPet = 19;

			public const int EquipLight = 20;

			public const int MouseItem = 21;

			public const int CraftingMaterial = 22;

			public const int Count = 23;
		}

		private static Item[] singleSlotArray;

		private static bool[] canFavoriteAt;

		private static int dyeSlotCount;

		private static int accSlotCount;

		static ItemSlot()
		{
			singleSlotArray = new Item[1];
			canFavoriteAt = new bool[23];
			dyeSlotCount = 0;
			accSlotCount = 0;
			canFavoriteAt[0] = true;
			canFavoriteAt[1] = true;
			canFavoriteAt[2] = true;
		}

		public static void Handle(ref Item inv, int context = 0)
		{
			singleSlotArray[0] = inv;
			Handle(singleSlotArray, context);
			inv = singleSlotArray[0];
			Recipe.FindRecipes();
		}

		public static void Handle(Item[] inv, int context = 0, int slot = 0)
		{
			OverrideHover(inv, context, slot);
			if (Main.mouseLeftRelease && Main.mouseLeft)
			{
				LeftClick(inv, context, slot);
				Recipe.FindRecipes();
			}
			else
			{
				RightClick(inv, context, slot);
			}
			MouseHover(inv, context, slot);
		}

		public static void OverrideHover(Item[] inv, int context = 0, int slot = 0)
		{
			Item ıtem = inv[slot];
			if (Main.keyState.IsKeyDown(Keys.LeftShift) && ıtem.type > 0 && ıtem.stack > 0 && !inv[slot].favorited)
			{
				switch (context)
				{
				case 0:
				case 1:
				case 2:
					if (Main.npcShop > 0 && !ıtem.favorited)
					{
						Main.cursorOverride = 10;
					}
					else if (Main.player[Main.myPlayer].chest != -1)
					{
						if (ChestUI.TryPlacingInChest(ıtem, true))
						{
							Main.cursorOverride = 9;
						}
					}
					else
					{
						Main.cursorOverride = 6;
					}
					break;
				case 3:
				case 4:
					if (Main.player[Main.myPlayer].ItemSpace(ıtem))
					{
						Main.cursorOverride = 8;
					}
					break;
				case 5:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
					if (Main.player[Main.myPlayer].ItemSpace(inv[slot]))
					{
						Main.cursorOverride = 7;
					}
					break;
				}
			}
			if (Main.keyState.IsKeyDown(Keys.LeftAlt) && canFavoriteAt[context])
			{
				if (ıtem.type > 0 && ıtem.stack > 0 && Main.chatMode)
				{
					Main.cursorOverride = 2;
				}
				else if (ıtem.type > 0 && ıtem.stack > 0)
				{
					Main.cursorOverride = 3;
				}
			}
		}

		private static bool OverrideLeftClick(Item[] inv, int context = 0, int slot = 0)
		{
			Item ıtem = inv[slot];
			if (Main.cursorOverride == 2)
			{
				if (ChatManager.AddChatText(Main.fontMouseText, ItemTagHandler.GenerateTag(ıtem), Vector2.One))
				{
					Main.PlaySound(12);
				}
				return true;
			}
			if (Main.cursorOverride == 3)
			{
				if (!canFavoriteAt[context])
				{
					return false;
				}
				ıtem.favorited = !ıtem.favorited;
				Main.PlaySound(12);
				return true;
			}
			if (Main.cursorOverride == 7)
			{
				inv[slot] = Main.player[Main.myPlayer].GetItem(Main.myPlayer, inv[slot], false, true);
				Main.PlaySound(12);
				return true;
			}
			if (Main.cursorOverride == 8)
			{
				inv[slot] = Main.player[Main.myPlayer].GetItem(Main.myPlayer, inv[slot], false, true);
				if (Main.player[Main.myPlayer].chest > -1)
				{
					NetMessage.SendData(32, -1, -1, "", Main.player[Main.myPlayer].chest, slot);
				}
				return true;
			}
			if (Main.cursorOverride == 9)
			{
				ChestUI.TryPlacingInChest(inv[slot], false);
				return true;
			}
			return false;
		}

		public static void LeftClick(ref Item inv, int context = 0)
		{
			singleSlotArray[0] = inv;
			LeftClick(singleSlotArray, context);
			inv = singleSlotArray[0];
		}

		public static void LeftClick(Item[] inv, int context = 0, int slot = 0)
		{
			if (OverrideLeftClick(inv, context, slot))
			{
				return;
			}
			inv[slot].newAndShiny = false;
			Player player = Main.player[Main.myPlayer];
			bool flag = false;
			switch (context)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
				flag = (player.chest == -1);
				break;
			}
			if (Main.keyState.IsKeyDown(Keys.LeftShift) && flag)
			{
				if (inv[slot].type <= 0)
				{
					return;
				}
				if (Main.npcShop > 0 && !inv[slot].favorited)
				{
					Chest chest = Main.instance.shop[Main.npcShop];
					if (inv[slot].type < 71 || inv[slot].type > 74)
					{
						if (player.SellItem(inv[slot].value, inv[slot].stack))
						{
							chest.AddShop(inv[slot]);
							inv[slot].SetDefaults();
							Main.PlaySound(18);
							Recipe.FindRecipes();
						}
						else if (inv[slot].value == 0)
						{
							chest.AddShop(inv[slot]);
							inv[slot].SetDefaults();
							Main.PlaySound(7);
							Recipe.FindRecipes();
						}
					}
				}
				else if (!inv[slot].favorited && !Options.DisableLeftShiftTrashCan)
				{
					Main.PlaySound(7);
					player.trashItem = inv[slot].Clone();
					inv[slot].SetDefaults();
					if (context == 3 && Main.netMode == 1)
					{
						NetMessage.SendData(32, -1, -1, "", player.chest, slot);
					}
					Recipe.FindRecipes();
				}
			}
			else
			{
				if ((player.selectedItem == slot && player.itemAnimation > 0) || player.itemTime != 0)
				{
					return;
				}
				switch (PickItemMovementAction(inv, context, slot, Main.mouseItem))
				{
				case 0:
					if (context == 6 && Main.mouseItem.type != 0)
					{
						inv[slot].SetDefaults();
					}
					Utils.Swap(ref inv[slot], ref Main.mouseItem);
					if (inv[slot].stack > 0)
					{
						switch (context)
						{
						case 0:
							AchievementsHelper.NotifyItemPickup(player, inv[slot]);
							break;
						case 8:
						case 9:
						case 10:
						case 11:
						case 12:
						case 16:
						case 17:
							AchievementsHelper.HandleOnEquip(player, inv[slot], context);
							break;
						}
					}
					if (inv[slot].type == 0 || inv[slot].stack < 1)
					{
						inv[slot] = new Item();
					}
					if (Main.mouseItem.IsTheSameAs(inv[slot]))
					{
						Utils.Swap(ref inv[slot].favorited, ref Main.mouseItem.favorited);
						if (inv[slot].stack != inv[slot].maxStack && Main.mouseItem.stack != Main.mouseItem.maxStack)
						{
							if (Main.mouseItem.stack + inv[slot].stack <= Main.mouseItem.maxStack)
							{
								inv[slot].stack += Main.mouseItem.stack;
								Main.mouseItem.stack = 0;
							}
							else
							{
								int num = Main.mouseItem.maxStack - inv[slot].stack;
								inv[slot].stack += num;
								Main.mouseItem.stack -= num;
							}
						}
					}
					if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
					{
						Main.mouseItem = new Item();
					}
					if (Main.mouseItem.type > 0 || inv[slot].type > 0)
					{
						Recipe.FindRecipes();
						Main.PlaySound(7);
					}
					if (context == 3 && Main.netMode == 1)
					{
						NetMessage.SendData(32, -1, -1, "", player.chest, slot);
					}
					break;
				case 1:
					if (Main.mouseItem.stack == 1 && Main.mouseItem.type > 0 && inv[slot].type > 0 && inv[slot].IsNotTheSameAs(Main.mouseItem))
					{
						Utils.Swap(ref inv[slot], ref Main.mouseItem);
						Main.PlaySound(7);
						if (inv[slot].stack > 0)
						{
							switch (context)
							{
							case 0:
								AchievementsHelper.NotifyItemPickup(player, inv[slot]);
								break;
							case 8:
							case 9:
							case 10:
							case 11:
							case 12:
							case 16:
							case 17:
								AchievementsHelper.HandleOnEquip(player, inv[slot], context);
								break;
							}
						}
					}
					else if (Main.mouseItem.type == 0 && inv[slot].type > 0)
					{
						Utils.Swap(ref inv[slot], ref Main.mouseItem);
						if (inv[slot].type == 0 || inv[slot].stack < 1)
						{
							inv[slot] = new Item();
						}
						if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
						{
							Main.mouseItem = new Item();
						}
						if (Main.mouseItem.type > 0 || inv[slot].type > 0)
						{
							Recipe.FindRecipes();
							Main.PlaySound(7);
						}
					}
					else
					{
						if (Main.mouseItem.type <= 0 || inv[slot].type != 0)
						{
							break;
						}
						if (Main.mouseItem.stack == 1)
						{
							Utils.Swap(ref inv[slot], ref Main.mouseItem);
							if (inv[slot].type == 0 || inv[slot].stack < 1)
							{
								inv[slot] = new Item();
							}
							if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
							{
								Main.mouseItem = new Item();
							}
							if (Main.mouseItem.type > 0 || inv[slot].type > 0)
							{
								Recipe.FindRecipes();
								Main.PlaySound(7);
							}
						}
						else
						{
							Main.mouseItem.stack--;
							inv[slot].SetDefaults(Main.mouseItem.type);
							Recipe.FindRecipes();
							Main.PlaySound(7);
						}
						if (inv[slot].stack > 0)
						{
							switch (context)
							{
							case 0:
								AchievementsHelper.NotifyItemPickup(player, inv[slot]);
								break;
							case 8:
							case 9:
							case 10:
							case 11:
							case 12:
							case 16:
							case 17:
								AchievementsHelper.HandleOnEquip(player, inv[slot], context);
								break;
							}
						}
					}
					break;
				case 2:
					if (Main.mouseItem.stack == 1 && Main.mouseItem.dye > 0 && inv[slot].type > 0 && inv[slot].type != Main.mouseItem.type)
					{
						Utils.Swap(ref inv[slot], ref Main.mouseItem);
						Main.PlaySound(7);
						if (inv[slot].stack > 0)
						{
							switch (context)
							{
							case 0:
								AchievementsHelper.NotifyItemPickup(player, inv[slot]);
								break;
							case 8:
							case 9:
							case 10:
							case 11:
							case 12:
							case 16:
							case 17:
								AchievementsHelper.HandleOnEquip(player, inv[slot], context);
								break;
							}
						}
					}
					else if (Main.mouseItem.type == 0 && inv[slot].type > 0)
					{
						Utils.Swap(ref inv[slot], ref Main.mouseItem);
						if (inv[slot].type == 0 || inv[slot].stack < 1)
						{
							inv[slot] = new Item();
						}
						if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
						{
							Main.mouseItem = new Item();
						}
						if (Main.mouseItem.type > 0 || inv[slot].type > 0)
						{
							Recipe.FindRecipes();
							Main.PlaySound(7);
						}
					}
					else
					{
						if (Main.mouseItem.dye <= 0 || inv[slot].type != 0)
						{
							break;
						}
						if (Main.mouseItem.stack == 1)
						{
							Utils.Swap(ref inv[slot], ref Main.mouseItem);
							if (inv[slot].type == 0 || inv[slot].stack < 1)
							{
								inv[slot] = new Item();
							}
							if (Main.mouseItem.type == 0 || Main.mouseItem.stack < 1)
							{
								Main.mouseItem = new Item();
							}
							if (Main.mouseItem.type > 0 || inv[slot].type > 0)
							{
								Recipe.FindRecipes();
								Main.PlaySound(7);
							}
						}
						else
						{
							Main.mouseItem.stack--;
							inv[slot].SetDefaults(Main.mouseItem.type);
							Recipe.FindRecipes();
							Main.PlaySound(7);
						}
						if (inv[slot].stack > 0)
						{
							switch (context)
							{
							case 0:
								AchievementsHelper.NotifyItemPickup(player, inv[slot]);
								break;
							case 8:
							case 9:
							case 10:
							case 11:
							case 12:
							case 16:
							case 17:
								AchievementsHelper.HandleOnEquip(player, inv[slot], context);
								break;
							}
						}
					}
					break;
				case 3:
					Main.mouseItem.netDefaults(inv[slot].netID);
					if (inv[slot].buyOnce)
					{
						Main.mouseItem.Prefix(inv[slot].prefix);
					}
					else
					{
						Main.mouseItem.Prefix(-1);
					}
					Main.mouseItem.position = player.Center - new Vector2(Main.mouseItem.width, Main.mouseItem.headSlot) / 2f;
					ItemText.NewText(Main.mouseItem, Main.mouseItem.stack);
					if (inv[slot].buyOnce && --inv[slot].stack <= 0)
					{
						inv[slot].SetDefaults();
					}
					if (inv[slot].value > 0)
					{
						Main.PlaySound(18);
					}
					else
					{
						Main.PlaySound(7);
					}
					break;
				case 4:
				{
					Chest chest2 = Main.instance.shop[Main.npcShop];
					if (player.SellItem(Main.mouseItem.value, Main.mouseItem.stack))
					{
						chest2.AddShop(Main.mouseItem);
						Main.mouseItem.SetDefaults();
						Main.PlaySound(18);
					}
					else if (Main.mouseItem.value == 0)
					{
						chest2.AddShop(Main.mouseItem);
						Main.mouseItem.SetDefaults();
						Main.PlaySound(7);
					}
					Recipe.FindRecipes();
					break;
				}
				}
				switch (context)
				{
				case 0:
				case 1:
				case 2:
				case 5:
					return;
				}
				inv[slot].favorited = false;
			}
		}

		public static int PickItemMovementAction(Item[] inv, int context, int slot, Item checkItem)
		{
			Player player = Main.player[Main.myPlayer];
			int result = -1;
			switch (context)
			{
			case 0:
				result = 0;
				break;
			case 1:
				if (checkItem.type == 0 || checkItem.type == 71 || checkItem.type == 72 || checkItem.type == 73 || checkItem.type == 74)
				{
					result = 0;
				}
				break;
			case 2:
				if (((checkItem.type == 0 || checkItem.ammo > 0 || checkItem.bait > 0) && !checkItem.notAmmo) || checkItem.type == 530)
				{
					result = 0;
				}
				break;
			case 3:
				result = 0;
				break;
			case 4:
				result = 0;
				break;
			case 5:
				if (checkItem.Prefix(-3) || checkItem.type == 0)
				{
					result = 0;
				}
				break;
			case 6:
				result = 0;
				break;
			case 7:
				if (checkItem.material || checkItem.type == 0)
				{
					result = 0;
				}
				break;
			case 8:
				if (checkItem.type == 0 || (checkItem.headSlot > -1 && slot == 0) || (checkItem.bodySlot > -1 && slot == 1) || (checkItem.legSlot > -1 && slot == 2))
				{
					result = 1;
				}
				break;
			case 9:
				if (checkItem.type == 0 || (checkItem.headSlot > -1 && slot == 10) || (checkItem.bodySlot > -1 && slot == 11) || (checkItem.legSlot > -1 && slot == 12))
				{
					result = 1;
				}
				break;
			case 10:
				if (checkItem.type == 0 || (checkItem.accessory && !AccCheck(checkItem, slot)))
				{
					result = 1;
				}
				break;
			case 11:
				if (checkItem.type == 0 || (checkItem.accessory && !AccCheck(checkItem, slot)))
				{
					result = 1;
				}
				break;
			case 12:
				result = 2;
				break;
			case 15:
				if (checkItem.type == 0 && inv[slot].type > 0)
				{
					if (player.BuyItem(inv[slot].value))
					{
						result = 3;
					}
				}
				else if (inv[slot].type == 0 && checkItem.type > 0 && (checkItem.type < 71 || checkItem.type > 74))
				{
					result = 4;
				}
				break;
			case 16:
				if (checkItem.type == 0 || Main.projHook[checkItem.shoot])
				{
					result = 1;
				}
				break;
			case 17:
				if (checkItem.type == 0 || (checkItem.mountType != -1 && !MountID.Sets.Cart[checkItem.mountType]))
				{
					result = 1;
				}
				break;
			case 19:
				if (checkItem.type == 0 || (checkItem.buffType > 0 && Main.vanityPet[checkItem.buffType] && !Main.lightPet[checkItem.buffType]))
				{
					result = 1;
				}
				break;
			case 18:
				if (checkItem.type == 0 || (checkItem.mountType != -1 && MountID.Sets.Cart[checkItem.mountType]))
				{
					result = 1;
				}
				break;
			case 20:
				if (checkItem.type == 0 || (checkItem.buffType > 0 && Main.lightPet[checkItem.buffType]))
				{
					result = 1;
				}
				break;
			}
			return result;
		}

		public static void RightClick(ref Item inv, int context = 0)
		{
			singleSlotArray[0] = inv;
			RightClick(singleSlotArray, context);
			inv = singleSlotArray[0];
		}

		public static void RightClick(Item[] inv, int context = 0, int slot = 0)
		{
			Player player = Main.player[Main.myPlayer];
			inv[slot].newAndShiny = false;
			if (player.itemAnimation > 0)
			{
				return;
			}
			bool flag = false;
			switch (context)
			{
			case 0:
				flag = true;
				if (Main.mouseRight && inv[slot].type >= 3318 && inv[slot].type <= 3332)
				{
					if (Main.mouseRightRelease)
					{
						player.OpenBossBag(inv[slot].type);
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults();
						}
						Main.PlaySound(7);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && ((inv[slot].type >= 2334 && inv[slot].type <= 2336) || (inv[slot].type >= 3203 && inv[slot].type <= 3208)))
				{
					if (Main.mouseRightRelease)
					{
						player.openCrate(inv[slot].type);
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults();
						}
						Main.PlaySound(7);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && inv[slot].type == 3093)
				{
					if (Main.mouseRightRelease)
					{
						player.openHerbBag();
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults();
						}
						Main.PlaySound(7);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && inv[slot].type == 1774)
				{
					if (Main.mouseRightRelease)
					{
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults();
						}
						Main.PlaySound(7);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						player.openGoodieBag();
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && inv[slot].type == 3085)
				{
					if (Main.mouseRightRelease && player.consumeItem(327))
					{
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults();
						}
						Main.PlaySound(7);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						player.openLockBox();
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && inv[slot].type == 1869)
				{
					if (Main.mouseRightRelease)
					{
						inv[slot].stack--;
						if (inv[slot].stack == 0)
						{
							inv[slot].SetDefaults();
						}
						Main.PlaySound(7);
						Main.stackSplit = 30;
						Main.mouseRightRelease = false;
						player.openPresent();
						Recipe.FindRecipes();
					}
				}
				else if (Main.mouseRight && Main.mouseRightRelease && (inv[slot].type == 599 || inv[slot].type == 600 || inv[slot].type == 601))
				{
					Main.PlaySound(7);
					Main.stackSplit = 30;
					Main.mouseRightRelease = false;
					int num2 = Main.rand.Next(14);
					if (num2 == 0 && Main.hardMode)
					{
						inv[slot].SetDefaults(602);
					}
					else if (num2 <= 7)
					{
						inv[slot].SetDefaults(586);
						inv[slot].stack = Main.rand.Next(20, 50);
					}
					else
					{
						inv[slot].SetDefaults(591);
						inv[slot].stack = Main.rand.Next(20, 50);
					}
					Recipe.FindRecipes();
				}
				else
				{
					flag = false;
				}
				break;
			case 9:
			case 11:
			{
				flag = true;
				if (!Main.mouseRight || !Main.mouseRightRelease || ((inv[slot].type <= 0 || inv[slot].stack <= 0) && (inv[slot - 10].type <= 0 || inv[slot - 10].stack <= 0)))
				{
					break;
				}
				bool flag2 = true;
				if (flag2 && context == 11 && inv[slot].wingSlot > 0)
				{
					for (int j = 3; j < 10; j++)
					{
						if (inv[j].wingSlot > 0 && j != slot - 10)
						{
							flag2 = false;
						}
					}
				}
				if (!flag2)
				{
					break;
				}
				Utils.Swap(ref inv[slot], ref inv[slot - 10]);
				Main.PlaySound(7);
				Recipe.FindRecipes();
				if (inv[slot].stack > 0)
				{
					switch (context)
					{
					case 0:
						AchievementsHelper.NotifyItemPickup(player, inv[slot]);
						break;
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 16:
					case 17:
						AchievementsHelper.HandleOnEquip(player, inv[slot], context);
						break;
					}
				}
				break;
			}
			case 12:
				flag = true;
				if (Main.mouseRight && Main.mouseRightRelease && Main.mouseItem.stack < Main.mouseItem.maxStack && Main.mouseItem.type > 0 && inv[slot].type > 0 && Main.mouseItem.type == inv[slot].type)
				{
					Main.mouseItem.stack++;
					inv[slot].SetDefaults();
					Main.PlaySound(7);
				}
				break;
			case 15:
			{
				flag = true;
				Chest chest = Main.instance.shop[Main.npcShop];
				if (Main.stackSplit > 1 || !Main.mouseRight || inv[slot].type <= 0 || (!Main.mouseItem.IsTheSameAs(inv[slot]) && Main.mouseItem.type != 0))
				{
					break;
				}
				int num = Main.superFastStack + 1;
				for (int i = 0; i < num; i++)
				{
					if ((Main.mouseItem.stack >= Main.mouseItem.maxStack && Main.mouseItem.type != 0) || !player.BuyItem(inv[slot].value) || inv[slot].stack <= 0)
					{
						continue;
					}
					if (i == 0)
					{
						Main.PlaySound(18);
					}
					if (Main.mouseItem.type == 0)
					{
						Main.mouseItem.netDefaults(inv[slot].netID);
						if (inv[slot].prefix != 0)
						{
							Main.mouseItem.Prefix(inv[slot].prefix);
						}
						Main.mouseItem.stack = 0;
					}
					Main.mouseItem.stack++;
					if (Main.stackSplit == 0)
					{
						Main.stackSplit = 15;
					}
					else
					{
						Main.stackSplit = Main.stackDelay;
					}
					if (inv[slot].buyOnce && --inv[slot].stack <= 0)
					{
						inv[slot].SetDefaults();
					}
				}
				break;
			}
			}
			if (flag)
			{
				return;
			}
			if ((context == 0 || context == 4 || context == 3) && Main.mouseRight && Main.mouseRightRelease && inv[slot].maxStack == 1)
			{
				bool success;
				if (inv[slot].dye > 0)
				{
					inv[slot] = DyeSwap(inv[slot], out success);
					if (success)
					{
						Main.EquipPageSelected = 0;
						AchievementsHelper.HandleOnEquip(player, inv[slot], 12);
					}
				}
				else if (Main.projHook[inv[slot].shoot])
				{
					inv[slot] = EquipSwap(inv[slot], player.miscEquips, 4, out success);
					if (success)
					{
						Main.EquipPageSelected = 2;
						AchievementsHelper.HandleOnEquip(player, inv[slot], 16);
					}
				}
				else if (inv[slot].mountType != -1 && !MountID.Sets.Cart[inv[slot].mountType])
				{
					inv[slot] = EquipSwap(inv[slot], player.miscEquips, 3, out success);
					if (success)
					{
						Main.EquipPageSelected = 2;
						AchievementsHelper.HandleOnEquip(player, inv[slot], 17);
					}
				}
				else if (inv[slot].mountType != -1 && MountID.Sets.Cart[inv[slot].mountType])
				{
					inv[slot] = EquipSwap(inv[slot], player.miscEquips, 2, out success);
					if (success)
					{
						Main.EquipPageSelected = 2;
					}
				}
				else if (inv[slot].buffType > 0 && Main.lightPet[inv[slot].buffType])
				{
					inv[slot] = EquipSwap(inv[slot], player.miscEquips, 1, out success);
					if (success)
					{
						Main.EquipPageSelected = 2;
					}
				}
				else if (inv[slot].buffType > 0 && Main.vanityPet[inv[slot].buffType])
				{
					inv[slot] = EquipSwap(inv[slot], player.miscEquips, 0, out success);
					if (success)
					{
						Main.EquipPageSelected = 2;
					}
				}
				else
				{
					inv[slot] = ArmorSwap(inv[slot], out success);
					if (success)
					{
						Main.EquipPageSelected = 0;
						AchievementsHelper.HandleOnEquip(player, inv[slot], 8);
					}
				}
				Recipe.FindRecipes();
				if (context == 3 && Main.netMode == 1)
				{
					NetMessage.SendData(32, -1, -1, "", player.chest, slot);
				}
			}
			else
			{
				if (Main.stackSplit > 1 || !Main.mouseRight)
				{
					return;
				}
				bool flag3 = true;
				if (context == 0 && inv[slot].maxStack <= 1)
				{
					flag3 = false;
				}
				if (context == 3 && inv[slot].maxStack <= 1)
				{
					flag3 = false;
				}
				if (context == 4 && inv[slot].maxStack <= 1)
				{
					flag3 = false;
				}
				if (!flag3 || (!Main.mouseItem.IsTheSameAs(inv[slot]) && Main.mouseItem.type != 0) || (Main.mouseItem.stack >= Main.mouseItem.maxStack && Main.mouseItem.type != 0))
				{
					return;
				}
				if (Main.mouseItem.type == 0)
				{
					Main.mouseItem = inv[slot].Clone();
					Main.mouseItem.stack = 0;
					if (inv[slot].favorited && inv[slot].maxStack == 1)
					{
						Main.mouseItem.favorited = true;
					}
					else
					{
						Main.mouseItem.favorited = false;
					}
				}
				Main.mouseItem.stack++;
				inv[slot].stack--;
				if (inv[slot].stack <= 0)
				{
					inv[slot] = new Item();
				}
				Recipe.FindRecipes();
				Main.soundInstanceMenuTick.Stop();
				Main.soundInstanceMenuTick = Main.soundMenuTick.CreateInstance();
				Main.PlaySound(12);
				if (Main.stackSplit == 0)
				{
					Main.stackSplit = 15;
				}
				else
				{
					Main.stackSplit = Main.stackDelay;
				}
				if (context == 3 && Main.netMode == 1)
				{
					NetMessage.SendData(32, -1, -1, "", player.chest, slot);
				}
			}
		}

		public static void Draw(SpriteBatch spriteBatch, ref Item inv, int context, Vector2 position, Color lightColor = default(Color))
		{
			singleSlotArray[0] = inv;
			Draw(spriteBatch, singleSlotArray, context, 0, position, lightColor);
			inv = singleSlotArray[0];
		}

		public static void Draw(SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor = default(Color))
		{
			Player player = Main.player[Main.myPlayer];
			Item ıtem = inv[slot];
			float inventoryScale = Main.inventoryScale;
			Color color = Color.White;
			if (lightColor != Color.Transparent)
			{
				color = lightColor;
			}
			Texture2D texture2D = Main.inventoryBackTexture;
			Color color2 = Main.inventoryBack;
			bool flag = false;
			if (ıtem.type > 0 && ıtem.stack > 0 && ıtem.favorited && context != 13 && context != 21 && context != 22)
			{
				texture2D = Main.inventoryBack10Texture;
			}
			else if (ıtem.type > 0 && ıtem.stack > 0 && Options.HighlightNewItems && ıtem.newAndShiny && context != 13 && context != 21 && context != 22)
			{
				texture2D = Main.inventoryBack15Texture;
				float num = (float)(int)Main.mouseTextColor / 255f;
				num = num * 0.2f + 0.8f;
				color2 = color2.MultiplyRGBA(new Color(num, num, num));
			}
			else if (context == 0 && slot < 10)
			{
				texture2D = Main.inventoryBack9Texture;
			}
			else
			{
				switch (context)
				{
				case 8:
				case 10:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
					texture2D = Main.inventoryBack3Texture;
					break;
				case 9:
				case 11:
					texture2D = Main.inventoryBack8Texture;
					break;
				case 12:
					texture2D = Main.inventoryBack12Texture;
					break;
				case 3:
					texture2D = Main.inventoryBack5Texture;
					break;
				case 4:
					texture2D = Main.inventoryBack2Texture;
					break;
				case 5:
				case 7:
					texture2D = Main.inventoryBack4Texture;
					break;
				case 6:
					texture2D = Main.inventoryBack7Texture;
					break;
				case 13:
				{
					byte b = 200;
					if (slot == Main.player[Main.myPlayer].selectedItem)
					{
						texture2D = Main.inventoryBack14Texture;
						b = byte.MaxValue;
					}
					color2 = new Color(b, b, b, b);
					break;
				}
				case 14:
				case 21:
					flag = true;
					break;
				case 15:
					texture2D = Main.inventoryBack6Texture;
					break;
				case 22:
					texture2D = Main.inventoryBack4Texture;
					break;
				}
			}
			if (!flag)
			{
				spriteBatch.Draw(texture2D, position, null, color2, 0f, default(Vector2), inventoryScale, SpriteEffects.None, 0f);
			}
			int num2 = -1;
			switch (context)
			{
			case 8:
				if (slot == 0)
				{
					num2 = 0;
				}
				if (slot == 1)
				{
					num2 = 6;
				}
				if (slot == 2)
				{
					num2 = 12;
				}
				break;
			case 9:
				if (slot == 10)
				{
					num2 = 3;
				}
				if (slot == 11)
				{
					num2 = 9;
				}
				if (slot == 12)
				{
					num2 = 15;
				}
				break;
			case 10:
				num2 = 11;
				break;
			case 11:
				num2 = 2;
				break;
			case 12:
				num2 = 1;
				break;
			case 16:
				num2 = 4;
				break;
			case 17:
				num2 = 13;
				break;
			case 19:
				num2 = 10;
				break;
			case 18:
				num2 = 7;
				break;
			case 20:
				num2 = 17;
				break;
			}
			if ((ıtem.type <= 0 || ıtem.stack <= 0) && num2 != -1)
			{
				Texture2D texture2D2 = Main.extraTexture[54];
				Rectangle rectangle = texture2D2.Frame(3, 6, num2 % 3, num2 / 3);
				rectangle.Width -= 2;
				rectangle.Height -= 2;
				spriteBatch.Draw(texture2D2, position + texture2D.Size() / 2f * inventoryScale, rectangle, Color.White * 0.35f, 0f, rectangle.Size() / 2f, inventoryScale, SpriteEffects.None, 0f);
			}
			if (ıtem.type > 0 && ıtem.stack > 0)
			{
				Texture2D texture2D3 = Main.itemTexture[ıtem.type];
				Rectangle rectangle2 = (Main.itemAnimations[ıtem.type] == null) ? texture2D3.Frame() : Main.itemAnimations[ıtem.type].GetFrame(texture2D3);
				Color currentColor = color;
				float scale = 1f;
				GetItemLight(ref currentColor, ref scale, ıtem);
				float num3 = 1f;
				if (rectangle2.Width > 32 || rectangle2.Height > 32)
				{
					num3 = ((rectangle2.Width <= rectangle2.Height) ? (32f / (float)rectangle2.Height) : (32f / (float)rectangle2.Width));
				}
				num3 *= inventoryScale;
				Vector2 position2 = position + texture2D.Size() * inventoryScale / 2f - rectangle2.Size() * num3 / 2f;
				Vector2 origin = rectangle2.Size() * (scale / 2f - 0.5f);
				spriteBatch.Draw(texture2D3, position2, rectangle2, ıtem.GetAlpha(currentColor), 0f, origin, num3 * scale, SpriteEffects.None, 0f);
				if (ıtem.color != Color.Transparent)
				{
					spriteBatch.Draw(texture2D3, position2, rectangle2, ıtem.GetColor(color), 0f, origin, num3 * scale, SpriteEffects.None, 0f);
				}
				if (ıtem.stack > 1)
				{
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, ıtem.stack.ToString(), position + new Vector2(10f, 26f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
				}
				int num4 = -1;
				if (context == 13)
				{
					if (ıtem.useAmmo > 0)
					{
						int useAmmo = ıtem.useAmmo;
						num4 = 0;
						for (int i = 0; i < 58; i++)
						{
							if (inv[i].ammo == useAmmo)
							{
								num4 += inv[i].stack;
							}
						}
					}
					if (ıtem.fishingPole > 0)
					{
						num4 = 0;
						for (int j = 0; j < 58; j++)
						{
							if (inv[j].bait > 0)
							{
								num4 += inv[j].stack;
							}
						}
					}
					if (ıtem.tileWand > 0)
					{
						int tileWand = ıtem.tileWand;
						num4 = 0;
						for (int k = 0; k < 58; k++)
						{
							if (inv[k].type == tileWand)
							{
								num4 += inv[k].stack;
							}
						}
					}
					if (ıtem.type == 509 || ıtem.type == 851 || ıtem.type == 850)
					{
						num4 = 0;
						for (int l = 0; l < 58; l++)
						{
							if (inv[l].type == 530)
							{
								num4 += inv[l].stack;
							}
						}
					}
				}
				if (num4 != -1)
				{
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, num4.ToString(), position + new Vector2(8f, 30f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale * 0.8f), -1f, inventoryScale);
				}
				if (context == 13)
				{
					string text = string.Concat(slot + 1);
					if (text == "10")
					{
						text = "0";
					}
					ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, position + new Vector2(8f, 4f) * inventoryScale, color, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
				}
				if (context == 13 && ıtem.potion)
				{
					Vector2 position3 = position + texture2D.Size() * inventoryScale / 2f - Main.cdTexture.Size() * inventoryScale / 2f;
					Color color3 = ıtem.GetAlpha(color) * ((float)player.potionDelay / (float)player.potionDelayTime);
					spriteBatch.Draw(Main.cdTexture, position3, null, color3, 0f, default(Vector2), num3, SpriteEffects.None, 0f);
				}
				if ((context == 10 || context == 18) && ıtem.expertOnly && !Main.expertMode)
				{
					Vector2 position4 = position + texture2D.Size() * inventoryScale / 2f - Main.cdTexture.Size() * inventoryScale / 2f;
					Color white = Color.White;
					spriteBatch.Draw(Main.cdTexture, position4, null, white, 0f, default(Vector2), num3, SpriteEffects.None, 0f);
				}
			}
			else if (context == 6)
			{
				Texture2D trashTexture = Main.trashTexture;
				Vector2 position5 = position + texture2D.Size() * inventoryScale / 2f - trashTexture.Size() * inventoryScale / 2f;
				spriteBatch.Draw(trashTexture, position5, null, new Color(100, 100, 100, 100), 0f, default(Vector2), inventoryScale, SpriteEffects.None, 0f);
			}
			if (context == 0 && slot < 10)
			{
				float num5 = inventoryScale;
				string text2 = string.Concat(slot + 1);
				if (text2 == "10")
				{
					text2 = "0";
				}
				Color inventoryBack = Main.inventoryBack;
				int num6 = 0;
				if (Main.player[Main.myPlayer].selectedItem == slot)
				{
					num6 -= 3;
					inventoryBack.R = byte.MaxValue;
					inventoryBack.B = 0;
					inventoryBack.G = 210;
					inventoryBack.A = 100;
					num5 *= 1.4f;
				}
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text2, position + new Vector2(6f, 4 + num6) * inventoryScale, inventoryBack, 0f, Vector2.Zero, new Vector2(inventoryScale), -1f, inventoryScale);
			}
		}

		public static void MouseHover(ref Item inv, int context = 0)
		{
			singleSlotArray[0] = inv;
			MouseHover(singleSlotArray, context);
			inv = singleSlotArray[0];
		}

		public static void MouseHover(Item[] inv, int context = 0, int slot = 0)
		{
			if (context == 6 && Main.hoverItemName == null)
			{
				Main.hoverItemName = Lang.inter[3].Value;
			}
			if (inv[slot].type > 0 && inv[slot].stack > 0)
			{
				Main.hoverItemName = inv[slot].Name;
				if (inv[slot].stack > 1)
				{
					object hoverItemName = Main.hoverItemName;
					Main.hoverItemName = string.Concat(hoverItemName, " (", inv[slot].stack, ")");
				}
				Main.toolTip = inv[slot].Clone();
				if (context == 8 && slot <= 2)
				{
					Main.toolTip.wornArmor = true;
				}
				if (context == 11 || context == 9)
				{
					Main.toolTip.social = true;
				}
				if (context == 15)
				{
					Main.toolTip.buy = true;
				}
				return;
			}
			if (context == 10 || context == 11)
			{
				Main.hoverItemName = Lang.inter[9].Value;
			}
			if (context == 11)
			{
				Main.hoverItemName = string.Concat(Lang.inter[11], " ", Main.hoverItemName);
			}
			if (context == 8 || context == 9)
			{
				if (slot == 0 || slot == 10)
				{
					Main.hoverItemName = Lang.inter[12].Value;
				}
				if (slot == 1 || slot == 11)
				{
					Main.hoverItemName = Lang.inter[13].Value;
				}
				if (slot == 2 || slot == 12)
				{
					Main.hoverItemName = Lang.inter[14].Value;
				}
				if (slot >= 10)
				{
					Main.hoverItemName = string.Concat(Lang.inter[11], " ", Main.hoverItemName);
				}
			}
			if (context == 12)
			{
				Main.hoverItemName = Lang.inter[57].Value;
			}
			if (context == 16)
			{
				Main.hoverItemName = Lang.inter[90].Value;
			}
			if (context == 17)
			{
				Main.hoverItemName = Lang.inter[91].Value;
			}
			if (context == 19)
			{
				Main.hoverItemName = Lang.inter[92].Value;
			}
			if (context == 18)
			{
				Main.hoverItemName = Lang.inter[93].Value;
			}
			if (context == 20)
			{
				Main.hoverItemName = Lang.inter[94].Value;
			}
		}

		private static bool AccCheck(Item item, int slot)
		{
			Player player = Main.player[Main.myPlayer];
			if (slot != -1)
			{
				if (player.armor[slot].IsTheSameAs(item))
				{
					return false;
				}
				if (player.armor[slot].wingSlot > 0 && item.wingSlot > 0)
				{
					return false;
				}
			}
			for (int i = 0; i < player.armor.Length; i++)
			{
				if (slot < 10 && i < 10)
				{
					if (item.wingSlot > 0 && player.armor[i].wingSlot > 0)
					{
						return true;
					}
					if (slot >= 10 && i >= 10 && item.wingSlot > 0 && player.armor[i].wingSlot > 0)
					{
						return true;
					}
				}
				if (item.IsTheSameAs(player.armor[i]))
				{
					return true;
				}
			}
			return false;
		}

		private static Item DyeSwap(Item item, out bool success)
		{
			success = false;
			if (item.dye <= 0)
			{
				return item;
			}
			Player player = Main.player[Main.myPlayer];
			Item ıtem = item;
			for (int i = 0; i < 10; i++)
			{
				if (player.dye[i].type == 0)
				{
					dyeSlotCount = i;
					break;
				}
			}
			if (dyeSlotCount >= 10)
			{
				dyeSlotCount = 0;
			}
			if (dyeSlotCount < 0)
			{
				dyeSlotCount = 9;
			}
			ıtem = player.dye[dyeSlotCount].Clone();
			player.dye[dyeSlotCount] = item.Clone();
			dyeSlotCount++;
			if (dyeSlotCount >= 10)
			{
				accSlotCount = 0;
			}
			Main.PlaySound(7);
			Recipe.FindRecipes();
			success = true;
			return ıtem;
		}

		private static Item ArmorSwap(Item item, out bool success)
		{
			success = false;
			if (item.headSlot == -1 && item.bodySlot == -1 && item.legSlot == -1 && !item.accessory)
			{
				return item;
			}
			Player player = Main.player[Main.myPlayer];
			int num = (item.vanity && !item.accessory) ? 10 : 0;
			item.favorited = false;
			Item result = item;
			if (item.headSlot != -1)
			{
				result = player.armor[num].Clone();
				player.armor[num] = item.Clone();
			}
			else if (item.bodySlot != -1)
			{
				result = player.armor[num + 1].Clone();
				player.armor[num + 1] = item.Clone();
			}
			else if (item.legSlot != -1)
			{
				result = player.armor[num + 2].Clone();
				player.armor[num + 2] = item.Clone();
			}
			else if (item.accessory)
			{
				int num2 = 5 + Main.player[Main.myPlayer].extraAccessorySlots;
				for (int i = 3; i < 3 + num2; i++)
				{
					if (player.armor[i].type == 0)
					{
						accSlotCount = i - 3;
						break;
					}
				}
				for (int j = 0; j < player.armor.Length; j++)
				{
					if (item.IsTheSameAs(player.armor[j]))
					{
						accSlotCount = j - 3;
					}
					if (j < 10 && item.wingSlot > 0 && player.armor[j].wingSlot > 0)
					{
						accSlotCount = j - 3;
					}
				}
				if (accSlotCount >= num2)
				{
					accSlotCount = 0;
				}
				if (accSlotCount < 0)
				{
					accSlotCount = num2 - 1;
				}
				int num3 = 3 + accSlotCount;
				for (int k = 0; k < player.armor.Length; k++)
				{
					if (item.IsTheSameAs(player.armor[k]))
					{
						num3 = k;
					}
				}
				result = player.armor[num3].Clone();
				player.armor[num3] = item.Clone();
				accSlotCount++;
				if (accSlotCount >= num2)
				{
					accSlotCount = 0;
				}
			}
			Main.PlaySound(7);
			Recipe.FindRecipes();
			success = true;
			return result;
		}

		private static Item EquipSwap(Item item, Item[] inv, int slot, out bool success)
		{
			success = false;
			Player player = Main.player[Main.myPlayer];
			item.favorited = false;
			Item result = inv[slot].Clone();
			inv[slot] = item.Clone();
			Main.PlaySound(7);
			Recipe.FindRecipes();
			success = true;
			return result;
		}

		public static void EquipPage(Item item)
		{
			Main.EquipPage = -1;
			if (Main.projHook[item.shoot])
			{
				Main.EquipPage = 2;
			}
			else if (item.mountType != -1)
			{
				Main.EquipPage = 2;
			}
			else if (item.buffType > 0 && Main.vanityPet[item.buffType])
			{
				Main.EquipPage = 2;
			}
			else if (item.buffType > 0 && Main.lightPet[item.buffType])
			{
				Main.EquipPage = 2;
			}
			else if (item.dye > 0 && Main.EquipPageSelected == 1)
			{
				Main.EquipPage = 0;
			}
			else if (item.legSlot != -1 || item.headSlot != -1 || item.bodySlot != -1 || item.accessory)
			{
				Main.EquipPage = 0;
			}
		}

		public static void DrawMoney(SpriteBatch sb, string text, float shopx, float shopy, int[] coinsArray, bool horizontal = false)
		{
			Utils.DrawBorderStringFourWay(sb, Main.fontMouseText, text, shopx, shopy + 40f, Color.White * ((float)(int)Main.mouseTextColor / 255f), Color.Black, Vector2.Zero);
			if (horizontal)
			{
				for (int i = 0; i < 4; i++)
				{
					if (i == 0)
					{
						int num2 = coinsArray[3 - i];
					}
					Vector2 position = new Vector2(shopx + ChatManager.GetStringSize(Main.fontMouseText, text, Vector2.One).X + (float)(24 * i) + 45f, shopy + 50f);
					sb.Draw(Main.itemTexture[74 - i], position, null, Color.White, 0f, Main.itemTexture[74 - i].Size() / 2f, 1f, SpriteEffects.None, 0f);
					Utils.DrawBorderStringFourWay(sb, Main.fontItemStack, coinsArray[3 - i].ToString(), position.X - 11f, position.Y, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
				}
			}
			else
			{
				for (int j = 0; j < 4; j++)
				{
					int num = (j == 0 && coinsArray[3 - j] > 99) ? (-6) : 0;
					sb.Draw(Main.itemTexture[74 - j], new Vector2(shopx + 11f + (float)(24 * j), shopy + 75f), null, Color.White, 0f, Main.itemTexture[74 - j].Size() / 2f, 1f, SpriteEffects.None, 0f);
					Utils.DrawBorderStringFourWay(sb, Main.fontItemStack, coinsArray[3 - j].ToString(), shopx + (float)(24 * j) + (float)num, shopy + 75f, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
				}
			}
		}

		public static void DrawSavings(SpriteBatch sb, float shopx, float shopy, bool horizontal = false)
		{
			Player player = Main.player[Main.myPlayer];
			bool overFlowing;
			long num = Utils.CoinsCount(out overFlowing, player.bank.item);
			long num2 = Utils.CoinsCount(out overFlowing, player.bank2.item);
			long num3 = Utils.CoinsCombineStacks(out overFlowing, num, num2);
			if (num3 > 0)
			{
				if (num2 > 0)
				{
					sb.Draw(Main.itemTexture[346], Utils.CenteredRectangle(new Vector2(shopx + 80f, shopy + 50f), Main.itemTexture[346].Size() * 0.65f), null, Color.White);
				}
				if (num > 0)
				{
					sb.Draw(Main.itemTexture[87], Utils.CenteredRectangle(new Vector2(shopx + 70f, shopy + 60f), Main.itemTexture[87].Size() * 0.65f), null, Color.White);
				}
				DrawMoney(sb, Lang.inter[66].Value, shopx, shopy, Utils.CoinsSplit(num3), horizontal);
			}
		}

		public static void GetItemLight(ref Color currentColor, Item item, bool outInTheWorld = false)
		{
			float scale = 1f;
			GetItemLight(ref currentColor, ref scale, item, outInTheWorld);
		}

		public static void GetItemLight(ref Color currentColor, int type, bool outInTheWorld = false)
		{
			float scale = 1f;
			GetItemLight(ref currentColor, ref scale, type, outInTheWorld);
		}

		public static void GetItemLight(ref Color currentColor, ref float scale, Item item, bool outInTheWorld = false)
		{
			GetItemLight(ref currentColor, ref scale, item.type, outInTheWorld);
		}

		public static Color GetItemLight(ref Color currentColor, ref float scale, int type, bool outInTheWorld = false)
		{
			if (type < 0 || type > 3602)
			{
				return currentColor;
			}
			if (type == 662 || type == 663)
			{
				currentColor.R = (byte)Main.DiscoR;
				currentColor.G = (byte)Main.DiscoG;
				currentColor.B = (byte)Main.DiscoB;
				currentColor.A = byte.MaxValue;
			}
			else if (ItemID.Sets.ItemIconPulse[type])
			{
				scale = Main.essScale;
				currentColor.R = (byte)((float)(int)currentColor.R * scale);
				currentColor.G = (byte)((float)(int)currentColor.G * scale);
				currentColor.B = (byte)((float)(int)currentColor.B * scale);
				currentColor.A = (byte)((float)(int)currentColor.A * scale);
			}
			else if (type == 58 || type == 184)
			{
				scale = Main.essScale * 0.25f + 0.75f;
				currentColor.R = (byte)((float)(int)currentColor.R * scale);
				currentColor.G = (byte)((float)(int)currentColor.G * scale);
				currentColor.B = (byte)((float)(int)currentColor.B * scale);
				currentColor.A = (byte)((float)(int)currentColor.A * scale);
			}
			return currentColor;
		}
	}
}
