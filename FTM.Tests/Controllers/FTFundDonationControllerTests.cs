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
using ConfirmDonationRequest = FTM.API.Controllers.ConfirmDonationRequest;

namespace FTM.Tests.Controllers
{
    public class FTFundDonationControllerTests
    {
        private readonly Mock<IFTFundDonationService> _mockDonationService;
        private readonly Mock<IBlobStorageService> _mockBlobStorageService;
        private readonly Mock<IFTMemberService> _mockMemberService;
        private readonly Mock<ILogger<FTFundDonationController>> _mockLogger;
        private readonly FTFundDonationController _controller;
        private readonly ITestOutputHelper _output;

        public FTFundDonationControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockDonationService = new Mock<IFTFundDonationService>();
            _mockBlobStorageService = new Mock<IBlobStorageService>();
            _mockMemberService = new Mock<IFTMemberService>();
            _mockLogger = new Mock<ILogger<FTFundDonationController>>();
            _controller = new FTFundDonationController(
                _mockDonationService.Object,
                _mockLogger.Object,
                _mockBlobStorageService.Object,
                _mockMemberService.Object);
        }

        #region GetPendingDonations Tests - GET /api/donations/pending

        [Fact(DisplayName = "UTCID01 - GetPendingDonations - Thành công - Trả về danh sách pending donations")]
        public async Task GetPendingDonations_Success_ReturnsDonations()
        {
            // Arrange
            var fundId = Guid.NewGuid();
            var page = 1;
            var pageSize = 20;
            var expectedDonations = new List<FTFundDonation>
            {
                new FTFundDonation
                {
                    Id = Guid.NewGuid(),
                    FTFundId = fundId,
                    DonationMoney = 500000,
                    DonorName = "Nguyễn Văn A",
                    PaymentMethod = PaymentMethod.Cash,
                    Status = DonationStatus.Pending
                }
            };
            var paginatedResponse = new PaginatedResponse<FTFundDonation>
            {
                Items = expectedDonations,
                TotalCount = 1,
                Page = page,
                PageSize = pageSize
            };

            _mockDonationService
                .Setup(s => s.GetPendingDonationsAsync(fundId, page, pageSize))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller.GetPendingDonations(fundId, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Pending donations retrieved successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - GetPendingDonations - Thành công - Trả về danh sách pending donations");
        }

        [Fact(DisplayName = "UTCID02 - GetPendingDonations - Thành công - Trả về danh sách rỗng")]
        public async Task GetPendingDonations_Success_ReturnsEmptyList()
        {
            // Arrange
            var fundId = (Guid?)null;
            var page = 1;
            var pageSize = 20;
            var paginatedResponse = new PaginatedResponse<FTFundDonation>
            {
                Items = new List<FTFundDonation>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize
            };

            _mockDonationService
                .Setup(s => s.GetPendingDonationsAsync(fundId, page, pageSize))
                .ReturnsAsync(paginatedResponse);

            // Act
            var result = await _controller.GetPendingDonations(fundId, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);

            _output.WriteLine("✅ PASSED - UTCID02 - GetPendingDonations - Thành công - Trả về danh sách rỗng");
        }

        [Fact(DisplayName = "UTCID03 - GetPendingDonations - Thất bại - Lỗi server")]
        public async Task GetPendingDonations_ServerError_ReturnsServerError()
        {
            // Arrange
            var fundId = (Guid?)null;
            var page = 1;
            var pageSize = 20;

            _mockDonationService
                .Setup(s => s.GetPendingDonationsAsync(fundId, page, pageSize))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetPendingDonations(fundId, page, pageSize);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusResult.Value);
            Assert.Contains("Error retrieving pending donations", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - GetPendingDonations - Thất bại - Lỗi server");
        }

        #endregion

        #region UploadProofImages Tests - POST /api/donations/{donationId}/upload-proof

        [Fact(DisplayName = "UTCID01 - UploadProofImages - Thành công - Upload proof images")]
        public async Task UploadProofImages_Success_ReturnsOk()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("proof1.jpg");
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.ContentType).Returns("image/jpeg");
            mockFile.Setup(f => f.OpenReadStream()).Returns(new MemoryStream());

            var images = new List<IFormFile> { mockFile.Object };

            var donation = new FTFundDonation
            {
                Id = donationId,
                FTFundId = Guid.NewGuid(),
                DonationMoney = 500000,
                Status = DonationStatus.Pending
            };

            _mockDonationService
                .Setup(s => s.GetByIdAsync(donationId))
                .ReturnsAsync(donation);

            _mockBlobStorageService
                .Setup(s => s.UploadFileAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync("https://blobstorage.com/donations/proof1.jpg");

            _mockDonationService
                .Setup(s => s.UpdateDonationAsync(It.IsAny<FTFundDonation>()))
                .ReturnsAsync(donation);

            // Act
            var result = await _controller.UploadProofImages(donationId, images);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Proof images uploaded successfully", apiSuccess.Message);
            Assert.NotNull(apiSuccess.Data);

            _output.WriteLine("✅ PASSED - UTCID01 - UploadProofImages - Thành công - Upload proof images");
        }

