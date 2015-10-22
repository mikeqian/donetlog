using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Framework.Qlh.Log.Loggers
{
    [ExcludeFromCodeCoverage]
    internal static class ExceptionHelper
    {
        /// <summary>
        /// Determines whether the exception must be rethrown.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns>True if the exception must be rethrown, false otherwise.</returns>
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
