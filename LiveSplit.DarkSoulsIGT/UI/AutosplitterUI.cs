using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using LiveSplit.Model;
using LiveSplit.DarkSoulsIGT.Conditions;
using System.Windows.Forms;

namespace LiveSplit.DarkSoulsIGT
{
    public class AutosplitterUI
    {
        internal class ConditionUI
        {
            internal TableLayoutPanel panel = null;
        }

        internal class SplitUI
        {
            internal List<ConditionUI> conditions;
            internal GroupBox box = null;
            internal TableLayoutPanel panel = null;
            internal Label label = null;
            internal Button addButton = null;

            internal SplitUI()
            {
                conditions = new List<ConditionUI>();
            }
        }

        List<ConditionList> splitConditions;
        List<SplitUI> splitUIs;

        public AutosplitterUI(TableLayoutPanel tblAutosplitter, IRun run)
        {
            int splitCount = run.Count;

            splitConditions = new List<ConditionList>(splitCount);
            splitUIs = new List<SplitUI>(splitCount);

            tblAutosplitter.RowCount = splitCount;
            tblAutosplitter.Controls.Clear();

            for (int i = 0; i < splitCount; i++)
            {
                ConditionList conditionList = new ConditionList(new List<Condition>());
                splitConditions.Add(conditionList);

                ISegment split = run[i];

                RowStyle splitStyle = new RowStyle(SizeType.AutoSize, 50f);
                tblAutosplitter.RowStyles.Add(splitStyle);

                SplitUI splitUI = CreateSplitUI(split, conditionList);
                splitUIs.Add(splitUI);

                tblAutosplitter.Controls.Add(splitUI.box, 0, i);
            }

            // NOTE(scewps): Do we still need this?
            ComboBox dropdown = new ComboBox();
            ConditionBuilder.Builders.ForEach(builder =>
            {
                //groupBoxAutosplitter.Controls.Add(builder.GetControl());
                builder.OnConditionReady += (condition) =>
                {
                    Console.WriteLine("lmao"); // lulw
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

        static SplitUI CreateSplitUI(ISegment split, ConditionList conditionList)
        {
            SplitUI splitUI = new SplitUI();

            GroupBox box = new GroupBox();
            box.Dock = DockStyle.Fill;
            box.AutoSize = true;
            box.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            splitUI.box = box;

            int conditionCount = conditionList.conditions.Count;

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.RowCount = conditionCount + 1; // + 1 for the add button
            panel.ColumnCount = 1;
            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            splitUI.panel = panel;

            // NOTE(scewps): The group box header disappears for some reason when you add contents to it, so I added a label to replace it
            Label label = new Label();
            label.Text = split.Name;
            panel.Controls.Add(label);
            splitUI.label = label;

            Button addButton = CreateAddConditionButtonUI(conditionList, splitUI);
            panel.Controls.Add(addButton);
            splitUI.addButton = addButton;

            box.Controls.Add(panel);

            return splitUI;
        }

        static Button CreateAddConditionButtonUI(ConditionList conditionList, SplitUI splitUI)
        {
            Button addConditionButton = new Button();
            addConditionButton.Text = "Add Condition";
            addConditionButton.AutoSize = true;
            addConditionButton.Click += (sender, e) =>
            {
                int conditionIdx = conditionList.conditions.Count;
                conditionList.conditions.Add(null);

                ConditionUI conditionUI = CreateConditionUI(null, conditionIdx, conditionList, splitUI);
                splitUI.conditions.Add(conditionUI);
                splitUI.panel.Controls.Add(conditionUI.panel);
                splitUI.panel.Controls.SetChildIndex(conditionUI.panel, 1 + conditionIdx);
            };

            return addConditionButton;
        }

        static ConditionUI CreateConditionUI(Condition condition, int conditionIdx, ConditionList conditionList, SplitUI splitUI)
        {
            ConditionUI conditionUI = new ConditionUI();

            TableLayoutPanel panel = new TableLayoutPanel();
            panel.ColumnCount = 2;
            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.8f));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 0.2f));
            conditionUI.panel = panel;

            TableLayoutPanel optionsPanel = new TableLayoutPanel();
            optionsPanel.ColumnCount = 3; // TODO(scewps): set depending on condition type
            optionsPanel.RowCount = 1;
            optionsPanel.AutoSize = true;
            optionsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            ComboBox conditionTypeDropdown = new ComboBox();
            optionsPanel.Controls.Add(conditionTypeDropdown);

            conditionTypeDropdown.Items.AddRange(Condition.Names);
            conditionTypeDropdown.SelectedIndexChanged += (sender, e) =>
            {
                ConditionType conditionType = (ConditionType)conditionTypeDropdown.SelectedIndex;

                Control conditionOptions = CreateConditionOptions(conditionType, optionsPanel);
                for (int k = optionsPanel.Controls.IndexOf(conditionTypeDropdown) + 1; k < optionsPanel.Controls.Count; k++)
                    optionsPanel.Controls.RemoveAt(k);
                optionsPanel.Controls.Add(conditionOptions);

                Condition newCondition = CreateCondition(conditionType);
                conditionList.conditions[conditionIdx] = newCondition;
                //Control conditionOptions = CreateConditionOptions(conditionType.SelectedIndex, conditionPanel);
                //conditionList.conditions[i] = CreateCondition(conditionType.SelectedIndex);

                // Remove all options after the condition type selection box, then add the options specific to this condition type
            };

            panel.Controls.Add(optionsPanel);

            Button removeButton = new Button();
            //removeButton.Text = "Remove condition";
            removeButton.BackgroundImage = Image.FromFile("Components/LiveSplit.DarkSoulsIGT.Icon_ConditionRemove.png");
            removeButton.TextImageRelation = TextImageRelation.Overlay;
            //removeButton.AutoSize = true;
            removeButton.Size = new Size(28, 28);
            removeButton.BackgroundImageLayout = ImageLayout.Stretch;
            removeButton.Click += (sender, e) =>
            {
                conditionList.conditions.RemoveAt(conditionIdx);
                splitUI.conditions.RemoveAt(conditionIdx);
                splitUI.panel.Controls.RemoveAt(1 + conditionIdx);
            };
            panel.Controls.Add(removeButton);

            return conditionUI;
        }

