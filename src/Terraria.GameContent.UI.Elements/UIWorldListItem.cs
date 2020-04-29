using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;
using Terraria.IO;
using Terraria.Social;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	internal class UIWorldListItem : UIPanel
	{
		private WorldFileData _data;

		private Texture2D _dividerTexture;

		private Texture2D _innerPanelTexture;

		private UIImage _worldIcon;

		private UIText _buttonLabel;

		private UIText _deleteButtonLabel;

		private Texture2D _buttonCloudActiveTexture;

		private Texture2D _buttonCloudInactiveTexture;

		private Texture2D _buttonFavoriteActiveTexture;

		private Texture2D _buttonFavoriteInactiveTexture;

		private Texture2D _buttonPlayTexture;

		private Texture2D _buttonDeleteTexture;

		private UIImageButton _deleteButton;

		public bool IsFavorite => _data.IsFavorite;

		public UIWorldListItem(WorldFileData data)
		{
			BorderColor = new Color(89, 116, 213) * 0.7f;
			_dividerTexture = TextureManager.Load("Images/UI/Divider");
			_innerPanelTexture = TextureManager.Load("Images/UI/InnerPanelBackground");
			_buttonCloudActiveTexture = TextureManager.Load("Images/UI/ButtonCloudActive");
			_buttonCloudInactiveTexture = TextureManager.Load("Images/UI/ButtonCloudInactive");
			_buttonFavoriteActiveTexture = TextureManager.Load("Images/UI/ButtonFavoriteActive");
			_buttonFavoriteInactiveTexture = TextureManager.Load("Images/UI/ButtonFavoriteInactive");
			_buttonPlayTexture = TextureManager.Load("Images/UI/ButtonPlay");
			_buttonDeleteTexture = TextureManager.Load("Images/UI/ButtonDelete");
			Height.Set(96f, 0f);
			Width.Set(0f, 1f);
			SetPadding(6f);
			_data = data;
			_worldIcon = new UIImage(GetIcon());
			_worldIcon.Left.Set(4f, 0f);
			_worldIcon.OnDoubleClick += PlayGame;
			Append(_worldIcon);
			UIImageButton uIImageButton = new UIImageButton(_buttonPlayTexture);
			uIImageButton.VAlign = 1f;
			uIImageButton.Left.Set(4f, 0f);
			uIImageButton.OnClick += PlayGame;
			base.OnDoubleClick += PlayGame;
			uIImageButton.OnMouseOver += PlayMouseOver;
			uIImageButton.OnMouseOut += ButtonMouseOut;
			Append(uIImageButton);
			UIImageButton uIImageButton2 = new UIImageButton(_data.IsFavorite ? _buttonFavoriteActiveTexture : _buttonFavoriteInactiveTexture);
			uIImageButton2.VAlign = 1f;
			uIImageButton2.Left.Set(28f, 0f);
			uIImageButton2.OnClick += FavoriteButtonClick;
			uIImageButton2.OnMouseOver += FavoriteMouseOver;
			uIImageButton2.OnMouseOut += ButtonMouseOut;
			uIImageButton2.SetVisibility(1f, _data.IsFavorite ? 0.8f : 0.4f);
			Append(uIImageButton2);
			if (SocialAPI.Cloud != null)
			{
				UIImageButton uIImageButton3 = new UIImageButton(_data.IsCloudSave ? _buttonCloudActiveTexture : _buttonCloudInactiveTexture);
				uIImageButton3.VAlign = 1f;
				uIImageButton3.Left.Set(52f, 0f);
				uIImageButton3.OnClick += CloudButtonClick;
				uIImageButton3.OnMouseOver += CloudMouseOver;
				uIImageButton3.OnMouseOut += ButtonMouseOut;
				Append(uIImageButton3);
			}
			UIImageButton uIImageButton4 = new UIImageButton(_buttonDeleteTexture);
			uIImageButton4.VAlign = 1f;
			uIImageButton4.HAlign = 1f;
			uIImageButton4.OnClick += DeleteButtonClick;
			uIImageButton4.OnMouseOver += DeleteMouseOver;
			uIImageButton4.OnMouseOut += DeleteMouseOut;
			_deleteButton = uIImageButton4;
			if (!_data.IsFavorite)
			{
				Append(uIImageButton4);
			}
			_buttonLabel = new UIText("");
			_buttonLabel.VAlign = 1f;
			_buttonLabel.Left.Set(80f, 0f);
			_buttonLabel.Top.Set(-3f, 0f);
			Append(_buttonLabel);
			_deleteButtonLabel = new UIText("");
			_deleteButtonLabel.VAlign = 1f;
			_deleteButtonLabel.HAlign = 1f;
			_deleteButtonLabel.Left.Set(-30f, 0f);
			_deleteButtonLabel.Top.Set(-3f, 0f);
			Append(_deleteButtonLabel);
		}

		private Texture2D GetIcon()
		{
			return TextureManager.Load("Images/UI/Icon" + (_data.IsHardMode ? "Hallow" : "") + (_data.HasCorruption ? "Corruption" : "Crimson"));
		}

		private void FavoriteMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			if (_data.IsFavorite)
			{
				_buttonLabel.SetText("Unfavorite");
			}
			else
			{
				_buttonLabel.SetText("Favorite");
			}
		}

		private void CloudMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			if (_data.IsCloudSave)
			{
				_buttonLabel.SetText("Move off cloud");
			}
			else
			{
				_buttonLabel.SetText("Move to cloud");
			}
		}

		private void PlayMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			_buttonLabel.SetText("Play");
		}

		private void DeleteMouseOver(UIMouseEvent evt, UIElement listeningElement)
		{
			_deleteButtonLabel.SetText("Delete");
		}

		private void DeleteMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			_deleteButtonLabel.SetText("");
		}

		private void ButtonMouseOut(UIMouseEvent evt, UIElement listeningElement)
		{
			_buttonLabel.SetText("");
		}

		private void CloudButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			if (_data.IsCloudSave)
			{
				_data.MoveToLocal();
			}
			else
			{
				_data.MoveToCloud();
			}
			((UIImageButton)evt.Target).SetImage(_data.IsCloudSave ? _buttonCloudActiveTexture : _buttonCloudInactiveTexture);
			if (_data.IsCloudSave)
			{
				_buttonLabel.SetText("Move off cloud");
			}
			else
			{
				_buttonLabel.SetText("Move to cloud");
			}
		}

		private void DeleteButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			int num = 0;
			while (true)
			{
				if (num < Main.WorldList.Count)
				{
					if (Main.WorldList[num] == _data)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			Main.PlaySound(10);
			Main.selectedWorld = num;
			Main.menuMode = 9;
		}

		private void PlayGame(UIMouseEvent evt, UIElement listeningElement)
		{
			if (listeningElement == evt.Target)
			{
				_data.SetAsActive();
				Main.PlaySound(10);
				Main.GetInputText("");
				if (Main.menuMultiplayer && SocialAPI.Network != null)
				{
					Main.menuMode = 889;
				}
				else if (Main.menuMultiplayer)
				{
					Main.menuMode = 30;
				}
				else
				{
					Main.menuMode = 10;
				}
				if (!Main.menuMultiplayer)
				{
					WorldGen.playWorld();
				}
			}
		}

		private void FavoriteButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			_data.ToggleFavorite();
			((UIImageButton)evt.Target).SetImage(_data.IsFavorite ? _buttonFavoriteActiveTexture : _buttonFavoriteInactiveTexture);
			((UIImageButton)evt.Target).SetVisibility(1f, _data.IsFavorite ? 0.8f : 0.4f);
			if (_data.IsFavorite)
			{
				_buttonLabel.SetText("Unfavorite");
				RemoveChild(_deleteButton);
			}
			else
			{
				_buttonLabel.SetText("Favorite");
				Append(_deleteButton);
			}
			(Parent.Parent as UIList)?.UpdateOrder();
		}

		public override int CompareTo(object obj)
		{
			UIWorldListItem uIWorldListItem = obj as UIWorldListItem;
			if (uIWorldListItem != null)
			{
				if (IsFavorite && !uIWorldListItem.IsFavorite)
				{
					return -1;
				}
				if (!IsFavorite && uIWorldListItem.IsFavorite)
				{
					return 1;
				}
				return _data.Name.CompareTo(uIWorldListItem._data.Name);
			}
			return base.CompareTo(obj);
		}

		public override void MouseOver(UIMouseEvent evt)
		{
			base.MouseOver(evt);
			BackgroundColor = new Color(73, 94, 171);
			BorderColor = new Color(89, 116, 213);
		}

		public override void MouseOut(UIMouseEvent evt)
		{
			base.MouseOut(evt);
			BackgroundColor = new Color(63, 82, 151) * 0.7f;
			BorderColor = new Color(89, 116, 213) * 0.7f;
		}

		private void DrawPanel(SpriteBatch spriteBatch, Vector2 position, float width)
		{
			spriteBatch.Draw(_innerPanelTexture, position, new Rectangle(0, 0, 8, _innerPanelTexture.Height), Color.White);
			spriteBatch.Draw(_innerPanelTexture, new Vector2(position.X + 8f, position.Y), new Rectangle(8, 0, 8, _innerPanelTexture.Height), Color.White, 0f, Vector2.Zero, new Vector2((width - 16f) / 8f, 1f), SpriteEffects.None, 0f);
			spriteBatch.Draw(_innerPanelTexture, new Vector2(position.X + width - 8f, position.Y), new Rectangle(16, 0, 8, _innerPanelTexture.Height), Color.White);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			CalculatedStyle ınnerDimensions = GetInnerDimensions();
			CalculatedStyle dimensions = _worldIcon.GetDimensions();
			float num = dimensions.X + dimensions.Width;
			Color color = _data.IsValid ? Color.White : Color.Red;
			Utils.DrawBorderString(spriteBatch, _data.Name, new Vector2(num + 6f, dimensions.Y - 2f), color);
			spriteBatch.Draw(_dividerTexture, new Vector2(num, ınnerDimensions.Y + 21f), null, Color.White, 0f, Vector2.Zero, new Vector2((GetDimensions().X + GetDimensions().Width - num) / 8f, 1f), SpriteEffects.None, 0f);
			Vector2 vector = new Vector2(num + 6f, ınnerDimensions.Y + 29f);
			float num2 = 80f;
			DrawPanel(spriteBatch, vector, num2);
			string text = _data.IsExpertMode ? "Expert" : "Normal";
			float x = Main.fontMouseText.MeasureString(text).X;
			float x2 = num2 * 0.5f - x * 0.5f;
			Utils.DrawBorderString(spriteBatch, text, vector + new Vector2(x2, 3f), _data.IsExpertMode ? new Color(217, 143, 244) : Color.White);
			vector.X += num2 + 5f;
			float num3 = 140f;
			DrawPanel(spriteBatch, vector, num3);
			string text2 = _data.WorldSizeName + " World";
			float x3 = Main.fontMouseText.MeasureString(text2).X;
			float x4 = num3 * 0.5f - x3 * 0.5f;
			Utils.DrawBorderString(spriteBatch, text2, vector + new Vector2(x4, 3f), Color.White);
			vector.X += num3 + 5f;
			float num4 = ınnerDimensions.X + ınnerDimensions.Width - vector.X;
			DrawPanel(spriteBatch, vector, num4);
			string text3 = "Created: " + _data.CreationTime.ToString("d MMMM yyyy");
			float x5 = Main.fontMouseText.MeasureString(text3).X;
			float x6 = num4 * 0.5f - x5 * 0.5f;
			Utils.DrawBorderString(spriteBatch, text3, vector + new Vector2(x6, 3f), Color.White);
			vector.X += num4 + 5f;
		}
	}
}
