using Core.Entity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class User_MeetingRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<User_MeetingRepository> logger;

        public User_MeetingRepository(ApplicationDbContext dbContext,ILogger<User_MeetingRepository> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        //方法命名规则 动作_条件_异步
        //动作 add添加 get查询 del删除 update更新 has存在
        //条件不写默认为id   条件这部分只适用于仓储吧
        //异步不写默认为同步

        /// <summary>
        /// 查询一条Id为id的信息
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public async Task<User_Meeting> Get_Id_Async(int id) {
           return  await dbContext.
                User_Meeting.
                Where(x=>x.Id==id).
                Include(x=>x.Meeting).
                //Include(x=>x.User).
                FirstOrDefaultAsync();
        }

        /// <summary>
        /// 查询所有UserId为userId的信息，包含meeting表
        /// </summary>
        /// <returns></returns>
        public async Task<IList<User_Meeting>> Gets_UserId_Async(string userId) {
            return await dbContext.User_Meeting.
                Where(x => x.UserId == userId).
                Include(x => x.Meeting).
                ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meetingId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User_Meeting>> Gets_MeetingId_Async(int meetingId) {
          return  await dbContext.User_Meeting.Include(x=>x.User).Where(x => x.MeetingId == meetingId).ToListAsync();
        }

        /// <summary>
        /// 查询所有 UserId为userId的会议，meeting表中DateTime为datetime的信息，包含meeting表
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public async Task<IEnumerable<User_Meeting>> Gets_UserIdDate_Async(string userId,DateTime dateTime)
        {
            return await dbContext.User_Meeting.
                 
                Include(x => x.Meeting).
                Where(x => (x.UserId == userId && EF.Functions.DateDiffDay(x.Meeting.DateTime,dateTime)==0 )).
                ToListAsync();
        }

        /// <summary>
        /// 查询UserId=userId&&meetingId=MeetingId的信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="meetingId"></param>
        /// <returns></returns>
        public async Task<User_Meeting> Get_UserIdMeetingId_Async(string userId, int meetingId)
        {
            return await dbContext.User_Meeting.FirstOrDefaultAsync(x => x.UserId == userId && x.MeetingId == meetingId);
        }


        /// <summary>
        /// 添加一条参会记录
        /// </summary>
        /// <param name="user_Meeting"></param>
        public void Add(User_Meeting user_Meeting) {
            user_Meeting.RegistrationDate = DateTime.Now;
            logger.LogDebug("添加了一条参会记录数据");
            dbContext.User_Meeting.Add(user_Meeting);
        }


        /// <summary>
        /// 判断MeetingId=meetingId&&UserId=userId的信息是否存在
        /// </summary>
        /// <param name="user_Meeting"></param>
        /// <returns></returns>
        public bool Has(User_Meeting user_Meeting) {
            logger.LogDebug("判断是否存在此条记录");
            return  dbContext.User_Meeting.Any(x => 
                x.UserId == user_Meeting.UserId && x.MeetingId == user_Meeting.MeetingId);
        }

        /// <summary>
        /// 根据id删除记录
        /// </summary>
        /// <param name="user_Meeting"></param>
        public void Del(User_Meeting user_Meeting) {
            dbContext.User_Meeting.Remove(user_Meeting);
        }



        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="user_Meeting"></param>
        public void Update(User_Meeting user_Meeting) {
            dbContext.Entry(user_Meeting).State = EntityState.Modified;
        }
    }
}
