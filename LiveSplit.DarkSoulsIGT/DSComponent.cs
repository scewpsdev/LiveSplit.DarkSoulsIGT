using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Xml;

namespace LiveSplit.DarkSoulsIGT
{
    public class DSComponent : LogicComponent
    {
        private Model model;
        private LiveSplitState state;
        private DSSettings settings;

        public override string ComponentName => "Dark Souls & Dark Souls: Remastered In-Game Timer";

        public DSComponent(LiveSplitState state)
        {
            model = new Model();
            settings = new DSSettings();

            this.state = state;
            this.state.OnStart += State_OnStart;
            this.state.OnReset += State_OnReset;
        }

        private void State_OnStart(object sender, EventArgs e)
        {
            state.IsGameTimePaused = true;
            model.Reset();

            if (settings.InventoryResetEnabled)
            {
                model.ResetIndexes();
            }
        }

        private void State_OnReset(object sender, TimerPhase value)
        {
            model.Reset();
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
            // Manually refresh the model because LiveSplit is taking care of the update loop
            model.Refresh();

            if (this.state.CurrentPhase == TimerPhase.Running)
            {
                this.state.SetGameTime(new TimeSpan(0, 0, 0, 0, model.GetInGameTime()));
            }
        }

        public override void Dispose()
        {
            model.Dispose();
        }
    }
}
