using Pastel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RubikCubeSolver
{
	public enum RColor
	{
		White,
		Yellow,
		Green,
		Blue,
		Red,
		Orange
	}

	public enum RTransform
	{
		Up,
		Left,
		Front,
		Right,
		Back,
		Down
	}

	public enum RMove
	{
		R,
		U,
		L,
		D,
		F,
		B,
		Ri,
		Ui,
		Li,
		Di,
		Fi,
		Bi
	}

	class Program
	{
		public static Dictionary<RMove, RMove> RmoveDuplicates = new Dictionary<RMove, RMove>()
		{
			[RMove.R] = RMove.Ri,
			[RMove.U] = RMove.Ui,
			[RMove.L] = RMove.Li,
			[RMove.D] = RMove.Di,
			[RMove.F] = RMove.Fi,
			[RMove.B] = RMove.Bi,
			[RMove.Ri] = RMove.R,
			[RMove.Ui] = RMove.U,
			[RMove.Li] = RMove.L,
			[RMove.Di] = RMove.D,
			[RMove.Fi] = RMove.F,
			[RMove.Bi] = RMove.B
		};

		public static Dictionary<RTransform, List<RColor>> RubiksCube = new Dictionary<RTransform, List<RColor>>();

		static void Main(string[] args)
		{
			//INITALIZE DEAFULT COLORS
			//KOCIEMBA COLORS
			/*RubiksCube[RTransform.Up] = new List<RColor>() { RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow };
			RubiksCube[RTransform.Down] = new List<RColor>() { RColor.White, RColor.White, RColor.White, RColor.White, RColor.White, RColor.White, RColor.White, RColor.White, RColor.White };
			RubiksCube[RTransform.Left] = new List<RColor>() { RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue };
			RubiksCube[RTransform.Front] = new List<RColor>() { RColor.Red, RColor.Red, RColor.Red, RColor.Red, RColor.Red, RColor.Red, RColor.Red, RColor.Red, RColor.Red };
			RubiksCube[RTransform.Right] = new List<RColor>() { RColor.Green, RColor.Green, RColor.Green, RColor.Green, RColor.Green, RColor.Green, RColor.Green, RColor.Green, RColor.Green };
			RubiksCube[RTransform.Back] = new List<RColor>() { RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange };*/

			//DEFAULT COLORS
			RubiksCube[RTransform.Up] = new List<RColor>() { RColor.White, RColor.White, RColor.White, RColor.White, RColor.White, RColor.White, RColor.White, RColor.White, RColor.White };
			RubiksCube[RTransform.Down] = new List<RColor>() { RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow, RColor.Yellow };
			RubiksCube[RTransform.Left] = new List<RColor>() { RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange, RColor.Orange };
			RubiksCube[RTransform.Back] = new List<RColor>() { RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue, RColor.Blue };
			RubiksCube[RTransform.Right] = new List<RColor>() { RColor.Red, RColor.Red, RColor.Red, RColor.Red, RColor.Red, RColor.Red, RColor.Red, RColor.Red, RColor.Red };
			RubiksCube[RTransform.Front] = new List<RColor>() { RColor.Green, RColor.Green, RColor.Green, RColor.Green, RColor.Green, RColor.Green, RColor.Green, RColor.Green, RColor.Green };
			
			Console.WriteLine("SCRAMBLING...");

			DoMoves(GenerateScramble(1000));
			PrintCurrentRubiksState();
			Console.WriteLine(CubeToString(RubiksCube, true));

			Console.WriteLine("GENERATING SOLUTION...");

			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo = new System.Diagnostics.ProcessStartInfo()
			{
				UseShellExecute = false,
				CreateNoWindow = false,
				WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
				FileName = "python3",
				Arguments = @$"kociemba.py {CubeToString(RubiksCube, true)}",
				RedirectStandardError = true,
				RedirectStandardOutput = true
			};
			process.Start();
			string output = process.StandardOutput.ReadToEnd().Replace(Environment.NewLine, "");
			process.WaitForExit();

			Console.WriteLine("SOLUTION: " + output);

			DoMoves(output);

			PrintCurrentRubiksState();
		}

		static string GenerateScramble(int length)
		{
			Random r = new Random();
			int max = Enum.GetValues(typeof(RMove)).Cast<int>().Max();
			List<RMove> rmoves = new List<RMove>();
			RMove lastmove = RMove.Ri;

			for(int i = 0; i < length; i++)
			{
				RMove rm = (RMove)r.Next(0, max + 1);
				if(RmoveDuplicates[rm] != lastmove)
				{
					rmoves.Add(rm);
					lastmove = rm;
				}
			}

			return string.Join(',', rmoves.ToArray());
		}

		static void DoMoves(string moveString)
		{
			string[] alg = moveString.Replace("\'", "i").Split(',', ' ');

			for(int i = 0; i < alg.Length; i++)
			{
				char[] algp = alg[i].ToCharArray();
				if (algp.Length > 1)
				{
					if (int.TryParse(algp[algp.Length-1].ToString(), out int mc))
					{
						for(int f = 0; f < mc; f++)
						{
							RMove mv = (RMove)Enum.Parse(typeof(RMove), alg[i].Replace(mc.ToString(), ""));
							DoMove(mv);
						}
					}
					else
					{
						RMove mv = (RMove)Enum.Parse(typeof(RMove), alg[i]);
						DoMove(mv);
					}
				}
				else
				{
					RMove mv = (RMove)Enum.Parse(typeof(RMove), alg[i]);
					DoMove(mv);
				}
			}
		}

		static void PrintCurrentRubiksState()
		{
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Up][0])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Up][1])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Up][2]));
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			Console.Write(Environment.NewLine);

			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Up][3])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Up][4])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Up][5]));
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			Console.Write(Environment.NewLine);

			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Up][6])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Up][7])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Up][8]));
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			Console.Write(Environment.NewLine);


			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Left][0])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Left][1])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Left][2]));
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Front][0])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Front][1])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Front][2]));
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Right][0])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Right][1])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Right][2]));
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Back][0])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Back][1])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Back][2]));
			Console.Write(Environment.NewLine);

			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Left][3])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Left][4])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Left][5]));
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Front][3])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Front][4])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Front][5]));
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Right][3])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Right][4])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Right][5]));
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Back][3])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Back][4])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Back][5]));
			Console.Write(Environment.NewLine);

			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Left][6])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Left][7])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Left][8]));
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Front][6])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Front][7])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Front][8]));
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Right][6])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Right][7])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Right][8]));
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Back][6])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Back][7])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Back][8]));
			Console.Write(Environment.NewLine);


			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Down][0])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Down][1])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Down][2]));
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			Console.Write(Environment.NewLine);

			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Down][3])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Down][4])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Down][5]));
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			Console.Write(Environment.NewLine);

			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Down][6])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Down][7])); PrintColorChar("#", GetConsoleColor(RubiksCube[RTransform.Down][8]));
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White); PrintColorChar(" ", Color.White);
			Console.Write(Environment.NewLine);
		}

		static void DoMove(RMove move)
		{
			Dictionary<RTransform, RColor[]> tmp = TransformDict(RubiksCube);

			switch (move)
			{
				case RMove.R:
					RubiksCube[RTransform.Back][0] = tmp[RTransform.Up][8];
					RubiksCube[RTransform.Back][3] = tmp[RTransform.Up][5];
					RubiksCube[RTransform.Back][6] = tmp[RTransform.Up][2];

					RubiksCube[RTransform.Down][2] = tmp[RTransform.Back][6];
					RubiksCube[RTransform.Down][5] = tmp[RTransform.Back][3];
					RubiksCube[RTransform.Down][8] = tmp[RTransform.Back][0];

					RubiksCube[RTransform.Front][2] = tmp[RTransform.Down][2];
					RubiksCube[RTransform.Front][5] = tmp[RTransform.Down][5];
					RubiksCube[RTransform.Front][8] = tmp[RTransform.Down][8];

					RubiksCube[RTransform.Up][2] = tmp[RTransform.Front][2];
					RubiksCube[RTransform.Up][5] = tmp[RTransform.Front][5];
					RubiksCube[RTransform.Up][8] = tmp[RTransform.Front][8];
					RotateFace(RTransform.Right);
					break;

				case RMove.U:
					RubiksCube[RTransform.Left][0] = tmp[RTransform.Front][0];
					RubiksCube[RTransform.Left][1] = tmp[RTransform.Front][1];
					RubiksCube[RTransform.Left][2] = tmp[RTransform.Front][2];

					RubiksCube[RTransform.Back][0] = tmp[RTransform.Left][0];
					RubiksCube[RTransform.Back][1] = tmp[RTransform.Left][1];
					RubiksCube[RTransform.Back][2] = tmp[RTransform.Left][2];

					RubiksCube[RTransform.Right][0] = tmp[RTransform.Back][0];
					RubiksCube[RTransform.Right][1] = tmp[RTransform.Back][1];
					RubiksCube[RTransform.Right][2] = tmp[RTransform.Back][2];

					RubiksCube[RTransform.Front][0] = tmp[RTransform.Right][0];
					RubiksCube[RTransform.Front][1] = tmp[RTransform.Right][1];
					RubiksCube[RTransform.Front][2] = tmp[RTransform.Right][2];
					RotateFace(RTransform.Up);
					break;

				case RMove.F:
					RubiksCube[RTransform.Left][2] = tmp[RTransform.Down][0];
					RubiksCube[RTransform.Left][5] = tmp[RTransform.Down][1];
					RubiksCube[RTransform.Left][8] = tmp[RTransform.Down][2];

					RubiksCube[RTransform.Down][0] = tmp[RTransform.Right][6];
					RubiksCube[RTransform.Down][1] = tmp[RTransform.Right][3];
					RubiksCube[RTransform.Down][2] = tmp[RTransform.Right][0];

					RubiksCube[RTransform.Right][0] = tmp[RTransform.Up][6];
					RubiksCube[RTransform.Right][3] = tmp[RTransform.Up][7];
					RubiksCube[RTransform.Right][6] = tmp[RTransform.Up][8];

					RubiksCube[RTransform.Up][6] = tmp[RTransform.Left][8];
					RubiksCube[RTransform.Up][7] = tmp[RTransform.Left][5];
					RubiksCube[RTransform.Up][8] = tmp[RTransform.Left][2];
					RotateFace(RTransform.Front);
					break;

				case RMove.L:
					RubiksCube[RTransform.Up][0] = tmp[RTransform.Back][8];
					RubiksCube[RTransform.Up][3] = tmp[RTransform.Back][5];
					RubiksCube[RTransform.Up][6] = tmp[RTransform.Back][2];

					RubiksCube[RTransform.Back][8] = tmp[RTransform.Down][0];
					RubiksCube[RTransform.Back][5] = tmp[RTransform.Down][3];
					RubiksCube[RTransform.Back][2] = tmp[RTransform.Down][6];

					RubiksCube[RTransform.Down][0] = tmp[RTransform.Front][0];
					RubiksCube[RTransform.Down][3] = tmp[RTransform.Front][3];
					RubiksCube[RTransform.Down][6] = tmp[RTransform.Front][6];

					RubiksCube[RTransform.Front][0] = tmp[RTransform.Up][0];
					RubiksCube[RTransform.Front][3] = tmp[RTransform.Up][3];
					RubiksCube[RTransform.Front][6] = tmp[RTransform.Up][6];
					RotateFace(RTransform.Left);
					break;

				case RMove.D:
					RubiksCube[RTransform.Front][6] = tmp[RTransform.Left][6];
					RubiksCube[RTransform.Front][7] = tmp[RTransform.Left][7];
					RubiksCube[RTransform.Front][8] = tmp[RTransform.Left][8];

					RubiksCube[RTransform.Left][6] = tmp[RTransform.Back][6];
					RubiksCube[RTransform.Left][7] = tmp[RTransform.Back][7];
					RubiksCube[RTransform.Left][8] = tmp[RTransform.Back][8];

					RubiksCube[RTransform.Back][6] = tmp[RTransform.Right][6];
					RubiksCube[RTransform.Back][7] = tmp[RTransform.Right][7];
					RubiksCube[RTransform.Back][8] = tmp[RTransform.Right][8];

					RubiksCube[RTransform.Right][6] = tmp[RTransform.Front][6];
					RubiksCube[RTransform.Right][7] = tmp[RTransform.Front][7];
					RubiksCube[RTransform.Right][8] = tmp[RTransform.Front][8];
					RotateFace(RTransform.Down);
					break;

				case RMove.B:
					RubiksCube[RTransform.Left][0] = tmp[RTransform.Up][2];
					RubiksCube[RTransform.Left][3] = tmp[RTransform.Up][1];
					RubiksCube[RTransform.Left][6] = tmp[RTransform.Up][0];

					RubiksCube[RTransform.Up][0] = tmp[RTransform.Right][2];
					RubiksCube[RTransform.Up][1] = tmp[RTransform.Right][5];
					RubiksCube[RTransform.Up][2] = tmp[RTransform.Right][8];

					RubiksCube[RTransform.Right][2] = tmp[RTransform.Down][8];
					RubiksCube[RTransform.Right][5] = tmp[RTransform.Down][7];
					RubiksCube[RTransform.Right][8] = tmp[RTransform.Down][6];

					RubiksCube[RTransform.Down][8] = tmp[RTransform.Left][6];
					RubiksCube[RTransform.Down][7] = tmp[RTransform.Left][3];
					RubiksCube[RTransform.Down][6] = tmp[RTransform.Left][0];
					RotateFace(RTransform.Back);
					break;



				case RMove.Ri:
					RubiksCube[RTransform.Back][0] = tmp[RTransform.Down][8];
					RubiksCube[RTransform.Back][3] = tmp[RTransform.Down][5];
					RubiksCube[RTransform.Back][6] = tmp[RTransform.Down][2];

					RubiksCube[RTransform.Down][2] = tmp[RTransform.Front][2];
					RubiksCube[RTransform.Down][5] = tmp[RTransform.Front][5];
					RubiksCube[RTransform.Down][8] = tmp[RTransform.Front][8];

					RubiksCube[RTransform.Front][2] = tmp[RTransform.Up][2];
					RubiksCube[RTransform.Front][5] = tmp[RTransform.Up][5];
					RubiksCube[RTransform.Front][8] = tmp[RTransform.Up][8];

					RubiksCube[RTransform.Up][2] = tmp[RTransform.Back][6];
					RubiksCube[RTransform.Up][5] = tmp[RTransform.Back][3];
					RubiksCube[RTransform.Up][8] = tmp[RTransform.Back][0];
					RotateFace(RTransform.Right, true);
					break;

				case RMove.Ui:
					RubiksCube[RTransform.Left][0] = tmp[RTransform.Back][0];
					RubiksCube[RTransform.Left][1] = tmp[RTransform.Back][1];
					RubiksCube[RTransform.Left][2] = tmp[RTransform.Back][2];

					RubiksCube[RTransform.Back][0] = tmp[RTransform.Right][0];
					RubiksCube[RTransform.Back][1] = tmp[RTransform.Right][1];
					RubiksCube[RTransform.Back][2] = tmp[RTransform.Right][2];

					RubiksCube[RTransform.Right][0] = tmp[RTransform.Front][0];
					RubiksCube[RTransform.Right][1] = tmp[RTransform.Front][1];
					RubiksCube[RTransform.Right][2] = tmp[RTransform.Front][2];

					RubiksCube[RTransform.Front][0] = tmp[RTransform.Left][0];
					RubiksCube[RTransform.Front][1] = tmp[RTransform.Left][1];
					RubiksCube[RTransform.Front][2] = tmp[RTransform.Left][2];
					RotateFace(RTransform.Up, true);
					break;

				case RMove.Fi:
					RubiksCube[RTransform.Left][2] = tmp[RTransform.Up][8];
					RubiksCube[RTransform.Left][5] = tmp[RTransform.Up][7];
					RubiksCube[RTransform.Left][8] = tmp[RTransform.Up][6];

					RubiksCube[RTransform.Up][6] = tmp[RTransform.Right][0];
					RubiksCube[RTransform.Up][7] = tmp[RTransform.Right][3];
					RubiksCube[RTransform.Up][8] = tmp[RTransform.Right][6];

					RubiksCube[RTransform.Right][0] = tmp[RTransform.Down][2];
					RubiksCube[RTransform.Right][3] = tmp[RTransform.Down][1];
					RubiksCube[RTransform.Right][6] = tmp[RTransform.Down][0];

					RubiksCube[RTransform.Down][0] = tmp[RTransform.Left][2];
					RubiksCube[RTransform.Down][1] = tmp[RTransform.Left][5];
					RubiksCube[RTransform.Down][2] = tmp[RTransform.Left][8];
					RotateFace(RTransform.Front, true);
					break;

				case RMove.Li:
					RubiksCube[RTransform.Up][0] = tmp[RTransform.Front][0];
					RubiksCube[RTransform.Up][3] = tmp[RTransform.Front][3];
					RubiksCube[RTransform.Up][6] = tmp[RTransform.Front][6];

					RubiksCube[RTransform.Front][0] = tmp[RTransform.Down][0];
					RubiksCube[RTransform.Front][3] = tmp[RTransform.Down][3];
					RubiksCube[RTransform.Front][6] = tmp[RTransform.Down][6];

					RubiksCube[RTransform.Down][0] = tmp[RTransform.Back][8];
					RubiksCube[RTransform.Down][3] = tmp[RTransform.Back][5];
					RubiksCube[RTransform.Down][6] = tmp[RTransform.Back][2];

					RubiksCube[RTransform.Back][8] = tmp[RTransform.Up][0];
					RubiksCube[RTransform.Back][5] = tmp[RTransform.Up][3];
					RubiksCube[RTransform.Back][2] = tmp[RTransform.Up][6];
					RotateFace(RTransform.Left, true);
					break;

				case RMove.Di:
					RubiksCube[RTransform.Front][6] = tmp[RTransform.Right][6];
					RubiksCube[RTransform.Front][7] = tmp[RTransform.Right][7];
					RubiksCube[RTransform.Front][8] = tmp[RTransform.Right][8];

					RubiksCube[RTransform.Right][6] = tmp[RTransform.Back][6];
					RubiksCube[RTransform.Right][7] = tmp[RTransform.Back][7];
					RubiksCube[RTransform.Right][8] = tmp[RTransform.Back][8];

					RubiksCube[RTransform.Back][6] = tmp[RTransform.Left][6];
					RubiksCube[RTransform.Back][7] = tmp[RTransform.Left][7];
					RubiksCube[RTransform.Back][8] = tmp[RTransform.Left][8];

					RubiksCube[RTransform.Left][6] = tmp[RTransform.Front][6];
					RubiksCube[RTransform.Left][7] = tmp[RTransform.Front][7];
					RubiksCube[RTransform.Left][8] = tmp[RTransform.Front][8];
					RotateFace(RTransform.Down, true);
					break;

				case RMove.Bi:
					RubiksCube[RTransform.Left][0] = tmp[RTransform.Down][6];
					RubiksCube[RTransform.Left][3] = tmp[RTransform.Down][7];
					RubiksCube[RTransform.Left][6] = tmp[RTransform.Down][8];

					RubiksCube[RTransform.Down][6] = tmp[RTransform.Right][8];
					RubiksCube[RTransform.Down][7] = tmp[RTransform.Right][5];
					RubiksCube[RTransform.Down][8] = tmp[RTransform.Right][2];

					RubiksCube[RTransform.Right][8] = tmp[RTransform.Up][2];
					RubiksCube[RTransform.Right][5] = tmp[RTransform.Up][1];
					RubiksCube[RTransform.Right][2] = tmp[RTransform.Up][0];

					RubiksCube[RTransform.Up][0] = tmp[RTransform.Left][6];
					RubiksCube[RTransform.Up][1] = tmp[RTransform.Left][3];
					RubiksCube[RTransform.Up][2] = tmp[RTransform.Left][0];
					RotateFace(RTransform.Back, true);
					break;
			}
		}

		static string CubeToString(Dictionary<RTransform, List<RColor>> cube, bool kociambaColors = false)
		{
			string tmp = "";

			foreach(RTransform t in Enum.GetValues(typeof(RTransform)))
			{
				for(int i = 0; i < 9; i++)
				{
					tmp += ColorToChar(cube[t][i]);
				}
			}

			if (kociambaColors)
			{
				string ktmp = "";
				for(int i = 0; i < tmp.Length; i++)
				{
					ktmp += GetKociambaChar(tmp[i]);
				}

				return ktmp;
			}

			return tmp;
		}

		static string ColorToChar(RColor col)
		{
			switch (col)
			{
				case RColor.White:
					return "W";
				case RColor.Yellow:
					return "Y";
				case RColor.Green:
					return "G";
				case RColor.Blue:
					return "B";
				case RColor.Red:
					return "R";
				case RColor.Orange:
					return "O";
			}

			return "X";
		}

		static char GetKociambaChar(char c)
		{
			switch (c)
			{
				case 'W':
					return 'Y';
				case 'Y':
					return 'W';
				case 'O':
					return 'B';
				case 'G':
					return 'R';
				case 'R':
					return 'G';
				case 'B':
					return 'O';
			}

			return ' ';
		}

		static void RotateFace(RTransform face, bool invert = false)
		{
			Dictionary<RTransform, RColor[]> tmp = TransformDict(RubiksCube);

			if (invert)
			{
				RubiksCube[face][0] = tmp[face][2];
				RubiksCube[face][1] = tmp[face][5];
				RubiksCube[face][2] = tmp[face][8];
				RubiksCube[face][3] = tmp[face][1];
				RubiksCube[face][5] = tmp[face][7];
				RubiksCube[face][6] = tmp[face][0];
				RubiksCube[face][7] = tmp[face][3];
				RubiksCube[face][8] = tmp[face][6];
			}
			else
			{
				RubiksCube[face][0] = tmp[face][6];
				RubiksCube[face][1] = tmp[face][3];
				RubiksCube[face][2] = tmp[face][0];
				RubiksCube[face][3] = tmp[face][7];
				RubiksCube[face][5] = tmp[face][1];
				RubiksCube[face][6] = tmp[face][8];
				RubiksCube[face][7] = tmp[face][5];
				RubiksCube[face][8] = tmp[face][2];
			}
		}

		static Dictionary<RTransform, RColor[]> TransformDict(Dictionary<RTransform, List<RColor>> rCube)
		{
			Dictionary<RTransform, RColor[]> tmp = new Dictionary<RTransform, RColor[]>();
			foreach (KeyValuePair<RTransform, List<RColor>> kvp in rCube)
			{
				tmp[kvp.Key] = kvp.Value.ToArray();
			}

			return tmp;
		}

		static void PrintColorChar(string character, Color color)
		{
			Console.Write(character.Pastel(color));
		}

		static Color GetConsoleColor(RColor col)
		{
			switch (col)
			{
				case RColor.White:
					return Color.White;
				case RColor.Yellow:
					return Color.Yellow;
				case RColor.Green:
					return Color.Lime;
				case RColor.Blue:
					return Color.Blue;
				case RColor.Red:
					return Color.Red;
				case RColor.Orange:
					return Color.Orange;
			}

			return Color.Black;
		}
	}
}
