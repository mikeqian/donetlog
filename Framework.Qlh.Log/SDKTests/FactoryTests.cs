using System.IO;
using Framework.Qlh.Log;
using Framework.Qlh.Log.Loggers;
using NLog;
using NUnit.Framework;

namespace SDKTests
{
    [TestFixture]
    public class FactoryTests
    {
        [Test]
        public void CreateAppLoggerTest()
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

            File.Copy("Config1.nlog", "NLog.Test.exe.config", true);
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
            File.Copy("Config2.nlog", "NLog.Test.exe.config", true);
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
