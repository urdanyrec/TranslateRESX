using System;

namespace TranslateRESX.Core.Helpers
{
    public class ControllerTaskResult
    {
        public int AllCount { get; set; }

        public int CompleteCount { get; set; }

        public bool TargetFileExists { get; set; }

        public bool TargetFileRecovered { get; set; }

        public bool Success { get; set; }

        public Exception Error { get; set; }
    }
}
