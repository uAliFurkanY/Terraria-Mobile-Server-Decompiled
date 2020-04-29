using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.UI;

namespace Terraria.GameContent.UI.Elements
{
	internal class UIList : UIElement
	{
		public delegate bool ElementSearchMethod(UIElement element);

		private class UIInnerList : UIElement
		{
			public override bool ContainsPoint(Vector2 point)
			{
				return true;
			}

			protected override void DrawChildren(SpriteBatch spriteBatch)
			{
				Vector2 position = Parent.GetDimensions().Position();
				Vector2 dimensions = new Vector2(Parent.GetDimensions().Width, Parent.GetDimensions().Height);
				foreach (UIElement element in Elements)
				{
					Vector2 position2 = element.GetDimensions().Position();
					Vector2 dimensions2 = new Vector2(element.GetDimensions().Width, element.GetDimensions().Height);
					if (Collision.CheckAABBvAABBCollision(position, dimensions, position2, dimensions2))
					{
						element.Draw(spriteBatch);
					}
				}
			}
		}

		protected List<UIElement> _items = new List<UIElement>();

		protected UIScrollbar _scrollbar;

		private UIElement _innerList = new UIInnerList();

		private float _innerListHeight;

		public float ListPadding = 5f;

		public int Count => _items.Count;

		public UIList()
		{
			_innerList.OverflowHidden = false;
			_innerList.Width.Set(0f, 1f);
			_innerList.Height.Set(0f, 1f);
			OverflowHidden = true;
			Append(_innerList);
		}

		public void Goto(ElementSearchMethod searchMethod)
		{
			int num = 0;
			while (true)
			{
				if (num < _items.Count)
				{
					if (searchMethod(_items[num]))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_scrollbar.ViewPosition = _items[num].Top.Pixels;
		}

		public virtual void Add(UIElement item)
		{
			_items.Add(item);
			_innerList.Append(item);
			UpdateOrder();
			_innerList.Recalculate();
		}

		public virtual bool Remove(UIElement item)
		{
			_innerList.RemoveChild(item);
			UpdateOrder();
			return _items.Remove(item);
		}

		public virtual void Clear()
		{
			_innerList.RemoveAllChildren();
			_items.Clear();
		}

		public override void Recalculate()
		{
			base.Recalculate();
			UpdateScrollbar();
		}

		public override void ScrollWheel(UIScrollWheelEvent evt)
		{
			base.ScrollWheel(evt);
			if (_scrollbar != null)
			{
				_scrollbar.ViewPosition -= evt.ScrollWheelValue;
			}
		}

		public override void RecalculateChildren()
		{
			base.RecalculateChildren();
			float num = 0f;
			for (int i = 0; i < _items.Count; i++)
			{
				_items[i].Top.Set(num, 0f);
				_items[i].Recalculate();
				CalculatedStyle dimensions = _items[i].GetDimensions();
				num += dimensions.Height + ListPadding;
			}
			_innerListHeight = num;
		}

		private void UpdateScrollbar()
		{
			if (_scrollbar != null)
			{
				_scrollbar.SetView(GetInnerDimensions().Height, _innerListHeight);
			}
		}

		public void SetScrollbar(UIScrollbar scrollbar)
		{
			_scrollbar = scrollbar;
			UpdateScrollbar();
		}

		public void UpdateOrder()
		{
			_items.Sort(SortMethod);
			UpdateScrollbar();
		}

		public int SortMethod(UIElement item1, UIElement item2)
		{
			return item1.CompareTo(item2);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (_scrollbar != null)
			{
				_innerList.Top.Set(0f - _scrollbar.GetValue(), 0f);
			}
			Recalculate();
		}
	}
}
