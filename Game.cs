using System;
namespace mines
{
	public class Game
	{
		int turn;
		Board board;
		const int Width = 5; //TODO options for the game
		const int Height = 5;
		const int NMines = 6;

		bool running = true;

		public Game()
		{
			board = new Board(Width, Height, NMines);
			turn = 0;

		}

		public bool IsRunning()
		{
			return running;
		}

		public string GetState() {
			string state = string.Format("Turn {0}. {1} out of {2} flags placed.\n", turn, board.GetFlagsPlaced(), NMines);
			return state + board.GetGrid();
		}

		internal void Parse(string line)
		{
			if (string.IsNullOrEmpty(line)) return; //TODO show help

			line = line.Trim().ToLower();

			if (line.StartsWith("show")) {
				Console.WriteLine("SHOWING ALL");
				board.ShowAll();
				return;
			}

			if (string.IsNullOrEmpty(line)) return; //TODO show help

			var words = line.Split(' ');
			var coords = words.Length == 1 ? words[0] : words[1];

			var colLetter = coords[0];
			var rowString = coords.Substring(1);

			var col = colLetter - 'a';
			var row = 0;
			if (!int.TryParse(rowString, out row)) return; // TODO input error
			row--;

			if (row < 0 || row > Height-1) return; // TODO input error
			if (col < 0 || col > Width-1) return; // TODO input error

			var verb = words[0];
			var command = verb[0];

			if (words.Length == 1) command = 'p';

			switch(command) {
				case 'p': board.PeekAt(col, row); break;
				case 'f': board.SetFlagAt(col, row, !board.HasFlagAt(col, row)); break;
				default: return;
			}
		}
	}
}
