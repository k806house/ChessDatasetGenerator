using System;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public class MainProcess : MonoBehaviour
{
    public Camera MainCamera;

    public int Counter;

    private const int ResWidth = 640;
    private const int ResHeight = 360;

    public const string BoardObjectName = "Board";
    public const string WhiteChessObjectName = "WhiteChess";
    public const string BlackChessObjectName = "BlackChess";

    public GameObject ChessBoard;
    public List<GameObject> ChessFiguresWhite;
    public List<GameObject> ChessFiguresBlack;

    public float BoardSide;
    // public GameObject[ , ] BoardMatrix = new GameObject[BoardSide, BoardSide];
    public Vector3 InitPos = new Vector3(0.0f, 0.0f, 0.0f);

    void InitChessBoard(Object Resource)
    {
        Quaternion Rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        Vector3 Position = new Vector3(0.0f, 0.0f, 0.0f);

        ChessBoard = (GameObject) Instantiate(Resource, Position, Rotation);
        
        ChessBoard.AddComponent<RectTransform>();
        RectTransform BoardRt = (RectTransform)ChessBoard.transform;
        BoardSide = BoardRt.rect.width / 10.0f / 2.0f;

        // для наглядности располагаю по углам игрового поля кубики
        Vector3 BoardPosition = ChessBoard.transform.position;
        GameObject Cube;
        Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = new Vector3(BoardPosition.x + BoardSide, 0.0f, BoardPosition.z + BoardSide);

        Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = new Vector3(BoardPosition.x - BoardSide, 0.0f, BoardPosition.z - BoardSide);
    
        Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = new Vector3(BoardPosition.x + BoardSide, 0.0f, BoardPosition.z - BoardSide);

        Cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Cube.transform.position = new Vector3(BoardPosition.x - BoardSide, 0.0f, BoardPosition.z + BoardSide);
    }

    List<GameObject> InitChessFigures(Object Resource)
    {
        Quaternion Rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        Vector3 Position = new Vector3(0.0f, 0.0f, 0.0f);

        GameObject ChessFiguresResource = (GameObject) Instantiate( Resource, Position, Rotation);

        Transform[] ChessFiguresTs = ChessFiguresResource.GetComponentsInChildren<Transform>();

        List<GameObject> ChessFigures = new List<GameObject>();
        foreach (Transform child in ChessFiguresTs) {
            ChessFigures.Add(child.gameObject);
        }

        return ChessFigures;
    }


    void Start()
    {
        MainCamera.CopyFrom(Camera.main);

        InitChessBoard(Resources.Load(BoardObjectName));

        ChessFiguresWhite = InitChessFigures(Resources.Load(WhiteChessObjectName));

        foreach(GameObject figure in ChessFiguresWhite) {
            figure.transform.position = new Vector3(
                figure.transform.position.x,
                figure.transform.position.z + 1.0f,
                figure.transform.position.y + Random.Range(0, 1.0f));
        }




        // for (int i = 0; i < BoardSide; i++) {
        //     for (int j = 0; j < BoardSide; j++) {
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
        Debug.Log("...");
        // Thread.Sleep(500);
        if (Counter >= 1)
            Application.Quit();
        //рандомная трансформация для красочности
        // var currentAngle = Cube.transform.localPosition;
        // Cube.transform.Rotate(currentAngle + 10 * Vector3.one, Space.Self);
    }

    void OnPostRender()
    {
        Debug.Log($"Отрендерилось!{Counter++}");
        var file = TakeScreenshot(MainCamera);

        Vector3 BoardCenterPosition = ChessBoard.transform.position;
        List<Vector3> CornerPositions = new List<Vector3>();
        var x = BoardCenterPosition.x;
        var y = BoardCenterPosition.y;
        var z = BoardCenterPosition.z;
        float coefWidth = Convert.ToSingle(ResWidth / MainCamera.pixelWidth);
        float coefHeight = Convert.ToSingle(ResHeight / MainCamera.pixelHeight);
        CornerPositions.Add(MainCamera.WorldToScreenPoint(new Vector3(x + BoardSide, y, z + BoardSide)));
        CornerPositions.Add(MainCamera.WorldToScreenPoint(new Vector3(x - BoardSide, y, z - BoardSide)));
        CornerPositions.Add(MainCamera.WorldToScreenPoint(new Vector3(x + BoardSide, y, z - BoardSide)));
        CornerPositions.Add(MainCamera.WorldToScreenPoint(new Vector3(x - BoardSide, y, z + BoardSide)));

        foreach(Vector3 screenPos in CornerPositions) {
            Debug.Log("target is " + coefWidth + " " + coefHeight);
            Debug.Log("target is " + screenPos.x + " " + screenPos.y);
        }
        // For testing purposes, also write to a file in the project folder
        // пока просто сохраняется в папку с проектом
        File.WriteAllBytes(Application.dataPath + $"/../SavedScreen{Counter}.png", file);
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
}
