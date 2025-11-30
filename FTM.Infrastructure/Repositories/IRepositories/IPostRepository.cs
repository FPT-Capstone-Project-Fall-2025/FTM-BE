using FTM.Domain.Entities.Posts;
using FTM.Domain.Specification.Posts;
using FTM.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<IEnumerable<Post>> GetPostsAsync(PostSpecParams specParams);
        Task<int> CountPostsAsync(PostSpecParams specParams);
        Task<Post> GetPostWithDetailsAsync(Guid postId);
        Task<Post> GetPostWithAttachmentsAsync(Guid postId);
    }
}
