using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Xml;

namespace LiveSplit.DarkSoulsIGT
{
    public class DSComponent : LogicComponent
    {
        private LiveSplitState state;
        private DSProcess gameProcess;
        private DSIGT dsigt;
        private DSInventoryReset inventoryReset;
        private DSSettings settings;

        public override string ComponentName => "Dark Souls & Dark Souls: Remastered In-Game Timer";

        public DSComponent(LiveSplitState state)
        {
            gameProcess = new DSProcess();
            dsigt = new DSIGT(gameProcess);
            inventoryReset = new DSInventoryReset(gameProcess);
            settings = new DSSettings();

            this.state = state;
            this.state.IsGameTimePaused = true;
            this.state.OnStart += State_OnStart;
            this.state.OnReset += State_OnReset;
        }

        private void State_OnStart(object sender, EventArgs e)
        {
            dsigt.Reset();

            if (settings.InventoryResetEnabled)
            {
                inventoryReset.ResetInventory();
            }
        }

        private void State_OnReset(object sender, TimerPhase value)
        {
            dsigt.Reset();
        }

        public override void Dispose()
        {
            gameProcess.Dispose();
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
            // https://github.com/LiveSplit/LiveSplit/issues/1302
            state.IsGameTimePaused = true;

            gameProcess.Update();

            if (state.CurrentPhase == TimerPhase.Running)
            {
                state.SetGameTime(new TimeSpan(0, 0, 0, 0, dsigt.IGT));
            }
        }
    }
}
