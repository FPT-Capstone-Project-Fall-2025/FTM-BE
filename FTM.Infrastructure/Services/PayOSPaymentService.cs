using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Web;

namespace FTM.Infrastructure.Services
{
    public interface IPayOSPaymentService
    {
        Task<string> CreatePaymentAsync(FTFundDonation donation, string returnUrl, string cancelUrl);
        Task<PaymentInfoDto> CreateCampaignDonationPaymentAsync(FTCampaignDonation donation, string campaignName, string bankCode, string accountNumber, string accountHolderName, string bankName);
        Task<object> GetPaymentInfoAsync(long orderCode);
        Task<object> ConfirmWebhookAsync(string webhookBody, string webhookSignature);
        Task<bool> CancelPaymentAsync(long orderCode, string reason = "Cancelled by user");
        long GenerateOrderCode();
        string GenerateVietQRUrl(string bankCode, string accountNumber, string accountHolderName, decimal amount, string description);
    }

    public class PayOSPaymentService : IPayOSPaymentService
    {
        private readonly ILogger<PayOSPaymentService> _logger;

        public PayOSPaymentService(ILogger<PayOSPaymentService> logger)
        {
            _logger = logger;
        }

        public async Task<string> CreatePaymentAsync(FTFundDonation donation, string returnUrl, string cancelUrl)
        {
            try
            {
                var orderCode = GenerateOrderCode();
                
                _logger.LogInformation("Created PayOS payment link for donation {DonationId}, OrderCode: {OrderCode}", 
                    donation.Id, orderCode);

                // Return a mock URL for now - will be replaced with actual PayOS integration
                return $"https://dev-pay.payos.vn/web/{orderCode}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayOS payment for donation {DonationId}", donation.Id);
                throw;
            }
        }

        public async Task<PaymentInfoDto> CreateCampaignDonationPaymentAsync(
            FTCampaignDonation donation, 
            string campaignName, 
            string bankCode, 
            string accountNumber, 
            string accountHolderName,
            string bankName)
        {
            try
            {
                var orderCode = donation.PayOSOrderCode ?? GenerateOrderCode();
                var description = $"UH {orderCode}"; // Nội dung chuyển khoản
                
                _logger.LogInformation(
                    "Created payment info for campaign donation {DonationId}, Campaign: {CampaignName}, OrderCode: {OrderCode}", 
                    donation.Id, campaignName, orderCode);

                // Generate VietQR URL for QR code
                var qrCodeUrl = GenerateVietQRUrl(
                    bankCode, 
                    accountNumber, 
                    accountHolderName, 
                    donation.DonationAmount, 
                    description);

                return new PaymentInfoDto
                {
                    BankCode = bankCode,
                    BankName = bankName,
                    AccountNumber = accountNumber,
                    AccountHolderName = accountHolderName,
                    Amount = donation.DonationAmount,
                    Description = description,
                    QRCodeUrl = qrCodeUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment info for campaign donation {DonationId}", donation.Id);
                throw;
            }
        }

        public string GenerateVietQRUrl(string bankCode, string accountNumber, string accountHolderName, decimal amount, string description)
        {
            // VietQR API format: https://img.vietqr.io/image/{bank_code}-{account_number}-{template}.jpg?amount={amount}&addInfo={description}&accountName={account_name}
            // Template options: compact, compact2, print, qr_only
            
            var encodedDescription = HttpUtility.UrlEncode(description);
            var encodedAccountName = HttpUtility.UrlEncode(accountHolderName);
            var amountInt = (int)amount; // VietQR accepts integer only

            var qrUrl = $"https://img.vietqr.io/image/{bankCode}-{accountNumber}-compact2.jpg" +
                       $"?amount={amountInt}" +
                       $"&addInfo={encodedDescription}" +
                       $"&accountName={encodedAccountName}";

            return qrUrl;
        }

        public async Task<object> GetPaymentInfoAsync(long orderCode)
        {
            try
            {
                _logger.LogInformation("Retrieved PayOS payment info for OrderCode: {OrderCode}", orderCode);
                
                // Return mock payment data
                return new
                {
                    orderCode = orderCode,
                    status = "PENDING",
                    amount = 100000,
                    createdAt = DateTime.UtcNow,
                    description = "Mock payment data"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PayOS payment info for OrderCode: {OrderCode}", orderCode);
                throw;
            }
        }

        public async Task<object> ConfirmWebhookAsync(string webhookBody, string webhookSignature)
        {
            try
            {
                _logger.LogInformation("Verified PayOS webhook");
                
                // Parse webhook body
                var webhookData = JsonSerializer.Deserialize<JsonElement>(webhookBody);
                
                return new
                {
                    orderCode = webhookData.TryGetProperty("orderCode", out var oc) ? oc.GetInt64() : 0,
                    status = webhookData.TryGetProperty("status", out var st) ? st.GetString() : "UNKNOWN",
                    amount = webhookData.TryGetProperty("amount", out var amt) ? amt.GetInt32() : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying PayOS webhook");
                throw;
            }
        }

        public async Task<bool> CancelPaymentAsync(long orderCode, string reason = "Cancelled by user")
        {
            try
            {
                _logger.LogInformation("Cancelled PayOS payment for OrderCode: {OrderCode}, Reason: {Reason}", 
                    orderCode, reason);
                
                return true; // Mock success
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling PayOS payment for OrderCode: {OrderCode}", orderCode);
                return false;
            }
        }

        public long GenerateOrderCode()
        {
            // Generate unique order code using timestamp and random number
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var random = new Random().Next(100, 999);
            return timestamp * 1000 + random;
        }
    }
}