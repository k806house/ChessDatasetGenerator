using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.DTO;
using static Assets.Scripts.Core.Constants;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class StateManager : Singleton<StateManager>
    {

        private static readonly ILogger Logger = Debug.unityLogger;
        private static readonly Queue<string[,]> Fens = new Queue<string[,]>();
        private static int currentNumberOfScreenShoots;
        private static int currentStateIndex;

        private void Start()
        {
            try
            {
                var fenStrings = File.ReadAllText("Fen.data")
                    .Split('\n')
                    .Select(str => str.Trim())
                    .Where(str => !string.IsNullOrEmpty(str));

                foreach (var fen in fenStrings)
                {
                    Fens.Enqueue(GetGameMatrixFromFem(fen));
                }
            }
            catch (Exception e)
            {
                Logger.Log(KTag, $"Error read Fen.data: {e}");
            }
        }

        public string[,] GetCurrentFenMatrix()
        {
            if (Fens.Count == 0) return null;

            if (currentStateIndex == 0)
            {
                currentNumberOfScreenShoots = SettingsManager.Instance.GetNumberOfScreenshotsPerFen();
            }

            if (currentNumberOfScreenShoots != 0)
            {
                currentNumberOfScreenShoots--;
                return Fens.Peek();
            }

            Fens.Dequeue();
            currentStateIndex++;
            currentNumberOfScreenShoots = SettingsManager.Instance.GetNumberOfScreenshotsPerFen();
            GetCurrentFenMatrix();
            return null;
        }

        public int GetCurrentStateIndex() => currentStateIndex;

        private static string[,] GetGameMatrixFromFem(string fenString)
        {
            const int boardSize = 8;

            var board = new string[boardSize, boardSize];
            var j = 0;
            var i = 7;

            foreach (var c in fenString)
            {
                if (c == '/')
                {
                    i--;
                    j = 0;
                }
                else if (char.IsNumber(c))
                {
                    var inx = int.Parse(char.ToString(c));
                    while (inx > 0)
                    {
                        board[i, j] = string.Empty;
                        j++;
                        inx--;
                    }
                }
                else
                {
                    //TODO: enum mapping
                    switch (c)
                    {
                        case 'K':
                            board[i, j] = "KingWhite";
                            break;
                        case 'Q':
                            board[i, j] = "QueenWhite";
                            break;
                        case 'R':
                            board[i, j] = "RookWhite";
                            break;
                        case 'B':
                            board[i, j] = "BishopWhite";
                            break;
                        case 'N':
                            board[i, j] = "KnightWhite";
                            break;
                        case 'P':
                            board[i, j] = "PawnWhite";
                            break;
                        case 'k':
                            board[i, j] = "KingBlack";
                            break;
                        case 'q':
                            board[i, j] = "QueenBlack";
                            break;
                        case 'r':
                            board[i, j] = "RookBlack";
                            break;
                        case 'b':
                            board[i, j] = "BishopBlack";
                            break;
                        case 'n':
                            board[i, j] = "KnightBlack";
                            break;
                        case 'p':
                            board[i, j] = "PawnBlack";
                            break;
                        default:
                            board[i, j] = board[i, j];
                            break;
                    }
                    j++;
                }
            }
            return board;
        }
    }
}
