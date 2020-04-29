namespace Terraria.World.Generation
{
	public class GenerationProgress
	{
		private string _message = "";

		private float _value;

		private float _totalProgress;

		public float TotalWeight;

		public float CurrentPassWeight = 1f;

		public string Message
		{
			get
			{
				return string.Format(_message, Value);
			}
			set
			{
				_message = value.Replace("%", "{0:0.0%}");
			}
		}

		public float Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = Utils.Clamp(value, 0f, 1f);
			}
		}

		public float TotalProgress
		{
			get
			{
				if (TotalWeight == 0f)
				{
					return 0f;
				}
				return (Value * CurrentPassWeight + _totalProgress) / TotalWeight;
			}
		}

		public void Set(float value)
		{
			Value = value;
		}

		public void Start(float weight)
		{
			CurrentPassWeight = weight;
			_value = 0f;
		}

		public void End()
		{
			_totalProgress += CurrentPassWeight;
		}
	}
}
