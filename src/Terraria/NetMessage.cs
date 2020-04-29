using Ionic.Zlib;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Net;
using System.Text;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.Social;

namespace Terraria
{
	public class NetMessage
	{
		public static MessageBuffer[] buffer = new MessageBuffer[18];

		public static void SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0, int number6 = 0, int number7 = 0)
		{
			if (Main.netMode != 0)
			{
				int num = 17;
				if (Main.netMode == 2 && remoteClient >= 0)
				{
					num = remoteClient;
				}
				lock (buffer[num])
				{
					BinaryWriter writer = buffer[num].writer;
					if (writer == null)
					{
						buffer[num].ResetWriter();
						writer = buffer[num].writer;
					}
					writer.BaseStream.Position = 0L;
					long position = writer.BaseStream.Position;
					writer.BaseStream.Position += 2L;
					writer.Write((byte)msgType);
					switch (msgType)
					{
					case 1:
						writer.Write("Terraria" + Main.curRelease);
						break;
					case 2:
						writer.Write(text);
						if (Main.dedServ)
						{
							Console.WriteLine(Netplay.Clients[num].Socket.GetRemoteAddress().ToString() + " was booted: " + text);
						}
						break;
					case 3:
						writer.Write((byte)remoteClient);
						break;
					case 4:
					{
						Player player4 = Main.player[number];
						writer.Write((byte)number);
						writer.Write((byte)player4.skinVariant);
						writer.Write((byte)player4.hair);
						writer.Write(text);
						writer.Write(player4.hairDye);
						BitsByte bb5 = (byte)0;
						for (int j = 0; j < 8; j++)
						{
							bb5[j] = player4.hideVisual[j];
						}
						writer.Write(bb5);
						bb5 = (byte)0;
						for (int k = 0; k < 2; k++)
						{
							bb5[k] = player4.hideVisual[k + 8];
						}
						writer.Write(bb5);
						writer.Write(player4.hideMisc);
						writer.WriteRGB(player4.hairColor);
						writer.WriteRGB(player4.skinColor);
						writer.WriteRGB(player4.eyeColor);
						writer.WriteRGB(player4.shirtColor);
						writer.WriteRGB(player4.underShirtColor);
						writer.WriteRGB(player4.pantsColor);
						writer.WriteRGB(player4.shoeColor);
						BitsByte bb6 = (byte)0;
						if (player4.difficulty == 1)
						{
							bb6[0] = true;
						}
						else if (player4.difficulty == 2)
						{
							bb6[1] = true;
						}
						bb6[2] = player4.extraAccessory;
						writer.Write(bb6);
						break;
					}
					case 5:
					{
						writer.Write((byte)number);
						writer.Write((byte)number2);
						Player player5 = Main.player[number];
						Item ıtem3 = null;
						int num6 = 0;
						int num7 = 0;
						ıtem3 = ((number2 > (float)(58 + player5.armor.Length + player5.dye.Length + player5.miscEquips.Length + player5.miscDyes.Length + player5.bank.item.Length + player5.bank2.item.Length)) ? player5.trashItem : ((number2 > (float)(58 + player5.armor.Length + player5.dye.Length + player5.miscEquips.Length + player5.miscDyes.Length + player5.bank.item.Length)) ? player5.bank2.item[(int)number2 - 58 - (player5.armor.Length + player5.dye.Length + player5.miscEquips.Length + player5.miscDyes.Length + player5.bank.item.Length) - 1] : ((number2 > (float)(58 + player5.armor.Length + player5.dye.Length + player5.miscEquips.Length + player5.miscDyes.Length)) ? player5.bank.item[(int)number2 - 58 - (player5.armor.Length + player5.dye.Length + player5.miscEquips.Length + player5.miscDyes.Length) - 1] : ((number2 > (float)(58 + player5.armor.Length + player5.dye.Length + player5.miscEquips.Length)) ? player5.miscDyes[(int)number2 - 58 - (player5.armor.Length + player5.dye.Length + player5.miscEquips.Length) - 1] : ((number2 > (float)(58 + player5.armor.Length + player5.dye.Length)) ? player5.miscEquips[(int)number2 - 58 - (player5.armor.Length + player5.dye.Length) - 1] : ((number2 > (float)(58 + player5.armor.Length)) ? player5.dye[(int)number2 - 58 - player5.armor.Length - 1] : ((!(number2 > 58f)) ? player5.inventory[(int)number2] : player5.armor[(int)number2 - 58 - 1])))))));
						if (ıtem3.Name == "" || ıtem3.stack == 0 || ıtem3.type == 0)
						{
							ıtem3.SetDefaults();
						}
						num6 = ıtem3.stack;
						num7 = ıtem3.netID;
						if (num6 < 0)
						{
							num6 = 0;
						}
						writer.Write((short)num6);
						writer.Write((byte)number3);
						writer.Write((short)num7);
						break;
					}
					case 7:
					{
						writer.Write((int)Main.time);
						BitsByte bb10 = (byte)0;
						bb10[0] = Main.dayTime;
						bb10[1] = Main.bloodMoon;
						bb10[2] = Main.eclipse;
						writer.Write(bb10);
						writer.Write((byte)Main.moonPhase);
						writer.Write((short)Main.maxTilesX);
						writer.Write((short)Main.maxTilesY);
						writer.Write((short)Main.spawnTileX);
						writer.Write((short)Main.spawnTileY);
						writer.Write((short)Main.worldSurface);
						writer.Write((short)Main.rockLayer);
						writer.Write(Main.worldID);
						writer.Write(Main.worldName);
						writer.Write((byte)Main.moonType);
						writer.Write((byte)WorldGen.treeBG);
						writer.Write((byte)WorldGen.corruptBG);
						writer.Write((byte)WorldGen.jungleBG);
						writer.Write((byte)WorldGen.snowBG);
						writer.Write((byte)WorldGen.hallowBG);
						writer.Write((byte)WorldGen.crimsonBG);
						writer.Write((byte)WorldGen.desertBG);
						writer.Write((byte)WorldGen.oceanBG);
						writer.Write((byte)Main.iceBackStyle);
						writer.Write((byte)Main.jungleBackStyle);
						writer.Write((byte)Main.hellBackStyle);
						writer.Write(Main.windSpeedSet);
						writer.Write((byte)Main.numClouds);
						for (int num13 = 0; num13 < 3; num13++)
						{
							writer.Write(Main.treeX[num13]);
						}
						for (int num14 = 0; num14 < 4; num14++)
						{
							writer.Write((byte)Main.treeStyle[num14]);
						}
						for (int num15 = 0; num15 < 3; num15++)
						{
							writer.Write(Main.caveBackX[num15]);
						}
						for (int num16 = 0; num16 < 4; num16++)
						{
							writer.Write((byte)Main.caveBackStyle[num16]);
						}
						if (!Main.raining)
						{
							Main.maxRaining = 0f;
						}
						writer.Write(Main.maxRaining);
						BitsByte bb11 = (byte)0;
						bb11[0] = WorldGen.shadowOrbSmashed;
						bb11[1] = NPC.downedBoss1;
						bb11[2] = NPC.downedBoss2;
						bb11[3] = NPC.downedBoss3;
						bb11[4] = Main.hardMode;
						bb11[5] = NPC.downedClown;
						bb11[7] = NPC.downedPlantBoss;
						writer.Write(bb11);
						BitsByte bb12 = (byte)0;
						bb12[0] = NPC.downedMechBoss1;
						bb12[1] = NPC.downedMechBoss2;
						bb12[2] = NPC.downedMechBoss3;
						bb12[3] = NPC.downedMechBossAny;
						bb12[4] = (Main.cloudBGActive >= 1f);
						bb12[5] = WorldGen.crimson;
						bb12[6] = Main.pumpkinMoon;
						bb12[7] = Main.snowMoon;
						writer.Write(bb12);
						BitsByte bb13 = (byte)0;
						bb13[0] = Main.expertMode;
						bb13[1] = Main.fastForwardTime;
						bb13[2] = Main.slimeRain;
						bb13[3] = NPC.downedSlimeKing;
						bb13[4] = NPC.downedQueenBee;
						bb13[5] = NPC.downedFishron;
						bb13[6] = NPC.downedMartians;
						bb13[7] = NPC.downedAncientCultist;
						writer.Write(bb13);
						BitsByte bb14 = (byte)0;
						bb14[0] = NPC.downedMoonlord;
						bb14[1] = NPC.downedHalloweenKing;
						bb14[2] = NPC.downedHalloweenTree;
						bb14[3] = NPC.downedChristmasIceQueen;
						bb14[4] = NPC.downedChristmasSantank;
						bb14[5] = NPC.downedChristmasTree;
						bb14[6] = NPC.downedGolemBoss;
						writer.Write(bb14);
						writer.Write((sbyte)Main.invasionType);
						if (SocialAPI.Network != null)
						{
							writer.Write(SocialAPI.Network.GetLobbyId());
						}
						else
						{
							writer.Write(0uL);
						}
						break;
					}
					case 8:
						writer.Write(number);
						writer.Write((int)number2);
						break;
					case 9:
						writer.Write(number);
						writer.Write(text);
						break;
					case 10:
					{
						int num8 = CompressTileBlock(number, (int)number2, (short)number3, (short)number4, buffer[num].writeBuffer, (int)writer.BaseStream.Position);
						writer.BaseStream.Position += num8;
						break;
					}
					case 11:
						writer.Write((short)number);
						writer.Write((short)number2);
						writer.Write((short)number3);
						writer.Write((short)number4);
						break;
					case 12:
						writer.Write((byte)number);
						writer.Write((short)Main.player[number].SpawnX);
						writer.Write((short)Main.player[number].SpawnY);
						break;
					case 13:
					{
						Player player = Main.player[number];
						writer.Write((byte)number);
						BitsByte bb = (byte)0;
						bb[0] = player.controlUp;
						bb[1] = player.controlDown;
						bb[2] = player.controlLeft;
						bb[3] = player.controlRight;
						bb[4] = player.controlJump;
						bb[5] = player.controlUseItem;
						bb[6] = (player.direction == 1);
						writer.Write(bb);
						BitsByte bb2 = (byte)0;
						bb2[0] = player.pulley;
						bb2[1] = (player.pulley && player.pulleyDir == 2);
						bb2[2] = (player.velocity != Vector2.Zero);
						bb2[3] = player.vortexStealthActive;
						bb2[4] = (player.gravDir == 1f);
						writer.Write(bb2);
						writer.Write((byte)player.selectedItem);
						writer.WriteVector2(player.position);
						if (bb2[2])
						{
							writer.WriteVector2(player.velocity);
						}
						break;
					}
					case 14:
						writer.Write((byte)number);
						writer.Write((byte)number2);
						break;
					case 16:
						writer.Write((byte)number);
						writer.Write((short)Main.player[number].statLife);
						writer.Write((short)Main.player[number].statLifeMax);
						break;
					case 17:
						writer.Write((byte)number);
						writer.Write((short)number2);
						writer.Write((short)number3);
						writer.Write((short)number4);
						writer.Write((byte)number5);
						break;
					case 18:
						writer.Write((byte)(Main.dayTime ? 1 : 0));
						writer.Write((int)Main.time);
						writer.Write(Main.sunModY);
						writer.Write(Main.moonModY);
						break;
					case 19:
						writer.Write((byte)number);
						writer.Write((short)number2);
						writer.Write((short)number3);
						writer.Write((byte)((number4 == 1f) ? 1 : 0));
						break;
					case 20:
					{
						int num10 = (int)number2;
						int num11 = (int)number3;
						if (num10 < number)
						{
							num10 = number;
						}
						if (num10 >= Main.maxTilesX + number)
						{
							num10 = Main.maxTilesX - number - 1;
						}
						if (num11 < number)
						{
							num11 = number;
						}
						if (num11 >= Main.maxTilesY + number)
						{
							num11 = Main.maxTilesY - number - 1;
						}
						writer.Write((short)number);
						writer.Write((short)num10);
						writer.Write((short)num11);
						for (int n = num10; n < num10 + number; n++)
						{
							for (int num12 = num11; num12 < num11 + number; num12++)
							{
								BitsByte bb8 = (byte)0;
								BitsByte bb9 = (byte)0;
								byte b2 = 0;
								byte b3 = 0;
								Tile tile2 = Main.tile[n, num12];
								bb8[0] = tile2.active();
								bb8[2] = (tile2.wall > 0);
								bb8[3] = (tile2.liquid > 0 && Main.netMode == 2);
								bb8[4] = tile2.wire();
								bb8[5] = tile2.halfBrick();
								bb8[6] = tile2.actuator();
								bb8[7] = tile2.inActive();
								bb9[0] = tile2.wire2();
								bb9[1] = tile2.wire3();
								if (tile2.active() && tile2.color() > 0)
								{
									bb9[2] = true;
									b2 = tile2.color();
								}
								if (tile2.wall > 0 && tile2.wallColor() > 0)
								{
									bb9[3] = true;
									b3 = tile2.wallColor();
								}
								bb9 = (byte)((byte)bb9 + (byte)(tile2.slope() << 4));
								writer.Write(bb8);
								writer.Write(bb9);
								if (b2 > 0)
								{
									writer.Write(b2);
								}
								if (b3 > 0)
								{
									writer.Write(b3);
								}
								if (tile2.active())
								{
									writer.Write(tile2.type);
									if (Main.tileFrameImportant[tile2.type])
									{
										writer.Write(tile2.frameX);
										writer.Write(tile2.frameY);
									}
								}
								if (tile2.wall > 0)
								{
									writer.Write(tile2.wall);
								}
								if (tile2.liquid > 0 && Main.netMode == 2)
								{
									writer.Write(tile2.liquid);
									writer.Write(tile2.liquidType());
								}
							}
						}
						break;
					}
					case 21:
					case 90:
					{
						Item ıtem5 = Main.item[number];
						writer.Write((short)number);
						writer.WriteVector2(ıtem5.position);
						writer.WriteVector2(ıtem5.velocity);
						writer.Write((short)ıtem5.stack);
						writer.Write(ıtem5.prefix);
						writer.Write((byte)number2);
						short value4 = 0;
						if (ıtem5.active && ıtem5.stack > 0)
						{
							value4 = (short)ıtem5.netID;
						}
						writer.Write(value4);
						break;
					}
					case 22:
						writer.Write((short)number);
						writer.Write((byte)Main.item[number].owner);
						break;
					case 23:
					{
						NPC nPC2 = Main.npc[number];
						writer.Write((short)number);
						writer.WriteVector2(nPC2.position);
						writer.WriteVector2(nPC2.velocity);
						writer.Write((byte)nPC2.target);
						int num18 = nPC2.life;
						if (!nPC2.active)
						{
							num18 = 0;
						}
						if (!nPC2.active || nPC2.life <= 0)
						{
							nPC2.netSkip = 0;
						}
						if (nPC2.name == null)
						{
							nPC2.name = "";
						}
						short value7 = (short)nPC2.netID;
						bool[] array = new bool[4];
						BitsByte bb16 = (byte)0;
						bb16[0] = (nPC2.direction > 0);
						bb16[1] = (nPC2.directionY > 0);
						bb16[2] = (array[0] = (nPC2.ai[0] != 0f));
						bb16[3] = (array[1] = (nPC2.ai[1] != 0f));
						bb16[4] = (array[2] = (nPC2.ai[2] != 0f));
						bb16[5] = (array[3] = (nPC2.ai[3] != 0f));
						bb16[6] = (nPC2.spriteDirection > 0);
						bb16[7] = (num18 == nPC2.lifeMax);
						writer.Write(bb16);
						for (int num19 = 0; num19 < NPC.maxAI; num19++)
						{
							if (array[num19])
							{
								writer.Write(nPC2.ai[num19]);
							}
						}
						writer.Write(value7);
						if (!bb16[7])
						{
							byte b4 = Main.npcLifeBytes[nPC2.netID];
							writer.Write(b4);
							switch (b4)
							{
							case 2:
								writer.Write((short)num18);
								break;
							case 4:
								writer.Write(num18);
								break;
							default:
								writer.Write((sbyte)num18);
								break;
							}
						}
						if (Main.npcCatchable[nPC2.type])
						{
							writer.Write((byte)nPC2.releaseOwner);
						}
						break;
					}
					case 24:
						writer.Write((short)number);
						writer.Write((byte)number2);
						break;
					case 25:
						writer.Write((byte)number);
						writer.Write((byte)number2);
						writer.Write((byte)number3);
						writer.Write((byte)number4);
						writer.Write(text);
						break;
					case 26:
					{
						writer.Write((byte)number);
						writer.Write((byte)(number2 + 1f));
						writer.Write((short)number3);
						writer.Write(text);
						BitsByte bb17 = (byte)0;
						bb17[0] = (number4 == 1f);
						bb17[1] = (number5 == 1);
						bb17[2] = (number6 == 0);
						bb17[3] = (number6 == 1);
						writer.Write(bb17);
						break;
					}
					case 27:
					{
						Projectile projectile = Main.projectile[number];
						writer.Write((short)projectile.identity);
						writer.WriteVector2(projectile.position);
						writer.WriteVector2(projectile.velocity);
						writer.Write(projectile.knockBack);
						writer.Write((short)projectile.damage);
						writer.Write((byte)projectile.owner);
						writer.Write((short)projectile.type);
						BitsByte bb7 = (byte)0;
						for (int l = 0; l < Projectile.maxAI; l++)
						{
							if (projectile.ai[l] != 0f)
							{
								bb7[l] = true;
							}
						}
						if (projectile.type > 0 && projectile.type < 651 && ProjectileID.Sets.NeedsUUID[projectile.type])
						{
							bb7[Projectile.maxAI] = true;
						}
						writer.Write(bb7);
						for (int m = 0; m < Projectile.maxAI; m++)
						{
							if (bb7[m])
							{
								writer.Write(projectile.ai[m]);
							}
						}
						if (bb7[Projectile.maxAI])
						{
							writer.Write((short)projectile.projUUID);
						}
						break;
					}
					case 28:
						writer.Write((short)number);
						writer.Write((short)number2);
						writer.Write(number3);
						writer.Write((byte)(number4 + 1f));
						writer.Write((byte)number5);
						break;
					case 29:
						writer.Write((short)number);
						writer.Write((byte)number2);
						break;
					case 30:
						writer.Write((byte)number);
						writer.Write(Main.player[number].hostile);
						break;
					case 31:
						writer.Write((short)number);
						writer.Write((short)number2);
						break;
					case 32:
					{
						Item ıtem4 = Main.chest[number].item[(byte)number2];
						writer.Write((short)number);
						writer.Write((byte)number2);
						short value = (short)ıtem4.netID;
						writer.Write((short)ıtem4.stack);
						writer.Write(ıtem4.prefix);
						writer.Write(value);
						break;
					}
					case 33:
					{
						int num3 = 0;
						int num4 = 0;
						int num5 = 0;
						string text2 = null;
						if (number > -1)
						{
							num3 = Main.chest[number].x;
							num4 = Main.chest[number].y;
						}
						if (number2 == 1f)
						{
							num5 = (byte)text.Length;
							if (num5 == 0 || num5 > 20)
							{
								num5 = 255;
							}
							else
							{
								text2 = text;
							}
						}
						writer.Write((short)number);
						writer.Write((short)num3);
						writer.Write((short)num4);
						writer.Write((byte)num5);
						if (text2 != null)
						{
							writer.Write(text2);
						}
						break;
					}
					case 34:
						writer.Write((byte)number);
						writer.Write((short)number2);
						writer.Write((short)number3);
						writer.Write((short)number4);
						if (Main.netMode == 2)
						{
							Netplay.GetSectionX((int)number2);
							Netplay.GetSectionY((int)number3);
							writer.Write((short)number5);
						}
						break;
					case 35:
					case 66:
						writer.Write((byte)number);
						writer.Write((short)number2);
						break;
					case 36:
					{
						Player player3 = Main.player[number];
						writer.Write((byte)number);
						writer.Write(player3.zone1);
						writer.Write(player3.zone2);
						break;
					}
					case 38:
						writer.Write(text);
						break;
					case 39:
						writer.Write((short)number);
						break;
					case 40:
						writer.Write((byte)number);
						writer.Write((short)Main.player[number].talkNPC);
						break;
					case 41:
						writer.Write((byte)number);
						writer.Write(Main.player[number].itemRotation);
						writer.Write((short)Main.player[number].itemAnimation);
						break;
					case 42:
						writer.Write((byte)number);
						writer.Write((short)Main.player[number].statMana);
						writer.Write((short)Main.player[number].statManaMax);
						break;
					case 43:
						writer.Write((byte)number);
						writer.Write((short)number2);
						break;
					case 44:
						writer.Write((byte)number);
						writer.Write((byte)(number2 + 1f));
						writer.Write((short)number3);
						writer.Write((byte)number4);
						writer.Write(text);
						break;
					case 45:
						writer.Write((byte)number);
						writer.Write((byte)Main.player[number].team);
						break;
					case 46:
						writer.Write((short)number);
						writer.Write((short)number2);
						break;
					case 47:
						writer.Write((short)number);
						writer.Write((short)Main.sign[number].x);
						writer.Write((short)Main.sign[number].y);
						writer.Write(Main.sign[number].text);
						writer.Write((byte)number2);
						break;
					case 48:
					{
						Tile tile = Main.tile[number, (int)number2];
						writer.Write((short)number);
						writer.Write((short)number2);
						writer.Write(tile.liquid);
						writer.Write(tile.liquidType());
						break;
					}
					case 50:
					{
						writer.Write((byte)number);
						for (int num21 = 0; num21 < 22; num21++)
						{
							writer.Write((byte)Main.player[number].buffType[num21]);
						}
						break;
					}
					case 51:
						writer.Write((byte)number);
						writer.Write((byte)number2);
						break;
					case 52:
						writer.Write((byte)number2);
						writer.Write((short)number3);
						writer.Write((short)number4);
						break;
					case 53:
						writer.Write((short)number);
						writer.Write((byte)number2);
						writer.Write((short)number3);
						break;
					case 54:
					{
						writer.Write((short)number);
						for (int num20 = 0; num20 < 5; num20++)
						{
							writer.Write((byte)Main.npc[number].buffType[num20]);
							writer.Write((short)Main.npc[number].buffTime[num20]);
						}
						break;
					}
					case 55:
						writer.Write((byte)number);
						writer.Write((byte)number2);
						writer.Write((short)number3);
						break;
					case 56:
					{
						string value6 = null;
						if (Main.netMode == 2)
						{
							value6 = Main.npc[number].GivenName;
						}
						else if (Main.netMode == 1)
						{
							value6 = text;
						}
						writer.Write((short)number);
						writer.Write(value6);
						break;
					}
					case 57:
						writer.Write(WorldGen.tGood);
						writer.Write(WorldGen.tEvil);
						writer.Write(WorldGen.tBlood);
						break;
					case 58:
						writer.Write((byte)number);
						writer.Write(number2);
						break;
					case 59:
						writer.Write((short)number);
						writer.Write((short)number2);
						break;
					case 60:
						writer.Write((short)number);
						writer.Write((short)number2);
						writer.Write((short)number3);
						writer.Write((byte)number4);
						break;
					case 61:
						writer.Write((short)number);
						writer.Write((short)number2);
						break;
					case 62:
						writer.Write((byte)number);
						writer.Write((byte)number2);
						break;
					case 63:
					case 64:
						writer.Write((short)number);
						writer.Write((short)number2);
						writer.Write((byte)number3);
						break;
					case 65:
					{
						BitsByte bb15 = (byte)0;
						bb15[0] = ((number & 1) == 1);
						bb15[1] = ((number & 2) == 2);
						bb15[2] = ((number5 & 1) == 1);
						bb15[3] = ((number5 & 2) == 2);
						writer.Write(bb15);
						writer.Write((short)number2);
						writer.Write(number3);
						writer.Write(number4);
						break;
					}
					case 68:
						writer.Write(Main.clientUUID);
						break;
					case 69:
						Netplay.GetSectionX((int)number2);
						Netplay.GetSectionY((int)number3);
						writer.Write((short)number);
						writer.Write((short)number2);
						writer.Write((short)number3);
						writer.Write(text);
						break;
					case 70:
						writer.Write((short)number);
						writer.Write((byte)number2);
						break;
					case 71:
						writer.Write(number);
						writer.Write((int)number2);
						writer.Write((short)number3);
						writer.Write((byte)number4);
						break;
					case 72:
					{
						for (int num17 = 0; num17 < 40; num17++)
						{
							writer.Write((short)Main.travelShop[num17]);
						}
						break;
					}
					case 74:
					{
						writer.Write((byte)Main.anglerQuest);
						bool value5 = Main.anglerWhoFinishedToday.Contains(text);
						writer.Write(value5);
						break;
					}
					case 76:
						writer.Write((byte)number);
						writer.Write(Main.player[number].anglerQuestsFinished);
						break;
					case 77:
						if (Main.netMode != 2)
						{
							return;
						}
						writer.Write((short)number);
						writer.Write((ushort)number2);
						writer.Write((short)number3);
						writer.Write((short)number4);
						break;
					case 78:
						writer.Write(number);
						writer.Write((int)number2);
						writer.Write((sbyte)number3);
						writer.Write((sbyte)number4);
						break;
					case 79:
						writer.Write((short)number);
						writer.Write((short)number2);
						writer.Write((short)number3);
						writer.Write((short)number4);
						writer.Write((byte)number5);
						writer.Write((sbyte)number6);
						writer.Write(number7 == 1);
						break;
					case 80:
						writer.Write((byte)number);
						writer.Write((short)number2);
						break;
					case 81:
					{
						writer.Write(number2);
						writer.Write(number3);
						Color c = default(Color);
						c.PackedValue = (uint)number;
						writer.WriteRGB(c);
						writer.Write(text);
						break;
					}
					case 83:
					{
						int num9 = number;
						if (num9 < 0 && num9 >= 251)
						{
							num9 = 1;
						}
						int value3 = NPC.killCount[num9];
						writer.Write((short)num9);
						writer.Write(value3);
						break;
					}
					case 84:
					{
						byte b = (byte)number;
						float stealth = Main.player[b].stealth;
						writer.Write(b);
						writer.Write(stealth);
						break;
					}
					case 85:
					{
						byte value2 = (byte)number;
						writer.Write(value2);
						break;
					}
					case 86:
					{
						writer.Write(number);
						bool flag = TileEntity.ByID.ContainsKey(number);
						writer.Write(flag);
						if (flag)
						{
							TileEntity.Write(writer, TileEntity.ByID[number]);
						}
						break;
					}
					case 87:
						writer.Write((short)number);
						writer.Write((short)number2);
						writer.Write((byte)number3);
						break;
					case 88:
					{
						BitsByte bb3 = (byte)number2;
						BitsByte bb4 = (byte)number3;
						writer.Write((short)number);
						writer.Write(bb3);
						Item ıtem2 = Main.item[number];
						if (bb3[0])
						{
							writer.Write(ıtem2.color.PackedValue);
						}
						if (bb3[1])
						{
							writer.Write((ushort)ıtem2.damage);
						}
						if (bb3[2])
						{
							writer.Write(ıtem2.knockBack);
						}
						if (bb3[3])
						{
							writer.Write((ushort)ıtem2.useAnimation);
						}
						if (bb3[4])
						{
							writer.Write((ushort)ıtem2.useTime);
						}
						if (bb3[5])
						{
							writer.Write((short)ıtem2.shoot);
						}
						if (bb3[6])
						{
							writer.Write(ıtem2.shootSpeed);
						}
						if (bb3[7])
						{
							writer.Write(bb4);
							if (bb4[0])
							{
								writer.Write((ushort)ıtem2.width);
							}
							if (bb4[1])
							{
								writer.Write((ushort)ıtem2.height);
							}
							if (bb4[2])
							{
								writer.Write(ıtem2.scale);
							}
							if (bb4[3])
							{
								writer.Write((short)ıtem2.ammo);
							}
							if (bb4[4])
							{
								writer.Write((short)ıtem2.useAmmo);
							}
							if (bb4[5])
							{
								writer.Write(ıtem2.notAmmo);
							}
						}
						break;
					}
					case 89:
					{
						writer.Write((short)number);
						writer.Write((short)number2);
						Item ıtem = Main.player[(int)number4].inventory[(int)number3];
						writer.Write((short)ıtem.netID);
						writer.Write(ıtem.prefix);
						writer.Write(ıtem.stack);
						break;
					}
					case 91:
						writer.Write(number);
						writer.Write((byte)number2);
						if (number2 != 255f)
						{
							writer.Write((ushort)number3);
							writer.Write((byte)number4);
							writer.Write((byte)number5);
							if (number5 < 0)
							{
								writer.Write((short)number6);
							}
						}
						break;
					case 92:
						writer.Write((short)number);
						writer.Write(number2);
						writer.Write(number3);
						writer.Write(number4);
						break;
					case 95:
						writer.Write((ushort)number);
						break;
					case 96:
					{
						writer.Write((byte)number);
						Player player2 = Main.player[number];
						writer.Write((short)number4);
						writer.Write(number2);
						writer.Write(number3);
						writer.WriteVector2(player2.velocity);
						break;
					}
					case 97:
						writer.Write((short)number);
						break;
					case 98:
						writer.Write((short)number);
						break;
					case 99:
						writer.Write((byte)number);
						writer.WriteVector2(Main.player[number].MinionTargetPoint);
						break;
					case 100:
					{
						writer.Write((ushort)number);
						NPC nPC = Main.npc[number];
						writer.Write((short)number4);
						writer.Write(number2);
						writer.Write(number3);
						writer.WriteVector2(nPC.velocity);
						break;
					}
					case 101:
						writer.Write((ushort)NPC.ShieldStrengthTowerSolar);
						writer.Write((ushort)NPC.ShieldStrengthTowerVortex);
						writer.Write((ushort)NPC.ShieldStrengthTowerNebula);
						writer.Write((ushort)NPC.ShieldStrengthTowerStardust);
						break;
					case 102:
						writer.Write((byte)number);
						writer.Write((byte)number2);
						writer.Write(number3);
						writer.Write(number4);
						break;
					case 103:
						writer.Write(NPC.MoonLordCountdown);
						break;
					case 104:
						writer.Write((byte)number);
						writer.Write((short)number2);
						writer.Write(((short)number3 < 0) ? 0f : number3);
						writer.Write((byte)number4);
						writer.Write(number5);
						writer.Write((byte)number6);
						break;
					case 105:
						if (Main.netMode == 2)
						{
							int num2 = 0;
							for (int i = 0; i < 16; i++)
							{
								if (Main.player[i].active)
								{
									num2++;
								}
							}
							writer.Write(Netplay.ListenPort);
							writer.Write(Main.worldName);
							writer.Write(Dns.GetHostName());
							writer.Write((ushort)Main.maxTilesX);
							writer.Write(Main.ActiveWorldFileData.HasCrimson);
							writer.Write(Main.ActiveWorldFileData.IsExpertMode);
							writer.Write((byte)Main.maxNetPlayers);
							writer.Write((byte)num2);
						}
						break;
					}
					int num22 = (int)writer.BaseStream.Position;
					writer.BaseStream.Position = position;
					writer.Write((short)num22);
					writer.BaseStream.Position = num22;
					if (Main.netMode == 1)
					{
						if (Netplay.Connection.Socket.IsConnected())
						{
							try
							{
								buffer[num].spamCount++;
								Main.txMsg++;
								Main.txData += num22;
								Main.txMsgType[msgType]++;
								Main.txDataType[msgType] += num22;
								Netplay.Connection.Socket.AsyncSend(buffer[num].writeBuffer, 0, num22, Netplay.Connection.ClientWriteCallBack);
							}
							catch
							{
							}
						}
					}
					else if (remoteClient == -1)
					{
						switch (msgType)
						{
						case 34:
						case 69:
						{
							for (int num24 = 0; num24 < 17; num24++)
							{
								if (num24 != ignoreClient && buffer[num24].broadcast && Netplay.Clients[num24].IsConnected())
								{
									try
									{
										buffer[num24].spamCount++;
										Main.txMsg++;
										Main.txData += num22;
										Main.txMsgType[msgType]++;
										Main.txDataType[msgType] += num22;
										Netplay.Clients[num24].Socket.AsyncSend(buffer[num].writeBuffer, 0, num22, Netplay.Clients[num24].ServerWriteCallBack);
									}
									catch
									{
									}
								}
							}
							break;
						}
						case 20:
						{
							for (int num28 = 0; num28 < 17; num28++)
							{
								if (num28 != ignoreClient && buffer[num28].broadcast && Netplay.Clients[num28].IsConnected() && Netplay.Clients[num28].SectionRange(number, (int)number2, (int)number3))
								{
									try
									{
										buffer[num28].spamCount++;
										Main.txMsg++;
										Main.txData += num22;
										Main.txMsgType[msgType]++;
										Main.txDataType[msgType] += num22;
										Netplay.Clients[num28].Socket.AsyncSend(buffer[num].writeBuffer, 0, num22, Netplay.Clients[num28].ServerWriteCallBack);
									}
									catch
									{
									}
								}
							}
							break;
						}
						case 23:
						{
							NPC nPC4 = Main.npc[number];
							for (int num29 = 0; num29 < 17; num29++)
							{
								if (num29 != ignoreClient && buffer[num29].broadcast && Netplay.Clients[num29].IsConnected())
								{
									bool flag4 = false;
									if (nPC4.boss || nPC4.netAlways || nPC4.townNPC || !nPC4.active)
									{
										flag4 = true;
									}
									else if (nPC4.netSkip <= 0)
									{
										Rectangle rect5 = Main.player[num29].getRect();
										Rectangle rect6 = nPC4.getRect();
										rect6.X -= 2500;
										rect6.Y -= 2500;
										rect6.Width += 5000;
										rect6.Height += 5000;
										if (rect5.Intersects(rect6))
										{
											flag4 = true;
										}
									}
									else
									{
										flag4 = true;
									}
									if (flag4)
									{
										try
										{
											buffer[num29].spamCount++;
											Main.txMsg++;
											Main.txData += num22;
											Main.txMsgType[msgType]++;
											Main.txDataType[msgType] += num22;
											Netplay.Clients[num29].Socket.AsyncSend(buffer[num].writeBuffer, 0, num22, Netplay.Clients[num29].ServerWriteCallBack);
										}
										catch
										{
										}
									}
								}
							}
							nPC4.netSkip++;
							if (nPC4.netSkip > 4)
							{
								nPC4.netSkip = 0;
							}
							break;
						}
						case 28:
						{
							NPC nPC3 = Main.npc[number];
							for (int num26 = 0; num26 < 17; num26++)
							{
								if (num26 != ignoreClient && buffer[num26].broadcast && Netplay.Clients[num26].IsConnected())
								{
									bool flag3 = false;
									if (nPC3.life <= 0)
									{
										flag3 = true;
									}
									else
									{
										Rectangle rect3 = Main.player[num26].getRect();
										Rectangle rect4 = nPC3.getRect();
										rect4.X -= 3000;
										rect4.Y -= 3000;
										rect4.Width += 6000;
										rect4.Height += 6000;
										if (rect3.Intersects(rect4))
										{
											flag3 = true;
										}
									}
									if (flag3)
									{
										try
										{
											buffer[num26].spamCount++;
											Main.txMsg++;
											Main.txData += num22;
											Main.txMsgType[msgType]++;
											Main.txDataType[msgType] += num22;
											Netplay.Clients[num26].Socket.AsyncSend(buffer[num].writeBuffer, 0, num22, Netplay.Clients[num26].ServerWriteCallBack);
										}
										catch
										{
										}
									}
								}
							}
							break;
						}
						case 13:
						{
							for (int num27 = 0; num27 < 17; num27++)
							{
								if (num27 != ignoreClient && buffer[num27].broadcast && Netplay.Clients[num27].IsConnected())
								{
									try
									{
										buffer[num27].spamCount++;
										Main.txMsg++;
										Main.txData += num22;
										Main.txMsgType[msgType]++;
										Main.txDataType[msgType] += num22;
										Netplay.Clients[num27].Socket.AsyncSend(buffer[num].writeBuffer, 0, num22, Netplay.Clients[num27].ServerWriteCallBack);
									}
									catch
									{
									}
								}
							}
							Main.player[number].netSkip++;
							if (Main.player[number].netSkip > 2)
							{
								Main.player[number].netSkip = 0;
							}
							break;
						}
						case 27:
						{
							Projectile projectile2 = Main.projectile[number];
							for (int num25 = 0; num25 < 17; num25++)
							{
								if (num25 != ignoreClient && buffer[num25].broadcast && Netplay.Clients[num25].IsConnected())
								{
									bool flag2 = false;
									if (projectile2.type == 12 || Main.projPet[projectile2.type] || projectile2.aiStyle == 11 || projectile2.netImportant)
									{
										flag2 = true;
									}
									else
									{
										Rectangle rect = Main.player[num25].getRect();
										Rectangle rect2 = projectile2.getRect();
										rect2.X -= 5000;
										rect2.Y -= 5000;
										rect2.Width += 10000;
										rect2.Height += 10000;
										if (rect.Intersects(rect2))
										{
											flag2 = true;
										}
									}
									if (flag2)
									{
										try
										{
											buffer[num25].spamCount++;
											Main.txMsg++;
											Main.txData += num22;
											Main.txMsgType[msgType]++;
											Main.txDataType[msgType] += num22;
											Netplay.Clients[num25].Socket.AsyncSend(buffer[num].writeBuffer, 0, num22, Netplay.Clients[num25].ServerWriteCallBack);
										}
										catch
										{
										}
									}
								}
							}
							break;
						}
						default:
						{
							for (int num23 = 0; num23 < 17; num23++)
							{
								if (num23 != ignoreClient && (buffer[num23].broadcast || (Netplay.Clients[num23].State >= 3 && msgType == 10)) && Netplay.Clients[num23].IsConnected())
								{
									try
									{
										buffer[num23].spamCount++;
										Main.txMsg++;
										Main.txData += num22;
										Main.txMsgType[msgType]++;
										Main.txDataType[msgType] += num22;
										Netplay.Clients[num23].Socket.AsyncSend(buffer[num].writeBuffer, 0, num22, Netplay.Clients[num23].ServerWriteCallBack);
									}
									catch
									{
									}
								}
							}
							break;
						}
						}
					}
					else if (Netplay.Clients[remoteClient].IsConnected())
					{
						try
						{
							buffer[remoteClient].spamCount++;
							Main.txMsg++;
							Main.txData += num22;
							Main.txMsgType[msgType]++;
							Main.txDataType[msgType] += num22;
							Netplay.Clients[remoteClient].Socket.AsyncSend(buffer[num].writeBuffer, 0, num22, Netplay.Clients[remoteClient].ServerWriteCallBack);
						}
						catch
						{
						}
					}
					if (Main.verboseNetplay)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine("Sent:");
						for (int num30 = 0; num30 < num22; num30++)
						{
							stringBuilder.Append(buffer[num].writeBuffer[num30] + " ");
						}
						stringBuilder.AppendLine("");
					}
					buffer[num].writeLocked = false;
					if (msgType == 19 && Main.netMode == 1)
					{
						SendTileSquare(num, (int)number2, (int)number3, 5);
					}
					if (msgType == 2 && Main.netMode == 2)
					{
						Netplay.Clients[num].PendingTermination = true;
					}
				}
			}
		}

		public static int CompressTileBlock(int xStart, int yStart, short width, short height, byte[] buffer, int bufferStart)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(xStart);
					binaryWriter.Write(yStart);
					binaryWriter.Write(width);
					binaryWriter.Write(height);
					CompressTileBlock_Inner(binaryWriter, xStart, yStart, width, height);
					int num = buffer.Length;
					if (bufferStart + memoryStream.Length > num)
					{
						return (int)(num - bufferStart + memoryStream.Length);
					}
					memoryStream.Position = 0L;
					MemoryStream memoryStream2 = new MemoryStream();
					using (DeflateStream deflateStream = new DeflateStream(memoryStream2, CompressionMode.Compress, true))
					{
						memoryStream.CopyTo(deflateStream);
						deflateStream.Flush();
						deflateStream.Close();
						deflateStream.Dispose();
					}
					if (memoryStream.Length <= memoryStream2.Length)
					{
						memoryStream.Position = 0L;
						buffer[bufferStart] = 0;
						bufferStart++;
						memoryStream.Read(buffer, bufferStart, (int)memoryStream.Length);
						return (int)memoryStream.Length + 1;
					}
					memoryStream2.Position = 0L;
					buffer[bufferStart] = 1;
					bufferStart++;
					memoryStream2.Read(buffer, bufferStart, (int)memoryStream2.Length);
					return (int)memoryStream2.Length + 1;
				}
			}
		}

		public static void CompressTileBlock_Inner(BinaryWriter writer, int xStart, int yStart, int width, int height)
		{
			short[] array = new short[1000];
			short[] array2 = new short[1000];
			short[] array3 = new short[1000];
			short num = 0;
			short num2 = 0;
			short num3 = 0;
			short num4 = 0;
			int num5 = 0;
			int num6 = 0;
			byte b = 0;
			byte[] array4 = new byte[13];
			Tile tile = null;
			for (int i = yStart; i < yStart + height; i++)
			{
				for (int j = xStart; j < xStart + width; j++)
				{
					Tile tile2 = Main.tile[j, i];
					if (tile2.isTheSameAs(tile))
					{
						num4 = (short)(num4 + 1);
						continue;
					}
					if (tile != null)
					{
						if (num4 > 0)
						{
							array4[num5] = (byte)(num4 & 0xFF);
							num5++;
							if (num4 > 255)
							{
								b = (byte)(b | 0x80);
								array4[num5] = (byte)((num4 & 0xFF00) >> 8);
								num5++;
							}
							else
							{
								b = (byte)(b | 0x40);
							}
						}
						array4[num6] = b;
						writer.Write(array4, num6, num5 - num6);
						num4 = 0;
					}
					num5 = 3;
					byte b2;
					byte b3;
					b = (b3 = (b2 = 0));
					if (tile2.active())
					{
						b = (byte)(b | 2);
						array4[num5] = (byte)tile2.type;
						num5++;
						if (tile2.type > 255)
						{
							array4[num5] = (byte)(tile2.type >> 8);
							num5++;
							b = (byte)(b | 0x20);
						}
						if (tile2.type == 21 && tile2.frameX % 36 == 0 && tile2.frameY % 36 == 0)
						{
							short num7 = (short)Chest.FindChest(j, i);
							if (num7 != -1)
							{
								array[num] = num7;
								num = (short)(num + 1);
							}
						}
						if (tile2.type == 88 && tile2.frameX % 54 == 0 && tile2.frameY % 36 == 0)
						{
							short num8 = (short)Chest.FindChest(j, i);
							if (num8 != -1)
							{
								array[num] = num8;
								num = (short)(num + 1);
							}
						}
						if (tile2.type == 85 && tile2.frameX % 36 == 0 && tile2.frameY % 36 == 0)
						{
							short num9 = (short)Sign.ReadSign(j, i);
							if (num9 != -1)
							{
								array2[num2++] = num9;
							}
						}
						if (tile2.type == 55 && tile2.frameX % 36 == 0 && tile2.frameY % 36 == 0)
						{
							short num10 = (short)Sign.ReadSign(j, i);
							if (num10 != -1)
							{
								array2[num2++] = num10;
							}
						}
						if (tile2.type == 378 && tile2.frameX % 36 == 0 && tile2.frameY == 0)
						{
							int num11 = TETrainingDummy.Find(j, i);
							if (num11 != -1)
							{
								array3[num3++] = (short)num11;
							}
						}
						if (tile2.type == 395 && tile2.frameX % 36 == 0 && tile2.frameY == 0)
						{
							int num12 = TEItemFrame.Find(j, i);
							if (num12 != -1)
							{
								array3[num3++] = (short)num12;
							}
						}
						if (Main.tileFrameImportant[tile2.type])
						{
							array4[num5] = (byte)(tile2.frameX & 0xFF);
							num5++;
							array4[num5] = (byte)((tile2.frameX & 0xFF00) >> 8);
							num5++;
							array4[num5] = (byte)(tile2.frameY & 0xFF);
							num5++;
							array4[num5] = (byte)((tile2.frameY & 0xFF00) >> 8);
							num5++;
						}
						if (tile2.color() != 0)
						{
							b2 = (byte)(b2 | 8);
							array4[num5] = tile2.color();
							num5++;
						}
					}
					if (tile2.wall != 0)
					{
						b = (byte)(b | 4);
						array4[num5] = tile2.wall;
						num5++;
						if (tile2.wallColor() != 0)
						{
							b2 = (byte)(b2 | 0x10);
							array4[num5] = tile2.wallColor();
							num5++;
						}
					}
					if (tile2.liquid != 0)
					{
						b = (tile2.lava() ? ((byte)(b | 0x10)) : ((!tile2.honey()) ? ((byte)(b | 8)) : ((byte)(b | 0x18))));
						array4[num5] = tile2.liquid;
						num5++;
					}
					if (tile2.wire())
					{
						b3 = (byte)(b3 | 2);
					}
					if (tile2.wire2())
					{
						b3 = (byte)(b3 | 4);
					}
					if (tile2.wire3())
					{
						b3 = (byte)(b3 | 8);
					}
					int num13 = tile2.halfBrick() ? 16 : ((tile2.slope() != 0) ? (tile2.slope() + 1 << 4) : 0);
					b3 = (byte)(b3 | (byte)num13);
					if (tile2.actuator())
					{
						b2 = (byte)(b2 | 2);
					}
					if (tile2.inActive())
					{
						b2 = (byte)(b2 | 4);
					}
					num6 = 2;
					if (b2 != 0)
					{
						b3 = (byte)(b3 | 1);
						array4[num6] = b2;
						num6--;
					}
					if (b3 != 0)
					{
						b = (byte)(b | 1);
						array4[num6] = b3;
						num6--;
					}
					tile = tile2;
				}
			}
			if (num4 > 0)
			{
				array4[num5] = (byte)(num4 & 0xFF);
				num5++;
				if (num4 > 255)
				{
					b = (byte)(b | 0x80);
					array4[num5] = (byte)((num4 & 0xFF00) >> 8);
					num5++;
				}
				else
				{
					b = (byte)(b | 0x40);
				}
			}
			array4[num6] = b;
			writer.Write(array4, num6, num5 - num6);
			writer.Write(num);
			for (int k = 0; k < num; k++)
			{
				Chest chest = Main.chest[array[k]];
				writer.Write(array[k]);
				writer.Write((short)chest.x);
				writer.Write((short)chest.y);
				writer.Write(chest.name);
			}
			writer.Write(num2);
			for (int l = 0; l < num2; l++)
			{
				Sign sign = Main.sign[array2[l]];
				writer.Write(array2[l]);
				writer.Write((short)sign.x);
				writer.Write((short)sign.y);
				writer.Write(sign.text);
			}
			writer.Write(num3);
			for (int m = 0; m < num3; m++)
			{
				TileEntity.Write(writer, TileEntity.ByID[array3[m]]);
			}
		}

		public static void DecompressTileBlock(byte[] buffer, int bufferStart, int bufferLength)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				memoryStream.Write(buffer, bufferStart, bufferLength);
				memoryStream.Position = 0L;
				MemoryStream memoryStream3;
				if (memoryStream.ReadByte() != 0)
				{
					MemoryStream memoryStream2 = new MemoryStream();
					using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress, true))
					{
						deflateStream.CopyTo(memoryStream2);
						deflateStream.Close();
					}
					memoryStream3 = memoryStream2;
					memoryStream3.Position = 0L;
				}
				else
				{
					memoryStream3 = memoryStream;
					memoryStream3.Position = 1L;
				}
				using (BinaryReader binaryReader = new BinaryReader(memoryStream3))
				{
					int xStart = binaryReader.ReadInt32();
					int yStart = binaryReader.ReadInt32();
					short width = binaryReader.ReadInt16();
					short height = binaryReader.ReadInt16();
					DecompressTileBlock_Inner(binaryReader, xStart, yStart, width, height);
				}
			}
		}

		public static void DecompressTileBlock_Inner(BinaryReader reader, int xStart, int yStart, int width, int height)
		{
			Tile tile = null;
			int num = 0;
			for (int i = yStart; i < yStart + height; i++)
			{
				for (int j = xStart; j < xStart + width; j++)
				{
					if (num != 0)
					{
						num--;
						if (Main.tile[j, i] == null)
						{
							Main.tile[j, i] = new Tile(tile);
						}
						else
						{
							Main.tile[j, i].CopyFrom(tile);
						}
						continue;
					}
					byte b;
					byte b2 = b = 0;
					tile = Main.tile[j, i];
					if (tile == null)
					{
						tile = new Tile();
						Main.tile[j, i] = tile;
					}
					else
					{
						tile.ClearEverything();
					}
					byte b3 = reader.ReadByte();
					if ((b3 & 1) == 1)
					{
						b2 = reader.ReadByte();
						if ((b2 & 1) == 1)
						{
							b = reader.ReadByte();
						}
					}
					bool flag = tile.active();
					byte b4;
					if ((b3 & 2) == 2)
					{
						tile.active(true);
						ushort type = tile.type;
						int num2;
						if ((b3 & 0x20) == 32)
						{
							b4 = reader.ReadByte();
							num2 = reader.ReadByte();
							num2 = ((num2 << 8) | b4);
						}
						else
						{
							num2 = reader.ReadByte();
						}
						tile.type = (ushort)num2;
						if (Main.tileFrameImportant[num2])
						{
							tile.frameX = reader.ReadInt16();
							tile.frameY = reader.ReadInt16();
						}
						else if (!flag || tile.type != type)
						{
							tile.frameX = -1;
							tile.frameY = -1;
						}
						if ((b & 8) == 8)
						{
							tile.color(reader.ReadByte());
						}
					}
					if ((b3 & 4) == 4)
					{
						tile.wall = reader.ReadByte();
						if ((b & 0x10) == 16)
						{
							tile.wallColor(reader.ReadByte());
						}
					}
					b4 = (byte)((b3 & 0x18) >> 3);
					if (b4 != 0)
					{
						tile.liquid = reader.ReadByte();
						if (b4 > 1)
						{
							if (b4 == 2)
							{
								tile.lava(true);
							}
							else
							{
								tile.honey(true);
							}
						}
					}
					if (b2 > 1)
					{
						if ((b2 & 2) == 2)
						{
							tile.wire(true);
						}
						if ((b2 & 4) == 4)
						{
							tile.wire2(true);
						}
						if ((b2 & 8) == 8)
						{
							tile.wire3(true);
						}
						b4 = (byte)((b2 & 0x70) >> 4);
						if (b4 != 0 && Main.tileSolid[tile.type])
						{
							if (b4 == 1)
							{
								tile.halfBrick(true);
							}
							else
							{
								tile.slope((byte)(b4 - 1));
							}
						}
					}
					if (b > 0)
					{
						if ((b & 2) == 2)
						{
							tile.actuator(true);
						}
						if ((b & 4) == 4)
						{
							tile.inActive(true);
						}
					}
					switch ((byte)((b3 & 0xC0) >> 6))
					{
					case 0:
						num = 0;
						break;
					case 1:
						num = reader.ReadByte();
						break;
					default:
						num = reader.ReadInt16();
						break;
					}
				}
			}
			short num3 = reader.ReadInt16();
			for (int k = 0; k < num3; k++)
			{
				short num4 = reader.ReadInt16();
				short x = reader.ReadInt16();
				short y = reader.ReadInt16();
				string name = reader.ReadString();
				if (num4 >= 0 && num4 < 1000)
				{
					if (Main.chest[num4] == null)
					{
						Main.chest[num4] = new Chest();
					}
					Main.chest[num4].name = name;
					Main.chest[num4].x = x;
					Main.chest[num4].y = y;
				}
			}
			num3 = reader.ReadInt16();
			for (int l = 0; l < num3; l++)
			{
				short num5 = reader.ReadInt16();
				short x2 = reader.ReadInt16();
				short y2 = reader.ReadInt16();
				string text = reader.ReadString();
				if (num5 >= 0 && num5 < 1000)
				{
					if (Main.sign[num5] == null)
					{
						Main.sign[num5] = new Sign();
					}
					Main.sign[num5].text = text;
					Main.sign[num5].x = x2;
					Main.sign[num5].y = y2;
				}
			}
			num3 = reader.ReadInt16();
			for (int m = 0; m < num3; m++)
			{
				TileEntity tileEntity = TileEntity.Read(reader);
				TileEntity.ByID[tileEntity.ID] = tileEntity;
				TileEntity.ByPosition[tileEntity.Position] = tileEntity;
			}
		}

		public static void RecieveBytes(byte[] bytes, int streamLength, int i = 17)
		{
			lock (buffer[i])
			{
				try
				{
					Buffer.BlockCopy(bytes, 0, buffer[i].readBuffer, buffer[i].totalData, streamLength);
					buffer[i].totalData += streamLength;
					buffer[i].checkBytes = true;
				}
				catch
				{
					if (Main.netMode == 1)
					{
						Main.menuMode = 15;
						Main.statusText = "Bad header lead to a read buffer overflow.";
						Netplay.disconnect = true;
					}
					else
					{
						Netplay.Clients[i].PendingTermination = true;
					}
				}
			}
		}

		public static void CheckBytes(int bufferIndex = 17)
		{
			DateTime now = DateTime.Now;
			lock (buffer[bufferIndex])
			{
				int num = 0;
				int num2 = buffer[bufferIndex].totalData;
				try
				{
					while (num2 >= 2)
					{
						int num3 = BitConverter.ToUInt16(buffer[bufferIndex].readBuffer, num);
						if (num2 < num3)
						{
							break;
						}
						if ((DateTime.Now - now).TotalSeconds > 0.05000000074505806)
						{
							now = DateTime.Now;
							SendData(106, -1, -1, "", -1);
						}
						int messageType;
						buffer[bufferIndex].GetData(num + 2, num3 - 2, out messageType);
						num2 -= num3;
						num += num3;
					}
				}
				catch
				{
					if (num < buffer.Length - 100)
					{
						Console.WriteLine("Error on message " + buffer[num + 2]);
					}
					num2 = 0;
					num = 0;
				}
				if (num2 != buffer[bufferIndex].totalData)
				{
					for (int i = 0; i < num2; i++)
					{
						buffer[bufferIndex].readBuffer[i] = buffer[bufferIndex].readBuffer[i + num];
					}
					buffer[bufferIndex].totalData = num2;
				}
				buffer[bufferIndex].checkBytes = false;
			}
		}

		public static void BootPlayer(int plr, string msg)
		{
			SendData(2, plr, -1, msg);
		}

		public static void SendObjectPlacment(int whoAmi, int x, int y, int type, int style, int alternative, int random, int direction)
		{
			int remoteClient;
			int ignoreClient;
			if (Main.netMode == 2)
			{
				remoteClient = -1;
				ignoreClient = whoAmi;
			}
			else
			{
				remoteClient = whoAmi;
				ignoreClient = -1;
			}
			SendData(79, remoteClient, ignoreClient, "", x, y, type, style, alternative, random, direction);
		}

		public static void SendTemporaryAnimation(int whoAmi, int animationType, int tileType, int xCoord, int yCoord)
		{
			SendData(77, whoAmi, -1, "", animationType, tileType, xCoord, yCoord);
		}

		public static void SendTileRange(int whoAmi, int tileX, int tileY, int xSize, int ySize)
		{
			int number = (xSize >= ySize) ? xSize : ySize;
			SendData(20, whoAmi, -1, "", number, tileX, tileY);
		}

		public static void SendTileSquare(int whoAmi, int tileX, int tileY, int size)
		{
			int num = (size - 1) / 2;
			SendData(20, whoAmi, -1, "", size, tileX - num, tileY - num);
		}

		public static void SendTravelShop()
		{
			if (Main.netMode == 2)
			{
				SendData(72);
			}
		}

		public static void SendAnglerQuest()
		{
			if (Main.netMode != 2)
			{
				return;
			}
			for (int i = 0; i < 16; i++)
			{
				if (Netplay.Clients[i].State == 10)
				{
					SendData(74, i, -1, Main.player[i].name, Main.anglerQuest);
				}
			}
		}

		public static void SendSection(int whoAmi, int sectionX, int sectionY, bool skipSent = false)
		{
			if (Main.netMode == 2)
			{
				try
				{
					if (sectionX >= 0 && sectionY >= 0 && sectionX < Main.maxSectionsX && sectionY < Main.maxSectionsY && (!skipSent || !Netplay.Clients[whoAmi].TileSections[sectionX, sectionY]))
					{
						Netplay.Clients[whoAmi].TileSections[sectionX, sectionY] = true;
						int num = sectionX * 200;
						int num2 = sectionY * 150;
						int num3 = 150;
						int num4 = num2 + 150;
						if (num4 > Main.maxTilesY)
						{
							num4 = Main.maxTilesY;
						}
						int num5 = 200;
						if (num + num5 > Main.maxTilesX)
						{
							num5 = Main.maxTilesX - num;
						}
						for (int i = num2; i < num4; i += num3)
						{
							int num6 = num3;
							if (i + num3 > Main.maxTilesY)
							{
								num6 = Main.maxTilesY - i;
							}
							SendData(10, whoAmi, -1, "", num, i, num5, num6);
						}
						for (int j = 0; j < 200; j++)
						{
							if (Main.npc[j].active && Main.npc[j].townNPC)
							{
								int sectionX2 = Netplay.GetSectionX((int)(Main.npc[j].position.X / 16f));
								int sectionY2 = Netplay.GetSectionY((int)(Main.npc[j].position.Y / 16f));
								if (sectionX2 == sectionX && sectionY2 == sectionY)
								{
									SendData(23, whoAmi, -1, "", j);
								}
							}
						}
					}
				}
				catch
				{
				}
			}
		}

		public static void greetPlayer(int plr)
		{
			if (Main.motd == "")
			{
				SendData(25, plr, -1, string.Concat(Lang.mp[18], " ", Main.worldName, "!"), 16, 255f, 240f, 20f);
			}
			else
			{
				SendData(25, plr, -1, Main.motd, 16, 255f, 240f, 20f);
			}
			string text = "";
			for (int i = 0; i < 16; i++)
			{
				if (Main.player[i].active)
				{
					text = ((!(text == "")) ? (text + ", " + Main.player[i].name) : (text + Main.player[i].name));
				}
			}
			SendData(25, plr, -1, Language.GetTextValue("Game.JoinGreeting", text), 16, 255f, 240f, 20f);
		}

		public static void sendWater(int x, int y)
		{
			if (Main.netMode == 1)
			{
				SendData(48, -1, -1, "", x, y);
				return;
			}
			for (int i = 0; i < 17; i++)
			{
				if ((buffer[i].broadcast || Netplay.Clients[i].State >= 3) && Netplay.Clients[i].IsConnected())
				{
					int num = x / 200;
					int num2 = y / 150;
					if (Netplay.Clients[i].TileSections[num, num2])
					{
						SendData(48, i, -1, "", x, y);
					}
				}
			}
		}

		public static void syncPlayers()
		{
			bool flag = false;
			for (int i = 0; i < 16; i++)
			{
				int num = 0;
				if (Main.player[i].active)
				{
					num = 1;
				}
				if (Netplay.Clients[i].State == 10)
				{
					if (Main.autoShutdown && !flag && Netplay.Clients[i].Socket.GetRemoteAddress().IsLocalHost())
					{
						flag = true;
					}
					SendData(14, -1, i, "", i, num);
					SendData(4, -1, i, Main.player[i].name, i);
					SendData(13, -1, i, "", i);
					SendData(16, -1, i, "", i);
					SendData(30, -1, i, "", i);
					SendData(45, -1, i, "", i);
					SendData(42, -1, i, "", i);
					SendData(50, -1, i, "", i);
					for (int j = 0; j < 59; j++)
					{
						SendData(5, -1, i, "", i, j, (int)Main.player[i].inventory[j].prefix);
					}
					for (int k = 0; k < Main.player[i].armor.Length; k++)
					{
						SendData(5, -1, i, "", i, 59 + k, (int)Main.player[i].armor[k].prefix);
					}
					for (int l = 0; l < Main.player[i].dye.Length; l++)
					{
						SendData(5, -1, i, "", i, 58 + Main.player[i].armor.Length + 1 + l, (int)Main.player[i].dye[l].prefix);
					}
					for (int m = 0; m < Main.player[i].miscEquips.Length; m++)
					{
						SendData(5, -1, i, "", i, 58 + Main.player[i].armor.Length + Main.player[i].dye.Length + 1 + m, (int)Main.player[i].miscEquips[m].prefix);
					}
					for (int n = 0; n < Main.player[i].miscDyes.Length; n++)
					{
						SendData(5, -1, i, "", i, 58 + Main.player[i].armor.Length + Main.player[i].dye.Length + Main.player[i].miscEquips.Length + 1 + n, (int)Main.player[i].miscDyes[n].prefix);
					}
					if (!Netplay.Clients[i].IsAnnouncementCompleted)
					{
						Netplay.Clients[i].IsAnnouncementCompleted = true;
						SendData(25, -1, i, Lang.mp[19].Format(Main.player[i].name), 16, 255f, 240f, 20f);
						if (Main.dedServ)
						{
							Console.WriteLine(Lang.mp[19].Format(Main.player[i].name));
						}
					}
					continue;
				}
				num = 0;
				SendData(14, -1, i, "", i, num);
				if (Netplay.Clients[i].IsAnnouncementCompleted)
				{
					Netplay.Clients[i].IsAnnouncementCompleted = false;
					SendData(25, -1, i, Lang.mp[20].Format(Netplay.Clients[i].Name), 16, 255f, 240f, 20f);
					if (Main.dedServ)
					{
						Console.WriteLine(Lang.mp[20].Format(Netplay.Clients[i].Name));
					}
					Netplay.Clients[i].Name = "Anonymous";
				}
			}
			bool flag2 = false;
			for (int num2 = 0; num2 < 200; num2++)
			{
				if (Main.npc[num2].active && Main.npc[num2].townNPC && NPC.TypeToNum(Main.npc[num2].type) != -1)
				{
					if (!flag2 && Main.npc[num2].type == 368)
					{
						flag2 = true;
					}
					int num3 = 0;
					if (Main.npc[num2].homeless)
					{
						num3 = 1;
					}
					SendData(60, -1, -1, "", num2, Main.npc[num2].homeTileX, Main.npc[num2].homeTileY, num3);
				}
			}
			if (flag2)
			{
				SendTravelShop();
			}
			SendAnglerQuest();
			if (Main.autoShutdown && !flag)
			{
				Console.WriteLine("Local player left. Autoshutdown starting.");
				WorldFile.saveWorld();
				Netplay.disconnect = true;
			}
		}
	}
}
