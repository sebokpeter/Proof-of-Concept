using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PoC.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PoC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSingleton<APIConnection, APIConnection>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseDefaultFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseWebSockets(new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(20)});

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            var connection = services.GetService<APIConnection>();
        

            app.Use(async (context, next) => {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                    {
                        TaskCompletionSource<object> socketFinishedTCS = new TaskCompletionSource<object>();

                        connection.SetWebSocket(webSocket, socketFinishedTCS);
                        connection.StartListening();

                        await socketFinishedTCS.Task;

                        //await Echo(context, webSocket);

                       // await SendForever(context, webSocket);
                    } 
                }
                else
                {
                    await next();
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private async Task SendForever(HttpContext context, WebSocket webSocket)
        {
            Random r = new Random();
            while (true)
            {
                int d = r.Next();
                var data = Encoding.ASCII.GetBytes(d.ToString());
                await webSocket.SendAsync(new ArraySegment<byte>(data, 0, 1), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            await webSocket.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);

        }

        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }

    internal class BackgroundSocketProcessor
    {
        private static WebSocket socket;
        private static TaskCompletionSource<object> completionSource;

        internal async static void AddSocket(WebSocket webSocket, TaskCompletionSource<object> socketFinishedTCS)
        {
            socket = webSocket;
            completionSource = socketFinishedTCS;
            await StartBroadcastAsync();
        }

        private static async Task StartBroadcastAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                byte[] data = Encoding.ASCII.GetBytes(i.ToString());
                await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, false, CancellationToken.None);
                Thread.Sleep(1000);
            }
            completionSource.TrySetResult(socket);
        }
    }
}
