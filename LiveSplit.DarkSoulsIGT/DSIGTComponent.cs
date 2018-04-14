using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Xml;

namespace LiveSplit.DarkSoulsIGT
{
    public class DSIGTComponent : LogicComponent
    {
        private LiveSplitState state;
        private DSIGT dsigt;
        private int localIGT;

        public override string ComponentName => "Dark Souls In-Game Timer";

        public DSIGTComponent(LiveSplitState state)
        {
            dsigt = new DSIGT();
            localIGT = 0;

            this.state = state;
            this.state.IsGameTimePaused = true;
            this.state.OnReset += (sender, value) => { Reset(); } ;
            this.state.OnStart += (sender, e) => { Reset(); };
        }

        private void Reset()
        {
            localIGT = 0;
        }

        public override void Dispose()
        {
            dsigt.Unhook();
        }

        public override XmlNode GetSettings(XmlDocument document)
        {
            return document.CreateElement("Settings");
        }

        public override System.Windows.Forms.Control GetSettingsControl(LayoutMode mode)
        {
            return null;
        }

        public override void SetSettings(XmlNode settings)
        {

        }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (state.CurrentPhase == TimerPhase.Running)
            {
                localIGT = dsigt.IGT;
            }

            state.IsGameTimePaused = true;
            state.SetGameTime(new TimeSpan(0, 0, 0, 0, localIGT));
        }
    }
}
