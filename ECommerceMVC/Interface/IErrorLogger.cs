using ECommerceMVC.Models;

namespace ECommerceMVC.Interface
{
    public interface IErrorLogger
    {
        Task Log(ErrorLog errorLog);
        Task SaveChanges();
    }
}
