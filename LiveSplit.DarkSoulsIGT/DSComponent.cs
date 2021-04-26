using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Xml;
using LiveSplit.DarkSoulsIGT.Conditions;
using System.Collections.Generic;

namespace LiveSplit.DarkSoulsIGT {
    public class DSComponent : LogicComponent {
        private readonly TimerModel timerModel;
        private readonly LiveSplitState state;
        private readonly DSSettings settings;

        LinkedListNode<ConditionList> current;
        LinkedList<ConditionList> splits = new LinkedList<ConditionList>();

        public override string ComponentName => "Dark Souls & Dark Souls: Remastered In-Game Timer";

        public DSComponent(LiveSplitState state)
        {
            this.settings = new DSSettings();
            this.state = state;
            this.state.OnStart += State_OnStart;
            this.state.OnReset += State_OnReset;

            this.timerModel = new TimerModel
            {
                CurrentState = this.state
            };

            Model.Instance.OnDarkSoulsHooked += Instance_OnDarkSoulsHooked;
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

        private void State_OnStart(object sender, EventArgs e)
        {
            // TODO get the list from settings
            List<ConditionList> fromSettings = new List<ConditionList>();

            fromSettings.Add(new ConditionList(new List<Condition>()
            {
                new OnBossDied(Flags.Bosses[0]),
                new OnBossDied(Flags.Bosses[1]),
            }));

            fromSettings.Add(new ConditionList(new List<Condition>()
            {
                new OnQuitout(),
            }));

            LinkedList<ConditionList> autosplitter = new LinkedList<ConditionList>(fromSettings);

            if (autosplitter.First != null)
            {
                current = autosplitter.First;
                WaitNext();
            }

            state.IsGameTimePaused = true;
        }

        public void WaitNext()
        {
            current.Value.OnConditionListComplete += Value_OnConditionListComplete;
            current.Value.Start();
        }

        private void Value_OnConditionListComplete()
        {
            current.Value.OnConditionListComplete -= Value_OnConditionListComplete;

            if (state.CurrentPhase == TimerPhase.Running)
            {
                timerModel.Split();
            }

            if (current.Next != null)
            {
                current = current.Next;
                WaitNext();
            }
        }

        private void State_OnReset(object sender, TimerPhase value)
        {
            Model.Instance.Reset();
            current.Value.OnConditionListComplete -= Value_OnConditionListComplete;

            var enumerator = splits.GetEnumerator();
            while (enumerator.MoveNext()) enumerator.Current.Reset();

            splits.Clear();
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
