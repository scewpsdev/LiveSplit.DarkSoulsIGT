using System;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.DarkSoulsIGT
{
    public partial class DSSettings : UserControl
    {
        private const bool DEFAULT_INVENTORY_RESET_ENABLED = false;

        public bool InventoryResetEnabled { get; set; }

        public DSSettings()
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            this.cbxInventoryReset.DataBindings.Add("Checked", this, nameof(InventoryResetEnabled), false, DataSourceUpdateMode.OnPropertyChanged);
            this.InventoryResetEnabled = DEFAULT_INVENTORY_RESET_ENABLED;
            this.lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.cbxInventoryReset.Text = "Enable";
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            XmlElement settingsNode = document.CreateElement("Settings");
            settingsNode.AppendChild(ToElement(document, nameof(this.InventoryResetEnabled), this.InventoryResetEnabled));
            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            this.InventoryResetEnabled = settings[nameof(this.InventoryResetEnabled)] != null ?
               (Boolean.TryParse(settings[nameof(this.InventoryResetEnabled)].InnerText, out bool bval) ? bval : DEFAULT_INVENTORY_RESET_ENABLED)
               : DEFAULT_INVENTORY_RESET_ENABLED;
        }

        static XmlElement ToElement<T>(XmlDocument document, string name, T value)
        {
            XmlElement str = document.CreateElement(name);
            str.InnerText = value.ToString();
            return str;
        }

        private void cbxInventoryReset_MouseHover(object sender, EventArgs e)
        {
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.SetToolTip(this.cbxInventoryReset, "Resets equipment inventory indexes when you start/reset a run as those normally carry over to other/new characters.");
        }
    }
}
