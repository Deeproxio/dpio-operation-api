﻿using Deeproxio.Infrastructure.Runtime;
using Microsoft.AspNetCore.Hosting;

namespace Deeproxio.UserManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var server = new SelfHostServerBuilder<Startup>(args))
            {
                server.Build().RunAsync().GetAwaiter().GetResult();
            }
        }
    }
}
