using FTM.Domain.Entities.Posts;
using FTM.Domain.Specification.Posts;
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

        public async Task<IEnumerable<PostComment>> GetCommentsAsync(CommentSpecParams specParams)
        {
            var query = Context.PostComments
                .Include(c => c.FTMember)
                .Include(c => c.ChildComments)
                    .ThenInclude(cc => cc.FTMember)
                .Where(c => c.IsDeleted == false && c.ParentCommentId == null);

            // Filter by PostId if provided
            if (specParams.PostId.HasValue)
            {
                query = query.Where(c => c.PostId == specParams.PostId.Value);
            }

            // Apply ordering
            query = query.OrderBy(c => c.CreatedOn);

            // Apply pagination
            return await query
                .Skip(specParams.Skip)
                .Take(specParams.Take)
                .ToListAsync();
        }

        public async Task<int> CountCommentsAsync(CommentSpecParams specParams)
        {
            var query = Context.PostComments
                .Where(c => c.IsDeleted == false && c.ParentCommentId == null);

            // Filter by PostId if provided
            if (specParams.PostId.HasValue)
            {
                query = query.Where(c => c.PostId == specParams.PostId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<PostComment>> GetCommentsByParentAsync(Guid parentCommentId)
        {
            return await Context.PostComments
                .Include(c => c.FTMember)
                .Where(c => c.ParentCommentId == parentCommentId && c.IsDeleted == false)
                .OrderBy(c => c.CreatedOn)
                .ToListAsync();
        }

        public async Task<PostComment> GetCommentWithChildrenAsync(Guid commentId)
        {
            return await Context.PostComments
                .Include(c => c.FTMember)
                .Include(c => c.ChildComments)
                    .ThenInclude(cc => cc.FTMember)
                .FirstOrDefaultAsync(c => c.Id == commentId && c.IsDeleted == false);
        }
    }
}

