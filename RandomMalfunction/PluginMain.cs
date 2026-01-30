using LabApi.Loader.Features.Plugins;
using System;

namespace RandomMalfunction
{
    public class PluginMain : Plugin<Config>
    {
        public static PluginMain Singleton { get; set; }

        public override string Name => "Random Malfunction";

        public override string Description => "Do random malfunction and other";

        public override string Author => "Meisner";

        public override Version Version => new Version(1,0,0);

        public override Version RequiredApiVersion => throw new NotImplementedException();

        public override void Enable()
        {
            Singleton = this;
        }
        public override void Disable()
        {
            Singleton = null;
        }
    }
}
