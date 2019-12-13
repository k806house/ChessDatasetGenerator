using Newtonsoft.Json;

namespace JsonExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var testJson = SerializeExample();
            var testObject = DeserializeExample();
        }

        public static string SerializeExample()
        {
            var doubleInterval = new Interval<double>
            {
                Start = 1,
                End = 2
            };

            var intInterval = new Interval<int>
            {
                Start = 1,
                End = 2
            };


            var serializeTestRange = new Range
            {
                PositionX = doubleInterval,
                PositionY = doubleInterval,
                CameraPhi = doubleInterval,
                CameraTheta = doubleInterval,
                SpotLightNumber = intInterval,
                SpotLightPositionX = doubleInterval,
                SpotLightPositionY = doubleInterval,
                SpotLightPositionZ = doubleInterval,
                SpotLightBrightness = doubleInterval,
                AmbientLightBrightness = doubleInterval,
                ChessBoardWidth = doubleInterval,
                ChessmanOffset = doubleInterval,
                SnapshotPerFen = 1,
                PathToModels = "test"
            };

            return JsonConvert.SerializeObject(serializeTestRange);
        }


        public static Range DeserializeExample()
        {
            var deserializeTestJson = @"{
                                          'positionX': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'positionY': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'cameraPhi': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'cameraTheta': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'spotLightNumber': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'spotLightPositionZ': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'spotLightPositionX': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'spotLightPositionY': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'spotLightBrightness': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'ambientLightBrightness': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'chessBoardWidth': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'chessmanOffset': {
                                              'start': 1,
                                              'end': 2
                                          },
                                          'snapshotPerFen': 0,
                                          'pathToModels': 'test'
                                       }";

            return JsonConvert.DeserializeObject<Range>(deserializeTestJson);
        }
    }
}
