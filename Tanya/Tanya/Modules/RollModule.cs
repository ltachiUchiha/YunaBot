using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Tanya.Modules
{
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        [Command("roll")]
        public async Task RollCommand()
        {
            await Context.Channel.SendMessageAsync("Монета подбрасывается...");
            Random rand = new Random();
            int val = rand.Next(1, 11);
            if (val >= 1 && val <= 4)
            {
                await Context.Channel.SendMessageAsync(":full_moon: Орел!");
            }
            else if (val >= 5 && val <= 9)
            {
                await Context.Channel.SendMessageAsync(":new_moon: Решка!");
            }
            else if (val == 10)
            {
                await Context.Channel.SendMessageAsync(":last_quarter_moon: Монета упала ребром!");
            }
        }
    }
}
