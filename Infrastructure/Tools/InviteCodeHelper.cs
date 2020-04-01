using Core.Entity;
using Infrastructure.Data;
using Infrastructure.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Tools
{
    public class InviteCodeHelper
    {
        private readonly MeetingRepository _meetingRepository;
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly string key;

        public InviteCodeHelper(MeetingRepository meetingRepository,UnitOfWork unitOfWork, IConfiguration configuration,ILogger<InviteCodeHelper> logger)
        {
            this._meetingRepository = meetingRepository;
            this._unitOfWork = unitOfWork;
            this._configuration = configuration;
            this._logger = logger;
            this.key= _configuration.GetSection("AESKey").Value;
        }

        /// <summary>
        /// 创建邀请码
        /// </summary>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public async Task<bool>  create(Meeting meeting ) {

            //json序列化
            string content = JsonConvert.SerializeObject(new
            {
                meeting.Id,
                meeting.MeetingName
            });

            //进行加密
            string InviteCode = AESHelper.AesEncrypt(content, key);

            //更新数据
            meeting.InviteCode = InviteCode;
            _meetingRepository.UpdateMeeting(meeting);

            //保存
            if (!await _unitOfWork.save())
            {
                _logger.LogError("创建邀请码失败！");
                return false;
            }

            return true;
        }

        public bool tryAnalysis(string InviteCode,out int resultValue) {

            Meeting meeting = null;
            try
            {
                //解析字符串
                string str = AESHelper.AesDecrypt(InviteCode, key);
                //反序列化
                 meeting = JsonConvert.DeserializeObject<Meeting>(str);
            }
            catch (Exception)
            {
                resultValue = 0;
                return false;
            }
            //赋值
           resultValue = meeting.Id;
           return true;
        }


    }
}
