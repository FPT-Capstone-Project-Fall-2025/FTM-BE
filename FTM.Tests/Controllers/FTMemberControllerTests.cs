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

        #region Add Tests - POST /api/ftmember/{ftId}

        [Fact(DisplayName = "UTCID01 - Add - Thành công - Thêm thành viên mới với dữ liệu hợp lệ")]
        public async Task Add_Success_ReturnsOk()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpsertFTMemberRequest 
            { 
                Fullname = "Nguyễn Văn A",
                Gender = 1,
                IsDeath = false,
                CategoryCode = 0,
                FTId = ftId
            };
            var expectedMember = new FTMemberDetailsDto 
            { 
                Id = Guid.NewGuid(), 
                Fullname = "Nguyễn Văn A",
                FTId = ftId
            };

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
            Assert.Equal(200, okResult.StatusCode);

            _output.WriteLine("✅ PASSED - UTCID01 - Add - Thành công - Thêm thành viên mới với dữ liệu hợp lệ");
        }

        [Fact(DisplayName = "UTCID02 - Add - Thất bại - Dữ liệu không hợp lệ (ModelState Invalid)")]
        public async Task Add_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpsertFTMemberRequest 
            { 
                Fullname = null, // Required field is null
                Gender = null, // Required field is null
            };
            _controller.ModelState.AddModelError("Fullname", "Fullname is required");
            _controller.ModelState.AddModelError("Gender", "Gender is required");

            // Act
            var result = await _controller.Add(ftId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(apiError.Message);
            Assert.Contains("Fullname is required", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - Add - Thất bại - Dữ liệu không hợp lệ (ModelState Invalid)");
        }

        [Fact(DisplayName = "UTCID03 - Add - Thất bại - Không tìm thấy thành viên được thêm mối quan hệ")]
        public async Task Add_RootMemberNotFound_ThrowsArgumentException()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpsertFTMemberRequest 
            { 
                Fullname = "Nguyễn Văn A",
                Gender = 1,
                IsDeath = false,
                CategoryCode = 1,
                FTId = ftId,
                RootId = Guid.NewGuid() // Root member that doesn't exist
            };

            _mockMemberService
                .Setup(s => s.Add(ftId, request))
                .ThrowsAsync(new ArgumentException("Không tìm thấy thành viên được thêm mối quan hệ."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.Add(ftId, request));
            Assert.Contains("Không tìm thấy thành viên được thêm mối quan hệ", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - Add - Thất bại - Không tìm thấy thành viên được thêm mối quan hệ");
        }

        [Fact(DisplayName = "UTCID04 - Add - Thất bại - Người được mời không tồn tại trong hệ thống")]
        public async Task Add_UserNotFound_ThrowsArgumentException()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpsertFTMemberRequest 
            { 
                Fullname = "Nguyễn Văn A",
                Gender = 1,
                IsDeath = false,
                CategoryCode = 0,
                FTId = ftId,
                UserId = Guid.NewGuid() // User that doesn't exist
            };

            _mockMemberService
                .Setup(s => s.Add(ftId, request))
                .ThrowsAsync(new ArgumentException("Người được mời không tồn tại trong hệ thống."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.Add(ftId, request));
            Assert.Contains("Người được mời không tồn tại trong hệ thống", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - Add - Thất bại - Người được mời không tồn tại trong hệ thống");
        }

        [Fact(DisplayName = "UTCID05 - Add - Thất bại - Người dùng đã được liên kết với thành viên khác")]
        public async Task Add_UserAlreadyConnected_ThrowsArgumentException()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpsertFTMemberRequest 
            { 
                Fullname = "Nguyễn Văn A",
                Gender = 1,
                IsDeath = false,
                CategoryCode = 0,
                FTId = ftId,
                UserId = Guid.NewGuid() // User already connected
            };

            _mockMemberService
                .Setup(s => s.Add(ftId, request))
                .ThrowsAsync(new ArgumentException("Nguời dùng đã được liên kết với một thành viên khác trong gia phả"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.Add(ftId, request));
            Assert.Contains("Nguời dùng đã được liên kết với một thành viên khác trong gia phả", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID05 - Add - Thất bại - Người dùng đã được liên kết với thành viên khác");
        }

        // #endregion

        // #region GetListOfMembers Tests - GET /api/ftmember/list

        // [Fact(DisplayName = "UTCID01 - GetListOfMembers - Thành công - Trả về danh sách thành viên với dữ liệu")]
        // public async Task GetListOfMembers_Success_ReturnsMembers()
        // {
        //     // Arrange
        //     var requestParams = new SearchWithPaginationRequest 
        //     { 
        //         PageIndex = 1, 
        //         PageSize = 10,
        //         Search = "",
        //         PropertyFilters = "",
        //         OrderBy = ""
        //     };
        //     var expectedMembers = new List<FTMemberSimpleDto>
        //     {
        //         new FTMemberSimpleDto { Id = Guid.NewGuid(), Fullname = "Nguyễn Văn A" },
        //         new FTMemberSimpleDto { Id = Guid.NewGuid(), Fullname = "Nguyễn Văn B" }
        //     };

        //     _mockMemberService
        //         .Setup(s => s.GetListOfMembers(It.IsAny<FTMemberSpecParams>()))
        //         .ReturnsAsync(expectedMembers);

        //     _mockMemberService
        //         .Setup(s => s.CountMembers(It.IsAny<FTMemberSpecParams>()))
        //         .ReturnsAsync(2);

        //     // Act
        //     var result = await _controller.GetListOfMembers(requestParams);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result);
        //     var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
        //     Assert.Equal(200, okResult.StatusCode);
        //     Assert.Equal("Lấy danh sách thành viên của gia phả thành công", apiSuccess.Message);
        //     var pagination = Assert.IsType<Pagination<FTMemberSimpleDto>>(apiSuccess.Data);
        //     Assert.Equal(2, pagination.TotalItems);
        //     Assert.Equal(2, pagination.Data.Count);

        //     _output.WriteLine("✅ PASSED - UTCID01 - GetListOfMembers - Thành công - Trả về danh sách thành viên với dữ liệu");
        // }

        // [Fact(DisplayName = "UTCID02 - GetListOfMembers - Thành công - Trả về danh sách rỗng")]
        // public async Task GetListOfMembers_Success_ReturnsEmptyList()
        // {
        //     // Arrange
        //     var requestParams = new SearchWithPaginationRequest 
        //     { 
        //         PageIndex = 1, 
        //         PageSize = 10,
        //         Search = "NonExistentMember",
        //         PropertyFilters = "",
        //         OrderBy = ""
        //     };
        //     var emptyMembers = new List<FTMemberSimpleDto>();

        //     _mockMemberService
        //         .Setup(s => s.GetListOfMembers(It.IsAny<FTMemberSpecParams>()))
        //         .ReturnsAsync(emptyMembers);

        //     _mockMemberService
        //         .Setup(s => s.CountMembers(It.IsAny<FTMemberSpecParams>()))
        //         .ReturnsAsync(0);

        //     // Act
        //     var result = await _controller.GetListOfMembers(requestParams);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result);
        //     var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
        //     Assert.Equal(200, okResult.StatusCode);
        //     Assert.Equal("Lấy danh sách thành viên của gia phả thành công", apiSuccess.Message);
        //     var pagination = Assert.IsType<Pagination<FTMemberSimpleDto>>(apiSuccess.Data);
        //     Assert.Equal(0, pagination.TotalItems);
        //     Assert.Equal(0, pagination.Data.Count);

        //     _output.WriteLine("✅ PASSED - UTCID02 - GetListOfMembers - Thành công - Trả về danh sách rỗng");
        // }

        // [Fact(DisplayName = "UTCID03 - GetListOfMembers - Thành công - Trả về danh sách với phân trang")]
        // public async Task GetListOfMembers_Success_ReturnsPaginatedList()
        // {
        //     // Arrange
        //     var requestParams = new SearchWithPaginationRequest 
        //     { 
        //         PageIndex = 2, 
        //         PageSize = 5,
        //         Search = "",
        //         PropertyFilters = "",
        //         OrderBy = "fullname"
        //     };
        //     var expectedMembers = new List<FTMemberSimpleDto>
        //     {
        //         new FTMemberSimpleDto { Id = Guid.NewGuid(), Fullname = "Member 6" },
        //         new FTMemberSimpleDto { Id = Guid.NewGuid(), Fullname = "Member 7" }
        //     };

        //     _mockMemberService
        //         .Setup(s => s.GetListOfMembers(It.IsAny<FTMemberSpecParams>()))
        //         .ReturnsAsync(expectedMembers);

        //     _mockMemberService
        //         .Setup(s => s.CountMembers(It.IsAny<FTMemberSpecParams>()))
        //         .ReturnsAsync(12); // Total 12 items

        //     // Act
        //     var result = await _controller.GetListOfMembers(requestParams);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result);
        //     var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
        //     Assert.Equal(200, okResult.StatusCode);
        //     var pagination = Assert.IsType<Pagination<FTMemberSimpleDto>>(apiSuccess.Data);
        //     Assert.Equal(2, pagination.PageIndex);
        //     Assert.Equal(5, pagination.PageSize);
        //     Assert.Equal(12, pagination.TotalItems);
        //     Assert.Equal(3, pagination.TotalPages); // Ceiling(12/5) = 3

        //     _output.WriteLine("✅ PASSED - UTCID03 - GetListOfMembers - Thành công - Trả về danh sách với phân trang");
        // }

        [Fact(DisplayName = "UTCID04 - GetListOfMembers - Thất bại - Lỗi từ service")]
        public async Task GetListOfMembers_ServiceError_ThrowsException()
        {
            // Arrange
            var requestParams = new SearchWithPaginationRequest 
            { 
                PageIndex = 1, 
                PageSize = 10
            };

            _mockMemberService
                .Setup(s => s.GetListOfMembers(It.IsAny<FTMemberSpecParams>()))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.GetListOfMembers(requestParams));
            Assert.Contains("Database connection error", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - GetListOfMembers - Thất bại - Lỗi từ service");
        }

        #endregion

        #region GetDetailedMemberOfFamilyTreeByUserId Tests - GET /api/ftmember/{ftid}/get-by-userid

        [Fact(DisplayName = "UTCID01 - GetDetailedMemberOfFamilyTreeByUserId - Thành công - Trả về chi tiết thành viên theo UserId")]
        public async Task GetDetailedMemberOfFamilyTreeByUserId_Success_ReturnsMember()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var expectedMember = new FTMemberDetailsDto 
            { 
                Id = Guid.NewGuid(), 
                Fullname = "Nguyễn Văn A",
                UserId = userId,
                FTId = ftId,
                Gender = 1
            };

            _mockMemberService
                .Setup(s => s.GetByUserId(ftId, userId))
                .ReturnsAsync(expectedMember);

            // Act
            var result = await _controller.GetDetailedMemberOfFamilyTreeByUserId(ftId, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedMember, apiSuccess.Data);
            Assert.Equal("Lấy thông tin của thành viên gia phả thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - UTCID01 - GetDetailedMemberOfFamilyTreeByUserId - Thành công - Trả về chi tiết thành viên theo UserId");
        }

        [Fact(DisplayName = "UTCID02 - GetDetailedMemberOfFamilyTreeByUserId - Thất bại - Không tìm thấy thành viên")]
        public async Task GetDetailedMemberOfFamilyTreeByUserId_NotFound_ThrowsArgumentException()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockMemberService
                .Setup(s => s.GetByUserId(ftId, userId))
                .ThrowsAsync(new ArgumentException("Không tìm thấy thành viên gia phả."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _controller.GetDetailedMemberOfFamilyTreeByUserId(ftId, userId));
            Assert.Contains("Không tìm thấy thành viên gia phả", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - GetDetailedMemberOfFamilyTreeByUserId - Thất bại - Không tìm thấy thành viên");
        }

        #endregion

        #region GetDetailedMemberOfFamilyTreeByMemberId Tests - GET /api/ftmember/{ftid}/get-by-memberid

        [Fact(DisplayName = "UTCID01 - GetDetailedMemberOfFamilyTreeByMemberId - Thành công - Trả về chi tiết thành viên theo MemberId")]
        public async Task GetDetailedMemberOfFamilyTreeByMemberId_Success_ReturnsMember()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var expectedMember = new FTMemberDetailsDto 
            { 
                Id = memberId, 
                Fullname = "Nguyễn Văn B",
                FTId = ftId,
                Gender = 1,
                Birthday = DateTime.Now.AddYears(-25)
            };

            _mockMemberService
                .Setup(s => s.GetByMemberId(ftId, memberId))
                .ReturnsAsync(expectedMember);

            // Act
            var result = await _controller.GetDetailedMemberOfFamilyTreeByMemberId(ftId, memberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedMember, apiSuccess.Data);
            Assert.Equal("Lấy thông tin của thành viên gia phả thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - UTCID01 - GetDetailedMemberOfFamilyTreeByMemberId - Thành công - Trả về chi tiết thành viên theo MemberId");
        }

        [Fact(DisplayName = "UTCID02 - GetDetailedMemberOfFamilyTreeByMemberId - Thất bại - Không tìm thấy thành viên")]
        public async Task GetDetailedMemberOfFamilyTreeByMemberId_NotFound_ThrowsArgumentException()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var memberId = Guid.NewGuid();

            _mockMemberService
                .Setup(s => s.GetByMemberId(ftId, memberId))
                .ThrowsAsync(new ArgumentException("Không tìm thấy thành viên gia phả."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _controller.GetDetailedMemberOfFamilyTreeByMemberId(ftId, memberId));
            Assert.Contains("Không tìm thấy thành viên gia phả", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - GetDetailedMemberOfFamilyTreeByMemberId - Thất bại - Không tìm thấy thành viên");
        }

        #endregion

        #region GetMembersTreeViewAsync Tests - GET /api/ftmember/member-tree

        [Fact(DisplayName = "UTCID01 - GetMembersTreeViewAsync - Thành công - Trả về cây gia phả có dữ liệu")]
        public async Task GetMembersTreeViewAsync_Success_ReturnsTree()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var rootId = Guid.NewGuid();
            var expectedTree = new FTMemberTreeDto 
            { 
                Root = rootId, 
                Datalist = new List<KeyValueModel>
                {
                    new KeyValueModel { Key = rootId, Value = new { Id = rootId, Fullname = "Nguyễn Văn A" } }
                }
            };

            _mockMemberService
                .Setup(s => s.GetMembersTree(ftId))
                .ReturnsAsync(expectedTree);

            // Act
            var result = await _controller.GetMembersTreeViewAsync(ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedTree, apiSuccess.Data);
            Assert.Equal("Lấy cây gia phả thành công", apiSuccess.Message);
            var tree = Assert.IsType<FTMemberTreeDto>(apiSuccess.Data);
            Assert.NotNull(tree.Root);
            Assert.NotEmpty(tree.Datalist);

            _output.WriteLine("✅ PASSED - UTCID01 - GetMembersTreeViewAsync - Thành công - Trả về cây gia phả có dữ liệu");
        }

        [Fact(DisplayName = "UTCID02 - GetMembersTreeViewAsync - Thành công - Trả về cây gia phả rỗng")]
        public async Task GetMembersTreeViewAsync_Success_ReturnsEmptyTree()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var expectedTree = new FTMemberTreeDto 
            { 
                Root = null, 
                Datalist = new List<KeyValueModel>()
            };

            _mockMemberService
                .Setup(s => s.GetMembersTree(ftId))
                .ReturnsAsync(expectedTree);

            // Act
            var result = await _controller.GetMembersTreeViewAsync(ftId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Lấy cây gia phả thành công", apiSuccess.Message);
            var tree = Assert.IsType<FTMemberTreeDto>(apiSuccess.Data);
            Assert.Null(tree.Root);
            Assert.Empty(tree.Datalist);

            _output.WriteLine("✅ PASSED - UTCID02 - GetMembersTreeViewAsync - Thành công - Trả về cây gia phả rỗng");
        }

        #endregion

        #region UpdateMemberDetails Tests - PUT /api/ftmember/{ftId}

        [Fact(DisplayName = "UTCID01 - UpdateMemberDetails - Thành công - Cập nhật thông tin thành viên với dữ liệu hợp lệ")]
        public async Task UpdateMemberDetails_Success_ReturnsOk()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var request = new UpdateFTMemberRequest 
            { 
                ftMemberId = memberId,
                Fullname = "Nguyễn Văn B Updated",
                Gender = 1,
                Birthday = DateTime.Now.AddYears(-30)
            };
            var expectedMember = new FTMemberDetailsDto 
            { 
                Id = memberId, 
                Fullname = "Nguyễn Văn B Updated",
                FTId = ftId
            };

            _mockMemberService
                .Setup(s => s.UpdateDetailsByMemberId(ftId, request))
                .ReturnsAsync(expectedMember);

            // Act
            var result = await _controller.UpdateMemberDetails(ftId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedMember, apiSuccess.Data);
            Assert.Equal("Cập nhật thông tin thành viên thành công", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - UTCID01 - UpdateMemberDetails - Thành công - Cập nhật thông tin thành viên với dữ liệu hợp lệ");
        }

        [Fact(DisplayName = "UTCID02 - UpdateMemberDetails - Thất bại - Dữ liệu không hợp lệ (ModelState Invalid)")]
        public async Task UpdateMemberDetails_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpdateFTMemberRequest 
            { 
                ftMemberId = Guid.Empty // Invalid: Empty GUID for required field
            };
            _controller.ModelState.AddModelError("ftMemberId", "ftMemberId is required and cannot be empty");

            // Act
            var result = await _controller.UpdateMemberDetails(ftId, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(apiError.Message);
            Assert.Contains("ftMemberId is required and cannot be empty", apiError.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - UpdateMemberDetails - Thất bại - Dữ liệu không hợp lệ (ModelState Invalid)");
        }

        [Fact(DisplayName = "UTCID03 - UpdateMemberDetails - Thất bại - Thành viên không tồn tại trong gia phả")]
        public async Task UpdateMemberDetails_MemberNotFound_ThrowsArgumentException()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpdateFTMemberRequest 
            { 
                ftMemberId = Guid.NewGuid() // Member that doesn't exist
            };

            _mockMemberService
                .Setup(s => s.UpdateDetailsByMemberId(ftId, request))
                .ThrowsAsync(new ArgumentException("Thành viên không tồn tại trong gia phả"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.UpdateMemberDetails(ftId, request));
            Assert.Contains("Thành viên không tồn tại trong gia phả", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - UpdateMemberDetails - Thất bại - Thành viên không tồn tại trong gia phả");
        }

        [Fact(DisplayName = "UTCID04 - UpdateMemberDetails - Thất bại - Người được mời không tồn tại trong hệ thống")]
        public async Task UpdateMemberDetails_UserNotFound_ThrowsArgumentException()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpdateFTMemberRequest 
            { 
                ftMemberId = Guid.NewGuid(),
                UserId = Guid.NewGuid() // User that doesn't exist
            };

            _mockMemberService
                .Setup(s => s.UpdateDetailsByMemberId(ftId, request))
                .ThrowsAsync(new ArgumentException("Người được mời không tồn tại trong hệ thống."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.UpdateMemberDetails(ftId, request));
            Assert.Contains("Người được mời không tồn tại trong hệ thống", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - UpdateMemberDetails - Thất bại - Người được mời không tồn tại trong hệ thống");
        }

        [Fact(DisplayName = "UTCID05 - UpdateMemberDetails - Thất bại - Người dùng đã được liên kết với thành viên khác")]
        public async Task UpdateMemberDetails_UserAlreadyConnected_ThrowsArgumentException()
        {
            // Arrange
            var ftId = Guid.NewGuid();
            var request = new UpdateFTMemberRequest 
            { 
                ftMemberId = Guid.NewGuid(),
                UserId = Guid.NewGuid() // User already connected to another member
            };

            _mockMemberService
                .Setup(s => s.UpdateDetailsByMemberId(ftId, request))
                .ThrowsAsync(new ArgumentException("Nguời dùng đã được liên kết với một thành viên khác trong gia phả"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _controller.UpdateMemberDetails(ftId, request));
            Assert.Contains("Nguời dùng đã được liên kết với một thành viên khác trong gia phả", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID05 - UpdateMemberDetails - Thất bại - Người dùng đã được liên kết với thành viên khác");
        }

        #endregion

        #region DeleteMember Tests - DELETE /api/ftmember/{ftMemberId}

        [Fact(DisplayName = "UTCID01 - DeleteMember - Thành công - Xóa thành viên")]
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
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Success", apiSuccess.Message);

            _output.WriteLine("✅ PASSED - UTCID01 - DeleteMember - Thành công - Xóa thành viên");
        }

        [Fact(DisplayName = "UTCID02 - DeleteMember - Thất bại - Không tìm thấy thành viên")]
        public async Task DeleteMember_NotFound_ThrowsArgumentException()
        {
            // Arrange
            var memberId = Guid.NewGuid();

            _mockMemberService
                .Setup(s => s.Delete(memberId))
                .ThrowsAsync(new ArgumentException("Không tìm thấy thành viên trong cây gia phả."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _controller.DeleteMember(memberId));
            Assert.Contains("Không tìm thấy thành viên trong cây gia phả", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - DeleteMember - Thất bại - Không tìm thấy thành viên");
        }

        [Fact(DisplayName = "UTCID03 - DeleteMember - Thất bại - Thành viên có nhiều con")]
        public async Task DeleteMember_HasManyChildren_ThrowsArgumentException()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var memberName = "Nguyễn Văn A";

            _mockMemberService
                .Setup(s => s.Delete(memberId))
                .ThrowsAsync(new ArgumentException($"Thành viên {memberName} có nhiều con trong cây gia phả, nên không thể xóa."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _controller.DeleteMember(memberId));
            Assert.Contains("có nhiều con trong cây gia phả", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID03 - DeleteMember - Thất bại - Thành viên có nhiều con");
        }

        [Fact(DisplayName = "UTCID04 - DeleteMember - Thất bại - Thành viên có mối quan hệ vợ/chồng")]
        public async Task DeleteMember_HasPartnerRelationship_ThrowsArgumentException()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var memberName = "Nguyễn Văn B";

            _mockMemberService
                .Setup(s => s.Delete(memberId))
                .ThrowsAsync(new ArgumentException(
                    $"Không thể xóa thành viên {memberName} vì họ vẫn còn mối quan hệ vợ/chồng(Người thật) trong gia phả. " +
                    $"Vui lòng xóa hoặc vô hiệu hóa mối quan hệ đó trước."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _controller.DeleteMember(memberId));
            Assert.Contains("vẫn còn mối quan hệ vợ/chồng", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID04 - DeleteMember - Thất bại - Thành viên có mối quan hệ vợ/chồng");
        }

        [Fact(DisplayName = "UTCID05 - DeleteMember - Thất bại - Thành viên vừa là cha/mẹ vừa là con")]
        public async Task DeleteMember_IsBothParentAndChild_ThrowsArgumentException()
        {
            // Arrange
            var memberId = Guid.NewGuid();
            var memberName = "Nguyễn Văn C";

            _mockMemberService
                .Setup(s => s.Delete(memberId))
                .ThrowsAsync(new ArgumentException(
                    $"Không thể xóa thành viên {memberName} vì họ vừa là mối quan hệ cha/mẹ và con trong gia phả. " +
                    $"Vui lòng xóa hoặc vô hiệu hóa các mối quan hệ đó trước."));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _controller.DeleteMember(memberId));
            Assert.Contains("vừa là mối quan hệ cha/mẹ và con", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID05 - DeleteMember - Thất bại - Thành viên vừa là cha/mẹ vừa là con");
        }

        #endregion

        #region GetRelationship Tests - GET /api/ftmember/{ftMemberId}/relationship

        [Fact(DisplayName = "UTCID01 - GetRelationship - Thành công - Trả về mối quan hệ của thành viên")]
        public async Task GetRelationship_Success_ReturnsRelationship()
        {
            // Arrange
            var ftMemberId = Guid.NewGuid();
            var expectedRelationship = new MemberRelationshipDto
            {
                HasFather = true,
                HasMother = true,
                HasSiblings = false,
                HasPartner = true,
                HasChildren = true
            };

            _mockMemberService
                .Setup(s => s.CheckRelationship(ftMemberId))
                .ReturnsAsync(expectedRelationship);

            // Act
            var result = await _controller.GetRelationship(ftMemberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(expectedRelationship, apiSuccess.Data);
            Assert.Equal("lấy mối quan hệ của thành viên trong gia phả thành công", apiSuccess.Message);
            var relationship = Assert.IsType<MemberRelationshipDto>(apiSuccess.Data);
            Assert.True(relationship.HasFather);
            Assert.True(relationship.HasMother);
            Assert.True(relationship.HasPartner);
            Assert.True(relationship.HasChildren);

            _output.WriteLine("✅ PASSED - UTCID01 - GetRelationship - Thành công - Trả về mối quan hệ của thành viên");
        }

        [Fact(DisplayName = "UTCID02 - GetRelationship - Thất bại - Không tìm thấy thành viên")]
        public async Task GetRelationship_NotFound_ThrowsArgumentException()
        {
            // Arrange
            var ftMemberId = Guid.NewGuid();

            _mockMemberService
                .Setup(s => s.CheckRelationship(ftMemberId))
                .ThrowsAsync(new ArgumentException("Thành viên không tồn tại"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _controller.GetRelationship(ftMemberId));
            Assert.Contains("Thành viên không tồn tại", exception.Message);

            _output.WriteLine("✅ PASSED - UTCID02 - GetRelationship - Thất bại - Không tìm thấy thành viên");
        }

        [Fact(DisplayName = "UTCID03 - GetRelationship - Thành công - Trả về mối quan hệ không có cha mẹ")]
        public async Task GetRelationship_Success_ReturnsRelationshipWithoutParents()
        {
            // Arrange
            var ftMemberId = Guid.NewGuid();
            var expectedRelationship = new MemberRelationshipDto
            {
                HasFather = false,
                HasMother = false,
                HasSiblings = true,
                HasPartner = false,
                HasChildren = false
            };

            _mockMemberService
                .Setup(s => s.CheckRelationship(ftMemberId))
                .ReturnsAsync(expectedRelationship);

            // Act
            var result = await _controller.GetRelationship(ftMemberId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(200, okResult.StatusCode);
            var relationship = Assert.IsType<MemberRelationshipDto>(apiSuccess.Data);
            Assert.False(relationship.HasFather);
            Assert.False(relationship.HasMother);
            Assert.True(relationship.HasSiblings);
            Assert.False(relationship.HasPartner);
            Assert.False(relationship.HasChildren);

            _output.WriteLine("✅ PASSED - UTCID03 - GetRelationship - Thành công - Trả về mối quan hệ không có cha mẹ");
        }

        #endregion
    }
}
