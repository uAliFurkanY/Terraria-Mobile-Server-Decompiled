using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Terraria.Utilities
{
	public class MultiTimer
	{
		private struct TimerData
		{
			public readonly double Min;

			public readonly double Max;

			public readonly double Ticks;

			public readonly double Total;

			public double Average => Total / Ticks;

			private TimerData(double min, double max, double ticks, double total)
			{
				Min = min;
				Max = max;
				Ticks = ticks;
				Total = total;
			}

			public TimerData(double startTime)
			{
				Min = startTime;
				Max = startTime;
				Ticks = 1.0;
				Total = startTime;
			}

			public TimerData AddTick(double time)
			{
				return new TimerData(Math.Min(Min, time), Math.Max(Max, time), Ticks + 1.0, Total + time);
			}
		}

		private int _ticksBetweenPrint = 100;

		private int _ticksElapsedForPrint;

		private Stopwatch _timer = new Stopwatch();

		private Dictionary<string, TimerData> _timerDataMap = new Dictionary<string, TimerData>();

		public MultiTimer()
		{
		}

		public MultiTimer(int ticksBetweenPrint)
		{
			_ticksBetweenPrint = ticksBetweenPrint;
		}

		public void Start()
		{
			_timer.Reset();
			_timer.Start();
		}

		public void Record(string key)
		{
			double totalMilliseconds = _timer.Elapsed.TotalMilliseconds;
			TimerData value;
			if (!_timerDataMap.TryGetValue(key, out value))
			{
				_timerDataMap.Add(key, new TimerData(totalMilliseconds));
			}
			else
			{
				_timerDataMap[key] = value.AddTick(totalMilliseconds);
			}
			_timer.Restart();
		}

		public bool StopAndPrint()
		{
			_ticksElapsedForPrint++;
			if (_ticksElapsedForPrint == _ticksBetweenPrint)
			{
				_ticksElapsedForPrint = 0;
				Console.WriteLine("Average elapsed time: ");
				double num = 0.0;
				int num2 = 0;
				foreach (KeyValuePair<string, TimerData> item in _timerDataMap)
				{
					num2 = Math.Max(item.Key.Length, num2);
				}
				foreach (KeyValuePair<string, TimerData> item2 in _timerDataMap)
				{
					TimerData value = item2.Value;
					string text = new string(' ', num2 - item2.Key.Length);
					Console.WriteLine(item2.Key + text + " : (Average: " + value.Average.ToString("F4") + " Min: " + value.Min.ToString("F4") + " Max: " + value.Max.ToString("F4") + " from " + (int)value.Ticks + " records)");
					num += value.Total;
				}
				_timerDataMap.Clear();
				Console.WriteLine("Total : " + (float)num / (float)_ticksBetweenPrint + "ms");
				return true;
			}
			return false;
		}
	}
}
