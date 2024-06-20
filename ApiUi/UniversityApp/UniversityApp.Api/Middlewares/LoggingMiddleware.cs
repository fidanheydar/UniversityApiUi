﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text;

namespace UniversityApp.Api.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await Console.Out.WriteLineAsync($"{context.Request.Method} - {context.Request.GetDisplayUrl()} - {DateTime.UtcNow.ToString("ddd-MM-yyyy HH:mm:ss")}");
           
            context.Request.EnableBuffering();
            //var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            //await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            //var requestContent = Encoding.UTF8.GetString(buffer);
            //await Console.Out.WriteLineAsync(requestContent);

            var requestBodyStr = await new StreamReader(context.Request.Body, System.Text.Encoding.UTF8, true).ReadToEndAsync();

            context.Request.Body.Position = 0;

            var path = Path.Combine("wwwroot/logs", $"log-{DateTime.UtcNow.ToString("ddMMyyyy")}.txt");

            using(var sw  = new StreamWriter(path,true))
            {
                sw.WriteLine("\n=======================REQUEST===========================");
                await sw.WriteLineAsync($"{context.Request.Method} - {context.Request.GetDisplayUrl()} - {DateTime.UtcNow.ToString("ddd-MM-yyyy HH:mm:ss")}");
                await sw.WriteLineAsync(requestBodyStr);
            }

          
            var originalBody = context.Response.Body;
            
            using(var ms = new  MemoryStream())
            {

                context.Response.Body = ms;
                await _next(context);

                context.Response.Body.Position = 0;
                var responseBodyStr = await new StreamReader(ms, System.Text.Encoding.UTF8, true).ReadToEndAsync();

                context.Response.Body.Position = 0;
                await context.Response.Body.CopyToAsync(originalBody);
                context.Response.Body = originalBody;

                using (var sw = new StreamWriter(path, true))
                {
                    sw.WriteLine("\n=======================RESPONSE===========================");
                    await sw.WriteLineAsync($"{context.Response.StatusCode} - {context.Request.GetDisplayUrl()} - {DateTime.UtcNow.ToString("ddd-MM-yyyy HH:mm:ss")}");
                    await sw.WriteLineAsync(responseBodyStr);
                }
            }


            await Console.Out.WriteLineAsync($"{context.Response.StatusCode} - {context.Request.GetDisplayUrl()} - {DateTime.UtcNow.ToString("ddd-MM-yyyy HH:mm:ss")}");
        }
    }
}
