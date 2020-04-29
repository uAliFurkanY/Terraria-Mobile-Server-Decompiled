using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Terraria.Net.Sockets
{
	public class TcpSocket : ISocket
	{
		private TcpClient _connection;

		private TcpListener _listener;

		private SocketConnectionAccepted _listenerCallback;

		private RemoteAddress _remoteAddress;

		private bool _isListening;

		public TcpSocket()
		{
			_connection = new TcpClient();
			_connection.NoDelay = true;
		}

		public TcpSocket(TcpClient tcpClient)
		{
			_connection = tcpClient;
			_connection.NoDelay = true;
			IPEndPoint ıPEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
			_remoteAddress = new TcpAddress(ıPEndPoint.Address, ıPEndPoint.Port);
		}

		void ISocket.Close()
		{
			_remoteAddress = null;
			_connection.Close();
		}

		bool ISocket.IsConnected()
		{
			if (_connection == null || _connection.Client == null)
			{
				return false;
			}
			return _connection.Connected;
		}

		void ISocket.Connect(RemoteAddress address)
		{
			TcpAddress tcpAddress = (TcpAddress)address;
			_connection.Connect(tcpAddress.Address, tcpAddress.Port);
			_remoteAddress = address;
		}

		private void ReadCallback(IAsyncResult result)
		{
			Tuple<SocketReceiveCallback, object> tuple = (Tuple<SocketReceiveCallback, object>)result.AsyncState;
			tuple.Item1(tuple.Item2, _connection.GetStream().EndRead(result));
		}

		private void SendCallback(IAsyncResult result)
		{
			Tuple<SocketSendCallback, object> tuple = (Tuple<SocketSendCallback, object>)result.AsyncState;
			try
			{
				_connection.GetStream().EndWrite(result);
				tuple.Item1(tuple.Item2);
			}
			catch (Exception)
			{
				((ISocket)this).Close();
			}
		}

		void ISocket.AsyncSend(byte[] data, int offset, int size, SocketSendCallback callback, object state)
		{
			_connection.GetStream().BeginWrite(data, 0, size, SendCallback, new Tuple<SocketSendCallback, object>(callback, state));
		}

		void ISocket.AsyncReceive(byte[] data, int offset, int size, SocketReceiveCallback callback, object state)
		{
			_connection.GetStream().BeginRead(data, offset, size, ReadCallback, new Tuple<SocketReceiveCallback, object>(callback, state));
		}

		bool ISocket.IsDataAvailable()
		{
			return _connection.GetStream().DataAvailable;
		}

		RemoteAddress ISocket.GetRemoteAddress()
		{
			return _remoteAddress;
		}

		bool ISocket.StartListening(SocketConnectionAccepted callback)
		{
			IPAddress address = IPAddress.Any;
			string value;
			if (Program.LaunchParameters.TryGetValue("-ip", out value) && !IPAddress.TryParse(value, out address))
			{
				address = IPAddress.Any;
			}
			_isListening = true;
			_listenerCallback = callback;
			if (_listener == null)
			{
				_listener = new TcpListener(address, Netplay.ListenPort);
			}
			try
			{
				_listener.Start();
			}
			catch (Exception)
			{
				return false;
			}
			ThreadPool.QueueUserWorkItem(ListenLoop);
			return true;
		}

		void ISocket.StopListening()
		{
			_isListening = false;
		}

		private void ListenLoop(object unused)
		{
			while (_isListening && !Netplay.disconnect)
			{
				try
				{
					ISocket socket = new TcpSocket(_listener.AcceptTcpClient());
					Console.WriteLine(string.Concat(socket.GetRemoteAddress(), " is connecting..."));
					_listenerCallback(socket);
				}
				catch (Exception)
				{
				}
			}
			_listener.Stop();
		}
	}
}
