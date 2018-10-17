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

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;

            Cell[,] board = InitBoard();
            Player currentPlayer = Player.Player1;
            bool isPlayer1Castling = false, isPlayer2Castling = false;

          
            while(true)
            {
                DrawBoard(board);
       
                Command command;
                if (!NextCommand(currentPlayer, out command))
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

        private static bool Castling(Player currentPlayer, CastlingType castlingType, Cell[,] board)
        {
            // TODO: implement later
            return true;
        }

        private static bool IsCheckmate()
        {
            // TODO: implement later
            return false;
        }

        private static bool IsStalemate()
        {
            // TODO: implement later
            return false;
        }

        private static void Quit()
        {
            // closes the application with Error Code 0,
            // which means Success (if not 0, that indicates a problem)
            Environment.Exit(0);
        }

        private static void Undo()
        {
            // TODO: implement later
        }

        private static bool TryMove(Player currentPlayer, Cell[,] board, 
            CellAddress from, CellAddress to)
        {
            // substracting characters will return a number, because each 
            // character is represented as number / position in Unicode table
            Cell fromCell = board[from.Y, char.ToLower(from.X) - 'a'];
            Cell toCell = board[to.Y, char.ToLower(to.X) - 'a'];

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

        private static bool MoveQueen(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the Queen is not implemented.");

            return false;
        }

        private static bool MoveKing(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the King is not implemented.");

            return false;
        }

        private static bool MoveKnight(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the Knight is not implemented.");

            return false;
        }

        private static bool MoveBishop(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the Bishop is not implemented.");

            return false;
        }

        private static bool MoveRook(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the Rook is not implemented.");

            return false;
        }

        private static bool MovePawn(Player currentPlayer, Cell[,] board, CellAddress from, CellAddress to)
        {
            WriteError("Moving the Pawn is not implemented.");

            return false;
        }

        private static bool NextCommand(Player currentPlayer, out Command command)
        {
            Console.Write($"{currentPlayer}:");

            string input = Console.ReadLine().Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                WriteError("Unrecognized command!");
                WriteError("Expects [M]ove, [U]ndo, [C]astling [K]ing, [C]astling [Q]ueen or [Q]uit");

                command = default(Command);
                return false;
            }

            var cmd = Convert.ToChar(input.Substring(0, 1));

            if (cmd == 'q' && input.Length == 1)
            {
                command = new Command() { Action = 'q' };
                return true;
            }

            if (cmd == 'u' && input.Length == 1)
            {
                command = new Command() { Action = 'u' };
                return true;
            }

            if (cmd == 'm' /* TODO: more condition for homework :) */)
            {
                // TODO: parsing (splitting the string) to be implemented for homework :)
                CellAddress from = new CellAddress() { X = 'D', Y = 1 };
                CellAddress to = new CellAddress() { X = 'D', Y = 2 };

                command = new Command() { Action = cmd, From = from, To = to };
                return true;
            }

            if (cmd == 'c')
            {
                string[] parts = input.Split(' ');

                if (parts.Length != 2)
                {
                    WriteError("Unrecognized command!");
                    WriteError("Expects [C]astling [K]ing or [C]astling [Q]ueen");
                    command = default(Command);
                    return false;
                }

                if (parts[1].Length == 1 && (parts[1][0] == 'q' || parts[1][0] == 'k'))
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

            WriteError("Unrecognized command!");
            WriteError("Expects [M]ove, [U]ndo, [C]astling [K]ing, [C]astling [Q]ueen or [Q]uit");

            command = default(Command);
            return false;
        }

        private static void WriteError(string msg)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = originalColor;
        }

        private static void DrawBoard(Cell[,] board)
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

                Console.Write($"{board.GetLength(1) - i} ");
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    Console.Write("# {0} ", figureMap[board[i,j].Figure]);
                }

                Console.WriteLine($"# {board.GetLength(1) - i}");
            }

            // footer (bottom line)
            Console.WriteLine("  #################################");
            Console.WriteLine("    A   B   C   D   E   F   G   H");
        }

        private static Cell[,] InitBoard()
        {
            var initialBoard = new Cell[8, 8];

            // add player 1 figures
            initialBoard[0, 0] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Rook
            };
            initialBoard[0, 1] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Knight
            };
            initialBoard[0, 2] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Bishop
            };
            initialBoard[0, 3] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.King
            };
            initialBoard[0, 4] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Queen
            };
            initialBoard[0, 5] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Bishop
            };
            initialBoard[0, 6] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Knight
            };
            initialBoard[0, 7] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Rook
            };

            // add player 1 pawns
            for (int row = 1, col = 0; col < initialBoard.GetLength(0); col++)
            {
                initialBoard[row, col] = new Cell
                {
                    Player = Player.Player1,
                    Figure = Figure.Pawn
                };
            }

            // add player 2 figures
            initialBoard[7, 0] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Rook
            };
            initialBoard[7, 1] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Knight
            };
            initialBoard[7, 2] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Bishop
            };
            initialBoard[7, 3] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Queen
            };
            initialBoard[7, 4] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.King
            };
            initialBoard[7, 5] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Bishop
            };
            initialBoard[7, 6] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Knight
            };
            initialBoard[7, 7] = new Cell
            {
                Player = Player.Player1,
                Figure = Figure.Rook
            };

            // add player 2 pawns
            for (int row = 6, col = 0; col < initialBoard.GetLength(0); col++)
            {
                initialBoard[row, col] = new Cell
                {
                    Player = Player.Player2,
                    Figure = Figure.Pawn
                };
            }

            return initialBoard;
        }
    }
}
