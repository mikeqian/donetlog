using System;

namespace Framework.Qlh.Log
{
    public interface IBaseLogger : ILogger, IDisposable
    {
        void Flush();
    }
}