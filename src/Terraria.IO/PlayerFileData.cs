using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Terraria.Social;
using Terraria.Utilities;

namespace Terraria.IO
{
	public class PlayerFileData : FileData
	{
		private Player _player;

		private TimeSpan _playTime = TimeSpan.Zero;

		private Stopwatch _timer = new Stopwatch();

		private bool _isTimerActive;

		public Player Player
		{
			get
			{
				return _player;
			}
			set
			{
				_player = value;
				if (value != null)
				{
					Name = _player.name;
				}
			}
		}

		public PlayerFileData()
			: base("Player")
		{
		}

		public PlayerFileData(string path, bool cloudSave)
			: base("Player", path, cloudSave)
		{
		}

		public static PlayerFileData CreateAndSave(Player player)
		{
			PlayerFileData playerFileData = new PlayerFileData();
			playerFileData.Metadata = FileMetadata.FromCurrentSettings(FileType.Player);
			playerFileData.Player = player;
			playerFileData._isCloudSave = (SocialAPI.Cloud != null && SocialAPI.Cloud.EnabledByDefault);
			playerFileData._path = Main.GetPlayerPathFromName(player.name, playerFileData.IsCloudSave);
			(playerFileData.IsCloudSave ? Main.CloudFavoritesData : Main.LocalFavoriteData).ClearEntry(playerFileData);
			Player.SavePlayer(playerFileData, true);
			return playerFileData;
		}

		public override void SetAsActive()
		{
			Main.ActivePlayerFileData = this;
			Main.player[Main.myPlayer] = Player;
		}

		public override void MoveToCloud()
		{
			if (base.IsCloudSave || SocialAPI.Cloud == null)
			{
				return;
			}
			string playerPathFromName = Main.GetPlayerPathFromName(Name, true);
			if (!FileUtilities.MoveToCloud(base.Path, playerPathFromName))
			{
				return;
			}
			string fileName = GetFileName(false);
			string path = Main.PlayerPath + System.IO.Path.DirectorySeparatorChar + fileName + System.IO.Path.DirectorySeparatorChar;
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path);
				for (int i = 0; i < files.Length; i++)
				{
					string cloudPath = Main.CloudPlayerPath + "/" + fileName + "/" + FileUtilities.GetFileName(files[i]);
					FileUtilities.MoveToCloud(files[i], cloudPath);
				}
			}
			Main.LocalFavoriteData.ClearEntry(this);
			_isCloudSave = true;
			_path = playerPathFromName;
			Main.CloudFavoritesData.SaveFavorite(this);
		}

		public override void MoveToLocal()
		{
			if (!base.IsCloudSave || SocialAPI.Cloud == null)
			{
				return;
			}
			string playerPathFromName = Main.GetPlayerPathFromName(Name, false);
			if (FileUtilities.MoveToLocal(base.Path, playerPathFromName))
			{
				string fileName = GetFileName(false);
				char directorySeparatorChar = System.IO.Path.DirectorySeparatorChar;
				string matchPattern = Regex.Escape(Main.CloudPlayerPath) + "/" + Regex.Escape(fileName) + "/.+\\.map";
				List<string> files = SocialAPI.Cloud.GetFiles(matchPattern);
				for (int i = 0; i < files.Count; i++)
				{
					string localPath = Main.PlayerPath + directorySeparatorChar + fileName + directorySeparatorChar + FileUtilities.GetFileName(files[i]);
					FileUtilities.MoveToLocal(files[i], localPath);
				}
				Main.CloudFavoritesData.ClearEntry(this);
				_isCloudSave = false;
				_path = playerPathFromName;
				Main.LocalFavoriteData.SaveFavorite(this);
			}
		}

		public void UpdatePlayTimer()
		{
			if (Main.instance.IsActive && !Main.gamePaused && Main.hasFocus && _isTimerActive)
			{
				StartPlayTimer();
			}
			else
			{
				PausePlayTimer();
			}
		}

		public void StartPlayTimer()
		{
			_isTimerActive = true;
			if (!_timer.IsRunning)
			{
				_timer.Start();
			}
		}

		public void PausePlayTimer()
		{
			if (_timer.IsRunning)
			{
				_timer.Stop();
			}
		}

		public TimeSpan GetPlayTime()
		{
			if (_timer.IsRunning)
			{
				return _playTime + _timer.Elapsed;
			}
			return _playTime;
		}

		public void StopPlayTimer()
		{
			_isTimerActive = false;
			if (_timer.IsRunning)
			{
				_playTime += _timer.Elapsed;
				_timer.Reset();
			}
		}

		public void SetPlayTime(TimeSpan time)
		{
			_playTime = time;
		}
	}
}
