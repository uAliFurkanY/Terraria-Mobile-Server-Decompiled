using Steamworks;
using System.Diagnostics;
using Terraria.IO;
using Terraria.Net;
using Terraria.Net.Sockets;

namespace Terraria.Social.Steam
{
	public class NetClientSocialModule : NetSocialModule
	{
		private Callback<GameLobbyJoinRequested_t> _gameLobbyJoinRequested;

		private Callback<P2PSessionRequest_t> _p2pSessionRequest;

		private Callback<P2PSessionConnectFail_t> _p2pSessionConnectfail;

		private HAuthTicket _authTicket = HAuthTicket.Invalid;

		private byte[] _authData = new byte[1021];

		private uint _authDataLength;

		private bool _hasLocalHost;

		public NetClientSocialModule()
			: base(2, 1)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			_gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequest);
			_p2pSessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
			_p2pSessionConnectfail = Callback<P2PSessionConnectFail_t>.Create(OnSessionConnectFail);
			Main.OnEngineLoad += CheckParameters;
		}

		private void CheckParameters()
		{
			ulong result;
			if (Program.LaunchParameters.ContainsKey("+connect_lobby") && ulong.TryParse(Program.LaunchParameters["+connect_lobby"], out result))
			{
				CSteamID lobbySteamId = new CSteamID(result);
				if (lobbySteamId.IsValid())
				{
					Main.OpenPlayerSelect(delegate(PlayerFileData playerData)
					{
						Main.ServerSideCharacter = false;
						playerData.SetAsActive();
						Main.menuMode = 882;
						Main.statusText = "Joining...";
						_lobby.Join(lobbySteamId, OnLobbyEntered);
					});
				}
			}
		}

		public override void LaunchLocalServer(Process process, ServerMode mode)
		{
			if (_lobby.State != 0)
			{
				_lobby.Leave();
			}
			ProcessStartInfo startInfo = process.StartInfo;
			startInfo.Arguments = startInfo.Arguments + " -steam -localsteamid " + SteamUser.GetSteamID().m_SteamID;
			if (mode.HasFlag(ServerMode.Lobby))
			{
				_hasLocalHost = true;
				if (mode.HasFlag(ServerMode.FriendsCanJoin))
				{
					process.StartInfo.Arguments += " -lobby friends";
				}
				else
				{
					process.StartInfo.Arguments += " -lobby private";
				}
				if (mode.HasFlag(ServerMode.FriendsOfFriends))
				{
					process.StartInfo.Arguments += " -friendsoffriends";
				}
			}
			SteamFriends.SetRichPresence("status", "Playing online.");
			Netplay.OnDisconnect += OnDisconnect;
			process.Start();
		}

		public override ulong GetLobbyId()
		{
			return 0uL;
		}

		public override bool StartListening(SocketConnectionAccepted callback)
		{
			return false;
		}

		public override void StopListening()
		{
		}

		public override void Close(RemoteAddress address)
		{
			SteamFriends.ClearRichPresence();
			CSteamID user = RemoteAddressToSteamId(address);
			Close(user);
		}

		public override bool CanInvite()
		{
			if (_hasLocalHost || _lobby.State == LobbyState.Active || Main.LobbyId != 0)
			{
				return Main.netMode != 0;
			}
			return false;
		}

		public override void OpenInviteInterface()
		{
			_lobby.OpenInviteOverlay();
		}

		private void Close(CSteamID user)
		{
			if (_connectionStateMap.ContainsKey(user))
			{
				SteamNetworking.CloseP2PSessionWithUser(user);
				ClearAuthTicket();
				_connectionStateMap[user] = ConnectionState.Inactive;
				_lobby.Leave();
				_reader.ClearUser(user);
				_writer.ClearUser(user);
			}
		}

		public override void Connect(RemoteAddress address)
		{
		}

		public override void CancelJoin()
		{
			if (_lobby.State != 0)
			{
				_lobby.Leave();
			}
		}

		private void OnLobbyJoinRequest(GameLobbyJoinRequested_t result)
		{
			if (_lobby.State != 0)
			{
				_lobby.Leave();
			}
			string friendName = SteamFriends.GetFriendPersonaName(result.m_steamIDFriend);
			Main.OpenPlayerSelect(delegate(PlayerFileData playerData)
			{
				Main.ServerSideCharacter = false;
				playerData.SetAsActive();
				Main.menuMode = 882;
				Main.statusText = "Joining " + friendName + "...";
				_lobby.Join(result.m_steamIDLobby, OnLobbyEntered);
			});
		}

		private void OnLobbyEntered(LobbyEnter_t result, bool failure)
		{
			SteamNetworking.AllowP2PPacketRelay(true);
			SendAuthTicket(_lobby.Owner);
			int num = 0;
			P2PSessionState_t pConnectionState;
			while (SteamNetworking.GetP2PSessionState(_lobby.Owner, out pConnectionState) && pConnectionState.m_bConnectionActive != 1)
			{
				switch (pConnectionState.m_eP2PSessionError)
				{
				case 2:
					ClearAuthTicket();
					return;
				case 1:
					ClearAuthTicket();
					return;
				case 3:
					ClearAuthTicket();
					return;
				case 5:
					ClearAuthTicket();
					return;
				case 4:
					if (++num > 5)
					{
						ClearAuthTicket();
						return;
					}
					SteamNetworking.CloseP2PSessionWithUser(_lobby.Owner);
					SendAuthTicket(_lobby.Owner);
					break;
				}
			}
			_connectionStateMap[_lobby.Owner] = ConnectionState.Connected;
			SteamFriends.SetPlayedWith(_lobby.Owner);
			SteamFriends.SetRichPresence("status", "Playing online.");
			Main.clrInput();
			Netplay.ServerPassword = "";
			Main.GetInputText("");
			Main.autoPass = false;
			Main.netMode = 1;
			Netplay.OnConnectedToSocialServer(new SocialSocket(new SteamAddress(_lobby.Owner)));
		}

		private void SendAuthTicket(CSteamID address)
		{
			if (_authTicket == HAuthTicket.Invalid)
			{
				_authTicket = SteamUser.GetAuthSessionTicket(_authData, _authData.Length, out _authDataLength);
			}
			int num = (int)(_authDataLength + 3);
			byte[] array = new byte[num];
			array[0] = (byte)(num & 0xFF);
			array[1] = (byte)((num >> 8) & 0xFF);
			array[2] = 93;
			for (int i = 0; i < _authDataLength; i++)
			{
				array[i + 3] = _authData[i];
			}
			SteamNetworking.SendP2PPacket(address, array, (uint)num, EP2PSend.k_EP2PSendReliable, 1);
		}

		private void ClearAuthTicket()
		{
			if (_authTicket != HAuthTicket.Invalid)
			{
				SteamUser.CancelAuthTicket(_authTicket);
			}
			_authTicket = HAuthTicket.Invalid;
			for (int i = 0; i < _authData.Length; i++)
			{
				_authData[i] = 0;
			}
			_authDataLength = 0u;
		}

		private void OnDisconnect()
		{
			SteamFriends.ClearRichPresence();
			_hasLocalHost = false;
			Netplay.OnDisconnect -= OnDisconnect;
		}

		private void OnSessionConnectFail(P2PSessionConnectFail_t result)
		{
			Close(result.m_steamIDRemote);
		}

		private void OnP2PSessionRequest(P2PSessionRequest_t result)
		{
			CSteamID steamIDRemote = result.m_steamIDRemote;
			if (_connectionStateMap.ContainsKey(steamIDRemote) && _connectionStateMap[steamIDRemote] != 0)
			{
				SteamNetworking.AcceptP2PSessionWithUser(steamIDRemote);
			}
		}
	}
}
