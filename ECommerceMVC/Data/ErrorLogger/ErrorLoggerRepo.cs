using ECommerceMVC.Interface;
using ECommerceMVC.Models;

namespace ECommerceMVC.Data.ErrorLogger
{
    public class ErrorLoggerRepo : IErrorLogger
    {

        private readonly EcommerceApiContext _context;

        public ErrorLoggerRepo(EcommerceApiContext context)
        {
            _context = context;
        }

        public async Task Log(ErrorLog errorLog)
        { 
            await _context.Errors.AddAsync(errorLog);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
