using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using FTM.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;
using CampaignDonateRequest = FTM.Domain.DTOs.Funds.CampaignDonateRequest;
using ConfirmDonationDto = FTM.Domain.DTOs.Funds.ConfirmDonationDto;

namespace FTM.Tests.Controllers
{
    public class FTCampaignDonationControllerTests
    {
        private readonly Mock<IFTCampaignDonationService> _mockDonationService;
        private readonly Mock<IPayOSPaymentService> _mockPayOSService;
        private readonly Mock<IFTCampaignService> _mockCampaignService;
        private readonly Mock<IBlobStorageService> _mockBlobStorageService;
        private readonly FTCampaignDonationController _controller;
        private readonly ITestOutputHelper _output;

        public FTCampaignDonationControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockDonationService = new Mock<IFTCampaignDonationService>();
            _mockPayOSService = new Mock<IPayOSPaymentService>();
            _mockCampaignService = new Mock<IFTCampaignService>();
            _mockBlobStorageService = new Mock<IBlobStorageService>();
            _controller = new FTCampaignDonationController(
                _mockDonationService.Object,
                _mockPayOSService.Object,
                _mockCampaignService.Object,
                _mockBlobStorageService.Object);
        }

        #region DonateToCampaign Tests - POST /api/ftcampaigndonation/campaign/{campaignId}/donate

        [Fact(DisplayName = "UTCID01 - DonateToCampaign - Thành công - Tạo donation với cash payment")]
        public async Task DonateToCampaign_Success_CashPayment_ReturnsOk()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var request = new CampaignDonateRequest
            {
                MemberId = memberId,
                DonorName = "Nguyễn Văn A",
                Amount = 500000,
                PaymentMethod = PaymentMethod.Cash,
                PaymentNotes = "Tiền mặt",
                ProofImages = "https://blobstorage.com/proof1.jpg"
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignName = "Chiến dịch từ thiện",
                BankAccountNumber = "1234567890",
                BankCode = "970436",
                BankName = "Vietcombank",
                AccountHolderName = "Nguyễn Văn B"
            };
            var expectedDonation = new FTCampaignDonation
            {
                Id = Guid.NewGuid(),
                CampaignId = campaignId,
                FTMemberId = memberId,
                DonationAmount = 500000,
                DonorName = "Nguyễn Văn A",
                PaymentMethod = PaymentMethod.Cash,
                DonorNotes = "Tiền mặt",
                Status = DonationStatus.Pending,
                ProofImages = "https://blobstorage.com/proof1.jpg"
            };

            _mockDonationService
                .Setup(s => s.GetCampaignForDonationAsync(campaignId))
                .ReturnsAsync(campaign);

            _mockDonationService
                .Setup(s => s.AddAsync(It.IsAny<FTCampaignDonation>()))
                .ReturnsAsync(expectedDonation);

            // Act
            var result = await _controller.DonateToCampaign(campaignId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Donation created successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - DonateToCampaign - Thành công - Tạo donation với cash payment");
        }

        [Fact(DisplayName = "UTCID02 - DonateToCampaign - Thành công - Tạo donation với bank transfer")]
        public async Task DonateToCampaign_Success_BankTransfer_ReturnsOk()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var request = new CampaignDonateRequest
            {
                MemberId = memberId,
                DonorName = "Nguyễn Văn A",
                Amount = 1000000,
                PaymentMethod = PaymentMethod.BankTransfer,
                PaymentNotes = "Chuyển khoản"
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignName = "Chiến dịch từ thiện",
                BankAccountNumber = "1234567890",
                BankCode = "970436",
                BankName = "Vietcombank",
                AccountHolderName = "Nguyễn Văn B"
            };
            var expectedDonation = new FTCampaignDonation
            {
                Id = Guid.NewGuid(),
                CampaignId = campaignId,
                FTMemberId = memberId,
                DonationAmount = 1000000,
                DonorName = "Nguyễn Văn A",
                PaymentMethod = PaymentMethod.BankTransfer,
                DonorNotes = "Chuyển khoản",
                Status = DonationStatus.Pending,
                PayOSOrderCode = 123456789
            };
            var paymentInfo = new PaymentInfoDto
            {
                BankCode = "970436",
                BankName = "Vietcombank",
                AccountNumber = "1234567890",
                AccountHolderName = "Nguyễn Văn B",
                Amount = 1000000,
                Description = "Chiến dịch từ thiện",
                QRCodeUrl = "https://img.vietqr.io/image/970436-1234567890-compact2.jpg"
            };

