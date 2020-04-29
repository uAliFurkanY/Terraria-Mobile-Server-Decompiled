using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Terraria.Graphics.Capture
{
	internal class CaptureCamera
	{
		private class CaptureChunk
		{
			public readonly Microsoft.Xna.Framework.Rectangle Area;

			public readonly Microsoft.Xna.Framework.Rectangle ScaledArea;

			public CaptureChunk(Microsoft.Xna.Framework.Rectangle area, Microsoft.Xna.Framework.Rectangle scaledArea)
			{
				Area = area;
				ScaledArea = scaledArea;
			}
		}

		public const int CHUNK_SIZE = 128;

		public const int FRAMEBUFFER_PIXEL_SIZE = 2048;

		public const int INNER_CHUNK_SIZE = 126;

		public const int MAX_IMAGE_SIZE = 4096;

		public const string CAPTURE_DIRECTORY = "Captures";

		private static bool CameraExists;

		private RenderTarget2D _frameBuffer;

		private RenderTarget2D _scaledFrameBuffer;

		private GraphicsDevice _graphics;

		private readonly object _captureLock = new object();

		private bool _isDisposed;

		private CaptureSettings _activeSettings;

		private Queue<CaptureChunk> _renderQueue = new Queue<CaptureChunk>();

		private SpriteBatch _spriteBatch;

		private byte[] _scaledFrameData;

		private byte[] _outputData;

		private Size _outputImageSize;

		private SamplerState _downscaleSampleState;

		private float _tilesProcessed;

		private float _totalTiles;

		public bool IsCapturing
		{
			get
			{
				Monitor.Enter(_captureLock);
				bool result = _activeSettings != null;
				Monitor.Exit(_captureLock);
				return result;
			}
		}

		public CaptureCamera(GraphicsDevice graphics)
		{
			CameraExists = true;
			_graphics = graphics;
			_spriteBatch = new SpriteBatch(graphics);
			try
			{
				_frameBuffer = new RenderTarget2D(graphics, 2048, 2048, false, graphics.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
			}
			catch
			{
				Main.CaptureModeDisabled = true;
				return;
			}
			_downscaleSampleState = SamplerState.AnisotropicClamp;
		}

		~CaptureCamera()
		{
			Dispose();
		}

		public void Capture(CaptureSettings settings)
		{
			Main.GlobalTimerPaused = true;
			Monitor.Enter(_captureLock);
			if (_activeSettings != null)
			{
				throw new InvalidOperationException("Capture called while another capture was already active.");
			}
			_activeSettings = settings;
			Microsoft.Xna.Framework.Rectangle area = settings.Area;
			float num = 1f;
			if (settings.UseScaling)
			{
				if (area.Width << 4 > 4096)
				{
					num = 4096f / (float)(area.Width << 4);
				}
				if (area.Height << 4 > 4096)
				{
					num = Math.Min(num, 4096f / (float)(area.Height << 4));
				}
				num = Math.Min(1f, num);
				_outputImageSize = new Size((int)MathHelper.Clamp((int)(num * (float)(area.Width << 4)), 1f, 4096f), (int)MathHelper.Clamp((int)(num * (float)(area.Height << 4)), 1f, 4096f));
				_outputData = new byte[4 * _outputImageSize.Width * _outputImageSize.Height];
				int num2 = (int)Math.Floor(num * 2048f);
				_scaledFrameData = new byte[4 * num2 * num2];
				_scaledFrameBuffer = new RenderTarget2D(_graphics, num2, num2, false, _graphics.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
			}
			else
			{
				_outputData = new byte[16777216];
			}
			_tilesProcessed = 0f;
			_totalTiles = area.Width * area.Height;
			for (int i = area.X; i < area.X + area.Width; i += 126)
			{
				for (int j = area.Y; j < area.Y + area.Height; j += 126)
				{
					int num3 = Math.Min(128, area.X + area.Width - i);
					int num4 = Math.Min(128, area.Y + area.Height - j);
					int width = (int)Math.Floor(num * (float)(num3 << 4));
					int height = (int)Math.Floor(num * (float)(num4 << 4));
					int x = (int)Math.Floor(num * (float)(i - area.X << 4));
					int y = (int)Math.Floor(num * (float)(j - area.Y << 4));
					_renderQueue.Enqueue(new CaptureChunk(new Microsoft.Xna.Framework.Rectangle(i, j, num3, num4), new Microsoft.Xna.Framework.Rectangle(x, y, width, height)));
				}
			}
			Monitor.Exit(_captureLock);
		}

		public void DrawTick()
		{
			Monitor.Enter(_captureLock);
			if (_activeSettings != null)
			{
				CaptureChunk captureChunk = _renderQueue.Dequeue();
				_graphics.SetRenderTarget(_frameBuffer);
				_graphics.Clear(Microsoft.Xna.Framework.Color.Transparent);
				Main.instance.DrawCapture(captureChunk.Area, _activeSettings);
				if (_activeSettings.UseScaling)
				{
					_graphics.SetRenderTarget(_scaledFrameBuffer);
					_graphics.Clear(Microsoft.Xna.Framework.Color.Transparent);
					_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, _downscaleSampleState, DepthStencilState.Default, RasterizerState.CullNone);
					_spriteBatch.Draw(_frameBuffer, new Microsoft.Xna.Framework.Rectangle(0, 0, _scaledFrameBuffer.Width, _scaledFrameBuffer.Height), Microsoft.Xna.Framework.Color.White);
					_spriteBatch.End();
					_graphics.SetRenderTarget(null);
					_scaledFrameBuffer.GetData(_scaledFrameData, 0, _scaledFrameBuffer.Width * _scaledFrameBuffer.Height * 4);
					DrawBytesToBuffer(_scaledFrameData, _outputData, _scaledFrameBuffer.Width, _outputImageSize.Width, captureChunk.ScaledArea);
				}
				else
				{
					_graphics.SetRenderTarget(null);
					SaveImage(_frameBuffer, captureChunk.ScaledArea.Width, captureChunk.ScaledArea.Height, ImageFormat.Png, _activeSettings.OutputName, captureChunk.Area.X + "-" + captureChunk.Area.Y + ".png");
				}
				_tilesProcessed += captureChunk.Area.Width * captureChunk.Area.Height;
				if (_renderQueue.Count == 0)
				{
					FinishCapture();
				}
				Monitor.Exit(_captureLock);
			}
		}

		private unsafe void DrawBytesToBuffer(byte[] sourceBuffer, byte[] destinationBuffer, int sourceBufferWidth, int destinationBufferWidth, Microsoft.Xna.Framework.Rectangle area)
		{
			fixed (byte* ptr3 = &destinationBuffer[0])
			{
				fixed (byte* ptr = &sourceBuffer[0])
				{
					byte* ptr2 = ptr;
					byte* ptr4 = ptr3 + (destinationBufferWidth * area.Y + area.X << 2);
					for (int i = 0; i < area.Height; i++)
					{
						for (int j = 0; j < area.Width; j++)
						{
							ptr4[2] = *ptr2;
							ptr4[1] = ptr2[1];
							*ptr4 = ptr2[2];
							ptr4[3] = ptr2[3];
							ptr2 += 4;
							ptr4 += 4;
						}
						ptr2 += sourceBufferWidth - area.Width << 2;
						ptr4 += destinationBufferWidth - area.Width << 2;
					}
				}
			}
		}

		public float GetProgress()
		{
			return _tilesProcessed / _totalTiles;
		}

		private bool SaveImage(int width, int height, ImageFormat imageFormat, string filename)
		{
			try
			{
				Directory.CreateDirectory(Main.SavePath + Path.DirectorySeparatorChar + "Captures" + Path.DirectorySeparatorChar);
				using (Bitmap bitmap = new Bitmap(width, height))
				{
					System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
					BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
					IntPtr scan = bitmapData.Scan0;
					Marshal.Copy(_outputData, 0, scan, width * height * 4);
					bitmap.UnlockBits(bitmapData);
					bitmap.Save(filename, imageFormat);
					bitmap.Dispose();
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void SaveImage(Texture2D texture, int width, int height, ImageFormat imageFormat, string foldername, string filename)
		{
			Directory.CreateDirectory(Main.SavePath + Path.DirectorySeparatorChar + "Captures" + Path.DirectorySeparatorChar + foldername);
			using (Bitmap bitmap = new Bitmap(width, height))
			{
				System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
				int elementCount = texture.Width * texture.Height * 4;
				texture.GetData(_outputData, 0, elementCount);
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < height; i++)
				{
					for (int j = 0; j < width; j++)
					{
						byte b = _outputData[num + 2];
						_outputData[num2 + 2] = _outputData[num];
						_outputData[num2] = b;
						_outputData[num2 + 1] = _outputData[num + 1];
						_outputData[num2 + 3] = _outputData[num + 3];
						num += 4;
						num2 += 4;
					}
					num += texture.Width - width << 2;
				}
				BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
				IntPtr scan = bitmapData.Scan0;
				Marshal.Copy(_outputData, 0, scan, width * height * 4);
				bitmap.UnlockBits(bitmapData);
				bitmap.Save(Main.SavePath + Path.DirectorySeparatorChar + "Captures" + Path.DirectorySeparatorChar + foldername + Path.DirectorySeparatorChar + filename, imageFormat);
			}
		}

		private void FinishCapture()
		{
			if (_activeSettings.UseScaling)
			{
				int num = 0;
				while (!SaveImage(_outputImageSize.Width, _outputImageSize.Height, ImageFormat.Png, Main.SavePath + Path.DirectorySeparatorChar + "Captures" + Path.DirectorySeparatorChar + _activeSettings.OutputName + ".png"))
				{
					GC.Collect();
					Thread.Sleep(5);
					num++;
					Console.WriteLine("An error occured while saving the capture. Attempting again...");
					if (num > 5)
					{
						Console.WriteLine("Unable to capture.");
						break;
					}
				}
			}
			_outputData = null;
			_scaledFrameData = null;
			Main.GlobalTimerPaused = false;
			CaptureInterface.EndCamera();
			if (_scaledFrameBuffer != null)
			{
				_scaledFrameBuffer.Dispose();
				_scaledFrameBuffer = null;
			}
			_activeSettings = null;
		}

		public void Dispose()
		{
		}
	}
}
