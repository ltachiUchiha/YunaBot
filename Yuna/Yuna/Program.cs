using System.Threading.Tasks;
using Yuna.Services;

namespace Yuna
{
    class Program
    {
        private static Task Main()
            => new DiscordService().InitializeAsync();
    }
}
