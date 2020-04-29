using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Net;
using Terraria.UI;

namespace Terraria
{
	public class MessageBuffer
	{
		public const int readBufferMax = 131070;

		public const int writeBufferMax = 131070;

		public bool broadcast;

		public byte[] readBuffer = new byte[131070];

		public byte[] writeBuffer = new byte[131070];

		public bool writeLocked;

		public int messageLength;

		public int totalData;

		public int whoAmI;

		public int spamCount;

		public int maxSpam;

		public bool checkBytes;

		public MemoryStream readerStream;

		public MemoryStream writerStream;

		public BinaryReader reader;

		public BinaryWriter writer;

		public void Reset()
		{
			Array.Clear(readBuffer, 0, readBuffer.Length);
			Array.Clear(writeBuffer, 0, writeBuffer.Length);
			writeLocked = false;
			messageLength = 0;
			totalData = 0;
			spamCount = 0;
			broadcast = false;
			checkBytes = false;
			ResetReader();
			ResetWriter();
		}

		public void ResetReader()
		{
			if (readerStream != null)
			{
				readerStream.Close();
			}
			readerStream = new MemoryStream(readBuffer);
			reader = new BinaryReader(readerStream);
		}

		public void ResetWriter()
		{
			if (writerStream != null)
			{
				writerStream.Close();
			}
			writerStream = new MemoryStream(writeBuffer);
			writer = new BinaryWriter(writerStream);
		}

