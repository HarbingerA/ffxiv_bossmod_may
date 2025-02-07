﻿namespace BossMod.Endwalker.Savage.P4S1Hesperos
{
    // state related to elegant evisceration mechanic (dual hit tankbuster)
    // TODO: consider showing some tank swap / invul hint...
    public class ElegantEvisceration : CommonComponents.CastCounter
    {
        public ElegantEvisceration() : base(ActionID.MakeSpell(AID.ElegantEviscerationSecond)) { }
    }

    [ConfigDisplay(Order = 0x141, Parent = typeof(EndwalkerConfig))]
    public class P4S1Config : CooldownPlanningConfigNode { }

    public class P4S1 : BossModule
    {
        public P4S1(WorldState ws, Actor primary) : base(ws, primary, new ArenaBoundsSquare(new(100, 100), 20)) { }
    }
}
