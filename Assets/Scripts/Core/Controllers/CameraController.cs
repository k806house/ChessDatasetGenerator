using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Core.Services;
using Assets.Scripts.Utils;
using UnityEngine;
using static Assets.Scripts.Core.Constants;
using static Assets.Scripts.Core.Helpers;


namespace Assets.Scripts.Core.Controllers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private GameObject chessBoard = default;

        private Camera cameraForScreenshots;
        private Camera mainCamera;

        private int resWidth;
        private int resHeight;

        private static SphericalCoordinates sphericalCoordinates;

        private SettingsService settingsService;
        private StateService stateService;
        private BoardController boardController;
        private LightController lightController;

        // ReSharper disable ParameterHidesMember
        public void Init(SettingsService settingsService, StateService stateService, BoardController boardController, LightController lightController)
        {
            this.settingsService = settingsService;
            this.stateService = stateService;
            this.lightController = lightController;
            this.boardController = boardController;
        }

        private void Start()
        {
            settingsService.RandomizeSettings();
            var rangeValues = settingsService.GetRandomizedValues();

            resWidth = rangeValues.ScreenshotWidth;
            resHeight = rangeValues.ScreenshotHeight;

            mainCamera = gameObject.GetComponent<Camera>();
            var cameraObj = new GameObject("OtherCamera");
            cameraForScreenshots = cameraObj.AddComponent<Camera>();

            sphericalCoordinates = new SphericalCoordinates(mainCamera.transform.position);
            mainCamera.transform.position = sphericalCoordinates.ToCartesian + chessBoard.transform.position;
        }

        public void OnPostRender()
        {
            //only for debug in Unity
            if (!stateService.IsNewStateAvailable()) return;

            settingsService.RandomizeSettings();
            var randomizedValues = settingsService.GetRandomizedValues();

            var (bounds, _) = GetObjectRendererParams(chessBoard);
            chessBoard.transform.position += new Vector3(0.0f, bounds.y / 2.0f, 0.0f);
            var boardWidth = bounds.x * 2;

            mainCamera.transform.position = sphericalCoordinates.SetRotation(
                                                randomizedValues.CameraPhi, randomizedValues.CameraTheta).ToCartesian + chessBoard.transform.position;
            mainCamera.transform.position = sphericalCoordinates.SetRadius(
                                                GetDstFromCm(randomizedValues.CameraRadius, boardWidth)).ToCartesian + chessBoard.transform.position;
            mainCamera.transform.LookAt(chessBoard.transform.position);

            boardController.RenderBoard();
            lightController.RenderLights();

            //only for debug in Unity
            if (!stateService.IsNewStateAvailable()) return;

            cameraForScreenshots.CopyFrom(mainCamera);

            var file = TakeScreenshot(cameraForScreenshots, resWidth, resHeight);
            var cornerPositions = new List<Vector3>();

            (bounds, _) = GetObjectRendererParams(chessBoard);
            chessBoard.transform.position += new Vector3(0.0f, bounds.y / 2.0f, 0.0f);
            boardWidth = bounds.x * 2;

            var gridStep = GetDstFromCm(BoardSquareCm, boardWidth); // !!! мина !!!
            var gridCenterOffset = (gridStep * BoardSize - gridStep) / 2.0f;
            var gridOffset = new Vector3(-gridCenterOffset, bounds.y, -gridCenterOffset);

            var boardPosition = chessBoard.transform.position + gridOffset;
            cornerPositions.Add(ConvertToPixel(cameraForScreenshots, resWidth, resHeight, boardPosition + new Vector3(0 * gridStep, 0.0f, 0 * gridStep)));
            cornerPositions.Add(ConvertToPixel(cameraForScreenshots, resWidth, resHeight, boardPosition + new Vector3(0 * gridStep, 0.0f, 7 * gridStep)));
            cornerPositions.Add(ConvertToPixel(cameraForScreenshots, resWidth, resHeight, boardPosition + new Vector3(7 * gridStep, 0.0f, 0 * gridStep)));
            cornerPositions.Add(ConvertToPixel(cameraForScreenshots, resWidth, resHeight, boardPosition + new Vector3(7 * gridStep, 0.0f, 7 * gridStep)));

            var filenameCoords = "SavedScreen";
            foreach (var screenPos in cornerPositions)
            {
                filenameCoords += $"_{screenPos.x:###}_{screenPos.y:###}"; ;
            }

            // For testing purposes, also write to a file in the project folder
            var path = Application.dataPath + $"/../{stateService.GetCurrentStateIndex()}";
            Directory.CreateDirectory(path);
            File.WriteAllBytes(path + $"/{filenameCoords}.png", file);
            chessBoard.transform.position -= new Vector3(GetDstFromCm(randomizedValues.BoardPositionX, boardWidth), 0.0f,
                GetDstFromCm(randomizedValues.BoardPositionY, boardWidth));

            //TODO:  пустой ресайз не очевидно, что делает
            boardController.ResizeChessBoard();
            boardController.RemoveChessFigures();
        }

        private static Vector3 ConvertToPixel(Camera camera, int resWidth, int resHeight, Vector3 origPosition)
        {
            var coefficientWidth = resWidth / Convert.ToSingle(camera.pixelWidth);
            var coefficientHeight = resHeight / Convert.ToSingle(camera.pixelHeight);
            var pixelPosition = camera.WorldToScreenPoint(origPosition);
            pixelPosition.x *= coefficientWidth;
            pixelPosition.y *= coefficientHeight;
            return pixelPosition;
        }

        //Стырено из статьи https://towardsdatascience.com/transcribe-live-chess-with-machine-learning-part-1-928f73306e1f
        private static byte[] TakeScreenshot(Camera camera, int resWidth, int resHeight)
        {
            var rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt;
            var screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            camera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            var screenShotPng = screenShot.EncodeToPNG();
            camera.targetTexture = null;
            RenderTexture.active = null;
            Destroy(rt);
            Destroy(screenShot);
            return screenShotPng;
        }
    }
}
