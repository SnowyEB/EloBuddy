using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Nunu
{
    public static class NunuMethods
    {
        public enum AttackSpell
        {
            Q,
            E,
            Ignite
        }
        private static AIHeroClient _Player { get { return ObjectManager.Player; } }

        public static Obj_AI_Base GetEnemy(float range, GameObjectType type)
        {
            return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health).Where(a => a.IsEnemy
            && a.Type == type
            && a.Distance(_Player) <= range
            && !a.IsDead
            && !a.IsInvulnerable
            && a.IsValidTarget(range)).FirstOrDefault();
        }
        public static Obj_AI_Base GetEnemyKS(GameObjectType type, AttackSpell spell)
        {
            var range = 0f;
            if (spell == AttackSpell.Q)
                range = Program.Q.Range;
            else if (spell == AttackSpell.E)
                range = Program.E.Range;

            return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health).Where(a => (a.IsEnemy || a.IsMonster)
                && a.Type == type
                && a.Distance(_Player) <= range
                && !a.IsDead
                && !a.IsInvulnerable
                && a.IsValidTarget(range)
                &&
                (
                (a.Health <= NunuCalcs.Q(a) && AttackSpell.Q == spell) ||
                (a.Health <= NunuCalcs.E(a) && AttackSpell.E == spell)
                )).FirstOrDefault();
        }

        public static bool ChannelingR()
        {
            return Player.Instance.Spellbook.IsChanneling || Player.Instance.HasBuff("Absolute Zero");
        }

        public static void Combo()
        {
            bool WCHECK = Program.ComboMenu["W"].Cast<CheckBox>().CurrentValue;
            bool ECHECK = Program.ComboMenu["E"].Cast<CheckBox>().CurrentValue;
            bool RCHECK = Program.ComboMenu["R"].Cast<CheckBox>().CurrentValue;
            int MINR = Program.ComboMenu["MinR"].Cast<Slider>().CurrentValue;
            bool IgniteCHECK = Program.ComboMenu["Ignite"].Cast<CheckBox>().CurrentValue;
            bool WREADY = Program.W.IsReady();
            bool EREADY = Program.E.IsReady();
            bool RREADY = Program.R.IsReady();
            bool isUlting = ChannelingR();

            if(isUlting)
            {
                var enemy = (AIHeroClient)GetEnemy(Program.R.Range, GameObjectType.AIHeroClient);
                if(_Player.CountEnemyChampionsInRange(Program.R.Range - 75) < MINR)
                {
                    Orbwalker.DisableMovement = false;
                    if(enemy != null)
                        Player.IssueOrder(GameObjectOrder.MoveTo, enemy);
                    Orbwalker.DisableAttacking = false;
                }
            }
            else
            {
                Orbwalker.DisableMovement = false;
                Orbwalker.DisableAttacking = false;
            }

            if(RCHECK && RREADY)
            {
                if(_Player.CountEnemyChampionsInRange(Program.R.Range - 75) >= MINR)
                {
                    Orbwalker.DisableAttacking = true;
                    Orbwalker.DisableMovement = true;
                    Program.R.Cast();
                    return;
                }
            }

            if(ECHECK && EREADY && !isUlting)
            {
                var enemy = (AIHeroClient)GetEnemy(Program.E.Range, GameObjectType.AIHeroClient);
                if(enemy != null)
                {
                    Program.E.Cast(enemy);
                }
            }

            if(WCHECK && WREADY && !isUlting)
            {
                var ally = EntityManager.Heroes.Allies.OrderByDescending(a => a.TotalAttackDamage).FirstOrDefault(b => b.Distance(Player.Instance) < 1000);
                if(ally != null && _Player.CountEnemyChampionsInRange(1500) > 0)
                {
                    Program.W.Cast(ally);
                }
                else if(_Player.CountEnemyChampionsInRange(1500) > 0)
                {
                    Program.W.Cast(_Player);
                }
            }

            if (IgniteCHECK && Program.Ignite != null && Program.Ignite.IsReady())
            {
                var enemy = (AIHeroClient)GetEnemy(Program.Ignite.Range, GameObjectType.AIHeroClient);

                if (enemy != null)
                    Program.Ignite.Cast(enemy);
            }
        }

        public static void Harass()
        {
            bool WCHECK = Program.HarassMenu["W"].Cast<CheckBox>().CurrentValue;
            bool ECHECK = Program.HarassMenu["E"].Cast<CheckBox>().CurrentValue;
            bool WREADY = Program.W.IsReady();
            bool EREADY = Program.E.IsReady();

            if (ECHECK && EREADY)
            {
                var enemy = (AIHeroClient)GetEnemy(Program.E.Range, GameObjectType.AIHeroClient);
                if (enemy != null)
                {
                    Program.E.Cast(enemy);
                }
            }

            if (WCHECK && WREADY)
            {
                var ally = EntityManager.Heroes.Allies.OrderByDescending(a => a.TotalAttackDamage).FirstOrDefault(b => b.Distance(Player.Instance) < 1000);
                if (ally != null && _Player.CountEnemyChampionsInRange(1500) > 0)
                {
                    Program.W.Cast(ally);
                }
                else if (_Player.CountEnemyChampionsInRange(1500) > 0)
                {
                    Program.W.Cast(_Player);
                }
            }
        }

        public static void LaneClear()
        {
            bool QCHECK = Program.LaneClearMenu["Q"].Cast<CheckBox>().CurrentValue;
            bool WCHECK = Program.LaneClearMenu["W"].Cast<CheckBox>().CurrentValue;
            bool ECHECK = Program.LaneClearMenu["E"].Cast<CheckBox>().CurrentValue;
            bool QREADY = Program.Q.IsReady();
            bool WREADY = Program.W.IsReady();
            bool EREADY = Program.E.IsReady();

            if(QCHECK && QREADY)
            {
                var enemy = (Obj_AI_Minion)GetEnemy(Program.Q.Range, GameObjectType.obj_AI_Minion);
                if(enemy != null)
                {
                    Program.Q.Cast(enemy);
                }
            }

            if (ECHECK && EREADY)
            {
                var enemy = (Obj_AI_Minion)GetEnemy(Program.E.Range, GameObjectType.obj_AI_Minion);
                if (enemy != null)
                {
                    Program.E.Cast(enemy);
                }
            }

            if (WCHECK && WREADY)
            {
                var ally = EntityManager.Heroes.Allies.OrderByDescending(a => a.TotalAttackDamage).FirstOrDefault(b => b.Distance(Player.Instance) < 1000);
                if (ally != null && _Player.CountEnemyChampionsInRange(1500) > 0)
                {
                    Program.W.Cast(ally);
                }
                else if(ally != null)
                {
                    Program.W.Cast(ally);
                }
                else
                {
                    Program.W.Cast(_Player);
                }
            }
        }

        public static void LastHit()
        {
            bool QCHECK = Program.LastHitMenu["Q"].Cast<CheckBox>().CurrentValue;
            bool ECHECK = Program.LastHitMenu["E"].Cast<CheckBox>().CurrentValue;
            bool QREADY = Program.Q.IsReady();
            bool EREADY = Program.E.IsReady();

            if (QCHECK && QREADY)
            {
                var enemy = (Obj_AI_Minion)GetEnemyKS(GameObjectType.obj_AI_Minion, AttackSpell.Q);
                if (enemy != null)
                {
                    Program.Q.Cast(enemy);
                }
            }

            if (ECHECK && EREADY)
            {
                var enemy = (Obj_AI_Minion)GetEnemyKS(GameObjectType.obj_AI_Minion, AttackSpell.E);
                if (enemy != null)
                {
                    Program.E.Cast(enemy);
                }
            }
        }

        public static void Flee()
        {
            bool WREADY = Program.W.IsReady();
            
            if (WREADY)
            {
                var ally = EntityManager.Heroes.Allies.OrderByDescending(a => a.TotalAttackDamage).FirstOrDefault(b => b.Distance(Player.Instance) < 1000);
                if (ally != null)
                {
                    Program.W.Cast(ally);
                }
                else
                {
                    Program.W.Cast(_Player);
                }
            }

            Orbwalker.MoveTo(Game.CursorPos);
        }

        public static void UseItems()
        {
            InventorySlot[] items = _Player.InventoryItems;

            foreach (InventorySlot item in items)
            {
                if (item.CanUseItem())
                {
                    if (item.Id == ItemId.Health_Potion || item.Id == ItemId.Total_Biscuit_of_Rejuvenation
                        && _Player.Health <= (_Player.MaxHealth * 0.45)
                        && !_Player.IsRecalling()
                        && _Player.CountEnemyChampionsInRange(2000) <= 1
                        && !_Player.IsInShopRange()
                        && !_Player.HasBuff("RegenerationPotion"))
                    {
                        item.Cast();
                    }
                }
            }
        }
    }
}
