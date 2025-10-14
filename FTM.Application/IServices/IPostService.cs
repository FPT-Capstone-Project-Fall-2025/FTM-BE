using FTM.Domain.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTM.Application.IServices
{
    public interface IPostService
    {
        // Post operations
        Task<PostResponseDto> CreatePostWithFilesAsync(CreatePostWithFilesRequest request);
        Task<PostResponseDto> UpdatePostWithFilesAsync(Guid postId, UpdatePostWithFilesRequest request);
        Task<bool> DeletePostAsync(Guid postId);
        Task<PostResponseDto> GetPostByIdAsync(Guid postId);
        Task<IEnumerable<PostResponseDto>> GetPostsByFamilyTreeAsync(Guid familyTreeId);
        Task<IEnumerable<PostResponseDto>> GetPostsByMemberAsync(Guid memberId);
        
        // Comment operations
        Task<PostCommentDto> CreateCommentAsync(CreateCommentRequest request);
        Task<PostCommentDto> UpdateCommentAsync(UpdateCommentRequest request);
        Task<bool> DeleteCommentAsync(Guid commentId);
        Task<IEnumerable<PostCommentDto>> GetCommentsByPostAsync(Guid postId);
        Task<IEnumerable<PostCommentDto>> GetCommentsByParentAsync(Guid parentCommentId);
        
        // Reaction operations
        Task<PostReactionDto> CreateOrUpdateReactionAsync(CreateReactionRequest request);
        Task<bool> DeleteReactionAsync(Guid reactionId);
        Task<IEnumerable<PostReactionDto>> GetReactionsByPostAsync(Guid postId);
        Task<Dictionary<string, int>> GetReactionsSummaryForPostAsync(Guid postId);
    }
}
