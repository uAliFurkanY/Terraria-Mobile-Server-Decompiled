using System;
using System.Collections.Generic;

namespace Terraria.ID
{
	public class SetFactory
	{
		protected int _size;

		private Queue<int[]> _intBufferCache = new Queue<int[]>();

		private Queue<bool[]> _boolBufferCache = new Queue<bool[]>();

		private object _queueLock = new object();

		public SetFactory(int size)
		{
			_size = size;
		}

		protected bool[] GetBoolBuffer()
		{
			lock (_queueLock)
			{
				if (_boolBufferCache.Count == 0)
				{
					return new bool[_size];
				}
				return _boolBufferCache.Dequeue();
			}
		}

		protected int[] GetIntBuffer()
		{
			lock (_queueLock)
			{
				if (_boolBufferCache.Count == 0)
				{
					return new int[_size];
				}
				return _intBufferCache.Dequeue();
			}
		}

		public void Recycle<T>(T[] buffer)
		{
			lock (_queueLock)
			{
				if (typeof(T).Equals(typeof(bool)))
				{
					_boolBufferCache.Enqueue((bool[])(object)buffer);
				}
				else if (typeof(T).Equals(typeof(int)))
				{
					_intBufferCache.Enqueue((int[])(object)buffer);
				}
			}
		}

		public bool[] CreateBoolSet(params int[] types)
		{
			return CreateBoolSet(false, types);
		}

		public bool[] CreateBoolSet(bool defaultState, params int[] types)
		{
			bool[] boolBuffer = GetBoolBuffer();
			for (int i = 0; i < boolBuffer.Length; i++)
			{
				boolBuffer[i] = defaultState;
			}
			for (int j = 0; j < types.Length; j++)
			{
				boolBuffer[types[j]] = !defaultState;
			}
			return boolBuffer;
		}

		public int[] CreateIntSet(int defaultState, params int[] inputs)
		{
			if (inputs.Length % 2 != 0)
			{
				throw new Exception("You have a bad length for inputs on CreateArraySet");
			}
			int[] ıntBuffer = GetIntBuffer();
			for (int i = 0; i < ıntBuffer.Length; i++)
			{
				ıntBuffer[i] = defaultState;
			}
			for (int j = 0; j < inputs.Length; j += 2)
			{
				ıntBuffer[inputs[j]] = inputs[j + 1];
			}
			return ıntBuffer;
		}

		public T[] CreateCustomSet<T>(T defaultState, params object[] inputs)
		{
			if (inputs.Length % 2 != 0)
			{
				throw new Exception("You have a bad length for inputs on CreateCustomSet");
			}
			T[] array = new T[_size];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = defaultState;
			}
			for (int j = 0; j < inputs.Length; j += 2)
			{
				array[(short)inputs[j]] = (T)inputs[j + 1];
			}
			return array;
		}
	}
}
