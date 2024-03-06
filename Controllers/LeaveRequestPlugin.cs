using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace isRock.Template
{

    public class LeaveRequestPlugin
    {
        const string AdminUserId = "👉Admin_User_ID";
        const string ChannelAccessToken = "👉Channel_Access_Token";

        [KernelFunction]
        [Description("取得今天日期")]
        public DateTime GetCurrentDate()
        {
            return DateTime.UtcNow.AddHours(8);
        }

        [KernelFunction]
        [Description("取得請假天數")]
        public int GetLeaveRecordAmount([Description("要查詢請假天數的員工名稱")] string employeeName)
        {
            isRock.LineBot.Bot bot = new LineBot.Bot(ChannelAccessToken);
            bot.PushMessage(AdminUserId, $"[action : 查詢 {employeeName} 假單]");

            if (employeeName.ToLower() == "david")
                return 3;
            else
                return 5;
        }

        [KernelFunction]
        [Description("進行請假")]
        public bool LeaveRequest([Description("請假起始日期")] DateTime 請假起始日期, [Description("請假天數")] string 天數, [Description("請假事由")] string 請假事由, [Description("代理人")] string 代理人,
        [Description("請假者姓名")] string 請假者姓名)
        {
            isRock.LineBot.Bot bot = new LineBot.Bot(ChannelAccessToken);
            bot.PushMessage(AdminUserId, $"action [建立假單:  {請假者姓名} 請假 {天數}天 從 {請假起始日期} 開始，事由為 {請假事由}，代理人 {代理人}]");

            return true;
        }
    }

}
