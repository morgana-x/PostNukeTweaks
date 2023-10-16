using Exiled.API.Interfaces;
using System.ComponentModel;
using UnityEngine;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.API.Enums;
using PlayerRoles;

namespace PostNukeSurfaceTweaks
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("Delay before the following changes are applied")]
        public float NukeChangesDelay { get; set; } = 5f;

        [Description("Enable changing room light colors - Whether any of the changes related to room lights will apply")]
        public bool RoomChangeColors { get; set; } = true;

        [Description("Blackout rooms before changing colors for smoother effect")]
        public bool RoomBlackout { get; set; } = true;

        [Description("Light color for rooms, leave all values to -1 for default")]
        public Color RoomColor { get; set; } = new Color(-1, -1, -1);

        [Description("Disable MTF/Chaos spawn after nuke (Respawn timer may get a bit confused!)")]
        public bool DisableRespawn { get; set; } = true;

        [Description("Whether radiation is enabled")]
        public bool RadiationEnabled { get; set; } = true;

        [Description("Delay before radiation begins after lights reset")]
        public float RadiationDelay { get; set; } = 5f;

        [Description("Radiation damage per interval")]
        public float RadiationDamage { get; set; } = 0.5f;

        [Description("Radiation damage multiplier for scps")]
        public float RadiationSCPMultiplier { get; set; } = 25;

        [Description("Should radiation only affect people on surface")]
        public bool RadiationOnlyAffectSurface { get; set; } = true;

        [Description("How often radiation affects are applied")]
        public float RadiationInterval { get; set; } = 2f;

        [Description("Radiation custom death message")]
        public string RadiationDeathMessage { get; set; } = "Died from Radiation Poisioning.";

        [Description("List of effects given from radiation")]
        public List<EffectType> RadiationEffects { get; set; } = new List<EffectType>()
        {
            EffectType.Burned,
            EffectType.Exhausted,
            EffectType.Concussed
        };

        [Description("List of roles immune to radiation")]
        public List<RoleTypeId> RadiationImmuneRoles { get; set; } = new List<RoleTypeId>()
        {
            RoleTypeId.Tutorial
        };
    }
}