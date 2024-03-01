﻿# ex_linebot_with_sk

這段程式碼是一個名為 LineBotChatGPTWebHookController 的 ASP.NET Core 控制器，它繼承自 isRock.LineBot.LineWebHookControllerBase。這個控制器主要用於處理來自 Line Bot 的 Webhook 事件。

在 POST 方法中，首先設定了一些常數，包括管理員的用戶 ID、Line Bot 的通道存取權杖、OpenAI 的模型名稱和 API 金鑰。然後，該方法嘗試處理從 Line Bot 接收到的訊息。

如果接收到的訊息事件為空，或者回覆權杖為特定值（這可能是 Line 的驗證請求），則方法會直接回傳 OK。

接著，程式碼從接收到的訊息中取得第一個事件，並建立一個新的語意核心（Semantic Kernel）實例，該實例包含了 OpenAI 的聊天完成插件和一個名為 LeaveRequestPlugin 的插件。

然後，程式碼從靜態儲存庫中取得與用戶相關的聊天歷史。如果找不到歷史記錄，則會創建一個新的聊天歷史實例。

如果接收到的事件是文字訊息，程式碼會將用戶的輸入加入聊天歷史，並從 AI 獲取回應。然後，將 AI 的回應加入聊天歷史，並儲存聊天歷史。最後，程式碼會將 AI 的回應回傳給用戶。

如果接收到的事件不是文字訊息，程式碼會回傳一個包含事件類型的訊息。

在處理過程中如果發生任何異常，程式碼會捕獲該異常，並將錯誤訊息推送給管理員。

最後，getHistoryFromStaticRepo 和 saveHistory 方法用於從靜態儲存庫中取得和儲存與特定用戶相關的聊天歷史。
