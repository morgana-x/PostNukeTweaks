using Exiled.Events.EventArgs;
using Exiled.API.Features;
using Exiled.API.Enums;
using MEC;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Respawning;
using Exiled.Events.EventArgs.Server;

namespace PostNukeSurfaceTweaks
{
    public class EventHandlers
    {
        public CoroutineHandle applyChangesHandle;
        public CoroutineHandle radiationHandle;
        public IEnumerator<float> radiationTimer()
        {
            Log.Debug("Waiting for radiation delay");
            yield return Timing.WaitForSeconds(Plugin.Instance.Config.RadiationDelay);
            Log.Debug("started radiation");
            while (Plugin.Instance.Config.RadiationEnabled) 
            {
                foreach (Player pl in Player.List) 
                {
                    float dmg = Plugin.Instance.Config.RadiationDamage;

                    if (pl.IsScp)
                    {
                        dmg = dmg * Plugin.Instance.Config.RadiationSCPMultiplier;
                    }
                    pl.Hurt(dmg, DamageType.Warhead);
                    foreach(EffectType effect in Plugin.Instance.Config.RadiationEffects)
                    {
                        pl.EnableEffect(effect, 999);
                    }
                }
                yield return Timing.WaitForSeconds(Plugin.Instance.Config.RadiationInterval);
            }
            yield break;
        }
        public List<Room> getSurfaceRooms()
        {
            List<Room> surfaceRooms = new List<Room>();
            foreach(Room r in Room.List)
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
            Log.Debug("Resetting room colors.");
            List<Room> SurfaceRooms = getSurfaceRooms();
            foreach (Room room in SurfaceRooms)
            {
                room.Color = Color.white;
                Log.Debug("Reset " + room.ToString() + "'s color");
                if (Plugin.Instance.Config.RoomColor == new Color(-1, -1, -1))
                    continue;
                room.Color = Plugin.Instance.Config.RoomColor;
                Log.Debug("Set " + room.ToString() + "'s color to " + room.Color.ToString());
            }
        }
        public IEnumerator<float> applyChanges()
        {
            Log.Debug("waiting to apply changes");

            yield return Timing.WaitForSeconds(Plugin.Instance.Config.NukeChangesDelay);

            Log.Debug("Applying changes~");


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
   
            Log.Debug("Applied changes.");
            yield break;
        }
        public void OnNukeDetonated()
        {
            Log.Debug("Nuke detonated!");
            Timing.KillCoroutines(applyChangesHandle);
            applyChangesHandle = Timing.RunCoroutine(applyChanges());
            Log.Debug("Started apply changes handle!");
        }
        public void OnWaitingForPlayers()
        {
            Timing.KillCoroutines(applyChangesHandle);
            Timing.KillCoroutines(radiationHandle);
            Log.Debug("Cleaned up coroutines.");
            List<Room> SurfaceRooms = Room.Get(ZoneType.Surface).ToList();
            foreach (Room room in SurfaceRooms)
            {
                Log.Debug(room.ToString() + "'s color is " + room.Color.ToString());
            }
        }

        public void RespawningTeam(RespawningTeamEventArgs ev)
        {
            if (!(Plugin.Instance.Config.DisableRespawn && Warhead.IsDetonated))
            {
                return;
            }
            Log.Debug("Disallowing respawn");
            ev.IsAllowed = false;
        }
    }
}