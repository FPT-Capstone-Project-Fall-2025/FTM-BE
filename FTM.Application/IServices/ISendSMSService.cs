using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.IServices
{
    public interface ISendSMSService
    {
        public void SendSMS(string phone, string content);
    }
}
