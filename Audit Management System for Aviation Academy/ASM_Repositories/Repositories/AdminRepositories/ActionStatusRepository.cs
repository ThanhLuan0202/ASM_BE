using ASM_Repositories.DBContext;
using ASM_Repositories.Entities;
using ASM_Repositories.Interfaces.AdminInterfaces;
using ASM_Repositories.Models.ActionStatusDTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_Repositories.Repositories.AdminRepositories
{
    public class ActionStatusRepository : IActionStatusRepository
    {
        private readonly AuditManagementSystemForAviationAcademyContext _context;
        private readonly IMapper _mapper;

        public ActionStatusRepository(AuditManagementSystemForAviationAcademyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ViewActionStatus>> GetAllAsync()
        {
            var data = await _context.ActionStatuses.ToListAsync();
            return _mapper.Map<IEnumerable<ViewActionStatus>>(data);
        }

        public async Task<ViewActionStatus?> GetByIdAsync(string actionStatus)
        {
            var entity = await _context.ActionStatuses
                .FirstOrDefaultAsync(x => x.ActionStatus1 == actionStatus);
            return entity == null ? null : _mapper.Map<ViewActionStatus>(entity);
        }

        public async Task<ViewActionStatus> CreateAsync(CreateActionStatus dto)
        {
            bool isExist = await _context.ActionStatuses
                .AnyAsync(x => x.ActionStatus1 == dto.ActionStatus1);

            if (isExist)
                throw new InvalidOperationException("ActionStatus already exists!");

            var entity = _mapper.Map<ActionStatus>(dto);
            _context.ActionStatuses.Add(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewActionStatus>(entity);
        }

        public async Task<ViewActionStatus?> UpdateAsync(string actionStatus, UpdateActionStatus dto)
        {
            var entity = await _context.ActionStatuses
                .FirstOrDefaultAsync(x => x.ActionStatus1 == actionStatus);

            if (entity == null) return null;

            bool isExist = await _context.ActionStatuses
                .AnyAsync(x => x.ActionStatus1 == dto.ActionStatus1 && dto.ActionStatus1 != actionStatus);

            if (isExist)
                throw new InvalidOperationException("ActionStatus already exists!");

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ViewActionStatus>(entity);
        }

        public async Task<bool> DeleteAsync(string actionStatus)
        {
            var entity = await _context.ActionStatuses
                .Include(x => x.Actions)
                .FirstOrDefaultAsync(x => x.ActionStatus1 == actionStatus);

            if (entity == null) return false;

            if (entity.Actions.Any())
                throw new InvalidOperationException("Cannot delete this ActionStatus because it is being used by one or more Actions!");

            _context.ActionStatuses.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
