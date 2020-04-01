using Core.Entity;
using Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Dto
{
    public class User_MeetingInputDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MeetingId { get; set; }
        public User_MeetingStateEnum User_MeetingStats { get; set; }
        public string InviteCode { get; set; }
    }

    public class User_MeetingOutputDto
    {
        public int Id { get; set; }
        public User_MeetingStateEnum User_MeetingStats { get; set; }
        public string RegistrationDate { get; set; }
        public string CheckInDate { get; set; }
        //用户表信息
        //参会者名字
        public string UserName { get; set; }
        ///会议表信息
        public string MeetingName { get; set; }
        public string Address { get; set; }
        public string Content { get; set; }
        public string DateTime { get; set; }
        public MeetingStateEnum MeetingState { get; set; }
        //创建者姓名 需要使用meeting表中的创建者id二次查询出姓名
        public string CreatorName { get; set; }


    }
}
