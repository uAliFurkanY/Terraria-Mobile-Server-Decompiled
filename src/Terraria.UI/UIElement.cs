using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Terraria.UI
{
	public class UIElement : IComparable
	{
		public delegate void MouseEvent(UIMouseEvent evt, UIElement listeningElement);

		public delegate void ScrollWheelEvent(UIScrollWheelEvent evt, UIElement listeningElement);

		public string Id = "";

		public UIElement Parent;

		protected List<UIElement> Elements = new List<UIElement>();

		public StyleDimension Top;

		public StyleDimension Left;

		public StyleDimension Width;

		public StyleDimension Height;

		public StyleDimension MaxWidth = StyleDimension.Fill;

		public StyleDimension MaxHeight = StyleDimension.Fill;

		public StyleDimension MinWidth = StyleDimension.Empty;

		public StyleDimension MinHeight = StyleDimension.Empty;

		private bool _isInitialized;

		public bool OverflowHidden;

		public float PaddingTop;

		public float PaddingLeft;

		public float PaddingRight;

		public float PaddingBottom;

		public float HAlign;

		public float VAlign;

		private CalculatedStyle _innerDimensions;

		private CalculatedStyle _dimensions;

		private static RasterizerState _overflowHiddenRasterizerState;

		protected bool _useImmediateMode;

		private bool _isMouseHovering;

		public bool IsMouseHovering => _isMouseHovering;

		public event MouseEvent OnMouseDown;

		public event MouseEvent OnMouseUp;

		public event MouseEvent OnClick;

		public event MouseEvent OnMouseOver;

		public event MouseEvent OnMouseOut;

		public event MouseEvent OnDoubleClick;

		public event ScrollWheelEvent OnScrollWheel;

		public UIElement()
		{
			if (_overflowHiddenRasterizerState == null)
			{
				_overflowHiddenRasterizerState = new RasterizerState
				{
					CullMode = CullMode.None,
					ScissorTestEnable = true
				};
			}
		}

		protected virtual void DrawSelf(SpriteBatch spriteBatch)
		{
		}

		protected virtual void DrawChildren(SpriteBatch spriteBatch)
		{
			foreach (UIElement element in Elements)
			{
				element.Draw(spriteBatch);
			}
		}

		public void Append(UIElement element)
		{
			element.Remove();
			element.Parent = this;
			Elements.Add(element);
			element.Recalculate();
		}

		public void Remove()
		{
			if (Parent != null)
			{
				Parent.RemoveChild(this);
			}
		}

		public void RemoveChild(UIElement child)
		{
			Elements.Remove(child);
			child.Parent = null;
		}

		public void RemoveAllChildren()
		{
			foreach (UIElement element in Elements)
			{
				element.Parent = null;
			}
			Elements.Clear();
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			bool overflowHidden = OverflowHidden;
			bool useImmediateMode = _useImmediateMode;
			RasterizerState rasterizerState = spriteBatch.GraphicsDevice.RasterizerState;
			Rectangle scissorRectangle = spriteBatch.GraphicsDevice.ScissorRectangle;
			if (useImmediateMode)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, _overflowHiddenRasterizerState);
				DrawSelf(spriteBatch);
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, _overflowHiddenRasterizerState);
			}
			else
			{
				DrawSelf(spriteBatch);
			}
			if (overflowHidden)
			{
				spriteBatch.End();
				Rectangle scissorRectangle2 = new Rectangle((int)_innerDimensions.X, (int)_innerDimensions.Y, (int)_innerDimensions.Width, (int)_innerDimensions.Height);
				int width = spriteBatch.GraphicsDevice.Viewport.Width;
				int height = spriteBatch.GraphicsDevice.Viewport.Height;
				scissorRectangle2.X = Utils.Clamp(scissorRectangle2.X, 0, width);
				scissorRectangle2.Y = Utils.Clamp(scissorRectangle2.Y, 0, height);
				scissorRectangle2.Width = Utils.Clamp(scissorRectangle2.Width, 0, width - scissorRectangle2.X);
				scissorRectangle2.Height = Utils.Clamp(scissorRectangle2.Height, 0, height - scissorRectangle2.Y);
				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, _overflowHiddenRasterizerState);
			}
			DrawChildren(spriteBatch);
			if (overflowHidden)
			{
				spriteBatch.End();
				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState);
			}
		}

		public virtual void Recalculate()
		{
			CalculatedStyle calculatedStyle = (Parent == null) ? UserInterface.ActiveInstance.GetDimensions() : Parent.GetInnerDimensions();
			CalculatedStyle calculatedStyle2 = default(CalculatedStyle);
			calculatedStyle2.X = Left.GetValue(calculatedStyle.Width) + calculatedStyle.X;
			calculatedStyle2.Y = Top.GetValue(calculatedStyle.Height) + calculatedStyle.Y;
			float value = MinWidth.GetValue(calculatedStyle.Width);
			float value2 = MaxWidth.GetValue(calculatedStyle.Width);
			float value3 = MinHeight.GetValue(calculatedStyle.Height);
			float value4 = MaxHeight.GetValue(calculatedStyle.Height);
			calculatedStyle2.Width = MathHelper.Clamp(Width.GetValue(calculatedStyle.Width), value, value2);
			calculatedStyle2.Height = MathHelper.Clamp(Height.GetValue(calculatedStyle.Height), value3, value4);
			calculatedStyle2.X += calculatedStyle.Width * HAlign - calculatedStyle2.Width * HAlign;
			calculatedStyle2.Y += calculatedStyle.Height * VAlign - calculatedStyle2.Height * VAlign;
			_dimensions = calculatedStyle2;
			calculatedStyle2.X += PaddingLeft;
			calculatedStyle2.Y += PaddingTop;
			calculatedStyle2.Width -= PaddingLeft + PaddingRight;
			calculatedStyle2.Height -= PaddingTop + PaddingBottom;
			_innerDimensions = calculatedStyle2;
			RecalculateChildren();
		}

		public UIElement GetElementAt(Vector2 point)
		{
			UIElement uIElement = null;
			foreach (UIElement element in Elements)
			{
				if (element.ContainsPoint(point))
				{
					uIElement = element;
					break;
				}
			}
			if (uIElement != null)
			{
				return uIElement.GetElementAt(point);
			}
			if (ContainsPoint(point))
			{
				return this;
			}
			return null;
		}

		public virtual bool ContainsPoint(Vector2 point)
		{
			if (point.X > _dimensions.X && point.Y > _dimensions.Y && point.X < _dimensions.X + _dimensions.Width)
			{
				return point.Y < _dimensions.Y + _dimensions.Height;
			}
			return false;
		}

		public void SetPadding(float pixels)
		{
			PaddingBottom = pixels;
			PaddingLeft = pixels;
			PaddingRight = pixels;
			PaddingTop = pixels;
		}

		public virtual void RecalculateChildren()
		{
			foreach (UIElement element in Elements)
			{
				element.Recalculate();
			}
		}

		public CalculatedStyle GetInnerDimensions()
		{
			return _innerDimensions;
		}

		public CalculatedStyle GetDimensions()
		{
			return _dimensions;
		}

		public void CopyStyle(UIElement element)
		{
			Top = element.Top;
			Left = element.Left;
			Width = element.Width;
			Height = element.Height;
			PaddingBottom = element.PaddingBottom;
			PaddingLeft = element.PaddingLeft;
			PaddingRight = element.PaddingRight;
			PaddingTop = element.PaddingTop;
			HAlign = element.HAlign;
			VAlign = element.VAlign;
			MinWidth = element.MinWidth;
			MaxWidth = element.MaxWidth;
			MinHeight = element.MinHeight;
			MaxHeight = element.MaxHeight;
			Recalculate();
		}

		public virtual void MouseDown(UIMouseEvent evt)
		{
			if (this.OnMouseDown != null)
			{
				this.OnMouseDown(evt, this);
			}
			if (Parent != null)
			{
				Parent.MouseDown(evt);
			}
		}

		public virtual void MouseUp(UIMouseEvent evt)
		{
			if (this.OnMouseUp != null)
			{
				this.OnMouseUp(evt, this);
			}
			if (Parent != null)
			{
				Parent.MouseUp(evt);
			}
		}

		public virtual void MouseOver(UIMouseEvent evt)
		{
			_isMouseHovering = true;
			if (this.OnMouseOver != null)
			{
				this.OnMouseOver(evt, this);
			}
			if (Parent != null)
			{
				Parent.MouseOver(evt);
			}
		}

		public virtual void MouseOut(UIMouseEvent evt)
		{
			_isMouseHovering = false;
			if (this.OnMouseOut != null)
			{
				this.OnMouseOut(evt, this);
			}
			if (Parent != null)
			{
				Parent.MouseOut(evt);
			}
		}

		public virtual void Click(UIMouseEvent evt)
		{
			if (this.OnClick != null)
			{
				this.OnClick(evt, this);
			}
			if (Parent != null)
			{
				Parent.Click(evt);
			}
		}

		public virtual void DoubleClick(UIMouseEvent evt)
		{
			if (this.OnDoubleClick != null)
			{
				this.OnDoubleClick(evt, this);
			}
			if (Parent != null)
			{
				Parent.DoubleClick(evt);
			}
		}

		public virtual void ScrollWheel(UIScrollWheelEvent evt)
		{
			if (this.OnScrollWheel != null)
			{
				this.OnScrollWheel(evt, this);
			}
			if (Parent != null)
			{
				Parent.ScrollWheel(evt);
			}
		}

		public void Activate()
		{
			if (!_isInitialized)
			{
				Initialize();
			}
			OnActivate();
			foreach (UIElement element in Elements)
			{
				element.Activate();
			}
		}

		public virtual void OnActivate()
		{
		}

		public void Deactivate()
		{
			OnDeactivate();
			foreach (UIElement element in Elements)
			{
				element.Deactivate();
			}
		}

		public virtual void OnDeactivate()
		{
		}

		public void Initialize()
		{
			OnInitialize();
			_isInitialized = true;
		}

		public virtual void OnInitialize()
		{
		}

		public virtual int CompareTo(object obj)
		{
			return 0;
		}
	}
}
