using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics;

namespace Terraria.Initializers
{
	internal static class PlayerDataInitializer
	{
		public static void Load()
		{
			Main.playerTextures = new Texture2D[8, 15];
			LoadStarterMale();
			LoadStarterFemale();
			LoadStickerMale();
			LoadStickerFemale();
			LoadGangsterMale();
			LoadGangsterFemale();
			LoadFancyMale();
			LoadFancyFemale();
		}

		private static void LoadDebugs()
		{
			CopyVariant(8, 0);
			CopyVariant(9, 4);
			for (int i = 8; i < 10; i++)
			{
				Main.playerTextures[i, 4] = Main.armorArmTexture[191];
				Main.playerTextures[i, 6] = Main.armorArmTexture[191];
				Main.playerTextures[i, 11] = Main.armorArmTexture[191];
				Main.playerTextures[i, 12] = Main.armorArmTexture[191];
				Main.playerTextures[i, 13] = Main.armorArmTexture[191];
				Main.playerTextures[i, 8] = Main.armorArmTexture[191];
			}
		}

		private static void LoadVariant(int ID, int[] pieceIDs)
		{
			for (int i = 0; i < pieceIDs.Length; i++)
			{
				Main.playerTextures[ID, pieceIDs[i]] = TextureManager.Load("Images/Player_" + ID + "_" + pieceIDs[i]);
			}
		}

		private static void CopyVariant(int to, int from)
		{
			for (int i = 0; i < 15; i++)
			{
				Main.playerTextures[to, i] = Main.playerTextures[from, i];
			}
		}

		private static void LoadStarterMale()
		{
			LoadVariant(0, new int[14]
			{
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13
			});
			Main.playerTextures[0, 14] = TextureManager.BlankTexture;
		}

		private static void LoadStickerMale()
		{
			CopyVariant(1, 0);
			LoadVariant(1, new int[6]
			{
				4,
				6,
				8,
				11,
				12,
				13
			});
		}

		private static void LoadGangsterMale()
		{
			CopyVariant(2, 0);
			LoadVariant(2, new int[6]
			{
				4,
				6,
				8,
				11,
				12,
				13
			});
		}

		private static void LoadFancyMale()
		{
			CopyVariant(3, 0);
			LoadVariant(3, new int[7]
			{
				4,
				6,
				8,
				11,
				12,
				13,
				14
			});
		}

		private static void LoadStarterFemale()
		{
			CopyVariant(4, 0);
			LoadVariant(4, new int[11]
			{
				3,
				4,
				5,
				6,
				7,
				8,
				9,
				10,
				11,
				12,
				13
			});
		}

		private static void LoadStickerFemale()
		{
			CopyVariant(5, 4);
			LoadVariant(5, new int[6]
			{
				4,
				6,
				8,
				11,
				12,
				13
			});
		}

		private static void LoadGangsterFemale()
		{
			CopyVariant(6, 4);
			LoadVariant(6, new int[6]
			{
				4,
				6,
				8,
				11,
				12,
				13
			});
		}

		private static void LoadFancyFemale()
		{
			CopyVariant(7, 4);
			LoadVariant(7, new int[6]
			{
				4,
				6,
				8,
				11,
				12,
				13
			});
		}
	}
}
