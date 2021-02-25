﻿using System;
using BepInEx;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace HenryMod
{
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "SurvivorAPI",
        "LoadoutAPI",
        "BuffAPI",
        "LanguageAPI",
        "SoundAPI",
        "EffectAPI",
        "UnlockablesAPI",
        "ResourcesAPI"
    })]

    public class HenryPlugin : BaseUnityPlugin
    {
        // if you don't change these you're giving permission to deprecate the mod-
        //  please change the names to your own stuff, thanks
        //   this shouldn't even have to be said
        public const string MODUID = "com.rob.HenryMod";
        public const string MODNAME = "HenryMod";
        public const string MODVERSION = "0.0.8";

        // a prefix for name tokens to prevent conflicts
        public const string developerPrefix = "ROB";

        // soft dependency stuff
        public static bool starstormInstalled = false;

        public static HenryPlugin instance;

        public static event Action awake;
        public static event Action start;

        // plugin constructor, ignore this
        public HenryPlugin()
        {
            awake += HenryPlugin_Load;
            start += HenryPlugin_LoadStart;
        }

        private void HenryPlugin_Load()
        {
            instance = this;

            // check for soft dependencies
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.Starstorm2")) starstormInstalled = true;

            // load assets and read config
            Modules.Assets.PopulateAssets();
            Modules.Config.ReadConfig();

            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            Modules.Survivors.Henry.CreateCharacter();

            Modules.States.RegisterStates(); // register states for networking
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens

            Hook();
        }

        private void HenryPlugin_LoadStart()
        {
            // any code you need to run in Start() goes here
        }

        public void Awake()
        {
            Action awake = HenryPlugin.awake;
            if (awake == null)
            {
                return;
            }
            awake();
        }

        public void Start()
        {
            Action start = HenryPlugin.start;
            if (start == null)
            {
                return;
            }
            start();
        }

        private void Hook()
        {
            // run hooks here, disabling one is as simple as commenting out the line
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            // a simple stat hook, adds armor after stats are recalculated
            if (self)
            {
                if (self.HasBuff(Modules.Buffs.armorBuff))
                {
                    self.armor += 300f;
                }
            }
        }
    }
}