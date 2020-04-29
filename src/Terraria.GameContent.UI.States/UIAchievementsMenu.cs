using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Achievements;
using Terraria.GameContent.UI.Elements;
using Terraria.Graphics;
using Terraria.UI;

namespace Terraria.GameContent.UI.States
{
	public class UIAchievementsMenu : UIState
	{
		private UIList _achievementsList;

		private List<UIAchievementListItem> _achievementElements = new List<UIAchievementListItem>();

		private List<UIToggleImage> _categoryButtons = new List<UIToggleImage>();

		private UIElement _outerContainer;

		public override void OnInitialize()
		{
			UIElement uIElement = new UIElement();
			uIElement.Width.Set(0f, 0.8f);
			uIElement.MaxWidth.Set(800f, 0f);
			uIElement.MinWidth.Set(600f, 0f);
			uIElement.Top.Set(220f, 0f);
			uIElement.Height.Set(-220f, 1f);
			uIElement.HAlign = 0.5f;
			_outerContainer = uIElement;
			Append(uIElement);
			UIPanel uIPanel = new UIPanel();
			uIPanel.Width.Set(0f, 1f);
			uIPanel.Height.Set(-110f, 1f);
			uIPanel.BackgroundColor = new Color(33, 43, 79) * 0.8f;
			uIPanel.PaddingTop = 0f;
			uIElement.Append(uIPanel);
			_achievementsList = new UIList();
			_achievementsList.Width.Set(-25f, 1f);
			_achievementsList.Height.Set(-50f, 1f);
			_achievementsList.Top.Set(50f, 0f);
			_achievementsList.ListPadding = 5f;
			uIPanel.Append(_achievementsList);
			UITextPanel uITextPanel = new UITextPanel("Achievements", 1f, true);
			uITextPanel.HAlign = 0.5f;
			uITextPanel.Top.Set(-33f, 0f);
			uITextPanel.SetPadding(13f);
			uITextPanel.BackgroundColor = new Color(73, 94, 171);
			uIElement.Append(uITextPanel);
			UITextPanel uITextPanel2 = new UITextPanel("Back", 0.7f, true);
			uITextPanel2.Width.Set(-10f, 0.5f);
			uITextPanel2.Height.Set(50f, 0f);
			uITextPanel2.VAlign = 1f;
			uITextPanel2.HAlign = 0.5f;
			uITextPanel2.Top.Set(-45f, 0f);
			uITextPanel2.OnMouseOver += FadedMouseOver;
			uITextPanel2.OnMouseOut += FadedMouseOut;
			uITextPanel2.OnClick += GoBackClick;
			uIElement.Append(uITextPanel2);
			List<Achievement> list = Main.Achievements.CreateAchievementsList();
			for (int i = 0; i < list.Count; i++)
			{
				UIAchievementListItem item = new UIAchievementListItem(list[i]);
				_achievementsList.Add(item);
				_achievementElements.Add(item);
			}
			UIScrollbar uIScrollbar = new UIScrollbar();
			uIScrollbar.SetView(100f, 1000f);
			uIScrollbar.Height.Set(-50f, 1f);
			uIScrollbar.Top.Set(50f, 0f);
			uIScrollbar.HAlign = 1f;
			uIPanel.Append(uIScrollbar);
			_achievementsList.SetScrollbar(uIScrollbar);
			UIElement uIElement2 = new UIElement();
			uIElement2.Width.Set(0f, 1f);
			uIElement2.Height.Set(32f, 0f);
			uIElement2.Top.Set(10f, 0f);
			Texture2D texture = TextureManager.Load("Images/UI/Achievement_Categories");
			for (int j = 0; j < 4; j++)
			{
				UIToggleImage uIToggleImage = new UIToggleImage(texture, 32, 32, new Point(34 * j, 0), new Point(34 * j, 34));
				uIToggleImage.Left.Set(j * 36 + 8, 0f);
				uIToggleImage.SetState(true);
				uIToggleImage.OnClick += FilterList;
				_categoryButtons.Add(uIToggleImage);
				uIElement2.Append(uIToggleImage);
			}
			uIPanel.Append(uIElement2);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			int num = 0;
			while (true)
			{
				if (num < _categoryButtons.Count)
				{
					if (_categoryButtons[num].IsMouseHovering)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			string text = "";
			switch (num)
			{
			case 3:
				text = "Challenger";
				break;
			case 1:
				text = "Collector";
				break;
			case 2:
				text = "Explorer";
				break;
			case 0:
				text = "Slayer";
				break;
			case -1:
				text = "None";
				break;
			default:
				text = "None";
				break;
			}
			float x = Main.fontMouseText.MeasureString(text).X;
			Vector2 vector = new Vector2(Main.mouseX, Main.mouseY) + new Vector2(16f);
			if (vector.Y > (float)(Main.screenHeight - 30))
			{
				vector.Y = Main.screenHeight - 30;
			}
			if (vector.X > (float)Main.screenWidth - x)
			{
				vector.X = Main.screenWidth - 460;
			}
			Utils.DrawBorderStringFourWay(spriteBatch, Main.fontMouseText, text, vector.X, vector.Y, new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), Color.Black, Vector2.Zero);
		}

		public void GotoAchievement(Achievement achievement)
		{
			_achievementsList.Goto(delegate(UIElement element)
			{
				UIAchievementListItem uIAchievementListItem = element as UIAchievementListItem;
				return uIAchievementListItem != null && uIAchievementListItem.GetAchievement() == achievement;
			});
		}

		private void GoBackClick(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.menuMode = 0;
			AchievementsUI.Close();
		}

		private void FadedMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			Main.PlaySound(12);
			((UIPanel)evt.Target).BackgroundColor = new Color(73, 94, 171);
		}

		private void FadedMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			((UIPanel)evt.Target).BackgroundColor = new Color(63, 82, 151) * 0.8f;
		}

		private void FilterList(UIMouseEvent evt, UIElement listeningElement)
		{
			_achievementsList.Clear();
			foreach (UIAchievementListItem achievementElement in _achievementElements)
			{
				if (_categoryButtons[(int)achievementElement.GetAchievement().Category].IsOn)
				{
					_achievementsList.Add(achievementElement);
				}
			}
			Recalculate();
		}

		public override void OnActivate()
		{
			if (Main.gameMenu)
			{
				_outerContainer.Top.Set(220f, 0f);
				_outerContainer.Height.Set(-220f, 1f);
			}
			else
			{
				_outerContainer.Top.Set(120f, 0f);
				_outerContainer.Height.Set(-120f, 1f);
			}
			_achievementsList.UpdateOrder();
		}
	}
}
