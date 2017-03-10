using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace Chogath
{
    public static class ChoMethods
    {
        public enum AttackSpell
        {
            Q,
            W,
            R,
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
            else if (spell == AttackSpell.W)
                range = Program.W.Range;
            else if (spell == AttackSpell.R)
                range = Program.R.Range;

            return ObjectManager.Get<Obj_AI_Base>().OrderBy(a => a.Health).Where(a => (a.IsEnemy || a.IsMonster)
                && a.Type == type
                && a.Distance(_Player) <= range
                && !a.IsDead
                && !a.IsInvulnerable
                && a.IsValidTarget(range)
                &&
                (
                (a.Health <= ChoCalcs.Q(a) && AttackSpell.Q == spell) ||
                (a.Health <= ChoCalcs.W(a) && AttackSpell.W == spell) ||
                (a.Health <= ChoCalcs.R(a) && AttackSpell.R == spell)
                )).FirstOrDefault();
        }

        public static void Combo()
        {
            bool QCHECK = Program.ComboMenu["Q"].Cast<CheckBox>().CurrentValue;
            bool WCHECK = Program.ComboMenu["W"].Cast<CheckBox>().CurrentValue;
            bool RCHECK = Program.ComboMenu["R"].Cast<CheckBox>().CurrentValue;
            bool IgniteCHECK = Program.ComboMenu["Ignite"].Cast<CheckBox>().CurrentValue;
            bool QREADY = Program.Q.IsReady();
            bool WREADY = Program.W.IsReady();
            bool RREADY = Program.R.IsReady();

            if (RCHECK && RREADY)
            {
                var enemy = (AIHeroClient)GetEnemyKS(GameObjectType.AIHeroClient, AttackSpell.R);

                if (enemy != null)
                    Program.R.Cast(enemy);
            }

            if (QCHECK && QREADY)
            {
                var pos = GetBestQLocation(GameObjectType.AIHeroClient);

                if (pos != Vector3.Zero)
                    Program.Q.Cast(pos);
            }

            if (WCHECK && WREADY)
            {
                Vector3 pos;
                GetBestWLocationUnits(GameObjectType.AIHeroClient, out pos);

                if (pos != Vector3.Zero)
                    Program.W.Cast(pos);
                else
                {
                    var enemy = (AIHeroClient)GetEnemy(Program.W.Range, GameObjectType.AIHeroClient);

                    if (enemy != null)
                        Program.W.Cast(enemy.Position);
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
            bool QCHECK = Program.HarassMenu["Q"].Cast<CheckBox>().CurrentValue;
            bool WCHECK = Program.HarassMenu["W"].Cast<CheckBox>().CurrentValue;
            bool QREADY = Program.Q.IsReady();
            bool WREADY = Program.W.IsReady();

            if (QCHECK && QREADY)
            {
                var enemy = (AIHeroClient)GetEnemy(Program.Q.Range, GameObjectType.AIHeroClient);

                if (enemy != null)
                    Program.Q.Cast(enemy);
            }

            if (WCHECK && WREADY)
            {
                Vector3 pos;
                GetBestWLocationUnits(GameObjectType.AIHeroClient, out pos);

                if (pos != Vector3.Zero)
                    Program.W.Cast(pos);
            }
        }

        public static void LaneClear()
        {
            bool QCHECK = Program.LaneClearMenu["Q"].Cast<CheckBox>().CurrentValue;
            bool WCHECK = Program.LaneClearMenu["W"].Cast<CheckBox>().CurrentValue;
            bool RCHECK = Program.LaneClearMenu["R"].Cast<CheckBox>().CurrentValue;
            bool QREADY = Program.Q.IsReady();
            bool WREADY = Program.W.IsReady();
            bool RREADY = Program.R.IsReady();

            if (QCHECK && QREADY)
            {
                Vector3 pos;
                GetBestQLocationUnits(GameObjectType.obj_AI_Minion, out pos);

                if (pos != Vector3.Zero)
                    Program.Q.Cast(pos);
            }

            if (WCHECK && WREADY)
            {
                Vector3 pos;
                GetBestWLocationUnits(GameObjectType.obj_AI_Minion, out pos);

                if (pos != Vector3.Zero)
                    Program.W.Cast(pos);
                else
                {
                    var enemy = (Obj_AI_Minion)GetEnemy(Program.W.Range, GameObjectType.obj_AI_Minion);

                    if (enemy != null)
                        Program.W.Cast(enemy.Position);
                }
            }


            if (RCHECK && RREADY)
            {
                var enemy = (Obj_AI_Minion)GetEnemyKS(GameObjectType.obj_AI_Minion, AttackSpell.R);

                if (enemy != null)
                    Program.R.Cast(enemy);
            }
        }

        public static void LastHit()
        {
            bool QCHECK = Program.LastHitMenu["Q"].Cast<CheckBox>().CurrentValue;
            bool WCHECK = Program.LastHitMenu["W"].Cast<CheckBox>().CurrentValue;
            bool RCHECK = Program.LastHitMenu["R"].Cast<CheckBox>().CurrentValue;
            bool QREADY = Program.Q.IsReady();
            bool WREADY = Program.W.IsReady();
            bool RREADY = Program.R.IsReady();

            if (QCHECK && QREADY)
            {
                var enemy = (Obj_AI_Minion)GetEnemyKS(GameObjectType.obj_AI_Minion, AttackSpell.Q);

                if (enemy != null)
                    Program.Q.Cast(enemy.Position);
            }

            if (WCHECK && WREADY)
            {
                var enemy = (Obj_AI_Minion)GetEnemyKS(GameObjectType.obj_AI_Minion, AttackSpell.W);

                if (enemy != null)
                    Program.W.Cast(enemy.Position);
            }

            if (RCHECK && RREADY)
            {
                var enemy = (Obj_AI_Minion)GetEnemyKS(GameObjectType.obj_AI_Minion, AttackSpell.R);

                if (enemy != null)
                    Program.R.Cast(enemy);
            }
        }

        public static void Flee()
        {
            Orbwalker.MoveTo(Game.CursorPos);
        }

        public static void UseItems()
        {
            InventorySlot[] items = _Player.InventoryItems;

            foreach (InventorySlot item in items)
            {
                if (item.CanUseItem())
                {
                    if (item.Id == ItemId.Health_Potion ||item.Id == ItemId.Total_Biscuit_of_Rejuvenation
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

        public static Vector3 GetBestQLocation(GameObjectType type)
        {
            Spell.Skillshot.BestPosition pos = Program.Q.GetBestCircularCastPosition(EntityManager.Enemies.Where(a => a.Type == type && a.Distance(_Player) <= Program.Q.Range + (Program.Q.Width / 2)));
            if (pos.HitNumber >= 1)
                return pos.CastPosition;
            else
                return Vector3.Zero;
        }

        public static int GetBestQLocationUnits(GameObjectType type, out Vector3 pos)
        {
            Spell.Skillshot.BestPosition position = Program.Q.GetBestCircularCastPosition(EntityManager.Enemies.Where(a => a.Type == type && a.Distance(_Player) <= Program.Q.Range + (Program.Q.Width / 2)));
            pos = position.CastPosition;
            if (position.HitNumber >= 1)
                return position.HitNumber;
            else
                return 0;
        }

        public static int GetBestWLocationUnits(GameObjectType type, out Vector3 pos)
        {
            var sectorList = new List<Geometry.Polygon.Sector>();
            pos = Vector3.Zero;

            var minionList = EntityManager.Enemies.Where(it => !it.IsDead && it.IsValidTarget(Program.W.Range)).OrderByDescending(it => it.Distance(_Player)).ToList();
            List<AIHeroClient> championList = EntityManager.Heroes.Enemies.Where(it => !it.IsDead && it.IsValidTarget(Program.W.Range)).OrderByDescending(it => it.Distance(_Player)).ToList();

            Obj_AI_Base enemy = (type == GameObjectType.AIHeroClient) ?
                (Obj_AI_Base)championList.FirstOrDefault() : (Obj_AI_Base)minionList.FirstOrDefault();

            if (enemy == null)
                return 0;

            var Vectors = new List<Vector3>()
            {
                new Vector3(enemy.ServerPosition.X + 550, enemy.ServerPosition.Y, enemy.ServerPosition.Z),
                new Vector3(enemy.ServerPosition.X - 550, enemy.ServerPosition.Y, enemy.ServerPosition.Z),
                new Vector3(enemy.ServerPosition.X, enemy.ServerPosition.Y + 550, enemy.ServerPosition.Z),
                new Vector3(enemy.ServerPosition.X, enemy.ServerPosition.Y - 550, enemy.ServerPosition.Z),
                new Vector3(enemy.ServerPosition.X + 230, enemy.ServerPosition.Y, enemy.ServerPosition.Z),
                new Vector3(enemy.ServerPosition.X - 230, enemy.ServerPosition.Y, enemy.ServerPosition.Z),
                new Vector3(enemy.ServerPosition.X, enemy.ServerPosition.Y + 230, enemy.ServerPosition.Z),
                new Vector3(enemy.ServerPosition.X, enemy.ServerPosition.Y - 230, enemy.ServerPosition.Z),
                enemy.ServerPosition
            };

            float ANGLE = (float)(5 * Math.PI / 18);

            var sector1 = new Geometry.Polygon.Sector(_Player.Position, Vectors[0], ANGLE, 585);
            var sector2 = new Geometry.Polygon.Sector(_Player.Position, Vectors[1], ANGLE, 585);
            var sector3 = new Geometry.Polygon.Sector(_Player.Position, Vectors[2], ANGLE, 585);
            var sector4 = new Geometry.Polygon.Sector(_Player.Position, Vectors[3], ANGLE, 585);
            var sector5 = new Geometry.Polygon.Sector(_Player.Position, Vectors[4], ANGLE, 585);
            var sector6 = new Geometry.Polygon.Sector(_Player.Position, Vectors[5], ANGLE, 585);
            var sector7 = new Geometry.Polygon.Sector(_Player.Position, Vectors[6], ANGLE, 585);
            var sector8 = new Geometry.Polygon.Sector(_Player.Position, Vectors[7], ANGLE, 585);
            var sector9 = new Geometry.Polygon.Sector(_Player.Position, Vectors[8], ANGLE, 585);

            sectorList.Add(sector1);
            sectorList.Add(sector2);
            sectorList.Add(sector3);
            sectorList.Add(sector4);
            sectorList.Add(sector5);
            sectorList.Add(sector6);
            sectorList.Add(sector7);
            sectorList.Add(sector8);
            sectorList.Add(sector9);

            var CSHits = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int count = 0; count < 9; count++)
                if (type == GameObjectType.AIHeroClient)
                {
                    foreach (Obj_AI_Base champion in championList)
                        if (sectorList.ElementAt(count).IsInside(champion))
                            CSHits[count]++;
                }
                else
                {
                    foreach (Obj_AI_Base minion in minionList)
                        if (sectorList.ElementAt(count).IsInside(minion))
                            CSHits[count]++;
                }

            int i = CSHits.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;

            pos = Vectors[i];
            return CSHits[i];
        }
    }
}