		public void GetData(int start, int length, out int messageType)
		{
			if (whoAmI < 17)
			{
				Netplay.Clients[whoAmI].TimeOutTimer = 0;
			}
			else
			{
				Netplay.Connection.TimeOutTimer = 0;
			}
			byte b = 0;
			int num = 0;
			num = start + 1;
			b = (byte)(messageType = readBuffer[start]);
			if (b >= 107)
			{
				return;
			}
			Main.rxMsg++;
			Main.rxData += length;
			Main.rxMsgType[b]++;
			Main.rxDataType[b] += length;
			if (Main.netMode == 1 && Netplay.Connection.StatusMax > 0)
			{
				Netplay.Connection.StatusCount++;
			}
			if (Main.verboseNetplay)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(Main.myPlayer + " Recieve:");
				for (int i = start; i < start + length; i++)
				{
					stringBuilder.Append(readBuffer[i] + " ");
				}
				stringBuilder.AppendLine("");
			}
			if (Main.netMode == 2 && b != 38 && Netplay.Clients[whoAmI].State == -1)
			{
				NetMessage.SendData(2, whoAmI, -1, Lang.mp[1].Value);
				return;
			}
			if (Main.netMode == 2 && Netplay.Clients[whoAmI].State < 10 && b > 12 && b != 93 && b != 16 && b != 42 && b != 50 && b != 38 && b != 68 && b != 106)
			{
				NetMessage.BootPlayer(whoAmI, Lang.mp[2].Value);
			}
			if (reader == null)
			{
				ResetReader();
			}
			reader.BaseStream.Position = num;
			switch (b)
			{
			case 15:
			case 67:
			case 93:
			case 94:
				break;
			case 1:
				if (Main.netMode != 2)
				{
					break;
				}
				if (Main.dedServ && Netplay.IsBanned(Netplay.Clients[whoAmI].Socket.GetRemoteAddress()))
				{
					NetMessage.SendData(2, whoAmI, -1, Lang.mp[3].Value);
				}
				else
				{
					if (Netplay.Clients[whoAmI].State != 0)
					{
						break;
					}
					string a = reader.ReadString();
					if (a == "Terraria" + Main.curRelease)
					{
						if (string.IsNullOrEmpty(Netplay.ServerPassword))
						{
							Netplay.Clients[whoAmI].State = 1;
							NetMessage.SendData(3, whoAmI);
						}
						else
						{
							Netplay.Clients[whoAmI].State = -1;
							NetMessage.SendData(37, whoAmI);
						}
					}
					else
					{
						NetMessage.SendData(2, whoAmI, -1, Lang.mp[4].Value);
					}
				}
				break;
			case 2:
				if (Main.netMode == 1)
				{
					Netplay.disconnect = true;
					Main.statusText = reader.ReadString();
				}
				break;
			case 3:
				if (Main.netMode == 1)
				{
					if (Netplay.Connection.State == 1)
					{
						Netplay.Connection.State = 2;
					}
					int num89 = reader.ReadByte();
					if (num89 != Main.myPlayer)
					{
						Main.player[num89] = Main.ActivePlayerFileData.Player;
						Main.player[Main.myPlayer] = new Player();
					}
					Main.player[num89].whoAmI = num89;
					Main.myPlayer = num89;
					Player player8 = Main.player[num89];
					NetMessage.SendData(4, -1, -1, player8.name, num89);
					NetMessage.SendData(68, -1, -1, "", num89);
					NetMessage.SendData(16, -1, -1, "", num89);
					NetMessage.SendData(42, -1, -1, "", num89);
					NetMessage.SendData(50, -1, -1, "", num89);
					for (int num90 = 0; num90 < 59; num90++)
					{
						NetMessage.SendData(5, -1, -1, "", num89, num90, (int)player8.inventory[num90].prefix);
					}
					for (int num91 = 0; num91 < player8.armor.Length; num91++)
					{
						NetMessage.SendData(5, -1, -1, "", num89, 59 + num91, (int)player8.armor[num91].prefix);
					}
					for (int num92 = 0; num92 < player8.dye.Length; num92++)
					{
						NetMessage.SendData(5, -1, -1, "", num89, 58 + player8.armor.Length + 1 + num92, (int)player8.dye[num92].prefix);
					}
					for (int num93 = 0; num93 < player8.miscEquips.Length; num93++)
					{
						NetMessage.SendData(5, -1, -1, "", num89, 58 + player8.armor.Length + player8.dye.Length + 1 + num93, (int)player8.miscEquips[num93].prefix);
					}
					for (int num94 = 0; num94 < player8.miscDyes.Length; num94++)
					{
						NetMessage.SendData(5, -1, -1, "", num89, 58 + player8.armor.Length + player8.dye.Length + player8.miscEquips.Length + 1 + num94, (int)player8.miscDyes[num94].prefix);
					}
					for (int num95 = 0; num95 < player8.bank.item.Length; num95++)
					{
						NetMessage.SendData(5, -1, -1, "", num89, 58 + player8.armor.Length + player8.dye.Length + player8.miscEquips.Length + player8.miscDyes.Length + 1 + num95, (int)player8.bank.item[num95].prefix);
					}
					for (int num96 = 0; num96 < player8.bank2.item.Length; num96++)
					{
						NetMessage.SendData(5, -1, -1, "", num89, 58 + player8.armor.Length + player8.dye.Length + player8.miscEquips.Length + player8.miscDyes.Length + player8.bank.item.Length + 1 + num96, (int)player8.bank2.item[num96].prefix);
					}
					NetMessage.SendData(5, -1, -1, "", num89, 58 + player8.armor.Length + player8.dye.Length + player8.miscEquips.Length + player8.miscDyes.Length + player8.bank.item.Length + player8.bank2.item.Length + 1, (int)player8.trashItem.prefix);
					NetMessage.SendData(6);
					if (Netplay.Connection.State == 2)
					{
						Netplay.Connection.State = 3;
					}
				}
				break;
			case 4:
			{
				int num198 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num198 = whoAmI;
				}
				if (num198 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					break;
				}
				Player player18 = Main.player[num198];
				player18.whoAmI = num198;
				player18.skinVariant = reader.ReadByte();
				player18.skinVariant = (int)MathHelper.Clamp(player18.skinVariant, 0f, 7f);
				player18.hair = reader.ReadByte();
				if (player18.hair >= 134)
				{
					player18.hair = 0;
				}
				player18.name = reader.ReadString().Trim().Trim();
				player18.hairDye = reader.ReadByte();
				BitsByte bitsByte15 = reader.ReadByte();
				for (int num199 = 0; num199 < 8; num199++)
				{
					player18.hideVisual[num199] = bitsByte15[num199];
				}
				bitsByte15 = reader.ReadByte();
				for (int num200 = 0; num200 < 2; num200++)
				{
					player18.hideVisual[num200 + 8] = bitsByte15[num200];
				}
				player18.hideMisc = reader.ReadByte();
				player18.hairColor = reader.ReadRGB();
				player18.skinColor = reader.ReadRGB();
				player18.eyeColor = reader.ReadRGB();
				player18.shirtColor = reader.ReadRGB();
				player18.underShirtColor = reader.ReadRGB();
				player18.pantsColor = reader.ReadRGB();
				player18.shoeColor = reader.ReadRGB();
				BitsByte bitsByte16 = reader.ReadByte();
				player18.difficulty = 0;
				if (bitsByte16[0])
				{
					player18.difficulty++;
				}
				if (bitsByte16[1])
				{
					player18.difficulty += 2;
				}
				if (player18.difficulty > 2)
				{
					player18.difficulty = 2;
				}
				player18.extraAccessory = bitsByte16[2];
				if (Main.netMode != 2)
				{
					break;
				}
				bool flag11 = false;
				if (Netplay.Clients[whoAmI].State < 10)
				{
					for (int num201 = 0; num201 < 16; num201++)
					{
						if (num201 != num198 && player18.name == Main.player[num201].name && Netplay.Clients[num201].IsActive)
						{
							flag11 = true;
						}
					}
				}
				if (flag11)
				{
					NetMessage.SendData(2, whoAmI, -1, Lang.mp[5].Format(player18.name));
					break;
				}
				if (player18.name.Length > Player.nameLen)
				{
					NetMessage.SendData(2, whoAmI, -1, "Name is too long.");
					break;
				}
				if (player18.name == "")
				{
					NetMessage.SendData(2, whoAmI, -1, "Empty name.");
					break;
				}
				Netplay.Clients[whoAmI].Name = player18.name;
				Netplay.Clients[whoAmI].Name = player18.name;
				NetMessage.SendData(4, -1, whoAmI, player18.name, num198);
				break;
			}
			case 5:
			{
				int num154 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num154 = whoAmI;
				}
				if (num154 != Main.myPlayer || Main.ServerSideCharacter || Main.player[num154].IsStackingItems())
				{
					Player player14 = Main.player[num154];
					lock (player14)
					{
						int num155 = reader.ReadByte();
						int stack5 = reader.ReadInt16();
						int num156 = reader.ReadByte();
						int type8 = reader.ReadInt16();
						Item[] array3 = null;
						int num157 = 0;
						bool flag6 = false;
						if (num155 > 58 + player14.armor.Length + player14.dye.Length + player14.miscEquips.Length + player14.miscDyes.Length + player14.bank.item.Length + player14.bank2.item.Length)
						{
							flag6 = true;
						}
						else if (num155 > 58 + player14.armor.Length + player14.dye.Length + player14.miscEquips.Length + player14.miscDyes.Length + player14.bank.item.Length)
						{
							num157 = num155 - 58 - (player14.armor.Length + player14.dye.Length + player14.miscEquips.Length + player14.miscDyes.Length + player14.bank.item.Length) - 1;
							array3 = player14.bank2.item;
						}
						else if (num155 > 58 + player14.armor.Length + player14.dye.Length + player14.miscEquips.Length + player14.miscDyes.Length)
						{
							num157 = num155 - 58 - (player14.armor.Length + player14.dye.Length + player14.miscEquips.Length + player14.miscDyes.Length) - 1;
							array3 = player14.bank.item;
						}
						else if (num155 > 58 + player14.armor.Length + player14.dye.Length + player14.miscEquips.Length)
						{
							num157 = num155 - 58 - (player14.armor.Length + player14.dye.Length + player14.miscEquips.Length) - 1;
							array3 = player14.miscDyes;
						}
						else if (num155 > 58 + player14.armor.Length + player14.dye.Length)
						{
							num157 = num155 - 58 - (player14.armor.Length + player14.dye.Length) - 1;
							array3 = player14.miscEquips;
						}
						else if (num155 > 58 + player14.armor.Length)
						{
							num157 = num155 - 58 - player14.armor.Length - 1;
							array3 = player14.dye;
						}
						else if (num155 > 58)
						{
							num157 = num155 - 58 - 1;
							array3 = player14.armor;
						}
						else
						{
							num157 = num155;
							array3 = player14.inventory;
						}
						if (flag6)
						{
							player14.trashItem = new Item();
							player14.trashItem.netDefaults(type8);
							player14.trashItem.stack = stack5;
							player14.trashItem.Prefix(num156);
						}
						else if (num155 <= 58)
						{
							int type9 = array3[num157].type;
							int stack6 = array3[num157].stack;
							array3[num157] = new Item();
							array3[num157].netDefaults(type8);
							array3[num157].stack = stack5;
							array3[num157].Prefix(num156);
							if (num154 == Main.myPlayer && num157 == 58)
							{
								Main.mouseItem = array3[num157].Clone();
							}
							if (num154 == Main.myPlayer && Main.netMode == 1)
							{
								Main.player[num154].inventoryChestStack[num155] = false;
								if (array3[num157].stack != stack6 || array3[num157].type != type9)
								{
									Recipe.FindRecipes();
									Main.PlaySound(7);
								}
							}
						}
						else
						{
							array3[num157] = new Item();
							array3[num157].netDefaults(type8);
							array3[num157].stack = stack5;
							array3[num157].Prefix(num156);
						}
						if (Main.netMode == 2 && num154 == whoAmI && num155 <= 58 + player14.armor.Length + player14.dye.Length + player14.miscEquips.Length + player14.miscDyes.Length)
						{
							NetMessage.SendData(5, -1, whoAmI, "", num154, num155, num156);
						}
					}
				}
				break;
			}
			case 6:
				if (Main.netMode == 2)
				{
					if (Netplay.Clients[whoAmI].State == 1)
					{
						Netplay.Clients[whoAmI].State = 2;
						Netplay.Clients[whoAmI].ResetSections();
					}
					NetMessage.SendData(7, whoAmI);
					Main.SyncAnInvasion(whoAmI);
				}
				break;
			case 7:
				if (Main.netMode == 1)
				{
					Main.time = reader.ReadInt32();
					BitsByte bitsByte10 = reader.ReadByte();
					Main.dayTime = bitsByte10[0];
					Main.bloodMoon = bitsByte10[1];
					Main.eclipse = bitsByte10[2];
					Main.moonPhase = reader.ReadByte();
					Main.maxTilesX = reader.ReadInt16();
					Main.maxTilesY = reader.ReadInt16();
					Main.spawnTileX = reader.ReadInt16();
					Main.spawnTileY = reader.ReadInt16();
					Main.worldSurface = reader.ReadInt16();
					Main.rockLayer = reader.ReadInt16();
					Main.worldID = reader.ReadInt32();
					Main.worldName = reader.ReadString();
					Main.moonType = reader.ReadByte();
					WorldGen.setBG(0, reader.ReadByte());
					WorldGen.setBG(1, reader.ReadByte());
					WorldGen.setBG(2, reader.ReadByte());
					WorldGen.setBG(3, reader.ReadByte());
					WorldGen.setBG(4, reader.ReadByte());
					WorldGen.setBG(5, reader.ReadByte());
					WorldGen.setBG(6, reader.ReadByte());
					WorldGen.setBG(7, reader.ReadByte());
					Main.iceBackStyle = reader.ReadByte();
					Main.jungleBackStyle = reader.ReadByte();
					Main.hellBackStyle = reader.ReadByte();
					Main.windSpeedSet = reader.ReadSingle();
					Main.numClouds = reader.ReadByte();
					for (int num194 = 0; num194 < 3; num194++)
					{
						Main.treeX[num194] = reader.ReadInt32();
					}
					for (int num195 = 0; num195 < 4; num195++)
					{
						Main.treeStyle[num195] = reader.ReadByte();
					}
					for (int num196 = 0; num196 < 3; num196++)
					{
						Main.caveBackX[num196] = reader.ReadInt32();
					}
					for (int num197 = 0; num197 < 4; num197++)
					{
						Main.caveBackStyle[num197] = reader.ReadByte();
					}
					Main.maxRaining = reader.ReadSingle();
					Main.raining = (Main.maxRaining > 0f);
					BitsByte bitsByte11 = reader.ReadByte();
					WorldGen.shadowOrbSmashed = bitsByte11[0];
					NPC.downedBoss1 = bitsByte11[1];
					NPC.downedBoss2 = bitsByte11[2];
					NPC.downedBoss3 = bitsByte11[3];
					Main.hardMode = bitsByte11[4];
					NPC.downedClown = bitsByte11[5];
					Main.ServerSideCharacter = bitsByte11[6];
					NPC.downedPlantBoss = bitsByte11[7];
					BitsByte bitsByte12 = reader.ReadByte();
					NPC.downedMechBoss1 = bitsByte12[0];
					NPC.downedMechBoss2 = bitsByte12[1];
					NPC.downedMechBoss3 = bitsByte12[2];
					NPC.downedMechBossAny = bitsByte12[3];
					Main.cloudBGActive = (bitsByte12[4] ? 1 : 0);
					WorldGen.crimson = bitsByte12[5];
					Main.pumpkinMoon = bitsByte12[6];
					Main.snowMoon = bitsByte12[7];
					BitsByte bitsByte13 = reader.ReadByte();
					Main.expertMode = bitsByte13[0];
					Main.fastForwardTime = bitsByte13[1];
					Main.UpdateSundial();
					bool flag10 = bitsByte13[2];
					NPC.downedSlimeKing = bitsByte13[3];
					NPC.downedQueenBee = bitsByte13[4];
					NPC.downedFishron = bitsByte13[5];
					NPC.downedMartians = bitsByte13[6];
					NPC.downedAncientCultist = bitsByte13[7];
					BitsByte bitsByte14 = reader.ReadByte();
					NPC.downedMoonlord = bitsByte14[0];
					NPC.downedHalloweenKing = bitsByte14[1];
					NPC.downedHalloweenTree = bitsByte14[2];
					NPC.downedChristmasIceQueen = bitsByte14[3];
					NPC.downedChristmasSantank = bitsByte14[4];
					NPC.downedChristmasTree = bitsByte14[5];
					NPC.downedGolemBoss = bitsByte14[6];
					if (flag10)
					{
						Main.StartSlimeRain();
					}
					else
					{
						Main.StopSlimeRain();
					}
					Main.invasionType = reader.ReadSByte();
					Main.LobbyId = reader.ReadUInt64();
					if (Netplay.Connection.State == 3)
					{
						Netplay.Connection.State = 4;
					}
				}
				break;
			case 8:
			{
				if (Main.netMode != 2)
				{
					break;
				}
				int num30 = reader.ReadInt32();
				int num31 = reader.ReadInt32();
				bool flag = true;
				if (num30 == -1 || num31 == -1)
				{
					flag = false;
				}
				else if (num30 < 10 || num30 > Main.maxTilesX - 10)
				{
					flag = false;
				}
				else if (num31 < 10 || num31 > Main.maxTilesY - 10)
				{
					flag = false;
				}
				int num32 = Netplay.GetSectionX(Main.spawnTileX) - 2;
				int num33 = Netplay.GetSectionY(Main.spawnTileY) - 1;
				int num34 = num32 + 5;
				int num35 = num33 + 3;
				if (num32 < 0)
				{
					num32 = 0;
				}
				if (num34 >= Main.maxSectionsX)
				{
					num34 = Main.maxSectionsX - 1;
				}
				if (num33 < 0)
				{
					num33 = 0;
				}
				if (num35 >= Main.maxSectionsY)
				{
					num35 = Main.maxSectionsY - 1;
				}
				int num36 = (num34 - num32) * (num35 - num33);
				List<Point> list = new List<Point>();
				for (int l = num32; l < num34; l++)
				{
					for (int m = num33; m < num35; m++)
					{
						list.Add(new Point(l, m));
					}
				}
				int num37 = -1;
				int num38 = -1;
				if (flag)
				{
					num30 = Netplay.GetSectionX(num30) - 2;
					num31 = Netplay.GetSectionY(num31) - 1;
					num37 = num30 + 5;
					num38 = num31 + 3;
					if (num30 < 0)
					{
						num30 = 0;
					}
					if (num37 >= Main.maxSectionsX)
					{
						num37 = Main.maxSectionsX - 1;
					}
					if (num31 < 0)
					{
						num31 = 0;
					}
					if (num38 >= Main.maxSectionsY)
					{
						num38 = Main.maxSectionsY - 1;
					}
					for (int n = num30; n < num37; n++)
					{
						for (int num39 = num31; num39 < num38; num39++)
						{
							if (n < num32 || n >= num34 || num39 < num33 || num39 >= num35)
							{
								list.Add(new Point(n, num39));
								num36++;
							}
						}
					}
				}
				int num40 = 1;
				List<Point> portals;
				List<Point> portalCenters;
				PortalHelper.SyncPortalsOnPlayerJoin(whoAmI, 1, list, out portals, out portalCenters);
				num36 += portals.Count;
				if (Netplay.Clients[whoAmI].State == 2)
				{
					Netplay.Clients[whoAmI].State = 3;
				}
				NetMessage.SendData(9, whoAmI, -1, Lang.inter[44].Value, num36);
				Netplay.Clients[whoAmI].StatusText2 = "is receiving tile data";
				Netplay.Clients[whoAmI].StatusMax += num36;
				for (int num41 = num32; num41 < num34; num41++)
				{
					for (int num42 = num33; num42 < num35; num42++)
					{
						NetMessage.SendSection(whoAmI, num41, num42);
					}
				}
				NetMessage.SendData(11, whoAmI, -1, "", num32, num33, num34 - 1, num35 - 1);
				if (flag)
				{
					for (int num43 = num30; num43 < num37; num43++)
					{
						for (int num44 = num31; num44 < num38; num44++)
						{
							NetMessage.SendSection(whoAmI, num43, num44, true);
						}
					}
					NetMessage.SendData(11, whoAmI, -1, "", num30, num31, num37 - 1, num38 - 1);
				}
				for (int num45 = 0; num45 < portals.Count; num45++)
				{
					NetMessage.SendSection(whoAmI, portals[num45].X, portals[num45].Y, true);
				}
				for (int num46 = 0; num46 < portalCenters.Count; num46++)
				{
					NetMessage.SendData(11, whoAmI, -1, "", portalCenters[num46].X - num40, portalCenters[num46].Y - num40, portalCenters[num46].X + num40 + 1, portalCenters[num46].Y + num40 + 1);
				}
				for (int num47 = 0; num47 < 400; num47++)
				{
					if (Main.item[num47].active)
					{
						NetMessage.SendData(21, whoAmI, -1, "", num47);
						NetMessage.SendData(22, whoAmI, -1, "", num47);
					}
				}
				for (int num48 = 0; num48 < 200; num48++)
				{
					if (Main.npc[num48].active)
					{
						NetMessage.SendData(23, whoAmI, -1, "", num48);
					}
				}
				for (int num49 = 0; num49 < 1000; num49++)
				{
					if (Main.projectile[num49].active && (Main.projPet[Main.projectile[num49].type] || Main.projectile[num49].netImportant))
					{
						NetMessage.SendData(27, whoAmI, -1, "", num49);
					}
				}
				for (int num50 = 0; num50 < 251; num50++)
				{
					NetMessage.SendData(83, whoAmI, -1, "", num50);
				}
				NetMessage.SendData(49, whoAmI);
				NetMessage.SendData(57, whoAmI);
				NetMessage.SendData(7, whoAmI);
				NetMessage.SendData(103, -1, -1, "", NPC.MoonLordCountdown);
				NetMessage.SendData(101, whoAmI);
				break;
			}
			case 9:
				if (Main.netMode == 1)
				{
					Netplay.Connection.StatusMax += reader.ReadInt32();
					Netplay.Connection.StatusText = reader.ReadString();
				}
				break;
			case 10:
				if (Main.netMode == 1)
				{
					NetMessage.DecompressTileBlock(readBuffer, num, length);
				}
				break;
			case 11:
				if (Main.netMode == 1)
				{
					WorldGen.SectionTileFrame(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
				}
				break;
			case 12:
			{
				int num148 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num148 = whoAmI;
				}
				Player player12 = Main.player[num148];
				player12.SpawnX = reader.ReadInt16();
				player12.SpawnY = reader.ReadInt16();
				player12.Spawn();
				if (num148 == Main.myPlayer && Main.netMode != 2)
				{
					Main.ActivePlayerFileData.StartPlayTimer();
					Player.EnterWorld(Main.player[Main.myPlayer]);
				}
				if (Main.netMode == 2 && Netplay.Clients[whoAmI].State >= 3)
				{
					if (Netplay.Clients[whoAmI].State == 3)
					{
						Netplay.Clients[whoAmI].State = 10;
						NetMessage.greetPlayer(whoAmI);
						NetMessage.buffer[whoAmI].broadcast = true;
						NetMessage.syncPlayers();
						NetMessage.SendData(12, -1, whoAmI, "", whoAmI);
						NetMessage.SendData(74, whoAmI, -1, Main.player[whoAmI].name, Main.anglerQuest);
					}
					else
					{
						NetMessage.SendData(12, -1, whoAmI, "", whoAmI);
					}
				}
				break;
			}
			case 13:
			{
				int num183 = reader.ReadByte();
				if (num183 != Main.myPlayer || Main.ServerSideCharacter)
				{
					if (Main.netMode == 2)
					{
						num183 = whoAmI;
					}
					Player player16 = Main.player[num183];
					BitsByte bitsByte6 = reader.ReadByte();
					player16.controlUp = bitsByte6[0];
					player16.controlDown = bitsByte6[1];
					player16.controlLeft = bitsByte6[2];
					player16.controlRight = bitsByte6[3];
					player16.controlJump = bitsByte6[4];
					player16.controlUseItem = bitsByte6[5];
					player16.direction = (bitsByte6[6] ? 1 : (-1));
					BitsByte bitsByte7 = reader.ReadByte();
					if (bitsByte7[0])
					{
						player16.pulley = true;
						player16.pulleyDir = (byte)((!bitsByte7[1]) ? 1 : 2);
					}
					else
					{
						player16.pulley = false;
					}
					player16.selectedItem = reader.ReadByte();
					player16.position = reader.ReadVector2();
					if (bitsByte7[2])
					{
						player16.velocity = reader.ReadVector2();
					}
					else
					{
						player16.velocity = Vector2.Zero;
					}
					player16.vortexStealthActive = bitsByte7[3];
					player16.gravDir = (bitsByte7[4] ? 1 : (-1));
					if (Main.netMode == 2 && Netplay.Clients[whoAmI].State == 10)
					{
						NetMessage.SendData(13, -1, whoAmI, "", num183);
					}
				}
				break;
			}
			case 14:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num7 = reader.ReadByte();
				int num8 = reader.ReadByte();
				if (num8 == 1)
				{
					if (!Main.player[num7].active)
					{
						Main.player[num7] = new Player();
					}
					Main.player[num7].active = true;
				}
				else
				{
					Main.player[num7].active = false;
				}
				break;
			}
			case 16:
			{
				int num213 = reader.ReadByte();
				if (num213 != Main.myPlayer || Main.ServerSideCharacter)
				{
					if (Main.netMode == 2)
					{
						num213 = whoAmI;
					}
					Player player19 = Main.player[num213];
					player19.statLife = reader.ReadInt16();
					player19.statLifeMax = reader.ReadInt16();
					if (player19.statLifeMax < 100)
					{
						player19.statLifeMax = 100;
					}
					player19.dead = (player19.statLife <= 0);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(16, -1, whoAmI, "", num213);
					}
				}
				break;
			}
			case 17:
			{
				byte b12 = reader.ReadByte();
				int num162 = reader.ReadInt16();
				int num163 = reader.ReadInt16();
				short num164 = reader.ReadInt16();
				int num165 = reader.ReadByte();
				bool flag7 = num164 == 1;
				if (!WorldGen.InWorld(num162, num163, 3))
				{
					break;
				}
				if (Main.tile[num162, num163] == null)
				{
					Main.tile[num162, num163] = new Tile();
				}
				if (Main.netMode == 2)
				{
					if (!flag7)
					{
						if (b12 == 0 || b12 == 2 || b12 == 4)
						{
							Netplay.Clients[whoAmI].SpamDeleteBlock += 1f;
						}
						if (b12 == 1 || b12 == 3)
						{
							Netplay.Clients[whoAmI].SpamAddBlock += 1f;
						}
					}
					if (!Netplay.Clients[whoAmI].TileSections[Netplay.GetSectionX(num162), Netplay.GetSectionY(num163)])
					{
						flag7 = true;
					}
				}
				if (b12 == 0)
				{
					WorldGen.KillTile(num162, num163, flag7);
				}
				if (b12 == 1)
				{
					WorldGen.PlaceTile(num162, num163, num164, false, true, -1, num165);
				}
				if (b12 == 2)
				{
					WorldGen.KillWall(num162, num163, flag7);
				}
				if (b12 == 3)
				{
					WorldGen.PlaceWall(num162, num163, num164);
				}
				if (b12 == 4)
				{
					WorldGen.KillTile(num162, num163, flag7, false, true);
				}
				if (b12 == 5)
				{
					WorldGen.PlaceWire(num162, num163);
				}
				if (b12 == 6)
				{
					WorldGen.KillWire(num162, num163);
				}
				if (b12 == 7)
				{
					WorldGen.PoundTile(num162, num163);
				}
				if (b12 == 8)
				{
					WorldGen.PlaceActuator(num162, num163);
				}
				if (b12 == 9)
				{
					WorldGen.KillActuator(num162, num163);
				}
				if (b12 == 10)
				{
					WorldGen.PlaceWire2(num162, num163);
				}
				if (b12 == 11)
				{
					WorldGen.KillWire2(num162, num163);
				}
				if (b12 == 12)
				{
					WorldGen.PlaceWire3(num162, num163);
				}
				if (b12 == 13)
				{
					WorldGen.KillWire3(num162, num163);
				}
				if (b12 == 14)
				{
					WorldGen.SlopeTile(num162, num163, num164);
				}
				if (b12 == 15)
				{
					Minecart.FrameTrack(num162, num163, true);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(17, -1, whoAmI, "", b12, num162, num163, num164, num165);
					if (b12 == 1 && num164 == 53)
					{
						NetMessage.SendTileSquare(-1, num162, num163, 1);
					}
				}
				break;
			}
			case 18:
				if (Main.netMode == 1)
				{
					Main.dayTime = (reader.ReadByte() == 1);
					Main.time = reader.ReadInt32();
					Main.sunModY = reader.ReadInt16();
					Main.moonModY = reader.ReadInt16();
				}
				break;
			case 19:
			{
				byte b3 = reader.ReadByte();
				int num20 = reader.ReadInt16();
				int num21 = reader.ReadInt16();
				if (WorldGen.InWorld(num20, num21, 3))
				{
					int num22 = (reader.ReadByte() != 0) ? 1 : (-1);
					switch (b3)
					{
					case 0:
						WorldGen.OpenDoor(num20, num21, num22);
						break;
					case 1:
						WorldGen.CloseDoor(num20, num21, true);
						break;
					case 2:
						WorldGen.ShiftTrapdoor(num20, num21, num22 == 1, 1);
						break;
					case 3:
						WorldGen.ShiftTrapdoor(num20, num21, num22 == 1, 0);
						break;
					case 4:
						WorldGen.ShiftTallGate(num20, num21, false);
						break;
					case 5:
						WorldGen.ShiftTallGate(num20, num21, true);
						break;
					}
					if (Main.netMode == 2)
					{
						NetMessage.SendData(19, -1, whoAmI, "", b3, num20, num21, (num22 == 1) ? 1 : 0);
					}
				}
				break;
			}
			case 20:
			{
				short num187 = reader.ReadInt16();
				int num188 = reader.ReadInt16();
				int num189 = reader.ReadInt16();
				if (!WorldGen.InWorld(num188, num189, 3))
				{
					break;
				}
				BitsByte bitsByte8 = (byte)0;
				BitsByte bitsByte9 = (byte)0;
				Tile tile2 = null;
				for (int num190 = num188; num190 < num188 + num187; num190++)
				{
					for (int num191 = num189; num191 < num189 + num187; num191++)
					{
						if (Main.tile[num190, num191] == null)
						{
							Main.tile[num190, num191] = new Tile();
						}
						tile2 = Main.tile[num190, num191];
						bool flag8 = tile2.active();
						bitsByte8 = reader.ReadByte();
						bitsByte9 = reader.ReadByte();
						tile2.active(bitsByte8[0]);
						tile2.wall = (byte)(bitsByte8[2] ? 1 : 0);
						bool flag9 = bitsByte8[3];
						if (Main.netMode != 2)
						{
							tile2.liquid = (byte)(flag9 ? 1 : 0);
						}
						tile2.wire(bitsByte8[4]);
						tile2.halfBrick(bitsByte8[5]);
						tile2.actuator(bitsByte8[6]);
						tile2.inActive(bitsByte8[7]);
						tile2.wire2(bitsByte9[0]);
						tile2.wire3(bitsByte9[1]);
						if (bitsByte9[2])
						{
							tile2.color(reader.ReadByte());
						}
						if (bitsByte9[3])
						{
							tile2.wallColor(reader.ReadByte());
						}
						if (tile2.active())
						{
							int type10 = tile2.type;
							tile2.type = reader.ReadUInt16();
							if (Main.tileFrameImportant[tile2.type])
							{
								tile2.frameX = reader.ReadInt16();
								tile2.frameY = reader.ReadInt16();
							}
							else if (!flag8 || tile2.type != type10)
							{
								tile2.frameX = -1;
								tile2.frameY = -1;
							}
							byte b15 = 0;
							if (bitsByte9[4])
							{
								b15 = (byte)(b15 + 1);
							}
							if (bitsByte9[5])
							{
								b15 = (byte)(b15 + 2);
							}
							if (bitsByte9[6])
							{
								b15 = (byte)(b15 + 4);
							}
							tile2.slope(b15);
						}
						if (tile2.wall > 0)
						{
							tile2.wall = reader.ReadByte();
						}
						if (flag9)
						{
							tile2.liquid = reader.ReadByte();
							tile2.liquidType(reader.ReadByte());
						}
					}
				}
				WorldGen.RangeFrame(num188, num189, num188 + num187, num189 + num187);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(b, -1, whoAmI, "", num187, num188, num189);
				}
				break;
			}
			case 21:
			case 90:
			{
				int num105 = reader.ReadInt16();
				Vector2 position2 = reader.ReadVector2();
				Vector2 velocity3 = reader.ReadVector2();
				int stack3 = reader.ReadInt16();
				int pre3 = reader.ReadByte();
				int num106 = reader.ReadByte();
				int num107 = reader.ReadInt16();
				if (Main.netMode == 1)
				{
					if (num107 == 0)
					{
						Main.item[num105].active = false;
						break;
					}
					int num108 = num105;
					Item ıtem = Main.item[num108];
					bool newAndShiny = (ıtem.newAndShiny || ıtem.netID != num107) && ItemSlot.Options.HighlightNewItems && (num107 < 0 || num107 >= 3602 || !ItemID.Sets.NeverShiny[num107]);
					ıtem.netDefaults(num107);
					ıtem.newAndShiny = newAndShiny;
					ıtem.Prefix(pre3);
					ıtem.stack = stack3;
					ıtem.position = position2;
					ıtem.velocity = velocity3;
					ıtem.active = true;
					if (b == 90)
					{
						ıtem.instanced = true;
						ıtem.owner = Main.myPlayer;
						ıtem.keepTime = 600;
					}
					ıtem.wet = Collision.WetCollision(ıtem.position, ıtem.width, ıtem.height);
				}
				else
				{
					if (Main.itemLockoutTime[num105] > 0)
					{
						break;
					}
					if (num107 == 0)
					{
						if (num105 < 400)
						{
							Main.item[num105].active = false;
							NetMessage.SendData(21, -1, -1, "", num105);
						}
						break;
					}
					bool flag4 = false;
					if (num105 == 400)
					{
						flag4 = true;
					}
					if (flag4)
					{
						Item ıtem2 = new Item();
						ıtem2.netDefaults(num107);
						num105 = Item.NewItem((int)position2.X, (int)position2.Y, ıtem2.width, ıtem2.height, ıtem2.type, stack3, true);
					}
					Item ıtem3 = Main.item[num105];
					ıtem3.netDefaults(num107);
					ıtem3.Prefix(pre3);
					ıtem3.stack = stack3;
					ıtem3.position = position2;
					ıtem3.velocity = velocity3;
					ıtem3.active = true;
					ıtem3.owner = Main.myPlayer;
					if (flag4)
					{
						NetMessage.SendData(21, -1, -1, "", num105);
						if (num106 == 0)
						{
							Main.item[num105].ownIgnore = whoAmI;
							Main.item[num105].ownTime = 100;
						}
						Main.item[num105].FindOwner(num105);
					}
					else
					{
						NetMessage.SendData(21, -1, whoAmI, "", num105);
					}
				}
				break;
			}
			case 22:
			{
				int num9 = reader.ReadInt16();
				int num10 = reader.ReadByte();
				if (Main.netMode != 2 || Main.item[num9].owner == whoAmI)
				{
					Main.item[num9].owner = num10;
					if (num10 == Main.myPlayer)
					{
						Main.item[num9].keepTime = 15;
					}
					else
					{
						Main.item[num9].keepTime = 0;
					}
					if (Main.netMode == 2)
					{
						Main.item[num9].owner = 16;
						Main.item[num9].keepTime = 15;
						NetMessage.SendData(22, -1, -1, "", num9);
					}
				}
				break;
			}
			case 23:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num114 = reader.ReadInt16();
				Vector2 position3 = reader.ReadVector2();
				Vector2 velocity4 = reader.ReadVector2();
				int target = reader.ReadByte();
				BitsByte bitsByte3 = reader.ReadByte();
				float[] array = new float[NPC.maxAI];
				for (int num115 = 0; num115 < NPC.maxAI; num115++)
				{
					if (bitsByte3[num115 + 2])
					{
						array[num115] = reader.ReadSingle();
					}
					else
					{
						array[num115] = 0f;
					}
				}
				int num116 = reader.ReadInt16();
				int num117 = 0;
				if (!bitsByte3[7])
				{
					switch (reader.ReadByte())
					{
					case 2:
						num117 = reader.ReadInt16();
						break;
					case 4:
						num117 = reader.ReadInt32();
						break;
					default:
						num117 = reader.ReadSByte();
						break;
					}
				}
				int num118 = -1;
				NPC nPC3 = Main.npc[num114];
				if (!nPC3.active || nPC3.netID != num116)
				{
					if (nPC3.active)
					{
						num118 = nPC3.type;
					}
					nPC3.active = true;
					nPC3.netDefaults(num116);
				}
				nPC3.position = position3;
				nPC3.velocity = velocity4;
				nPC3.target = target;
				nPC3.direction = (bitsByte3[0] ? 1 : (-1));
				nPC3.directionY = (bitsByte3[1] ? 1 : (-1));
				nPC3.spriteDirection = (bitsByte3[6] ? 1 : (-1));
				if (bitsByte3[7])
				{
					num117 = (nPC3.life = nPC3.lifeMax);
				}
				else
				{
					nPC3.life = num117;
				}
				if (num117 <= 0)
				{
					nPC3.active = false;
				}
				for (int num119 = 0; num119 < NPC.maxAI; num119++)
				{
					nPC3.ai[num119] = array[num119];
				}
				if (num118 > -1 && num118 != nPC3.type)
				{
					nPC3.TransformVisuals(num118, nPC3.type);
				}
				if (num116 == 262)
				{
					NPC.plantBoss = num114;
				}
				if (num116 == 245)
				{
					NPC.golemBoss = num114;
				}
				if (Main.npcCatchable[nPC3.type])
				{
					nPC3.releaseOwner = reader.ReadByte();
				}
				break;
			}
			case 24:
			{
				int num152 = reader.ReadInt16();
				int num153 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num153 = whoAmI;
				}
				Player player13 = Main.player[num153];
				Main.npc[num152].StrikeNPC(player13.inventory[player13.selectedItem].damage, player13.inventory[player13.selectedItem].knockBack, player13.direction);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(24, -1, whoAmI, "", num152, num153);
					NetMessage.SendData(23, -1, -1, "", num152);
				}
				break;
			}
			case 25:
			{
				int num62 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num62 = whoAmI;
				}
				Color color2 = reader.ReadRGB();
				if (Main.netMode == 2)
				{
					color2 = new Color(255, 255, 255);
				}
				string text2 = reader.ReadString();
				if (Main.netMode == 1)
				{
					string newText = text2;
					if (num62 < 16)
					{
						newText = NameTagHandler.GenerateTag(Main.player[num62].name) + " " + text2;
						Main.player[num62].chatOverhead.NewMessage(text2, Main.chatLength / 2);
					}
					Main.NewText(newText, color2.R, color2.G, color2.B);
				}
				else
				{
					if (Main.netMode != 2)
					{
						break;
					}
					string text3 = text2.ToLower();
					if (text3 == Lang.mp[6].Value || text3 == Lang.mp[21].Value)
					{
						string text4 = "";
						for (int num63 = 0; num63 < 16; num63++)
						{
							if (Main.player[num63].active)
							{
								text4 = ((!(text4 == "")) ? (text4 + ", " + Main.player[num63].name) : Main.player[num63].name);
							}
						}
						NetMessage.SendData(25, whoAmI, -1, Language.GetTextValue("Game.JoinGreeting", text4), 16, 255f, 240f, 20f);
					}
					else if (text3.StartsWith("/me "))
					{
						NetMessage.SendData(25, -1, -1, "*" + Main.player[whoAmI].name + " " + text2.Substring(4), 16, 200f, 100f);
					}
					else if (text3 == Lang.mp[8].Value)
					{
						NetMessage.SendData(25, -1, -1, string.Concat("*", Main.player[whoAmI].name, " ", Lang.mp[9], " ", Main.rand.Next(1, 101)), 16, 255f, 240f, 20f);
					}
					else if (text3.StartsWith("/p "))
					{
						int team = Main.player[whoAmI].team;
						color2 = Main.teamColor[team];
						if (team != 0)
						{
							for (int num64 = 0; num64 < 16; num64++)
							{
								if (Main.player[num64].team == team)
								{
									NetMessage.SendData(25, num64, -1, text2.Substring(3), num62, (int)color2.R, (int)color2.G, (int)color2.B);
								}
							}
						}
						else
						{
							NetMessage.SendData(25, whoAmI, -1, Lang.mp[10].Value, 16, 255f, 240f, 20f);
						}
					}
					else
					{
						if (Main.player[whoAmI].difficulty == 2)
						{
							color2 = Main.hcColor;
						}
						else if (Main.player[whoAmI].difficulty == 1)
						{
							color2 = Main.mcColor;
						}
						NetMessage.SendData(25, -1, -1, text2, num62, (int)color2.R, (int)color2.G, (int)color2.B);
						if (Main.dedServ)
						{
							Console.WriteLine("<" + Main.player[whoAmI].name + "> " + text2);
						}
					}
				}
				break;
			}
			case 26:
			{
				int num101 = reader.ReadByte();
				if (Main.netMode != 2 || whoAmI == num101 || (Main.player[num101].hostile && Main.player[whoAmI].hostile))
				{
					int num102 = reader.ReadByte() - 1;
					int num103 = reader.ReadInt16();
					string text6 = reader.ReadString();
					BitsByte bitsByte2 = reader.ReadByte();
					bool flag2 = bitsByte2[0];
					bool flag3 = bitsByte2[1];
					int num104 = (!bitsByte2[2]) ? (-1) : 0;
					if (bitsByte2[3])
					{
						num104 = 1;
					}
					Main.player[num101].Hurt(num103, num102, flag2, true, text6, flag3, num104);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(26, -1, whoAmI, text6, num101, num102, num103, flag2 ? 1 : 0, flag3 ? 1 : 0, num104);
					}
				}
				break;
			}
			case 27:
			{
				int num129 = reader.ReadInt16();
				Vector2 position4 = reader.ReadVector2();
				Vector2 velocity5 = reader.ReadVector2();
				float knockBack = reader.ReadSingle();
				int damage = reader.ReadInt16();
				int num130 = reader.ReadByte();
				int num131 = reader.ReadInt16();
				BitsByte bitsByte4 = reader.ReadByte();
				float[] array2 = new float[Projectile.maxAI];
				for (int num132 = 0; num132 < Projectile.maxAI; num132++)
				{
					if (bitsByte4[num132])
					{
						array2[num132] = reader.ReadSingle();
					}
					else
					{
						array2[num132] = 0f;
					}
				}
				int num133 = bitsByte4[Projectile.maxAI] ? reader.ReadInt16() : (-1);
				if (num133 >= 1000)
				{
					num133 = -1;
				}
				if (Main.netMode == 2)
				{
					num130 = whoAmI;
					if (Main.projHostile[num131])
					{
						break;
					}
				}
				int num134 = 1000;
				for (int num135 = 0; num135 < 1000; num135++)
				{
					if (Main.projectile[num135].owner == num130 && Main.projectile[num135].identity == num129 && Main.projectile[num135].active)
					{
						num134 = num135;
						break;
					}
				}
				if (num134 == 1000)
				{
					for (int num136 = 0; num136 < 1000; num136++)
					{
						if (!Main.projectile[num136].active)
						{
							num134 = num136;
							break;
						}
					}
				}
				Projectile projectile2 = Main.projectile[num134];
				if (!projectile2.active || projectile2.type != num131)
				{
					projectile2.SetDefaults(num131);
					if (Main.netMode == 2)
					{
						Netplay.Clients[whoAmI].SpamProjectile += 1f;
					}
				}
				projectile2.identity = num129;
				projectile2.position = position4;
				projectile2.velocity = velocity5;
				projectile2.type = num131;
				projectile2.damage = damage;
				projectile2.knockBack = knockBack;
				projectile2.owner = num130;
				for (int num137 = 0; num137 < Projectile.maxAI; num137++)
				{
					projectile2.ai[num137] = array2[num137];
				}
				if (num133 >= 0)
				{
					projectile2.projUUID = num133;
					Main.projectileIdentity[num130, num133] = num134;
				}
				projectile2.ProjectileFixDesperation();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(27, -1, whoAmI, "", num134);
				}
				break;
			}
			case 28:
			{
				int num179 = reader.ReadInt16();
				int num180 = reader.ReadInt16();
				float num181 = reader.ReadSingle();
				int num182 = reader.ReadByte() - 1;
				byte b13 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					if (num180 < 0)
					{
						num180 = 0;
					}
					Main.npc[num179].PlayerInteraction(whoAmI);
				}
				if (num180 >= 0)
				{
					Main.npc[num179].StrikeNPC(num180, num181, num182, b13 == 1, false, true);
				}
				else
				{
					Main.npc[num179].life = 0;
					Main.npc[num179].HitEffect();
					Main.npc[num179].active = false;
				}
				if (Main.netMode != 2)
				{
					break;
				}
				NetMessage.SendData(28, -1, whoAmI, "", num179, num180, num181, num182, b13);
				if (Main.npc[num179].life <= 0)
				{
					NetMessage.SendData(23, -1, -1, "", num179);
				}
				else
				{
					Main.npc[num179].netUpdate = true;
				}
				if (Main.npc[num179].realLife >= 0)
				{
					if (Main.npc[Main.npc[num179].realLife].life <= 0)
					{
						NetMessage.SendData(23, -1, -1, "", Main.npc[num179].realLife);
					}
					else
					{
						Main.npc[Main.npc[num179].realLife].netUpdate = true;
					}
				}
				break;
			}
			case 29:
			{
				int num149 = reader.ReadInt16();
				int num150 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num150 = whoAmI;
				}
				for (int num151 = 0; num151 < 1000; num151++)
				{
					if (Main.projectile[num151].owner == num150 && Main.projectile[num151].identity == num149 && Main.projectile[num151].active)
					{
						Main.projectile[num151].Kill();
						break;
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(29, -1, whoAmI, "", num149, num150);
				}
				break;
			}
			case 30:
			{
				int num144 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num144 = whoAmI;
				}
				bool flag5 = reader.ReadBoolean();
				Main.player[num144].hostile = flag5;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(30, -1, whoAmI, "", num144);
					LocalizedText localizedText2 = Lang.mp[flag5 ? 11 : 12];
					Color color4 = Main.teamColor[Main.player[num144].team];
					NetMessage.SendData(25, -1, -1, localizedText2.Format(Main.player[num144].name), 16, (int)color4.R, (int)color4.G, (int)color4.B);
				}
				break;
			}
			case 31:
			{
				if (Main.netMode != 2)
				{
					break;
				}
				int x2 = reader.ReadInt16();
				int y2 = reader.ReadInt16();
				int num57 = Chest.FindChest(x2, y2);
				if (num57 > -1 && Chest.UsingChest(num57) == -1)
				{
					for (int num58 = 0; num58 < 40; num58++)
					{
						NetMessage.SendData(32, whoAmI, -1, "", num57, num58);
					}
					NetMessage.SendData(33, whoAmI, -1, "", num57);
					Main.player[whoAmI].chest = num57;
					if (Main.myPlayer == whoAmI)
					{
						Main.recBigList = false;
					}
					NetMessage.SendData(80, -1, whoAmI, "", whoAmI, num57);
				}
				break;
			}
			case 32:
			{
				int num17 = reader.ReadInt16();
				int num18 = reader.ReadByte();
				int stack2 = reader.ReadInt16();
				int pre2 = reader.ReadByte();
				int type3 = reader.ReadInt16();
				if (Main.chest[num17] == null)
				{
					Main.chest[num17] = new Chest();
				}
				if (Main.chest[num17].item[num18] == null)
				{
					Main.chest[num17].item[num18] = new Item();
				}
				Main.chest[num17].item[num18].netDefaults(type3);
				Main.chest[num17].item[num18].Prefix(pre2);
				Main.chest[num17].item[num18].stack = stack2;
				Recipe.FindRecipes();
				break;
			}
			case 33:
			{
				int num138 = reader.ReadInt16();
				int num139 = reader.ReadInt16();
				int num140 = reader.ReadInt16();
				int num141 = reader.ReadByte();
				string text7 = string.Empty;
				if (num141 != 0)
				{
					if (num141 <= 20)
					{
						text7 = reader.ReadString();
					}
					else if (num141 != 255)
					{
						num141 = 0;
					}
				}
				if (Main.netMode == 1)
				{
					Player player10 = Main.player[Main.myPlayer];
					if (player10.chest == -1)
					{
						Main.playerInventory = true;
						Main.PlaySound(10);
					}
					else if (player10.chest != num138 && num138 != -1)
					{
						Main.playerInventory = true;
						Main.PlaySound(12);
						Main.recBigList = false;
					}
					else if (player10.chest != -1 && num138 == -1)
					{
						Main.PlaySound(11);
						Main.recBigList = false;
					}
					player10.chest = num138;
					player10.chestX = num139;
					player10.chestY = num140;
					Recipe.FindRecipes();
					if (Main.tile[num139, num140].frameX >= 36 && Main.tile[num139, num140].frameX < 72)
					{
						AchievementsHelper.HandleSpecialEvent(Main.player[Main.myPlayer], 16);
					}
				}
				else
				{
					if (num141 != 0)
					{
						int chest = Main.player[whoAmI].chest;
						Chest chest2 = Main.chest[chest];
						chest2.name = text7;
						NetMessage.SendData(69, -1, whoAmI, text7, chest, chest2.x, chest2.y);
					}
					Main.player[whoAmI].chest = num138;
					Recipe.FindRecipes();
					NetMessage.SendData(80, -1, whoAmI, "", whoAmI, num138);
				}
				break;
			}
			case 34:
			{
				byte b9 = reader.ReadByte();
				int num83 = reader.ReadInt16();
				int num84 = reader.ReadInt16();
				int num85 = reader.ReadInt16();
				if (Main.netMode == 2)
				{
					switch (b9)
					{
					case 0:
					{
						int num86 = WorldGen.PlaceChest(num83, num84, 21, false, num85);
						if (num86 == -1)
						{
							NetMessage.SendData(34, whoAmI, -1, "", b9, num83, num84, num85, num86);
							Item.NewItem(num83 * 16, num84 * 16, 32, 32, Chest.chestItemSpawn[num85], 1, true);
						}
						else
						{
							NetMessage.SendData(34, -1, -1, "", b9, num83, num84, num85, num86);
						}
						break;
					}
					case 2:
					{
						int num87 = WorldGen.PlaceChest(num83, num84, 88, false, num85);
						if (num87 == -1)
						{
							NetMessage.SendData(34, whoAmI, -1, "", b9, num83, num84, num85, num87);
							Item.NewItem(num83 * 16, num84 * 16, 32, 32, Chest.dresserItemSpawn[num85], 1, true);
						}
						else
						{
							NetMessage.SendData(34, -1, -1, "", b9, num83, num84, num85, num87);
						}
						break;
					}
					default:
					{
						Tile tile = Main.tile[num83, num84];
						if (tile.type == 21 && b9 == 1)
						{
							if (tile.frameX % 36 != 0)
							{
								num83--;
							}
							if (tile.frameY % 36 != 0)
							{
								num84--;
							}
							int number = Chest.FindChest(num83, num84);
							WorldGen.KillTile(num83, num84);
							if (!tile.active())
							{
								NetMessage.SendData(34, -1, -1, "", b9, num83, num84, 0f, number);
							}
						}
						else if (tile.type == 88 && b9 == 3)
						{
							num83 -= tile.frameX % 54 / 18;
							if (tile.frameY % 36 != 0)
							{
								num84--;
							}
							int number2 = Chest.FindChest(num83, num84);
							WorldGen.KillTile(num83, num84);
							if (!tile.active())
							{
								NetMessage.SendData(34, -1, -1, "", b9, num83, num84, 0f, number2);
							}
						}
						break;
					}
					}
					break;
				}
				int num88 = reader.ReadInt16();
				switch (b9)
				{
				case 0:
					if (num88 == -1)
					{
						WorldGen.KillTile(num83, num84);
					}
					else
					{
						WorldGen.PlaceChestDirect(num83, num84, 21, num85, num88);
					}
					break;
				case 2:
					if (num88 == -1)
					{
						WorldGen.KillTile(num83, num84);
					}
					else
					{
						WorldGen.PlaceDresserDirect(num83, num84, 88, num85, num88);
					}
					break;
				default:
					Chest.DestroyChestDirect(num83, num84, num88);
					WorldGen.KillTile(num83, num84);
					break;
				}
				break;
			}
			case 35:
			{
				int num70 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num70 = whoAmI;
				}
				int num71 = reader.ReadInt16();
				if (num70 != Main.myPlayer || Main.ServerSideCharacter)
				{
					Main.player[num70].HealEffect(num71);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(35, -1, whoAmI, "", num70, num71);
				}
				break;
			}
			case 36:
			{
				int num19 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num19 = whoAmI;
				}
				Player player4 = Main.player[num19];
				player4.zone1 = reader.ReadByte();
				player4.zone2 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(36, -1, whoAmI, "", num19);
				}
				break;
			}
			case 37:
				if (Main.netMode == 1)
				{
					if (Main.autoPass)
					{
						NetMessage.SendData(38, -1, -1, Netplay.ServerPassword);
						Main.autoPass = false;
					}
					else
					{
						Netplay.ServerPassword = "";
						Main.menuMode = 31;
					}
				}
				break;
			case 38:
				if (Main.netMode == 2)
				{
					string a2 = reader.ReadString();
					if (a2 == Netplay.ServerPassword)
					{
						Netplay.Clients[whoAmI].State = 1;
						NetMessage.SendData(3, whoAmI);
					}
					else
					{
						NetMessage.SendData(2, whoAmI, -1, Lang.mp[1].Value);
					}
				}
				break;
			case 39:
				if (Main.netMode == 1)
				{
					int num178 = reader.ReadInt16();
					Main.item[num178].owner = 16;
					NetMessage.SendData(22, -1, -1, "", num178);
				}
				break;
			case 40:
			{
				int num161 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num161 = whoAmI;
				}
				int talkNPC = reader.ReadInt16();
				Main.player[num161].talkNPC = talkNPC;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(40, -1, whoAmI, "", num161);
				}
				break;
			}
			case 41:
			{
				int num177 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num177 = whoAmI;
				}
				Player player15 = Main.player[num177];
				float itemRotation = reader.ReadSingle();
				int itemAnimation = reader.ReadInt16();
				player15.itemRotation = itemRotation;
				player15.itemAnimation = itemAnimation;
				player15.channel = player15.inventory[player15.selectedItem].channel;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(41, -1, whoAmI, "", num177);
				}
				break;
			}
			case 42:
			{
				int num166 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num166 = whoAmI;
				}
				else if (Main.myPlayer == num166 && !Main.ServerSideCharacter)
				{
					break;
				}
				int statMana = reader.ReadInt16();
				int statManaMax = reader.ReadInt16();
				Main.player[num166].statMana = statMana;
				Main.player[num166].statManaMax = statManaMax;
				break;
			}
			case 43:
			{
				int num127 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num127 = whoAmI;
				}
				int num128 = reader.ReadInt16();
				if (num127 != Main.myPlayer)
				{
					Main.player[num127].ManaEffect(num128);
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(43, -1, whoAmI, "", num127, num128);
				}
				break;
			}
			case 44:
			{
				int num77 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num77 = whoAmI;
				}
				int num78 = reader.ReadByte() - 1;
				int num79 = reader.ReadInt16();
				byte b8 = reader.ReadByte();
				string text5 = reader.ReadString();
				Main.player[num77].KillMe(num79, num78, b8 == 1, text5);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(44, -1, whoAmI, text5, num77, num78, num79, (int)b8);
				}
				break;
			}
			case 45:
			{
				int num66 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num66 = whoAmI;
				}
				int num67 = reader.ReadByte();
				Player player5 = Main.player[num66];
				int team2 = player5.team;
				player5.team = num67;
				Color color3 = Main.teamColor[num67];
				if (Main.netMode != 2)
				{
					break;
				}
				NetMessage.SendData(45, -1, whoAmI, "", num66);
				LocalizedText localizedText = Lang.mp[13 + num67];
				if (num67 == 5)
				{
					localizedText = Lang.mp[22];
				}
				for (int num68 = 0; num68 < 16; num68++)
				{
					if (num68 == whoAmI || (team2 > 0 && Main.player[num68].team == team2) || (num67 > 0 && Main.player[num68].team == num67))
					{
						NetMessage.SendData(25, num68, -1, localizedText.Format(player5.name), 16, (int)color3.R, (int)color3.G, (int)color3.B);
					}
				}
				break;
			}
			case 46:
				if (Main.netMode == 2)
				{
					int i2 = reader.ReadInt16();
					int j2 = reader.ReadInt16();
					int num24 = Sign.ReadSign(i2, j2);
					if (num24 >= 0)
					{
						NetMessage.SendData(47, whoAmI, -1, "", num24, whoAmI);
					}
				}
				break;
			case 47:
			{
				int num205 = reader.ReadInt16();
				int x7 = reader.ReadInt16();
				int y7 = reader.ReadInt16();
				string text8 = reader.ReadString();
				string a3 = null;
				if (Main.sign[num205] != null)
				{
					a3 = Main.sign[num205].text;
				}
				Main.sign[num205] = new Sign();
				Main.sign[num205].x = x7;
				Main.sign[num205].y = y7;
				Sign.TextSign(num205, text8);
				int num206 = reader.ReadByte();
				if (Main.netMode == 2 && a3 != text8)
				{
					num206 = whoAmI;
					NetMessage.SendData(47, -1, whoAmI, "", num205, num206);
				}
				if (Main.netMode == 1 && num206 == Main.myPlayer && Main.sign[num205] != null)
				{
					Main.playerInventory = false;
					Main.player[Main.myPlayer].talkNPC = -1;
					Main.npcChatCornerItem = 0;
					Main.editSign = false;
					Main.PlaySound(10);
					Main.player[Main.myPlayer].sign = num205;
					Main.npcChatText = Main.sign[num205].text;
				}
				break;
			}
			case 48:
			{
				int num167 = reader.ReadInt16();
				int num168 = reader.ReadInt16();
				byte liquid = reader.ReadByte();
				byte liquidType = reader.ReadByte();
				if (Main.netMode == 2 && Netplay.spamCheck)
				{
					int num169 = whoAmI;
					int num170 = (int)(Main.player[num169].position.X + (float)(Main.player[num169].width / 2));
					int num171 = (int)(Main.player[num169].position.Y + (float)(Main.player[num169].height / 2));
					int num172 = 10;
					int num173 = num170 - num172;
					int num174 = num170 + num172;
					int num175 = num171 - num172;
					int num176 = num171 + num172;
					if (num167 < num173 || num167 > num174 || num168 < num175 || num168 > num176)
					{
						NetMessage.BootPlayer(whoAmI, "Cheating attempt detected: Liquid spam");
						break;
					}
				}
				if (Main.tile[num167, num168] == null)
				{
					Main.tile[num167, num168] = new Tile();
				}
				lock (Main.tile[num167, num168])
				{
					Main.tile[num167, num168].liquid = liquid;
					Main.tile[num167, num168].liquidType(liquidType);
					if (Main.netMode == 2)
					{
						WorldGen.SquareTileFrame(num167, num168);
					}
				}
				break;
			}
			case 49:
				if (Netplay.Connection.State == 6)
				{
					Netplay.Connection.State = 10;
					Main.ActivePlayerFileData.StartPlayTimer();
					Player.EnterWorld(Main.player[Main.myPlayer]);
					Main.player[Main.myPlayer].Spawn();
				}
				break;
			case 50:
			{
				int num145 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num145 = whoAmI;
				}
				else if (num145 == Main.myPlayer && !Main.ServerSideCharacter)
				{
					break;
				}
				Player player11 = Main.player[num145];
				for (int num146 = 0; num146 < 22; num146++)
				{
					player11.buffType[num146] = reader.ReadByte();
					if (player11.buffType[num146] > 0)
					{
						player11.buffTime[num146] = 60;
					}
					else
					{
						player11.buffTime[num146] = 0;
					}
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(50, -1, whoAmI, "", num145);
				}
				break;
			}
			case 51:
			{
				byte b10 = reader.ReadByte();
				byte b11 = reader.ReadByte();
				switch (b11)
				{
				case 1:
					NPC.SpawnSkeletron();
					break;
				case 2:
					if (Main.netMode == 2)
					{
						NetMessage.SendData(51, -1, whoAmI, "", b10, (int)b11);
					}
					else
					{
						Main.PlaySound(2, (int)Main.player[b10].position.X, (int)Main.player[b10].position.Y);
					}
					break;
				case 3:
					if (Main.netMode == 2)
					{
						Main.Sundialing();
					}
					break;
				case 4:
					Main.npc[b10].BigMimicSpawnSmoke();
					break;
				}
				break;
			}
			case 52:
			{
				int num120 = reader.ReadByte();
				int num121 = reader.ReadInt16();
				int num122 = reader.ReadInt16();
				if (num120 == 1)
				{
					Chest.Unlock(num121, num122);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(52, -1, whoAmI, "", 0, num120, num121, num122);
						NetMessage.SendTileSquare(-1, num121, num122, 2);
					}
				}
				if (num120 == 2)
				{
					WorldGen.UnlockDoor(num121, num122);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(52, -1, whoAmI, "", 0, num120, num121, num122);
						NetMessage.SendTileSquare(-1, num121, num122, 2);
					}
				}
				break;
			}
			case 53:
			{
				int num113 = reader.ReadInt16();
				int type6 = reader.ReadByte();
				int time = reader.ReadInt16();
				Main.npc[num113].AddBuff(type6, time, true);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(54, -1, -1, "", num113);
				}
				break;
			}
			case 54:
				if (Main.netMode == 1)
				{
					int num98 = reader.ReadInt16();
					NPC nPC2 = Main.npc[num98];
					for (int num99 = 0; num99 < 5; num99++)
					{
						nPC2.buffType[num99] = reader.ReadByte();
						nPC2.buffTime[num99] = reader.ReadInt16();
					}
				}
				break;
			case 55:
			{
				int num72 = reader.ReadByte();
				int num73 = reader.ReadByte();
				int num74 = reader.ReadInt16();
				if (Main.netMode != 2 || num72 == whoAmI || Main.pvpBuff[num73])
				{
					if (Main.netMode == 1 && num72 == Main.myPlayer)
					{
						Main.player[num72].AddBuff(num73, num74);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.SendData(55, num72, -1, "", num72, num73, num74);
					}
				}
				break;
			}
			case 56:
			{
				int num25 = reader.ReadInt16();
				if (num25 >= 0 && num25 < 200)
				{
					string givenName = reader.ReadString();
					if (Main.netMode == 1)
					{
						Main.npc[num25].GivenName = givenName;
					}
					else if (Main.netMode == 2)
					{
						NetMessage.SendData(56, whoAmI, -1, Main.npc[num25].GivenName, num25);
					}
				}
				break;
			}
			case 57:
				if (Main.netMode == 1)
				{
					WorldGen.tGood = reader.ReadByte();
					WorldGen.tEvil = reader.ReadByte();
					WorldGen.tBlood = reader.ReadByte();
				}
				break;
			case 58:
			{
				int num5 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num5 = whoAmI;
				}
				float num6 = reader.ReadSingle();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(58, -1, whoAmI, "", whoAmI, num6);
					break;
				}
				Player player = Main.player[num5];
				Main.harpNote = num6;
				int style = 26;
				if (player.inventory[player.selectedItem].type == 507)
				{
					style = 35;
				}
				Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, style);
				break;
			}
			case 59:
			{
				int num2 = reader.ReadInt16();
				int num3 = reader.ReadInt16();
				Wiring.HitSwitch(num2, num3);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(59, -1, whoAmI, "", num2, num3);
				}
				break;
			}
			case 60:
			{
				int num184 = reader.ReadInt16();
				int num185 = reader.ReadInt16();
				int num186 = reader.ReadInt16();
				byte b14 = reader.ReadByte();
				if (num184 >= 200)
				{
					NetMessage.BootPlayer(whoAmI, "cheating attempt detected: Invalid kick-out");
				}
				else if (Main.netMode == 1)
				{
					Main.npc[num184].homeless = (b14 == 1);
					Main.npc[num184].homeTileX = num185;
					Main.npc[num184].homeTileY = num186;
				}
				else if (b14 == 0)
				{
					WorldGen.kickOut(num184);
				}
				else
				{
					WorldGen.moveRoom(num185, num186, num184);
				}
				break;
			}
			case 61:
			{
				int plr = reader.ReadInt16();
				int num75 = reader.ReadInt16();
				if (Main.netMode != 2)
				{
					break;
				}
				if (num75 >= 0 && num75 < 540 && NPCID.Sets.MPAllowedEnemies[num75])
				{
					if (!NPC.AnyNPCs(num75))
					{
						NPC.SpawnOnPlayer(plr, num75);
					}
				}
				else if (num75 == -4)
				{
					if (!Main.dayTime)
					{
						NetMessage.SendData(25, -1, -1, Lang.misc[31].Value, 16, 50f, 255f, 130f);
						Main.startPumpkinMoon();
						NetMessage.SendData(7);
						NetMessage.SendData(78, -1, -1, "", 0, 1f, 2f, 1f);
					}
				}
				else if (num75 == -5)
				{
					if (!Main.dayTime)
					{
						NetMessage.SendData(25, -1, -1, Lang.misc[34].Value, 16, 50f, 255f, 130f);
						Main.startSnowMoon();
						NetMessage.SendData(7);
						NetMessage.SendData(78, -1, -1, "", 0, 1f, 1f, 1f);
					}
				}
				else if (num75 == -6)
				{
					if (Main.dayTime && !Main.eclipse)
					{
						NetMessage.SendData(25, -1, -1, Lang.misc[20].Value, 16, 50f, 255f, 130f);
						Main.eclipse = true;
						NetMessage.SendData(7);
					}
				}
				else if (num75 == -7)
				{
					NetMessage.SendData(25, -1, -1, "martian moon toggled", 16, 50f, 255f, 130f);
					Main.invasionDelay = 0;
					Main.StartInvasion(4);
					NetMessage.SendData(7);
					NetMessage.SendData(78, -1, -1, "", 0, 1f, Main.invasionType + 2);
				}
				else if (num75 == -8)
				{
					if (NPC.downedGolemBoss && Main.hardMode && !NPC.AnyDanger() && !NPC.AnyoneNearCultists())
					{
						WorldGen.StartImpendingDoom();
						NetMessage.SendData(7);
					}
				}
				else if (num75 < 0)
				{
					int num76 = 1;
					if (num75 > -5)
					{
						num76 = -num75;
					}
					if (num76 > 0 && Main.invasionType == 0)
					{
						Main.invasionDelay = 0;
						Main.StartInvasion(num76);
					}
					NetMessage.SendData(78, -1, -1, "", 0, 1f, Main.invasionType + 2);
				}
				break;
			}
			case 62:
			{
				int num55 = reader.ReadByte();
				int num56 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num55 = whoAmI;
				}
				if (num56 == 1)
				{
					Main.player[num55].NinjaDodge();
				}
				if (num56 == 2)
				{
					Main.player[num55].ShadowDodge();
				}
				if (Main.netMode == 2)
				{
					NetMessage.SendData(62, -1, whoAmI, "", num55, num56);
				}
				break;
			}
			case 63:
			{
				int num51 = reader.ReadInt16();
				int num52 = reader.ReadInt16();
				byte b7 = reader.ReadByte();
				WorldGen.paintTile(num51, num52, b7);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(63, -1, whoAmI, "", num51, num52, (int)b7);
				}
				break;
			}
			case 64:
			{
				int num26 = reader.ReadInt16();
				int num27 = reader.ReadInt16();
				byte b5 = reader.ReadByte();
				WorldGen.paintWall(num26, num27, b5);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(64, -1, whoAmI, "", num26, num27, (int)b5);
				}
				break;
			}
			case 65:
			{
				BitsByte bitsByte17 = reader.ReadByte();
				int num207 = reader.ReadInt16();
				if (Main.netMode == 2)
				{
					num207 = whoAmI;
				}
				Vector2 vector2 = reader.ReadVector2();
				int num208 = 0;
				int num209 = 0;
				if (bitsByte17[0])
				{
					num208++;
				}
				if (bitsByte17[1])
				{
					num208 += 2;
				}
				if (bitsByte17[2])
				{
					num209++;
				}
				if (bitsByte17[3])
				{
					num209 += 2;
				}
				switch (num208)
				{
				case 0:
					Main.player[num207].Teleport(vector2, num209);
					break;
				case 1:
					Main.npc[num207].Teleport(vector2, num209);
					break;
				case 2:
				{
					Main.player[num207].Teleport(vector2, num209);
					if (Main.netMode != 2)
					{
						break;
					}
					RemoteClient.CheckSection(whoAmI, vector2);
					NetMessage.SendData(65, -1, -1, "", 0, num207, vector2.X, vector2.Y, num209);
					int num210 = -1;
					float num211 = 9999f;
					for (int num212 = 0; num212 < 16; num212++)
					{
						if (Main.player[num212].active && num212 != whoAmI)
						{
							Vector2 vector3 = Main.player[num212].position - Main.player[whoAmI].position;
							if (vector3.Length() < num211)
							{
								num211 = vector3.Length();
								num210 = num212;
							}
						}
					}
					if (num210 >= 0)
					{
						NetMessage.SendData(25, -1, -1, Main.player[whoAmI].name + " has teleported to " + Main.player[num210].name, 16, 250f, 250f);
					}
					break;
				}
				}
				if (Main.netMode == 2 && num208 == 0)
				{
					NetMessage.SendData(65, -1, whoAmI, "", 0, num207, vector2.X, vector2.Y, num209);
				}
				break;
			}
			case 66:
			{
				int num192 = reader.ReadByte();
				int num193 = reader.ReadInt16();
				if (num193 > 0)
				{
					Player player17 = Main.player[num192];
					player17.statLife += num193;
					if (player17.statLife > player17.statLifeMax2)
					{
						player17.statLife = player17.statLifeMax2;
					}
					player17.HealEffect(num193, false);
					if (Main.netMode == 2)
					{
						NetMessage.SendData(66, -1, whoAmI, "", num192, num193);
					}
				}
				break;
			}
			case 68:
				reader.ReadString();
				break;
			case 69:
			{
				int num158 = reader.ReadInt16();
				int num159 = reader.ReadInt16();
				int num160 = reader.ReadInt16();
				if (Main.netMode == 1)
				{
					if (num158 >= 0 && num158 < 1000)
					{
						Chest chest3 = Main.chest[num158];
						if (chest3 == null)
						{
							chest3 = new Chest();
							chest3.x = num159;
							chest3.y = num160;
							Main.chest[num158] = chest3;
						}
						else if (chest3.x != num159 || chest3.y != num160)
						{
							break;
						}
						chest3.name = reader.ReadString();
					}
				}
				else
				{
					if (num158 < -1 || num158 >= 1000)
					{
						break;
					}
					if (num158 == -1)
					{
						num158 = Chest.FindChest(num159, num160);
						if (num158 == -1)
						{
							break;
						}
					}
					Chest chest4 = Main.chest[num158];
					if (chest4.x == num159 && chest4.y == num160)
					{
						NetMessage.SendData(69, whoAmI, -1, chest4.name, num158, num159, num160);
					}
				}
				break;
			}
			case 70:
				if (Main.netMode == 2)
				{
					int num147 = reader.ReadInt16();
					int who = reader.ReadByte();
					if (Main.netMode == 2)
					{
						who = whoAmI;
					}
					if (num147 < 200 && num147 >= 0)
					{
						NPC.CatchNPC(num147, who);
					}
				}
				break;
			case 71:
				if (Main.netMode == 2)
				{
					int x6 = reader.ReadInt32();
					int y6 = reader.ReadInt32();
					int type7 = reader.ReadInt16();
					byte style3 = reader.ReadByte();
					NPC.ReleaseNPC(x6, y6, type7, style3, whoAmI);
				}
				break;
			case 72:
				if (Main.netMode == 1)
				{
					for (int num142 = 0; num142 < 40; num142++)
					{
						Main.travelShop[num142] = reader.ReadInt16();
					}
				}
				break;
			case 73:
				Main.player[whoAmI].TeleportationPotion();
				break;
			case 74:
				if (Main.netMode == 1)
				{
					Main.anglerQuest = reader.ReadByte();
					Main.anglerQuestFinished = reader.ReadBoolean();
				}
				break;
			case 75:
				if (Main.netMode == 2)
				{
					string name = Main.player[whoAmI].name;
					if (!Main.anglerWhoFinishedToday.Contains(name))
					{
						Main.anglerWhoFinishedToday.Add(name);
					}
				}
				break;
			case 76:
			{
				int num100 = reader.ReadByte();
				if (num100 != Main.myPlayer || Main.ServerSideCharacter)
				{
					if (Main.netMode == 2)
					{
						num100 = whoAmI;
					}
					Player player9 = Main.player[num100];
					player9.anglerQuestsFinished = reader.ReadInt32();
					if (Main.netMode == 2)
					{
						NetMessage.SendData(76, -1, whoAmI, "", num100);
					}
				}
				break;
			}
			case 77:
			{
				short type5 = reader.ReadInt16();
				ushort tileType = reader.ReadUInt16();
				short x4 = reader.ReadInt16();
				short y4 = reader.ReadInt16();
				Animation.NewTemporaryAnimation(type5, tileType, x4, y4);
				break;
			}
			case 78:
				if (Main.netMode == 1)
				{
					Main.ReportInvasionProgress(reader.ReadInt32(), reader.ReadInt32(), reader.ReadSByte(), reader.ReadSByte());
				}
				break;
			case 79:
			{
				int x3 = reader.ReadInt16();
				int y3 = reader.ReadInt16();
				short type4 = reader.ReadInt16();
				int style2 = reader.ReadInt16();
				int num65 = reader.ReadByte();
				int random = reader.ReadSByte();
				int direction = reader.ReadBoolean() ? 1 : (-1);
				if (Main.netMode == 2)
				{
					Netplay.Clients[whoAmI].SpamAddBlock += 1f;
					if (!WorldGen.InWorld(x3, y3, 10) || !Netplay.Clients[whoAmI].TileSections[Netplay.GetSectionX(x3), Netplay.GetSectionY(y3)])
					{
						break;
					}
				}
				WorldGen.PlaceObject(x3, y3, type4, false, style2, num65, random, direction);
				if (Main.netMode == 2)
				{
					NetMessage.SendObjectPlacment(whoAmI, x3, y3, type4, style2, num65, random, direction);
				}
				break;
			}
			case 80:
				if (Main.netMode == 1)
				{
					int num53 = reader.ReadByte();
					int num54 = reader.ReadInt16();
					if (num54 >= -3 && num54 < 1000)
					{
						Main.player[num53].chest = num54;
						Recipe.FindRecipes();
					}
				}
				break;
			case 81:
				if (Main.netMode == 1)
				{
					int x = (int)reader.ReadSingle();
					int y = (int)reader.ReadSingle();
					Color color = reader.ReadRGB();
					string text = reader.ReadString();
					CombatText.NewText(new Rectangle(x, y, 0, 0), color, text);
				}
				break;
			case 82:
				NetManager.Instance.Read(reader, whoAmI);
				break;
			case 83:
				if (Main.netMode == 1)
				{
					int num28 = reader.ReadInt16();
					int num29 = reader.ReadInt32();
					if (num28 >= 0 && num28 < 251)
					{
						NPC.killCount[num28] = num29;
					}
				}
				break;
			case 84:
			{
				byte b6 = reader.ReadByte();
				float stealth = reader.ReadSingle();
				Main.player[b6].stealth = stealth;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(84, -1, whoAmI, "", b6);
				}
				break;
			}
			case 85:
			{
				int num23 = whoAmI;
				byte b4 = reader.ReadByte();
				if (Main.netMode == 2 && num23 < 16 && b4 < 58)
				{
					Chest.ServerPlaceItem(whoAmI, b4);
				}
				break;
			}
			case 86:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int key = reader.ReadInt32();
				if (!reader.ReadBoolean())
				{
					TileEntity value2;
					if (TileEntity.ByID.TryGetValue(key, out value2) && (value2 is TETrainingDummy || value2 is TEItemFrame))
					{
						TileEntity.ByID.Remove(key);
						TileEntity.ByPosition.Remove(value2.Position);
					}
				}
				else
				{
					TileEntity tileEntity = TileEntity.Read(reader);
					TileEntity.ByID[tileEntity.ID] = tileEntity;
					TileEntity.ByPosition[tileEntity.Position] = tileEntity;
				}
				break;
			}
			case 87:
			{
				if (Main.netMode != 2)
				{
					break;
				}
				int num202 = reader.ReadInt16();
				int num203 = reader.ReadInt16();
				int num204 = reader.ReadByte();
				if (num202 < 0 || num202 >= Main.maxTilesX || num203 < 0 || num203 >= Main.maxTilesY || TileEntity.ByPosition.ContainsKey(new Point16(num202, num203)))
				{
					break;
				}
				switch (num204)
				{
				case 0:
					if (TETrainingDummy.ValidTile(num202, num203))
					{
						TETrainingDummy.Place(num202, num203);
					}
					break;
				case 1:
					if (TEItemFrame.ValidTile(num202, num203))
					{
						int number3 = TEItemFrame.Place(num202, num203);
						NetMessage.SendData(86, -1, -1, "", number3, num202, num203);
					}
					break;
				}
				break;
			}
			case 88:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num143 = reader.ReadInt16();
				if (num143 < 0 || num143 > 400)
				{
					break;
				}
				Item ıtem4 = Main.item[num143];
				BitsByte bitsByte5 = reader.ReadByte();
				if (bitsByte5[0])
				{
					ıtem4.color.PackedValue = reader.ReadUInt32();
				}
				if (bitsByte5[1])
				{
					ıtem4.damage = reader.ReadUInt16();
				}
				if (bitsByte5[2])
				{
					ıtem4.knockBack = reader.ReadSingle();
				}
				if (bitsByte5[3])
				{
					ıtem4.useAnimation = reader.ReadUInt16();
				}
				if (bitsByte5[4])
				{
					ıtem4.useTime = reader.ReadUInt16();
				}
				if (bitsByte5[5])
				{
					ıtem4.shoot = reader.ReadInt16();
				}
				if (bitsByte5[6])
				{
					ıtem4.shootSpeed = reader.ReadSingle();
				}
				if (bitsByte5[7])
				{
					bitsByte5 = reader.ReadByte();
					if (bitsByte5[0])
					{
						ıtem4.width = reader.ReadInt16();
					}
					if (bitsByte5[1])
					{
						ıtem4.height = reader.ReadInt16();
					}
					if (bitsByte5[2])
					{
						ıtem4.scale = reader.ReadSingle();
					}
					if (bitsByte5[3])
					{
						ıtem4.ammo = reader.ReadInt16();
					}
					if (bitsByte5[4])
					{
						ıtem4.useAmmo = reader.ReadInt16();
					}
					if (bitsByte5[5])
					{
						ıtem4.notAmmo = reader.ReadBoolean();
					}
				}
				break;
			}
			case 89:
				if (Main.netMode == 2)
				{
					int x5 = reader.ReadInt16();
					int y5 = reader.ReadInt16();
					int netid = reader.ReadInt16();
					int prefix = reader.ReadByte();
					int stack4 = reader.ReadInt16();
					TEItemFrame.TryPlacing(x5, y5, netid, prefix, stack4);
				}
				break;
			case 91:
			{
				if (Main.netMode != 1)
				{
					break;
				}
				int num123 = reader.ReadInt32();
				int num124 = reader.ReadByte();
				if (num124 == 255)
				{
					if (EmoteBubble.byID.ContainsKey(num123))
					{
						EmoteBubble.byID.Remove(num123);
					}
					break;
				}
				int meta = reader.ReadUInt16();
				int num125 = reader.ReadByte();
				int num126 = reader.ReadByte();
				int metadata = 0;
				if (num126 < 0)
				{
					metadata = reader.ReadInt16();
				}
				WorldUIAnchor worldUIAnchor = EmoteBubble.DeserializeNetAnchor(num124, meta);
				lock (EmoteBubble.byID)
				{
					if (!EmoteBubble.byID.ContainsKey(num123))
					{
						EmoteBubble.byID[num123] = new EmoteBubble(num126, worldUIAnchor, num125);
					}
					else
					{
						EmoteBubble.byID[num123].lifeTime = num125;
						EmoteBubble.byID[num123].lifeTimeStart = num125;
						EmoteBubble.byID[num123].emote = num126;
						EmoteBubble.byID[num123].anchor = worldUIAnchor;
					}
					EmoteBubble.byID[num123].ID = num123;
					EmoteBubble.byID[num123].metadata = metadata;
				}
				break;
			}
			case 92:
			{
				int num109 = reader.ReadInt16();
				float num110 = reader.ReadSingle();
				float num111 = reader.ReadSingle();
				float num112 = reader.ReadSingle();
				if (num109 >= 0 && num109 <= 200)
				{
					if (Main.netMode == 1)
					{
						Main.npc[num109].moneyPing(new Vector2(num111, num112));
						Main.npc[num109].extraValue = num110;
					}
					else
					{
						Main.npc[num109].extraValue += num110;
						NetMessage.SendData(92, -1, -1, "", num109, Main.npc[num109].extraValue, num111, num112);
					}
				}
				break;
			}
			case 95:
			{
				if (Main.netMode != 2)
				{
					break;
				}
				ushort num97 = reader.ReadUInt16();
				if (num97 < 0 || num97 >= 1000)
				{
					break;
				}
				Projectile projectile = Main.projectile[num97];
				if (projectile.type == 602)
				{
					projectile.Kill();
					if (Main.netMode != 0)
					{
						NetMessage.SendData(29, -1, -1, "", projectile.whoAmI, projectile.owner);
					}
				}
				break;
			}
			case 96:
			{
				int num80 = reader.ReadByte();
				Player player7 = Main.player[num80];
				int num81 = reader.ReadInt16();
				Vector2 newPos2 = reader.ReadVector2();
				Vector2 velocity2 = reader.ReadVector2();
				int num82 = player7.lastPortalColorIndex = num81 + ((num81 % 2 == 0) ? 1 : (-1));
				player7.Teleport(newPos2, 4, num81);
				player7.velocity = velocity2;
				break;
			}
			case 97:
				if (Main.netMode == 1)
				{
					AchievementsHelper.NotifyNPCKilledDirect(Main.player[Main.myPlayer], reader.ReadInt16());
				}
				break;
			case 98:
				if (Main.netMode == 1)
				{
					AchievementsHelper.NotifyProgressionEvent(reader.ReadInt16());
				}
				break;
			case 99:
			{
				int num69 = reader.ReadByte();
				if (Main.netMode == 2)
				{
					num69 = whoAmI;
				}
				Player player6 = Main.player[num69];
				player6.MinionTargetPoint = reader.ReadVector2();
				if (Main.netMode == 2)
				{
					NetMessage.SendData(99, -1, whoAmI, "", num69);
				}
				break;
			}
			case 100:
			{
				int num59 = reader.ReadUInt16();
				NPC nPC = Main.npc[num59];
				int num60 = reader.ReadInt16();
				Vector2 newPos = reader.ReadVector2();
				Vector2 velocity = reader.ReadVector2();
				int num61 = nPC.lastPortalColorIndex = num60 + ((num60 % 2 == 0) ? 1 : (-1));
				nPC.Teleport(newPos, 4, num60);
				nPC.velocity = velocity;
				break;
			}
			case 101:
				if (Main.netMode != 2)
				{
					NPC.ShieldStrengthTowerSolar = reader.ReadUInt16();
					NPC.ShieldStrengthTowerVortex = reader.ReadUInt16();
					NPC.ShieldStrengthTowerNebula = reader.ReadUInt16();
					NPC.ShieldStrengthTowerStardust = reader.ReadUInt16();
					if (NPC.ShieldStrengthTowerSolar < 0)
					{
						NPC.ShieldStrengthTowerSolar = 0;
					}
					if (NPC.ShieldStrengthTowerVortex < 0)
					{
						NPC.ShieldStrengthTowerVortex = 0;
					}
					if (NPC.ShieldStrengthTowerNebula < 0)
					{
						NPC.ShieldStrengthTowerNebula = 0;
					}
					if (NPC.ShieldStrengthTowerStardust < 0)
					{
						NPC.ShieldStrengthTowerStardust = 0;
					}
					if (NPC.ShieldStrengthTowerSolar > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerSolar = NPC.LunarShieldPowerExpert;
					}
					if (NPC.ShieldStrengthTowerVortex > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerVortex = NPC.LunarShieldPowerExpert;
					}
					if (NPC.ShieldStrengthTowerNebula > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerNebula = NPC.LunarShieldPowerExpert;
					}
					if (NPC.ShieldStrengthTowerStardust > NPC.LunarShieldPowerExpert)
					{
						NPC.ShieldStrengthTowerStardust = NPC.LunarShieldPowerExpert;
					}
				}
				break;
			case 102:
			{
				int num11 = reader.ReadByte();
				byte b2 = reader.ReadByte();
				Vector2 other = reader.ReadVector2();
				if (Main.netMode == 2)
				{
					num11 = whoAmI;
					NetMessage.SendData(102, -1, -1, "", num11, (int)b2, other.X, other.Y);
					break;
				}
				Player player2 = Main.player[num11];
				for (int j = 0; j < 16; j++)
				{
					Player player3 = Main.player[j];
					if (!player3.active || player3.dead || (player2.team != 0 && player2.team != player3.team) || !(player3.Distance(other) < 700f))
					{
						continue;
					}
					Vector2 value3 = player2.Center - player3.Center;
					Vector2 vector = Vector2.Normalize(value3);
					if (!vector.HasNaNs())
					{
						int type2 = 90;
						float num12 = 0f;
						float num13 = (float)Math.PI / 15f;
						Vector2 spinningpoint = new Vector2(0f, -8f);
						Vector2 value4 = new Vector2(-3f);
						float num14 = 0f;
						float num15 = 0.005f;
						switch (b2)
						{
						case 179:
							type2 = 86;
							break;
						case 173:
							type2 = 90;
							break;
						case 176:
							type2 = 88;
							break;
						}
						for (int k = 0; (float)k < value3.Length() / 6f; k++)
						{
							Vector2 position = player3.Center + 6f * (float)k * vector + spinningpoint.RotatedBy(num12) + value4;
							num12 += num13;
							int num16 = Dust.NewDust(position, 6, 6, type2, 0f, 0f, 100, default(Color), 1.5f);
							Main.dust[num16].noGravity = true;
							Main.dust[num16].velocity = Vector2.Zero;
							num14 = (Main.dust[num16].fadeIn = num14 + num15);
							Main.dust[num16].velocity += vector * 1.5f;
						}
					}
					player3.NebulaLevelup(b2);
				}
				break;
			}
			case 103:
				if (Main.netMode == 1)
				{
					NPC.MoonLordCountdown = reader.ReadInt32();
				}
				break;
			case 104:
				if (Main.netMode == 1 && Main.npcShop > 0)
				{
					Item[] item = Main.instance.shop[Main.npcShop].item;
					int num4 = reader.ReadByte();
					int type = reader.ReadInt16();
					int stack = reader.ReadInt16();
					int pre = reader.ReadByte();
					int value = reader.ReadInt32();
					BitsByte bitsByte = reader.ReadByte();
					if (num4 < item.Length)
					{
						item[num4] = new Item();
						item[num4].netDefaults(type);
						item[num4].stack = stack;
						item[num4].Prefix(pre);
						item[num4].value = value;
						item[num4].buyOnce = bitsByte[0];
					}
				}
				break;
			case 105:
				if (Main.netMode == 2)
				{
					NetMessage.SendData(105, whoAmI, -1, "", -1, -1f, -1f, -1f);
				}
				break;
			case 106:
				if (Main.netMode == 2)
				{
					NetMessage.SendData(106, whoAmI, -1, "", -1, -1f, -1f, -1f);
				}
				break;
			}
		}
	}
}
