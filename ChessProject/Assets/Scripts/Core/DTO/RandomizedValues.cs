using System.Collections.Generic;

namespace Assets.Scripts.DTO
{
    public class RandomizedValues
    {
        public float BoardPositionX { get; set; }
        public float BoardPositionY { get; set; }
        public float CameraRadius { get; set; }
        public float CameraPhi { get; set; }
        public float CameraTheta { get; set; }
        public int SpotLightNumber { get; set; }
        public float SpotLightPositionZ { get; set; }
        public List<(float x, float y)> SpotLightPositions { get; set; }
        public float SpotLightBrightness { get; set; }
        public float AmbientLightBrightness { get; set; }
        public float AmbientLightPhi { get; set; }
        public float ChessBoardWidth { get; set; }
        public float ChessmanOffset { get; set; }
        public int ScreenshotPerFen { get; set; }
        public int ScreenshotWidth { get; set; }
        public int ScreenshotHeight { get; set; }
        public string PathToModels { get; set; }
    }
}
