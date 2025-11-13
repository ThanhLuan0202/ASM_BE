using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces;
using ASM_Repositories.Models.AttachmentEntityTypeDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories
{
    public class AttachmentEntityTypeRepository : IAttachmentEntityTypeRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public AttachmentEntityTypeRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewAttachmentEntityType>> GetAllAsync()
        {
            var data = await _context.AttachmentEntityTypes.ToListAsync();
            return _mapper.Map<IEnumerable<ViewAttachmentEntityType>>(data);
        }

        public async Task<ViewAttachmentEntityType?> GetByIdAsync(string entityType)
        {
            var entity = await _context.AttachmentEntityTypes
                .FirstOrDefaultAsync(x => x.EntityType == entityType);
            return entity == null ? null : _mapper.Map<ViewAttachmentEntityType>(entity);
        }

        public async Task<ViewAttachmentEntityType> CreateAsync(CreateAttachmentEntityType dto)
        {
            bool isExist = await _context.AttachmentEntityTypes
                .AnyAsync(x => x.EntityType == dto.EntityType);

            if (isExist)
                throw new InvalidOperationException("AttachmentEntityType already exists!");

            var entity = _mapper.Map<AttachmentEntityType>(dto);
            _context.AttachmentEntityTypes.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewAttachmentEntityType>(entity);
        }

        public async Task<ViewAttachmentEntityType?> UpdateAsync(string entityType, UpdateAttachmentEntityType dto)
        {
            var entity = await _context.AttachmentEntityTypes
                .FirstOrDefaultAsync(x => x.EntityType == entityType);

            if (entity == null) return null;

            bool isExist = await _context.AttachmentEntityTypes
                .AnyAsync(x => x.EntityType == dto.EntityType && dto.EntityType != entityType);

            if (isExist)
                throw new InvalidOperationException("AttachmentEntityType already exists!");

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewAttachmentEntityType>(entity);
        }

        public async Task<bool> DeleteAsync(string entityType)
        {
            var entity = await _context.AttachmentEntityTypes
                .Include(x => x.Attachments)
                .Include(x => x.Notifications)
                .FirstOrDefaultAsync(x => x.EntityType == entityType);

            if (entity == null) return false;

            if (entity.Attachments.Any())
                throw new InvalidOperationException("Cannot delete this AttachmentEntityType because it is being used by one or more Attachments!");

            if (entity.Notifications.Any())
                throw new InvalidOperationException("Cannot delete this AttachmentEntityType because it is being used by one or more Notifications!");

            _context.AttachmentEntityTypes.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

