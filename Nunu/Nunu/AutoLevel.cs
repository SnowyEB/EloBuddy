using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace Nunu
{
    public static class AutoLevel
    {
        public static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, Obj_AI_BaseLevelUpEventArgs args)
        {
            if (Program.MiscMenu["activateAutoLVL"].Cast<CheckBox>().CurrentValue && sender.IsMe)
                Core.DelayAction(LevelUpSpells, 150);
        }

        private static void LevelUpSpells()
        {
            if (Player.Instance.Spellbook.CanSpellBeUpgraded(SpellSlot.R))
                Player.Instance.Spellbook.LevelSpell(SpellSlot.R);

            var firstFocusSlot = GetSlotFromComboBox(Program.MiscMenu["firstFocus"].Cast<ComboBox>().CurrentValue);
            var secondFocusSlot = GetSlotFromComboBox(Program.MiscMenu["secondFocus"].Cast<ComboBox>().CurrentValue);
            var thirdFocusSlot = GetSlotFromComboBox(Program.MiscMenu["thirdFocus"].Cast<ComboBox>().CurrentValue);

            var secondSpell = Player.GetSpell(secondFocusSlot);
            var thirdSpell = Player.GetSpell(thirdFocusSlot);

            if (Player.Instance.Spellbook.CanSpellBeUpgraded(firstFocusSlot))
                Player.Instance.Spellbook.LevelSpell(firstFocusSlot);

            if (Player.Instance.Spellbook.CanSpellBeUpgraded(secondFocusSlot))
            {
                if (!thirdSpell.IsLearned && secondSpell.IsLearned)
                    Player.Instance.Spellbook.LevelSpell(thirdFocusSlot);
                Player.Instance.Spellbook.LevelSpell(secondFocusSlot);
            }

            if (Player.Instance.Spellbook.CanSpellBeUpgraded(thirdFocusSlot))
                Player.Instance.Spellbook.LevelSpell(thirdFocusSlot);
        }

        private static SpellSlot GetSlotFromComboBox(this int value)
        {
            switch (value)
            {
                case 0:
                    return SpellSlot.Q;
                case 1:
                    return SpellSlot.W;
                case 2:
                    return SpellSlot.E;
            }
            return SpellSlot.Unknown;
        }
    }
}
