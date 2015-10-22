namespace Framework.Qlh.Log
{
    public interface ILogger
    {
        void Info(string content);

        void Error(string content);

        void Warning(string content);

        void Debug(string content);
    }
}