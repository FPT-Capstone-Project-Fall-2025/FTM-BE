using FTM.Domain.Entities.Posts;
using FTM.Infrastructure.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories.IRepositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<IEnumerable<Post>> GetPostsByFamilyTreeAsync(Guid familyTreeId);
        Task<IEnumerable<Post>> GetPostsByMemberAsync(Guid memberId);
        Task<Post> GetPostWithDetailsAsync(Guid postId);
        Task<Post> GetPostWithAttachmentsAsync(Guid postId);
    }
}
