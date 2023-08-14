using Exiled.API.Features;
using Exiled.Events.Handlers;
using MEC;
namespace PostNukeSurfaceTweaks
{
    public sealed class Plugin : Plugin<Config>
    {
        public override string Author => "morgana";

        public override string Name => "PostNukeSurfaceTweaks";

        public override string Prefix => Name;

        public static Plugin Instance;

        private EventHandlers _handlers;

        public override void OnEnabled()
        {
            Instance = this;

            RegisterEvents();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();

            Instance = null;

            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            _handlers = new EventHandlers();
            Exiled.Events.Handlers.Warhead.Detonated += _handlers.OnNukeDetonated;
            Exiled.Events.Handlers.Server.RespawningTeam += _handlers.RespawningTeam;
            Exiled.Events.Handlers.Server.WaitingForPlayers += _handlers.OnWaitingForPlayers;
        }

        private void UnregisterEvents()
        {
            Exiled.Events.Handlers.Warhead.Detonated -= _handlers.OnNukeDetonated;
            Exiled.Events.Handlers.Server.RespawningTeam -= _handlers.RespawningTeam;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= _handlers.OnWaitingForPlayers;
            Timing.KillCoroutines(_handlers.applyChangesHandle, _handlers.radiationHandle);
            
            _handlers = null;
        }
    }
}