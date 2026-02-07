using Exiled.API.Features;
using Exiled.API.Features.Core.Generic;
using System;

namespace RandomMalfunction
{
    public class PluginMain : Plugin<Config>
    {
        public static PluginMain Singleton { get; set; }

        public override string Name => "Random Malfunction";

        public override string Author => "Meisner";

        public override Version Version => new Version(1,0,0);


        private EventHandlers _handlers;

        public override void OnEnabled()
        {
            Singleton = this;

            _handlers = new EventHandlers();

            Exiled.Events.Handlers.Server.RoundStarted += _handlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += _handlers.OnRoundEnded;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Singleton = null;

            Exiled.Events.Handlers.Server.RoundStarted -= _handlers.OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= _handlers.OnRoundEnded;

            _handlers = null;

            base.OnDisabled();
        }


    }
}
