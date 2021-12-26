using LiveSplit.DarkSoulsIGT.Conditions;
using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.DarkSoulsIGT
{
    public partial class DSSettings : UserControl
    {
        private const bool DEFAULT_USE_GAME_TIME = true;
        private const bool DEFAULT_INVENTORY_RESET_ENABLED = false;
        private const bool DEFAULT_START_TIMER_AUTOMATICALLY = false;

        public bool UseGameTime { get; set; }
        public bool InventoryResetEnabled { get; set; }
        public bool StartTimerAutomatically { get; set; }

        public DSSettings(LiveSplitState state)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            this.cbxUseIGT.DataBindings.Add("Checked", this, nameof(UseGameTime), false, DataSourceUpdateMode.OnPropertyChanged);
            this.cbxInventoryReset.DataBindings.Add("Checked", this, nameof(InventoryResetEnabled), false, DataSourceUpdateMode.OnPropertyChanged);
            this.cbxStartTimer.DataBindings.Add("Checked", this, nameof(StartTimerAutomatically), false, DataSourceUpdateMode.OnPropertyChanged);

            this.UseGameTime = DEFAULT_USE_GAME_TIME;
            this.InventoryResetEnabled = DEFAULT_INVENTORY_RESET_ENABLED;
            this.StartTimerAutomatically = DEFAULT_START_TIMER_AUTOMATICALLY;
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settingsNode = document.CreateElement("Settings");
            settingsNode.AppendChild(ToElement(document, nameof(this.UseGameTime), this.UseGameTime));
            settingsNode.AppendChild(ToElement(document, nameof(this.InventoryResetEnabled), this.InventoryResetEnabled));
            settingsNode.AppendChild(ToElement(document, nameof(this.StartTimerAutomatically), this.StartTimerAutomatically));
            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            bool bval;

            this.UseGameTime = settings[nameof(this.UseGameTime)] != null ?
               (Boolean.TryParse(settings[nameof(this.UseGameTime)].InnerText, out bval) ? bval : DEFAULT_USE_GAME_TIME)
               : DEFAULT_USE_GAME_TIME;

            this.InventoryResetEnabled = settings[nameof(this.InventoryResetEnabled)] != null ?
               (Boolean.TryParse(settings[nameof(this.InventoryResetEnabled)].InnerText, out bval) ? bval : DEFAULT_INVENTORY_RESET_ENABLED)
               : DEFAULT_INVENTORY_RESET_ENABLED;

            this.StartTimerAutomatically = settings[nameof(this.StartTimerAutomatically)] != null ?
               (Boolean.TryParse(settings[nameof(this.StartTimerAutomatically)].InnerText, out bval) ? bval : DEFAULT_START_TIMER_AUTOMATICALLY)
               : DEFAULT_START_TIMER_AUTOMATICALLY;
        }

        static XmlElement ToElement<T>(XmlDocument document, string name, T value)
        {
            XmlElement str = document.CreateElement(name);
            str.InnerText = value.ToString();
            return str;
        }
    }
}
