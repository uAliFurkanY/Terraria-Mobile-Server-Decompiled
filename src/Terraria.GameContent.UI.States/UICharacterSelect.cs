using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.UI;

namespace Terraria.GameContent.UI.States
{
	internal class UICharacterSelect : UIState
	{
		private UIList _playerList;

		public override void OnInitialize()
		{
			UIElement uIElement = new UIElement();
			uIElement.Width.Set(0f, 0.8f);
			uIElement.MaxWidth.Set(600f, 0f);
			uIElement.Top.Set(220f, 0f);
			uIElement.Height.Set(-220f, 1f);
			uIElement.HAlign = 0.5f;
			UIPanel uIPanel = new UIPanel();
			uIPanel.Width.Set(0f, 1f);
			uIPanel.Height.Set(-110f, 1f);
			uIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			uIElement.Append(uIPanel);
			_playerList = new UIList();
			_playerList.Width.Set(-25f, 1f);
			_playerList.Height.Set(0f, 1f);
			_playerList.ListPadding = 5f;
			uIPanel.Append(_playerList);
			UIScrollbar uIScrollbar = new UIScrollbar();
			uIScrollbar.SetView(100f, 1000f);
			uIScrollbar.Height.Set(0f, 1f);
			uIScrollbar.HAlign = 1f;
			uIPanel.Append(uIScrollbar);
			_playerList.SetScrollbar(uIScrollbar);
			UITextPanel uITextPanel = new UITextPanel("Select Player", 0.8f, true);
			uITextPanel.HAlign = 0.5f;
			uITextPanel.Top.Set(-35f, 0f);
			uITextPanel.SetPadding(15f);
			uITextPanel.BackgroundColor = new Color(73, 94, 171);
			uIElement.Append(uITextPanel);
			UITextPanel uITextPanel2 = new UITextPanel("Back", 0.7f, true);
			uITextPanel2.Width.Set(-10f, 0.5f);
			uITextPanel2.Height.Set(50f, 0f);
			uITextPanel2.VAlign = 1f;
			uITextPanel2.Top.Set(-45f, 0f);
			uITextPanel2.OnMouseOver += FadedMouseOver;
			uITextPanel2.OnMouseOut += FadedMouseOut;
			uITextPanel2.OnClick += GoBackClick;
			uIElement.Append(uITextPanel2);
			UITextPanel uITextPanel3 = new UITextPanel("New", 0.7f, true);
			uITextPanel3.CopyStyle(uITextPanel2);
			uITextPanel3.HAlign = 1f;
			uITextPanel3.OnMouseOver += FadedMouseOver;
			uITextPanel3.OnMouseOut += FadedMouseOut;
			uITextPanel3.OnClick += NewCharacterClick;
			uIElement.Append(uITextPanel3);
			Append(uIElement);
		}

		private void NewCharacterClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(10);
			Player player = new Player();
			player.inventory[0].SetDefaults(3507);
			player.inventory[0].Prefix(-1);
			player.inventory[1].SetDefaults(3509);
			player.inventory[1].Prefix(-1);
			player.inventory[2].SetDefaults(3506);
			player.inventory[2].Prefix(-1);
			Main.PendingPlayer = player;
			Main.menuMode = 2;
		}

		private void GoBackClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(11);
			Main.menuMode = 0;
		}

		private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(12);
			((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
		}

		private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.7f;
		}

		public override void OnActivate()
		{
			Main.ClearPendingPlayerSelectCallbacks();
			Main.LoadPlayers();
			_playerList.Clear();
			foreach (PlayerFileData player in Main.PlayerList)
			{
				_playerList.Add(new UICharacterListItem(player));
			}
		}
	}
}
