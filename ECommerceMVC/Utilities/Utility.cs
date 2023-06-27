using ECommerceMVC.Interface;
using ECommerceMVC.Models;

namespace ECommerceMVC.Utilities
{
    public class Utility
    {
        private readonly IErrorLogger _errorLogger;

        public Utility() { }
        public Utility(IErrorLogger errorLogger) 
        { 
            _errorLogger = errorLogger; 
        }

        public async Task<ErrorLog> Logger(Exception e)
        {
            ErrorLog log = new ErrorLog();

            log.ErrorMessage = e.Message;
            log.DateTime = DateTime.Now;

            await _errorLogger.Log(log);
            await _errorLogger.SaveChanges();

            return log;
        }
    }
}
