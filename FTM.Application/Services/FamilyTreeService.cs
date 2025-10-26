using AutoMapper;
using FTM.Application.IServices;
using FTM.Domain.Constants;
using FTM.Domain.DTOs.FamilyTree;
using FTM.Domain.Entities.FamilyTree;
using FTM.Domain.Entities.Identity;
using FTM.Domain.Enums;
using FTM.Domain.Specification.FamilyTrees;
using FTM.Domain.Specification.FTMembers;
using FTM.Infrastructure.Data;
using FTM.Infrastructure.Repositories.Implement;
using FTM.Infrastructure.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XAct.Users;
using static FTM.Domain.Constants.Constants;

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
        private readonly IGenericRepository<FamilyTree> _familyTreeRepository;
        private readonly IFTUserRepository _ftUserRepository;
        private readonly IMapper _mapper;
        private readonly IBlobStorageService _blobStorageService;

        public FamilyTreeService(
            FTMDbContext context,
            AppIdentityDbContext appIdentityDbContext,
            ICurrentUserResolver currentUserResolver,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IGenericRepository<FamilyTree> familyTreeRepository,
            IFTUserRepository ftUserRepository,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IBlobStorageService blobStorageService)
        {
            _context = context;
            _appIdentityDbContext = appIdentityDbContext;
            _currentUserResolver = currentUserResolver;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _familyTreeRepository = familyTreeRepository;
            _ftUserRepository = ftUserRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _blobStorageService = blobStorageService;
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
                    throw new UnauthorizedAccessException("Vui lòng chọn chế độ cho cây gia phả");
                }

                // Handle File
                var file = request.File;
                var filePath = "";
                if(file != null)
                {
                    // Upload to Blob Storage
                    filePath = await _blobStorageService.UploadFileAsync(file, "familytrees", null);
                }    

                var familyTree = new FamilyTree
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Owner = _currentUserResolver.Name,
                    OwnerId =  _currentUserResolver.UserId,
                    Description = request.Description,
                    GPModeCode = request.GPModeCode,
                    FilePath = filePath,
                    FileType = 1,
                    IsActive = true,
                    CreatedOn = DateTimeOffset.UtcNow,
                    CreatedBy = _currentUserResolver.Email,
                    LastModifiedOn = DateTimeOffset.UtcNow,
                    LastModifiedBy = _currentUserResolver.Email,
                    IsDeleted = false
                };

                await _familyTreeRepository.AddAsync(familyTree);

                // Assign Role
                var owner = new FTUser
                {
                    UserId = _currentUserResolver.UserId,
                    Name = _currentUserResolver.Name,
                    Username = _currentUserResolver.Username,
                    FTId = familyTree.Id,
                    FTRole = FTMRole.FTOwner
                };
                
                await _ftUserRepository.AddAsync(owner);
                await _unitOfWork.CompleteAsync();

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
                //var roles = new List<string>();
                //var currentUser = await _userManager.FindByIdAsync(_currentUserResolver.UserId.ToString());
                //if (currentUser != null)
                //{
                //    var userRoles = await _userManager.GetRolesAsync(currentUser);
                //    roles = userRoles.ToList();
                //}

                return new FamilyTreeDetailsDto
                {
                    Id = familyTree.Id,
                    CreatedBy = familyTree.CreatedBy,
                    CreatedOn = familyTree.CreatedOn,
                    LastModifiedBy = familyTree.LastModifiedBy,
                    LastModifiedOn = familyTree.LastModifiedOn,
                    Name = familyTree.Name,
                    OwnerId = familyTree.OwnerId,
                    Owner = familyTree.Owner,
                    FilePath = familyTree.FilePath,
                    Description = familyTree.Description,
                    IsActive = familyTree.IsActive ?? true,
                    GPModeCode = familyTree.GPModeCode,
                    NumberOfMember = numberOfMembers,
                    //Roles = roles,
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
                string newOwner = string.Empty;
                var familyTree = await _context.FamilyTrees
                    .FirstOrDefaultAsync(ft => ft.Id == id && ft.IsDeleted != true);


                if (familyTree == null)
                    throw new ArgumentException("Không tìm thấy gia phả");

                if(request.OwnerId != null)
                {
                    var user = await _userManager.FindByIdAsync(request.OwnerId.ToString());
                    if (user is null)
                    {
                        throw new ArgumentException("Không tìm thấy người sở hữu");
                    }
                    newOwner = user.Name;
                    var owner = await _ftUserRepository.FindOwnerAsync(familyTree.Id);

                    if (owner is null) throw new ArgumentException("Không tìm thấy người sở hữu cây gia phả");
                    owner.Name = newOwner;
                    owner.Username = user.UserName;
                    owner.UserId = user.Id;
                    _ftUserRepository.Update(owner);
                }
                

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


                // Handle File
                if (request.File != null)
                {
                    // Upload to Blob Storage
                    familyTree.FilePath = await _blobStorageService.UploadFileAsync(request.File, "familytrees", null);
                }

                // Update properties
                familyTree.Name = request.Name ?? familyTree.Name;
                if(request.OwnerId != null)
                {
                    familyTree.OwnerId = request.OwnerId.Value;
                    familyTree.Owner = newOwner;
                }
                familyTree.Description = request.Description?? familyTree.Description;
                familyTree.GPModeCode = request.GPModeCode ?? familyTree.GPModeCode;
                familyTree.LastModifiedOn = DateTimeOffset.UtcNow;
                familyTree.LastModifiedBy = _currentUserResolver.Username;

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

        public async Task<IReadOnlyList<FamilyTreeDataTableDto>> GetFamilyTreesAsync(FamilyTreeSpecParams specParams)
        {
            var spec = new FamilyTreeSpecification(specParams);
            var fts = await _familyTreeRepository.ListAsync(spec);

            return _mapper.Map<IReadOnlyList<FamilyTreeDataTableDto>>(fts);
        }

        public async Task<IReadOnlyList<FamilyTreeDataTableDto>> GetMyFamilyTreesAsync(FamilyTreeSpecParams specParams)
        {
            var spec = new MyFamilyTreeSpecification(_currentUserResolver.UserId,specParams);
            var fts = await _familyTreeRepository.ListAsync(spec);

            return _mapper.Map<IReadOnlyList<FamilyTreeDataTableDto>>(fts);
        }

        //public async Task<List<FamilyTreeDataTableDto>> GetMyFamilyTreesAsync()
        //{
        //    try
        //    {
        //        return await _context.FamilyTrees
        //            .Where(ft => ft.IsDeleted != true && 
        //                        ft.IsActive == true && 
        //                        ft.CreatedBy == _currentUserResolver.Email)
        //            .Select(ft => new FamilyTreeDataTableDto
        //            {
        //                Id = ft.Id,
        //                Name = ft.Name,
        //                OwnerId = ft.OwnerId,
        //                Owner = ft.Owner,
        //                Description = ft.Description,
        //                IsActive = ft.IsActive ?? true,
        //                GPModeCode = ft.GPModeCode,
        //                CreatedAt = ft.CreatedOn.DateTime,
        //                LastModifiedAt = ft.LastModifiedOn.DateTime,
        //                CreatedBy = ft.CreatedBy,
        //                LastModifiedBy = ft.LastModifiedBy,
        //                MemberCount = ft.FTMembers.Count(m => m.IsDeleted != true)
        //            })
        //            .OrderByDescending(ft => ft.CreatedAt)
        //            .ToListAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Lỗi khi lấy danh sách gia phả của tôi: {ex.Message}");
        //    }
        //}

        public async Task<int> CountFamilyTreesAsync(FamilyTreeSpecParams specParams)
        {
            var spec = new FamilyTreeForCountSpecification(specParams);
            return await _familyTreeRepository.CountAsync(spec);
        }

        public async Task<int> CountMyFamilyTreesAsync(FamilyTreeSpecParams specParams)
        {
            var spec = new MyFamilyTreeForCountSpecification(_currentUserResolver.UserId, specParams);
            return await _familyTreeRepository.CountAsync(spec);
        }
    }
}