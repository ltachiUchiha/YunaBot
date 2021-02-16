using System.Threading.Tasks;
using Tanya.Services;

namespace Tanya
{
    class Program
    {
        private static Task Main()
        {
            return new DiscordService().InitializeAsync();
        }
    }
}
