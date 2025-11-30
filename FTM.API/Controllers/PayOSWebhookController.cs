using Microsoft.AspNetCore.Mvc;
using FTM.Infrastructure.Services;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text;
using FTM.API.Reponses;
using System.Text.Json;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/webhooks")]
    public class PayOSWebhookController : ControllerBase
    {
        private readonly IPayOSPaymentService _payOSPaymentService;
        private readonly IGenericRepository<FTFundDonation> _donationRepository;
        private readonly ILogger<PayOSWebhookController> _logger;

        public PayOSWebhookController(
            IPayOSPaymentService payOSPaymentService,
            IGenericRepository<FTFundDonation> donationRepository,
            ILogger<PayOSWebhookController> logger)
        {
            _payOSPaymentService = payOSPaymentService;
            _donationRepository = donationRepository;
            _logger = logger;
        }

        /// <summary>
        /// PayOS webhook endpoint to handle payment confirmations
        /// </summary>
        [HttpPost("payos")]
        public async Task<IActionResult> HandlePayOSWebhook()
        {
            try
            {
                // Read webhook body
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                var webhookBody = await reader.ReadToEndAsync();

                // Get webhook signature from header
                var webhookSignature = Request.Headers["X-PayOS-Signature"].FirstOrDefault();
                
                if (string.IsNullOrEmpty(webhookSignature))
                {
                    _logger.LogWarning("PayOS webhook received without signature");
                    return BadRequest("Missing webhook signature");
                }

                // Verify webhook
                var paymentData = await _payOSPaymentService.ConfirmWebhookAsync(webhookBody, webhookSignature);
                
                // Parse webhook data
                var webhookJson = JsonSerializer.Deserialize<JsonElement>(webhookBody);
                var orderCode = webhookJson.TryGetProperty("orderCode", out var oc) ? oc.GetInt64() : 0L;
                var status = webhookJson.TryGetProperty("status", out var st) ? st.GetString() : "UNKNOWN";
                
                _logger.LogInformation("PayOS webhook received for OrderCode: {OrderCode}, Status: {Status}", 
                    orderCode, status);

                // Process payment confirmation
                await ProcessPaymentConfirmationAsync(orderCode, status ?? "UNKNOWN");

                return Ok(new ApiSuccess("Webhook processed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PayOS webhook");
                return StatusCode(500, new ApiError("Error processing webhook", ex.Message));
            }
        }

        /// <summary>
        /// Manual endpoint to check payment status
        /// </summary>
        [HttpGet("payos/check/{orderCode}")]
        public async Task<IActionResult> CheckPaymentStatus(long orderCode)
        {
            try
            {
                var paymentData = await _payOSPaymentService.GetPaymentInfoAsync(orderCode);
                await ProcessPaymentConfirmationAsync(orderCode, "PAID"); // Assume paid for manual check
                
                return Ok(new ApiSuccess("Payment status checked", paymentData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking payment status for OrderCode: {OrderCode}", orderCode);
                return StatusCode(500, new ApiError("Error checking payment status", ex.Message));
            }
        }

        private async Task ProcessPaymentConfirmationAsync(long orderCode, string status)
        {
            try
            {
                _logger.LogInformation("Processing payment confirmation for OrderCode: {OrderCode}, Status: {Status}", 
                    orderCode, status);

                // If payment is successful, confirm related donation
                if (status.Equals("PAID", StringComparison.OrdinalIgnoreCase))
                {
                    await ConfirmRelatedDonationAsync(orderCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment confirmation");
                throw;
            }
        }

        private async Task ConfirmRelatedDonationAsync(long orderCode)
        {
            try
            {
                // Find donation with matching order code
                var donation = await _donationRepository.GetQuery()
                    .Include(d => d.Fund)
                    .FirstOrDefaultAsync(d => d.PayOSOrderCode == orderCode && d.IsDeleted == false);

                if (donation == null)
                {
                    _logger.LogWarning("No donation found for PayOS OrderCode: {OrderCode}", orderCode);
                    return;
                }

                if (donation.Status == DonationStatus.Confirmed)
                {
                    _logger.LogInformation("Donation {DonationId} already confirmed", donation.Id);
                    return;
                }

                // Confirm donation
                donation.Status = DonationStatus.Confirmed;
                donation.ConfirmedOn = DateTimeOffset.UtcNow;
                donation.ConfirmationNotes = $"Auto-confirmed via PayOS payment. OrderCode: {orderCode}";

                _donationRepository.Update(donation);

                _logger.LogInformation("Auto-confirmed donation {DonationId} via PayOS payment {OrderCode}", 
                    donation.Id, orderCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming donation for OrderCode: {OrderCode}", orderCode);
                throw;
            }
        }
    }
}