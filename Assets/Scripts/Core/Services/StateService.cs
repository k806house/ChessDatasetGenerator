using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Core.Constants;

namespace Assets.Scripts.Core.Services
{
    public class StateService : MonoBehaviour
    {

        private static readonly ILogger Logger = Debug.unityLogger;
        private static readonly Queue<string[,]> Fens = new Queue<string[,]>();

        private static int currentNumberOfScreenShoots;
        private static int currentStateIndex;
        private static bool isFirstState = true;

        private SettingsService settingsService;

        public void Init(SettingsService service)
        {
            settingsService = service;
        }

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

        public string[,] GetNewFenMatrix()
        {
            if (Fens.Count == 0) return null;

            if (isFirstState)
            {
                currentNumberOfScreenShoots = settingsService.GetNumberOfScreenshotsPerFen();
                isFirstState = false;
            }

            if (currentNumberOfScreenShoots != 0)
            {
                currentNumberOfScreenShoots--;
                return Fens.Peek();
            }

            Fens.Dequeue();
            currentStateIndex++;
            currentNumberOfScreenShoots = settingsService.GetNumberOfScreenshotsPerFen();
            GetNewFenMatrix();
            return null;
        }

        public bool IsNewStateAvailable() => Fens.Count != 0;
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
