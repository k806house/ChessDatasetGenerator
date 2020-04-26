using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts
{
    public class FileLogHandler : ILogHandler
    {
        private readonly StreamWriter streamWriter;
        private readonly ILogHandler defaultLogHandler = Debug.unityLogger.logHandler;

        public FileLogHandler()
        {
            var filePath = Application.dataPath + "/../Logs/ChessProject.log";

            var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            streamWriter = new StreamWriter(fileStream);

            // Replace the default debug log handler
            Debug.unityLogger.logHandler = this;
        }

        public void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            streamWriter.WriteLine(format, args);
            streamWriter.Flush();
            defaultLogHandler.LogFormat(logType, context, format, args);
        }

        public void LogException(Exception exception, Object context)
        {
            defaultLogHandler.LogException(exception, context);
        }
    }
}