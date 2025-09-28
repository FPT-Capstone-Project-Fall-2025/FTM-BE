using FTM.Domain.Models.Authen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Interface
{
    public interface ISendOTPTrackingRepository : IGenericRepository<SendOTPTracking>
    {
        Task<SendOTPTracking> GetLastOTPTracking(string email = null, string phone = null);
        Task<List<SendOTPTracking>> GetSendOTPTrackingAsync(string ipAddress, string email = null, string phone = null);
    }
}
