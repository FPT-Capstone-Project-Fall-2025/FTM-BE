using FTM.API.Reponses;
using FTM.Application.IServices;
using FTM.Domain.DTOs.Posts;
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
        /// Get all posts by family tree ID
        /// </summary>
        [HttpGet("family-tree/{familyTreeId}")]
        public async Task<IActionResult> GetPostsByFamilyTree(Guid familyTreeId)
        {
            try
            {
                var result = await _postService.GetPostsByFamilyTreeAsync(familyTreeId);
                return Ok(new ApiSuccess(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Get all posts by member ID
        /// </summary>
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetPostsByMember(Guid memberId)
        {
            try
            {
                var result = await _postService.GetPostsByMemberAsync(memberId);
                return Ok(new ApiSuccess(result));
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
        /// Get all comments for a post (root level comments with nested replies)
        /// </summary>
        [HttpGet("{postId}/comments")]
        public async Task<IActionResult> GetCommentsByPost(Guid postId)
        {
            try
            {
                var result = await _postService.GetCommentsByPostAsync(postId);
                return Ok(new ApiSuccess(result));
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
        /// Get all reactions for a post
        /// </summary>
        [HttpGet("{postId}/reactions")]
        public async Task<IActionResult> GetReactionsByPost(Guid postId)
        {
            try
            {
                var result = await _postService.GetReactionsByPostAsync(postId);
                return Ok(new ApiSuccess(result));
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