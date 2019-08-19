using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Deeproxio.Domain.Models;
using Deeproxio.Persistence.Context;
using Microsoft.Extensions.Configuration;

namespace Deeproxio.Persistence
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Wait...");

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();	   

            var connectionString = configuration.GetConnectionString(nameof(DomainContext));


            Console.WriteLine(connectionString);

            Console.ReadKey();
        }
    }
}
