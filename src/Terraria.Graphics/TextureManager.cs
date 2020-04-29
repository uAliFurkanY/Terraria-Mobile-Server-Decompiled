using Microsoft.Xna.Framework.Graphics;
using System.Collections.Concurrent;
using System.Threading;

namespace Terraria.Graphics
{
	internal static class TextureManager
	{
		private struct LoadPair
		{
			public string Path;

			public Ref<Texture2D> TextureRef;

			public LoadPair(string path, Ref<Texture2D> textureRef)
			{
				Path = path;
				TextureRef = textureRef;
			}
		}

		private static ConcurrentDictionary<string, Texture2D> _textures = new ConcurrentDictionary<string, Texture2D>();

		private static ConcurrentQueue<LoadPair> _loadQueue = new ConcurrentQueue<LoadPair>();

		private static Thread _loadThread;

		private static readonly object _loadThreadLock = new object();

		public static Texture2D BlankTexture;

		public static void Initialize()
		{
			BlankTexture = new Texture2D(Main.graphics.GraphicsDevice, 4, 4);
		}

		public static Texture2D Load(string name)
		{
			if (!_textures.ContainsKey(name))
			{
				Texture2D blankTexture = BlankTexture;
				_textures[name] = blankTexture;
				return blankTexture;
			}
			return _textures[name];
		}

		public static Ref<Texture2D> Retrieve(string name)
		{
			Texture2D value = Load(name);
			return new Ref<Texture2D>(value);
		}

		public static void Run(object context)
		{
			bool looping = true;
			Main.instance.Exiting += delegate
			{
				looping = false;
				if (Monitor.TryEnter(_loadThreadLock))
				{
					Monitor.Pulse(_loadThreadLock);
					Monitor.Exit(_loadThreadLock);
				}
			};
			Monitor.Enter(_loadThreadLock);
			while (looping)
			{
				if (_loadQueue.Count != 0)
				{
					LoadPair result;
					if (_loadQueue.TryDequeue(out result))
					{
						result.TextureRef.Value = Load(result.Path);
					}
				}
				else
				{
					Monitor.Wait(_loadThreadLock);
				}
			}
			Monitor.Exit(_loadThreadLock);
		}
	}
}
