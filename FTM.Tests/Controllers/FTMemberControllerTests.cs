using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.API.Helpers;
using FTM.Application.IServices;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Models;
using FTM.Domain.Specification.FTMembers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FTM.Tests.Controllers
{
    /// <summary>
    /// Test các API của FTMemberController:
    /// - Add: Thêm thành viên mới
    /// - GetListOfMembers: Lấy danh sách thành viên với phân trang
    /// - GetDetailedMemberOfFamilyTreeByUserId: Lấy chi tiết thành viên theo UserId
    /// - GetDetailedMemberOfFamilyTreeByMemberId: Lấy chi tiết thành viên theo MemberId
    /// - GetMembersTreeViewAsync: Lấy cây gia phả
    /// - UpdateMemberDetails: Cập nhật thông tin thành viên
    /// - DeleteMember: Xóa thành viên
    /// </summary>
    public class FTMemberControllerTests
    {
        private readonly Mock<IFTMemberService> _mockMemberService;
        private readonly FTMemberController _controller;
        private readonly ITestOutputHelper _output;

        public FTMemberControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockMemberService = new Mock<IFTMemberService>();
            _controller = new FTMemberController(_mockMemberService.Object);
        }

        #region Add Tests

        [Fact(DisplayName = "Add - Thành công - Thêm thành viên mới")]
        public async Task Add_Success_ReturnsCreated()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpsertFTMemberRequest { Fullname = "New Member", Birthday = DateTime.Now };
            var expectedMember = new FTMemberDetailsDto { Id = Guid.NewGuid(), Fullname = "New Member" };

            _mockMemberService
                .Setup(s => s.Add(ftId, request))
                .ReturnsAsync(expectedMember);

            // Act
            var result = await _controller.Add(ftId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedMember, apiSuccess.Data);
            Assert.Equal("Tạo thành viên gia phả thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - Add - Thành công - Thêm thành viên mới");
        }

        [Fact(DisplayName = "Add - Thất bại - Dữ liệu không hợp lệ")]
        public async Task Add_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpsertFTMemberRequest(); // Invalid data
            _controller.ModelState.AddModelError("Fullname", "Required");

            // Act
            var result = await _controller.Add(ftId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Required", apiError.Message);

            _output.WriteLine("✅ PASSED - Add - Thất bại - Dữ liệu không hợp lệ");
        }

        #endregion

        #region GetListOfMembers Tests

        [Fact(DisplayName = "GetListOfMembers - Thành công - Trả về danh sách thành viên")]
        public async Task GetListOfMembers_Success_ReturnsMembers()
        {
            // Arrange
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var expectedMembers = new List<FTMemberSimpleDto>
            {
                new FTMemberSimpleDto { Id = Guid.NewGuid(), Fullname = "Member 1" },
                new FTMemberSimpleDto { Id = Guid.NewGuid(), Fullname = "Member 2" }
            };

            _mockMemberService
                .Setup(s => s.GetListOfMembers(It.IsAny<FTMemberSpecParams>()))
                .ReturnsAsync(expectedMembers);

            _mockMemberService
                .Setup(s => s.CountMembers(It.IsAny<FTMemberSpecParams>()))
                .ReturnsAsync(2);

            // Act
            var result = await _controller.GetListOfMembers(requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Lấy danh sách thành viên của gia phả thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - GetListOfMembers - Thành công - Trả về danh sách thành viên");
        }

        #endregion

        #region GetDetailedMemberOfFamilyTreeByUserId Tests

        [Fact(DisplayName = "GetDetailedMemberOfFamilyTreeByUserId - Thành công - Trả về chi tiết thành viên theo UserId")]
        public async Task GetDetailedMemberOfFamilyTreeByUserId_Success_ReturnsMember()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var expectedMember = new FTMemberDetailsDto { Id = Guid.NewGuid(), Fullname = "Test Member", UserId = userId };

            _mockMemberService
                .Setup(s => s.GetByUserId(ftId, userId))
                .ReturnsAsync(expectedMember);

            // Act
            var result = await _controller.GetDetailedMemberOfFamilyTreeByUserId(ftId, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedMember, apiSuccess.Data);
            Assert.Equal("Lấy thông tin của thành viên gia phả thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - GetDetailedMemberOfFamilyTreeByUserId - Thành công - Trả về chi tiết thành viên theo UserId");
        }

        #endregion

        #region GetDetailedMemberOfFamilyTreeByMemberId Tests

        [Fact(DisplayName = "GetDetailedMemberOfFamilyTreeByMemberId - Thành công - Trả về chi tiết thành viên theo MemberId")]
        public async Task GetDetailedMemberOfFamilyTreeByMemberId_Success_ReturnsMember()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var expectedMember = new FTMemberDetailsDto { Id = memberId, Fullname = "Test Member" };

            _mockMemberService
                .Setup(s => s.GetByMemberId(ftId, memberId))
                .ReturnsAsync(expectedMember);

            // Act
            var result = await _controller.GetDetailedMemberOfFamilyTreeByMemberId(ftId, memberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedMember, apiSuccess.Data);
            Assert.Equal("Lấy thông tin của thành viên gia phả thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - GetDetailedMemberOfFamilyTreeByMemberId - Thành công - Trả về chi tiết thành viên theo MemberId");
        }

        #endregion

        #region GetMembersTreeViewAsync Tests

        [Fact(DisplayName = "GetMembersTreeViewAsync - Thành công - Trả về cây gia phả")]
        public async Task GetMembersTreeViewAsync_Success_ReturnsTree()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var expectedTree = new FTMemberTreeDto { Root = Guid.NewGuid(), Datalist = new List<KeyValueModel>() };

            _mockMemberService
                .Setup(s => s.GetMembersTree(ftId))
                .ReturnsAsync(expectedTree);

            // Act
            var result = await _controller.GetMembersTreeViewAsync(ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedTree, apiSuccess.Data);
            Assert.Equal("Lấy cây gia phả thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - GetMembersTreeViewAsync - Thành công - Trả về cây gia phả");
        }

        #endregion

        #region UpdateMemberDetails Tests

        [Fact(DisplayName = "UpdateMemberDetails - Thành công - Cập nhật thông tin thành viên")]
        public async Task UpdateMemberDetails_Success_ReturnsUpdated()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpdateFTMemberRequest { Fullname = "Updated Name" };
            var expectedMember = new FTMemberDetailsDto { Id = Guid.NewGuid(), Fullname = "Updated Name" };

            _mockMemberService
                .Setup(s => s.UpdateDetailsByMemberId(ftId, request))
                .ReturnsAsync(expectedMember);

            // Act
            var result = await _controller.UpdateMemberDetails(ftId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedMember, apiSuccess.Data);
            Assert.Equal("Cập nhật thông tin thành viên thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - UpdateMemberDetails - Thành công - Cập nhật thông tin thành viên");
        }

        [Fact(DisplayName = "UpdateMemberDetails - Thất bại - Dữ liệu không hợp lệ")]
        public async Task UpdateMemberDetails_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpdateFTMemberRequest(); // Invalid data
            _controller.ModelState.AddModelError("Fullname", "Required");

            // Act
            var result = await _controller.UpdateMemberDetails(ftId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Required", apiError.Message);

            _output.WriteLine("✅ PASSED - UpdateMemberDetails - Thất bại - Dữ liệu không hợp lệ");
        }

        #endregion

        #region DeleteMember Tests

        [Fact(DisplayName = "DeleteMember - Thành công - Xóa thành viên")]
        public async Task DeleteMember_Success_ReturnsOk()
        {
            // Arrange
            var memberId = Guid.NewGuid();

            _mockMemberService
                .Setup(s => s.Delete(memberId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteMember(memberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Success", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - DeleteMember - Thành công - Xóa thành viên");
        }

        #endregion
    }
}
