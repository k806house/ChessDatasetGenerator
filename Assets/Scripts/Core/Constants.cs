namespace Assets.Scripts.Core
{
    public static class Constants 
    {
        //TODO: think about
        public const string KTag = "ChessTag";

        //TODO:enum or smth
        public const string BoardObjectName = "Board";
        public const string WhiteChessObjectName = "WhiteChess";
        public const string BlackChessObjectName = "BlackChess";

        public const int BoardSize = 8;
        public const float BoardWidthCm = 43.0f;
        public const float BoardOffsetCm = 2.0f;
        public const float BoardSquareCm = (BoardWidthCm - 2 * BoardOffsetCm) / BoardSize;
        public const float FigureHeightCm = 9.0f;
        public const float WorldScaleFactor = 1.0f;
    }
}
