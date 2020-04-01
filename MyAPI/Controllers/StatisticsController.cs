using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Repository;
using Infrastructure.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsRepository statisticsRepository;
        private readonly ILogger<StatisticsController> logger;

        public StatisticsController(
            StatisticsRepository statisticsRepository,
            ILogger<StatisticsController> logger)
        {
            this.statisticsRepository = statisticsRepository;
            this.logger = logger;
        }

        /// <summary>
        /// 按月统计我发起的会议数量
        /// 统计从本月开始向前12个月的数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("statist_12_Meeting/{userid}")]
        public async Task< IActionResult> getMyMeeting(string userid) {
            logger.LogDebug($"{userid}-查询12个月内组织的会议");

            var list = new List<StatisticsDto>();

            for (int i = 0; i < 12; i++)
            {
               StatisticsDto a = await statisticsRepository.MeetingAsync(i, userid);
               list.Add(a);
            }
            list.Reverse();
            return Ok(list);
        }

        [HttpGet("statist_12_UserMeeting/{userid}")]
        public async Task<IActionResult> getUser_Meeting(string userid) {

            logger.LogDebug($"{userid}-查询12个月内参加的会议");
            var list = new List<StatisticsDto>();

            for (int i = 0; i < 12; i++)
            {
                StatisticsDto a = await statisticsRepository.User_MeetingAsync(i,userid);
                list.Add(a);
            }

            list.Reverse();

            return Ok(list);
        }

        /// <summary>
        /// 获取该用户的会议数量包括我的会议数量和参加会议数量
        /// </summary>
        /// <returns></returns>
        [HttpGet("statistNumber/{userid}/{thisMonth}")]
        public async Task<IActionResult> getAll(string userid,bool thisMonth) {
            logger.LogDebug($"{userid}-查询当月/全部的会议数据");

            int meetingNumber = await statisticsRepository.getAllMeeting(userid, thisMonth);
            int userMeetingNumber = await statisticsRepository.getAllUserMeeting(userid, thisMonth);

            return Ok(new { meetingNumber,userMeetingNumber});
        }


    }
}