using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Xml;
using LiveSplit.DarkSoulsIGT.Conditions;
using System.Collections.Generic;

namespace LiveSplit.DarkSoulsIGT
{
    public class DSComponent : LogicComponent
    {
        private readonly TimerModel timerModel;
        private readonly LiveSplitState state;
        private readonly DSSettings settings;

        int current = 0;
        List<ConditionList> splits;

        AutosplitterUI autosplitterUI;

        public override string ComponentName => "Dark Souls & Dark Souls: Remastered In-Game Timer";


        public DSComponent(LiveSplitState state)
        {
            this.state = state;
            this.state.OnStart += State_OnStart;
            this.state.OnReset += State_OnReset;
            this.state.OnSplit += State_OnSplit;
            this.state.OnSkipSplit += State_OnSplit; // same as on split
            this.state.OnUndoSplit += State_OnUndoSplit;

            this.settings = new DSSettings(this.state);

            this.timerModel = new TimerModel
            {
                CurrentState = this.state
            };

            Model.Instance.OnDarkSoulsHooked += Instance_OnDarkSoulsHooked;

            autosplitterUI = new AutosplitterUI(settings.tblAutosplitter, state.Run);
        }

        private void Split_OnConditionListComplete()
        {
            if (state.CurrentPhase == TimerPhase.Running)
            {
                timerModel.Split();
            }
        }

        private void State_OnStart(object sender, EventArgs e)
        {
            current = 0;
            state.IsGameTimePaused = true;

            // TODO get the list from settings
            //List<ConditionList> fromSettings = new List<ConditionList>();
            List<ConditionList> fromSettings = autosplitterUI.splitConditions;

            fromSettings.Add(new ConditionList(new List<Condition>()
            {
                new OnBossDied(Flags.Bosses[0]),
                new OnBossDied(Flags.Bosses[1]),
            }));

            fromSettings.Add(new ConditionList(new List<Condition>()
            {
                new OnQuitout(),
            }));

            splits = fromSettings;

            foreach (var split in splits)
            {
                split.OnConditionListComplete += Split_OnConditionListComplete; ;
            }

            splits[current].Start();
        }


        private void State_OnReset(object sender, TimerPhase value)
        {
            Model.Instance.Reset();

            foreach (var split in splits)
            {
                split.OnConditionListComplete -= Split_OnConditionListComplete;
                split.Stop();
            }

            splits.Clear();
        }

        private void State_OnUndoSplit(object sender, EventArgs e)
        {
            splits[current].Reset();
            current = state.CurrentSplitIndex;
            // Not doing Start(), current split is basically now in manual mode.
            // will come back to "autosplitter mode" with State_OnSplit
        }

        private void State_OnSplit(object sender, EventArgs e)
        {
            splits[current].Stop();
            current = state.CurrentSplitIndex;
            splits[current].Start();
        }

        private void Instance_OnDarkSoulsHooked(DarkSouls game)
        {
            game.OnInGameTimeChanged += Game_OnInGameTimeChanged; ;
        }

        private void Game_OnInGameTimeChanged(int old, int current)
        {
            if (state.CurrentPhase == TimerPhase.Running)
            {
                state.SetGameTime(new TimeSpan(0, 0, 0, 0, current));
            }
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return settings.GetSettings(document);
        }

        public override System.Windows.Forms.Control GetSettingsControl(LayoutMode mode)
        {
            return settings;
        }

        public override void SetSettings(XmlNode settings)
        {
            this.settings.SetSettings(settings);
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            Model.Instance.Update();
        }

        public override void Dispose()
        {

        }
    }
}
