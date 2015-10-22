using System;

namespace Framework.Qlh.Log
{
    public interface IBaseLogger : IDisposable
    {
        void Flush();
    }
}