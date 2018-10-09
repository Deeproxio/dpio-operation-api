using Deeproxio.Infrastructure;
using Microsoft.AspNetCore.Hosting;

namespace Deeproxio.OperationApi
{
    public class Program
    {
		public static void Main(string[] args)
		{
			using (var server = new SelfHostServerBuilder<Startup>(args))
			{
				server.Build().Run();
			}
		}
	}
}
