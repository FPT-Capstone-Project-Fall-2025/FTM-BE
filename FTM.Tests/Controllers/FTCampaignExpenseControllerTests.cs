using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Funds;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;
using ApproveExpenseDto = FTM.API.Controllers.ApproveExpenseDto;

namespace FTM.Tests.Controllers
{
    public class FTCampaignExpenseControllerTests
    {
        private readonly Mock<IFTCampaignExpenseService> _mockExpenseService;
        private readonly Mock<IBlobStorageService> _mockBlobStorageService;
        private readonly Mock<IFTCampaignService> _mockCampaignService;
        private readonly FTCampaignExpenseController _controller;
        private readonly ITestOutputHelper _output;

        public FTCampaignExpenseControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockExpenseService = new Mock<IFTCampaignExpenseService>();
            _mockBlobStorageService = new Mock<IBlobStorageService>();
            _mockCampaignService = new Mock<IFTCampaignService>();
            _controller = new FTCampaignExpenseController(
                _mockExpenseService.Object,
                _mockBlobStorageService.Object,
                _mockCampaignService.Object);
        }

        #region CreateExpense Tests - POST /api/ftcampaignexpense

        [Fact(DisplayName = "UTCID01 - CreateExpense - Thành công - Tạo expense với receipt images")]
        public async Task CreateExpense_Success_WithReceiptImages_ReturnsOk()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var authorizedBy = Guid.NewGuid();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("receipt1.jpg");
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var request = new CreateExpenseDto
            {
                CampaignId = campaignId,
                Amount = 500000,
                Description = "Chi phí mua vật liệu",
                Category = ExpenseCategory.Infrastructure,
                AuthorizedBy = authorizedBy,
                ReceiptImages = new List<IFormFile> { mockFile.Object }
            };
            var createdExpense = new FTCampaignExpense
            {
                Id = Guid.NewGuid(),
                CampaignId = campaignId,
                ExpenseAmount = 500000,
                ExpenseDescription = "Chi phí mua vật liệu",
                Category = ExpenseCategory.Infrastructure,
                AuthorizedBy = authorizedBy,
                ApprovalStatus = ApprovalStatus.Pending,
                ReceiptImages = "https://blobstorage.com/campaign-expense-receipts/receipt1.jpg"
            };

            _mockBlobStorageService
                .Setup(s => s.UploadFileAsync(
                    It.IsAny<IFormFile>(),
                    "campaign-expense-receipts",
                    It.IsAny<string>()))
                .ReturnsAsync("https://blobstorage.com/campaign-expense-receipts/receipt1.jpg");

            _mockExpenseService
                .Setup(s => s.AddAsync(It.IsAny<FTCampaignExpense>()))
                .ReturnsAsync(createdExpense);

