using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Extentions;
using WebApplication1.Models;

namespace WebApplication1.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUsers, MemberDTO>()
                .ForMember(dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo,PhotoDTO>();
            CreateMap<MemberUpdateDTO,AppUsers>();
            CreateMap<Registerdto,AppUsers>();
            CreateMap<Message,MessageDTO>()
                .ForMember(d=>d.senderPhotoUrl,o=>o.MapFrom(s=>s.Sender.Photos.FirstOrDefault(x=>x.IsMain).Url))
                .ForMember(d => d.recipientPhotoUrl, o => o.MapFrom(r => r.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
        }
    }
}
