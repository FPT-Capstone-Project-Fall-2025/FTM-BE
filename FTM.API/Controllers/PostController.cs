using FTM.API.Helpers;
using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Posts;
using FTM.Domain.Specification.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        #region Post Endpoints

        /// <summary>
        /// Create a new post with file uploads
        /// </summary>
        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePostWithFiles([FromForm] CreatePostWithFilesRequest request)
        {
            try
            {
                var result = await _postService.CreatePostWithFilesAsync(request);
                return Ok(new ApiSuccess("Post created successfully with files", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Update an existing post with file uploads
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePostWithFiles(Guid id, [FromForm] UpdatePostWithFilesRequest request)
        {
            try
            {
                var result = await _postService.UpdatePostWithFilesAsync(id, request);
                return Ok(new ApiSuccess("Post updated successfully with files", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Delete a post
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            try
            {
                var result = await _postService.DeletePostAsync(id);
                if (!result)
                    return NotFound(new ApiError("Post not found"));

                return Ok(new ApiSuccess("Post deleted successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get post by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            try
            {
                var result = await _postService.GetPostByIdAsync(id);
                if (result == null)
                    return NotFound(new ApiError("Post not found"));

                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get all posts by family tree ID with pagination
        /// </summary>
        [HttpGet("family-tree/{familyTreeId}")]
        public async Task<IActionResult> GetPostsByFamilyTree(Guid familyTreeId, [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var specParams = new PostSpecParams()
                {
                    FamilyTreeId = familyTreeId,
                    Skip = (requestParams.PageIndex - 1) * requestParams.PageSize,
                    Take = requestParams.PageSize
                };

                var result = await _postService.GetPostsByFamilyTreeAsync(specParams);
                var totalItems = await _postService.CountPostsByFamilyTreeAsync(specParams);

                return Ok(new ApiSuccess("Lấy danh sách bài viết thành công", 
                    new Pagination<PostResponseDto>(requestParams.PageIndex, requestParams.PageSize, totalItems, result.ToList())));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get all posts by member ID with pagination
        /// </summary>
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetPostsByMember(Guid memberId, [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var specParams = new PostSpecParams()
                {
                    MemberId = memberId,
                    Skip = (requestParams.PageIndex - 1) * requestParams.PageSize,
                    Take = requestParams.PageSize
                };

                var result = await _postService.GetPostsByMemberAsync(specParams);
                var totalItems = await _postService.CountPostsByMemberAsync(specParams);

                return Ok(new ApiSuccess("Lấy danh sách bài viết thành công", 
                    new Pagination<PostResponseDto>(requestParams.PageIndex, requestParams.PageSize, totalItems, result.ToList())));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion

        #region Comment Endpoints

        /// <summary>
        /// Create a new comment on a post or reply to a comment
        /// </summary>
        [HttpPost("comments")]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {
            try
            {
                var result = await _postService.CreateCommentAsync(request);
                return Ok(new ApiSuccess("Comment created successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Update an existing comment
        /// </summary>
        [HttpPut("comments/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentRequest request)
        {
            try
            {
                if (id != request.Id)
                    return BadRequest(new ApiError("Comment ID mismatch"));

                var result = await _postService.UpdateCommentAsync(request);
                return Ok(new ApiSuccess("Comment updated successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Delete a comment
        /// </summary>
        [HttpDelete("comments/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            try
            {
                var result = await _postService.DeleteCommentAsync(id);
                if (!result)
                    return NotFound(new ApiError("Comment not found"));

                return Ok(new ApiSuccess("Comment deleted successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get all comments for a post (root level comments with nested replies) with pagination
        /// </summary>
        [HttpGet("{postId}/comments")]
        public async Task<IActionResult> GetCommentsByPost(Guid postId, [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var specParams = new CommentSpecParams()
                {
                    PostId = postId,
                    Skip = (requestParams.PageIndex - 1) * requestParams.PageSize,
                    Take = requestParams.PageSize
                };

                var result = await _postService.GetCommentsByPostAsync(specParams);
                var totalItems = await _postService.CountCommentsByPostAsync(specParams);

                return Ok(new ApiSuccess("Lấy danh sách bình luận thành công", 
                    new Pagination<PostCommentDto>(requestParams.PageIndex, requestParams.PageSize, totalItems, result.ToList())));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get all replies for a comment
        /// </summary>
        [HttpGet("comments/{commentId}/replies")]
        public async Task<IActionResult> GetCommentsByParent(Guid commentId)
        {
            try
            {
                var result = await _postService.GetCommentsByParentAsync(commentId);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion

        #region Reaction Endpoints

        /// <summary>
        /// Create or update a reaction on a post or comment
        /// </summary>
        [HttpPost("reactions")]
        [Authorize]
        public async Task<IActionResult> CreateOrUpdateReaction([FromBody] CreateReactionRequest request)
        {
            try
            {
                var result = await _postService.CreateOrUpdateReactionAsync(request);
                return Ok(new ApiSuccess("Reaction saved successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Delete a reaction
        /// </summary>
        [HttpDelete("reactions/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReaction(Guid id)
        {
            try
            {
                var result = await _postService.DeleteReactionAsync(id);
                if (!result)
                    return NotFound(new ApiError("Reaction not found"));

                return Ok(new ApiSuccess("Reaction removed successfully", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get all reactions for a post with pagination
        /// </summary>
        [HttpGet("{postId}/reactions")]
        public async Task<IActionResult> GetReactionsByPost(Guid postId, [FromQuery] SearchWithPaginationRequest requestParams)
        {
            try
            {
                var specParams = new ReactionSpecParams()
                {
                    PostId = postId,
                    Skip = (requestParams.PageIndex - 1) * requestParams.PageSize,
                    Take = requestParams.PageSize
                };

                var result = await _postService.GetReactionsByPostAsync(specParams);
                var totalItems = await _postService.CountReactionsByPostAsync(specParams);

                return Ok(new ApiSuccess("Lấy danh sách reactions thành công", 
                    new Pagination<PostReactionDto>(requestParams.PageIndex, requestParams.PageSize, totalItems, result.ToList())));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get reactions summary for a post
        /// </summary>
        [HttpGet("{postId}/reactions/summary")]
        public async Task<IActionResult> GetReactionsSummaryForPost(Guid postId)
        {
            try
            {
                var result = await _postService.GetReactionsSummaryForPostAsync(postId);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        #endregion
    }
}