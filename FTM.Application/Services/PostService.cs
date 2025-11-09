using FTM.Application.IServices;
using FTM.Domain.DTOs.Posts;
using FTM.Domain.Entities.Posts;
using FTM.Domain.Enums;
using FTM.Domain.Specification.Posts;
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
                FTId = request.FTId,
                Title = request.Title,
                Content = request.Content,
                FTMemberId = request.FTMemberId,
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

            // Automatically get current user's FTMemberId based on the post's family tree
            var currentUserFTMemberId = await GetCurrentUserFTMemberId(post.FTId);
            
            return await MapToPostResponseDto(post, currentUserFTMemberId);
        }

        public async Task<IEnumerable<PostResponseDto>> GetPostsByFamilyTreeAsync(PostSpecParams specParams)
        {
            var posts = await _postRepository.GetPostsAsync(specParams);
            var result = new List<PostResponseDto>();

            // Get current user's FTMemberId once for all posts (they're from same family tree)
            Guid? currentUserFTMemberId = null;
            if (specParams.FamilyTreeId.HasValue)
            {
                currentUserFTMemberId = await GetCurrentUserFTMemberId(specParams.FamilyTreeId.Value);
            }

            foreach (var post in posts)
            {
                result.Add(await MapToPostResponseDto(post, currentUserFTMemberId));
            }

            return result;
        }

        public async Task<int> CountPostsByFamilyTreeAsync(PostSpecParams specParams)
        {
            return await _postRepository.CountPostsAsync(specParams);
        }

        public async Task<IEnumerable<PostResponseDto>> GetPostsByMemberAsync(PostSpecParams specParams)
        {
            var posts = await _postRepository.GetPostsAsync(specParams);
            var result = new List<PostResponseDto>();

            // Get current user's FTMemberId - try to get from first post's family tree
            Guid? currentUserFTMemberId = null;
            var firstPost = posts.FirstOrDefault();
            if (firstPost != null)
            {
                currentUserFTMemberId = await GetCurrentUserFTMemberId(firstPost.FTId);
            }

            foreach (var post in posts)
            {
                result.Add(await MapToPostResponseDto(post, currentUserFTMemberId));
            }

            return result;
        }

        public async Task<int> CountPostsByMemberAsync(PostSpecParams specParams)
        {
            return await _postRepository.CountPostsAsync(specParams);
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
                FTMemberId = request.FTMemberId,
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

        public async Task<IEnumerable<PostCommentDto>> GetCommentsByPostAsync(CommentSpecParams specParams)
        {
            var comments = await _commentRepository.GetCommentsAsync(specParams);
            return comments.Select(c => MapToPostCommentDto(c)).ToList();
        }

        public async Task<int> CountCommentsByPostAsync(CommentSpecParams specParams)
        {
            return await _commentRepository.CountCommentsAsync(specParams);
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
                request.FTMemberId, request.PostId);

            if (existingReaction != null)
            {
                // Update existing reaction (restore if deleted)
                existingReaction.ReactionType = request.ReactionType;
                existingReaction.IsDeleted = false; // Restore if was deleted
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
                    FTMemberId = request.FTMemberId,
                    ReactionType = request.ReactionType,
                    CreatedOn = DateTimeOffset.UtcNow,
                    IsDeleted = false
                };
                await _reactionRepository.AddAsync(existingReaction);
            }

            await _unitOfWork.CompleteAsync();

            // Return the reaction with member info - query again to include FTMember
            var reaction = await _context.PostReactions
                .Include(r => r.FTMember)
                .FirstOrDefaultAsync(r => r.Id == existingReaction.Id);
            
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

        public async Task<IEnumerable<PostReactionDto>> GetReactionsByPostAsync(ReactionSpecParams specParams)
        {
            var reactions = await _reactionRepository.GetReactionsAsync(specParams);
            return reactions.Select(r => MapToPostReactionDto(r)).ToList();
        }

        public async Task<int> CountReactionsByPostAsync(ReactionSpecParams specParams)
        {
            return await _reactionRepository.CountReactionsAsync(specParams);
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

        private async Task<PostResponseDto> MapToPostResponseDto(Post post, Guid? currentUserFTMemberId = null)
        {
            var reactionsSummary = await GetReactionsSummaryForPostAsync(post.Id);
            
            // Get current user's reaction if currentUserFTMemberId is provided
            PostReactionDto? currentUserReaction = null;
            if (currentUserFTMemberId.HasValue && currentUserFTMemberId.Value != Guid.Empty)
            {
                var userReaction = await _reactionRepository.GetReactionByMemberAsync(
                    currentUserFTMemberId.Value, post.Id);
                
                if (userReaction != null && userReaction.IsDeleted == false)
                {
                    currentUserReaction = MapToPostReactionDto(userReaction);
                }
            }

            return new PostResponseDto
            {
                Id = post.Id,
                FTId = post.FTId,
                Title = post.Title,
                Content = post.Content,
                FTMemberId = post.FTMemberId,
                AuthorName = post.FTMember?.Fullname ?? "Unknown",
                AuthorPicture = post.FTMember?.Picture ?? "",
                Status = post.Status,
                ApprovedAt = post.ApprovedAt,
                ApprovedBy = post.ApprovedBy,
                CreatedOn = post.CreatedOn,
                LastModifiedOn = post.LastModifiedOn,
                TotalComments = post.PostComments?.Count(c => c.IsDeleted == false) ?? 0,
                TotalReactions = post.PostReactions?.Count(r => r.IsDeleted == false) ?? 0,
                ReactionsSummary = reactionsSummary,
                CurrentUserReaction = currentUserReaction,
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
                FTMemberId = comment.FTMemberId,
                AuthorName = comment.FTMember?.Fullname ?? "Unknown",
                AuthorPicture = comment.FTMember?.Picture ?? "",
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
                FTMemberId = reaction.FTMemberId,
                AuthorName = reaction.FTMember?.Fullname ?? "Unknown",
                AuthorPicture = reaction.FTMember?.Picture ?? "",
                ReactionType = reaction.ReactionType,
                CreatedOn = reaction.CreatedOn,
                HasReacted = true // Always true since this reaction exists
            };
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private async Task<Guid?> GetCurrentUserFTMemberId(Guid familyTreeId)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return null;

            // Query FTMember by UserId and FamilyTreeId
            var member = await _context.FTMembers
                .FirstOrDefaultAsync(m => m.UserId == userId && m.FTId == familyTreeId && m.IsDeleted == false);

            return member?.Id;
        }

        #endregion
    }
}


