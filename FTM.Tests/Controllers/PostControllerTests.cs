using FTM.API.Controllers;
using FTM.API.Reponses;
using FTM.API.Helpers;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Posts;
using FTM.Domain.Specification.Posts;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FTM.Tests.Controllers
{
    /// <summary>
    /// Test các API: CreatePostWithFiles, UpdatePostWithFiles, GetPosts, GetPostById, DeletePost
    /// </summary>
    public class PostControllerTests
    {
        private readonly Mock<IPostService> _mockPostService;
        private readonly PostController _controller;
        private readonly ITestOutputHelper _output;

        public PostControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _mockPostService = new Mock<IPostService>();
            _controller = new PostController(_mockPostService.Object);
        }

        #region CreatePostWithFiles Tests

        [Fact(DisplayName = "CreatePostWithFiles - Thành công - Tạo bài viết với file")]
        public async Task CreatePostWithFiles_Success_ReturnsOk()
        {
            // Arrange
            var request = new CreatePostWithFilesRequest { Title = "New Post" };
            var expectedResult = new PostResponseDto { Id = Guid.NewGuid(), Title = "New Post" };

            _mockPostService
                .Setup(s => s.CreatePostWithFilesAsync(request))
                .Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _controller.CreatePostWithFiles(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedResult, apiSuccess.Data);
            Assert.Equal("Post created successfully with files", apiSuccess.Message);
        }

        [Fact(DisplayName = "CreatePostWithFiles - Thất bại - Lỗi server")]
        public async Task CreatePostWithFiles_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var request = new CreatePostWithFilesRequest { Title = "New Post" };

            _mockPostService
                .Setup(s => s.CreatePostWithFilesAsync(request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.CreatePostWithFiles(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region UpdatePostWithFiles Tests

        [Fact(DisplayName = "UpdatePostWithFiles - Thành công - Cập nhật bài viết với file")]
        public async Task UpdatePostWithFiles_Success_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdatePostWithFilesRequest { Title = "Updated Post" };
            var expectedResult = new PostResponseDto { Id = id, Title = "Updated Post" };

            _mockPostService
                .Setup(s => s.UpdatePostWithFilesAsync(id, request))
                .Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _controller.UpdatePostWithFiles(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedResult, apiSuccess.Data);
        }

        [Fact(DisplayName = "UpdatePostWithFiles - Thất bại - Bài viết không tồn tại")]
        public async Task UpdatePostWithFiles_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdatePostWithFilesRequest { Title = "Updated Post" };

            _mockPostService
                .Setup(s => s.UpdatePostWithFilesAsync(id, request))
                .Returns(Task.FromResult((PostResponseDto?)null));

            // Act
            var result = await _controller.UpdatePostWithFiles(id, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Null(apiSuccess.Data);
        }

        [Fact(DisplayName = "UpdatePostWithFiles - Thất bại - Lỗi server")]
        public async Task UpdatePostWithFiles_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();
            var request = new UpdatePostWithFilesRequest { Title = "Updated Post" };

            _mockPostService
                .Setup(s => s.UpdatePostWithFilesAsync(id, request))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.UpdatePostWithFiles(id, request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region GetPostsByFamilyTree Tests

        [Fact(DisplayName = "GetPostsByFamilyTree - Thành công - Trả về danh sách bài viết")]
        public async Task GetPostsByFamilyTree_Success_ReturnsPosts()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };
            var expectedPosts = new List<PostResponseDto> { new PostResponseDto { Id = Guid.NewGuid(), Title = "Test Post" } };

            _mockPostService
                .Setup(s => s.GetPostsByFamilyTreeAsync(It.IsAny<PostSpecParams>()))
                .Returns(Task.FromResult((IEnumerable<PostResponseDto>)expectedPosts));

            _mockPostService
                .Setup(s => s.CountPostsByFamilyTreeAsync(It.IsAny<PostSpecParams>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _controller.GetPostsByFamilyTree(familyTreeId, requestParams);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.NotNull(apiSuccess.Data);
        }

        [Fact(DisplayName = "GetPostsByFamilyTree - Thất bại - Lỗi server")]
        public async Task GetPostsByFamilyTree_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var familyTreeId = Guid.NewGuid();
            var requestParams = new SearchWithPaginationRequest { PageIndex = 1, PageSize = 10 };

            _mockPostService
                .Setup(s => s.GetPostsByFamilyTreeAsync(It.IsAny<PostSpecParams>()))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetPostsByFamilyTree(familyTreeId, requestParams);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region GetPostById Tests

        [Fact(DisplayName = "GetPostById - Thành công - Trả về bài viết theo ID")]
        public async Task GetPostById_Success_ReturnsPost()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedResult = new PostResponseDto { Id = id, Title = "Test Post" };

            _mockPostService
                .Setup(s => s.GetPostByIdAsync(id))
                .Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _controller.GetPostById(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal(expectedResult, apiSuccess.Data);
        }

        [Fact(DisplayName = "GetPostById - Thất bại - Bài viết không tồn tại")]
        public async Task GetPostById_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockPostService
                .Setup(s => s.GetPostByIdAsync(id))
                .Returns(Task.FromResult((PostResponseDto?)null));

            // Act
            var result = await _controller.GetPostById(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Post not found", apiError.Message);
        }

        [Fact(DisplayName = "GetPostById - Thất bại - Lỗi server")]
        public async Task GetPostById_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockPostService
                .Setup(s => s.GetPostByIdAsync(id))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.GetPostById(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion

        #region DeletePost Tests

        [Fact(DisplayName = "DeletePost - Thành công - Xóa bài viết")]
        public async Task DeletePost_Success_ReturnsOk()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockPostService
                .Setup(s => s.DeletePostAsync(id))
                .Returns(Task.FromResult(true));

            // Act
            var result = await _controller.DeletePost(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiSuccess = Assert.IsType<ApiSuccess>(okResult.Value);
            Assert.Equal("Post deleted successfully", apiSuccess.Message);
        }

        [Fact(DisplayName = "DeletePost - Thất bại - Bài viết không tồn tại")]
        public async Task DeletePost_NotFound_ReturnsNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockPostService
                .Setup(s => s.DeletePostAsync(id))
                .Returns(Task.FromResult(false));

            // Act
            var result = await _controller.DeletePost(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(notFoundResult.Value);
            Assert.Equal("Post not found", apiError.Message);
        }

        [Fact(DisplayName = "DeletePost - Thất bại - Lỗi server")]
        public async Task DeletePost_ServerError_ReturnsBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();

            _mockPostService
                .Setup(s => s.DeletePostAsync(id))
                .ThrowsAsync(new Exception("Server error"));

            // Act
            var result = await _controller.DeletePost(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiError = Assert.IsType<ApiError>(badRequestResult.Value);
            Assert.Contains("Server error", apiError.Message);
        }

        #endregion
    }
}
