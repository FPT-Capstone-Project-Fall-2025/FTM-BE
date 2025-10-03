using FTM.Domain.DTOs.Authen;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.Implement
{
    public class SendOTPTrackingRepository : GenericRepository<SendOTPTracking>, ISendOTPTrackingRepository
    {
        private readonly FTMDbContext _context;

        public SendOTPTrackingRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) : base(context, currentUserResolver)
        {
            _context = context;
        }

        public async Task<List<SendOTPTracking>> GetSendOTPTrackingAsync(string ipAddress, string email = null, string phone = null)
        {
            var currentTime = DateTimeOffset.Now;
            return await _context.SendOTPTrackings
                .OrderByDescending(x => x.LastModifiedOn)
                .Where(x => x.RemoteIpAddress == ipAddress
                    && (email == null || x.Email == email)
                    && (phone == null || x.PhoneNumber == phone)
                    && x.LastModifiedOn >= currentTime.AddMinutes(-10))
                .ToListAsync();
        }

        public async Task<SendOTPTracking> GetLastOTPTracking(string email = null, string phone = null)
        {
            return await _context.SendOTPTrackings
                .OrderByDescending(x => x.LastModifiedOn)
                .FirstOrDefaultAsync(x => (string.IsNullOrEmpty(email) || x.Email == email)
                    && (string.IsNullOrEmpty(phone) || x.PhoneNumber == phone));
        }
    }
}
