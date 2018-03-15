using System;
namespace mines
{
	public class Game
	{
		int turn;
		Board board;
		const int Width = 6; //TODO options for the game
		const int Height = 6;
		const int NMines = 3;

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
			string state = string.Format("Turn {0}. {1} out of {2} flags placed.\n\n", turn, board.CountFlags(), NMines);
			state += board.GetGrid();

			if (!running) {
				if (board.hitMine) state += "\nYOU LOSE\n";
				else if (board.AllSeen()) state += "\nYOU WIN!\n";
				else state += "It's over somehow!\n";
			}

			return state;
		}

		public void Parse(string line)
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
			if (command == 'p' && board.SeenAt(col, row) >= 0) command = 'x';

			switch(command) {
				case 'p': board.PeekAt(col, row); turn++; break;
				case 'x': board.ExpandPeekFrom(col, row); turn++; break;
				case 'f': board.SetFlagAt(col, row, !board.HasFlagAt(col, row)); break;
				default: return;
			}

			if (board.hitMine || board.AllSeen()) {
				running = false;
			}
		}
	}
}
