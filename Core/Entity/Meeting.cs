using Core.Enum;
using System;
using System.Collections.Generic;

namespace Core.Entity
{
    public class Meeting
    {
        public int Id { get; set; }

        //会议创建者id
        public string CreaterId { get; set; }
        public string MeetingName { get; set; }
        public string Address { get; set; }
        public string Content { get; set; }
        public DateTime DateTime { get; set; }
        public MeetingStateEnum MeetingState { get; set; }
        public string InviteCode { get; set; }
        public List<User_Meeting>  User_Meetings { get; set; }
    }
}
