using System;
using System.Diagnostics;
using System.Device.Gpio;

namespace StepperMotorDriver
{
	class StepperDriver
	{
		public int[] GpioList { get; set; }
		public int StepsPerRevolution { get; set; }
		public int[] Phase = new int[] { 1, 1, 0, 0 };
		public GpioController controller;

		//public double WaitTime => 0.002;
		public double WaitTime => 0.0019;

		public StepperDriver(int[] gpioList, int stepsPerRev = 2048)
		{
			controller = new GpioController();

			Console.CancelKeyPress += (s, e) =>
			{
				Dispose();
			};
			AppDomain.CurrentDomain.ProcessExit += (s, e) => { Dispose(); };

			GpioList = gpioList;
			StepsPerRevolution = stepsPerRev;

			//INIT GPIO
			foreach(int pin in GpioList)
			{
				controller.OpenPin(pin, PinMode.Output);
				controller.Write(pin, PinValue.Low);
			}
		}

		public void RotateStep(bool Clockwise)
		{
			if (Clockwise) RollRight();
			else RollLeft();

			for(int i = 0; i < GpioList.Length; i++)
			{
				controller.Write(GpioList[i], Phase[i] == 1 ? PinValue.High : PinValue.Low);
			}
		}

		public void Rotate(int degrees, bool Clockwise = true)
		{
			int steps = StepsPerRevolution * degrees / 360;
			for(int i = 0; i < steps; i++)
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				RotateStep(Clockwise);
				while (stopwatch.Elapsed.TotalSeconds < WaitTime) { }
			}

			foreach (int pin in GpioList)
			{
				controller.Write(pin, PinValue.Low);
			}
		}

		public void Dispose()
		{
			controller.Dispose();
		}

		void RollRight()
		{
			var temp = Phase[Phase.Length - 1];
			Array.Copy(Phase, 0, Phase, 1, Phase.Length - 1);
			Phase[0] = temp;
		}

		void RollLeft()
		{
			var temp = Phase[0];
			Array.Copy(Phase, 1, Phase, 0, Phase.Length - 1);
			Phase[Phase.Length - 1] = temp;
		}
	}
}