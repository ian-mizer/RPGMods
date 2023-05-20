﻿using RPGMods.Commands;
using RPGMods.Systems;

namespace RPGMods.Utils
{
    public static class AutoSaveSystem
    {
        //-- AutoSave is now directly hooked into the Server game save activity.
        public static void SaveDatabase()
        {
            PermissionSystem.SaveUserPermission(); //-- Nothing new to save.
            GodMode.SaveGodMode();
            /*
            SunImmunity.SaveImmunity();
            Waypoint.SaveWaypoints();
            NoCooldown.SaveCooldown();
            Speed.SaveSpeed();
            AutoRespawn.SaveAutoRespawn();
            //Kit.SaveKits();   //-- Nothing to save here for now.
            PowerUp.SavePowerUp();*/

            //-- System Related
            ExperienceSystem.SaveEXPData();
            PvPSystem.SavePvPStat();
            WeaponMasterSystem.SaveWeaponMastery();
            Bloodlines.saveBloodlines();
            BanSystem.SaveBanList();
            WorldDynamicsSystem.SaveFactionStats();
            WorldDynamicsSystem.SaveIgnoredMobs();

            Plugin.Logger.LogInfo("All database saved to JSON file.");
        }

        public static void LoadDatabase()
        {
            //-- Commands Related
            PermissionSystem.LoadPermissions();/*
            SunImmunity.LoadSunImmunity();
            Waypoint.LoadWaypoints();
            NoCooldown.LoadNoCooldown();*/
            GodMode.LoadGodMode();/*
            Speed.LoadSpeed();
            AutoRespawn.LoadAutoRespawn();
            Kit.LoadKits();
            PowerUp.LoadPowerUp();*/

            //-- System Related
            PvPSystem.LoadPvPStat();
            ExperienceSystem.LoadEXPData();
            WeaponMasterSystem.LoadWeaponMastery();
            Bloodlines.loadBloodlines();
            BanSystem.LoadBanList();
            WorldDynamicsSystem.LoadFactionStats();
            WorldDynamicsSystem.LoadIgnoredMobs();

            Plugin.Logger.LogInfo("All database is now loaded.");
        }
    }
}
