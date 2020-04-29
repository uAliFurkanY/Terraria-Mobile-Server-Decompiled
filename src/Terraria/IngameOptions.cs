using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Terraria.GameContent;
using Terraria.Social;
using Terraria.UI;

namespace Terraria
{
	public static class IngameOptions
	{
		public const int width = 670;

		public const int height = 480;

		public static float[] leftScale = new float[8]
		{
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f
		};

		public static float[] rightScale = new float[15]
		{
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f,
			0.7f
		};

		public static int leftHover = -1;

		public static int rightHover = -1;

		public static int oldLeftHover = -1;

		public static int oldRightHover = -1;

		public static int rightLock = -1;

		public static bool inBar = false;

		public static bool notBar = false;

		public static bool noSound = false;

		private static Rectangle _GUIHover = default(Rectangle);

		public static int category = 0;

		public static Vector2 valuePosition = Vector2.Zero;

		public static void Open()
		{
			Main.playerInventory = false;
			Main.editChest = false;
			Main.npcChatText = "";
			Main.PlaySound(10);
			Main.ingameOptionsWindow = true;
			category = 0;
			for (int i = 0; i < leftScale.Length; i++)
			{
				leftScale[i] = 0f;
			}
			for (int j = 0; j < rightScale.Length; j++)
			{
				rightScale[j] = 0f;
			}
			leftHover = -1;
			rightHover = -1;
			oldLeftHover = -1;
			oldRightHover = -1;
			rightLock = -1;
			inBar = false;
			notBar = false;
			noSound = false;
		}

		public static void Close()
		{
			if (Main.setKey == -1)
			{
				Main.ingameOptionsWindow = false;
				Main.PlaySound(11);
				Recipe.FindRecipes();
				Main.playerInventory = true;
				Main.SaveSettings();
			}
		}

