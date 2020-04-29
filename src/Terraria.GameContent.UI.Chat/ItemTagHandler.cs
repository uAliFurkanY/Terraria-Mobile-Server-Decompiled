using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Terraria.GameContent.UI.Chat
{
	internal class ItemTagHandler : ITagHandler
	{
		private class ItemSnippet : TextSnippet
		{
			private Item _item;

			public ItemSnippet(Item item)
			{
				_item = item;
				Color = ItemRarity.GetColor(item.rare);
			}

			public override void OnHover()
			{
				Main.toolTip = _item.Clone();
				Main.instance.MouseText(_item.Name, _item.rare, 0);
			}

			public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default(Vector2), Color color = default(Color), float scale = 1f)
			{
				float num = 1f;
				float num2 = 1f;
				if (Main.netMode != 2 && !Main.dedServ)
				{
					Texture2D texture2D = Main.itemTexture[_item.type];
					Rectangle rectangle = (Main.itemAnimations[_item.type] == null) ? texture2D.Frame() : Main.itemAnimations[_item.type].GetFrame(texture2D);
					if (rectangle.Height > 32)
					{
						num2 = 32f / (float)rectangle.Height;
					}
				}
				num2 *= scale;
				num *= num2;
				if (num > 0.75f)
				{
					num = 0.75f;
				}
				if (!justCheckingString && color != Color.Black)
				{
					float inventoryScale = Main.inventoryScale;
					Main.inventoryScale = scale * num;
					ItemSlot.Draw(spriteBatch, ref _item, 14, position - new Vector2(10f) * scale * num, Color.White);
					Main.inventoryScale = inventoryScale;
				}
				size = new Vector2(32f) * scale * num;
				return true;
			}
		}

		TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
		{
			Item ıtem = new Item();
			int result;
			if (int.TryParse(text, out result))
			{
				ıtem.netDefaults(result);
			}
			else
			{
				ıtem.SetDefaults(text);
			}
			if (ıtem.type <= 0)
			{
				return new TextSnippet(text);
			}
			ıtem.stack = 1;
			if (options != null)
			{
				string[] array = options.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Length == 0)
					{
						continue;
					}
					switch (array[i][0])
					{
					case 's':
					case 'x':
					{
						int result3;
						if (int.TryParse(array[i].Substring(1), out result3))
						{
							ıtem.stack = Utils.Clamp(result3, 1, ıtem.maxStack);
						}
						break;
					}
					case 'p':
					{
						int result2;
						if (int.TryParse(array[i].Substring(1), out result2))
						{
							ıtem.Prefix((byte)Utils.Clamp(result2, 0, 84));
						}
						break;
					}
					}
				}
			}
			string str = "";
			if (ıtem.stack > 1)
			{
				str = " (" + ıtem.stack + ")";
			}
			ItemSnippet ıtemSnippet = new ItemSnippet(ıtem);
			ıtemSnippet.Text = "[" + ıtem.AffixName() + str + "]";
			ıtemSnippet.CheckForHover = true;
			ıtemSnippet.DeleteWhole = true;
			return ıtemSnippet;
		}

		public static string GenerateTag(Item I)
		{
			string text = "[i";
			if (I.prefix != 0)
			{
				text = text + "/p" + I.prefix;
			}
			if (I.stack != 1)
			{
				text = text + "/s" + I.stack;
			}
			object obj = text;
			return string.Concat(obj, ":", I.netID, "]");
		}
	}
}
