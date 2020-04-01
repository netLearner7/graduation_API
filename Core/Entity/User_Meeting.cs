using Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entity
{
    public class User_Meeting
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public MyIdentityUser User { get; set; }
        public int MeetingId { get; set; }
        public Meeting Meeting { get; set; }
        public User_MeetingStateEnum User_MeetingStats { get; set; }

        public DateTime RegistrationDate { get; set; }
        public DateTime CheckInDate { get; set; }
    }
}
