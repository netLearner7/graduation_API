using Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Tools
{
    public static class  sort
    {
        /// <summary>
        /// 针对Meeting集合排序 排序条件:时间为今天的数据排在前面
        /// </summary>
        /// <param name="list">Meeting集合</param>
        /// <returns></returns>
        public static IList<Meeting> todayMeetingSort(IList<Meeting> list)
        {
           
            //记录换到哪个位置
            int j = 0;
            //存储对象
            Meeting m = new Meeting();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].DateTime.Year == DateTime.Now.Year
                    && list[i].DateTime.Month == DateTime.Now.Month
                    && list[i].DateTime.Day == DateTime.Now.Day)
                {
                    m = list[j];
                    list[j] = list[i];
                    list[i] = m;
                    j++;
                }
            }
            return list;
        }


        /// <summary>
        /// 针对Meeting集合排序 排序条件:时间为今天的数据排在前面
        /// </summary>
        /// <param name="list">Meeting集合</param>
        /// <returns></returns>
        public static IList<User_Meeting> todayUserMeetingSort(IList<User_Meeting> list)
        {
            //记录换到哪个位置
            int j = 0;
            //存储对象
            User_Meeting m = new User_Meeting();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].CheckInDate.Year==DateTime.Now.Year
                    && list[i].CheckInDate.Month == DateTime.Now.Month 
                    && list[i].CheckInDate.Day == DateTime.Now.Day )
                {
                    m = list[j];
                    list[j] = list[i];
                    list[i] = m;
                    j++;
                }
            }
            return list;
        }

    }
}
