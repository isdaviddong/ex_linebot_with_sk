using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;

namespace isRock.Template
{
    public class LineBotChatGPTWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
        [Route("api/LineBotChatGPTWebHook")]
        [HttpPost]
        public IActionResult POST()
        {
            const string AdminUserId = "👉Admin_User_ID";  
            const string ChannelAccessToken = "👉Channel_Access_Token"; 
            const string OpenAIModelName = "gpt-4-0125-preview";
            const string OpenAIApiKey = "👉OpenAIApiKey";
             
            try
            {
                //設定ChannelAccessToken
                this.ChannelAccessToken = ChannelAccessToken;
                //配合Line Verify
                if (ReceivedMessage.events == null || ReceivedMessage.events.Count() <= 0 ||
                    ReceivedMessage.events.FirstOrDefault().replyToken == "00000000000000000000000000000000") return Ok();

                //取得Line Event
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();

                // Create a new kernel builder
                var builder = Kernel.CreateBuilder()
                    .AddOpenAIChatCompletion(OpenAIModelName, OpenAIApiKey);
                //.AddAzureOpenAIChatCompletion(DeployName, Endpoint, ApiKey);
                builder.Plugins.AddFromType<LeaveRequestPlugin>(); // Add the LightPlugin to the kernel
                Kernel kernel = builder.Build();

                // Create chat history 物件，並且加入
                var history = getHistoryFromStaticRepo(LineEvent.source.userId);
                if (history == null || history.Count() <= 0)
                    history = new ChatHistory(@"你是企業的請假助理，可以協助員工進行請假，或是查詢請假天數等功能。
                 若員工需要請假，你需要蒐集請假起始日期、天數、請假事由、代理人、請假者姓名等資訊。最後呼叫 LeaveRequest Method。
                 若員工需要查詢請假天數，你需要蒐集請假者姓名，最後呼叫 GetLeaveRecordAmount Method。
                 --------------
                 * 所有對談請用正體中文回答
                 * 請以口語化的方式來回答，要適合對談機器人的角色
                ");


                // Get chat completion service
                var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

                var responseMsg = "";
                //準備回覆訊息
                if (LineEvent.type.ToLower() == "message" && LineEvent.message.type == "text")
                {
                    // Add user input
                    history.AddUserMessage(LineEvent.message.text);

                    // Enable auto function calling
                    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                    {
                        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                    };

                    // Get the response from the AI
                    var result = chatCompletionService.GetChatMessageContentAsync(
                        history,
                        executionSettings: openAIPromptExecutionSettings,
                        kernel: kernel).Result;

                    // Add the message from the agent to the chat history
                    history.AddMessage(result.Role, result.Content ?? string.Empty);
                    // Save the chat history
                    saveHistory(LineEvent.source.userId, history);
                    responseMsg = result.Content;
                }
                else if (LineEvent.type.ToLower() == "message")
                    responseMsg = $"收到 event : {LineEvent.type} type: {LineEvent.message.type} ";
                else
                    responseMsg = $"收到 event : {LineEvent.type} ";
                //回覆訊息
                this.ReplyMessage(LineEvent.replyToken, responseMsg);
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }

        static Dictionary<string, ChatHistory> ChatHistoryByUser = new Dictionary<string, ChatHistory>();

        private ChatHistory getHistoryFromStaticRepo(string UserId)
        {
            if (ChatHistoryByUser.ContainsKey(UserId))
                return ChatHistoryByUser[UserId];
            else
                return new ChatHistory();
        }

        private void saveHistory(string UserId, ChatHistory chatHistory)
        {
            if (ChatHistoryByUser.ContainsKey(UserId))
                ChatHistoryByUser[UserId] = chatHistory;
            else
                ChatHistoryByUser.Add(UserId, chatHistory);
        }
    }
}