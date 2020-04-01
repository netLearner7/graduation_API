using Core.Entity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class MeetingRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MeetingRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        /// <summary>
        /// 添加会议
        /// </summary>
        /// <param name="meeting">会议具体信息</param>
        public void addMeeting(Meeting meeting) {
             _dbContext.Meeting.Add(meeting);
        }

        /// <summary>
        /// 根据会议id查出具体信息
        /// </summary>
        /// <param name="meetingId"></param>
        /// <returns></returns>
        public async Task<Meeting> getMeeting(int meetingId) {
            return await _dbContext.Meeting.FirstOrDefaultAsync(x => x.Id == meetingId);
        }

        /// <summary>
        /// 根据用户id查该用户发起的会议
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IList<Meeting>> getMeetings(string userId) {
            return await _dbContext.Meeting.Where(x=>x.CreaterId==userId).ToListAsync();
        }

        /// <summary>
        /// 更新会议
        /// </summary>
        /// <param name="meeting"></param>
        public void UpdateMeeting(Meeting meeting)
        {
            _dbContext.Entry(meeting).State = EntityState.Modified;
        }



    }
}
