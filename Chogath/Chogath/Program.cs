﻿using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System;
using System.Collections.Generic;

namespace Chogath
{
    class Program
    {
        public static Menu GlobalMenu, ComboMenu, HarassMenu, LaneClearMenu, LastHitMenu, MiscMenu, DrawingsMenu;
        public static Spell.Skillshot Q;
        public static Spell.Skillshot W;
        public static Spell.Targeted R;
        public static Spell.Targeted Ignite;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static AIHeroClient _Player { get { return ObjectManager.Player; } }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Chogath)
                return;

            Bootstrap.Init(null);

            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Circular, 250, null, 625, DamageType.Magical);
            W = new Spell.Skillshot(SpellSlot.W, 700, SkillShotType.Cone, 250, null, null, DamageType.Magical)
            {
                ConeAngleDegrees = 50
            };
            R = new Spell.Targeted(SpellSlot.R, 175);
            SpellDataInst Sum1 = _Player.Spellbook.GetSpell(SpellSlot.Summoner1);
            SpellDataInst Sum2 = _Player.Spellbook.GetSpell(SpellSlot.Summoner2);
            if (Sum1.Name == "summonerdot")
                Ignite = new Spell.Targeted(SpellSlot.Summoner1, 600);
            else if (Sum2.Name == "summonerdot")
                Ignite = new Spell.Targeted(SpellSlot.Summoner2, 600);

            GlobalMenu = MainMenu.AddMenu("Snowys Cho", "SnowysCho");

            ComboMenu = GlobalMenu.AddSubMenu("Combo", "combomenu");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("R", new CheckBox("Use R"));
            ComboMenu.Add("Ignite", new CheckBox("Use Ignite"));

            LaneClearMenu = GlobalMenu.AddSubMenu("Lane Clear", "laneclearmenu");
            LaneClearMenu.AddGroupLabel("Lane Clear Settings");
            LaneClearMenu.Add("Q", new CheckBox("Use Q"));
            LaneClearMenu.Add("W", new CheckBox("Use W"));
            LaneClearMenu.Add("R", new CheckBox("Use R", false));

            HarassMenu = GlobalMenu.AddSubMenu("Harass", "harassmanu");
            HarassMenu.AddGroupLabel("Harass Settings");
            HarassMenu.Add("Q", new CheckBox("Use Q"));
            HarassMenu.Add("W", new CheckBox("Use W"));

            LastHitMenu = GlobalMenu.AddSubMenu("Last Hit", "lasthitmenu");
            LastHitMenu.AddGroupLabel("Last Hit Settings");
            LastHitMenu.Add("Q", new CheckBox("Use Q"));
            LastHitMenu.Add("W", new CheckBox("Use W", false));
            LastHitMenu.Add("R", new CheckBox("Use R", false));

            DrawingsMenu = GlobalMenu.AddSubMenu("Drawings", "drawingsmenu");
            DrawingsMenu.AddGroupLabel("Drawings Settings");
            DrawingsMenu.Add("Q", new CheckBox("Draw Q"));
            DrawingsMenu.Add("W", new CheckBox("Draw W"));
            DrawingsMenu.Add("R", new CheckBox("Draw R"));

            MiscMenu = GlobalMenu.AddSubMenu("Misc", "miscmenu");
            MiscMenu.AddGroupLabel("Misc Settings");
            MiscMenu.Add("HealthPotions", new CheckBox("Auto-Use Health Potions"));
            MiscMenu.AddGroupLabel("Auto Level UP");
            MiscMenu.Add("activateAutoLVL", new CheckBox("Activate Auto Leveler", false));
            MiscMenu.AddLabel("The Auto Leveler will always Focus R than the rest of the Spells");
            MiscMenu.Add("firstFocus", new ComboBox("1 Spell to Focus", new List<string> { "Q", "W", "E" }));
            MiscMenu.Add("secondFocus", new ComboBox("2 Spell to Focus", new List<string> { "Q", "W", "E" }, 1));
            MiscMenu.Add("thirdFocus", new ComboBox("3 Spell to Focus", new List<string> { "Q", "W", "E" }, 2));

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnLevelUp += AutoLevel.Obj_AI_Base_OnLevelUp;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawingsMenu["Q"].Cast<CheckBox>().CurrentValue && Q.IsLearned)
                Drawing.DrawCircle(_Player.Position, Q.Range, System.Drawing.Color.BlueViolet);
            if (DrawingsMenu["W"].Cast<CheckBox>().CurrentValue && W.IsLearned)
                Drawing.DrawCircle(_Player.Position, W.Range, System.Drawing.Color.BlueViolet);
            if (DrawingsMenu["R"].Cast<CheckBox>().CurrentValue && R.IsLearned)
                Drawing.DrawCircle(_Player.Position, R.Range, System.Drawing.Color.BlueViolet);
        }

        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                ChoMethods.Combo();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                ChoMethods.LastHit();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                ChoMethods.Harass();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                ChoMethods.LaneClear();
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
                ChoMethods.Flee();
            if (MiscMenu["HealthPotions"].Cast<CheckBox>().CurrentValue)
                ChoMethods.UseItems();
        }
    }
}