            _mockDonationService
                .Setup(s => s.GetCampaignForDonationAsync(campaignId))
                .ReturnsAsync(campaign);

            _mockPayOSService
                .Setup(s => s.CreateCampaignDonationPaymentAsync(
                    It.IsAny<FTCampaignDonation>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(paymentInfo);

            _mockDonationService
                .Setup(s => s.AddAsync(It.IsAny<FTCampaignDonation>()))
                .ReturnsAsync(expectedDonation);

            // Act
            var result = await _controller.DonateToCampaign(campaignId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Donation created successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID02 - DonateToCampaign - Thành công - Tạo donation với bank transfer");
        }

        [Fact(DisplayName = "UTCID03 - DonateToCampaign - Thất bại - Campaign không tồn tại")]
        public async Task DonateToCampaign_CampaignNotFound_ReturnsNotFound()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var request = new CampaignDonateRequest
            {
                MemberId = Guid.NewGuid(),
                DonorName = "Nguyễn Văn A",
                Amount = 500000,
                PaymentMethod = PaymentMethod.Cash
            };

            _mockDonationService
                .Setup(s => s.GetCampaignForDonationAsync(campaignId))
                .ReturnsAsync((FTFundCampaign?)null);

            // Act
            var result = await _controller.DonateToCampaign(campaignId, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Campaign not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - DonateToCampaign - Thất bại - Campaign không tồn tại");
        }

        [Fact(DisplayName = "UTCID04 - DonateToCampaign - Thất bại - Campaign chưa có thông tin ngân hàng")]
        public async Task DonateToCampaign_CampaignMissingBankInfo_ReturnsBadRequest()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var request = new CampaignDonateRequest
            {
                MemberId = Guid.NewGuid(),
                Amount = 1000000,
                PaymentMethod = PaymentMethod.BankTransfer
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignName = "Chiến dịch từ thiện",
                BankAccountNumber = null, // Missing bank info
                BankCode = null,
                BankName = null,
                AccountHolderName = null
            };

            _mockDonationService
                .Setup(s => s.GetCampaignForDonationAsync(campaignId))
                .ReturnsAsync(campaign);

            // Act
            var result = await _controller.DonateToCampaign(campaignId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("bank account information", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - DonateToCampaign - Thất bại - Campaign chưa có thông tin ngân hàng");
        }

        #endregion

        #region GetPendingDonationsForManager Tests - GET /api/ftcampaigndonation/pending/manager/{managerId}

        [Fact(DisplayName = "UTCID01 - GetPendingDonationsForManager - Thành công - Trả về danh sách pending donations")]
        public async Task GetPendingDonationsForManager_Success_ReturnsDonations()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var page = 1;
            var pageSize = 20;
            var expectedDonations = new List<FTCampaignDonationResponseDto>
            {
                new FTCampaignDonationResponseDto
                {
                    Id = Guid.NewGuid(),
                    CampaignId = Guid.NewGuid(),
                    Amount = 500000,
                    DonorName = "Nguyễn Văn A",
                    StatusName = DonationStatus.Pending.ToString()
                }
            };
            var paginatedResponse = new PaginatedResponse<FTCampaignDonationResponseDto>
            {
                Items = expectedDonations,
                TotalCount = 1,
                Page = page,
                PageSize = pageSize
            };

            _mockDonationService
                .Setup(s => s.GetPendingDonationsForManagerAsync(managerId, page, pageSize))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller.GetPendingDonationsForManager(managerId, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Pending donations retrieved successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetPendingDonationsForManager - Thành công - Trả về danh sách pending donations");
        }

        [Fact(DisplayName = "UTCID02 - GetPendingDonationsForManager - Thành công - Trả về danh sách rỗng")]
        public async Task GetPendingDonationsForManager_Success_ReturnsEmptyList()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var page = 1;
            var pageSize = 20;
            var paginatedResponse = new PaginatedResponse<FTCampaignDonationResponseDto>
            {
                Items = new List<FTCampaignDonationResponseDto>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize
            };

            _mockDonationService
                .Setup(s => s.GetPendingDonationsForManagerAsync(managerId, page, pageSize))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller.GetPendingDonationsForManager(managerId, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            _output.WriteLine("✅ PASSED - UTCID02 - GetPendingDonationsForManager - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetPendingDonationsForManager - Thất bại - Lỗi server")]
        public async Task GetPendingDonationsForManager_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var page = 1;
            var pageSize = 20;

            _mockDonationService
                .Setup(s => s.GetPendingDonationsForManagerAsync(managerId, page, pageSize))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetPendingDonationsForManager(managerId, page, pageSize);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetPendingDonationsForManager - Thất bại - Lỗi server");
        }

        #endregion

        #region ConfirmDonation Tests - POST /api/ftcampaigndonation/{donationId}/confirm

        [Fact(DisplayName = "UTCID01 - ConfirmDonation - Thành công - Xác nhận donation thành công")]
        public async Task ConfirmDonation_Success_ReturnsOk()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var request = new ConfirmDonationDto
            {
                DonationId = donationId,
                ConfirmedBy = managerId,
                Notes = "Đã xác nhận"
            };
            var donation = new FTCampaignDonation
            {
                Id = donationId,
                CampaignId = campaignId,
                DonationAmount = 500000,
                Status = DonationStatus.Pending,
                ProofImages = "https://blobstorage.com/proof1.jpg,https://blobstorage.com/proof2.jpg"
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignManagerId = managerId,
                CurrentBalance = 10000000
            };
            var confirmedDonation = new FTCampaignDonation
            {
                Id = donationId,
                CampaignId = campaignId,
                DonationAmount = 500000,
                Status = DonationStatus.Completed,
                ConfirmedBy = managerId,
                ConfirmedOn = DateTimeOffset.UtcNow,
                ConfirmationNotes = "Đã xác nhận",
                ProofImages = "https://blobstorage.com/proof1.jpg,https://blobstorage.com/proof2.jpg"
            };

            _mockDonationService
                .Setup(s => s.GetByIdAsync(donationId))
                .ReturnsAsync(donation);

            _mockDonationService
                .Setup(s => s.GetCampaignForDonationAsync(campaignId))
                .ReturnsAsync(campaign);

            _mockDonationService
                .Setup(s => s.UpdateAsync(It.IsAny<FTCampaignDonation>()))
                .ReturnsAsync(confirmedDonation);

            _mockCampaignService
                .Setup(s => s.UpdateAsync(It.IsAny<FTFundCampaign>()))
                .ReturnsAsync(campaign);

            // Act
            var result = await _controller.ConfirmDonation(donationId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Donation confirmed successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - ConfirmDonation - Thành công - Xác nhận donation thành công");
        }

        [Fact(DisplayName = "UTCID02 - ConfirmDonation - Thất bại - Donation ID mismatch")]
        public async Task ConfirmDonation_DonationIdMismatch_ReturnsBadRequest()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var request = new ConfirmDonationDto
            {
                DonationId = Guid.NewGuid(), // Different ID
                ConfirmedBy = Guid.NewGuid(),
                Notes = "Đã xác nhận"
            };

            // Act
            var result = await _controller.ConfirmDonation(donationId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Donation ID mismatch", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - ConfirmDonation - Thất bại - Donation ID mismatch");
        }

        [Fact(DisplayName = "UTCID03 - ConfirmDonation - Thất bại - Donation không tồn tại")]
        public async Task ConfirmDonation_DonationNotFound_ReturnsNotFound()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var request = new ConfirmDonationDto
            {
                DonationId = donationId,
                ConfirmedBy = Guid.NewGuid(),
                Notes = "Đã xác nhận"
            };

            _mockDonationService
                .Setup(s => s.GetByIdAsync(donationId))
                .ReturnsAsync((FTCampaignDonation?)null);

            // Act
            var result = await _controller.ConfirmDonation(donationId, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Donation not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - ConfirmDonation - Thất bại - Donation không tồn tại");
        }

        [Fact(DisplayName = "UTCID04 - ConfirmDonation - Thất bại - Không phải Campaign Manager")]
        public async Task ConfirmDonation_NotCampaignManager_ReturnsForbidden()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var request = new ConfirmDonationDto
            {
                DonationId = donationId,
                ConfirmedBy = otherUserId, // Not the manager
                Notes = "Đã xác nhận"
            };
            var donation = new FTCampaignDonation
            {
                Id = donationId,
                CampaignId = campaignId,
                DonationAmount = 500000,
                Status = DonationStatus.Pending
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignManagerId = managerId // Different manager
            };

            _mockDonationService
                .Setup(s => s.GetByIdAsync(donationId))
                .ReturnsAsync(donation);

            _mockDonationService
                .Setup(s => s.GetCampaignForDonationAsync(campaignId))
                .ReturnsAsync(campaign);

            // Act
            var result = await _controller.ConfirmDonation(donationId, request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, statusResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusResult.Value);
            Assert.Contains("Only the Campaign Manager can confirm donations", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - ConfirmDonation - Thất bại - Không phải Campaign Manager");
        }

        [Fact(DisplayName = "UTCID05 - ConfirmDonation - Thất bại - Donation đã được xác nhận")]
        public async Task ConfirmDonation_AlreadyConfirmed_ReturnsBadRequest()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var request = new ConfirmDonationDto
            {
                DonationId = donationId,
                ConfirmedBy = managerId,
                Notes = "Đã xác nhận"
            };
            var donation = new FTCampaignDonation
            {
                Id = donationId,
                CampaignId = campaignId,
                DonationAmount = 500000,
                Status = DonationStatus.Completed // Already confirmed
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignManagerId = managerId
            };

            _mockDonationService
                .Setup(s => s.GetByIdAsync(donationId))
                .ReturnsAsync(donation);

            _mockDonationService
                .Setup(s => s.GetCampaignForDonationAsync(campaignId))
                .ReturnsAsync(campaign);

            // Act
            var result = await _controller.ConfirmDonation(donationId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Donation already confirmed", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID05 - ConfirmDonation - Thất bại - Donation đã được xác nhận");
        }

        [Fact(DisplayName = "UTCID06 - ConfirmDonation - Thất bại - Thiếu proof images")]
        public async Task ConfirmDonation_MissingProofImages_ReturnsBadRequest()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var request = new ConfirmDonationDto
            {
                DonationId = donationId,
                ConfirmedBy = managerId,
                Notes = "Đã xác nhận"
            };
            var donation = new FTCampaignDonation
            {
                Id = donationId,
                CampaignId = campaignId,
                DonationAmount = 500000,
                Status = DonationStatus.Pending,
                ProofImages = null // Missing proof images
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignManagerId = managerId
            };

            _mockDonationService
                .Setup(s => s.GetByIdAsync(donationId))
                .ReturnsAsync(donation);

            _mockDonationService
                .Setup(s => s.GetCampaignForDonationAsync(campaignId))
                .ReturnsAsync(campaign);

            // Act
            var result = await _controller.ConfirmDonation(donationId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Proof images are required", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID06 - ConfirmDonation - Thất bại - Thiếu proof images");
        }

        #endregion
    }
}

