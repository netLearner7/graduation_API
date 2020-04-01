using Core.Enum;

namespace Infrastructure.Dto
{
    /// <summary>
    /// meeting输出dto
    /// </summary>
    public class MeetingOutputDto
    {
        public int Id { get; set; }
        public string CreaterId { get; set; }
        public string UserName { get; set; }
        public string MeetingName { get; set; }
        public string Address { get; set; }
        public string Content { get; set; }
        public MeetingStateEnum MeetingState { get; set; }
        public string DateTime { get; set; }
        public string InviteCode { get; set; }

    }

    /// <summary>
    /// meeting输入dto
    /// </summary>
    public class MeetingInputDto {
        public int Id { get; set; }
        public string CreaterId { get; set; }
        public string MeetingName { get; set; }
        public string Address { get; set; }
        public string Content { get; set; }
        public MeetingStateEnum MeetingState { get; set; }
        public string DateTime { get; set; }
        public string InviteCode { get; set; }
    }

}
