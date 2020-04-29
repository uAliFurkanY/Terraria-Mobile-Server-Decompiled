using Microsoft.Xna.Framework;
using Terraria.Net.Sockets;

namespace Terraria
{
	public class RemoteClient
	{
		public ISocket Socket;

		public int Id;

		public string Name = "Anonymous";

		public bool IsActive;

		public bool PendingTermination;

		public bool IsAnnouncementCompleted;

		public bool IsReading;

		public int State;

		public int TimeOutTimer;

		public string StatusText = "";

		public string StatusText2;

		public int StatusCount;

		public int StatusMax;

		public bool[,] TileSections = new bool[Main.maxTilesX / 200 + 1, Main.maxTilesY / 150 + 1];

		public byte[] ReadBuffer;

		public float SpamProjectile;

		public float SpamAddBlock;

		public float SpamDeleteBlock;

		public float SpamWater;

		public float SpamProjectileMax = 100f;

		public float SpamAddBlockMax = 100f;

		public float SpamDeleteBlockMax = 500f;

		public float SpamWaterMax = 50f;

		public bool IsConnected()
		{
			if (Socket != null)
			{
				return Socket.IsConnected();
			}
			return false;
		}

		public void SpamUpdate()
		{
			if (!Netplay.spamCheck)
			{
				SpamProjectile = 0f;
				SpamDeleteBlock = 0f;
				SpamAddBlock = 0f;
				SpamWater = 0f;
				return;
			}
			if (SpamProjectile > SpamProjectileMax)
			{
				NetMessage.BootPlayer(Id, "Cheating attempt detected: Projectile spam");
			}
			if (SpamAddBlock > SpamAddBlockMax)
			{
				NetMessage.BootPlayer(Id, "Cheating attempt detected: Add tile spam");
			}
			if (SpamDeleteBlock > SpamDeleteBlockMax)
			{
				NetMessage.BootPlayer(Id, "Cheating attempt detected: Remove tile spam");
			}
			if (SpamWater > SpamWaterMax)
			{
				NetMessage.BootPlayer(Id, "Cheating attempt detected: Liquid spam");
			}
			SpamProjectile -= 0.4f;
			if (SpamProjectile < 0f)
			{
				SpamProjectile = 0f;
			}
			SpamAddBlock -= 0.3f;
			if (SpamAddBlock < 0f)
			{
				SpamAddBlock = 0f;
			}
			SpamDeleteBlock -= 5f;
			if (SpamDeleteBlock < 0f)
			{
				SpamDeleteBlock = 0f;
			}
			SpamWater -= 0.2f;
			if (SpamWater < 0f)
			{
				SpamWater = 0f;
			}
		}

		public void SpamClear()
		{
			SpamProjectile = 0f;
			SpamAddBlock = 0f;
			SpamDeleteBlock = 0f;
			SpamWater = 0f;
		}

		public static void CheckSection(int playerIndex, Vector2 position, int fluff = 1)
		{
			int sectionX = Netplay.GetSectionX((int)(position.X / 16f));
			int sectionY = Netplay.GetSectionY((int)(position.Y / 16f));
			int num = 0;
			for (int i = sectionX - fluff; i < sectionX + fluff + 1; i++)
			{
				for (int j = sectionY - fluff; j < sectionY + fluff + 1; j++)
				{
					if (i >= 0 && i < Main.maxSectionsX && j >= 0 && j < Main.maxSectionsY && !Netplay.Clients[playerIndex].TileSections[i, j])
					{
						num++;
					}
				}
			}
			if (num <= 0)
			{
				return;
			}
			int num2 = num;
			NetMessage.SendData(9, playerIndex, -1, Lang.inter[44].Value, num2);
			Netplay.Clients[playerIndex].StatusText2 = "is receiving tile data";
			Netplay.Clients[playerIndex].StatusMax += num2;
			for (int k = sectionX - fluff; k < sectionX + fluff + 1; k++)
			{
				for (int l = sectionY - fluff; l < sectionY + fluff + 1; l++)
				{
					if (k >= 0 && k < Main.maxSectionsX && l >= 0 && l < Main.maxSectionsY && !Netplay.Clients[playerIndex].TileSections[k, l])
					{
						NetMessage.SendSection(playerIndex, k, l);
						NetMessage.SendData(11, playerIndex, -1, "", k, l, k, l);
					}
				}
			}
		}

		public bool SectionRange(int size, int firstX, int firstY)
		{
			for (int i = 0; i < 4; i++)
			{
				int num = firstX;
				int num2 = firstY;
				if (i == 1)
				{
					num += size;
				}
				if (i == 2)
				{
					num2 += size;
				}
				if (i == 3)
				{
					num += size;
					num2 += size;
				}
				int sectionX = Netplay.GetSectionX(num);
				int sectionY = Netplay.GetSectionY(num2);
				if (TileSections[sectionX, sectionY])
				{
					return true;
				}
			}
			return false;
		}

		public void ResetSections()
		{
			for (int i = 0; i < Main.maxSectionsX; i++)
			{
				for (int j = 0; j < Main.maxSectionsY; j++)
				{
					TileSections[i, j] = false;
				}
			}
		}

		public void Reset()
		{
			ResetSections();
			if (Id < 16)
			{
				Main.player[Id] = new Player();
			}
			TimeOutTimer = 0;
			StatusCount = 0;
			StatusMax = 0;
			StatusText2 = "";
			StatusText = "";
			State = 0;
			IsReading = false;
			PendingTermination = false;
			SpamClear();
			IsActive = false;
			NetMessage.buffer[Id].Reset();
			if (Socket != null)
			{
				Socket.Close();
			}
		}

		public void ServerWriteCallBack(object state)
		{
			NetMessage.buffer[Id].spamCount--;
			if (StatusMax > 0)
			{
				StatusCount++;
			}
		}

		public void ServerReadCallBack(object state, int length)
		{
			if (!Netplay.disconnect)
			{
				if (length == 0)
				{
					PendingTermination = true;
				}
				else if (Main.ignoreErrors)
				{
					try
					{
						NetMessage.RecieveBytes(ReadBuffer, length, Id);
					}
					catch
					{
					}
				}
				else
				{
					NetMessage.RecieveBytes(ReadBuffer, length, Id);
				}
			}
			IsReading = false;
		}
	}
}
