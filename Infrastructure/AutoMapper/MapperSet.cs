using AutoMapper;
using Core.Entity;
using Infrastructure.Dto;
using System;

namespace Infrastructure.AutoMapper
{
    public class MapperSet : Profile
    {
        public MapperSet()
        {
            #region 会议与dto
            CreateMap<MeetingInputDto, Meeting>()
                //.ForMember(o=>o.CreaterId,opt=>opt.MapFrom(x=>x.UserId))
                .ForMember(o => o.DateTime, opt => opt.MapFrom(x => Convert.ToDateTime(x.DateTime))).ReverseMap();
               

            CreateMap<Meeting, MeetingOutputDto>()
                .ForMember(o => o.DateTime, opt => opt.MapFrom(x=>x.DateTime.ToString("yyyy-MM-dd HH:mm:ss")));

            CreateMap<MyIdentityUser, MeetingOutputDto>().
                ForMember(o => o.UserName, opt => opt.MapFrom(x => x.UserName)).
                ForMember(o=>o.Id,opt=>opt.Ignore());
            #endregion 

            #region 中间表与dto

            CreateMap< User_MeetingInputDto, User_Meeting>().ReverseMap();
            CreateMap<User_Meeting, User_MeetingOutputDto>().
                ForMember(x => x.MeetingName, y => y.MapFrom(o => o.Meeting.MeetingName)).
                ForMember(x => x.Address, y => y.MapFrom(o => o.Meeting.Address)).
                ForMember(x => x.Content, y => y.MapFrom(o => o.Meeting.Content)).
                ForMember(x => x.DateTime, y => y.MapFrom(o => o.Meeting.DateTime.ToString("yyyy-MM-dd HH:mm:ss"))).
                ForMember(x => x.MeetingState, y => y.MapFrom(o => o.Meeting.MeetingState)).
                ForMember(x=>x.UserName,y=>y.MapFrom(o=>o.User.UserName)).

                ForMember(x => x.RegistrationDate, y => y.MapFrom(o => o.RegistrationDate.ToString("yyyy-MM-dd HH:mm:ss"))).
                ForMember(x => x.CheckInDate, y => y.MapFrom(o => o.CheckInDate.ToString("yyyy-MM-dd HH:mm:ss")));

                
           
            #endregion 
        }
    }

}
