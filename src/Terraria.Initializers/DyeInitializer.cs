using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Shaders;

namespace Terraria.Initializers
{
	internal static class DyeInitializer
	{
		private static void LoadBasicColorDye(int baseDyeItem, int blackDyeItem, int brightDyeItem, int silverDyeItem, float r, float g, float b, float saturation = 1f, int oldShader = 1)
		{
			Effect pixelShader = Main.pixelShader;
			GameShaders.Armor.BindShader(baseDyeItem, new ArmorShaderData(pixelShader, "ArmorColored")).UseColor(r, g, b).UseSaturation(saturation);
			GameShaders.Armor.BindShader(blackDyeItem, new ArmorShaderData(pixelShader, "ArmorColoredAndBlack")).UseColor(r, g, b).UseSaturation(saturation);
			GameShaders.Armor.BindShader(brightDyeItem, new ArmorShaderData(pixelShader, "ArmorColored")).UseColor(r * 0.5f + 0.5f, g * 0.5f + 0.5f, b * 0.5f + 0.5f).UseSaturation(saturation);
			GameShaders.Armor.BindShader(silverDyeItem, new ArmorShaderData(pixelShader, "ArmorColoredAndSilverTrim")).UseColor(r, g, b).UseSaturation(saturation);
		}

		private static void LoadBasicColorDye(int baseDyeItem, float r, float g, float b, float saturation = 1f, int oldShader = 1)
		{
			LoadBasicColorDye(baseDyeItem, baseDyeItem + 12, baseDyeItem + 31, baseDyeItem + 44, r, g, b, saturation, oldShader);
		}

		private static void LoadBasicColorDyes()
		{
			LoadBasicColorDye(1007, 1f, 0f, 0f, 1.2f);
			LoadBasicColorDye(1008, 1f, 0.5f, 0f, 1.2f, 2);
			LoadBasicColorDye(1009, 1f, 1f, 0f, 1.2f, 3);
			LoadBasicColorDye(1010, 0.5f, 1f, 0f, 1.2f, 4);
			LoadBasicColorDye(1011, 0f, 1f, 0f, 1.2f, 5);
			LoadBasicColorDye(1012, 0f, 1f, 0.5f, 1.2f, 6);
			LoadBasicColorDye(1013, 0f, 1f, 1f, 1.2f, 7);
			LoadBasicColorDye(1014, 0.2f, 0.5f, 1f, 1.2f, 8);
			LoadBasicColorDye(1015, 0f, 0f, 1f, 1.2f, 9);
			LoadBasicColorDye(1016, 0.5f, 0f, 1f, 1.2f, 10);
			LoadBasicColorDye(1017, 1f, 0f, 1f, 1.2f, 11);
			LoadBasicColorDye(1018, 1f, 0.1f, 0.5f, 1.3f, 12);
			LoadBasicColorDye(2874, 2875, 2876, 2877, 0.4f, 0.2f, 0f);
		}

