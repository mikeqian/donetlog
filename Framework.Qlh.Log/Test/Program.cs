using System.IO;
using Framework.Qlh.Log;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var l = LogManager.GetLogger("Aaa");
            var l2 = LogManager.GetLogger("Bbb");

            l.Debug("to jest debug");
            l.Info("to jest info");
            l.Warn("to jest warning");
            l2.Debug("to jest debug");
            l2.Info("to jest info");
            l2.Warn("to jest warning");
            l.Error("to jest error");
            l.Fatal("to jest fatal");
            l2.Error("to jest error");
            l2.Fatal("to jest fatal");

            File.Copy("Config1.mlog", "Test.exe.config", true);
            System.Threading.Thread.Sleep(100);

            l.Debug("to jest debug");
            l.Info("to jest info");
            l.Warn("to jest warning");
            l2.Debug("to jest debug");
            l2.Info("to jest info");
            l2.Warn("to jest warning");
            l.Error("to jest error");
            l.Fatal("to jest fatal");
            l2.Error("to jest error");
            l2.Fatal("to jest fatal");
            File.Copy("Config2.mlog", "Test.exe.config", true);
            System.Threading.Thread.Sleep(100);
            l.Debug("to jest debug");
            l.Info("to jest info");
            l.Warn("to jest warning");
            l2.Debug("to jest debug");
            l2.Info("to jest info");
            l2.Warn("to jest warning");
            l.Error("to jest error");
            l.Fatal("to jest fatal");
            l2.Error("to jest error");
            l2.Fatal("to jest fatal");
        }
    }
}
