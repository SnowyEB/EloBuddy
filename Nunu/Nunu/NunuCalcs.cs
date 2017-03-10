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
                (new float[] { 0, 400, 550, 700, 850, 1000 }[Program.Q.Level] + (1.0f * _Player.FlatMagicDamageMod)));
        }

        public static float E(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (new float[] { 0, 85, 130, 175, 225, 275 }[Program.W.Level] + (1.0f * _Player.FlatMagicDamageMod)));
        }

        public static float Ignite(Obj_AI_Base target)
        {
            return ((10 + (4 * Program._Player.Level)) * 5) - ((target.HPRegenRate / 2) * 5);
        }
    }
}
