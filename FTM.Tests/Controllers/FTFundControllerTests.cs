using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using FTM.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;
using CreateFundRequest = FTM.API.Controllers.CreateFundRequest;
using DonateRequest = FTM.API.Controllers.DonateRequest;

namespace FTM.Tests.Controllers
{
    public class FTFundControllerTests
    {
        private readonly Mock<IFTFundService> _mockFundService;
        private readonly Mock<IFTFundDonationService> _mockDonationService;
        private readonly Mock<IPayOSPaymentService> _mockPaymentService;
        private readonly Mock<ILogger<FTFundController>> _mockLogger;
        private readonly FTFundController _controller;
        private readonly ITestOutputHelper _output;

        public FTFundControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockFundService = new Mock<IFTFundService>();
            _mockDonationService = new Mock<IFTFundDonationService>();
            _mockPaymentService = new Mock<IPayOSPaymentService>();
            _mockLogger = new Mock<ILogger<FTFundController>>();
            _controller = new FTFundController(
                _mockFundService.Object,
                _mockDonationService.Object,
                _mockPaymentService.Object,
                _mockLogger.Object);
        }

        #region CreateFund Tests - POST /api/funds

        [Fact(DisplayName = "UTCID01 - CreateFund - Thành công - Tạo quỹ mới với dữ liệu hợp lệ")]
        public async Task CreateFund_Success_ReturnsOk()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new CreateFundRequest
            {
                FamilyTreeId = ftId,
                FundName = "Quỹ Dòng Tộc Nguyễn",
                Description = "Quỹ chung của dòng tộc",
                BankAccountNumber = "1234567890",
                BankCode = "970436",
                BankName = "Vietcombank",
                AccountHolderName = "Nguyễn Văn A"
            };
            var expectedFund = new FTFund
            {
                Id = Guid.NewGuid(),
                FTId = ftId,
                FundName = "Quỹ Dòng Tộc Nguyễn",
                FundNote = "Quỹ chung của dòng tộc",
                BankAccountNumber = "1234567890",
                BankCode = "970436",
                BankName = "Vietcombank",
                AccountHolderName = "Nguyễn Văn A"
            };

            _mockFundService
                .Setup(s => s.CreateFundAsync(It.IsAny<FTFund>()))
                .ReturnsAsync(expectedFund);

            // Act
            var result = await _controller.CreateFund(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Fund created successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - CreateFund - Thành công - Tạo quỹ mới với dữ liệu hợp lệ");
        }

        [Fact(DisplayName = "UTCID02 - CreateFund - Thất bại - Family tree không tồn tại")]
        public async Task CreateFund_FamilyTreeNotFound_ReturnsServerError()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new CreateFundRequest
            {
                FamilyTreeId = ftId,
                FundName = "Quỹ Dòng Tộc",
                Description = "Mô tả"
            };

            _mockFundService
                .Setup(s => s.CreateFundAsync(It.IsAny<FTFund>()))
                .ThrowsAsync(new Exception("Family tree not found"));

            // Act
            var result = await _controller.CreateFund(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusResult.Value);
            Assert.Contains("Error creating fund", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - CreateFund - Thất bại - Family tree không tồn tại");
        }

        [Fact(DisplayName = "UTCID03 - CreateFund - Thất bại - Thiếu thông tin bắt buộc")]
        public async Task CreateFund_MissingRequiredInfo_ReturnsServerError()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new CreateFundRequest
            {
                FamilyTreeId = ftId,
                FundName = "", // Empty name
                Description = "Mô tả"
            };

            _mockFundService
                .Setup(s => s.CreateFundAsync(It.IsAny<FTFund>()))
                .ThrowsAsync(new Exception("Fund name is required"));

            // Act
            var result = await _controller.CreateFund(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusResult.Value);
            Assert.Contains("Error creating fund", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - CreateFund - Thất bại - Thiếu thông tin bắt buộc");
        }

