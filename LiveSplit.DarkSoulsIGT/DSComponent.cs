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
            this.gameProcess = new DSProcess();
            this.dsigt = new DSIGT(this.gameProcess);
            this.inventoryReset = new DSInventoryReset(this.gameProcess);
            this.settings = new DSSettings();

            this.state = state;
            this.state.OnStart += State_OnStart;
            this.state.OnReset += State_OnReset;
        }

        private void State_OnStart(object sender, EventArgs e)
        {
            this.dsigt.Reset();
            this.state.IsGameTimePaused = true;

            if (settings.InventoryResetEnabled)
            {
                this.inventoryReset.ResetInventory();
            }
        }

        private void State_OnReset(object sender, TimerPhase value)
        {
            this.dsigt.Reset();
        }

        public override void Dispose()
        {
            this.gameProcess.Dispose();
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
            this.gameProcess.Update();

            if (state.CurrentPhase == TimerPhase.Running)
            {
                state.SetGameTime(new TimeSpan(0, 0, 0, 0, dsigt.IGT));
            }
        }
    }
}
