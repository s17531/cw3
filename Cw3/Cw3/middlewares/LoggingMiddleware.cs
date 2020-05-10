using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cw3.middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                string path = httpContext.Request.Path; 
                string querystring = httpContext.Request?.QueryString.ToString();
                string method = httpContext.Request.Method.ToString();
                string bodyStr = "";

                using (StreamReader reader
                 = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;

                }

                string FileName = "requestsLog.txt";

               byte[] data = Encoding.UTF8.GetBytes("\n"+method + " " + path + " " + bodyStr + " " + querystring);
                FileStream fs = new System.IO.FileStream(FileName, FileMode.Append);
                fs.Write(data, 0, data.Length);
                fs.Close();

 //               File.WriteAllText(FileName,method + " " + path + " " + bodyStr + " " + querystring);
            }

            await _next(httpContext);
        }


    }
}