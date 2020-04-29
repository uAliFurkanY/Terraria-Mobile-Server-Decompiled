using System;
using System.IO;
using Terraria.Net.Sockets;

namespace Terraria
{
	public class RemoteServer
	{
		public ISocket Socket = new TcpSocket();

		public bool IsActive;

		public int State;

		public int TimeOutTimer;

		public bool IsReading;

		public byte[] ReadBuffer;

		public string StatusText;

		public int StatusCount;

		public int StatusMax;

		public void ClientWriteCallBack(object state)
		{
			NetMessage.buffer[17].spamCount--;
		}

		public void ClientReadCallBack(object state, int length)
		{
			try
			{
				if (!Netplay.disconnect)
				{
					if (length == 0)
					{
						Netplay.disconnect = true;
						Main.statusText = "Lost connection";
					}
					else if (Main.ignoreErrors)
					{
						try
						{
							NetMessage.RecieveBytes(ReadBuffer, length);
						}
						catch
						{
						}
					}
					else
					{
						NetMessage.RecieveBytes(ReadBuffer, length);
					}
				}
				IsReading = false;
			}
			catch (Exception value)
			{
				try
				{
					using (StreamWriter streamWriter = new StreamWriter("client-crashlog.txt", true))
					{
						streamWriter.WriteLine(DateTime.Now);
						streamWriter.WriteLine(value);
						streamWriter.WriteLine("");
					}
				}
				catch
				{
				}
				Netplay.disconnect = true;
			}
		}
	}
}