		public static void Draw(Main mainInstance, SpriteBatch sb)
		{
			if (Main.player[Main.myPlayer].dead && !Main.player[Main.myPlayer].ghost)
			{
				Main.setKey = -1;
				Close();
				Main.playerInventory = false;
				return;
			}
			new Vector2(Main.mouseX, Main.mouseY);
			bool flag = Main.mouseLeft && Main.mouseLeftRelease;
			Vector2 value = new Vector2(Main.screenWidth, Main.screenHeight);
			Vector2 value2 = new Vector2(670f, 480f);
			Vector2 value3 = value / 2f - value2 / 2f;
			int num = 20;
			_GUIHover = new Rectangle((int)(value3.X - (float)num), (int)(value3.Y - (float)num), (int)(value2.X + (float)(num * 2)), (int)(value2.Y + (float)(num * 2)));
			Utils.DrawInvBG(sb, value3.X - (float)num, value3.Y - (float)num, value2.X + (float)(num * 2), value2.Y + (float)(num * 2), new Color(33, 15, 91, 255) * 0.685f);
			if (new Rectangle((int)value3.X - num, (int)value3.Y - num, (int)value2.X + num * 2, (int)value2.Y + num * 2).Contains(new Point(Main.mouseX, Main.mouseY)))
			{
				Main.player[Main.myPlayer].mouseInterface = true;
			}
			Utils.DrawInvBG(sb, value3.X + (float)(num / 2), value3.Y + (float)(num * 5 / 2), value2.X / 2f - (float)num, value2.Y - (float)(num * 3));
			Utils.DrawInvBG(sb, value3.X + value2.X / 2f + (float)num, value3.Y + (float)(num * 5 / 2), value2.X / 2f - (float)(num * 3 / 2), value2.Y - (float)(num * 3));
			Utils.DrawBorderString(sb, "Settings Menu", value3 + value2 * new Vector2(0.5f, 0f), Color.White, 1f, 0.5f);
			float num2 = 0.7f;
			float num3 = 0.8f;
			float num4 = 0.01f;
			if (oldLeftHover != leftHover && leftHover != -1)
			{
				Main.PlaySound(12);
			}
			if (oldRightHover != rightHover && rightHover != -1)
			{
				Main.PlaySound(12);
			}
			if (flag && rightHover != -1 && !noSound)
			{
				Main.PlaySound(12);
			}
			oldLeftHover = leftHover;
			oldRightHover = rightHover;
			noSound = false;
			bool flag2 = SocialAPI.Network != null && SocialAPI.Network.CanInvite();
			int num5 = flag2 ? 1 : 0;
			int num6 = 6 + num5;
			Vector2 anchor = new Vector2(value3.X + value2.X / 4f, value3.Y + (float)(num * 5 / 2));
			Vector2 offset = new Vector2(0f, value2.Y - (float)(num * 5)) / (num6 + 1);
			for (int i = 0; i <= num6; i++)
			{
				if (leftHover == i || i == category)
				{
					leftScale[i] += num4;
				}
				else
				{
					leftScale[i] -= num4;
				}
				if (leftScale[i] < num2)
				{
					leftScale[i] = num2;
				}
				if (leftScale[i] > num3)
				{
					leftScale[i] = num3;
				}
			}
			leftHover = -1;
			int num7 = category;
			if (DrawLeftSide(sb, Lang.menu[114].Value, 0, anchor, offset, leftScale))
			{
				leftHover = 0;
				if (flag)
				{
					category = 0;
					Main.PlaySound(10);
				}
			}
			if (DrawLeftSide(sb, Lang.menu[63].Value, 1, anchor, offset, leftScale))
			{
				leftHover = 1;
				if (flag)
				{
					category = 1;
					Main.PlaySound(10);
				}
			}
			if (DrawLeftSide(sb, Lang.menu[66].Value, 2, anchor, offset, leftScale))
			{
				leftHover = 2;
				if (flag)
				{
					category = 2;
					Main.PlaySound(10);
				}
			}
			if (DrawLeftSide(sb, Lang.menu[115].Value, 3, anchor, offset, leftScale))
			{
				leftHover = 3;
				if (flag)
				{
					category = 3;
					Main.PlaySound(10);
				}
			}
			if (flag2 && DrawLeftSide(sb, Lang.menu[147].Value, 4, anchor, offset, leftScale))
			{
				leftHover = 4;
				if (flag)
				{
					Close();
					SocialAPI.Network.OpenInviteInterface();
				}
			}
			if (DrawLeftSide(sb, Lang.menu[131].Value, 4 + num5, anchor, offset, leftScale))
			{
				leftHover = 4 + num5;
				if (flag)
				{
					Close();
					AchievementsUI.Open();
				}
			}
			if (DrawLeftSide(sb, Lang.menu[118].Value, 5 + num5, anchor, offset, leftScale))
			{
				leftHover = 5 + num5;
				if (flag)
				{
					Close();
				}
			}
			if (DrawLeftSide(sb, Lang.inter[35].Value, 6 + num5, anchor, offset, leftScale))
			{
				leftHover = 6 + num5;
				if (flag)
				{
					Close();
					Main.menuMode = 10;
					WorldGen.SaveAndQuit();
				}
			}
			if (num7 != category)
			{
				for (int j = 0; j < rightScale.Length; j++)
				{
					rightScale[j] = 0f;
				}
			}
			int num8 = 0;
			switch (category)
			{
			case 0:
				num8 = 11;
				num2 = 1f;
				num3 = 1.001f;
				num4 = 0.001f;
				break;
			case 1:
				num8 = 8;
				num2 = 1f;
				num3 = 1.001f;
				num4 = 0.001f;
				break;
			case 2:
				num8 = 14;
				num2 = 0.8f;
				num3 = 0.801f;
				num4 = 0.001f;
				break;
			case 3:
				num8 = 7;
				num2 = 0.8f;
				num3 = 0.801f;
				num4 = 0.001f;
				break;
			}
			Vector2 anchor2 = new Vector2(value3.X + value2.X * 3f / 4f, value3.Y + (float)(num * 5 / 2));
			Vector2 offset2 = new Vector2(0f, value2.Y - (float)(num * 3)) / (num8 + 1);
			if (category == 2)
			{
				offset2.Y -= 2f;
			}
			for (int k = 0; k < 15; k++)
			{
				if (rightLock == k || (rightHover == k && rightLock == -1))
				{
					rightScale[k] += num4;
				}
				else
				{
					rightScale[k] -= num4;
				}
				if (rightScale[k] < num2)
				{
					rightScale[k] = num2;
				}
				if (rightScale[k] > num3)
				{
					rightScale[k] = num3;
				}
			}
			inBar = false;
			rightHover = -1;
			if (!Main.mouseLeft)
			{
				rightLock = -1;
			}
			if (rightLock == -1)
			{
				notBar = false;
			}
			if (category == 0)
			{
				int num9 = 0;
				anchor2.X -= 70f;
				if (DrawRightSide(sb, string.Concat(Lang.menu[99], " ", Math.Round(Main.musicVolume * 100f), "%"), num9, anchor2, offset2, rightScale[num9], (rightScale[num9] - num2) / (num3 - num2)))
				{
					if (rightLock == -1)
					{
						notBar = true;
					}
					noSound = true;
					rightHover = num9;
				}
				valuePosition.X = value3.X + value2.X - (float)(num / 2) - 20f;
				valuePosition.Y -= 3f;
				float musicVolume = DrawValueBar(sb, 0.75f, Main.musicVolume);
				if ((inBar || rightLock == num9) && !notBar)
				{
					rightHover = num9;
					if (Main.mouseLeft && rightLock == num9)
					{
						Main.musicVolume = musicVolume;
					}
				}
				if ((float)Main.mouseX > value3.X + value2.X * 2f / 3f + (float)num && (float)Main.mouseX < valuePosition.X + 3.75f && (float)Main.mouseY > valuePosition.Y - 10f && (float)Main.mouseY <= valuePosition.Y + 10f)
				{
					if (rightLock == -1)
					{
						notBar = true;
					}
					rightHover = num9;
				}
				num9++;
				if (DrawRightSide(sb, string.Concat(Lang.menu[98], " ", Math.Round(Main.soundVolume * 100f), "%"), num9, anchor2, offset2, rightScale[num9], (rightScale[num9] - num2) / (num3 - num2)))
				{
					if (rightLock == -1)
					{
						notBar = true;
					}
					rightHover = num9;
				}
				valuePosition.X = value3.X + value2.X - (float)(num / 2) - 20f;
				valuePosition.Y -= 3f;
				float soundVolume = DrawValueBar(sb, 0.75f, Main.soundVolume);
				if ((inBar || rightLock == num9) && !notBar)
				{
					rightHover = num9;
					if (Main.mouseLeft && rightLock == num9)
					{
						Main.soundVolume = soundVolume;
						noSound = true;
					}
				}
				if ((float)Main.mouseX > value3.X + value2.X * 2f / 3f + (float)num && (float)Main.mouseX < valuePosition.X + 3.75f && (float)Main.mouseY > valuePosition.Y - 10f && (float)Main.mouseY <= valuePosition.Y + 10f)
				{
					if (rightLock == -1)
					{
						notBar = true;
					}
					rightHover = num9;
				}
				num9++;
				if (DrawRightSide(sb, string.Concat(Lang.menu[119], " ", Math.Round(Main.ambientVolume * 100f), "%"), num9, anchor2, offset2, rightScale[num9], (rightScale[num9] - num2) / (num3 - num2)))
				{
					if (rightLock == -1)
					{
						notBar = true;
					}
					rightHover = num9;
				}
				valuePosition.X = value3.X + value2.X - (float)(num / 2) - 20f;
				valuePosition.Y -= 3f;
				float ambientVolume = DrawValueBar(sb, 0.75f, Main.ambientVolume);
				if ((inBar || rightLock == num9) && !notBar)
				{
					rightHover = num9;
					if (Main.mouseLeft && rightLock == num9)
					{
						Main.ambientVolume = ambientVolume;
						noSound = true;
					}
				}
				if ((float)Main.mouseX > value3.X + value2.X * 2f / 3f + (float)num && (float)Main.mouseX < valuePosition.X + 3.75f && (float)Main.mouseY > valuePosition.Y - 10f && (float)Main.mouseY <= valuePosition.Y + 10f)
				{
					if (rightLock == -1)
					{
						notBar = true;
					}
					rightHover = num9;
				}
				num9++;
				anchor2.X += 70f;
				if (DrawRightSide(sb, Main.autoSave ? Lang.menu[67].Value : Lang.menu[68].Value, num9, anchor2, offset2, rightScale[num9], (rightScale[num9] - num2) / (num3 - num2)))
				{
					rightHover = num9;
					if (flag)
					{
						Main.autoSave = !Main.autoSave;
					}
				}
				num9++;
				if (DrawRightSide(sb, Main.autoPause ? Lang.menu[69].Value : Lang.menu[70].Value, num9, anchor2, offset2, rightScale[num9], (rightScale[num9] - num2) / (num3 - num2)))
				{
					rightHover = num9;
					if (flag)
					{
						Main.autoPause = !Main.autoPause;
					}
				}
				num9++;
				if (DrawRightSide(sb, Main.showItemText ? Lang.menu[71].Value : Lang.menu[72].Value, num9, anchor2, offset2, rightScale[num9], (rightScale[num9] - num2) / (num3 - num2)))
				{
					rightHover = num9;
					if (flag)
					{
						Main.showItemText = !Main.showItemText;
					}
				}
				num9++;
				if (DrawRightSide(sb, Main.cSmartToggle ? Lang.menu[121].Value : Lang.menu[122].Value, num9, anchor2, offset2, rightScale[num9], (rightScale[num9] - num2) / (num3 - num2)))
				{
					rightHover = num9;
					if (flag)
					{
						Main.cSmartToggle = !Main.cSmartToggle;
					}
				}
				num9++;
				if (DrawRightSide(sb, string.Concat(Lang.menu[123], " ", Lang.menu[124 + Main.invasionProgressMode]), num9, anchor2, offset2, rightScale[num9], (rightScale[num9] - num2) / (num3 - num2)))
				{
					rightHover = num9;
					if (flag)
					{
						Main.invasionProgressMode++;
						if (Main.invasionProgressMode >= 3)
						{
							Main.invasionProgressMode = 0;
						}
					}
				}
				num9++;
				if (DrawRightSide(sb, Main.placementPreview ? Lang.menu[128].Value : Lang.menu[129].Value, num9, anchor2, offset2, rightScale[num9], (rightScale[num9] - num2) / (num3 - num2)))
				{
					rightHover = num9;
					if (flag)
					{
						Main.placementPreview = !Main.placementPreview;
					}
				}
				num9++;
				if (DrawRightSide(sb, ChildSafety.Disabled ? Lang.menu[132].Value : Lang.menu[133].Value, num9, anchor2, offset2, rightScale[num9], (rightScale[num9] - num2) / (num3 - num2)))
				{
					rightHover = num9;
					if (flag)
					{
						ChildSafety.Disabled = !ChildSafety.Disabled;
					}
				}
				num9++;
				if (DrawRightSide(sb, ItemSlot.Options.HighlightNewItems ? Lang.inter[117].Value : Lang.inter[116].Value, num9, anchor2, offset2, rightScale[num9], (rightScale[num9] - num2) / (num3 - num2)))
				{
					rightHover = num9;
					if (flag)
					{
						ItemSlot.Options.HighlightNewItems = !ItemSlot.Options.HighlightNewItems;
					}
				}
				num9++;
			}
			if (category == 1)
			{
				int num10 = 0;
				if (DrawRightSide(sb, Main.graphics.IsFullScreen ? Lang.menu[49].Value : Lang.menu[50].Value, num10, anchor2, offset2, rightScale[num10], (rightScale[num10] - num2) / (num3 - num2)))
				{
					rightHover = num10;
					if (flag)
					{
						Main.ToggleFullScreen();
					}
				}
				num10++;
				if (DrawRightSide(sb, string.Concat(Lang.menu[51], ": ", Main.PendingResolutionWidth, "x", Main.PendingResolutionHeight), num10, anchor2, offset2, rightScale[num10], (rightScale[num10] - num2) / (num3 - num2)))
				{
					rightHover = num10;
					if (flag)
					{
						int num11 = 0;
						for (int l = 0; l < Main.numDisplayModes; l++)
						{
							if (Main.displayWidth[l] == Main.PendingResolutionWidth && Main.displayHeight[l] == Main.PendingResolutionHeight)
							{
								num11 = l;
								break;
							}
						}
						num11++;
						if (num11 >= Main.numDisplayModes)
						{
							num11 = 0;
						}
						Main.PendingResolutionWidth = Main.displayWidth[num11];
						Main.PendingResolutionHeight = Main.displayHeight[num11];
					}
				}
				num10++;
				anchor2.X -= 70f;
				if (DrawRightSide(sb, string.Concat(Lang.menu[52], ": ", Main.bgScroll, "%"), num10, anchor2, offset2, rightScale[num10], (rightScale[num10] - num2) / (num3 - num2)))
				{
					if (rightLock == -1)
					{
						notBar = true;
					}
					noSound = true;
					rightHover = num10;
				}
				valuePosition.X = value3.X + value2.X - (float)(num / 2) - 20f;
				valuePosition.Y -= 3f;
				float num12 = DrawValueBar(sb, 0.75f, (float)Main.bgScroll / 100f);
				if ((inBar || rightLock == num10) && !notBar)
				{
					rightHover = num10;
					if (Main.mouseLeft && rightLock == num10)
					{
						Main.bgScroll = (int)(num12 * 100f);
						Main.caveParallax = 1f - (float)Main.bgScroll / 500f;
					}
				}
				if ((float)Main.mouseX > value3.X + value2.X * 2f / 3f + (float)num && (float)Main.mouseX < valuePosition.X + 3.75f && (float)Main.mouseY > valuePosition.Y - 10f && (float)Main.mouseY <= valuePosition.Y + 10f)
				{
					if (rightLock == -1)
					{
						notBar = true;
					}
					rightHover = num10;
				}
				num10++;
				anchor2.X += 70f;
				if (DrawRightSide(sb, Main.terrariasFixedTiming ? Lang.menu[53].Value : Lang.menu[54].Value, num10, anchor2, offset2, rightScale[num10], (rightScale[num10] - num2) / (num3 - num2)))
				{
					rightHover = num10;
					if (flag)
					{
						Main.terrariasFixedTiming = !Main.terrariasFixedTiming;
					}
				}
				num10++;
				if (DrawRightSide(sb, Lang.menu[55 + Lighting.lightMode].Value, num10, anchor2, offset2, rightScale[num10], (rightScale[num10] - num2) / (num3 - num2)))
				{
					rightHover = num10;
					if (flag)
					{
						Lighting.NextLightMode();
					}
				}
				num10++;
				if (DrawRightSide(sb, string.Concat(Lang.menu[116], " ", (Lighting.LightingThreads > 0) ? string.Concat(Lighting.LightingThreads + 1) : Lang.menu[117].Value), num10, anchor2, offset2, rightScale[num10], (rightScale[num10] - num2) / (num3 - num2)))
				{
					rightHover = num10;
					if (flag)
					{
						Lighting.LightingThreads++;
						if (Lighting.LightingThreads > Environment.ProcessorCount - 1)
						{
							Lighting.LightingThreads = 0;
						}
					}
				}
				num10++;
				if (DrawRightSide(sb, Lang.menu[59 + Main.qaStyle].Value, num10, anchor2, offset2, rightScale[num10], (rightScale[num10] - num2) / (num3 - num2)))
				{
					rightHover = num10;
					if (flag)
					{
						Main.qaStyle++;
						if (Main.qaStyle > 3)
						{
							Main.qaStyle = 0;
						}
					}
				}
				num10++;
				if (DrawRightSide(sb, Main.owBack ? Lang.menu[100].Value : Lang.menu[101].Value, num10, anchor2, offset2, rightScale[num10], (rightScale[num10] - num2) / (num3 - num2)))
				{
					rightHover = num10;
					if (flag)
					{
						Main.owBack = !Main.owBack;
					}
				}
				num10++;
			}
			if (category == 2)
			{
				int num13 = 0;
				int num14 = 0;
				anchor2.X -= 30f;
				num14 = 0;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cUp, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 1;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cDown, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 2;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cLeft, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 3;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cRight, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 4;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cJump, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 5;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cThrowItem, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 6;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cInv, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 7;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cHeal, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 8;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cMana, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 9;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cBuff, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 10;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cHook, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 11;
				if (DrawRightSide(sb, Lang.menu[74 + num14].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cTorch, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 12;
				if (DrawRightSide(sb, Lang.menu[120].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cSmart, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				num14 = 13;
				if (DrawRightSide(sb, Lang.menu[130].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2), (Main.setKey == num14) ? Color.Gold : default(Color)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num14) ? "_" : Main.cMount, num13, num3, (Main.setKey == num14) ? Color.Gold : ((rightHover == num13) ? Color.White : default(Color))))
				{
					rightHover = num13;
					if (flag)
					{
						Main.setKey = num14;
					}
				}
				num13++;
				anchor2.X += 30f;
				if (DrawRightSide(sb, Lang.menu[86].Value, num13, anchor2, offset2, rightScale[num13], (rightScale[num13] - num2) / (num3 - num2)))
				{
					rightHover = num13;
					if (flag)
					{
						Main.ResetKeyBindings();
						Main.setKey = -1;
					}
				}
				num13++;
				if (Main.setKey >= 0)
				{
					Main.blockInput = true;
					Keys[] pressedKeys = Main.keyState.GetPressedKeys();
					if (pressedKeys.Length > 0)
					{
						string text = string.Concat(pressedKeys[0]);
						if (text != "None")
						{
							if (Main.setKey == 0)
							{
								Main.cUp = text;
							}
							if (Main.setKey == 1)
							{
								Main.cDown = text;
							}
							if (Main.setKey == 2)
							{
								Main.cLeft = text;
							}
							if (Main.setKey == 3)
							{
								Main.cRight = text;
							}
							if (Main.setKey == 4)
							{
								Main.cJump = text;
							}
							if (Main.setKey == 5)
							{
								Main.cThrowItem = text;
							}
							if (Main.setKey == 6)
							{
								Main.cInv = text;
							}
							if (Main.setKey == 7)
							{
								Main.cHeal = text;
							}
							if (Main.setKey == 8)
							{
								Main.cMana = text;
							}
							if (Main.setKey == 9)
							{
								Main.cBuff = text;
							}
							if (Main.setKey == 10)
							{
								Main.cHook = text;
							}
							if (Main.setKey == 11)
							{
								Main.cTorch = text;
							}
							if (Main.setKey == 12)
							{
								Main.cSmart = text;
							}
							if (Main.setKey == 13)
							{
								Main.cMount = text;
							}
							Main.blockKey = pressedKeys[0];
							Main.blockInput = false;
							Main.setKey = -1;
						}
					}
				}
			}
			if (category == 3)
			{
				int num15 = 0;
				int num16 = 0;
				anchor2.X -= 30f;
				num16 = 0;
				if (DrawRightSide(sb, Lang.menu[106 + num16].Value, num15, anchor2, offset2, rightScale[num15], (rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapStyle, num15, num3, (Main.setKey == num16) ? Color.Gold : ((rightHover == num15) ? Color.White : default(Color))))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				num16 = 1;
				if (DrawRightSide(sb, Lang.menu[106 + num16].Value, num15, anchor2, offset2, rightScale[num15], (rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapFull, num15, num3, (Main.setKey == num16) ? Color.Gold : ((rightHover == num15) ? Color.White : default(Color))))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				num16 = 2;
				if (DrawRightSide(sb, Lang.menu[106 + num16].Value, num15, anchor2, offset2, rightScale[num15], (rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapZoomIn, num15, num3, (Main.setKey == num16) ? Color.Gold : ((rightHover == num15) ? Color.White : default(Color))))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				num16 = 3;
				if (DrawRightSide(sb, Lang.menu[106 + num16].Value, num15, anchor2, offset2, rightScale[num15], (rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapZoomOut, num15, num3, (Main.setKey == num16) ? Color.Gold : ((rightHover == num15) ? Color.White : default(Color))))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				num16 = 4;
				if (DrawRightSide(sb, Lang.menu[106 + num16].Value, num15, anchor2, offset2, rightScale[num15], (rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapAlphaUp, num15, num3, (Main.setKey == num16) ? Color.Gold : ((rightHover == num15) ? Color.White : default(Color))))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				num16 = 5;
				if (DrawRightSide(sb, Lang.menu[106 + num16].Value, num15, anchor2, offset2, rightScale[num15], (rightScale[num15] - num2) / (num3 - num2), (Main.setKey == num16) ? Color.Gold : default(Color)))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				valuePosition.X += 10f;
				if (DrawValue(sb, (Main.setKey == num16) ? "_" : Main.cMapAlphaDown, num15, num3, (Main.setKey == num16) ? Color.Gold : ((rightHover == num15) ? Color.White : default(Color))))
				{
					rightHover = num15;
					if (flag)
					{
						Main.setKey = num16;
					}
				}
				num15++;
				anchor2.X += 30f;
				if (DrawRightSide(sb, Lang.menu[86].Value, num15, anchor2, offset2, rightScale[num15], (rightScale[num15] - num2) / (num3 - num2)))
				{
					rightHover = num15;
					if (flag)
					{
						Main.cMapStyle = "Tab";
						Main.cMapFull = "M";
						Main.cMapZoomIn = "Add";
						Main.cMapZoomOut = "Subtract";
						Main.cMapAlphaUp = "PageUp";
						Main.cMapAlphaDown = "PageDown";
						Main.setKey = -1;
					}
				}
				num15++;
				if (Main.setKey >= 0)
				{
					Main.blockInput = true;
					Keys[] pressedKeys2 = Main.keyState.GetPressedKeys();
					if (pressedKeys2.Length > 0)
					{
						string text2 = string.Concat(pressedKeys2[0]);
						if (text2 != "None")
						{
							if (Main.setKey == 0)
							{
								Main.cMapStyle = text2;
							}
							if (Main.setKey == 1)
							{
								Main.cMapFull = text2;
							}
							if (Main.setKey == 2)
							{
								Main.cMapZoomIn = text2;
							}
							if (Main.setKey == 3)
							{
								Main.cMapZoomOut = text2;
							}
							if (Main.setKey == 4)
							{
								Main.cMapAlphaUp = text2;
							}
							if (Main.setKey == 5)
							{
								Main.cMapAlphaDown = text2;
							}
							Main.setKey = -1;
							Main.blockKey = pressedKeys2[0];
							Main.blockInput = false;
						}
					}
				}
			}
			if (rightHover != -1 && rightLock == -1)
			{
				rightLock = rightHover;
			}
			Main.mouseText = false;
			Main.instance.GUIBarsDraw();
			Main.instance.DrawMouseOver();
			Vector2 value4 = Main.DrawThickCursor();
			sb.Draw(Main.cursorTextures[0], new Vector2(Main.mouseX, Main.mouseY) + value4 + Vector2.One, null, new Color((int)((float)(int)Main.cursorColor.R * 0.2f), (int)((float)(int)Main.cursorColor.G * 0.2f), (int)((float)(int)Main.cursorColor.B * 0.2f), (int)((float)(int)Main.cursorColor.A * 0.5f)), 0f, default(Vector2), Main.cursorScale * 1.1f, SpriteEffects.None, 0f);
			sb.Draw(Main.cursorTextures[0], new Vector2(Main.mouseX, Main.mouseY) + value4, null, Main.cursorColor, 0f, default(Vector2), Main.cursorScale, SpriteEffects.None, 0f);
		}

		public static void MouseOver()
		{
			if (Main.ingameOptionsWindow && _GUIHover.Contains(Main.MouseScreen.ToPoint()))
			{
				Main.mouseText = true;
			}
		}

		public static bool DrawLeftSide(SpriteBatch sb, string txt, int i, Vector2 anchor, Vector2 offset, float[] scales, float minscale = 0.7f, float maxscale = 0.8f, float scalespeed = 0.01f)
		{
			bool flag = i == category;
			Color color = Color.Lerp(Color.Gray, Color.White, (scales[i] - minscale) / (maxscale - minscale));
			if (flag)
			{
				color = Color.Gold;
			}
			Vector2 vector = Utils.DrawBorderStringBig(sb, txt, anchor + offset * (1 + i), color, scales[i], 0.5f, 0.5f);
			if (new Rectangle((int)anchor.X - (int)vector.X / 2, (int)anchor.Y + (int)(offset.Y * (float)(1 + i)) - (int)vector.Y / 2, (int)vector.X, (int)vector.Y).Contains(new Point(Main.mouseX, Main.mouseY)))
			{
				return true;
			}
			return false;
		}

		public static bool DrawRightSide(SpriteBatch sb, string txt, int i, Vector2 anchor, Vector2 offset, float scale, float colorScale, Color over = default(Color))
		{
			Color color = Color.Lerp(Color.Gray, Color.White, colorScale);
			if (over != default(Color))
			{
				color = over;
			}
			Vector2 value = Utils.DrawBorderString(sb, txt, anchor + offset * (1 + i), color, scale, 0.5f, 0.5f);
			valuePosition = anchor + offset * (1 + i) + value * new Vector2(0.5f, 0f);
			if (new Rectangle((int)anchor.X - (int)value.X / 2, (int)anchor.Y + (int)(offset.Y * (float)(1 + i)) - (int)value.Y / 2, (int)value.X, (int)value.Y).Contains(new Point(Main.mouseX, Main.mouseY)))
			{
				return true;
			}
			return false;
		}

		public static bool DrawValue(SpriteBatch sb, string txt, int i, float scale, Color over = default(Color))
		{
			Color color = Color.Gray;
			Vector2 vector = Main.fontMouseText.MeasureString(txt) * scale;
			bool flag = new Rectangle((int)valuePosition.X, (int)valuePosition.Y - (int)vector.Y / 2, (int)vector.X, (int)vector.Y).Contains(new Point(Main.mouseX, Main.mouseY));
			if (flag)
			{
				color = Color.White;
			}
			if (over != default(Color))
			{
				color = over;
			}
			Utils.DrawBorderString(sb, txt, valuePosition, color, scale, 0f, 0.5f);
			valuePosition.X += vector.X;
			if (flag)
			{
				return true;
			}
			return false;
		}

		public static float DrawValueBar(SpriteBatch sb, float scale, float perc)
		{
			Texture2D colorBarTexture = Main.colorBarTexture;
			Vector2 vector = new Vector2(colorBarTexture.Width, colorBarTexture.Height) * scale;
			valuePosition.X -= (int)vector.X;
			Rectangle destinationRectangle = new Rectangle((int)valuePosition.X, (int)valuePosition.Y - (int)vector.Y / 2, (int)vector.X, (int)vector.Y);
			sb.Draw(colorBarTexture, destinationRectangle, Color.White);
			int num = 167;
			float num2 = (float)destinationRectangle.X + 5f * scale;
			float num3 = (float)destinationRectangle.Y + 4f * scale;
			for (float num4 = 0f; num4 < (float)num; num4 += 1f)
			{
				float amount = num4 / (float)num;
				sb.Draw(Main.colorBlipTexture, new Vector2(num2 + num4 * scale, num3), null, Color.Lerp(Color.Black, Color.White, amount), 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			}
			sb.Draw(Main.colorSliderTexture, new Vector2(num2 + 167f * scale * perc, num3 + 4f * scale), null, Color.White, 0f, new Vector2(0.5f * (float)Main.colorSliderTexture.Width, 0.5f * (float)Main.colorSliderTexture.Height), scale, SpriteEffects.None, 0f);
			destinationRectangle.X = (int)num2;
			destinationRectangle.Y = (int)num3;
			bool flag = destinationRectangle.Contains(new Point(Main.mouseX, Main.mouseY));
			if (Main.mouseX >= destinationRectangle.X && Main.mouseX <= destinationRectangle.X + destinationRectangle.Width)
			{
				inBar = flag;
				return (float)(Main.mouseX - destinationRectangle.X) / (float)destinationRectangle.Width;
			}
			inBar = false;
			if (destinationRectangle.X >= Main.mouseX)
			{
				return 0f;
			}
			return 1f;
		}
	}
}
