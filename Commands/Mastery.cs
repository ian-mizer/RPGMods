﻿using ProjectM;
using ProjectM.Network;
using RPGMods.Systems;
using RPGMods.Utils;
using Unity.Entities;

namespace RPGMods.Commands
{
    [Command("mastery, m", Usage = "mastery [<log> <on>|<off>]", Description = "Display your current mastery progression, or toggle the gain notification.")]
    public static class Mastery
    {
        private static EntityManager entityManager = Plugin.Server.EntityManager;
        public static bool detailedStatements = true;
        public static void Initialize(Context ctx)
        {
            if (!WeaponMasterSystem.isMasteryEnabled)
            {
                Output.CustomErrorMessage(ctx, "Weapon Mastery system is not enabled.");
                return;
            }
            var SteamID = ctx.Event.User.PlatformId;

            if (ctx.Args.Length > 1)
            {
                if (ctx.Args[0].ToLower().Equals("set") && ctx.Args.Length >= 3)
                {
                    bool isAllowed = ctx.Event.User.IsAdmin || PermissionSystem.PermissionCheck(ctx.Event.User.PlatformId, "mastery_args");
                    if (!isAllowed) return;
                    if (int.TryParse(ctx.Args[2], out int value))
                    {
                        string CharName = ctx.Event.User.CharacterName.ToString();
                        var UserEntity = ctx.Event.SenderUserEntity;
                        var CharEntity = ctx.Event.SenderCharacterEntity;
                        if (ctx.Args.Length == 4)
                        {
                            string name = ctx.Args[3];
                            if (Helper.FindPlayer(name, true, out var targetEntity, out var targetUserEntity))
                            {
                                SteamID = entityManager.GetComponentData<User>(targetUserEntity).PlatformId;
                                CharName = name;
                                UserEntity = targetUserEntity;
                                CharEntity = targetEntity;
                            }
                            else
                            {
                                Output.CustomErrorMessage(ctx, $"Could not find specified player \"{name}\".");
                                return;
                            }
                        }
                        string MasteryType = ctx.Args[1].ToLower();
                        if (MasteryType.Equals("sword")) WeaponMasterSystem.SetMastery(SteamID, WeaponType.Sword, value);
                        else if (MasteryType.Equals("none")) WeaponMasterSystem.SetMastery(SteamID, WeaponType.None, value);
                        else if (MasteryType.Equals("spear")) WeaponMasterSystem.SetMastery(SteamID, WeaponType.Spear, value);
                        else if (MasteryType.Equals("crossbow")) WeaponMasterSystem.SetMastery(SteamID, WeaponType.Crossbow, value);
                        else if (MasteryType.Equals("slashers")) WeaponMasterSystem.SetMastery(SteamID, WeaponType.Slashers, value);
                        else if (MasteryType.Equals("scythe")) WeaponMasterSystem.SetMastery(SteamID, WeaponType.Scythe, value);
                        else if (MasteryType.Equals("fishingpole")) WeaponMasterSystem.SetMastery(SteamID, WeaponType.FishingPole, value);
                        else if (MasteryType.Equals("mace")) WeaponMasterSystem.SetMastery(SteamID, WeaponType.Mace, value);
                        else if (MasteryType.Equals("axes")) WeaponMasterSystem.SetMastery(SteamID, WeaponType.Axes, value);
                        else
                        {
                            Output.InvalidArguments(ctx);
                            return;
                        }
                        Output.SendSystemMessage(ctx, $"{ctx.Args[1].ToUpper()} Mastery for \"{CharName}\" adjusted by <color=#fffffffe>{value * 0.001}%</color>");
                        Helper.ApplyBuff(UserEntity, CharEntity, Database.Buff.Buff_VBlood_Perk_Moose);
                        return;

                    }
                    else
                    {
                        Output.InvalidArguments(ctx);
                        return;
                    }
                }
                if (ctx.Args[0].ToLower().Equals("log"))
                {
                    if (ctx.Args[1].ToLower().Equals("on"))
                    {
                        Database.player_log_mastery[SteamID] = true;
                        Output.SendSystemMessage(ctx, $"Mastery gain is now logged.");
                        return;
                    }
                    else if (ctx.Args[1].ToLower().Equals("off"))
                    {
                        Database.player_log_mastery[SteamID] = false;
                        Output.SendSystemMessage(ctx, $"Mastery gain is no longer being logged.");
                        return;
                    }
                    else
                    {
                        Output.InvalidArguments(ctx);
                        return;
                    }
                }
            }
            else
            {
                bool isDataExist = Database.player_weaponmastery.TryGetValue(SteamID, out var MasteryData);
                if (!isDataExist)
                {
                    Output.CustomErrorMessage(ctx, "You haven't even tried to master anything...");
                    return;
                }

                Output.SendSystemMessage(ctx, "-- <color=#ffffffff>Weapon Mastery</color> --");


                if (ctx.Event.SenderCharacterEntity != null)
                {
                    int weapon = (int)WeaponMasterSystem.GetWeaponType(ctx.Event.SenderCharacterEntity);
                    if(weapon >= WeaponMasterSystem.masteryStats.Length)
                        Output.SendSystemMessage(ctx, $"Weapon type {weapon} beyond mastery stats weapon type limit of {WeaponMasterSystem.masteryStats.Length-1}");
                    string name = WeaponMasterSystem.typeToName((WeaponType)weapon);
                    double masteryPercent = WeaponMasterSystem.masteryDataByType((WeaponType)weapon, SteamID) * 0.001;
                    float effectiveness = 1;
                    if (Database.playerWeaponEffectiveness.TryGetValue(SteamID, out WeaponMasterEffectivenessData effectivenessData))
                        effectiveness = effectivenessData.data[weapon];
                    string print = $"{name}:<color=#fffffffe> {masteryPercent}%</color> (";
                    for (int i = 0; i < WeaponMasterSystem.masteryStats[weapon].Length; i++)
                    {
                        if (i > 0)
                            print += ",";
                        print += Helper.statTypeToString((UnitStatType)WeaponMasterSystem.masteryStats[weapon][i]);
                        print += " <color=#75FF33>";
                        print += WeaponMasterSystem.calcBuffValue(weapon, (float)masteryPercent, SteamID, i);
                        print += "</color>";
                    }
                    print += $") Effectiveness: {effectiveness * 100}%";
                    Output.SendSystemMessage(ctx, print);

                }
                else { 
                    if (detailedStatements)
                    {
                        Output.SendSystemMessage(ctx, $"Sword:<color=#fffffffe> {(double)MasteryData.Sword * 0.001}%</color> (ATK <color=#75FF33>{(MasteryData.Sword * 0.001 * 0.125).ToString("N2")}</color>, SPL <color=#75FF33>{(MasteryData.Sword * 0.001 * 0.125).ToString("N2")}</color>)");
                        Output.SendSystemMessage(ctx, $"Spear:<color=#fffffffe> {(double)MasteryData.Spear * 0.001}%</color> (ATK <color=#75FF33>{(MasteryData.Spear * 0.001 * 0.25).ToString("N2")}</color>)");
                        Output.SendSystemMessage(ctx, $"Axes:<color=#fffffffe> {(double)MasteryData.Axes * 0.001}%</color> (ATK <color=#75FF33>{(MasteryData.Axes * 0.001 * 0.125).ToString("N2")}</color>, HP <color=#75FF33>{(MasteryData.Axes * 0.001 * 0.5).ToString("N2")}</color>)");
                        Output.SendSystemMessage(ctx, $"Scythe:<color=#fffffffe> {(double)MasteryData.Scythe * 0.001}%</color> (ATK <color=#75FF33>{(MasteryData.Scythe * 0.001 * 0.125).ToString("N2")}</color>, CRIT <color=#75FF33>{(MasteryData.Scythe * 0.001 * 0.125).ToString("N2")}%</color>)");
                        Output.SendSystemMessage(ctx, $"Slashers:<color=#fffffffe> {(double)MasteryData.Slashers * 0.001}%</color> (CRIT <color=#75FF33>{(MasteryData.Slashers * 0.001 * 0.125).ToString("N2")}%</color>, MOV <color=#75FF33>{(MasteryData.Slashers * 0.001 * 0.005).ToString("N2")}</color>)");
                        Output.SendSystemMessage(ctx, $"Mace:<color=#fffffffe> {(double)MasteryData.Mace * 0.001}%</color> (HP <color=#75FF33>{(MasteryData.Mace * 0.001).ToString("N2")}</color>)");
                        Output.SendSystemMessage(ctx, $"Unarmed:<color=#fffffffe> {(double)MasteryData.None * 0.001}%</color> (ATK <color=#75FF33>{(MasteryData.None * 0.001 * 0.25).ToString("N2")}</color>, MOV <color=#75FF33>{(MasteryData.None * 0.001 * 0.01).ToString("N2")}</color>)");
                        Output.SendSystemMessage(ctx, $"Spell:<color=#fffffffe> {(double)MasteryData.Spell * 0.001}%</color> (CDR <color=#75FF33>{(WeaponMasterSystem.linearCDR ? (((MasteryData.Spell * 0.001) / ((MasteryData.Spell * 0.001) + 100.0)) * 100.0).ToString("N2") : ((MasteryData.Spell * 0.000005) * 100.0).ToString("N2"))}%</color>)");
                        Output.SendSystemMessage(ctx, $"Crossbow:<color=#fffffffe> {(double)MasteryData.Crossbow * 0.001}%</color> (CRIT <color=#75FF33>{(MasteryData.Crossbow * 0.001 * 0.25).ToString("N2")}%</color>)");

                    }
                    else
                    {


                        Output.SendSystemMessage(ctx, $"Sword:<color=#fffffffe> {(double)MasteryData.Sword * 0.001}%</color> (ATK <color=#75FF33>↑</color>, SPL <color=#75FF33>↑</color>)");
                        Output.SendSystemMessage(ctx, $"Spear:<color=#fffffffe> {(double)MasteryData.Spear * 0.001}%</color> (ATK <color=#75FF33>↑↑</color>)");
                        Output.SendSystemMessage(ctx, $"Axes:<color=#fffffffe> {(double)MasteryData.Axes * 0.001}%</color> (ATK <color=#75FF33>↑</color>, HP <color=#75FF33>↑</color>)");
                        Output.SendSystemMessage(ctx, $"Scythe:<color=#fffffffe> {(double)MasteryData.Scythe * 0.001}%</color> (ATK <color=#75FF33>↑</color>, CRIT <color=#75FF33>↑</color>)");
                        Output.SendSystemMessage(ctx, $"Slashers:<color=#fffffffe> {(double)MasteryData.Slashers * 0.001}%</color> (CRIT <color=#75FF33>↑</color>, MOV <color=#75FF33>↑</color>)");
                        Output.SendSystemMessage(ctx, $"Mace:<color=#fffffffe> {(double)MasteryData.Mace * 0.001}%</color> (HP <color=#75FF33>↑↑</color>)");
                        Output.SendSystemMessage(ctx, $"Unarmed:<color=#fffffffe> {(double)MasteryData.None * 0.001}%</color> (ATK <color=#75FF33>↑↑</color>, MOV <color=#75FF33>↑↑</color>)");
                        Output.SendSystemMessage(ctx, $"Spell:<color=#fffffffe> {(double)MasteryData.Spell * 0.001}%</color> (CD <color=#75FF33>↓↓</color>)");
                        Output.SendSystemMessage(ctx, $"Crossbow:<color=#fffffffe> {(double)MasteryData.Crossbow * 0.001}%</color> (CRIT <color=#75FF33>↑↑</color>)");
                    }
                    //Output.SendSystemMessage(ctx, $"Fishing Pole: <color=#ffffffff>{(double)MasteryData.FishingPole * 0.001}%</color> (??? ↑↑)");
                }
            }
        }
    }
}
