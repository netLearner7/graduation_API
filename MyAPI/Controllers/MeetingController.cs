using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Core.Entity;
using Infrastructure.Data;
using Infrastructure.Dto;
using Infrastructure.Repository;
using Infrastructure.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

//var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

namespace MyAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/Meeting")]
    public class MeetingController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly MeetingRepository _meetingRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger<MeetingController> _logger;
        private readonly UserRepository _userRepository;
        private readonly InviteCodeHelper _inviteCodeHelper;

        public MeetingController(IMapper mapper, MeetingRepository meetingRepository, UnitOfWork unitOfWork,ILogger<MeetingController> logger,UserRepository userRepository, InviteCodeHelper inviteCodeHelper)
        {
            this._mapper = mapper;
            this._meetingRepository = meetingRepository;
            this._unitOfWork = unitOfWork;
            this._logger = logger;
            this._userRepository = userRepository;
            this._inviteCodeHelper = inviteCodeHelper;
        }

        /// <summary>
        /// 测试使用
        /// </summary>
        /// <returns></returns>
        [HttpGet("ceshi")]
        [AllowAnonymous]
        public IActionResult ceshi() {
            return Ok(new {a=1,b=2,c=3 });
        }

        /// <summary>
        /// 根据会议id查询会议详情
        /// </summary>
        /// <param name="meetingId">会议id</param>
        /// <returns>meeting对象</returns>
        /// <response code="200">查询成功</response>
        /// <response code="500">程序内部错误，导致查询不成功</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpGet("{meetingId}")]
        public async Task<IActionResult> Get([FromRoute]int meetingId) {
            var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            _logger.LogDebug($"{userid}-开始查询id为{meetingId}的会议");
            var meeting= await _meetingRepository.getMeeting(meetingId);

            if (meeting == null) {
                _logger.LogDebug($"{userid}-未查询到会议信息");
                return NotFound("未找到该会议！");
            }

            _logger.LogDebug($"{userid}-查询id为{meetingId}的创建者信息");
            var user= await _userRepository.getUser(meeting.CreaterId);

            _logger.LogDebug($"{userid}-转换查询到的信息");
            var resultDto = _mapper.Map<Meeting, MeetingOutputDto>(meeting);
            resultDto = _mapper.Map<MyIdentityUser, MeetingOutputDto>(user, resultDto);

            _logger.LogDebug($"{userid}-返回数据,action结束");
            return Ok(resultDto);
        }

        /// <summary>
        /// 根据UserId查询会议列表
        /// </summary>
        /// <param name="userId">会议创建人id</param>
        /// <returns>List（meeting）对象</returns>
        /// <response code="200">查询成功</response>
        /// <response code="500">程序内部错误，导致查询不成功</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]string userId) {
            _logger.LogDebug($"{userId}-查询{userId}的会议列表");
            var  list= await _meetingRepository.getMeetings(userId);

            _logger.LogDebug($"{userId}-对{userId}会议列表进行排序");
            list = sort.todayMeetingSort(list);

            _logger.LogDebug($"{userId}-{userId}的会议列表转换为dto");
            var result= _mapper.Map<IEnumerable<Meeting>,IEnumerable<MeetingOutputDto>>(list);

            _logger.LogDebug($"{userId}-返回{userId}的会议列表,action结束");
            return Ok(result);
        }



        /// <summary>
        /// 创建会议
        /// </summary>
        /// <param name="meetingAddDto">会议信息（不需要填写id和userid）</param>
        /// <returns>返回创建完的Meeting</returns>
        /// <response code="201">创建成功</response>
        /// <response code="500">程序内部错误，导致创建不成功</response>
        [ProducesResponseType(201)]
        [ProducesResponseType(500)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MeetingInputDto meetingAddDto)
        {
            var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            _logger.LogDebug($"{userid}-开始创建会议,转换会议dto为meeting");
           var meeting= _mapper.Map<MeetingInputDto, Meeting>(meetingAddDto);

            _logger.LogDebug($"{userid}-添加meeting信息");
            _meetingRepository.addMeeting(meeting);

            if (! await _unitOfWork.save()) {
                _logger.LogError($"{userid}-创建会议失败！");
                throw new Exception();
            }
            _logger.LogDebug($"{userid}-会议创建成功,转换数据");

            var  resultDto = _mapper.Map<Meeting, MeetingOutputDto>(meeting);

            _logger.LogDebug($"{userid}-返回会议数据,action结束");
            return Created("", resultDto);
        }


        /// <summary>
        /// 会议局部修改
        /// </summary>
        /// <param name="meetingId">会议id int类型</param>
        /// <param name="document">
        /// 局部修改操作： [{op:"操作符(add/move/replace/remove)",path:"/参数名",value:"修改后的值"}]
        /// </param>
        /// <returns></returns>
        /// <response code="204">更新成功</response>
        /// <response code="400">数据格式不正确</response>
        /// <response code="404">未找到该会议</response>
        /// <response code="500">服务器内部错误，更新失败</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [HttpPatch("{meetingId}")]
        public async Task<IActionResult> Patch([FromRoute]int meetingId, [FromBody]JsonPatchDocument<MeetingInputDto> document)
        {
            var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            _logger.LogDebug($"{userid}-查询会议信息");
            //根据id查询出会议
            var entity = await _meetingRepository.getMeeting(meetingId);

            if (entity == null) {
                _logger.LogDebug($"{userid}-未找到{meetingId}会议,action结束返回404");
                return NotFound("未找到该会议");
            }

            
            //将会议传转成dto
            var meetingdto= _mapper.Map<Meeting, MeetingInputDto>(entity);

            _logger.LogDebug($"{userid}-修补会议信息");
            //修补操作到dto
            document.ApplyTo(meetingdto, ModelState);

            //重新验证模型状态
            TryValidateModel(meetingdto);

            if (!ModelState.IsValid) {
                _logger.LogDebug($"{userid}-修补会议信息失败,action结束返回404");
                return BadRequest("传送数据错误");
            }

            //转换回module
            _mapper.Map(meetingdto,entity);

            _logger.LogDebug($"{userid}-更新会议信息");
            //更新操作
            _meetingRepository.UpdateMeeting(entity);

            //验证更新结果
            if (!await _unitOfWork.save())
            {
                _logger.LogError($"{userid}-保存更新后的会议信息失败,返回500");
                throw new Exception("更新失败！");
            }
            _logger.LogDebug($"{userid}-action结束");
            return NoContent();
        }

        /// <summary>
        /// 修改操作
        /// </summary>
        /// <param name="meetingAddDto"></param>
        /// <returns>不做返回</returns>
        /// <response code="204">更新成功</response>
        /// <response code="500">更新失败</response>
        [ProducesResponseType(500)]
        [ProducesResponseType(204)]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] MeetingInputDto meetingAddDto) {
            var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;
            _logger.LogDebug($"{userid}-整体更新会议信息");
            var meeting= _mapper.Map<MeetingInputDto, Meeting>(meetingAddDto);
            _meetingRepository.UpdateMeeting(meeting);

            if ( !await _unitOfWork.save()) {
                _logger.LogError($"{userid}-保存更新后的会议信息失败,返回500");
                throw new Exception("保存失败");
            }

            _logger.LogDebug($"{userid}-action结束");
            return NoContent();
        }

        /// <summary>
        /// 生成邀请码
        /// </summary>
        /// <param name="meetingId">会议id</param>
        /// <returns>返回id为meetingid有邀请码的meeting</returns>
        /// <response code="200">创建成功并返回</response>
        /// <response code="500">服务器内部错误</response>
        [ProducesResponseType(500)]
        [ProducesResponseType(200)]
        [HttpPost("{meetingId}/createInviteCode")]
        public async Task<IActionResult> Post(int meetingId) {
            var meeting= await _meetingRepository.getMeeting(meetingId);

            await _inviteCodeHelper.create(meeting);

            var resultDto = _mapper.Map<Meeting, MeetingOutputDto>(meeting);

            return Ok(resultDto);
        }

        /// <summary>
        /// 解析邀请码，返回会议信息
        /// </summary>
        /// <param name="InviteCode">邀请码</param>
        /// <returns>返回会议信息</returns>
        /// <response code="200">解析成功返回meeting对象</response>
        /// <response code="400">传递邀请码错误</response>
        /// <response code="500">服务器内部错误</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [HttpGet("analysisInviteCode")]
        public async Task<IActionResult> AnalysisInviteCode(string InviteCode) {
            var userid = User.FindFirst(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (!_inviteCodeHelper.tryAnalysis(HttpUtility.UrlDecode(InviteCode), out int meetingId)) {
                _logger.LogDebug($"{userid}-解析无效邀请码返回400");
                return BadRequest("无效邀请码");
            }

            _logger.LogDebug($"{userid}-解析成功后,获取会议信息");
            var meeting = await _meetingRepository.getMeeting(meetingId);

            var resultDto = _mapper.Map<Meeting, MeetingOutputDto>(meeting);

            resultDto.Id = 0;
            _logger.LogDebug($"{userid}-返回数据,action结束");
            return Ok(resultDto);
        }


    }
}