		private static void LoadArmorDyes()
		{
			Effect pixelShader = Main.pixelShader;
			LoadBasicColorDyes();
			GameShaders.Armor.BindShader(1050, new ArmorShaderData(pixelShader, "ArmorBrightnessColored")).UseColor(0.6f, 0.6f, 0.6f);
			GameShaders.Armor.BindShader(1037, new ArmorShaderData(pixelShader, "ArmorBrightnessColored")).UseColor(1f, 1f, 1f);
			GameShaders.Armor.BindShader(3558, new ArmorShaderData(pixelShader, "ArmorBrightnessColored")).UseColor(1.5f, 1.5f, 1.5f);
			GameShaders.Armor.BindShader(2871, new ArmorShaderData(pixelShader, "ArmorBrightnessColored")).UseColor(0.05f, 0.05f, 0.05f);
			GameShaders.Armor.BindShader(3559, new ArmorShaderData(pixelShader, "ArmorColoredAndBlack")).UseColor(1f, 1f, 1f).UseSaturation(1.2f);
			GameShaders.Armor.BindShader(1031, new ArmorShaderData(pixelShader, "ArmorColoredGradient")).UseColor(1f, 0f, 0f).UseSecondaryColor(1f, 1f, 0f)
				.UseSaturation(1.2f);
			GameShaders.Armor.BindShader(1032, new ArmorShaderData(pixelShader, "ArmorColoredAndBlackGradient")).UseColor(1f, 0f, 0f).UseSecondaryColor(1f, 1f, 0f)
				.UseSaturation(1.5f);
			GameShaders.Armor.BindShader(3550, new ArmorShaderData(pixelShader, "ArmorColoredAndSilverTrimGradient")).UseColor(1f, 0f, 0f).UseSecondaryColor(1f, 1f, 0f)
				.UseSaturation(1.5f);
			GameShaders.Armor.BindShader(1063, new ArmorShaderData(pixelShader, "ArmorBrightnessGradient")).UseColor(1f, 0f, 0f).UseSecondaryColor(1f, 1f, 0f);
			GameShaders.Armor.BindShader(1035, new ArmorShaderData(pixelShader, "ArmorColoredGradient")).UseColor(0f, 0f, 1f).UseSecondaryColor(0f, 1f, 1f)
				.UseSaturation(1.2f);
			GameShaders.Armor.BindShader(1036, new ArmorShaderData(pixelShader, "ArmorColoredAndBlackGradient")).UseColor(0f, 0f, 1f).UseSecondaryColor(0f, 1f, 1f)
				.UseSaturation(1.5f);
			GameShaders.Armor.BindShader(3552, new ArmorShaderData(pixelShader, "ArmorColoredAndSilverTrimGradient")).UseColor(0f, 0f, 1f).UseSecondaryColor(0f, 1f, 1f)
				.UseSaturation(1.5f);
			GameShaders.Armor.BindShader(1065, new ArmorShaderData(pixelShader, "ArmorBrightnessGradient")).UseColor(0f, 0f, 1f).UseSecondaryColor(0f, 1f, 1f);
			GameShaders.Armor.BindShader(1033, new ArmorShaderData(pixelShader, "ArmorColoredGradient")).UseColor(0f, 1f, 0f).UseSecondaryColor(1f, 1f, 0f)
				.UseSaturation(1.2f);
			GameShaders.Armor.BindShader(1034, new ArmorShaderData(pixelShader, "ArmorColoredAndBlackGradient")).UseColor(0f, 1f, 0f).UseSecondaryColor(1f, 1f, 0f)
				.UseSaturation(1.5f);
			GameShaders.Armor.BindShader(3551, new ArmorShaderData(pixelShader, "ArmorColoredAndSilverTrimGradient")).UseColor(0f, 1f, 0f).UseSecondaryColor(1f, 1f, 0f)
				.UseSaturation(1.5f);
			GameShaders.Armor.BindShader(1064, new ArmorShaderData(pixelShader, "ArmorBrightnessGradient")).UseColor(0f, 1f, 0f).UseSecondaryColor(1f, 1f, 0f);
			GameShaders.Armor.BindShader(1068, new ArmorShaderData(pixelShader, "ArmorColoredGradient")).UseColor(0.5f, 1f, 0f).UseSecondaryColor(1f, 0.5f, 0f)
				.UseSaturation(1.5f);
			GameShaders.Armor.BindShader(1069, new ArmorShaderData(pixelShader, "ArmorColoredGradient")).UseColor(0f, 1f, 0.5f).UseSecondaryColor(0f, 0.5f, 1f)
				.UseSaturation(1.5f);
			GameShaders.Armor.BindShader(1070, new ArmorShaderData(pixelShader, "ArmorColoredGradient")).UseColor(1f, 0f, 0.5f).UseSecondaryColor(0.5f, 0f, 1f)
				.UseSaturation(1.5f);
			GameShaders.Armor.BindShader(1066, new ArmorShaderData(pixelShader, "ArmorColoredRainbow"));
			GameShaders.Armor.BindShader(1067, new ArmorShaderData(pixelShader, "ArmorBrightnessRainbow"));
			GameShaders.Armor.BindShader(3556, new ArmorShaderData(pixelShader, "ArmorMidnightRainbow"));
			GameShaders.Armor.BindShader(2869, new ArmorShaderData(pixelShader, "ArmorLivingFlame"));
			GameShaders.Armor.BindShader(2870, new ArmorShaderData(pixelShader, "ArmorLivingRainbow"));
			GameShaders.Armor.BindShader(2873, new ArmorShaderData(pixelShader, "ArmorLivingOcean"));
			GameShaders.Armor.BindShader(3026, new ReflectiveArmorShaderData(pixelShader, "ArmorReflectiveColor")).UseColor(1f, 1f, 1f);
			GameShaders.Armor.BindShader(3027, new ReflectiveArmorShaderData(pixelShader, "ArmorReflectiveColor")).UseColor(1.5f, 1.2f, 0.5f);
			GameShaders.Armor.BindShader(3553, new ReflectiveArmorShaderData(pixelShader, "ArmorReflectiveColor")).UseColor(1.35f, 0.7f, 0.4f);
			GameShaders.Armor.BindShader(3554, new ReflectiveArmorShaderData(pixelShader, "ArmorReflectiveColor")).UseColor(0.25f, 0f, 0.7f);
			GameShaders.Armor.BindShader(3555, new ReflectiveArmorShaderData(pixelShader, "ArmorReflectiveColor")).UseColor(0.4f, 0.4f, 0.4f);
			GameShaders.Armor.BindShader(3190, new ReflectiveArmorShaderData(pixelShader, "ArmorReflective"));
			GameShaders.Armor.BindShader(1969, new TeamArmorShaderData(pixelShader, "ArmorColored"));
			GameShaders.Armor.BindShader(2864, new ArmorShaderData(pixelShader, "ArmorMartian")).UseColor(0f, 2f, 3f);
			GameShaders.Armor.BindShader(2872, new ArmorShaderData(pixelShader, "ArmorInvert"));
			GameShaders.Armor.BindShader(2878, new ArmorShaderData(pixelShader, "ArmorWisp")).UseColor(0.7f, 1f, 0.9f).UseSecondaryColor(0.35f, 0.85f, 0.8f);
			GameShaders.Armor.BindShader(2879, new ArmorShaderData(pixelShader, "ArmorWisp")).UseColor(1f, 1.2f, 0f).UseSecondaryColor(1f, 0.6f, 0.3f);
			GameShaders.Armor.BindShader(2885, new ArmorShaderData(pixelShader, "ArmorWisp")).UseColor(1.2f, 0.8f, 0f).UseSecondaryColor(0.8f, 0.2f, 0f);
			GameShaders.Armor.BindShader(2884, new ArmorShaderData(pixelShader, "ArmorWisp")).UseColor(1f, 0f, 1f).UseSecondaryColor(1f, 0.3f, 0.6f);
			GameShaders.Armor.BindShader(2883, new ArmorShaderData(pixelShader, "ArmorHighContrastGlow")).UseColor(0f, 1f, 0f);
			GameShaders.Armor.BindShader(3025, new ArmorShaderData(pixelShader, "ArmorFlow")).UseColor(1f, 0.5f, 1f).UseSecondaryColor(0.6f, 0.1f, 1f);
			GameShaders.Armor.BindShader(3039, new ArmorShaderData(pixelShader, "ArmorTwilight")).UseImage("Images/Misc/noise").UseColor(0.5f, 0.1f, 1f);
			GameShaders.Armor.BindShader(3040, new ArmorShaderData(pixelShader, "ArmorAcid")).UseColor(0.5f, 1f, 0.3f);
			GameShaders.Armor.BindShader(3041, new ArmorShaderData(pixelShader, "ArmorMushroom")).UseColor(0.05f, 0.2f, 1f);
			GameShaders.Armor.BindShader(3042, new ArmorShaderData(pixelShader, "ArmorPhase")).UseImage("Images/Misc/noise").UseColor(0.4f, 0.2f, 1.5f);
			GameShaders.Armor.BindShader(3560, new ArmorShaderData(pixelShader, "ArmorAcid")).UseColor(0.9f, 0.2f, 0.2f);
			GameShaders.Armor.BindShader(3561, new ArmorShaderData(pixelShader, "ArmorGel")).UseImage("Images/Misc/noise").UseColor(0.4f, 0.7f, 1.4f)
				.UseSecondaryColor(0f, 0f, 0.1f);
			GameShaders.Armor.BindShader(3562, new ArmorShaderData(pixelShader, "ArmorGel")).UseImage("Images/Misc/noise").UseColor(1.4f, 0.75f, 1f)
				.UseSecondaryColor(0.45f, 0.1f, 0.3f);
			GameShaders.Armor.BindShader(3024, new ArmorShaderData(pixelShader, "ArmorGel")).UseImage("Images/Misc/noise").UseColor(-0.5f, -1f, 0f)
				.UseSecondaryColor(1.5f, 1f, 2.2f);
			GameShaders.Armor.BindShader(3534, new ArmorShaderData(pixelShader, "ArmorMirage"));
			GameShaders.Armor.BindShader(3028, new ArmorShaderData(pixelShader, "ArmorAcid")).UseColor(0.5f, 0.7f, 1.5f);
			GameShaders.Armor.BindShader(3557, new ArmorShaderData(pixelShader, "ArmorPolarized"));
			GameShaders.Armor.BindShader(3038, new ArmorShaderData(pixelShader, "ArmorHades")).UseColor(0.5f, 0.7f, 1.3f).UseSecondaryColor(0.5f, 0.7f, 1.3f);
			GameShaders.Armor.BindShader(3600, new ArmorShaderData(pixelShader, "ArmorHades")).UseColor(0.7f, 0.4f, 1.5f).UseSecondaryColor(0.7f, 0.4f, 1.5f);
			GameShaders.Armor.BindShader(3597, new ArmorShaderData(pixelShader, "ArmorHades")).UseColor(1.5f, 0.6f, 0.4f).UseSecondaryColor(1.5f, 0.6f, 0.4f);
			GameShaders.Armor.BindShader(3598, new ArmorShaderData(pixelShader, "ArmorHades")).UseColor(0.1f, 0.1f, 0.1f).UseSecondaryColor(0.4f, 0.05f, 0.025f);
			GameShaders.Armor.BindShader(3599, new ArmorShaderData(pixelShader, "ArmorLoki")).UseColor(0.1f, 0.1f, 0.1f);
			GameShaders.Armor.BindShader(3533, new ArmorShaderData(pixelShader, "ArmorShiftingSands")).UseImage("Images/Misc/noise").UseColor(1.1f, 1f, 0.5f)
				.UseSecondaryColor(0.7f, 0.5f, 0.3f);
			GameShaders.Armor.BindShader(3535, new ArmorShaderData(pixelShader, "ArmorShiftingPearlsands")).UseImage("Images/Misc/noise").UseColor(1.1f, 0.8f, 0.9f)
				.UseSecondaryColor(0.35f, 0.25f, 0.44f);
			GameShaders.Armor.BindShader(3526, new ArmorShaderData(pixelShader, "ArmorSolar")).UseColor(1f, 0f, 0f).UseSecondaryColor(1f, 1f, 0f);
			GameShaders.Armor.BindShader(3527, new ArmorShaderData(pixelShader, "ArmorNebula")).UseImage("Images/Misc/noise").UseColor(1f, 0f, 1f)
				.UseSecondaryColor(1f, 1f, 1f)
				.UseSaturation(1f);
			GameShaders.Armor.BindShader(3528, new ArmorShaderData(pixelShader, "ArmorVortex")).UseImage("Images/Misc/noise").UseColor(0.1f, 0.5f, 0.35f)
				.UseSecondaryColor(1f, 1f, 1f)
				.UseSaturation(1f);
			GameShaders.Armor.BindShader(3529, new ArmorShaderData(pixelShader, "ArmorStardust")).UseImage("Images/Misc/noise").UseColor(0.4f, 0.6f, 1f)
				.UseSecondaryColor(1f, 1f, 1f)
				.UseSaturation(1f);
			GameShaders.Armor.BindShader(3530, new ArmorShaderData(pixelShader, "ArmorVoid"));
			FixRecipes();
		}

