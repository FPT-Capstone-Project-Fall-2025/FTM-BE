using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FTM.Tests.Controllers
{
    public class FTCampaignControllerTests
    {
        private readonly Mock<IFTCampaignService> _mockCampaignService;
        private readonly Mock<ILogger<FTCampaignController>> _mockLogger;
        private readonly FTCampaignController _controller;
        private readonly ITestOutputHelper _output;

        public FTCampaignControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockCampaignService = new Mock<IFTCampaignService>();
            _mockLogger = new Mock<ILogger<FTCampaignController>>();
            _controller = new FTCampaignController(
                _mockCampaignService.Object,
                _mockLogger.Object);
        }

        #region CreateCampaign Tests - POST /api/ftcampaign

        [Fact(DisplayName = "UTCID01 - CreateCampaign - Thành công - Tạo campaign mới với dữ liệu hợp lệ")]
        public async Task CreateCampaign_Success_ReturnsOk()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var request = new CreateCampaignRequest
            {
                FamilyTreeId = ftId,
                CampaignName = "Chiến dịch từ thiện 2024",
                CampaignDescription = "Quyên góp ủng hộ đồng bào miền Trung",
                CampaignManagerId = managerId,
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddDays(30),
                FundGoal = 50000000,
                BankAccountNumber = "1234567890",
                BankCode = "970436",
                BankName = "Vietcombank",
                AccountHolderName = "Nguyễn Văn A"
            };
            var expectedCampaign = new FTFundCampaign
            {
                Id = Guid.NewGuid(),
                FTId = ftId,
                CampaignName = "Chiến dịch từ thiện 2024",
                CampaignDescription = "Quyên góp ủng hộ đồng bào miền Trung",
                CampaignManagerId = managerId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                FundGoal = 50000000,
                CurrentBalance = 0,
                Status = CampaignStatus.Active
            };

            _mockCampaignService
                .Setup(s => s.AddAsync(It.IsAny<FTFundCampaign>()))
                .ReturnsAsync(expectedCampaign);

            // Act
            var result = await _controller.CreateCampaign(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Campaign created successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - CreateCampaign - Thành công - Tạo campaign mới với dữ liệu hợp lệ");
        }

        [Fact(DisplayName = "UTCID02 - CreateCampaign - Thất bại - Request body là null")]
        public async Task CreateCampaign_NullRequest_ReturnsBadRequest()
        {
            // Arrange
            CreateCampaignRequest? request = null;

            // Act
            var result = await _controller.CreateCampaign(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Request body is required", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - CreateCampaign - Thất bại - Request body là null");
        }

        [Fact(DisplayName = "UTCID03 - CreateCampaign - Thất bại - Thiếu Campaign Manager ID")]
        public async Task CreateCampaign_MissingManagerId_ReturnsBadRequest()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new CreateCampaignRequest
            {
                FamilyTreeId = ftId,
                CampaignName = "Chiến dịch từ thiện",
                CampaignManagerId = null // Missing manager ID
            };

            // Act
            var result = await _controller.CreateCampaign(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Campaign manager ID is required", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - CreateCampaign - Thất bại - Thiếu Campaign Manager ID");
        }

        [Fact(DisplayName = "UTCID04 - CreateCampaign - Thất bại - Thiếu Family Tree ID")]
        public async Task CreateCampaign_MissingFamilyTreeId_ReturnsBadRequest()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var request = new CreateCampaignRequest
            {
                FamilyTreeId = Guid.Empty, // Empty GUID
                CampaignName = "Chiến dịch từ thiện",
                CampaignManagerId = managerId
            };

            // Act
            var result = await _controller.CreateCampaign(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Family tree ID is required", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - CreateCampaign - Thất bại - Thiếu Family Tree ID");
        }

        [Fact(DisplayName = "UTCID05 - CreateCampaign - Thất bại - Lỗi server")]
        public async Task CreateCampaign_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var request = new CreateCampaignRequest
            {
                FamilyTreeId = ftId,
                CampaignName = "Chiến dịch từ thiện",
                CampaignManagerId = managerId,
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddDays(30),
                FundGoal = 50000000
            };

            _mockCampaignService
                .Setup(s => s.AddAsync(It.IsAny<FTFundCampaign>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.CreateCampaign(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Error creating campaign", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID05 - CreateCampaign - Thất bại - Lỗi server");
        }

        #endregion

        #region GetCampaignById Tests - GET /api/ftcampaign/{id}

        [Fact(DisplayName = "UTCID01 - GetCampaignById - Thành công - Trả về campaign theo ID")]
        public async Task GetCampaignById_Success_ReturnsCampaign()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var expectedCampaign = new FTFundCampaign
            {
                Id = campaignId,
                FTId = ftId,
                CampaignName = "Chiến dịch từ thiện 2024",
                CampaignDescription = "Quyên góp ủng hộ",
                CampaignManagerId = managerId,
                StartDate = DateTimeOffset.Now,
                EndDate = DateTimeOffset.Now.AddDays(30),
                FundGoal = 50000000,
                CurrentBalance = 10000000,
                Status = CampaignStatus.Active,
                Donations = new List<FTCampaignDonation>(),
                Expenses = new List<FTCampaignExpense>()
            };

            _mockCampaignService
                .Setup(s => s.GetByIdAsync(campaignId))
                .ReturnsAsync(expectedCampaign);

            // Act
            var result = await _controller.GetCampaignById(campaignId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetCampaignById - Thành công - Trả về campaign theo ID");
        }

        [Fact(DisplayName = "UTCID02 - GetCampaignById - Thất bại - Campaign không tồn tại")]
        public async Task GetCampaignById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var campaignId = Guid.NewGuid();

            _mockCampaignService
                .Setup(s => s.GetByIdAsync(campaignId))
                .ReturnsAsync((FTFundCampaign?)null);

            // Act
            var result = await _controller.GetCampaignById(campaignId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Campaign not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - GetCampaignById - Thất bại - Campaign không tồn tại");
        }

        [Fact(DisplayName = "UTCID03 - GetCampaignById - Thất bại - Lỗi server")]
        public async Task GetCampaignById_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var campaignId = Guid.NewGuid();

            _mockCampaignService
                .Setup(s => s.GetByIdAsync(campaignId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetCampaignById(campaignId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetCampaignById - Thất bại - Lỗi server");
        }

        #endregion
    }
}

