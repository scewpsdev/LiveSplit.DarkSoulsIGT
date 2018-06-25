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

        public override string ComponentName => "Dark Souls In-Game Timer";

        public DSComponent(LiveSplitState state)
        {
            gameProcess = new DSProcess();
            dsigt = new DSIGT(gameProcess);
            inventoryReset = new DSInventoryReset(gameProcess);
            settings = new DSSettings();

            this.state = state;
            this.state.IsGameTimePaused = true;
            this.state.OnReset += (sender, value) => { Reset(); };
            this.state.OnStart += (sender, e) => { Reset(); };
        }

        private void Reset()
        {
            dsigt.Reset();

            if (settings.InventoryResetEnabled)
            {
                inventoryReset.ResetInventory();
            }
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
            gameProcess.Update();
            if (state.CurrentPhase == TimerPhase.Running)
            {
                state.SetGameTime(new TimeSpan(0, 0, 0, 0, dsigt.IGT));
            }
        }
    }
}