            // Act
            var result = await _controller.CreateExpense(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Expense request created with receipts, pending approval", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - CreateExpense - Thành công - Tạo expense với receipt images");
        }

        [Fact(DisplayName = "UTCID02 - CreateExpense - Thành công - Tạo expense không có receipt images")]
        public async Task CreateExpense_Success_WithoutReceiptImages_ReturnsOk()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var authorizedBy = Guid.NewGuid();
            var request = new CreateExpenseDto
            {
                CampaignId = campaignId,
                Amount = 300000,
                Description = "Chi phí vận chuyển",
                Category = ExpenseCategory.Administrative,
                AuthorizedBy = authorizedBy,
                ReceiptImages = null
            };
            var createdExpense = new FTCampaignExpense
            {
                Id = Guid.NewGuid(),
                CampaignId = campaignId,
                ExpenseAmount = 300000,
                ExpenseDescription = "Chi phí vận chuyển",
                Category = ExpenseCategory.Administrative,
                AuthorizedBy = authorizedBy,
                ApprovalStatus = ApprovalStatus.Pending
            };

            _mockExpenseService
                .Setup(s => s.AddAsync(It.IsAny<FTCampaignExpense>()))
                .ReturnsAsync(createdExpense);

            // Act
            var result = await _controller.CreateExpense(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Expense request created with receipts, pending approval", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID02 - CreateExpense - Thành công - Tạo expense không có receipt images");
        }

        [Fact(DisplayName = "UTCID03 - CreateExpense - Thất bại - Campaign không tồn tại")]
        public async Task CreateExpense_CampaignNotFound_ReturnsBadRequest()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var authorizedBy = Guid.NewGuid();
            var request = new CreateExpenseDto
            {
                CampaignId = campaignId,
                Amount = 500000,
                Description = "Chi phí mua vật liệu",
                Category = ExpenseCategory.Infrastructure,
                AuthorizedBy = authorizedBy
            };
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("receipt1.jpg");
            mockFile.Setup(f => f.Length).Returns(1024);
            request.ReceiptImages = new List<IFormFile> { mockFile.Object };

            _mockBlobStorageService
                .Setup(s => s.UploadFileAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync("https://blobstorage.com/campaign-expense-receipts/receipt1.jpg");

            _mockExpenseService
                .Setup(s => s.AddAsync(It.IsAny<FTCampaignExpense>()))
                .ThrowsAsync(new InvalidOperationException("Campaign not found"));

            // Act
            var result = await _controller.CreateExpense(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Campaign not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - CreateExpense - Thất bại - Campaign không tồn tại");
        }

        [Fact(DisplayName = "UTCID04 - CreateExpense - Thất bại - Lỗi upload file")]
        public async Task CreateExpense_UploadFileError_ReturnsBadRequest()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var authorizedBy = Guid.NewGuid();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("receipt1.jpg");
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var request = new CreateExpenseDto
            {
                CampaignId = campaignId,
                Amount = 500000,
                Description = "Chi phí mua vật liệu",
                Category = ExpenseCategory.Infrastructure,
                AuthorizedBy = authorizedBy,
                ReceiptImages = new List<IFormFile> { mockFile.Object }
            };

            _mockBlobStorageService
                .Setup(s => s.UploadFileAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new Exception("Blob storage error"));

            // Act
            var result = await _controller.CreateExpense(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Blob storage error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - CreateExpense - Thất bại - Lỗi upload file");
        }

        [Fact(DisplayName = "UTCID05 - CreateExpense - Thất bại - Lỗi server")]
        public async Task CreateExpense_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var campaignId = Guid.NewGuid();
            var authorizedBy = Guid.NewGuid();
            var request = new CreateExpenseDto
            {
                CampaignId = campaignId,
                Amount = 500000,
                Description = "Chi phí mua vật liệu",
                Category = ExpenseCategory.Infrastructure,
                AuthorizedBy = authorizedBy
            };

            _mockExpenseService
                .Setup(s => s.AddAsync(It.IsAny<FTCampaignExpense>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.CreateExpense(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Database error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID05 - CreateExpense - Thất bại - Lỗi server");
        }

        #endregion

        #region GetPendingExpensesForManager Tests - GET /api/ftcampaignexpense/pending/manager/{managerId}

        [Fact(DisplayName = "UTCID01 - GetPendingExpensesForManager - Thành công - Trả về danh sách pending expenses")]
        public async Task GetPendingExpensesForManager_Success_ReturnsExpenses()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var page = 1;
            var pageSize = 20;
            var expectedExpenses = new List<FTCampaignExpenseResponseDto>
            {
                new FTCampaignExpenseResponseDto
                {
                    Id = Guid.NewGuid(),
                    CampaignId = Guid.NewGuid(),
                    Amount = 500000,
                    Description = "Chi phí mua vật liệu",
                    StatusName = ApprovalStatus.Pending.ToString()
                }
            };
            var paginatedResponse = new PaginatedResponse<FTCampaignExpenseResponseDto>
            {
                Items = expectedExpenses,
                TotalCount = 1,
                Page = page,
                PageSize = pageSize
            };

            _mockExpenseService
                .Setup(s => s.GetPendingExpensesForManagerAsync(managerId, page, pageSize))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller.GetPendingExpensesForManager(managerId, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetPendingExpensesForManager - Thành công - Trả về danh sách pending expenses");
        }

        [Fact(DisplayName = "UTCID02 - GetPendingExpensesForManager - Thành công - Trả về danh sách rỗng")]
        public async Task GetPendingExpensesForManager_Success_ReturnsEmptyList()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var page = 1;
            var pageSize = 20;
            var paginatedResponse = new PaginatedResponse<FTCampaignExpenseResponseDto>
            {
                Items = new List<FTCampaignExpenseResponseDto>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize
            };

            _mockExpenseService
                .Setup(s => s.GetPendingExpensesForManagerAsync(managerId, page, pageSize))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller.GetPendingExpensesForManager(managerId, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID02 - GetPendingExpensesForManager - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetPendingExpensesForManager - Thất bại - Lỗi server")]
        public async Task GetPendingExpensesForManager_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var page = 1;
            var pageSize = 20;

            _mockExpenseService
                .Setup(s => s.GetPendingExpensesForManagerAsync(managerId, page, pageSize))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetPendingExpensesForManager(managerId, page, pageSize);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Server error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetPendingExpensesForManager - Thất bại - Lỗi server");
        }

        #endregion

        #region ApproveExpense Tests - PUT /api/ftcampaignexpense/{id}/approve

        [Fact(DisplayName = "UTCID01 - ApproveExpense - Thành công - Xác nhận expense với payment proof")]
        public async Task ApproveExpense_Success_WithPaymentProof_ReturnsOk()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("payment_proof.jpg");
            mockFile.Setup(f => f.Length).Returns(2048);
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var request = new ApproveExpenseDto
            {
                ApproverId = managerId,
                ApprovalNotes = "Đã chuyển khoản",
                PaymentProofImage = mockFile.Object
            };
            var expense = new FTCampaignExpense
            {
                Id = expenseId,
                CampaignId = campaignId,
                ExpenseAmount = 500000,
                ApprovalStatus = ApprovalStatus.Pending
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignManagerId = managerId,
                CurrentBalance = 10000000
            };
            var approvedExpense = new FTCampaignExpense
            {
                Id = expenseId,
                CampaignId = campaignId,
                ExpenseAmount = 500000,
                ApprovalStatus = ApprovalStatus.Approved,
                ApprovedBy = managerId,
                ApprovedOn = DateTimeOffset.UtcNow,
                ApprovalNotes = "Đã chuyển khoản\nPayment Proof: https://blobstorage.com/payment_proof.jpg"
            };

            _mockExpenseService
                .Setup(s => s.GetByIdAsync(expenseId))
                .ReturnsAsync(expense);

            _mockCampaignService
                .Setup(s => s.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            _mockBlobStorageService
                .Setup(s => s.UploadFileAsync(
                    It.IsAny<IFormFile>(),
                    "campaign-expense-payment-proofs",
                    It.IsAny<string>()))
                .ReturnsAsync("https://blobstorage.com/campaign-expense-payment-proofs/payment_proof.jpg");

            _mockExpenseService
                .Setup(s => s.ApproveExpenseAsync(expenseId, managerId, It.IsAny<string>()))
                .ReturnsAsync(approvedExpense);

            // Act
            var result = await _controller.ApproveExpense(expenseId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Expense approved successfully with payment proof", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - ApproveExpense - Thành công - Xác nhận expense với payment proof");
        }

        [Fact(DisplayName = "UTCID02 - ApproveExpense - Thành công - Xác nhận expense không có payment proof")]
        public async Task ApproveExpense_Success_WithoutPaymentProof_ReturnsOk()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var request = new ApproveExpenseDto
            {
                ApproverId = managerId,
                ApprovalNotes = "Đã chuyển khoản",
                PaymentProofImage = null
            };
            var expense = new FTCampaignExpense
            {
                Id = expenseId,
                CampaignId = campaignId,
                ExpenseAmount = 300000,
                ApprovalStatus = ApprovalStatus.Pending
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignManagerId = managerId,
                CurrentBalance = 10000000
            };
            var approvedExpense = new FTCampaignExpense
            {
                Id = expenseId,
                CampaignId = campaignId,
                ExpenseAmount = 300000,
                ApprovalStatus = ApprovalStatus.Approved,
                ApprovedBy = managerId,
                ApprovedOn = DateTimeOffset.UtcNow,
                ApprovalNotes = "Đã chuyển khoản"
            };

            _mockExpenseService
                .Setup(s => s.GetByIdAsync(expenseId))
                .ReturnsAsync(expense);

            _mockCampaignService
                .Setup(s => s.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            _mockExpenseService
                .Setup(s => s.ApproveExpenseAsync(expenseId, managerId, "Đã chuyển khoản"))
                .ReturnsAsync(approvedExpense);

            // Act
            var result = await _controller.ApproveExpense(expenseId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Expense approved successfully with payment proof", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID02 - ApproveExpense - Thành công - Xác nhận expense không có payment proof");
        }

        [Fact(DisplayName = "UTCID03 - ApproveExpense - Thất bại - Expense không tồn tại")]
        public async Task ApproveExpense_ExpenseNotFound_ReturnsNotFound()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var request = new ApproveExpenseDto
            {
                ApproverId = managerId,
                ApprovalNotes = "Đã chuyển khoản"
            };

            _mockExpenseService
                .Setup(s => s.GetByIdAsync(expenseId))
                .ReturnsAsync((FTCampaignExpense?)null);

            // Act
            var result = await _controller.ApproveExpense(expenseId, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Expense not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - ApproveExpense - Thất bại - Expense không tồn tại");
        }

        [Fact(DisplayName = "UTCID04 - ApproveExpense - Thất bại - Campaign không tồn tại")]
        public async Task ApproveExpense_CampaignNotFound_ReturnsNotFound()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var request = new ApproveExpenseDto
            {
                ApproverId = managerId,
                ApprovalNotes = "Đã chuyển khoản"
            };
            var expense = new FTCampaignExpense
            {
                Id = expenseId,
                CampaignId = campaignId,
                ExpenseAmount = 500000,
                ApprovalStatus = ApprovalStatus.Pending
            };

            _mockExpenseService
                .Setup(s => s.GetByIdAsync(expenseId))
                .ReturnsAsync(expense);

            _mockCampaignService
                .Setup(s => s.GetByIdAsync(campaignId))
                .ReturnsAsync((FTFundCampaign?)null);

            // Act
            var result = await _controller.ApproveExpense(expenseId, request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Campaign not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - ApproveExpense - Thất bại - Campaign không tồn tại");
        }

        [Fact(DisplayName = "UTCID05 - ApproveExpense - Thất bại - Không phải Campaign Manager")]
        public async Task ApproveExpense_NotCampaignManager_ReturnsForbidden()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var request = new ApproveExpenseDto
            {
                ApproverId = otherUserId, // Not the manager
                ApprovalNotes = "Đã chuyển khoản"
            };
            var expense = new FTCampaignExpense
            {
                Id = expenseId,
                CampaignId = campaignId,
                ExpenseAmount = 500000,
                ApprovalStatus = ApprovalStatus.Pending
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignManagerId = managerId // Different manager
            };

            _mockExpenseService
                .Setup(s => s.GetByIdAsync(expenseId))
                .ReturnsAsync(expense);

            _mockCampaignService
                .Setup(s => s.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            // Act
            var result = await _controller.ApproveExpense(expenseId, request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, statusResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusResult.Value);
            Assert.Contains("Only the Campaign Manager can approve expenses", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID05 - ApproveExpense - Thất bại - Không phải Campaign Manager");
        }

        [Fact(DisplayName = "UTCID06 - ApproveExpense - Thất bại - Lỗi upload payment proof")]
        public async Task ApproveExpense_UploadProofError_ReturnsBadRequest()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("payment_proof.jpg");
            mockFile.Setup(f => f.Length).Returns(2048);
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var request = new ApproveExpenseDto
            {
                ApproverId = managerId,
                ApprovalNotes = "Đã chuyển khoản",
                PaymentProofImage = mockFile.Object
            };
            var expense = new FTCampaignExpense
            {
                Id = expenseId,
                CampaignId = campaignId,
                ExpenseAmount = 500000,
                ApprovalStatus = ApprovalStatus.Pending
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignManagerId = managerId,
                CurrentBalance = 10000000
            };

            _mockExpenseService
                .Setup(s => s.GetByIdAsync(expenseId))
                .ReturnsAsync(expense);

            _mockCampaignService
                .Setup(s => s.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            _mockBlobStorageService
                .Setup(s => s.UploadFileAsync(
                    It.IsAny<IFormFile>(),
                    "campaign-expense-payment-proofs",
                    It.IsAny<string>()))
                .ThrowsAsync(new Exception("Blob storage error"));

            // Act
            var result = await _controller.ApproveExpense(expenseId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Blob storage error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID06 - ApproveExpense - Thất bại - Lỗi upload payment proof");
        }

        [Fact(DisplayName = "UTCID07 - ApproveExpense - Thất bại - Lỗi server")]
        public async Task ApproveExpense_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var campaignId = Guid.NewGuid();
            var managerId = Guid.NewGuid();
            var request = new ApproveExpenseDto
            {
                ApproverId = managerId,
                ApprovalNotes = "Đã chuyển khoản"
            };
            var expense = new FTCampaignExpense
            {
                Id = expenseId,
                CampaignId = campaignId,
                ExpenseAmount = 500000,
                ApprovalStatus = ApprovalStatus.Pending
            };
            var campaign = new FTFundCampaign
            {
                Id = campaignId,
                CampaignManagerId = managerId,
                CurrentBalance = 10000000
            };

            _mockExpenseService
                .Setup(s => s.GetByIdAsync(expenseId))
                .ReturnsAsync(expense);

            _mockCampaignService
                .Setup(s => s.GetByIdAsync(campaignId))
                .ReturnsAsync(campaign);

            _mockExpenseService
                .Setup(s => s.ApproveExpenseAsync(expenseId, managerId, It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.ApproveExpense(expenseId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Database error", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID07 - ApproveExpense - Thất bại - Lỗi server");
        }

        #endregion
    }
}