        #endregion

        #region GetFundsByTreeId Tests - GET /api/funds/tree/{treeId}

        [Fact(DisplayName = "UTCID01 - GetFundsByTreeId - Thành công - Trả về danh sách quỹ")]
        public async Task GetFundsByTreeId_Success_ReturnsFunds()
        {
            // Arrange
            var treeId = Guid.NewGuid();
            var expectedFunds = new List<FTFund>
            {
                new FTFund
                {
                    Id = Guid.NewGuid(),
                    FTId = treeId,
                    FundName = "Quỹ Dòng Tộc Nguyễn",
                    FundNote = "Quỹ chung",
                    CurrentMoney = 10000000,
                    BankAccountNumber = "1234567890",
                    BankCode = "970436",
                    BankName = "Vietcombank",
                    AccountHolderName = "Nguyễn Văn A",
                    Donations = new List<FTFundDonation>(),
                    Expenses = new List<FTFundExpense>()
                }
            };

            _mockFundService
                .Setup(s => s.GetFundsByTreeAsync(treeId))
                .ReturnsAsync(expectedFunds);

            // Act
            var result = await _controller.GetFundsByTreeId(treeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Funds retrieved successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetFundsByTreeId - Thành công - Trả về danh sách quỹ");
        }

        [Fact(DisplayName = "UTCID02 - GetFundsByTreeId - Thành công - Trả về danh sách rỗng")]
        public async Task GetFundsByTreeId_Success_ReturnsEmptyList()
        {
            // Arrange
            var treeId = Guid.NewGuid();
            var emptyFunds = new List<FTFund>();

            _mockFundService
                .Setup(s => s.GetFundsByTreeAsync(treeId))
                .ReturnsAsync(emptyFunds);

            // Act
            var result = await _controller.GetFundsByTreeId(treeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            _output.WriteLine("✅ PASSED - UTCID02 - GetFundsByTreeId - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetFundsByTreeId - Thất bại - Lỗi server")]
        public async Task GetFundsByTreeId_ServerError_ReturnsServerError()
        {
            // Arrange
            var treeId = Guid.NewGuid();

            _mockFundService
                .Setup(s => s.GetFundsByTreeAsync(treeId))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetFundsByTreeId(treeId);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusResult.Value);
            Assert.Contains("Error retrieving funds", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetFundsByTreeId - Thất bại - Lỗi server");
        }

        #endregion

        #region DonateTo Tests - POST /api/funds/{fundId}/donate

        [Fact(DisplayName = "UTCID01 - DonateTo - Thành công - Tạo donation với cash payment")]
        public async Task DonateTo_Success_CashPayment_ReturnsOk()
        {
            // Arrange
            var fundId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var request = new DonateRequest
            {
                MemberId = memberId,
                DonorName = "Nguyễn Văn A",
                Amount = 500000,
                PaymentMethod = PaymentMethod.Cash,
                PaymentNotes = "Tiền mặt"
            };
            var fund = new FTFund
            {
                Id = fundId,
                FundName = "Quỹ Dòng Tộc",
                BankAccountNumber = "1234567890",
                BankCode = "970436",
                BankName = "Vietcombank",
                AccountHolderName = "Nguyễn Văn B"
            };
            var expectedDonation = new FTFundDonation
            {
                Id = Guid.NewGuid(),
                FTFundId = fundId,
                FTMemberId = memberId,
                DonationMoney = 500000,
                DonorName = "Nguyễn Văn A",
                PaymentMethod = PaymentMethod.Cash,
                PaymentNotes = "Tiền mặt",
                Status = DonationStatus.Pending
            };

            _mockFundService
                .Setup(s => s.GetByIdAsync(fundId))
                .ReturnsAsync(fund);

            _mockDonationService
                .Setup(s => s.CreateDonationAsync(It.IsAny<FTFundDonation>()))
                .ReturnsAsync(expectedDonation);

            // Act
            var result = await _controller.DonateTo(fundId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Donation created successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - DonateTo - Thành công - Tạo donation với cash payment");
        }

        [Fact(DisplayName = "UTCID02 - DonateTo - Thành công - Tạo donation với bank transfer")]
        public async Task DonateTo_Success_BankTransfer_ReturnsOk()
        {
            // Arrange
            var fundId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var request = new DonateRequest
            {
                MemberId = memberId,
                DonorName = "Nguyễn Văn A",
                Amount = 1000000,
                PaymentMethod = PaymentMethod.BankTransfer,
                PaymentNotes = "Chuyển khoản"
            };
            var fund = new FTFund
            {
                Id = fundId,
                FundName = "Quỹ Dòng Tộc",
                BankAccountNumber = "1234567890",
                BankCode = "970436",
                BankName = "Vietcombank",
                AccountHolderName = "Nguyễn Văn B"
            };
            var expectedDonation = new FTFundDonation
            {
                Id = Guid.NewGuid(),
                FTFundId = fundId,
                FTMemberId = memberId,
                DonationMoney = 1000000,
                DonorName = "Nguyễn Văn A",
                PaymentMethod = PaymentMethod.BankTransfer,
                PaymentNotes = "Chuyển khoản",
                Status = DonationStatus.Pending,
                PayOSOrderCode = 123456789
            };

            _mockFundService
                .Setup(s => s.GetByIdAsync(fundId))
                .ReturnsAsync(fund);

            _mockPaymentService
                .Setup(s => s.GenerateOrderCode())
                .Returns(123456789);

            _mockPaymentService
                .Setup(s => s.GenerateVietQRUrl(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<decimal>(),
                    It.IsAny<string>()))
                .Returns("https://img.vietqr.io/image/970436-1234567890-compact2.jpg");

            _mockDonationService
                .Setup(s => s.CreateDonationAsync(It.IsAny<FTFundDonation>()))
                .ReturnsAsync(expectedDonation);

            // Act
            var result = await _controller.DonateTo(fundId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Donation created successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID02 - DonateTo - Thành công - Tạo donation với bank transfer");
        }

        [Fact(DisplayName = "UTCID03 - DonateTo - Thất bại - Fund không tồn tại")]
        public async Task DonateTo_FundNotFound_ReturnsNotFound()
        {
            // Arrange
            var fundId = Guid.NewGuid();
            var request = new DonateRequest
            {
                MemberId = Guid.NewGuid(),
                Amount = 500000,
                PaymentMethod = PaymentMethod.Cash
            };

            _mockFundService
                .Setup(s => s.GetByIdAsync(fundId))
                .ReturnsAsync((FTFund?)null);

            // Act
            var result = await _controller.DonateTo(fundId, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Fund not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - DonateTo - Thất bại - Fund không tồn tại");
        }

        [Fact(DisplayName = "UTCID04 - DonateTo - Thất bại - Fund chưa có thông tin ngân hàng")]
        public async Task DonateTo_FundMissingBankInfo_ReturnsBadRequest()
        {
            // Arrange
            var fundId = Guid.NewGuid();
            var request = new DonateRequest
            {
                MemberId = Guid.NewGuid(),
                Amount = 1000000,
                PaymentMethod = PaymentMethod.BankTransfer
            };
            var fund = new FTFund
            {
                Id = fundId,
                FundName = "Quỹ Dòng Tộc",
                BankAccountNumber = null, // Missing bank info
                BankCode = null,
                BankName = null,
                AccountHolderName = null
            };

            _mockFundService
                .Setup(s => s.GetByIdAsync(fundId))
                .ReturnsAsync(fund);

            // Act
            var result = await _controller.DonateTo(fundId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("bank account information", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - DonateTo - Thất bại - Fund chưa có thông tin ngân hàng");
        }

        #endregion
    }
}

