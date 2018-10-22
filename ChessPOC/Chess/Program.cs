using System;
using System.Collections;
using System.Text;

namespace Chess
{
    class Program
    {
        #region Enumerations

        public enum Player
        {
            Player1,
            Player2
        }

        public enum Figure
        {
            None = 0,   // <empty space>
            Pawn = 1,   // P
            Rook = 2,   // R
            Knight = 3, // N (K is already reserved for King!!!)
            Bishop = 4, // B
            Queen = 5,  // Q
            King = 6,   // K
        }

        public enum CastlingType
        {
            King, 
            Queen
        }

        #endregion

        #region Structures

        public struct CellAddress
        {
            public char X;
            public byte Y;
        }

        public struct Command
        {
            public char Action;

            public CellAddress From;

            public CellAddress To;

            public CastlingType CastlingType;
        }

        public struct Cell
        {
            public Player Player;
            public Figure Figure;
        }

        #endregion

        #region Program Entry Point (Main Method)

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            // declare shared variables
            Cell[,] board = InitBoard();

            Player currentPlayer = Player.Player1;
            bool isPlayer1Castling = false, isPlayer2Castling = false;

            while(true)
            {
                DrawBoard(board);
                WriteGeneralHelp();

                Command command;
                if (!TryGetCommand(currentPlayer, out command))
                    continue;

                switch (command.Action)
                {
                    case 'm':
                        if (!TryMove(currentPlayer, board, command.From, command.To))
                        {
                            // if the move was not valid 
                            // continue with the next iteration 
                            // with the same player
                            continue;
                        }
                            
                        break;
                    case 'u':
                        Undo();
                        break;
                    case 'c':
                       if (currentPlayer == Player.Player1)
                            isPlayer1Castling = Castling(currentPlayer, command.CastlingType, board);
                       else 
                            isPlayer2Castling = Castling(currentPlayer, command.CastlingType, board);
                       break;
                    case 'q':
                        Quit();
                        break;
                    default:
                        Console.WriteLine("Unexpected action!");
                        break;
                }

                if (IsStalemate() || IsCheckmate())
                {
                    Console.WriteLine($"{currentPlayer} won!");
                    break;
                }

                currentPlayer = (currentPlayer == Player.Player1) ? Player.Player2 : Player.Player1;
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        #endregion

        #region Game Over Conditions

        public static bool IsCheckmate()
        {
            // TODO: implement later
            return false;
        }

        public static bool IsStalemate()
        {
            // TODO: implement later
            return false;
        }

        #endregion

        #region Parse and Validate User Commands (translate plain text input to structure) 

        public static bool TryGetCommand(Player currentPlayer, out Command command)
        {
            Console.Write($"{currentPlayer}:");

            string input = Console.ReadLine().Trim().ToLower();

            // check if any input is provided
            if (string.IsNullOrWhiteSpace(input))
            {
                WriteError("Unrecognized command!");
                WriteGeneralHelp();

                command = default(Command);
                return false;
            }

            // get the first letter of the command
            char cmd = Convert.ToChar(input.Substring(0, 1));

            if (cmd == 'q' && input.Length == 1)
            {
                command = new Command() { Action = cmd };
                return true;
            }

            if (cmd == 'u' && input.Length == 1)
            {
                command = new Command() { Action = cmd };
                return true;
            }

            if (cmd == 'm' && input.Length > 1)
            {
                // we expect the move command with 2 arguments (ex. m C7 C6)
                // 1. we get everything after the 'm' (substring starting from index 1)
                // 2. we will split by space
                string[] parameters = input.Substring(1).Trim().Split(' ');

                // 3. we expect this to return exactly 2 strings (ex. A1 and A4)
                // 4. we expect each of those 2 strings to be exactly 2 characters  w
                if (parameters.Length != 2 ||
                    parameters[0].Length != 2 ||
                    parameters[1].Length != 2)
                {
                    WriteError("Unrecognized command!");
                    WriteError("Expects M XY XY (ex. M A2 A3)");

                    command = default(Command);
                    return false;
                }

                // 5. we expect those 2 characters (e.g. address) to be letter and digit
                if (!(char.IsLetter(parameters[0][0]) || char.IsDigit(parameters[0][1]))
                    || !(char.IsLetter(parameters[1][0]) || char.IsDigit(parameters[1][1])))
                {
                    WriteError("Unrecognized command!");
                    WriteError("Expects M XY XY (ex. M A2 A3)");

                    command = default(Command);
                    return false;
                }

                // 6. we expect the letter to be one of A, B, C, D, E, F, G, H
                if (!(parameters[0][0] >= 'a' && parameters[0][0] <= 'h') ||
                    !(parameters[1][0] >= 'a' && parameters[1][0] <= 'h'))
                {
                    WriteError("Unrecognized command!");
                    WriteError("Expects M XY XY (ex. M A2 A3)");

                    command = default(Command);
                    return false;
                }

                // 7. we expect the digit to be 1, 2, 3, 4, 5, 6, 7, 8
                if (!(parameters[0][1] >= '1' && parameters[0][1] <= '8') ||
                    !(parameters[1][1] >= '1' && parameters[1][1] <= '8'))
                {
                    WriteError("Unrecognized command!");
                    WriteError("Expects M XY XY (ex. M A2 A3)");

                    command = default(Command);
                    return false;
                }

                CellAddress from = new CellAddress() { X = parameters[0][0], Y = byte.Parse(parameters[0][1].ToString()) };
                CellAddress to = new CellAddress() { X = parameters[1][0], Y = byte.Parse(parameters[1][1].ToString()) };

                command = new Command() { Action = cmd, From = from, To = to };
                return true;
            }

            // ex. `c k` or `c q`
            if (cmd == 'c')
            {
                string[] parts = input.Split(' ');

                // check if there only two letters/words divided by space
                if (parts.Length != 2)
                {
                    WriteError("Unrecognized command!");
                    WriteError("Expects [C]astling [K]ing or [C]astling [Q]ueen");

                    command = default(Command);
                    return false;
                }

                // check that we have two parts that are 1 letter long
                // and the second parts is either q or k
                if (parts[0].Length == 1 && parts[1].Length == 1
                    && (parts[1][0] == 'q' || parts[1][0] == 'k'))
                {
                    if (parts[1][0] == 'q')
                    {
                        command = new Command() { Action = cmd, CastlingType = CastlingType.Queen };
                        return true;
                    }
                    else if (parts[1][0] == 'k')
                    {
                        command = new Command() { Action = cmd, CastlingType = CastlingType.King };
                        return true;
                    }
                }

                WriteError("Unrecognized command!");
                WriteError("Expects [C]astling [K]ing or [C]astling [Q]ueen");

                command = default(Command);
                return false;
            }

            // if we reach this point, we don't know how to handle 
            // the command 
            WriteError("Unrecognized command!");
            WriteGeneralHelp();

            command = default(Command);
            return false;
        }

        #endregion

        #region Commands (Move, Castling, Undo, Quit)

        public static bool TryMove(Player currentPlayer, Cell[,] board,
            CellAddress from, CellAddress to)
        {
            // substracting characters will return a number, because each 
            // character is represented as number / position in Unicode table
            Cell fromCell = board[board.GetLength(0) - from.Y, char.ToLower(from.X) - 'a'];
            Cell toCell = board[board.GetLength(0) - to.Y, char.ToLower(to.X) - 'a'];

            // check if the cell from which we are trying 
            // to move a figure contains any figure             
            if (fromCell.Figure == Figure.None)
            {
                WriteError("Invalid move! No figure at the given position.");
                return false;
            }

            // check if the cell from which we are trying
            // to move contains a figure of the current player
            if (fromCell.Player != currentPlayer)
            {
                WriteError("Invalid move! The figure at the given position is not yours.");
                return false;
            }

            // based on the figure and player (different only for pawns!) move a figure
            // note: no need to use break or default, as we are directly return-ing from the method
            switch (fromCell.Figure)
            {
                case Figure.Pawn:
                    return MovePawn(currentPlayer, board, from, to);
                case Figure.Rook:
                    return MoveRook(currentPlayer, board, from, to);
                case Figure.Bishop:
                    return MoveBishop(currentPlayer, board, from, to);
                case Figure.Knight:
                    return MoveKnight(currentPlayer, board, from, to);
                case Figure.King:
                    return MoveKing(currentPlayer, board, from, to);
                case Figure.Queen:
                    return MoveQueen(currentPlayer, board, from, to);
            }

            return false;
        }

        public static bool Castling(Player currentPlayer, CastlingType castlingType, Cell[,] board)
        {
            // TODO: implement later
            return true;
        }

        public static void Undo()
        {
            // TODO: implement later
        }

        public static void Quit()
        {
            // closes the application with Error Code 0,
            // which means Success (if not 0, that indicates a problem)
            Environment.Exit(0);
        }

        #endregion

        #region Move Operations (6 different methods for each figure type)

        public static bool MoveQueen(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the Queen is not implemented.");

            return false;
        }

        public static bool MoveKing(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the King is not implemented.");

            return false;
        }

        public static bool MoveKnight(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the Knight is not implemented.");

            return false;
        }

        public static bool MoveBishop(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the Bishop is not implemented.");

            return false;
        }

        public static bool MoveRook(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the Rook is not implemented.");

            return false;
        }

        public static bool MovePawn(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the Pawn is not implemented.");

            return false;
        }

        #endregion

        #region Board Drawing

        public static void DrawBoard(Cell[,] board)
        {
            // represents a map / relation between the enumeration 
            // and a one letter presentation when we print it on screen
            Hashtable figureMap = new Hashtable()
            {
                { Figure.None,      ' ' },
                { Figure.Pawn,      'P' },
                { Figure.Rook,      'R' },
                { Figure.Knight,    'N' }, // K is already reserved for King!!!
                { Figure.Bishop,    'B' },
                { Figure.Queen,     'Q' },
                { Figure.King,      'K' }
            };

            Console.WriteLine("    A   B   C   D   E   F   G   H");
            for (int i = 0; i < board.GetLength(0); i++)
            {
                // each row has top border
                Console.WriteLine("  #################################");

                // reverse row indexes to match real world chess :)
                Console.Write($"{board.GetLength(0) - i} ");

                for (int j = 0; j < board.GetLength(1); j++)
                {
                    Console.Write("#");

                    // TODO: change background color as well
                    var currentForeground = Console.ForegroundColor;

                    Console.ForegroundColor = board[i, j].Player == 
                        Player.Player1 ? ConsoleColor.Red : ConsoleColor.Blue;

                    Console.Write(" {0} ", figureMap[board[i,j].Figure]);

                    Console.ForegroundColor = currentForeground;
                }

                Console.WriteLine($"# {board.GetLength(0) - i}");
            }

            // footer (bottom line)
            Console.WriteLine("  #################################");
            Console.WriteLine("    A   B   C   D   E   F   G   H");
        }

        #endregion

        #region Initialize Board

        public static Cell[,] InitBoard()
        {
            var initialBoard = new Cell[8, 8];

            int pawnsRowPlayer1 = 1, pawnsRowPlayer2 = 6;

            InitPlayerFigures(initialBoard, Player.Player1, pawnsRowPlayer1);
            InitPlayerFigures(initialBoard, Player.Player2, pawnsRowPlayer2);
            
            return initialBoard;
        }

        public static void InitPlayerFigures(Cell[,] board, Player player, int pawnsRow)
        {
            var backRow = (player == Player.Player1) ? pawnsRow - 1 : pawnsRow + 1;

            board[backRow, 0] = new Cell
            {
                Player = player,
                Figure = Figure.Rook
            };
            board[backRow, 1] = new Cell
            {
                Player = player,
                Figure = Figure.Knight
            };
            board[backRow, 2] = new Cell
            {
                Player = player,
                Figure = Figure.Bishop
            };
            board[backRow, 3] = new Cell
            {
                Player = player,
                Figure = (player == Player.Player1) ? Figure.Queen : Figure.King
            };
            board[backRow, 4] = new Cell
            {
                Player = player,
                Figure = (player != Player.Player1) ? Figure.Queen : Figure.King
            };
            board[backRow, 5] = new Cell
            {
                Player = player,
                Figure = Figure.Bishop
            };
            board[backRow, 6] = new Cell
            {
                Player = player,
                Figure = Figure.Knight
            };
            board[backRow, 7] = new Cell
            {
                Player = player,
                Figure = Figure.Rook
            };

            // add player 2 pawns
            for (int row = pawnsRow, col = 0; col < board.GetLength(0); col++)
            {
                board[row, col] = new Cell
                {
                    Player = player,
                    Figure = Figure.Pawn
                };
            }
        }

        #endregion

        #region Helper Methods for Console Output

        public static void WriteGeneralHelp()
        {
            WriteError("Expects [M]ove, [U]ndo, [C]astling [K]ing, [C]astling [Q]ueen or [Q]uit");
        }

        public static void WriteError(string msg)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = originalColor;
        }

        #endregion 
    }
}
