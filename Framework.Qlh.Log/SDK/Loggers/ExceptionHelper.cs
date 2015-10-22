using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Framework.Qlh.Log.Loggers
{
    [ExcludeFromCodeCoverage]
    internal static class ExceptionHelper
    {
        public static bool MustBeRethrown(this Exception exception)
        {
            if (exception is StackOverflowException)
            {
                return true;
            }

            if (exception is ThreadAbortException)
            {
                return true;
            }

            if (exception is OutOfMemoryException)
            {
                return true;
            }

            return false;
        }
    }
}
