using System;
using System.Linq;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MainProcess : MonoBehaviour
{
    public Camera MainCamera;

    public int Counter;

    private const int ResWidth = 1600;
    private const int ResHeight = 1000;

    private const int BoardSize = 8;
    public float BoardSideWidth;

    public const string BoardObjectName = "Board";
    public const string WhiteChessObjectName = "WhiteChess";
    public const string BlackChessObjectName = "BlackChess";

    public GameObject ChessBoard;
    // Hashtable ChessFigures = new Hashtable();
    Dictionary <string, GameObject> ChessFigures = new Dictionary<string, GameObject>();

    public List<string[,]> GameStateMatrixLst = new List<string[,]> ();
    public Vector3 InitPos = new Vector3(0.0f, 0.0f, 0.0f);


    void InitChessBoard(UnityEngine.Object Resource)
    {
        Quaternion Rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
        Vector3 Position = new Vector3(0.0f, 0.0f, 0.0f);

        ChessBoard = (GameObject) Instantiate(Resource, Position, Rotation);
        
        ChessBoard.AddComponent<RectTransform>();
        RectTransform BoardRt = (RectTransform)ChessBoard.transform;
        BoardSideWidth = BoardRt.rect.width / 10.0f;

        // для наглядности располагаю по углам игрового поля кубики
        Vector3 BoardPosition = ChessBoard.transform.position;
        GameObject Cube;
        Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = new Vector3(BoardPosition.x + BoardSideWidth / 2.0f, 0.0f, BoardPosition.z + BoardSideWidth / 2.0f);

        Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = new Vector3(BoardPosition.x - BoardSideWidth / 2.0f, 0.0f, BoardPosition.z - BoardSideWidth / 2.0f);
    
        Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = new Vector3(BoardPosition.x + BoardSideWidth / 2.0f, 0.0f, BoardPosition.z - BoardSideWidth / 2.0f);

        Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = new Vector3(BoardPosition.x - BoardSideWidth / 2.0f, 0.0f, BoardPosition.z + BoardSideWidth / 2.0f);
    }

    void InitChessFigures(UnityEngine.Object Resource)
    {
        Quaternion Rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        Vector3 Position = new Vector3(0.0f, 0.0f, 0.0f);

        GameObject ChessFiguresResource = (GameObject) Instantiate(Resource, Position, Rotation);

        Transform[] ChessFiguresTs = ChessFiguresResource.GetComponentsInChildren<Transform>();
        Debug.Log("+++++");
        foreach (Transform child in ChessFiguresTs) {
            // child.gameObject.active = false;

            if (child.gameObject.name != ChessFiguresResource.name) {
                Debug.Log(child.gameObject.name);
                ChessFigures.Add(child.gameObject.name, child.gameObject);
            }
        }
        Debug.Log("+++++");
    }

    void InitChessFigure(UnityEngine.Object parentObj, int i, int j)
    {
        Quaternion rotation = Quaternion.Euler(-90.0f, 90.0f, 0.0f);
        float coef = 1.05f;
        Vector3 position = new Vector3(
            InitPos.x + i * coef - (BoardSize * coef / 2 - 0.5f),
            0.0f,
            InitPos.z + j * coef - (BoardSize * coef / 2 - 0.5f)
        );

        GameObject gameFigure = (GameObject) Instantiate(parentObj, position, rotation);
        gameFigure.transform.localScale *= 27;
        // gameFigure.active = true;        
    }

    void Start()
    {
        GameStateMatrixLst.Add(GetGameMatrixFromFem("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR"));
        string[,] CurGameState = GameStateMatrixLst.First();

        InitChessBoard(Resources.Load(BoardObjectName));
        InitChessFigures(Resources.Load(WhiteChessObjectName));
        InitChessFigures(Resources.Load(BlackChessObjectName));

        for (int i = 0; i < BoardSize; i++) {
            for (int j = 0; j < BoardSize; j++) {
                Debug.Log(""+CurGameState[i, j]);
                if (ChessFigures.ContainsKey(CurGameState[i, j])) {
                    InitChessFigure(ChessFigures[CurGameState[i, j]], i, j);
                }
                //Debug.Log(CurGameState[i, j]);


                // if ((i + j) % 2 == 0) {
                //     GameObject Figure = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //     Figure.transform.position = new Vector3((float)i, 0.0f, (float)j);
                //     BoardMatrix[i, j] = Figure;
                // }
            }
        }

        // foreach(GameObject figure in ChessFiguresWhite) {
        //     figure.transform.position = new Vector3(
        //         figure.transform.position.x,
        //         figure.transform.position.z + 1.0f,
        //         figure.transform.position.y + Random.Range(0, 1.0f));
        // }




        // for (int i = 0; i < BoardSideWidth; i++) {
        //     for (int j = 0; j < BoardSideWidth; j++) {
        //         Random random = new Random();
        //         if ((i + j) % 2 == 0) {
        //             GameObject Figure = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //             Figure.transform.position = new Vector3((float)i, 0.0f, (float)j);
        //             BoardMatrix[i, j] = Figure;
        //         }
        //     }
        // }

        // Cube.transform.position = new Vector3(-0.75f, 0.0f, 0.0f);
        // Cube.transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("...");
        // Thread.Sleep(500);
        //рандомная трансформация для красочности
        // var currentAngle = Cube.transform.localPosition;
        // Cube.transform.Rotate(currentAngle + 10 * Vector3.one, Space.Self);
    }

    void OnPostRender()
    {
        // Debug.Log($"Отрендерилось!{Counter++}");
        // var file = TakeScreenshot(Camera);
        // For testing purposes, also write to a file in the project folder
        // пока просто сохраняется в папку с проектом
        // File.WriteAllBytes(Application.dataPath + $"/../SavedScreen{Counter}.png", file);
    }

    //Стырено из статьи https://towardsdatascience.com/transcribe-live-chess-with-machine-learning-part-1-928f73306e1f
    public static byte[] TakeScreenshot(Camera camera)
    {
        RenderTexture rt = new RenderTexture(ResWidth, ResHeight, 24);
        camera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(ResWidth, ResHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, ResWidth, ResHeight), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        return screenShot.EncodeToPNG();
    }

    string[,] GetGameMatrixFromFem(string s)
    {
        int boardSize = 8;
        string[,] board = new string[boardSize, boardSize];

        int j = 0;
        int i = 7;
        foreach (char c in s)
        {
            if(c == '/') {
                i--;
                j = 0;
            } else if(char.IsNumber(c)) {
                int inx = Int32.Parse(char.ToString(c));
                while (inx > 0)
                {
                    board[i, j] = String.Empty;
                    j++;
                    inx--;
                }
            } else {
                switch(c)
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
                }
                j++;
            }
        }

        return board;
    }
}

