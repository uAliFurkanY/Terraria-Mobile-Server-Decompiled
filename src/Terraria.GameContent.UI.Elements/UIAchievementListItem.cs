using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Achievements;
using Terraria.Graphics;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI.Elements
{
	internal class UIAchievementListItem : UIPanel
	{
		private const int _iconSize = 64;

		private const int _iconSizeWithSpace = 66;

		private const int _iconsPerRow = 8;

		private Achievement _achievement;

		private UIImageFramed _achievementIcon;

		private UIImage _achievementIconBorders;

		private int _iconIndex;

		private Rectangle _iconFrame;

		private Rectangle _iconFrameUnlocked;

		private Rectangle _iconFrameLocked;

		private Texture2D _innerPanelTopTexture;

		private Texture2D _innerPanelBottomTexture;

		private Texture2D _categoryTexture;

		private bool _locked;

		public UIAchievementListItem(Achievement achievement)
		{
			BackgroundColor = new Color(26, 40, 89) * 0.8f;
			BorderColor = new Color(13, 20, 44) * 0.8f;
			_achievement = achievement;
			Height.Set(82f, 0f);
			Width.Set(0f, 1f);
			PaddingTop = 8f;
			PaddingLeft = 9f;
			int num = _iconIndex = Main.Achievements.GetIconIndex(achievement.Name);
			_iconFrameUnlocked = new Rectangle(num % 8 * 66, num / 8 * 66, 64, 64);
			_iconFrameLocked = _iconFrameUnlocked;
			_iconFrameLocked.X += 528;
			_iconFrame = _iconFrameLocked;
			UpdateIconFrame();
			_achievementIcon = new UIImageFramed(TextureManager.Load("Images/UI/Achievements"), _iconFrame);
			Append(_achievementIcon);
			_achievementIconBorders = new UIImage(TextureManager.Load("Images/UI/Achievement_Borders"));
			_achievementIconBorders.Left.Set(-4f, 0f);
			_achievementIconBorders.Top.Set(-4f, 0f);
			Append(_achievementIconBorders);
			_innerPanelTopTexture = TextureManager.Load("Images/UI/Achievement_InnerPanelTop");
			_innerPanelBottomTexture = TextureManager.Load("Images/UI/Achievement_InnerPanelBottom");
			_categoryTexture = TextureManager.Load("Images/UI/Achievement_Categories");
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			_locked = !_achievement.IsCompleted;
			UpdateIconFrame();
			CalculatedStyle ınnerDimensions = GetInnerDimensions();
			CalculatedStyle dimensions = _achievementIconBorders.GetDimensions();
			float num = dimensions.X + dimensions.Width;
			Vector2 value = new Vector2(num + 7f, ınnerDimensions.Y);
			Tuple<decimal, decimal> trackerValues = GetTrackerValues();
			bool flag = false;
			if ((!(trackerValues.Item1 == 0m) || !(trackerValues.Item2 == 0m)) && _locked)
			{
				flag = true;
			}
			float num2 = ınnerDimensions.Width - dimensions.Width + 1f;
			Vector2 baseScale = new Vector2(0.85f);
			Vector2 baseScale2 = new Vector2(0.92f);
			Vector2 stringSize = ChatManager.GetStringSize(Main.fontItemStack, _achievement.Description.Value, baseScale2, num2);
			if (stringSize.Y > 38f)
			{
				baseScale2.Y *= 38f / stringSize.Y;
			}
			Color value2 = _locked ? Color.Silver : Color.Gold;
			value2 = Color.Lerp(value2, Color.White, base.IsMouseHovering ? 0.5f : 0f);
			Color value3 = _locked ? Color.DarkGray : Color.Silver;
			value3 = Color.Lerp(value3, Color.White, base.IsMouseHovering ? 1f : 0f);
			Color color = base.IsMouseHovering ? Color.White : Color.Gray;
			Vector2 vector = value - Vector2.UnitY * 2f;
			DrawPanelTop(spriteBatch, vector, num2, color);
			AchievementCategory category = _achievement.Category;
			vector.Y += 2f;
			vector.X += 4f;
			spriteBatch.Draw(_categoryTexture, vector, _categoryTexture.Frame(4, 2, (int)category), base.IsMouseHovering ? Color.White : Color.Silver, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
			vector.X += 4f;
			vector.X += 17f;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, _achievement.FriendlyName.Value, vector, value2, 0f, Vector2.Zero, baseScale, num2);
			vector.X -= 17f;
			Vector2 position = value + Vector2.UnitY * 27f;
			DrawPanelBottom(spriteBatch, position, num2, color);
			position.X += 8f;
			position.Y += 4f;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, _achievement.Description.Value, position, value3, 0f, Vector2.Zero, baseScale2, num2 - 10f);
			if (flag)
			{
				Vector2 vector2 = vector + Vector2.UnitX * num2 + Vector2.UnitY;
				string text = (int)trackerValues.Item1 + "/" + (int)trackerValues.Item2;
				Vector2 baseScale3 = new Vector2(0.75f);
				Vector2 stringSize2 = ChatManager.GetStringSize(Main.fontItemStack, text, baseScale3);
				float progress = (float)(trackerValues.Item1 / trackerValues.Item2);
				float num3 = 80f;
				Color color2 = new Color(100, 255, 100);
				if (!base.IsMouseHovering)
				{
					color2 = Color.Lerp(color2, Color.Black, 0.25f);
				}
				Color color3 = new Color(255, 255, 255);
				if (!base.IsMouseHovering)
				{
					color3 = Color.Lerp(color3, Color.Black, 0.25f);
				}
				DrawProgressBar(spriteBatch, progress, vector2 - Vector2.UnitX * num3 * 0.7f, num3, color3, color2, color2.MultiplyRGBA(new Color(new Vector4(1f, 1f, 1f, 0.5f))));
				vector2.X -= num3 * 1.4f + stringSize2.X;
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontItemStack, text, vector2, value2, 0f, new Vector2(0f, 0f), baseScale3, 90f);
			}
		}

		private void UpdateIconFrame()
		{
			if (!_locked)
			{
				_iconFrame = _iconFrameUnlocked;
			}
			else
			{
				_iconFrame = _iconFrameLocked;
			}
			if (_achievementIcon != null)
			{
				_achievementIcon.SetFrame(_iconFrame);
			}
		}

		private void DrawPanelTop(SpriteBatch spriteBatch, Vector2 position, float width, Color color)
		{
			spriteBatch.Draw(_innerPanelTopTexture, position, new Rectangle(0, 0, 2, _innerPanelTopTexture.Height), color);
			spriteBatch.Draw(_innerPanelTopTexture, new Vector2(position.X + 2f, position.Y), new Rectangle(2, 0, 2, _innerPanelTopTexture.Height), color, 0f, Vector2.Zero, new Vector2((width - 4f) / 2f, 1f), SpriteEffects.None, 0f);
			spriteBatch.Draw(_innerPanelTopTexture, new Vector2(position.X + width - 2f, position.Y), new Rectangle(4, 0, 2, _innerPanelTopTexture.Height), color);
		}

		private void DrawPanelBottom(SpriteBatch spriteBatch, Vector2 position, float width, Color color)
		{
			spriteBatch.Draw(_innerPanelBottomTexture, position, new Rectangle(0, 0, 6, _innerPanelBottomTexture.Height), color);
			spriteBatch.Draw(_innerPanelBottomTexture, new Vector2(position.X + 6f, position.Y), new Rectangle(6, 0, 7, _innerPanelBottomTexture.Height), color, 0f, Vector2.Zero, new Vector2((width - 12f) / 7f, 1f), SpriteEffects.None, 0f);
			spriteBatch.Draw(_innerPanelBottomTexture, new Vector2(position.X + width - 6f, position.Y), new Rectangle(13, 0, 6, _innerPanelBottomTexture.Height), color);
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			BackgroundColor = new Color(46, 60, 119);
			BorderColor = new Color(20, 30, 56);
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);
			BackgroundColor = new Color(26, 40, 89) * 0.8f;
			BorderColor = new Color(13, 20, 44) * 0.8f;
		}

		public Achievement GetAchievement()
		{
			return _achievement;
		}

		private Tuple<decimal, decimal> GetTrackerValues()
		{
			if (!_achievement.HasTracker)
			{
				return Tuple.Create(0m, 0m);
			}
			IAchievementTracker tracker = _achievement.GetTracker();
			if (tracker.GetTrackerType() == TrackerType.Int)
			{
				AchievementTracker<int> achievementTracker = (AchievementTracker<int>)tracker;
				return Tuple.Create((decimal)achievementTracker.Value, (decimal)achievementTracker.MaxValue);
			}
			if (tracker.GetTrackerType() == TrackerType.Float)
			{
				AchievementTracker<float> achievementTracker2 = (AchievementTracker<float>)tracker;
				return Tuple.Create((decimal)achievementTracker2.Value, (decimal)achievementTracker2.MaxValue);
			}
			return Tuple.Create(0m, 0m);
		}

		private void DrawProgressBar(SpriteBatch spriteBatch, float progress, Vector2 spot, float Width = 169f, Color BackColor = default(Color), Color FillingColor = default(Color), Color BlipColor = default(Color))
		{
			if (BlipColor == Color.Transparent)
			{
				BlipColor = new Color(255, 165, 0, 127);
			}
			if (FillingColor == Color.Transparent)
			{
				FillingColor = new Color(255, 241, 51);
			}
			if (BackColor == Color.Transparent)
			{
				FillingColor = new Color(255, 255, 255);
			}
			Texture2D colorBarTexture = Main.colorBarTexture;
			Texture2D colorBlipTexture = Main.colorBlipTexture;
			Texture2D magicPixel = Main.magicPixel;
			float num = MathHelper.Clamp(progress, 0f, 1f);
			float num2 = Width * 1f;
			float num3 = 8f;
			float num4 = num2 / 169f;
			Vector2 position = spot + Vector2.UnitY * num3 + Vector2.UnitX * 1f;
			spriteBatch.Draw(colorBarTexture, spot, new Rectangle(5, 0, colorBarTexture.Width - 9, colorBarTexture.Height), BackColor, 0f, new Vector2(84.5f, 0f), new Vector2(num4, 1f), SpriteEffects.None, 0f);
			spriteBatch.Draw(colorBarTexture, spot + new Vector2((0f - num4) * 84.5f - 5f, 0f), new Rectangle(0, 0, 5, colorBarTexture.Height), BackColor, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
			spriteBatch.Draw(colorBarTexture, spot + new Vector2(num4 * 84.5f, 0f), new Rectangle(colorBarTexture.Width - 4, 0, 4, colorBarTexture.Height), BackColor, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
			position += Vector2.UnitX * (num - 0.5f) * num2;
			position.X -= 1f;
			spriteBatch.Draw(magicPixel, position, new Rectangle(0, 0, 1, 1), FillingColor, 0f, new Vector2(1f, 0.5f), new Vector2(num2 * num, num3), SpriteEffects.None, 0f);
			if (progress != 0f)
			{
				spriteBatch.Draw(magicPixel, position, new Rectangle(0, 0, 1, 1), BlipColor, 0f, new Vector2(1f, 0.5f), new Vector2(2f, num3), SpriteEffects.None, 0f);
			}
			spriteBatch.Draw(magicPixel, position, new Rectangle(0, 0, 1, 1), Color.Black, 0f, new Vector2(0f, 0.5f), new Vector2(num2 * (1f - num), num3), SpriteEffects.None, 0f);
		}

		public override int CompareTo(object obj)
		{
			UIAchievementListItem uIAchievementListItem = obj as UIAchievementListItem;
			if (uIAchievementListItem == null)
			{
				return 0;
			}
			if (_achievement.IsCompleted && !uIAchievementListItem._achievement.IsCompleted)
			{
				return -1;
			}
			if (!_achievement.IsCompleted && uIAchievementListItem._achievement.IsCompleted)
			{
				return 1;
			}
			return _achievement.Id.CompareTo(uIAchievementListItem._achievement.Id);
		}
	}
}
