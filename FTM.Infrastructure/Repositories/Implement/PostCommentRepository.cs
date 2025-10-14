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
    public class PostCommentRepository : GenericRepository<PostComment>, IPostCommentRepository
    {
        public PostCommentRepository(FTMDbContext context, ICurrentUserResolver currentUserResolver) 
            : base(context, currentUserResolver)
        {
        }

        public async Task<IEnumerable<PostComment>> GetCommentsByPostAsync(Guid postId)
        {
            return await Context.PostComments
                .Include(c => c.GPMember)
                .Include(c => c.ChildComments)
                    .ThenInclude(cc => cc.GPMember)
                .Where(c => c.PostId == postId && c.IsDeleted == false && c.ParentCommentId == null)
                .OrderBy(c => c.CreatedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<PostComment>> GetCommentsByParentAsync(Guid parentCommentId)
        {
            return await Context.PostComments
                .Include(c => c.GPMember)
                .Where(c => c.ParentCommentId == parentCommentId && c.IsDeleted == false)
                .OrderBy(c => c.CreatedOn)
                .ToListAsync();
        }

        public async Task<PostComment> GetCommentWithChildrenAsync(Guid commentId)
        {
            return await Context.PostComments
                .Include(c => c.GPMember)
                .Include(c => c.ChildComments)
                    .ThenInclude(cc => cc.GPMember)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.IsDeleted == false);
        }
    }
}
