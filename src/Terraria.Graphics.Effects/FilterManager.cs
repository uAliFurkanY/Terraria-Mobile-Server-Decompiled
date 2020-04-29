using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Terraria.Graphics.Effects
{
	internal class FilterManager : EffectManager<Filter>
	{
		private const float OPACITY_RATE = 0.05f;

		private LinkedList<Filter> _activeFilters = new LinkedList<Filter>();

		public override void OnActivate(Filter effect, Vector2 position)
		{
			if (_activeFilters.Contains(effect))
			{
				if (effect.Active)
				{
					return;
				}
				_activeFilters.Remove(effect);
			}
			else
			{
				effect.Opacity = 0f;
			}
			if (_activeFilters.Count == 0)
			{
				_activeFilters.AddLast(effect);
				return;
			}
			for (LinkedListNode<Filter> linkedListNode = _activeFilters.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				Filter value = linkedListNode.Value;
				if (effect.Priority < value.Priority)
				{
					_activeFilters.AddBefore(linkedListNode, effect);
					return;
				}
			}
			_activeFilters.AddLast(effect);
		}

		public void Apply()
		{
			LinkedListNode<Filter> linkedListNode = _activeFilters.First;
			int num = 0;
			int count = _activeFilters.Count;
			while (linkedListNode != null)
			{
				num++;
				Filter value = linkedListNode.Value;
				LinkedListNode<Filter> linkedListNode2 = linkedListNode.Next;
				if (value.Opacity > 0f || num == count)
				{
					value.Apply();
					if (num == count && value.Active)
					{
						value.Opacity = Math.Min(value.Opacity + 0.05f, 1f);
					}
					else
					{
						value.Opacity = Math.Max(value.Opacity - 0.05f, 0f);
					}
					linkedListNode2 = null;
				}
				if (!value.Active && value.Opacity == 0f)
				{
					_activeFilters.Remove(linkedListNode);
				}
				linkedListNode = linkedListNode2;
			}
		}

		public bool HasActiveFilter()
		{
			return _activeFilters.Count != 0;
		}
	}
}
