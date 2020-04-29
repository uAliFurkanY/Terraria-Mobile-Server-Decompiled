using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	internal class UICharacter : UIElement
	{
		private Player _player;

		private Texture2D _texture;

		private static Item _blankItem = new Item();

		public UICharacter(Player player)
		{
			_player = player;
			Width.Set(59f, 0f);
			Height.Set(58f, 0f);
			_texture = TextureManager.Load("Images/UI/PlayerBackground");
			_useImmediateMode = true;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			CalculatedStyle dimensions = GetDimensions();
			spriteBatch.Draw(_texture, dimensions.Position(), Color.White);
			Vector2 value = dimensions.Position() + new Vector2(dimensions.Width * 0.5f - (float)(_player.width >> 1), dimensions.Height * 0.5f - (float)(_player.height >> 1));
			Item ıtem = _player.inventory[_player.selectedItem];
			_player.inventory[_player.selectedItem] = _blankItem;
			Main.instance.DrawPlayer(_player, value + Main.screenPosition, 0f, Vector2.Zero);
			_player.inventory[_player.selectedItem] = ıtem;
		}
	}
}
