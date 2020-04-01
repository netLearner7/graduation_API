using Infrastructure.Data;
using Infrastructure.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class StatisticsRepository
    {
        private readonly ApplicationDbContext dbContext;

        public StatisticsRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// 获取用户该月/所有的会议数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> getAllMeeting(string userid,bool thisMonth) {
            if (thisMonth) {
                //获取改月
                DateTime start, end;
                getMonth(0, out start, out end);
                return await dbContext.Meeting.Where(x => x.CreaterId == userid&&x.DateTime>start&&x.DateTime<end).CountAsync();

            }
            else{
                //获取所有
                return await dbContext.Meeting.Where(x => x.CreaterId == userid).CountAsync();
            }
            
        }

        /// <summary>
        /// 获取用户该月/所有的参加的会议数量
        /// </summary>
        /// <returns></returns>
        public async Task<int> getAllUserMeeting(string userid, bool thisMonth) {

            if (thisMonth) {
                //获取改月
                DateTime start, end;
                getMonth(0, out start, out end);
                return await dbContext.User_Meeting.Where(x => x.UserId == userid&&x.CheckInDate>start&&x.CheckInDate<end).CountAsync();
            }

            return await dbContext.User_Meeting.Where(x => x.UserId ==userid ).CountAsync();
        }



        /// <summary>
        ///  统计从本月开始向前number个月的会议数量
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task< StatisticsDto> MeetingAsync(int number, string userid)
        {
            DateTime start, end;
            getMonth(number,out start,out end);

            var count= await dbContext.Meeting.Where(x => x.DateTime >= start && x.DateTime <= end&&x.CreaterId==userid).CountAsync();

            return getStatisticsDto(start, count);
        }

        /// <summary>
        /// 统计从本月开始向前number个月的参加会议数量
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public async Task<StatisticsDto> User_MeetingAsync(int number, string userid)
        {
            DateTime start, end;
            getMonth(number, out start, out end);

            var count = await dbContext.User_Meeting.Where(x => x.CheckInDate >= start && x.CheckInDate <= end&&x.UserId==userid).CountAsync();
            
            return getStatisticsDto(start,count);
        }

        /// <summary>
        /// 计算年月数
        /// </summary>
        /// <param name="number">向前nuber个月</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void getMonth(int number,out DateTime start,out DateTime end) {
            int m = DateTime.Now.Month - number;
            int y = DateTime.Now.Year;

            if (m < 1)
            {
                y--;
                m = 12 + m;
            }

             start = new DateTime(y, m, 1);
             end = start.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        /// <summary>
        /// 拼装数据
        /// </summary>
        /// <param name="start"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public StatisticsDto getStatisticsDto(DateTime start,int number) { 
            return new StatisticsDto()
            {
                date = start.Year + "-" + start.Month,
                number = number
            };
        }
    }
}
