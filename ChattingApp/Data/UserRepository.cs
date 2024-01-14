using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChattingApp.DTOs;
using ChattingApp.Helpers;
using ChattingApp.Interfaces;
using ChattingApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace ChattingApp.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(AppDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
                
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.AppUsers.Where(x => x.UserName == username)
                 .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                 .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userparams)
        {

            var query= _context.AppUsers.AsQueryable();
            query = query.Where(u => u.UserName != userparams.CurrentUsername);

            var minDob=DateOnly.FromDateTime(DateTime.Today.AddYears(-userparams.MaxAge));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userparams.MinAge));

            query=query.Where(u=>u.DateOfBirth>=minDob &&  u.DateOfBirth<=maxDob);

            query = userparams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _=> query.OrderByDescending(u => u.LastActive) //_ for default
            } ;


            return await PagedList<MemberDto>.CreateAsync(
                query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
                userparams.PageNumber,
                userparams.PageSize);
                
            //var query = _context.AppUsers

            //    .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            //    .AsNoTracking();

            //return await PagedList<MemberDto>.CreateAsync(query, userparams.PageNumber, userparams.PageSize);
                
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.AppUsers.FindAsync(id);
        }

        public async Task<AppUser> GetUSerByUsernameAsync(string username)
        {
            return await _context.AppUsers.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.AppUsers.Include(u => u.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

       

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        //Task<IEnumerable<AppUser>> IUSerRepositorye.GetUserByIdAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
