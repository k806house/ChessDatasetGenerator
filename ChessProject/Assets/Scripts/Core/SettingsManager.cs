using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Common;
using Assets.Scripts.DTO;
using Assets.Scripts.Utils;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Assets.Scripts.Core.Constants;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Core
{
    public class SettingsManager : Singleton<SettingsManager>
    {
        private static RandomizedValues values;
        private static RangeValues rangeValues;

        private static readonly ILogger Logger = Debug.unityLogger;

        private void Start()
        {
            Logger.Log(KTag, "Chess generator start.");
            try
            {
                rangeValues = JsonConvert.DeserializeObject<RangeValues>(File.ReadAllText("Intervals.json"));
            }
            catch (Exception e)
            {
                Logger.Log(KTag, $"Error read Intervals.json: {e}");
            }
            SceneManager.LoadScene("Main Scene");
        }

        public void RandomizeSettings()
        {
            values = IntervalRandomizer(rangeValues);
        }

        //TODO: в идеале отказаться от использования этого класса и написать функции для всех полей AmbientLightBrightness и тд
        public RandomizedValues GetRandomizedValues() => values;

        public int GetNumberOfScreenshotsPerFen() => rangeValues.ScreenshotPerFen;

        public static RandomizedValues IntervalRandomizer(RangeValues values)
        {
            var randomized = new RandomizedValues
            {
                AmbientLightBrightness = Random.Range(values.AmbientLightBrightness.Start, values.AmbientLightBrightness.End),
                AmbientLightPhi = Random.Range(values.AmbientLightPhi.Start, values.AmbientLightPhi.End),
                CameraRadius = Random.Range(values.CameraRadius.Start, values.CameraRadius.End),
                CameraPhi = Random.Range(MathHelpers.DegreeToRadian(values.CameraPhi.Start), MathHelpers.DegreeToRadian(values.CameraPhi.End)),
                CameraTheta = Random.Range(MathHelpers.DegreeToRadian(values.CameraTheta.Start), MathHelpers.DegreeToRadian(values.CameraTheta.End)),
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
}
