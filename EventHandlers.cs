using Exiled.Events.EventArgs;
using Exiled.API.Features;
using Exiled.API.Enums;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Respawning;
using Exiled.Events.EventArgs.Server;
using Exiled.API.Features.DamageHandlers;
using Exiled.Events.EventArgs.Player;

namespace PostNukeSurfaceTweaks
{
    public class EventHandlers
    {
        public CoroutineHandle applyChangesHandle;
        public CoroutineHandle radiationHandle;
        public IEnumerator<float> radiationTimer()
        {
            yield return Timing.WaitForSeconds(Plugin.Instance.Config.RadiationDelay);
            while (Plugin.Instance.Config.RadiationEnabled)
            {
                foreach (Player pl in Player.List)
                {
                    if (pl.IsDead)
                    {
                        continue;
                    }
                    if (Plugin.Instance.Config.RadiationImmuneRoles.Contains(pl.Role.Type))
                    {
                        continue;
                    }
                    if (Plugin.Instance.Config.RadiationOnlyAffectSurface && (pl.Zone != ZoneType.Surface))
                    {
                        continue;
                    }
                    float dmg = Plugin.Instance.Config.RadiationDamage;

                    if (pl.IsScp)
                    {
                        dmg = dmg * Plugin.Instance.Config.RadiationSCPMultiplier;
                    }
                    if (pl.Health - dmg <= 0)
                    {
                        pl.Kill(Plugin.Instance.Config.RadiationDeathMessage);
                        continue;
                    }
                    pl.Health = pl.Health - dmg; //pl.Hurt(dmg, DamageType.Decontamination);

                    foreach (EffectType effect in Plugin.Instance.Config.RadiationEffects)
                    {
                        pl.EnableEffect(effect, Plugin.Instance.Config.RadiationInterval);
                    }
                }
                yield return Timing.WaitForSeconds(Plugin.Instance.Config.RadiationInterval);
            }
            yield break;
        }
        public List<Room> getSurfaceRooms()
        {
            List<Room> surfaceRooms = new List<Room>();
            foreach (Room r in Room.List)
            {
                if (r.Zone == ZoneType.Surface)
                {
                    surfaceRooms.Add(r);
                }
            }
            return surfaceRooms;
        }
        public void ResetRoomColors()
        {
            List<Room> SurfaceRooms = getSurfaceRooms();
            foreach (Room room in SurfaceRooms)
            {
                room.Color = Color.white;
                if (Plugin.Instance.Config.RoomColor == new Color(-1, -1, -1))
                    continue;
                room.Color = Plugin.Instance.Config.RoomColor;
                //Log.Debug("Set " + room.ToString() + "'s color to " + room.Color.ToString());
            }
        }
        public IEnumerator<float> applyChanges()
        {

            yield return Timing.WaitForSeconds(Plugin.Instance.Config.NukeChangesDelay);

            if (Plugin.Instance.Config.Debug) // temp fix
            {
                foreach (Player pl in Player.List)
                {
                    if (pl.IsNPC)
                    {
                        pl.Health = pl.MaxHealth;
                    }
                }
            }

            if (Plugin.Instance.Config.RadiationEnabled)
            {
                Timing.KillCoroutines(radiationHandle);
                radiationHandle = Timing.RunCoroutine(radiationTimer());
            }

            if (Plugin.Instance.Config.RoomChangeColors)
            {
                if (Plugin.Instance.Config.RoomBlackout)
                {
                    foreach (Room room in getSurfaceRooms())
                    {
                        room.Blackout(1, DoorLockType.None);
                    }
                    yield return Timing.WaitForSeconds(1);
                }
                ResetRoomColors();
            }
            yield break;
        }
        public void OnNukeDetonated()
        {
            Timing.KillCoroutines(applyChangesHandle);
            applyChangesHandle = Timing.RunCoroutine(applyChanges());
        }
        public void OnWaitingForPlayers()
        {
            Timing.KillCoroutines(applyChangesHandle);
            Timing.KillCoroutines(radiationHandle);
        }

        public void RespawningTeam(RespawningTeamEventArgs ev)
        {
            if (!(Plugin.Instance.Config.DisableRespawn && Warhead.IsDetonated))
            {
                return;
            }
            ev.IsAllowed = false;
        }
    }
}