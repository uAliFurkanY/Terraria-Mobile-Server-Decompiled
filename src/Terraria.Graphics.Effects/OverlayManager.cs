using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Terraria.Graphics.Effects
{
	internal class OverlayManager : EffectManager<Overlay>
	{
		private const float OPACITY_RATE = 0.05f;

		private LinkedList<Overlay>[] _activeOverlays = new LinkedList<Overlay>[Enum.GetNames(typeof(EffectPriority)).Length];

		public OverlayManager()
		{
			for (int i = 0; i < _activeOverlays.Length; i++)
			{
				_activeOverlays[i] = new LinkedList<Overlay>();
			}
		}

		public override void OnActivate(Overlay overlay, Vector2 position)
		{
			LinkedList<Overlay> linkedList = _activeOverlays[(int)overlay.Priority];
			if (overlay.Mode != 0 && overlay.Mode != OverlayMode.Active)
			{
				if (overlay.Mode == OverlayMode.FadeOut)
				{
					linkedList.Remove(overlay);
				}
				else
				{
					overlay.Opacity = 0f;
				}
				if (linkedList.Count != 0)
				{
					foreach (Overlay item in linkedList)
					{
						item.Mode = OverlayMode.FadeOut;
					}
				}
				linkedList.AddLast(overlay);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			Overlay overlay = null;
			for (int i = 0; i < _activeOverlays.Length; i++)
			{
				foreach (Overlay item in _activeOverlays[i])
				{
					if (item.Mode == OverlayMode.Active)
					{
						overlay = item;
					}
				}
			}
			for (int j = 0; j < _activeOverlays.Length; j++)
			{
				LinkedListNode<Overlay> linkedListNode = _activeOverlays[j].First;
				while (linkedListNode != null)
				{
					Overlay value = linkedListNode.Value;
					value.Draw(spriteBatch);
					LinkedListNode<Overlay> next = linkedListNode.Next;
					switch (value.Mode)
					{
					case OverlayMode.Active:
						if (overlay != null && value != overlay)
						{
							value.Opacity = Math.Max(0f, value.Opacity - 0.05f);
						}
						else
						{
							value.Opacity = Math.Min(1f, value.Opacity + 0.05f);
						}
						break;
					case OverlayMode.FadeIn:
						value.Opacity += 0.05f;
						if (value.Opacity >= 1f)
						{
							value.Opacity = 1f;
							value.Mode = OverlayMode.Active;
						}
						break;
					case OverlayMode.FadeOut:
						value.Opacity -= 0.05f;
						if (value.Opacity <= 0f)
						{
							value.Opacity = 0f;
							value.Mode = OverlayMode.Inactive;
							_activeOverlays[j].Remove(linkedListNode);
						}
						break;
					}
					linkedListNode = next;
				}
			}
		}
	}
}
