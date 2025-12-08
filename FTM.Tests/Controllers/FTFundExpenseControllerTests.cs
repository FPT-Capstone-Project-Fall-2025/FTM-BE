using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.Funds;
using FTM.Domain.Enums;
using FTM.Domain.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;
using Xunit.Abstractions;
using CreateExpenseRequest = FTM.API.Controllers.CreateExpenseRequest;
using ApproveExpenseRequest = FTM.API.Controllers.ApproveExpenseRequest;

namespace FTM.Tests.Controllers
{
    public class FTFundExpenseControllerTests
    {
        private readonly Mock<IFTFundExpenseService> _mockExpenseService;
        private readonly Mock<IBlobStorageService> _mockBlobStorageService;
        private readonly Mock<IFTMemberService> _mockMemberService;
        private readonly Mock<ILogger<FTFundExpenseController>> _mockLogger;
        private readonly FTFundExpenseController _controller;
        private readonly ITestOutputHelper _output;

        public FTFundExpenseControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockExpenseService = new Mock<IFTFundExpenseService>();
            _mockBlobStorageService = new Mock<IBlobStorageService>();
            _mockMemberService = new Mock<IFTMemberService>();
            _mockLogger = new Mock<ILogger<FTFundExpenseController>>();
            _controller = new FTFundExpenseController(
                _mockExpenseService.Object,
                _mockLogger.Object,
                _mockBlobStorageService.Object,
                _mockMemberService.Object);
        }

        #region CreateExpense Tests - POST /api/expenses

