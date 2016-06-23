using System;

namespace MyHub.Tools
{
    public class TimeConverter
    {
        /// <summary>
        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="timestring">string类型的时间，可以是DateTime的string类型，也可以是Unix时间戳</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string timestring)
        {
            DateTime time = DateTime.MinValue;

            try// 如果直接转化能够转化成功，则说明是有效的DateTime格式的String 类型
            {
                time = DateTime.Parse(timestring);
                return time;
            }
            catch (Exception)// 如果是Unix时间戳格式
            {
                long timestamp = Convert.ToInt64(timestring);
                DateTime startTime = new DateTime(1970, 1, 1);
                time = startTime.AddSeconds(timestamp);
                return time.AddHours(8);// 通过直接加的方法更改到本地时区，因为默认是美国时区
            }
        }
    }
}
