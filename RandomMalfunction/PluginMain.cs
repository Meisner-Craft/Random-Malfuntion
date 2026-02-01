using Exiled.API.Features;
using System;

namespace RandomMalfunction
{
    public class PluginMain : Plugin<Config>
    {
        public static PluginMain Singleton { get; set; }

        public override string Name => "Random Malfunction";

        public override string Author => "Meisner";

        public override Version Version => new Version(1,0,0);

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted += ((EventHandlers)EventHandlers.Singleton).RoundStarted;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= ((EventHandlers)EventHandlers.Singleton).RoundStarted;
            base.OnDisabled();
        }

        
    }
}
