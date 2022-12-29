using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Common
{
    public static class Logger
    {
        private static readonly ILogger _logger;

        static Logger()
        {
            //_errorLogger = new LoggerConfiguration()
            //    .WriteTo.Async(a =>{
            //        a.File("C:/Temp/logs.log", rollingInterval: RollingInterval.Minute);
            //    })
            //    .MinimumLevel.Verbose()
            //    .CreateLogger();


            _logger = new LoggerConfiguration()
    .WriteTo.File("C:/Robomat/Log/logs.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
        }

        public static async Task LogError(string error)
        {
            await Task.Run(() =>
            {
                _logger.Error(error);

            });
        }

        public static async Task LogInfo(string info)
        {
            await Task.Run(() =>
            {
                _logger.Information(info);
            });
        }

        public static async Task LogDebug(string debug)
        {
            await Task.Run(() =>
            {
                _logger.Debug(debug);
            });
        }


        public static void Error(string error)
        {
            _logger.Error(error);
        }

        public static void Dispose()
        {

        }

    }


}
