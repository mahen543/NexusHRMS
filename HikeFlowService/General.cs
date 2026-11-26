using log4net;
using log4net.Config;
using System.Reflection;

namespace HikeFlowService
{
    public class General
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(General));
        private readonly bool codeLogRequired;

        public General(IConfiguration config)
        {
            GlobalContext.Properties["LogFolder"] = AppContext.BaseDirectory;
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()!),
                new FileInfo(Path.Combine(AppContext.BaseDirectory, "log4net.config")));
            codeLogRequired = config["AppSettings:codeLogRequired"] == "1";
        }

        public void CreateCodeLog(string className, string methodName, string message)
        {
            if (codeLogRequired)
                log.Info($"{className}.{methodName} - {message}");
        }

        public void CreateErrorLog(Exception ex)
        {
            log.Error(ex.ToString());
        }
    }
}