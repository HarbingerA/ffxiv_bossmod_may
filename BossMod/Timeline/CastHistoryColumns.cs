﻿using System.Collections.Generic;

namespace BossMod
{
    // a set of columns representing actor's action history
    public class CastHistoryColumns
    {
        private Class _class;
        private StateMachineTree _tree;
        private ActionUseColumn _autoAttacks;
        private ActionUseColumn _animLock;
        private List<ActionUseColumn> _columns = new();

        private float _trackWidth = 10;

        public CastHistoryColumns(Timeline timeline, Class @class, StateMachineTree tree, List<int> phaseBranches)
        {
            _class = @class;
            _tree = tree;

            _autoAttacks = timeline.AddColumn(new ActionUseColumn(timeline, tree, phaseBranches));
            _autoAttacks.Width = _trackWidth;

            _animLock = timeline.AddColumn(new ActionUseColumn(timeline, tree, phaseBranches));
            _animLock.Width = _trackWidth;

            foreach (var track in AbilityDefinitions.Classes[@class].Tracks)
            {
                var col = timeline.AddColumn(new ActionUseColumn(timeline, tree, phaseBranches));
                col.Width = _trackWidth;
                _columns.Add(col);
            }
        }

        public void AddEvent(ActionID aid, ActionUseColumn.Event ev)
        {
            if (aid == CommonRotation.IDAutoAttack)
            {
                _autoAttacks.Events.Add(ev);
            }
            else
            {
                var def = AbilityDefinitions.Classes[_class].Abilities.GetValueOrDefault(aid);
                if (def == null)
                    ev.Color = 0xff0000ff;

                _animLock.Events.Add(ev);
                _animLock.Entries.Add(new(ev.AttachNode, ev.Delay, 0, 0, def?.AnimLock ?? 0.6f, aid.ToString()));

                if (def != null)
                {
                    var col = _columns[def.CooldownTrack];
                    col.Events.Add(ev);
                    if (def.Cooldown > 0)
                        col.Entries.Add(new(ev.AttachNode, ev.Delay, 0, def.EffectDuration, def.Cooldown, aid.ToString()));
                }
            }
        }
    }
}
