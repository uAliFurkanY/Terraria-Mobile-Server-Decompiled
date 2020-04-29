using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Terraria.Graphics
{
	public class TileBatch
	{
		private struct SpriteData
		{
			public Vector4 Source;

			public Vector4 Destination;

			public Vector2 Origin;

			public SpriteEffects Effects;

			public VertexColors Colors;
		}

		private static float[] xCornerOffsets = new float[4]
		{
			0f,
			1f,
			1f,
			0f
		};

		private static float[] yCornerOffsets = new float[4]
		{
			0f,
			0f,
			1f,
			1f
		};

		private GraphicsDevice graphicsDevice;

		private SpriteData[] spriteDataQueue = new SpriteData[2048];

		private Texture2D[] spriteTextures;

		private int queuedSpriteCount;

		private SpriteBatch _spriteBatch;

		private static Vector2 vector2Zero;

		private static Rectangle? nullRectangle;

		private DynamicVertexBuffer vertexBuffer;

		private DynamicIndexBuffer indexBuffer;

		private short[] fallbackIndexData;

		private VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[8192];

		private int vertexBufferPosition;

		public TileBatch(GraphicsDevice graphicsDevice)
		{
			this.graphicsDevice = graphicsDevice;
			_spriteBatch = new SpriteBatch(graphicsDevice);
			Allocate();
		}

		private void Allocate()
		{
			if (vertexBuffer == null || vertexBuffer.IsDisposed)
			{
				vertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(VertexPositionColorTexture), 8192, BufferUsage.WriteOnly);
				vertexBufferPosition = 0;
				vertexBuffer.ContentLost += delegate
				{
					vertexBufferPosition = 0;
				};
			}
			if (indexBuffer != null && !indexBuffer.IsDisposed)
			{
				return;
			}
			if (fallbackIndexData == null)
			{
				fallbackIndexData = new short[12288];
				for (int i = 0; i < 2048; i++)
				{
					fallbackIndexData[i * 6] = (short)(i * 4);
					fallbackIndexData[i * 6 + 1] = (short)(i * 4 + 1);
					fallbackIndexData[i * 6 + 2] = (short)(i * 4 + 2);
					fallbackIndexData[i * 6 + 3] = (short)(i * 4);
					fallbackIndexData[i * 6 + 4] = (short)(i * 4 + 2);
					fallbackIndexData[i * 6 + 5] = (short)(i * 4 + 3);
				}
			}
			indexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(short), 12288, BufferUsage.WriteOnly);
			indexBuffer.SetData(fallbackIndexData);
			indexBuffer.ContentLost += delegate
			{
				indexBuffer.SetData(fallbackIndexData);
			};
		}

		private void FlushRenderState()
		{
			Allocate();
			graphicsDevice.SetVertexBuffer(vertexBuffer);
			graphicsDevice.Indices = indexBuffer;
			graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		}

		public void Dispose()
		{
			if (vertexBuffer != null)
			{
				vertexBuffer.Dispose();
			}
			if (indexBuffer != null)
			{
				indexBuffer.Dispose();
			}
		}

		public void Begin()
		{
			_spriteBatch.Begin();
			_spriteBatch.End();
		}

		public void Draw(Texture2D texture, Vector2 position, VertexColors colors)
		{
			Vector4 destination = default(Vector4);
			destination.X = position.X;
			destination.Y = position.Y;
			destination.Z = 1f;
			destination.W = 1f;
			InternalDraw(texture, ref destination, true, ref nullRectangle, ref colors, ref vector2Zero, SpriteEffects.None);
		}

		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, VertexColors colors, Vector2 origin, float scale, SpriteEffects effects)
		{
			Vector4 destination = default(Vector4);
			destination.X = position.X;
			destination.Y = position.Y;
			destination.Z = scale;
			destination.W = scale;
			InternalDraw(texture, ref destination, true, ref sourceRectangle, ref colors, ref origin, effects);
		}

		public void Draw(Texture2D texture, Vector4 destination, VertexColors colors)
		{
			InternalDraw(texture, ref destination, false, ref nullRectangle, ref colors, ref vector2Zero, SpriteEffects.None);
		}

		public void Draw(Texture2D texture, Vector2 position, VertexColors colors, Vector2 scale)
		{
			Vector4 destination = default(Vector4);
			destination.X = position.X;
			destination.Y = position.Y;
			destination.Z = scale.X;
			destination.W = scale.Y;
			InternalDraw(texture, ref destination, true, ref nullRectangle, ref colors, ref vector2Zero, SpriteEffects.None);
		}

		public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, VertexColors colors)
		{
			Vector4 destination = default(Vector4);
			destination.X = destinationRectangle.X;
			destination.Y = destinationRectangle.Y;
			destination.Z = destinationRectangle.Width;
			destination.W = destinationRectangle.Height;
			InternalDraw(texture, ref destination, false, ref sourceRectangle, ref colors, ref vector2Zero, SpriteEffects.None);
		}

		private static short[] CreateIndexData()
		{
			short[] array = new short[12288];
			for (int i = 0; i < 2048; i++)
			{
				array[i * 6] = (short)(i * 4);
				array[i * 6 + 1] = (short)(i * 4 + 1);
				array[i * 6 + 2] = (short)(i * 4 + 2);
				array[i * 6 + 3] = (short)(i * 4);
				array[i * 6 + 4] = (short)(i * 4 + 2);
				array[i * 6 + 5] = (short)(i * 4 + 3);
			}
			return array;
		}

		private unsafe void InternalDraw(Texture2D texture, ref Vector4 destination, bool scaleDestination, ref Rectangle? sourceRectangle, ref VertexColors colors, ref Vector2 origin, SpriteEffects effects)
		{
			if (queuedSpriteCount >= spriteDataQueue.Length)
			{
				Array.Resize(ref spriteDataQueue, spriteDataQueue.Length << 1);
			}
			fixed (SpriteData* ptr = &spriteDataQueue[queuedSpriteCount])
			{
				float num = destination.Z;
				float num2 = destination.W;
				if (sourceRectangle.HasValue)
				{
					Rectangle value = sourceRectangle.Value;
					ptr->Source.X = value.X;
					ptr->Source.Y = value.Y;
					ptr->Source.Z = value.Width;
					ptr->Source.W = value.Height;
					if (scaleDestination)
					{
						num *= (float)value.Width;
						num2 *= (float)value.Height;
					}
				}
				else
				{
					float num3 = texture.Width;
					float num4 = texture.Height;
					ptr->Source.X = 0f;
					ptr->Source.Y = 0f;
					ptr->Source.Z = num3;
					ptr->Source.W = num4;
					if (scaleDestination)
					{
						num *= num3;
						num2 *= num4;
					}
				}
				ptr->Destination.X = destination.X;
				ptr->Destination.Y = destination.Y;
				ptr->Destination.Z = num;
				ptr->Destination.W = num2;
				ptr->Origin.X = origin.X;
				ptr->Origin.Y = origin.Y;
				ptr->Effects = effects;
				ptr->Colors = colors;
			}
			if (spriteTextures == null || spriteTextures.Length != spriteDataQueue.Length)
			{
				Array.Resize(ref spriteTextures, spriteDataQueue.Length);
			}
			spriteTextures[queuedSpriteCount++] = texture;
		}

		public void End()
		{
			if (queuedSpriteCount != 0)
			{
				FlushRenderState();
				Flush();
			}
		}

		private void Flush()
		{
			Texture2D texture2D = null;
			int num = 0;
			for (int i = 0; i < queuedSpriteCount; i++)
			{
				if (spriteTextures[i] != texture2D)
				{
					if (i > num)
					{
						RenderBatch(texture2D, spriteDataQueue, num, i - num);
					}
					num = i;
					texture2D = spriteTextures[i];
				}
			}
			RenderBatch(texture2D, spriteDataQueue, num, queuedSpriteCount - num);
			Array.Clear(spriteTextures, 0, queuedSpriteCount);
			queuedSpriteCount = 0;
		}

		private unsafe void RenderBatch(Texture2D texture, SpriteData[] sprites, int offset, int count)
		{
			graphicsDevice.Textures[0] = texture;
			float num = 1f / (float)texture.Width;
			float num2 = 1f / (float)texture.Height;
			while (count > 0)
			{
				SetDataOptions options = SetDataOptions.NoOverwrite;
				int num3 = count;
				if (num3 > 2048 - vertexBufferPosition)
				{
					num3 = 2048 - vertexBufferPosition;
					if (num3 < 256)
					{
						vertexBufferPosition = 0;
						options = SetDataOptions.Discard;
						num3 = count;
						if (num3 > 2048)
						{
							num3 = 2048;
						}
					}
				}
				fixed (SpriteData* ptr = &sprites[offset])
				{
					fixed (VertexPositionColorTexture* ptr3 = &vertices[0])
					{
						SpriteData* ptr2 = ptr;
						VertexPositionColorTexture* ptr4 = ptr3;
						for (int i = 0; i < num3; i++)
						{
							float num4 = ptr2->Origin.X / ptr2->Source.Z;
							float num5 = ptr2->Origin.Y / ptr2->Source.W;
							ptr4->Color = ptr2->Colors.TopLeftColor;
							ptr4[1].Color = ptr2->Colors.TopRightColor;
							ptr4[2].Color = ptr2->Colors.BottomRightColor;
							ptr4[3].Color = ptr2->Colors.BottomLeftColor;
							for (int j = 0; j < 4; j++)
							{
								float num6 = xCornerOffsets[j];
								float num7 = yCornerOffsets[j];
								float num8 = (num6 - num4) * ptr2->Destination.Z;
								float num9 = (num7 - num5) * ptr2->Destination.W;
								float x = ptr2->Destination.X + num8;
								float y = ptr2->Destination.Y + num9;
								if ((ptr2->Effects & SpriteEffects.FlipVertically) != 0)
								{
									num6 = 1f - num6;
								}
								if ((ptr2->Effects & SpriteEffects.FlipHorizontally) != 0)
								{
									num7 = 1f - num7;
								}
								ptr4->Position.X = x;
								ptr4->Position.Y = y;
								ptr4->Position.Z = 0f;
								ptr4->TextureCoordinate.X = (ptr2->Source.X + num6 * ptr2->Source.Z) * num;
								ptr4->TextureCoordinate.Y = (ptr2->Source.Y + num7 * ptr2->Source.W) * num2;
								ptr4++;
							}
							ptr2++;
						}
					}
				}
				int offsetInBytes = vertexBufferPosition * sizeof(VertexPositionColorTexture) * 4;
				vertexBuffer.SetData(offsetInBytes, vertices, 0, num3 * 4, sizeof(VertexPositionColorTexture), options);
				int minVertexIndex = vertexBufferPosition * 4;
				int numVertices = num3 * 4;
				int startIndex = vertexBufferPosition * 6;
				int primitiveCount = num3 * 2;
				graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, minVertexIndex, numVertices, startIndex, primitiveCount);
				vertexBufferPosition += num3;
				offset += num3;
				count -= num3;
			}
		}
	}
}
