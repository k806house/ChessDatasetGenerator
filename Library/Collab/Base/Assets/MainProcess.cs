using Assets;
using Assets.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class MainProcess : MonoBehaviour
{
    private Camera mainCamera;
    private Camera cameraForScreenshot;
    
    public GameObject gameTable;
    private Vector3 initPos;

    private static readonly ILogger Logger = Debug.unityLogger;
    private static string kTag = "ChessTag";
    private SphericalCoordinates sphericalCoordinates;
    private int ResWidth;
    private int ResHeight;
    private const int BoardSize = 8;
    // все пропорции в мире шахмат считаются от ширины доски
    private float BoardWidth; // требует создания доски
    private float BoardHeight;

    private const float BoardWidthCm = 43.0f;
    private const float BoardOffsetCm = 2.0f;
    private const float BoardSquareCm = (BoardWidthCm - 2 * BoardOffsetCm) / BoardSize;
    
    private float figureScaleFactor;
    private const float FigureHeightCm = 9.0f;
    // сетка для шахматной доски
    private Vector3 gridOffset;
    private float gridStep;
    // ..
    private const float WorldScaleFactor = 1.0f;
    // ..
    private RangeValues rangeValues;
    private const string BoardObjectName = "Board";
    private const string WhiteChessObjectName = "WhiteChess";
    private const string BlackChessObjectName = "BlackChess";
    private GameObject chessBoard;
    private readonly Dictionary<string, GameObject> chessFigures = new Dictionary<string, GameObject>();
    private readonly List<GameObject> currentChessFigures = new List<GameObject>();
    private int gameStateIndex;
    private readonly List<Pair<string[,], int>> gameStateMatrixLst = new List<Pair<string[,], int>>();
    private readonly List<GameObject> lightGameObjects = new List<GameObject>();
    private GameObject light;

    private float GetCmFromDst(float dst) => dst * BoardWidthCm / BoardWidth;
    private float GetDstFromCm(float cmDst) => cmDst * BoardWidth / BoardWidthCm * WorldScaleFactor;
    private Vector3 VectorFromAngle(float theta) => new Vector3(Mathf.Cos(theta), 0.0f, Mathf.Sin(theta));

    static (Vector3 bounds, Vector3 center) GetObjectRendererParams(GameObject obj)
    {
        var rendr = obj.GetComponent<Renderer>();
        if (rendr == null) {
            rendr = obj.AddComponent<Renderer>();
        }
        return (rendr.bounds.extents, rendr.bounds.center);
    }

    void initGameTable()
    {
        var renderer = GetObjectRendererParams(gameTable);
        initPos = renderer.center + new Vector3(0.0f, renderer.bounds.y, 0.0f);
    }

    private void InitChessBoard(Object resource)
    {
        var rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        var position = initPos;

        chessBoard = (GameObject) Instantiate(resource, position, rotation);

        var renderer = GetObjectRendererParams(chessBoard);

        chessBoard.transform.position += new Vector3(0.0f, renderer.bounds.y, 0.0f);

        float proportion = 0.6f;
        float tableWidth = GetObjectRendererParams(gameTable).bounds.x;
        float scaleFactor = proportion * tableWidth / renderer.bounds.x;
        chessBoard.transform.localScale *= scaleFactor;

        renderer = GetObjectRendererParams(chessBoard);

        BoardWidth = renderer.bounds.x * 2;
        gridStep = GetDstFromCm(BoardSquareCm); // !!! мина !!!

        var gridCenterOffset = (gridStep * BoardSize - gridStep) / 2.0f;
        gridOffset = new Vector3(-gridCenterOffset, renderer.bounds.y, -gridCenterOffset);
    }

    private void InitChessFigures(Object resource)
    {
        var rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        var position = chessBoard.transform.position;

        var chessFiguresResource = (GameObject)Instantiate(resource, position, rotation);

        var chessFiguresTs = chessFiguresResource.GetComponentsInChildren<Transform>();
        var maxFigureHeight = 0.0f;

        foreach (var child in chessFiguresTs)
        {
            child.gameObject.SetActive(false);

            if (child.gameObject.name != chessFiguresResource.name)
            {
                var curFigureHeight = GetObjectRendererParams(child.gameObject).bounds.y * 2;
                if (curFigureHeight > maxFigureHeight)
                {
                    maxFigureHeight = curFigureHeight;
                }

                chessFigures.Add(child.gameObject.name, child.gameObject);
            }
        }
        figureScaleFactor = GetDstFromCm(FigureHeightCm) / maxFigureHeight;
    }

    private void RemoveChessFigures()
    {
        foreach (var figure in currentChessFigures)
        {
            Destroy(figure);
        }
        currentChessFigures.Clear();
    }

    private void InitChessFigure(Object parentObj, int i, int j, float offset = 0.0f)
    {
        // случайное вращение фигуры вокруг своей оси
        var rotation = Quaternion.Euler(-90.0f, Random.Range(0.0f, 360.0f), 0.0f);
        // позиция фигуры по сетке
        var position = chessBoard.transform.position + gridOffset + new Vector3(i * gridStep, 0.0f, j * gridStep);
        // сдвиг фигуры в случайную сторону на заданное расстояние
        // ..
        var direction = VectorFromAngle(Random.Range(0.0f, 360.0f));
        var offsetVector = direction * GetDstFromCm(offset);
        // ...
        var gameFigure = (GameObject)Instantiate(parentObj, position + offsetVector, rotation);

        gameFigure.transform.localScale *= figureScaleFactor;

        gameFigure.SetActive(true);
        currentChessFigures.Add(gameFigure);
    }

    private void InitLights(float height,
                            List<(float x, float y)> coordinates,
                            float intensitySpotLight,
                            float intensityAmbientLight,
                            float ambientLightPhi)
    {
        var rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        if (lightGameObjects.Count != 0)
        {
            foreach (var lightGameObject in lightGameObjects)
            {
                Destroy(lightGameObject);
            }
        }
        Destroy(light);
        // set spot light
        foreach (var (x, y) in coordinates)
        {
            var position = chessBoard.transform.position + new Vector3(GetDstFromCm(x), GetDstFromCm(height), GetDstFromCm(y));
            var lightGameObject = new GameObject($"SpotLight{x}:{y}");
            var lightComp = lightGameObject.AddComponent<Light>();
            lightComp.type = LightType.Spot;
            lightComp.color = Color.white;
            lightComp.intensity = intensitySpotLight;
            lightComp.shadows = LightShadows.Soft;
            lightGameObject.transform.position = position;
            lightGameObject.transform.rotation = rotation;
            lightGameObjects.Add(lightGameObject);
        }
        // set ambient light
        RenderSettings.ambientIntensity = intensityAmbientLight;

        var radiusCoeffficient = 100f;
        var lightPosition = new Vector3(radiusCoeffficient * Convert.ToSingle(Math.Cos(ambientLightPhi*Math.PI / 180.0)),
                                        400,
                                        -radiusCoeffficient * Convert.ToSingle(Math.Sin(ambientLightPhi*Math.PI / 180.0)));
        light = new GameObject($"Directional{lightPosition.x}:{lightPosition.z}");
        rotation = Quaternion.Euler(60.0f, ambientLightPhi, 0.0f);
        var lightCompt = light.AddComponent<Light>();
        lightCompt.type = LightType.Directional;
        lightCompt.color = Color.white;
        lightCompt.shadows = LightShadows.Soft;
        lightCompt.intensity = intensityAmbientLight;
        light.transform.position = lightPosition;
        light.transform.rotation = rotation;
        
    }

    private void Start()
    {
        Logger.Log(kTag, "Chess generator start.");
        initGameTable();
        try
        {
            rangeValues = JsonConvert.DeserializeObject<RangeValues>(File.ReadAllText("Intervals.json"));
        }
        catch (Exception e)
        {
            Logger.Log(kTag, $"Error read Intervals.json: {e}");
        }

        try
        {
            var fenStrings = File.ReadAllText("Fen.data")
                .Split('\n')
                .Select(str => str.Trim())
                .Where(str => !string.IsNullOrEmpty(str));

            foreach (var fen in fenStrings)
            {
                gameStateMatrixLst.Add(new Pair<string[,], int>(GetGameMatrixFromFem(fen), rangeValues.ScreenshotPerFen));
            }
        }
        catch (Exception e)
        {
            Logger.Log(kTag, $"Error read Fen.data: {e}");
        }

        InitChessBoard(Resources.Load(BoardObjectName));
        InitChessFigures(Resources.Load(WhiteChessObjectName));
        InitChessFigures(Resources.Load(BlackChessObjectName));

        ResWidth = rangeValues.ScreenshotWidth;
        ResHeight = rangeValues.ScreenshotHeight;

        gameStateIndex = 0;
        mainCamera = Camera.main;
        GameObject cameraObj = new GameObject("OtherCamera");
        cameraForScreenshot = cameraObj.AddComponent<Camera>();

        sphericalCoordinates = new SphericalCoordinates(mainCamera.transform.position);
        mainCamera.transform.position = sphericalCoordinates.toCartesian + chessBoard.transform.position;
    }


    public void Update()
    {
        if (gameStateIndex < gameStateMatrixLst.Count)
        {
            var shotCount = gameStateMatrixLst[gameStateIndex].Second;
            if (shotCount == 0)
            {
                gameStateIndex++;
            }
        }
        else
        {
            Application.Quit();
        }
    }

    public void OnPostRender()
    {
        if (gameStateIndex < gameStateMatrixLst.Count)
        {
            var randomizedValues = IntervalRandomizer(rangeValues);

            mainCamera.transform.position = sphericalCoordinates.SetRotation(
                randomizedValues.CameraPhi, randomizedValues.CameraTheta).toCartesian + chessBoard.transform.position;
            mainCamera.transform.position = sphericalCoordinates.SetRadius(
                GetDstFromCm(randomizedValues.CameraRadius)).toCartesian + chessBoard.transform.position;
            mainCamera.transform.LookAt(chessBoard.transform.position);

            chessBoard.transform.position += new Vector3(GetDstFromCm(randomizedValues.BoardPositionX), 0.0f,
                                                         GetDstFromCm(randomizedValues.BoardPositionY));


            var gameStateMatrix = gameStateMatrixLst[gameStateIndex].First;
            for (var i = 0; i < BoardSize; i++)
            {
                for (var j = 0; j < BoardSize; j++)
                {
                    if (chessFigures.ContainsKey(gameStateMatrix[i, j]))
                    {
                        InitChessFigure(chessFigures[gameStateMatrix[i, j]], i, j, randomizedValues.ChessmanOffset);
                    }
                }
            }

            InitLights(randomizedValues.SpotLightPositionZ,
                       randomizedValues.SpotLightPositions,
                       randomizedValues.SpotLightBrightness,
                       randomizedValues.AmbientLightBrightness,
                       randomizedValues.AmbientLightPhi);

            cameraForScreenshot.CopyFrom(mainCamera);

            var file = TakeScreenshot(cameraForScreenshot, ResWidth, ResHeight);
            var cornerPositions = new List<Vector3>();

            var boardPosition = chessBoard.transform.position + gridOffset;
            cornerPositions.Add(ConvertToPixel(cameraForScreenshot, ResWidth, ResHeight, boardPosition + new Vector3(0 * gridStep, 0.0f, 0 * gridStep)));
            cornerPositions.Add(ConvertToPixel(cameraForScreenshot, ResWidth, ResHeight, boardPosition + new Vector3(0 * gridStep, 0.0f, 7 * gridStep)));
            cornerPositions.Add(ConvertToPixel(cameraForScreenshot, ResWidth, ResHeight, boardPosition + new Vector3(7 * gridStep, 0.0f, 0 * gridStep)));
            cornerPositions.Add(ConvertToPixel(cameraForScreenshot, ResWidth, ResHeight, boardPosition + new Vector3(7 * gridStep, 0.0f, 7 * gridStep)));

            var filenameCoords = "SavedScreen";
            foreach (Vector3 screenPos in cornerPositions)
            {
                filenameCoords += $"_{screenPos.x:###}_{screenPos.y:###}"; ;
            }
            RemoveChessFigures();
            gameStateMatrixLst[gameStateIndex].Second -= 1;

            // For testing purposes, also write to a file in the project folder
            var path = Application.dataPath + $"/../{gameStateIndex}";
            Directory.CreateDirectory(path);
            File.WriteAllBytes(path + $"/{filenameCoords}.png", file);
            chessBoard.transform.position -= new Vector3(GetDstFromCm(randomizedValues.BoardPositionX), 0.0f,
                                                         GetDstFromCm(randomizedValues.BoardPositionY));
        }
        else
        {
            Application.Quit();
        }
    }

    private static Vector3 ConvertToPixel(Camera camera,
                                          int ResWidth,
                                          int ResHeight,
                                          Vector3 origPosition)
    {
        float coefWidth = ResWidth / Convert.ToSingle(camera.pixelWidth);
        float coefHeight = ResHeight / Convert.ToSingle(camera.pixelHeight);
        var pixelPosition = camera.WorldToScreenPoint(origPosition);
        pixelPosition.x *= coefWidth;
        pixelPosition.y *= coefHeight;
        return pixelPosition;
    }
    
    //Стырено из статьи https://towardsdatascience.com/transcribe-live-chess-with-machine-learning-part-1-928f73306e1f
    private static byte[] TakeScreenshot(Camera camera, int ResWidth, int ResHeight)
    {
        var rt = new RenderTexture(ResWidth, ResHeight, 24);
        camera.targetTexture = rt;
        var screenShot = new Texture2D(ResWidth, ResHeight, TextureFormat.RGB24, false);
        camera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, ResWidth, ResHeight), 0, 0);
        var screenShotPng = screenShot.EncodeToPNG();
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        Destroy(screenShot);
        return screenShotPng;
    }

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

    public static RandomizedValues IntervalRandomizer(RangeValues values)
    {
        var randomized = new RandomizedValues
        {
            AmbientLightBrightness = Random.Range(values.AmbientLightBrightness.Start, values.AmbientLightBrightness.End),
            AmbientLightPhi = Random.Range(values.AmbientLightPhi.Start, values.AmbientLightPhi.End),
            CameraRadius = Random.Range(values.CameraRadius.Start, values.CameraRadius.End),
            CameraPhi = Random.Range(Helpers.DegreeToRadian(values.CameraPhi.Start), Helpers.DegreeToRadian(values.CameraPhi.End)),
            CameraTheta = Random.Range(Helpers.DegreeToRadian(values.CameraTheta.Start), Helpers.DegreeToRadian(values.CameraTheta.End)),
            ChessBoardWidth = Random.Range(values.ChessBoardWidth.Start, values.ChessBoardWidth.End),
            ChessmanOffset = Random.Range(values.ChessmanOffset.Start, values.ChessmanOffset.End),
            PathToModels = values.PathToModels,
            BoardPositionX = Random.Range(values.BoardPositionX.Start, values.BoardPositionX.End),
            BoardPositionY = Random.Range(values.BoardPositionY.Start, values.BoardPositionY.End),
            ScreenshotPerFen = values.ScreenshotPerFen,
            ScreenshotWidth = values.ScreenshotWidth,
            ScreenshotHeight = values.ScreenshotHeight,
            SpotLightBrightness = Random.Range(values.SpotLightBrightness.Start, values.SpotLightBrightness.End),
            SpotLightNumber = Random.Range(values.SpotLightNumber.Start, values.SpotLightNumber.End),
            SpotLightPositionZ = Random.Range(values.SpotLightPositionZ.Start, values.SpotLightPositionZ.End),
            SpotLightPositions = new List<(float x, float y)>()
        };

        for (var i = 0; i < randomized.SpotLightNumber; i++)
        {
            var spotLightPositionX =
                Random.Range(values.SpotLightPositionX.Start, values.SpotLightPositionX.End);
            var spotLightPositionY = Random.Range(values.SpotLightPositionY.Start, values.SpotLightPositionY.End);
            randomized.SpotLightPositions.Add((spotLightPositionX, spotLightPositionY));
        }
        return randomized;
    }
}

