using AutoMapper;
using ChattingApp.DTOs;
using ChattingApp.Extensions;
using ChattingApp.Models;

namespace ChattingApp.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            ;
            CreateMap<Photo,PhotoDto>();

            CreateMap<MemberUpdateDto, AppUser>();
        }
    }
}