        // Returns the correct option menu for this condition type
        static Control CreateConditionOptions(ConditionType conditionType, Panel splitPanel)
        {
            switch (conditionType)
            {
                case ConditionType.BossDied:
                    return CreateBossConditionOption();
                case ConditionType.Quitout:
                    return null;
                case ConditionType.ItemObtained:
                    return CreateItemConditionOption(splitPanel);
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        static Condition CreateCondition(ConditionType conditionType)
        {
            switch (conditionType)
            {
                case ConditionType.BossDied:
                    return new OnBossDied();
                case ConditionType.Quitout:
                    return null;
                case ConditionType.ItemObtained:
                    //return new OnItemObtained(); TODO: implement this
                    return null;
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        static Control CreateBossConditionOption()
        {
            ComboBox bossList = new ComboBox();
            for (int i = 0; i < Flags.Bosses.Length; i++)
            {
                bossList.Items.Add(Flags.Bosses[i].Name);
            }
            return bossList;
        }

        static string[] GetItemNames(int itemTypeIdx)
        {
            /*
            switch (itemTypeIdx)
            {
                case 0: // Melee weapons
                    return new string[] { };
                case 1: // Rings
                    return new string[] { };
                default:
                    Debug.Assert(false);
                    return null;
            }
            */

            // Test items
            string[] testItemNames = new string[Flags.TestItems.Length];
            for (int i = 0; i < Flags.TestItems.Length; i++)
                testItemNames[i] = Flags.TestItems[i].name;
            return testItemNames;
        }

        static Control CreateItemConditionOption(Panel conditionPanel)
        {
            ComboBox itemType = new ComboBox();
            itemType.Items.AddRange(Flags.ItemTypes);
            itemType.SelectedIndexChanged += (sender, e) =>
            {
                ComboBox itemNames = new ComboBox();
                itemNames.Items.AddRange(GetItemNames(itemType.SelectedIndex));

                // Remove all options after the condition type selection box, then add the options specific to this condition type
                for (int j = conditionPanel.Controls.IndexOf(itemType) + 1; j < conditionPanel.Controls.Count; j++)
                    conditionPanel.Controls.RemoveAt(j);
                conditionPanel.Controls.Add(itemNames);
            };
            return itemType;
        }
    }
}
