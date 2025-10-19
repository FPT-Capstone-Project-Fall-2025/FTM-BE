using FTM.Domain.DTOs.Posts;
using FTM.Domain.Specification.Posts;
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
        Task<IEnumerable<PostResponseDto>> GetPostsByFamilyTreeAsync(PostSpecParams specParams);
        Task<int> CountPostsByFamilyTreeAsync(PostSpecParams specParams);
        Task<IEnumerable<PostResponseDto>> GetPostsByMemberAsync(PostSpecParams specParams);
        Task<int> CountPostsByMemberAsync(PostSpecParams specParams);
        
        // Comment operations
        Task<PostCommentDto> CreateCommentAsync(CreateCommentRequest request);
        Task<PostCommentDto> UpdateCommentAsync(UpdateCommentRequest request);
        Task<bool> DeleteCommentAsync(Guid commentId);
        Task<IEnumerable<PostCommentDto>> GetCommentsByPostAsync(CommentSpecParams specParams);
        Task<int> CountCommentsByPostAsync(CommentSpecParams specParams);
        Task<IEnumerable<PostCommentDto>> GetCommentsByParentAsync(Guid parentCommentId);
        
        // Reaction operations
        Task<PostReactionDto> CreateOrUpdateReactionAsync(CreateReactionRequest request);
        Task<bool> DeleteReactionAsync(Guid reactionId);
        Task<IEnumerable<PostReactionDto>> GetReactionsByPostAsync(ReactionSpecParams specParams);
        Task<int> CountReactionsByPostAsync(ReactionSpecParams specParams);
        Task<Dictionary<string, int>> GetReactionsSummaryForPostAsync(Guid postId);
    }
}
