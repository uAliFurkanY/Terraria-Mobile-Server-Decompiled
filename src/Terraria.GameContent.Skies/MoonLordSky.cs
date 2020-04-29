using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Graphics.Effects;

namespace Terraria.GameContent.Skies
{
	internal class MoonLordSky : CustomSky
	{
		private Random _random = new Random();

		private bool _isActive;

		private int _moonLordIndex = -1;

		public override void OnLoad()
		{
		}

		public override void Update()
		{
		}

		private float GetIntensity()
		{
			if (UpdateMoonLordIndex())
			{
				float x = 0f;
				if (_moonLordIndex != -1)
				{
					x = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[_moonLordIndex].Center);
				}
				return 1f - Utils.SmoothStep(3000f, 6000f, x);
			}
			return 0f;
		}

		public override Color OnTileColor(Color inColor)
		{
			float ıntensity = GetIntensity();
			return new Color(Vector4.Lerp(new Vector4(0.5f, 0.8f, 1f, 1f), inColor.ToVector4(), 1f - ıntensity));
		}

		private bool UpdateMoonLordIndex()
		{
			if (_moonLordIndex >= 0 && Main.npc[_moonLordIndex].active && Main.npc[_moonLordIndex].type == 398)
			{
				return true;
			}
			int num = -1;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == 398)
				{
					num = i;
					break;
				}
			}
			_moonLordIndex = num;
			return num != -1;
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (maxDepth >= 0f && minDepth < 0f)
			{
				float ıntensity = GetIntensity();
				spriteBatch.Draw(Main.blackTileTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * ıntensity);
			}
		}

		public override float GetCloudAlpha()
		{
			return 0f;
		}

		internal override void Activate(Vector2 position, params object[] args)
		{
			_isActive = true;
		}

		internal override void Deactivate(params object[] args)
		{
			_isActive = false;
		}

		public override void Reset()
		{
			_isActive = false;
		}

		public override bool IsActive()
		{
			return _isActive;
		}
	}
}
