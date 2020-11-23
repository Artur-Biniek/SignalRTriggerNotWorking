using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace RealTimeTestZero
{
    public static class Function1
    {

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
                   [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
                   [SignalRConnectionInfo(HubName = "chat")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }


        [FunctionName("send")]
        public async static Task<IActionResult> SendMessage(
            [HttpTrigger(methods: "get", Route = "chat/send")] HttpRequest req,
            [SignalR(HubName = "chat", ConnectionStringSetting = "AzureSignalRConnectionString")] IAsyncCollector<SignalRMessage> connectionInfo)
        {
            string message = req.Query["message"];

            await connectionInfo.AddAsync(new SignalRMessage()
            {
                Target = "newMessage",
                Arguments = new[] { message }
            });


            return new OkResult();
        }


        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-signalr-service-trigger?tabs=csharp#configuration
        [FunctionName("OnConnected")]
        public static void OnConnected(
            [SignalRTrigger(hubName:"chat", category:"connections", @event: "connected")] InvocationContext ctx,
            ILogger logger
            )
        {
            // This is not executed when browser client connectes to the hub.
            logger.LogWarning($"Client connected {ctx.ConnectionId}");
        }

        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-signalr-service-trigger?tabs=csharp#configuration
        [FunctionName("OnDisconnected")]
        public static void OnDisconnected(
            [SignalRTrigger(hubName: "chat", category: "connections", @event: "disconnected")] InvocationContext ctx,
            ILogger logger
            )
        {
            // This is not executed when browser client disconnectes from the hub.
            logger.LogWarning($"Client connected {ctx.ConnectionId}");
        }
    }
}