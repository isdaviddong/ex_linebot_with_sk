# Example : LINE Bot With Semantic Kernel

這段程式碼是一個名為 LineBotChatGPTWebHookController 的 ASP.NET Core 控制器，它繼承自 isRock.LineBot.LineWebHookControllerBase。這個控制器主要用於處理來自 Line Bot 的 Webhook 事件。

在 POST 方法中，首先設定了一些常數，包括管理員的用戶 ID、Line Bot 的通道存取權杖、OpenAI 的模型名稱和 API 金鑰。然後，該方法嘗試處理從 Line Bot 接收到的訊息。

如果接收到的訊息事件為空，或者回覆權杖為特定值（這可能是 Line 的驗證請求），則方法會直接回傳 OK。

接著，程式碼從接收到的訊息中取得第一個事件，並建立一個新的語意核心（Semantic Kernel）實例，該實例包含了 OpenAI 的聊天完成插件和一個名為 LeaveRequestPlugin 的插件。

然後，程式碼從靜態儲存庫中取得與用戶相關的聊天歷史。如果找不到歷史記錄，則會創建一個新的聊天歷史實例。

如果接收到的事件是文字訊息，程式碼會將用戶的輸入加入聊天歷史，並從 AI 獲取回應。然後，將 AI 的回應加入聊天歷史，並儲存聊天歷史。最後，程式碼會將 AI 的回應回傳給用戶。

如果接收到的事件不是文字訊息，程式碼會回傳一個包含事件類型的訊息。

在處理過程中如果發生任何異常，程式碼會捕獲該異常，並將錯誤訊息推送給管理員。

最後，getHistoryFromStaticRepo 和 saveHistory 方法用於從靜態儲存庫中取得和儲存與特定用戶相關的聊天歷史。

### LeaveRequestPlugin

這段程式碼定義了一個名為 LeaveRequestPlugin 的類別，該類別包含了三個方法，用於處理與員工請假相關的功能。

首先，我們看到兩個常數 AdminUserId 和 ChannelAccessToken，這兩個常數分別代表 Line Bot 管理員的用戶 ID 和 Line Bot 的通道存取權杖。

接著，我們看到三個方法，每個方法都被標記為 KernelFunction，這表示這些方法可以被 Semantic Kernel 使用。

第一個方法 GetCurrentDate 用於取得當前日期，並將 UTC 時間加上 8 小時以轉換為台灣時間。

第二個方法 GetLeaveRecordAmount 用於取得指定員工的請假天數。該方法首先創建一個新的 Line Bot 實例，並向管理員發送一條包含員工名稱的訊息。然後，如果員工名稱為 "david"，則返回 3，否則返回 5。這裡的返回值似乎是硬編碼的，實際應用中可能需要從資料庫或其他來源獲取實際的請假天數。

第三個方法 LeaveRequest 用於處理請假請求。該方法接收五個參數，分別是請假起始日期、請假天數、請假事由、代理人和請假者姓名。該方法首先創建一個新的 Line Bot 實例，並向管理員發送一條包含請假詳情的訊息。然後，返回 true。這裡的返回值似乎是硬編碼的，實際應用中可能需要根據請假請求的處理結果來返回相應的值。
