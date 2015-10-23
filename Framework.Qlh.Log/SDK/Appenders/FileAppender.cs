using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Framework.Qlh.Log.Appenders
{
    public class FileAppender : Appender
    {
        private bool _autoFlush = true;
        private int _concurrentWriteAttemptDelay = 1;
        private int _concurrentWriteAttempts = 10;
        private bool _concurrentWrites = true;
        private Encoding _encoding = System.Text.Encoding.Default;
        private Layout _fileNameLayout;
        private bool _keepFileOpen = true;
        private string _lastFileName = String.Empty;
        private StreamWriter _outputFile;
        private Random _random = new Random();

        public FileAppender()
        {
            CreateDirs = false;
        }

        public string FileName
        {
            get { return _fileNameLayout.Text; }
            set { _fileNameLayout = new Layout(value); }
        }

        public bool CreateDirs { get; set; }

        public bool KeepFileOpen
        {
            get { return _keepFileOpen; }
            set { _keepFileOpen = value; }
        }

        public bool AutoFlush
        {
            get { return _autoFlush; }
            set { _autoFlush = value; }
        }

        public string Encoding
        {
            get { return _encoding.WebName; }
            set { _encoding = System.Text.Encoding.GetEncoding(value); }
        }

        public bool ConcurrentWrites
        {
            get { return _concurrentWrites; }
            set { _concurrentWrites = value; }
        }

        public int ConcurrentWriteAttempts
        {
            get { return _concurrentWriteAttempts; }
            set { _concurrentWriteAttempts = value; }
        }

        public int ConcurrentWriteAttemptDelay
        {
            get { return _concurrentWriteAttemptDelay; }
            set { _concurrentWriteAttemptDelay = value; }
        }

        private StreamWriter OpenStreamWriter(string fileName)
        {
            StreamWriter retVal;

            var fi = new FileInfo(fileName);
            if (!fi.Exists)
            {
                if (!fi.Directory.Exists)
                {
                    Directory.CreateDirectory(fi.DirectoryName);
                }
            }

            if (!ConcurrentWrites)
            {
                retVal = new StreamWriter(fileName, true, _encoding);
            }
            else
            {
                int currentDelay = _concurrentWriteAttemptDelay;
                retVal = null;

                for (int i = 0; i < _concurrentWriteAttempts; ++i)
                {
                    try
                    {
                        retVal = new StreamWriter(fileName, true, _encoding);
                        break;
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine("ex: {0}", ex.Message);
                        int actualDelay = _random.Next(currentDelay);
                        currentDelay *= 2;
                        Thread.Sleep(actualDelay);
                    }
                }
            }

            retVal.AutoFlush = _autoFlush;
            return retVal;
        }

        public override void Append(LogEventInfo ev)
        {
            string fileName = _fileNameLayout.GetFormattedMessage(ev);

            if (fileName != _lastFileName && _outputFile != null)
            {
                _outputFile.Close();
                _outputFile = null;
            }
            _lastFileName = fileName;
            if (_outputFile == null)
            {
                _outputFile = OpenStreamWriter(fileName);
                if (_outputFile == null)
                    return;
            }
            _outputFile.WriteLine(CompiledLayout.GetFormattedMessage(ev));
            _outputFile.Flush();
            if (!KeepFileOpen || ConcurrentWrites)
            {
                _outputFile.Close();
                _outputFile = null;
            }
        }
    }
}