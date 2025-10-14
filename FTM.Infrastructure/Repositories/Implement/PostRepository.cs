using FTM.Domain.Entities.Posts;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using FTM.Infrastructure.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.Infrastructure.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) 
            : base(context, currentUserResolver)
        {
        }

        public async Task<IEnumerable<Post>> GetPostsByFamilyTreeAsync(Guid familyTreeId)
        {
            return await Context.Posts
                .Include(p => p.GPMember)
                .Include(p => p.PostAttachments)
                .Include(p => p.PostComments)
                .Include(p => p.PostReactions)
                .Where(p => p.GPId == familyTreeId && p.IsDeleted == false)
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByMemberAsync(Guid memberId)
        {
            return await Context.Posts
                .Include(p => p.GPMember)
                .Include(p => p.PostAttachments)
                .Include(p => p.PostComments)
                .Include(p => p.PostReactions)
                .Where(p => p.GPMemberId == memberId && p.IsDeleted == false)
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();
        }

        public async Task<Post> GetPostWithDetailsAsync(Guid postId)
        {
            return await Context.Posts
                .Include(p => p.GPMember)
                .Include(p => p.PostAttachments)
                .Include(p => p.PostComments)
                    .ThenInclude(c => c.GPMember)
                .Include(p => p.PostComments)
                    .ThenInclude(c => c.ChildComments)
                        .ThenInclude(cc => cc.GPMember)
                .Include(p => p.PostReactions)
                    .ThenInclude(r => r.GPMember)
                .FirstOrDefaultAsync(p => p.Id == postId && p.IsDeleted == false);
        }

        public async Task<Post> GetPostWithAttachmentsAsync(Guid postId)
        {
            return await Context.Posts
                .Include(p => p.PostAttachments)
                .FirstOrDefaultAsync(p => p.Id == postId && p.IsDeleted == false);
        }
    }
}
