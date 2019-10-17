namespace Instruments.Service
{
    using System.Threading.Tasks;
    using Common.Infrastructure.DependencyInjection;
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await Program<Startup>.Main(args);
        }
    }
}
