using EloBuddy;
using EloBuddy.SDK;

namespace Nunu
{
    public static class NunuCalcs
    {
        private static AIHeroClient _Player { get { return ObjectManager.Player; } }

        public static float Q(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.True,
                (new float[] { 0, 400, 550, 700, 850, 1000 }[Program.Q.Level]));
        }

        public static float E(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (new float[] { 0, 80, 120, 160, 200, 240 }[Program.W.Level] + (0.9f * _Player.FlatMagicDamageMod)));
        }

        public static float Ignite(Obj_AI_Base target)
        {
            return ((10 + (4 * Program._Player.Level)) * 5) - ((target.HPRegenRate / 2) * 5);
        }
    }
}
