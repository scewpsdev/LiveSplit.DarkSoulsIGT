using System.Windows.Forms;
using System.Xml;
using LiveSplit.UI;

namespace Ds1Igt {
    public partial class Ds1Control : UserControl {
        public LayoutMode Mode;
        public bool SecIgtEnabled { get; set; }
        public Ds1Control() {
            InitializeComponent();
        }

        public XmlNode GetSettings(XmlDocument doc) {
            var parent = doc.CreateElement("Settings");
            return parent;
        }

        public void SetSettings(XmlNode node) {

        }
    }
}