        [Fact(DisplayName = "UTCID02 - UploadProofImages - Thất bại - Donation không tồn tại")]
        public async Task UploadProofImages_DonationNotFound_ReturnsNotFound()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var mockFile = new Mock<IFormFile>();
            var images = new List<IFormFile> { mockFile.Object };

            _mockDonationService
                .Setup(s => s.GetByIdAsync(donationId))
                .ReturnsAsync((FTFundDonation?)null);

            // Act
            var result = await _controller.UploadProofImages(donationId, images);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Donation not found", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - UploadProofImages - Thất bại - Donation không tồn tại");
        }

        [Fact(DisplayName = "UTCID03 - UploadProofImages - Thất bại - Donation không ở trạng thái Pending")]
        public async Task UploadProofImages_DonationNotPending_ReturnsBadRequest()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var mockFile = new Mock<IFormFile>();
            var images = new List<IFormFile> { mockFile.Object };

            var donation = new FTFundDonation
            {
                Id = donationId,
                FTFundId = Guid.NewGuid(),
                DonationMoney = 500000,
                Status = DonationStatus.Confirmed // Not pending
            };

            _mockDonationService
                .Setup(s => s.GetByIdAsync(donationId))
                .ReturnsAsync(donation);

            // Act
            var result = await _controller.UploadProofImages(donationId, images);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Can only upload proof for pending donations", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - UploadProofImages - Thất bại - Donation không ở trạng thái Pending");
        }

        [Fact(DisplayName = "UTCID04 - UploadProofImages - Thất bại - Không có images")]
        public async Task UploadProofImages_NoImages_ReturnsBadRequest()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var images = new List<IFormFile>(); // Empty list

            var donation = new FTFundDonation
            {
                Id = donationId,
                FTFundId = Guid.NewGuid(),
                DonationMoney = 500000,
                Status = DonationStatus.Pending
            };

            _mockDonationService
                .Setup(s => s.GetByIdAsync(donationId))
                .ReturnsAsync(donation);

            // Act
            var result = await _controller.UploadProofImages(donationId, images);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("No images provided", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - UploadProofImages - Thất bại - Không có images");
        }

        #endregion

        #region ConfirmDonation Tests - POST /api/donations/{donationId}/confirm

        [Fact(DisplayName = "UTCID01 - ConfirmDonation - Thành công - Xác nhận donation thành công")]
        public async Task ConfirmDonation_Success_ReturnsOk()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var request = new ConfirmDonationRequest
            {
                Notes = "Đã xác nhận tiền mặt"
            };
            var memberDto = new FTMemberDetailsDto
            {
                Id = memberId,
                FTId = ftId,
                Fullname = "Nguyễn Văn A"
            };
            var confirmedDonation = new FTFundDonation
            {
                Id = donationId,
                FTFundId = Guid.NewGuid(),
                DonationMoney = 500000,
                Status = DonationStatus.Confirmed,
                ConfirmedBy = memberId,
                ConfirmedOn = DateTimeOffset.UtcNow,
                ConfirmationNotes = "Đã xác nhận tiền mặt"
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

            _mockDonationService
                .Setup(s => s.ConfirmDonationAsync(donationId, memberId, request.Notes))
                .ReturnsAsync(confirmedDonation);

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

        [Fact(DisplayName = "UTCID02 - ConfirmDonation - Thất bại - Member không tồn tại trong family tree")]
        public async Task ConfirmDonation_MemberNotFound_ReturnsBadRequest()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var request = new ConfirmDonationRequest
            {
                Notes = "Đã xác nhận"
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
            var result = await _controller.ConfirmDonation(donationId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Member not found in this family tree", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - ConfirmDonation - Thất bại - Member không tồn tại trong family tree");
        }

        [Fact(DisplayName = "UTCID03 - ConfirmDonation - Thất bại - Donation không ở trạng thái Pending")]
        public async Task ConfirmDonation_DonationNotPending_ReturnsBadRequest()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var request = new ConfirmDonationRequest
            {
                Notes = "Đã xác nhận"
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

            _mockDonationService
                .Setup(s => s.ConfirmDonationAsync(donationId, memberId, request.Notes))
                .ThrowsAsync(new InvalidOperationException("Can only confirm pending donations"));

            // Act
            var result = await _controller.ConfirmDonation(donationId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Contains("Can only confirm pending donations", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - ConfirmDonation - Thất bại - Donation không ở trạng thái Pending");
        }

        [Fact(DisplayName = "UTCID04 - ConfirmDonation - Thất bại - Lỗi server")]
        public async Task ConfirmDonation_ServerError_ReturnsServerError()
        {
            // Arrange
            var donationId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ftId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var request = new ConfirmDonationRequest
            {
                Notes = "Đã xác nhận"
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

            _mockDonationService
                .Setup(s => s.ConfirmDonationAsync(donationId, memberId, request.Notes))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.ConfirmDonation(donationId, request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
            var apiError = Assert.IsType<ApiError>(statusResult.Value);
            Assert.Contains("Error confirming donation", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - ConfirmDonation - Thất bại - Lỗi server");
        }

        #endregion
    }
}

