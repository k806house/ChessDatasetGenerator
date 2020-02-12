using System;
using System.Collections.Generic;
using Assets.Scripts.DTO;
using UnityEngine;
using static Assets.Scripts.Utils.MathHelpers;
using static Assets.Scripts.Core.Helpers;
using static Assets.Scripts.Core.Constants;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Core
{
    public class BoardProcess : MonoBehaviour
    {
        [SerializeField]
        private GameObject gameTable;
        
        private Vector3 initPos;
        // все пропорции в мире шахмат считаются от ширины доски
        private float boardWidth; // требует создания доски
        private float figureScaleFactor;

        // сетка для шахматной доски
        private Vector3 gridOffset;
        private float gridStep;
        private readonly Dictionary<string, GameObject> chessFigures = new Dictionary<string, GameObject>();
        private readonly List<GameObject> currentChessFigures = new List<GameObject>();

        private static readonly ILogger Logger = Debug.unityLogger;

        private void Start()
        {
            InitGameTable();
            InitChessBoard();
            try
            {
                InitChessFigures(Resources.Load("Models/" + WhiteChessObjectName));
                InitChessFigures(Resources.Load("Models/" + BlackChessObjectName));
            }
            catch (Exception e)
            {
                Logger.Log(KTag, $"Error loading models: {e}");
            }
        }

        public void RenderBoard()
        {
            var currentFenMatrix = StateManager.Instance.GetCurrentFenMatrix();
            if (currentFenMatrix == null)
            {
                Application.Quit();
                return;
            }

            var currentSettings = SettingsManager.Instance.GetRandomizedValues();

            gameObject.transform.position += new Vector3(GetDstFromCm(currentSettings.BoardPositionX, boardWidth), 0.0f,
                GetDstFromCm(currentSettings.BoardPositionY, boardWidth));

            ResizeChessBoard(currentSettings.ChessBoardWidth);

            for (var i = 0; i < BoardSize; i++)
            {
                for (var j = 0; j < BoardSize; j++)
                {
                    if (chessFigures.ContainsKey(currentFenMatrix[i, j]))
                    {
                        InitChessFigure(chessFigures[currentFenMatrix[i, j]], i, j, currentSettings.ChessmanOffset);
                    }
                }
            }
        }

        private void InitGameTable()
        {
            var (bounds, center) = GetObjectRendererParams(gameTable);
            initPos = center + new Vector3(0.0f, bounds.y, 0.0f);
        }

        private void InitChessBoard()
        {
            //TODO: переписать с учётом того, что доска уже на сцене
            var rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
            gameObject.transform.position = initPos;
            gameObject.transform.rotation = rotation;

            var (bounds, _) = GetObjectRendererParams(gameObject);

            gameObject.transform.position += new Vector3(0.0f, bounds.y / 2.0f, 0.0f);

            const float proportion = 0.6f;
            var tableWidth = GetObjectRendererParams(gameTable).bounds.x;
            var scaleFactor = proportion * tableWidth / bounds.x;
            gameObject.transform.localScale *= scaleFactor;

            boardWidth = bounds.x * 2;

            gridStep = GetDstFromCm(BoardSquareCm, boardWidth); // !!! мина !!!

            var gridCenterOffset = (gridStep * BoardSize - gridStep) / 2.0f;
            gridOffset = new Vector3(-gridCenterOffset, bounds.y, -gridCenterOffset);
        }

        private void InitChessFigures(Object resource)
        {
            var rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
            var position = gameObject.transform.position;

            var chessFiguresResource = (GameObject)Instantiate(resource, position, rotation);

            var chessFiguresTransform = chessFiguresResource.GetComponentsInChildren<Transform>();
            var maxFigureHeight = 0.0f;

            foreach (var child in chessFiguresTransform)
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
            figureScaleFactor = GetDstFromCm(FigureHeightCm, boardWidth) / maxFigureHeight;
        }

        public void RemoveChessFigures()
        {
            foreach (var figure in currentChessFigures)
            {
                Destroy(figure);
            }
            currentChessFigures.Clear();
        }

        public void ResizeChessBoard(float newSizeCm = BoardWidthCm)
        {
            var (bounds, _) = GetObjectRendererParams(gameObject);
            boardWidth = bounds.x * 2;

            var currentSizeCm = GetCmFromDst(bounds.x * 2, boardWidth);
            var scaleFactor = newSizeCm / currentSizeCm;

            gameObject.transform.localScale *= scaleFactor;
            gridOffset *= scaleFactor;
            gridStep *= scaleFactor;
        }

        private void InitChessFigure(Object parentObj, int i, int j, float offset = 0.0f)
        {
            // случайное вращение фигуры вокруг своей оси
            var rotation = Quaternion.Euler(-90.0f, Random.Range(0.0f, 360.0f), 0.0f);
            // позиция фигуры по сетке
            var position = gameObject.transform.position + gridOffset + new Vector3(i * gridStep, 0.0f, j * gridStep);
            // сдвиг фигуры в случайную сторону на заданное расстояние
            // ..
            var direction = VectorFromAngle(Random.Range(0.0f, 360.0f));
            var offsetVector = direction * GetDstFromCm(offset, boardWidth);
            // ...
            var gameFigure = (GameObject)Instantiate(parentObj, position + offsetVector, rotation);

            gameFigure.transform.localScale *= figureScaleFactor;

            gameFigure.SetActive(true);
            currentChessFigures.Add(gameFigure);
        }
    }
}