        [Fact(DisplayName = "UTCID01 - CreateExpense - Thành công - Tạo expense với receipt images")]
        public async Task CreateExpense_Success_WithReceiptImages_ReturnsOk()
        {
            // Arrange
            var fundId = Guid.NewGuid();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("receipt1.jpg");
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var request = new CreateExpenseRequest
            {
                FundId = fundId,
                Amount = 1000000,
                Description = "Chi tiêu cho sự kiện",
                Event = "Lễ hội gia đình",
                Recipient = "Nhà hàng ABC",
                ReceiptImages = new List<IFormFile> { mockFile.Object }
            };
            var expectedExpense = new FTFundExpense
            {
                Id = Guid.NewGuid(),
                FTFundId = fundId,
                ExpenseAmount = 1000000,
                ExpenseDescription = "Chi tiêu cho sự kiện",
                ExpenseEvent = "Lễ hội gia đình",
                Recipient = "Nhà hàng ABC",
                Status = TransactionStatus.Pending
            };

            _mockBlobStorageService
                .Setup(s => s.UploadFileAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync("https://blobstorage.com/expenses/receipt1.jpg");

            _mockExpenseService
                .Setup(s => s.CreateExpenseAsync(It.IsAny<FTFundExpense>()))
                .ReturnsAsync(expectedExpense);

            // Act
            var result = await _controller.CreateExpense(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Expense created successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - CreateExpense - Thành công - Tạo expense với receipt images");
        }

        [Fact(DisplayName = "UTCID02 - CreateExpense - Thành công - Tạo expense không có receipt images")]
        public async Task CreateExpense_Success_WithoutReceiptImages_ReturnsOk()
        {
            // Arrange
            var fundId = Guid.NewGuid();
            var request = new CreateExpenseRequest
            {
                FundId = fundId,
                Amount = 500000,
                Description = "Chi tiêu tạm thời",
                ReceiptImages = null
            };
            var expectedExpense = new FTFundExpense
            {
                Id = Guid.NewGuid(),
                FTFundId = fundId,
                ExpenseAmount = 500000,
                ExpenseDescription = "Chi tiêu tạm thời",
                Status = TransactionStatus.Pending
            };

            _mockExpenseService
                .Setup(s => s.CreateExpenseAsync(It.IsAny<FTFundExpense>()))
                .ReturnsAsync(expectedExpense);

            // Act
            var result = await _controller.CreateExpense(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Expense created successfully", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - CreateExpense - Thành công - Tạo expense không có receipt images");
        }

        [Fact(DisplayName = "UTCID03 - CreateExpense - Thất bại - Lỗi server")]
        public async Task CreateExpense_ServerError_ReturnsServerError()
        {
            // Arrange
            var fundId = Guid.NewGuid();
            var request = new CreateExpenseRequest
            {
                FundId = fundId,
                Amount = 1000000,
                Description = "Chi tiêu"
            };

            _mockExpenseService
                .Setup(s => s.CreateExpenseAsync(It.IsAny<FTFundExpense>()))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.CreateExpense(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusResult.Value);
            Assert.Contains("Error creating expense", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - CreateExpense - Thất bại - Lỗi server");
        }

        #endregion

        #region GetPendingExpenses Tests - GET /api/expenses/pending

        [Fact(DisplayName = "UTCID01 - GetPendingExpenses - Thành công - Trả về danh sách pending expenses")]
        public async Task GetPendingExpenses_Success_ReturnsExpenses()
        {
            // Arrange
            var fundId = Guid.NewGuid();
            var page = 1;
            var pageSize = 20;
            var expectedExpenses = new List<FTFundExpense>
            {
                new FTFundExpense
                {
                    Id = Guid.NewGuid(),
                    FTFundId = fundId,
                    ExpenseAmount = 1000000,
                    ExpenseDescription = "Chi tiêu chờ duyệt",
                    Status = TransactionStatus.Pending
                }
            };
            var paginatedResponse = new PaginatedResponse<FTFundExpense>
            {
                Items = expectedExpenses,
                TotalCount = 1,
                Page = page,
                PageSize = pageSize
            };

            _mockExpenseService
                .Setup(s => s.GetPendingExpensesAsync(fundId, page, pageSize))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller.GetPendingExpenses(fundId, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Pending expenses retrieved successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetPendingExpenses - Thành công - Trả về danh sách pending expenses");
        }

        [Fact(DisplayName = "UTCID02 - GetPendingExpenses - Thành công - Trả về danh sách rỗng")]
        public async Task GetPendingExpenses_Success_ReturnsEmptyList()
        {
            // Arrange
            var fundId = (Guid?)null;
            var page = 1;
            var pageSize = 20;
            var paginatedResponse = new PaginatedResponse<FTFundExpense>
            {
                Items = new List<FTFundExpense>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize
            };

            _mockExpenseService
                .Setup(s => s.GetPendingExpensesAsync(fundId, page, pageSize))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller.GetPendingExpenses(fundId, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            _output.WriteLine("✅ PASSED - UTCID02 - GetPendingExpenses - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetPendingExpenses - Thất bại - Lỗi server")]
        public async Task GetPendingExpenses_ServerError_ReturnsServerError()
        {
            // Arrange
            var fundId = (Guid?)null;
            var page = 1;
            var pageSize = 20;

            _mockExpenseService
                .Setup(s => s.GetPendingExpensesAsync(fundId, page, pageSize))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetPendingExpenses(fundId, page, pageSize);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusResult.Value);
            Assert.Contains("Error retrieving pending expenses", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetPendingExpenses - Thất bại - Lỗi server");
        }

        #endregion

        #region ApproveExpense Tests - POST /api/expenses/{expenseId}/approve

        [Fact(DisplayName = "UTCID01 - ApproveExpense - Thành công - Duyệt expense với payment proof")]
        public async Task ApproveExpense_Success_WithPaymentProof_ReturnsOk()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("payment_proof.jpg");
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var request = new ApproveExpenseRequest
            {
                Notes = "Đã duyệt và thanh toán",
                PaymentProof = mockFile.Object
            };
            var memberDto = new FTMemberDetailsDto
            {
                Id = memberId,
                FTId = ftId,
                Fullname = "Nguyễn Văn A"
            };
            var approvedExpense = new FTFundExpense
            {
                Id = expenseId,
                FTFundId = Guid.NewGuid(),
                ExpenseAmount = 1000000,
                Status = TransactionStatus.Approved,
                ApprovedBy = memberId,
                ApprovedOn = DateTimeOffset.UtcNow,
                ApprovalFeedback = "Đã duyệt và thanh toán"
            };

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal,
                    Request = { Headers = { { "X-Ftid", ftId.ToString() } } }
                }
            };

            _mockMemberService
                .Setup(s => s.GetByUserId(ftId, userId))
                .ReturnsAsync(memberDto);

            _mockBlobStorageService
                .Setup(s => s.UploadFileAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync("https://blobstorage.com/expenses/payment_proof.jpg");

            _mockExpenseService
                .Setup(s => s.ApproveExpenseAsync(expenseId, memberId, request.Notes, It.IsAny<string>()))
                .ReturnsAsync(approvedExpense);

            // Act
            var result = await _controller.ApproveExpense(expenseId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Expense approved successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - ApproveExpense - Thành công - Duyệt expense với payment proof");
        }

        [Fact(DisplayName = "UTCID02 - ApproveExpense - Thành công - Duyệt expense không có payment proof")]
        public async Task ApproveExpense_Success_WithoutPaymentProof_ReturnsOk()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var request = new ApproveExpenseRequest
            {
                Notes = "Đã duyệt",
                PaymentProof = null
            };
            var memberDto = new FTMemberDetailsDto
            {
                Id = memberId,
                FTId = ftId,
                Fullname = "Nguyễn Văn A"
            };
            var approvedExpense = new FTFundExpense
            {
                Id = expenseId,
                FTFundId = Guid.NewGuid(),
                ExpenseAmount = 1000000,
                Status = TransactionStatus.Approved,
                ApprovedBy = memberId,
                ApprovedOn = DateTimeOffset.UtcNow
            };

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal,
                    Request = { Headers = { { "X-Ftid", ftId.ToString() } } }
                }
            };

            _mockMemberService
                .Setup(s => s.GetByUserId(ftId, userId))
                .ReturnsAsync(memberDto);

            _mockExpenseService
                .Setup(s => s.ApproveExpenseAsync(expenseId, memberId, request.Notes, null))
                .ReturnsAsync(approvedExpense);

            // Act
            var result = await _controller.ApproveExpense(expenseId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Expense approved successfully", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - ApproveExpense - Thành công - Duyệt expense không có payment proof");
        }

        [Fact(DisplayName = "UTCID03 - ApproveExpense - Thất bại - Member không tồn tại trong family tree")]
        public async Task ApproveExpense_MemberNotFound_ReturnsBadRequest()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var request = new ApproveExpenseRequest
            {
                Notes = "Đã duyệt"
            };

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal,
                    Request = { Headers = { { "X-Ftid", ftId.ToString() } } }
                }
            };

            _mockMemberService
                .Setup(s => s.GetByUserId(ftId, userId))
                .ReturnsAsync((FTMemberDetailsDto?)null);

            // Act
            var result = await _controller.ApproveExpense(expenseId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Member not found in this family tree", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - ApproveExpense - Thất bại - Member không tồn tại trong family tree");
        }

        [Fact(DisplayName = "UTCID04 - ApproveExpense - Thất bại - Expense không ở trạng thái Pending")]
        public async Task ApproveExpense_ExpenseNotPending_ReturnsBadRequest()
        {
            // Arrange
            var expenseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var request = new ApproveExpenseRequest
            {
                Notes = "Đã duyệt"
            };
            var memberDto = new FTMemberDetailsDto
            {
                Id = memberId,
                FTId = ftId,
                Fullname = "Nguyễn Văn A"
            };

            // Mock User claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal,
                    Request = { Headers = { { "X-Ftid", ftId.ToString() } } }
                }
            };

            _mockMemberService
                .Setup(s => s.GetByUserId(ftId, userId))
                .ReturnsAsync(memberDto);

            _mockExpenseService
                .Setup(s => s.ApproveExpenseAsync(expenseId, memberId, request.Notes, null))
                .ThrowsAsync(new InvalidOperationException("Can only approve pending expenses"));

            // Act
            var result = await _controller.ApproveExpense(expenseId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Can only approve pending expenses", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - ApproveExpense - Thất bại - Expense không ở trạng thái Pending");
        }

        #endregion
    }
}

