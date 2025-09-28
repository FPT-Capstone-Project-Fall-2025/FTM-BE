using FTM.Application.IServices;
using FTM.Domain.Models.Authen;
using Infobip.Api.Client;
using Infobip.Api.Client.Api;
using Infobip.Api.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Application.Services
{
    public class SendSMSService : ISendSMSService
    {
        private readonly Configuration _configuration = new Configuration();
        private readonly TwilioConfig _twilioConfig = new TwilioConfig();

        public SendSMSService()
        {
            _configuration.BasePath = Environment.GetEnvironmentVariable("SMS_BASE_URL") ?? "https://y35w41.api.infobip.com";
            _configuration.ApiKey = Environment.GetEnvironmentVariable("SMS_API_KEY") ?? "62d26f33fd024ad2c5bd80b57364f2c0-0b3365d8-6ccf-4eaa-9630-08d7b75b0c4e";
            _configuration.ApiKeyPrefix = Environment.GetEnvironmentVariable("SMS_PREFIX") ?? "App";
        }

        public void SendSMS(string phone, string content)
        {

            var sendSmsApi = new SendSmsApi(_configuration);
            var smsMessage = new SmsTextualMessage()
            {
                From = "GP SMS",
                Destinations = new List<SmsDestination>()
            {
                new SmsDestination(to: FormatPhoneNumber(phone))
            },
                Text = content
            };

            var smsRequest = new SmsAdvancedTextualRequest()
            {
                Messages = new List<SmsTextualMessage>() { smsMessage }
            };
            try
            {
                var smsResponse = sendSmsApi.SendSmsMessage(smsRequest);

                Console.WriteLine("Response: " + smsResponse.Messages.FirstOrDefault());
            }
            catch (ApiException apiException)
            {
                Console.WriteLine("Error occurred! \n\tMessage: {0}\n\tError content", apiException.ErrorContent);
            }
        }

        private static string FormatPhoneNumber(string phone, string contryCode = "84")
        {
            if (phone.StartsWith("+"))
            {
                return phone.Substring(1);
            }

            if (phone.StartsWith("84"))
            {
                return phone;
            }
            return contryCode + phone.Substring(1);
        }
    }
}
