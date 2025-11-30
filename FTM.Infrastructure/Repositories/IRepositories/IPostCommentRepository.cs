using FTM.Domain.Entities.Posts;
using FTM.Domain.Specification.Posts;
using FTM.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IPostCommentRepository : IGenericRepository<PostComment>
    {
        Task<IEnumerable<PostComment>> GetCommentsAsync(CommentSpecParams specParams);
        Task<int> CountCommentsAsync(CommentSpecParams specParams);
        Task<IEnumerable<PostComment>> GetCommentsByParentAsync(Guid parentCommentId);
        Task<PostComment> GetCommentWithChildrenAsync(Guid commentId);
    }
}
