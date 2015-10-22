using System;

namespace Framework.Qlh.Log.Entity.Log
{
    internal abstract class BaseLog
    {
        public string AppNo { get; set; }

        public string Ip { get; set; }

        public string Message { get; set; }

        public DateTime CreateTime { get; set; }

        public string Content { get; set; }
    }
}
