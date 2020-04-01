using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Core.Entity;
using Core.Enum;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Repository;
using Infrastructure.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MyAPI.Controllers
{
    [Route("api/User_Meeting")]
    [ApiController]
    [Authorize]
    public class User_MeetingController : ControllerBase
    {
        private readonly InviteCodeHelper inviteCodeHelper;
        private readonly ILogger<User_MeetingController> logger;
        private readonly User_MeetingRepository user_MeetingRepository;
        private readonly IMapper mapper;
        private readonly UnitOfWork unitOfWork;
        private readonly UserRepository userRepository;
        private readonly MeetingRepository meetingRepository;

        public User_MeetingController(InviteCodeHelper inviteCodeHelper,
            ILogger<User_MeetingController> logger, 
            User_MeetingRepository user_MeetingRepository,
            IMapper mapper,UnitOfWork unitOfWork, 
            UserRepository userRepository, MeetingRepository meetingRepository )
        {
            this.inviteCodeHelper = inviteCodeHelper;
            this.logger = logger;
            this.user_MeetingRepository = user_MeetingRepository;
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.userRepository = userRepository;
            this.meetingRepository = meetingRepository;
        }

        /// <summary>
        /// 获取报名参加会议的列表
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <response code="200">查询成功</response>
        [HttpGet]
        public async Task<IActionResult> GetJoinList([FromQuery]string userId) {
            logger.LogDebug($"{userId}-获取参加会议的列表");
            var list= await user_MeetingRepository.Gets_UserId_Async(userId);
            list = sort.todayUserMeetingSort(list);
            var resultDto = mapper.Map<IEnumerable<User_Meeting>,IEnumerable <User_MeetingOutputDto>>(list);
            logger.LogDebug($"{userId}-获取成功,action结束");
            return Ok(resultDto);
        }


        /// <summary>
        /// 查询会议时间为今天，参与者id为当前用户的会议
        /// </summary>
        /// <response code="200">查询成功</response>
        [HttpGet("TodayUserMeeting")]
        public async Task<IActionResult> GetTodayMeetingList()
        {
            
            var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            logger.LogDebug($"{userid}-获取今天参加会议的列表");
            var list=  await user_MeetingRepository.Gets_UserIdDate_Async(userid, DateTime.Now);
            var result=  mapper.Map<IEnumerable< User_Meeting>, IEnumerable< User_MeetingOutputDto>>(list);

            return Ok(result);
        }

        /// <summary>
        /// 根据中间表id查询有关的所有信息
        /// </summary>
        /// <param name="Id"></param>
        /// <response code="200">查询成功</response>
        [HttpGet("{Id}")]
        public async Task< IActionResult> GetDetails([FromRoute]int Id) {
            var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            logger.LogDebug($"{userid}-获取中间表的所有信息");

            var user_meeting= await user_MeetingRepository.Get_Id_Async(Id);
            var user= await userRepository.getUser(user_meeting.Meeting.CreaterId);            
            var resultDto = mapper.Map<User_Meeting, User_MeetingOutputDto>(user_meeting);
            //可以改进
            resultDto.CreatorName = user.UserName;
            return Ok(resultDto);
        }

        /// <summary>
        /// 获取参会人员名单
        /// </summary>
        /// <param name="meetingId">会议id</param>
        /// <response code="200">查询成功</response>
        [HttpGet("PersonList")]
        public async Task<IActionResult> GetPersonList([FromQuery]int meetingId) {
            var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            logger.LogDebug($"{userid}-获取参会人员名单");
            var list = await user_MeetingRepository.Gets_MeetingId_Async(meetingId);
            var result = mapper.Map<IEnumerable<User_Meeting>, IEnumerable<User_MeetingOutputDto>>(list);
            return Ok(result);
        }


        /// <summary>
        /// 解析邀请码,添加一条会议记录
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        /// <response code="200">创建成功</response>
        /// <response code="400">邀请码无效，获取参会信息已经存在</response>
        /// <response code="500">程序内部错误</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpPost]
        public async Task<IActionResult> Post(User_MeetingInputDto inputDto) {

            var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            logger.LogDebug($"{userid}-添加一条中间表信息");

            if (!inviteCodeHelper.tryAnalysis(
                HttpUtility.UrlDecode(inputDto.InviteCode), out int resultValue)) 
            {

                logger.LogDebug($"{userid}-添加参会信息的时候解析邀请码无效，返回400");
                return BadRequest("邀请码无效");
            }

            inputDto.MeetingId = resultValue;

            var meeting= mapper.Map<User_MeetingInputDto, User_Meeting>(inputDto);

            if (user_MeetingRepository.Has(meeting)) {
                logger.LogDebug($"{userid}-尝试插入一条重复的中间表数据");
                return BadRequest("您已报名无需重复报名");
            }

            user_MeetingRepository.Add(meeting);

            if (! await unitOfWork.save()) {
                logger.LogError($"{userid}-参加会议时，保存失败！");
                throw new Exception("保存失败！");
           
            }

            var resultDto = mapper.Map<User_Meeting, User_MeetingOutputDto>(meeting);
            logger.LogDebug($"{userid}-参加会议成功,action结束");

            return Created("",resultDto);
        }


        /// <summary>
        /// 根据id删除一条参会记录,作用于取消参会
        /// </summary>
        /// <param name="Id"></param>
        /// <returns code="400">未找到会议</returns>
        /// <returns code="204">修改成功</returns>
        /// <returns code="500">保存时出现错误</returns>
        [HttpDelete("{Id}")]
        public async Task<IActionResult> Del(int Id) {
            var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var user_Meeting= await user_MeetingRepository.Get_Id_Async(Id);

            if (user_Meeting == null) {
                logger.LogDebug($"{userid}-取消参会,未找到改记录返回404");
                return NotFound("该会议不存在！");
            }

            user_MeetingRepository.Del(user_Meeting);

            if (!await unitOfWork.save()) {
                logger.LogError($"{userid}-取消参会时保存失败");
                throw new Exception("修改失败！");
            }
            logger.LogDebug($"{userid}-取消参会成功,action结束");
            return NoContent();
        }


        /// <summary>
        /// 中间表更新签到状态，作用于会议签到,应该是patch方法应为uniapp不支持所以改为post
        /// </summary>
        /// <param name="meetingId">会议id</param>
        /// <returns code="400">签到失败，可能是因为会议结束/取消 未参加会议，已签到完毕</returns>
        /// <returns code="204">签到成功</returns>
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [HttpPost("{meetingId}")]
        public async Task<IActionResult> Patch([FromRoute]int meetingId) 
        {
            var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var meeting = await meetingRepository.getMeeting(meetingId);

            if (meeting.MeetingState == MeetingStateEnum.Complete)
            {
                logger.LogDebug($"{userid}-会议签到失败,会议已完成");
                return BadRequest("会议已完成!");
            }

            if (meeting.MeetingState == MeetingStateEnum.Cancel)
            {
                logger.LogDebug($"{userid}-会议签到失败,会议已取消");
                return BadRequest("会议已取消!");
            }

            if (meeting.MeetingState == MeetingStateEnum.NotStarted)
            {
                logger.LogDebug($"{userid}-会议签到失败,会议未开始");
                return BadRequest("会议未开始!");
            }

       


            //筛选出对应信息
            var user_meeting=await user_MeetingRepository.Get_UserIdMeetingId_Async(userid, meetingId);

            if (user_meeting==null)
            {
                logger.LogDebug($"{userid}-会议签到失败,未参加该会议");
                return BadRequest( "您尚未参加此会议!");
            }
            if (user_meeting.User_MeetingStats == User_MeetingStateEnum.SignIn) {
                logger.LogDebug($"{userid}-会议签到失败,已签到");
                return BadRequest("您已签到无需重复签到!"  );
            }

            user_meeting.User_MeetingStats = User_MeetingStateEnum.SignIn;
            user_meeting.CheckInDate = DateTime.Now;

            logger.LogDebug($"{userid}-会议签到,修改签到状态");
            //更新
            user_MeetingRepository.Update(user_meeting);

            //验证更新结果
            if (!await unitOfWork.save())
            {
                logger.LogError($"{userid}-会议签到,修改签到状态失败");
                throw new Exception("更新失败！");
            }
            logger.LogDebug($"{userid}-会议签到成功,action结束");
            return Ok("签到成功");

        }

    }
}