		private static void LoadHairDyes()
		{
			Effect pixelShader = Main.pixelShader;
			LoadLegacyHairdyes();
			GameShaders.Hair.BindShader(3259, new HairShaderData(pixelShader, "ArmorTwilight")).UseImage("Images/Misc/noise").UseColor(0.5f, 0.1f, 1f);
		}

		private static void LoadLegacyHairdyes()
		{
			Effect pixelShader = Main.pixelShader;
			GameShaders.Hair.BindShader(1977, new LegacyHairShaderData().UseLegacyMethod(delegate(Player player, Color newColor, ref bool lighting)
			{
				newColor.R = (byte)((float)player.statLife / (float)player.statLifeMax2 * 235f + 20f);
				newColor.B = 20;
				newColor.G = 20;
				return newColor;
			}));
			GameShaders.Hair.BindShader(1978, new LegacyHairShaderData().UseLegacyMethod(delegate(Player player, Color newColor, ref bool lighting)
			{
				newColor.R = (byte)((1f - (float)player.statMana / (float)player.statManaMax2) * 200f + 50f);
				newColor.B = byte.MaxValue;
				newColor.G = (byte)((1f - (float)player.statMana / (float)player.statManaMax2) * 180f + 75f);
				return newColor;
			}));
			GameShaders.Hair.BindShader(1979, new LegacyHairShaderData().UseLegacyMethod(delegate(Player player, Color newColor, ref bool lighting)
			{
				float num29 = (float)(Main.worldSurface * 0.45) * 16f;
				float num30 = (float)(Main.worldSurface + Main.rockLayer) * 8f;
				float num31 = (float)(Main.rockLayer + (double)Main.maxTilesY) * 8f;
				float num32 = (float)(Main.maxTilesY - 150) * 16f;
				Vector2 center = player.Center;
				if (center.Y < num29)
				{
					float num33 = center.Y / num29;
					float num34 = 1f - num33;
					newColor.R = (byte)(116f * num34 + 28f * num33);
					newColor.G = (byte)(160f * num34 + 216f * num33);
					newColor.B = (byte)(249f * num34 + 94f * num33);
				}
				else if (center.Y < num30)
				{
					float num35 = num29;
					float num36 = (center.Y - num35) / (num30 - num35);
					float num37 = 1f - num36;
					newColor.R = (byte)(28f * num37 + 151f * num36);
					newColor.G = (byte)(216f * num37 + 107f * num36);
					newColor.B = (byte)(94f * num37 + 75f * num36);
				}
				else if (center.Y < num31)
				{
					float num38 = num30;
					float num39 = (center.Y - num38) / (num31 - num38);
					float num40 = 1f - num39;
					newColor.R = (byte)(151f * num40 + 128f * num39);
					newColor.G = (byte)(107f * num40 + 128f * num39);
					newColor.B = (byte)(75f * num40 + 128f * num39);
				}
				else if (center.Y < num32)
				{
					float num41 = num31;
					float num42 = (center.Y - num41) / (num32 - num41);
					float num43 = 1f - num42;
					newColor.R = (byte)(128f * num43 + 255f * num42);
					newColor.G = (byte)(128f * num43 + 50f * num42);
					newColor.B = (byte)(128f * num43 + 15f * num42);
				}
				else
				{
					newColor.R = byte.MaxValue;
					newColor.G = 50;
					newColor.B = 10;
				}
				return newColor;
			}));
			GameShaders.Hair.BindShader(1980, new LegacyHairShaderData().UseLegacyMethod(delegate(Player player, Color newColor, ref bool lighting)
			{
				int num17 = 0;
				for (int i = 0; i < 54; i++)
				{
					if (player.inventory[i].type == 71)
					{
						num17 += player.inventory[i].stack;
					}
					if (player.inventory[i].type == 72)
					{
						num17 += player.inventory[i].stack * 100;
					}
					if (player.inventory[i].type == 73)
					{
						num17 += player.inventory[i].stack * 10000;
					}
					if (player.inventory[i].type == 74)
					{
						num17 += player.inventory[i].stack * 1000000;
					}
				}
				float num18 = Item.buyPrice(0, 5);
				float num19 = Item.buyPrice(0, 50);
				float num20 = Item.buyPrice(2);
				Color color8 = new Color(226, 118, 76);
				Color color9 = new Color(174, 194, 196);
				Color color10 = new Color(204, 181, 72);
				Color color11 = new Color(161, 172, 173);
				if ((float)num17 < num18)
				{
					float num21 = (float)num17 / num18;
					float num22 = 1f - num21;
					newColor.R = (byte)((float)(int)color8.R * num22 + (float)(int)color9.R * num21);
					newColor.G = (byte)((float)(int)color8.G * num22 + (float)(int)color9.G * num21);
					newColor.B = (byte)((float)(int)color8.B * num22 + (float)(int)color9.B * num21);
				}
				else if ((float)num17 < num19)
				{
					float num23 = num18;
					float num24 = ((float)num17 - num23) / (num19 - num23);
					float num25 = 1f - num24;
					newColor.R = (byte)((float)(int)color9.R * num25 + (float)(int)color10.R * num24);
					newColor.G = (byte)((float)(int)color9.G * num25 + (float)(int)color10.G * num24);
					newColor.B = (byte)((float)(int)color9.B * num25 + (float)(int)color10.B * num24);
				}
				else if ((float)num17 < num20)
				{
					float num26 = num19;
					float num27 = ((float)num17 - num26) / (num20 - num26);
					float num28 = 1f - num27;
					newColor.R = (byte)((float)(int)color10.R * num28 + (float)(int)color11.R * num27);
					newColor.G = (byte)((float)(int)color10.G * num28 + (float)(int)color11.G * num27);
					newColor.B = (byte)((float)(int)color10.B * num28 + (float)(int)color11.B * num27);
				}
				else
				{
					newColor = color11;
				}
				return newColor;
			}));
			GameShaders.Hair.BindShader(1981, new LegacyHairShaderData().UseLegacyMethod(delegate(Player player, Color newColor, ref bool lighting)
			{
				Color color4 = new Color(1, 142, 255);
				Color color5 = new Color(255, 255, 0);
				Color color6 = new Color(211, 45, 127);
				Color color7 = new Color(67, 44, 118);
				if (Main.dayTime)
				{
					if (Main.time < 27000.0)
					{
						float num7 = (float)(Main.time / 27000.0);
						float num8 = 1f - num7;
						newColor.R = (byte)((float)(int)color4.R * num8 + (float)(int)color5.R * num7);
						newColor.G = (byte)((float)(int)color4.G * num8 + (float)(int)color5.G * num7);
						newColor.B = (byte)((float)(int)color4.B * num8 + (float)(int)color5.B * num7);
					}
					else
					{
						float num9 = 27000f;
						float num10 = (float)((Main.time - (double)num9) / (54000.0 - (double)num9));
						float num11 = 1f - num10;
						newColor.R = (byte)((float)(int)color5.R * num11 + (float)(int)color6.R * num10);
						newColor.G = (byte)((float)(int)color5.G * num11 + (float)(int)color6.G * num10);
						newColor.B = (byte)((float)(int)color5.B * num11 + (float)(int)color6.B * num10);
					}
				}
				else if (Main.time < 16200.0)
				{
					float num12 = (float)(Main.time / 16200.0);
					float num13 = 1f - num12;
					newColor.R = (byte)((float)(int)color6.R * num13 + (float)(int)color7.R * num12);
					newColor.G = (byte)((float)(int)color6.G * num13 + (float)(int)color7.G * num12);
					newColor.B = (byte)((float)(int)color6.B * num13 + (float)(int)color7.B * num12);
				}
				else
				{
					float num14 = 16200f;
					float num15 = (float)((Main.time - (double)num14) / (32400.0 - (double)num14));
					float num16 = 1f - num15;
					newColor.R = (byte)((float)(int)color7.R * num16 + (float)(int)color4.R * num15);
					newColor.G = (byte)((float)(int)color7.G * num16 + (float)(int)color4.G * num15);
					newColor.B = (byte)((float)(int)color7.B * num16 + (float)(int)color4.B * num15);
				}
				return newColor;
			}));
			GameShaders.Hair.BindShader(1982, new LegacyHairShaderData().UseLegacyMethod(delegate(Player player, Color newColor, ref bool lighting)
			{
				if (player.team >= 0 && player.team < Main.teamColor.Length)
				{
					newColor = Main.teamColor[player.team];
				}
				return newColor;
			}));
			GameShaders.Hair.BindShader(1983, new LegacyHairShaderData().UseLegacyMethod(delegate(Player player, Color newColor, ref bool lighting)
			{
				Color color2 = default(Color);
				color2 = ((Main.waterStyle == 2) ? new Color(124, 118, 242) : ((Main.waterStyle == 3) ? new Color(143, 215, 29) : ((Main.waterStyle == 4) ? new Color(78, 193, 227) : ((Main.waterStyle == 5) ? new Color(189, 231, 255) : ((Main.waterStyle == 6) ? new Color(230, 219, 100) : ((Main.waterStyle == 7) ? new Color(151, 107, 75) : ((Main.waterStyle == 8) ? new Color(128, 128, 128) : ((Main.waterStyle == 9) ? new Color(200, 0, 0) : ((Main.waterStyle == 10) ? new Color(208, 80, 80) : new Color(28, 216, 94))))))))));
				Color color3 = player.hairDyeColor;
				if (color3.A == 0)
				{
					color3 = color2;
				}
				if (color3.R > color2.R)
				{
					color3.R--;
				}
				if (color3.R < color2.R)
				{
					color3.R++;
				}
				if (color3.G > color2.G)
				{
					color3.G--;
				}
				if (color3.G < color2.G)
				{
					color3.G++;
				}
				if (color3.B > color2.B)
				{
					color3.B--;
				}
				if (color3.B < color2.B)
				{
					color3.B++;
				}
				newColor = color3;
				return newColor;
			}));
			GameShaders.Hair.BindShader(1984, new LegacyHairShaderData().UseLegacyMethod(delegate(Player player, Color newColor, ref bool lighting)
			{
				newColor = new Color(244, 22, 175);
				if (!Main.gameMenu && !Main.gamePaused)
				{
					if (Main.rand.Next(45) == 0)
					{
						int type = Main.rand.Next(139, 143);
						int num5 = Dust.NewDust(player.position, player.width, 8, type, 0f, 0f, 0, default(Color), 1.2f);
						Main.dust[num5].velocity.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.dust[num5].velocity.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.dust[num5].velocity.X += (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.dust[num5].velocity.Y += (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.dust[num5].velocity.Y -= 1f;
						Main.dust[num5].scale *= 0.7f + (float)Main.rand.Next(-30, 31) * 0.01f;
						Main.dust[num5].velocity += player.velocity * 0.2f;
					}
					if (Main.rand.Next(225) == 0)
					{
						int type2 = Main.rand.Next(276, 283);
						int num6 = Gore.NewGore(new Vector2(player.position.X + (float)Main.rand.Next(player.width), player.position.Y + (float)Main.rand.Next(8)), player.velocity, type2);
						Main.gore[num6].velocity.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.gore[num6].velocity.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.gore[num6].scale *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
						Main.gore[num6].velocity.X += (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.gore[num6].velocity.Y += (float)Main.rand.Next(-50, 51) * 0.01f;
						Main.gore[num6].velocity.Y -= 1f;
						Main.gore[num6].velocity += player.velocity * 0.2f;
					}
				}
				return newColor;
			}));
			GameShaders.Hair.BindShader(1985, new LegacyHairShaderData().UseLegacyMethod(delegate(Player player, Color newColor, ref bool lighting)
			{
				newColor = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
				return newColor;
			}));
			GameShaders.Hair.BindShader(1986, new LegacyHairShaderData().UseLegacyMethod(delegate(Player player, Color newColor, ref bool lighting)
			{
				float num = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
				float num2 = 10f;
				if (num > num2)
				{
					num = num2;
				}
				float num3 = num / num2;
				float num4 = 1f - num3;
				newColor.R = (byte)(75f * num3 + (float)(int)player.hairColor.R * num4);
				newColor.G = (byte)(255f * num3 + (float)(int)player.hairColor.G * num4);
				newColor.B = (byte)(200f * num3 + (float)(int)player.hairColor.B * num4);
				return newColor;
			}));
			GameShaders.Hair.BindShader(2863, new LegacyHairShaderData().UseLegacyMethod(delegate(Player player, Color newColor, ref bool lighting)
			{
				lighting = false;
				Color color = Lighting.GetColor((int)((double)player.position.X + (double)player.width * 0.5) / 16, (int)(((double)player.position.Y + (double)player.height * 0.25) / 16.0));
				newColor.R = (byte)(color.R + newColor.R >> 1);
				newColor.G = (byte)(color.G + newColor.G >> 1);
				newColor.B = (byte)(color.B + newColor.B >> 1);
				return newColor;
			}));
		}

		private static void LoadMisc()
		{
			Effect pixelShader = Main.pixelShader;
			GameShaders.Misc["ForceField"] = new MiscShaderData(pixelShader, "ForceField");
		}

		public static void Load()
		{
			LoadArmorDyes();
			LoadHairDyes();
			LoadMisc();
		}

		private static void FixRecipes()
		{
			for (int i = 0; i < Recipe.maxRecipes; i++)
			{
				Main.recipe[i].createItem.dye = (byte)GameShaders.Armor.GetShaderIdFromItemId(Main.recipe[i].createItem.type);
				Main.recipe[i].createItem.hairDye = GameShaders.Hair.GetShaderIdFromItemId(Main.recipe[i].createItem.type);
			}
		}
	}
}
