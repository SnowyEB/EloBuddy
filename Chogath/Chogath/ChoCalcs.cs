using EloBuddy;
using EloBuddy.SDK;

namespace Chogath
{
    public static class ChoCalcs
    {
        private static AIHeroClient _Player { get { return ObjectManager.Player; } }

        public static float Q(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (new float[] { 0, 80, 135, 190, 245, 305 }[Program.Q.Level] + (1.0f * _Player.FlatMagicDamageMod)));
        }

        public static float W(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (new float[] { 0, 75, 125, 175, 225, 275 }[Program.W.Level] + (0.7f * _Player.FlatMagicDamageMod)));
        }

        public static float R(Obj_AI_Base target)
        {
            return _Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (target is Obj_AI_Minion) ? (new float[] { 0, 1000, 1000, 1000 }[Program.R.Level] + (0.7f * _Player.FlatMagicDamageMod))
                                          : (new float[] { 0, 300, 475, 650 }[Program.R.Level] + (0.7f * _Player.FlatMagicDamageMod)));
        }

        public static float Ignite(Obj_AI_Base target)
        {
            return ((10 + (4 * Program._Player.Level)) * 5) - ((target.HPRegenRate / 2) * 5);
        }
    }
}
