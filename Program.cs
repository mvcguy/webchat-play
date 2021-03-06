using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebChatPlay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) 
        {

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

            //
            // for local env 
            // TODO: use a flag to switch code
            //
            // return Host.CreateDefaultBuilder(args)
            //     .ConfigureWebHostDefaults(webBuilder =>
            //     {
            //         webBuilder.UseKestrel(options =>
            //         {
            //             options.ListenAnyIP(5000, (c)=>{
            //                 c.UseHttps();
            //             });
            //         }).UseStartup<Startup>();
            //     });

        }
            
            
    }
}
