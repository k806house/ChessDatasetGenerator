using System;

namespace Assets.Scripts.DTO
{
    public class RangeValues
    {
        public Interval<float> BoardPositionX { get; set; }
        public Interval<float> BoardPositionY { get; set; }
        public Interval<float> CameraRadius { get; set; }
        public Interval<float> CameraPhi { get; set; }
        public Interval<float> CameraTheta { get; set; }
        public Interval<int> SpotLightNumber { get; set; }
        public Interval<float> SpotLightPositionZ { get; set; }
        public Interval<float> SpotLightPositionX { get; set; }
        public Interval<float> SpotLightPositionY { get; set; }
        public Interval<float> SpotLightBrightness { get; set; }
        public Interval<float> AmbientLightBrightness { get; set; }
        public Interval<float> AmbientLightPhi { get; set; }
        public Interval<float> ChessBoardWidth { get; set; }
        public Interval<float> ChessmanOffset { get; set; }
        public int ScreenshotPerFen { get; set; }
        public int ScreenshotWidth { get; set; }
        public int ScreenshotHeight { get; set; }
        public string PathToModels { get; set; }
    }


    public class Interval<T> where T : IComparable<T>
    {
        public T Start { get; set; }
        public T End { get; set; }
    }
}
