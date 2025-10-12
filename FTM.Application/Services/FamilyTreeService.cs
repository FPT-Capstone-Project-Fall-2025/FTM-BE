using FTM.Application.IServices;
using FTM.Domain.Constants;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.DTOss.FamilyTree;
using FTM.Domain.Entities.Identity;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTM.Application.Services
{
    public class FamilyTreeService : IFamilyTreeService
    {
        private readonly FTMDbContext _context;
        private readonly AppIdentityDbContext _appIdentityDbContext;
        private readonly ICurrentUserResolver _currentUserResolver;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public FamilyTreeService(
            FTMDbContext context,
            AppIdentityDbContext appIdentityDbContext,
            ICurrentUserResolver currentUserResolver,
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _appIdentityDbContext = appIdentityDbContext;
            _currentUserResolver = currentUserResolver;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<FamilyTreeDetailsDto> CreateFamilyTreeAsync(UpsertFamilyTreeRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrEmpty(request.Name))
                    throw new ArgumentException("Tên gia phả là bắt buộc");

                // Set default mode if not provided
                if (request.GPModeCode == null || request.GPModeCode == 0)
                {
                    request.GPModeCode = FamilyTreeModes.PRIVATE;
                }

                // Validate mode
                if (request.GPModeCode != FamilyTreeModes.PRIVATE && 
                    request.GPModeCode != FamilyTreeModes.PUBLIC && 
                    request.GPModeCode != FamilyTreeModes.SHARED)
                {
                    throw new UnauthorizedAccessException("Đối tượng có thể truy cập và xem cây gia phả không hợp lệ.");
                }

                var familyTree = new Domain.Entities.FamilyTree.FamilyTree
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Owner = request.Owner ?? _currentUserResolver.Name,
                    Description = request.Description,
                    Picture = request.Picture,
                    GPModeCode = request.GPModeCode,
                    IsActive = true,
                    CreatedOn = DateTimeOffset.UtcNow,
                    CreatedBy = _currentUserResolver.Email,
                    LastModifiedOn = DateTimeOffset.UtcNow,
                    LastModifiedBy = _currentUserResolver.Email,
                    IsDeleted = false
                };

                _context.FamilyTrees.Add(familyTree);
                await _context.SaveChangesAsync();

                // Complete the unit of work for FamilyTree creation
                await _unitOfWork.CompleteAsync();

                // Simplified: Just assign GPOwner role to user if not already assigned
                var currentUser = await _userManager.FindByIdAsync(_currentUserResolver.UserId.ToString());
                if (currentUser != null)
                {
                    // Ensure GPOwner role exists
                    var roleExists = await _roleManager.RoleExistsAsync(Roles.GPOwner);
                    if (!roleExists)
                    {
                        await _roleManager.CreateAsync(new ApplicationRole
                        {
                            Name = Roles.GPOwner,
                            NormalizedName = Roles.GPOwner.ToUpper()
                        });
                    }

                    var userRoles = await _userManager.GetRolesAsync(currentUser);
                    if (!userRoles.Contains(Roles.GPOwner))
                    {
                        await _userManager.AddToRoleAsync(currentUser, Roles.GPOwner);
                    }
                }

                return await GetFamilyTreeByIdAsync(familyTree.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo gia phả: {ex.Message}");
            }
        }

        public async Task<FamilyTreeDetailsDto> GetFamilyTreeByIdAsync(Guid id)
        {
            try
            {
                var familyTree = await _context.FamilyTrees
                    .Include(ft => ft.FTMembers.Where(m => m.IsDeleted != true))
                    .FirstOrDefaultAsync(ft => ft.Id == id && ft.IsDeleted != true);

                if (familyTree == null)
                    throw new ArgumentException("Không tìm thấy gia phả");

                // Count active members
                var numberOfMembers = familyTree.FTMembers?.Count(m => m.IsDeleted != true) ?? 0;

                // Get user roles for this family tree (simplified for now)
                var roles = new List<string>();
                var currentUser = await _userManager.FindByIdAsync(_currentUserResolver.UserId.ToString());
                if (currentUser != null)
                {
                    var userRoles = await _userManager.GetRolesAsync(currentUser);
                    roles = userRoles.ToList();
                }

                return new FamilyTreeDetailsDto
                {
                    Id = familyTree.Id,
                    CreatedBy = familyTree.CreatedBy,
                    CreatedOn = familyTree.CreatedOn,
                    LastModifiedBy = familyTree.LastModifiedBy,
                    LastModifiedOn = familyTree.LastModifiedOn,
                    Name = familyTree.Name,
                    Owner = familyTree.Owner,
                    Description = familyTree.Description,
                    Picture = familyTree.Picture,
                    IsActive = familyTree.IsActive ?? true,
                    GPModeCode = familyTree.GPModeCode,
                    NumberOfMember = numberOfMembers,
                    Roles = roles,
                    IsNeedConfirmAcceptInvited = false // TODO: Implement invitation system if needed
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy chi tiết gia phả: {ex.Message}");
            }
        }

        public async Task<FamilyTreeDetailsDto> UpdateFamilyTreeAsync(Guid id, UpsertFamilyTreeRequest request)
        {
            try
            {
                var familyTree = await _context.FamilyTrees
                    .FirstOrDefaultAsync(ft => ft.Id == id && ft.IsDeleted != true);

                if (familyTree == null)
                    throw new ArgumentException("Không tìm thấy gia phả");

                // Set default mode if not provided
                if (request.GPModeCode == null || request.GPModeCode == 0)
                {
                    request.GPModeCode = FamilyTreeModes.PRIVATE;
                }

                // Validate mode
                if (request.GPModeCode != FamilyTreeModes.PRIVATE && 
                    request.GPModeCode != FamilyTreeModes.PUBLIC && 
                    request.GPModeCode != FamilyTreeModes.SHARED)
                {
                    throw new ArgumentException("Đối tượng có thể truy cập và xem cây gia phả không hợp lệ.");
                }

                // Update properties
                familyTree.Name = request.Name;
                familyTree.Owner = request.Owner ?? familyTree.Owner;
                familyTree.Description = request.Description;
                familyTree.Picture = request.Picture;
                familyTree.GPModeCode = request.GPModeCode;
                familyTree.LastModifiedOn = DateTimeOffset.UtcNow;
                familyTree.LastModifiedBy = _currentUserResolver.Email;

                _context.FamilyTrees.Update(familyTree);
                await _context.SaveChangesAsync();

                return await GetFamilyTreeByIdAsync(familyTree.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật gia phả: {ex.Message}");
            }
        }

        public async Task DeleteFamilyTreeAsync(Guid id)
        {
            try
            {
                var familyTree = await _context.FamilyTrees
                    .FirstOrDefaultAsync(ft => ft.Id == id && ft.IsDeleted != true);

                if (familyTree == null)
                    throw new ArgumentException("Không tìm thấy gia phả");

                familyTree.IsDeleted = true;
                familyTree.LastModifiedOn = DateTimeOffset.UtcNow;
                familyTree.LastModifiedBy = _currentUserResolver.Email;

                _context.FamilyTrees.Update(familyTree);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa gia phả: {ex.Message}");
            }
        }

        public async Task<List<FamilyTreeDataTableDto>> GetFamilyTreesAsync()
        {
            try
            {
                return await _context.FamilyTrees
                    .Where(ft => ft.IsDeleted != true && ft.IsActive == true)
                    .Select(ft => new FamilyTreeDataTableDto
                    {
                        Id = ft.Id,
                        Name = ft.Name,
                        Owner = ft.Owner,
                        Description = ft.Description,
                        Picture = ft.Picture,
                        IsActive = ft.IsActive ?? true,
                        GPModeCode = ft.GPModeCode,
                        CreatedAt = ft.CreatedOn.DateTime,
                        LastModifiedAt = ft.LastModifiedOn.DateTime,
                        CreatedBy = ft.CreatedBy,
                        LastModifiedBy = ft.LastModifiedBy,
                        MemberCount = ft.FTMembers.Count(m => m.IsDeleted != true)
                    })
                    .OrderByDescending(ft => ft.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách gia phả: {ex.Message}");
            }
        }

        public async Task<List<FamilyTreeDataTableDto>> GetMyFamilyTreesAsync()
        {
            try
            {
                return await _context.FamilyTrees
                    .Where(ft => ft.IsDeleted != true && 
                                ft.IsActive == true && 
                                ft.CreatedBy == _currentUserResolver.Email)
                    .Select(ft => new FamilyTreeDataTableDto
                    {
                        Id = ft.Id,
                        Name = ft.Name,
                        Owner = ft.Owner,
                        Description = ft.Description,
                        Picture = ft.Picture,
                        IsActive = ft.IsActive ?? true,
                        GPModeCode = ft.GPModeCode,
                        CreatedAt = ft.CreatedOn.DateTime,
                        LastModifiedAt = ft.LastModifiedOn.DateTime,
                        CreatedBy = ft.CreatedBy,
                        LastModifiedBy = ft.LastModifiedBy,
                        MemberCount = ft.FTMembers.Count(m => m.IsDeleted != true)
                    })
                    .OrderByDescending(ft => ft.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách gia phả của tôi: {ex.Message}");
            }
        }
    }
}