using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using LiveSplit.Model;
using LiveSplit.DarkSoulsIGT.Conditions;
using System.Windows.Forms;

namespace LiveSplit.DarkSoulsIGT
{
    public class DSAutoSplitter
    {
        private static readonly string[] conditionTypeOptions =
        {
            "Boss",
            "Quitout",
            "Item",
        };

        public static void InitUI(TableLayoutPanel tblAutosplitter, IRun run)
        {
            // list
            List<ConditionList> fromSettings = new List<ConditionList>();

            int splitCount = run.Count;
            tblAutosplitter.RowCount = splitCount;

            for (int i = 0; i < splitCount; i++)
            {
                ISegment split = run[i];

                ConditionList conditionList = new ConditionList(new List<Condition>());
                fromSettings.Add(conditionList);

                RowStyle splitStyle = new RowStyle(SizeType.AutoSize, 50f);
                tblAutosplitter.RowStyles.Add(splitStyle);

                GroupBox box = new GroupBox()
                {
                    Text = "{Binding Name}",
                    Dock = DockStyle.Fill,
                };
                box.AutoSize = true;
                box.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                TableLayoutPanel boxContent = new TableLayoutPanel();
                boxContent.AutoSize = true;
                boxContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;

                // NOTE(scewps): The group box header disappears for some reason when you add contents to it, so I added a label to replace it
                Label title = new Label();
                title.Text = split.Name;
                boxContent.Controls.Add(title);

                ComboBox conditionType = new ComboBox();
                boxContent.Controls.Add(conditionType);

                conditionType.Items.AddRange(conditionTypeOptions);
                conditionType.SelectedIndexChanged += (sender, e) =>
                {
                    Control conditionOptions = GetConditionOptions(conditionType.SelectedIndex, boxContent);

                    // Remove all options after the condition type selection box, then add the options specific to this condition type
                    for (int j = boxContent.Controls.IndexOf(conditionType) + 1; j < boxContent.Controls.Count; j++)
                        boxContent.Controls.RemoveAt(j);
                    boxContent.Controls.Add(conditionOptions);
                };

                box.Controls.Add(boxContent);
                tblAutosplitter.Controls.Add(box, 0, i);
            }

            ComboBox dropdown = new ComboBox();
            ConditionBuilder.Builders.ForEach(builder =>
            {
                //groupBoxAutosplitter.Controls.Add(builder.GetControl());
                builder.OnConditionReady += (condition) =>
                {
                    Console.WriteLine("lmao");
                    //list
                };
                //dropdown.Items.Add(builder.Name);
            });

            //dropdown.SelectedIndexChanged += (sender, e) =>
            //{
            //    ConditionBuilder b = ConditionBuilder.Builders[dropdown.SelectedIndex];
            //    var table = b.GetControl();
            //};

            //TableLayoutPanel panel = new TableLayoutPanel();
            //panel.Anchor = AnchorStyles.Left;
            //ComboBox cbx = new ComboBox();
            //panel.Controls.Add(cbx);
            //this.groupBoxAutosplitter.Controls.Add(panel);    

            //this.lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        // Returns the correct option menu for this condition type
        static Control GetConditionOptions(int conditionType, Panel splitPanel)
        {
            switch (conditionType)
            {
                case 0: // Bosses
                    return GetBossConditionOption();
                case 1: // Quitout
                    return null;
                case 2: // Item
                    return GetItemConditionOption(splitPanel);
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        static Control GetBossConditionOption()
        {
            ComboBox bossList = new ComboBox();
            bossList.Items.AddRange(GameInfo.bossNames);
            return bossList;
        }

        static Control GetItemConditionOption(Panel splitPanel)
        {
            ComboBox itemType = new ComboBox();
            itemType.Items.AddRange(GameInfo.itemTypes);
            itemType.SelectedIndexChanged += (sender, e) =>
            {
                ComboBox itemNames = new ComboBox();
                itemNames.Items.AddRange(GameInfo.GetItemNames(itemType.SelectedIndex));

                // Remove all options after the condition type selection box, then add the options specific to this condition type
                for (int j = splitPanel.Controls.IndexOf(itemType) + 1; j < splitPanel.Controls.Count; j++)
                    splitPanel.Controls.RemoveAt(j);
                splitPanel.Controls.Add(itemNames);
            };
            return itemType;
        }
    }
}
