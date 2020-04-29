using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Terraria.Net.Sockets;

namespace Terraria.Net
{
	internal class NetManager
	{
		private class PacketTypeStorage<T> where T : NetModule
		{
			public static T Module;
		}

		public static NetManager Instance = new NetManager();

		private Dictionary<ushort, NetModule> _modules = new Dictionary<ushort, NetModule>();

		private ushort ModuleCount;

		private static long _trafficTotal = 0L;

		private static Stopwatch _trafficTimer = CreateStopwatch();

		private static Stopwatch CreateStopwatch()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}

		public void Register<T>() where T : NetModule, new()
		{
			T val = new T();
			val.Id = ModuleCount;
			PacketTypeStorage<T>.Module = val;
			_modules[ModuleCount] = val;
			ModuleCount++;
		}

		public NetModule GetModule<T>() where T : NetModule
		{
			return PacketTypeStorage<T>.Module;
		}

		public ushort GetId<T>() where T : NetModule
		{
			return PacketTypeStorage<T>.Module.Id;
		}

		public void Read(BinaryReader reader, int userId)
		{
			ushort key = reader.ReadUInt16();
			if (_modules.ContainsKey(key))
			{
				_modules[key].Deserialize(reader, userId);
			}
		}

		public void Broadcast(NetPacket packet, int ignoreClient = -1)
		{
			for (int i = 0; i < 17; i++)
			{
				if (i != ignoreClient && Netplay.Clients[i].IsConnected())
				{
					SendData(Netplay.Clients[i].Socket, packet);
				}
			}
		}

		public void SendToServer(NetPacket packet)
		{
			SendData(Netplay.Connection.Socket, packet);
		}

		public static void SendData(ISocket socket, NetPacket packet)
		{
			try
			{
				socket.AsyncSend(packet.Buffer.Data, 0, packet.Length, SendCallback, packet);
			}
			catch
			{
				Console.WriteLine("    Exception normal: Tried to send data to a client after losing connection");
			}
		}

		public static void SendCallback(object state)
		{
			((NetPacket)state).Recycle();
		}

		private static void UpdateStats(int length)
		{
			_trafficTotal += length;
			double totalSeconds = _trafficTimer.Elapsed.TotalSeconds;
			if (totalSeconds > 5.0)
			{
				double num = _trafficTotal;
				double d = num / totalSeconds;
				double num2 = Math.Floor(d) / 1000.0;
				Console.WriteLine("NetManager :: Sending at " + num2 + " kbps.");
				_trafficTimer.Restart();
				_trafficTotal = 0L;
			}
		}
	}
}
