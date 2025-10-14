using FTM.Application.IServices;
using FTM.Domain.DTOs.Posts;
using FTM.Domain.Entities.Posts;
using FTM.Domain.Enums;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.IRepositories;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FTM.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostCommentRepository _commentRepository;
        private readonly IPostReactionRepository _reactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobStorageService _blobStorageService;
        private readonly FTMDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(
            IPostRepository postRepository,
            IPostCommentRepository commentRepository,
            IPostReactionRepository reactionRepository,
            IUnitOfWork unitOfWork,
            IBlobStorageService blobStorageService,
            FTMDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _reactionRepository = reactionRepository;
            _unitOfWork = unitOfWork;
            _blobStorageService = blobStorageService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Post Operations

        public async Task<PostResponseDto> CreatePostWithFilesAsync(CreatePostWithFilesRequest request)
        {
            var post = new Post
            {
                Id = Guid.NewGuid(),
                GPId = request.GPId,
                Title = request.Title,
                Content = request.Content,
                GPMemberId = request.GPMemberId,
                Status = request.Status,
                CreatedOn = DateTimeOffset.UtcNow,
                IsDeleted = false
            };

            // Upload files to Blob Storage and create attachments
            if (request.Files != null && request.Files.Any())
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    var file = request.Files[i];
                    var caption = i < request.Captions?.Count ? request.Captions[i] : string.Empty;
                    var fileType = i < request.FileTypes?.Count ? request.FileTypes[i] : 1; // Default: Image

                    // Upload to Blob Storage
                    var fileUrl = await _blobStorageService.UploadFileAsync(file, "posts", null);

                    post.PostAttachments.Add(new PostAttachment
                    {
                        Id = Guid.NewGuid(),
                        PostId = post.Id,
                        FileUrl = fileUrl,
                        FileType = (PostFileType)fileType,
                        Caption = caption ?? string.Empty,
                        CreatedOn = DateTimeOffset.UtcNow,
                        IsDeleted = false
                    });
                }
            }

            await _postRepository.AddAsync(post);
            await _unitOfWork.CompleteAsync();

            return await GetPostByIdAsync(post.Id);
        }

        public async Task<PostResponseDto> UpdatePostWithFilesAsync(Guid postId, UpdatePostWithFilesRequest request)
        {
            // Load post WITHOUT attachments to avoid tracking conflicts
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                throw new Exception("Post not found");

            // Update post fields
            post.Title = request.Title;
            post.Content = request.Content;
            post.Status = request.Status;
            post.LastModifiedOn = DateTimeOffset.UtcNow;

            // Save post changes first
            _postRepository.Update(post);
            await _unitOfWork.CompleteAsync();

            // Now handle attachments in a separate context/query
            // Query existing attachments directly (not via navigation property)
            var existingAttachments = await _context.PostAttachments
                .Where(a => a.PostId == postId && a.IsDeleted == false)
                .ToListAsync();

            // Mark attachments as deleted if not in ExistingFileUrls
            foreach (var attachment in existingAttachments)
            {
                if (request.ExistingFileUrls == null || !request.ExistingFileUrls.Contains(attachment.FileUrl))
                {
                    attachment.IsDeleted = true;
                    attachment.LastModifiedOn = DateTimeOffset.UtcNow;
                }
            }
            
            if (existingAttachments.Any(a => a.IsDeleted == true))
            {
                await _unitOfWork.CompleteAsync();
            }

            // Upload and add new attachments
            if (request.Files != null && request.Files.Any())
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    var file = request.Files[i];
                    var caption = i < request.Captions?.Count ? request.Captions[i] : string.Empty;
                    var fileType = i < request.FileTypes?.Count ? request.FileTypes[i] : 1;

                    // Upload to Blob Storage
                    var fileUrl = await _blobStorageService.UploadFileAsync(file, "posts", null);

                    var newAttachment = new PostAttachment
                    {
                        Id = Guid.NewGuid(),
                        PostId = post.Id,
                        FileUrl = fileUrl,
                        FileType = (PostFileType)fileType,
                        Caption = caption ?? string.Empty,
                        CreatedOn = DateTimeOffset.UtcNow,
                        IsDeleted = false
                    };

                    _context.PostAttachments.Add(newAttachment);
                }
                await _unitOfWork.CompleteAsync();
            }

            return await GetPostByIdAsync(post.Id);
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
                return false;

            post.IsDeleted = true;
            post.LastModifiedOn = DateTimeOffset.UtcNow;

            _postRepository.Update(post);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<PostResponseDto> GetPostByIdAsync(Guid postId)
        {
            var post = await _postRepository.GetPostWithDetailsAsync(postId);
            if (post == null)
                return null;

            return await MapToPostResponseDto(post);
        }

        public async Task<IEnumerable<PostResponseDto>> GetPostsByFamilyTreeAsync(Guid familyTreeId)
        {
            var posts = await _postRepository.GetPostsByFamilyTreeAsync(familyTreeId);
            var result = new List<PostResponseDto>();

            foreach (var post in posts)
            {
                result.Add(await MapToPostResponseDto(post));
            }

            return result;
        }

        public async Task<IEnumerable<PostResponseDto>> GetPostsByMemberAsync(Guid memberId)
        {
            var posts = await _postRepository.GetPostsByMemberAsync(memberId);
            var result = new List<PostResponseDto>();

            foreach (var post in posts)
            {
                result.Add(await MapToPostResponseDto(post));
            }

            return result;
        }

        #endregion

        #region Comment Operations

        public async Task<PostCommentDto> CreateCommentAsync(CreateCommentRequest request)
        {
            // Validate Post exists
            var post = await _postRepository.GetByIdAsync(request.PostId);
            if (post == null)
                throw new Exception("Post not found");

            // Validate ParentComment exists if provided
            if (request.ParentCommentId.HasValue && request.ParentCommentId.Value != Guid.Empty)
            {
                var parentComment = await _commentRepository.GetByIdAsync(request.ParentCommentId.Value);
                if (parentComment == null)
                    throw new Exception("Parent comment not found");
            }

            var currentUserId = GetCurrentUserId();

            var comment = new PostComment
            {
                Id = Guid.NewGuid(),
                PostId = request.PostId,
                GPMemberId = request.GPMemberId,
                Content = request.Content,
                ParentCommentId = request.ParentCommentId.HasValue && request.ParentCommentId.Value != Guid.Empty 
                    ? request.ParentCommentId 
                    : null, // Set to null if empty GUID or null
                CreatedOn = DateTimeOffset.UtcNow,
                CreatedByUserId = currentUserId,
                IsDeleted = false
            };

            await _commentRepository.AddAsync(comment);
            await _unitOfWork.CompleteAsync();

            var savedComment = await _commentRepository.GetCommentWithChildrenAsync(comment.Id);
            return MapToPostCommentDto(savedComment);
        }

        public async Task<PostCommentDto> UpdateCommentAsync(UpdateCommentRequest request)
        {
            var comment = await _commentRepository.GetByIdAsync(request.Id);
            if (comment == null)
                throw new Exception("Comment not found");

            comment.Content = request.Content;
            comment.LastModifiedOn = DateTimeOffset.UtcNow;

            _commentRepository.Update(comment);
            await _unitOfWork.CompleteAsync();

            var updatedComment = await _commentRepository.GetCommentWithChildrenAsync(comment.Id);
            return MapToPostCommentDto(updatedComment);
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                return false;

            comment.IsDeleted = true;
            comment.LastModifiedOn = DateTimeOffset.UtcNow;

            _commentRepository.Update(comment);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<PostCommentDto>> GetCommentsByPostAsync(Guid postId)
        {
            var comments = await _commentRepository.GetCommentsByPostAsync(postId);
            return comments.Select(c => MapToPostCommentDto(c)).ToList();
        }

        public async Task<IEnumerable<PostCommentDto>> GetCommentsByParentAsync(Guid parentCommentId)
        {
            var comments = await _commentRepository.GetCommentsByParentAsync(parentCommentId);
            return comments.Select(c => MapToPostCommentDto(c)).ToList();
        }

        #endregion

        #region Reaction Operations

        public async Task<PostReactionDto> CreateOrUpdateReactionAsync(CreateReactionRequest request)
        {
            // Check if user already reacted
            var existingReaction = await _reactionRepository.GetReactionByMemberAsync(
                request.GPMemberId, request.PostId);

            if (existingReaction != null)
            {
                // Update existing reaction
                existingReaction.ReactionType = request.ReactionType;
                existingReaction.LastModifiedOn = DateTimeOffset.UtcNow;
                _reactionRepository.Update(existingReaction);
            }
            else
            {
                // Create new reaction
                existingReaction = new PostReaction
                {
                    Id = Guid.NewGuid(),
                    PostId = request.PostId,
                    GPMemberId = request.GPMemberId,
                    ReactionType = request.ReactionType,
                    CreatedOn = DateTimeOffset.UtcNow,
                    IsDeleted = false
                };
                await _reactionRepository.AddAsync(existingReaction);
            }

            await _unitOfWork.CompleteAsync();

            // Return the reaction with member info
            var reaction = await _reactionRepository.GetByIdAsync(existingReaction.Id);
            return MapToPostReactionDto(reaction);
        }

        public async Task<bool> DeleteReactionAsync(Guid reactionId)
        {
            var reaction = await _reactionRepository.GetByIdAsync(reactionId);
            if (reaction == null)
                return false;

            reaction.IsDeleted = true;
            reaction.LastModifiedOn = DateTimeOffset.UtcNow;

            _reactionRepository.Update(reaction);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<PostReactionDto>> GetReactionsByPostAsync(Guid postId)
        {
            var reactions = await _reactionRepository.GetReactionsByPostAsync(postId);
            return reactions.Select(r => MapToPostReactionDto(r)).ToList();
        }

        public async Task<Dictionary<string, int>> GetReactionsSummaryForPostAsync(Guid postId)
        {
            var summary = await _reactionRepository.GetReactionsSummaryForPostAsync(postId);
            return summary.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => kvp.Value
            );
        }

        #endregion

        #region Mapping Methods

        private async Task<PostResponseDto> MapToPostResponseDto(Post post)
        {
            var reactionsSummary = await GetReactionsSummaryForPostAsync(post.Id);

            return new PostResponseDto
            {
                Id = post.Id,
                GPId = post.GPId,
                Title = post.Title,
                Content = post.Content,
                GPMemberId = post.GPMemberId,
                AuthorName = post.GPMember?.Fullname ?? "Unknown",
                AuthorPicture = post.GPMember?.Picture ?? "",
                Status = post.Status,
                ApprovedAt = post.ApprovedAt,
                ApprovedBy = post.ApprovedBy,
                CreatedOn = post.CreatedOn,
                LastModifiedOn = post.LastModifiedOn,
                TotalComments = post.PostComments?.Count(c => c.IsDeleted == false) ?? 0,
                TotalReactions = post.PostReactions?.Count(r => r.IsDeleted == false) ?? 0,
                ReactionsSummary = reactionsSummary,
                Attachments = post.PostAttachments?
                    .Where(a => a.IsDeleted == false)
                    .Select(a => new PostAttachmentDto
                    {
                        Id = a.Id,
                        FileUrl = a.FileUrl,
                        FileType = (int)a.FileType,
                        Caption = a.Caption,
                        CreatedOn = a.CreatedOn
                    }).ToList() ?? new List<PostAttachmentDto>(),
                Comments = post.PostComments?
                    .Where(c => c.IsDeleted == false && c.ParentCommentId == null)
                    .Select(c => MapToPostCommentDto(c))
                    .ToList() ?? new List<PostCommentDto>()
            };
        }

        private PostCommentDto MapToPostCommentDto(PostComment comment)
        {
            if (comment == null)
                return null;

            return new PostCommentDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                GPMemberId = comment.GPMemberId,
                AuthorName = comment.GPMember?.Fullname ?? "Unknown",
                AuthorPicture = comment.GPMember?.Picture ?? "",
                Content = comment.Content,
                ParentCommentId = comment.ParentCommentId,
                CreatedOn = comment.CreatedOn,
                LastModifiedOn = comment.LastModifiedOn,
                ChildComments = comment.ChildComments?
                    .Where(c => c.IsDeleted == false)
                    .Select(c => MapToPostCommentDto(c))
                    .ToList() ?? new List<PostCommentDto>()
            };
        }

        private PostReactionDto MapToPostReactionDto(PostReaction reaction)
        {
            if (reaction == null)
                return null;

            return new PostReactionDto
            {
                Id = reaction.Id,
                PostId = reaction.PostId,
                GPMemberId = reaction.GPMemberId,
                AuthorName = reaction.GPMember?.Fullname ?? "Unknown",
                AuthorPicture = reaction.GPMember?.Picture ?? "",
                ReactionType = reaction.ReactionType,
                CreatedOn = reaction.CreatedOn
            };
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        #endregion
    }
}
