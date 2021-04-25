using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Xml;

namespace LiveSplit.DarkSoulsIGT
{
    public class DSComponent : LogicComponent
    {
        private readonly int IGT_START_THRESHOLD = 100; // ms

        private readonly TimerModel timerModel;
        private readonly Model model;
        private readonly LiveSplitState state;
        private readonly DSSettings settings;
        private bool resetIndexesLatch;

        public override string ComponentName => "Dark Souls & Dark Souls: Remastered In-Game Timer";

        public DSComponent(LiveSplitState state)
        {
            this.model = new Model();
            this.settings = new DSSettings();

            this.state = state;
            this.state.OnStart += State_OnStart;
            this.state.OnReset += State_OnReset;
            this.resetIndexesLatch = false;

            this.timerModel = new TimerModel
            {
                CurrentState = this.state
            };

            this.model.Start();
        }


        private void State_OnStart(object sender, EventArgs e)
        {
            this.state.IsGameTimePaused = this.settings.UseGameTime;
            this.model.Reset();
        }

        private void State_OnReset(object sender, TimerPhase value)
        {
            this.resetIndexesLatch = false;
            this.model.Reset();
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return this.settings.GetSettings(document);
        }

        public override System.Windows.Forms.Control GetSettingsControl(LayoutMode mode)
        {
            return this.settings;
        }

        public override void SetSettings(XmlNode settings)
        {
            this.settings.SetSettings(settings);
        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            int memoryIGT = this.model.GetMemoryInGameTime();
            this.state.IsGameTimePaused = this.settings.UseGameTime;

            if (this.settings.UseGameTime && this.state.CurrentPhase == TimerPhase.Running)
            {
                this.state.SetGameTime(new TimeSpan(0, 0, 0, 0, this.model.GetInGameTime()));
            }

            if (memoryIGT > 0 && memoryIGT < IGT_START_THRESHOLD)
            {
                if (this.settings.StartTimerAutomatically && this.state.CurrentPhase == TimerPhase.NotRunning)
                {
                    this.timerModel.Start();
                }

                if (this.settings.InventoryResetEnabled && !this.resetIndexesLatch)
                {
                    this.model.ResetIndexes();
                    this.resetIndexesLatch = true;
                }
            }
        }

        public override void Dispose()
        {
            this.model.Stop();
            this.model.Dispose();
        }
    }
}
