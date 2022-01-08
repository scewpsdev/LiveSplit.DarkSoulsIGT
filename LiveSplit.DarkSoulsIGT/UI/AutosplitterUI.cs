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

        public List<ConditionList> splitConditions;
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

            int conditionCount = conditionList.ConditionCount;

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
                int conditionIdx = conditionList.ConditionCount;
                conditionList.AddCondition(null);

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

                Condition newCondition = CreateCondition(conditionType);
                conditionList.AddCondition(conditionIdx, newCondition);

                // Remove all options after the condition type selection box, then add the options specific to this condition type
                for (int k = optionsPanel.Controls.IndexOf(conditionTypeDropdown) + 1; k < optionsPanel.Controls.Count; k++)
                    optionsPanel.Controls.RemoveAt(k);
                if (newCondition != null)
                {
                    Control conditionOptions = CreateOptionsForCondition(newCondition, optionsPanel);
                    optionsPanel.Controls.Add(conditionOptions);
                }
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
                conditionList.RemoveCondition(conditionIdx);
                splitUI.conditions.RemoveAt(conditionIdx);
                splitUI.panel.Controls.RemoveAt(1 + conditionIdx);
            };
            panel.Controls.Add(removeButton);

            return conditionUI;
        }

        // Returns the correct option menu for this condition type
        static Control CreateOptionsForCondition(Condition condition, Panel splitPanel)
        {
            switch (condition.type)
            {
                case ConditionType.BossDied:
                    return CreateBossConditionOption((OnBossDied)condition);
                case ConditionType.ItemPickup:
                    return CreateItemConditionOption((OnItemPickup)condition, splitPanel);
                case ConditionType.Quitout:
                    return CreateQuitoutConditionOption((OnQuitout)condition);
                case ConditionType.Warp:
                    return CreateWarpConditionOption((OnWarp)condition);
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
                case ConditionType.ItemPickup:
                    return new OnItemPickup();
                case ConditionType.Quitout:
                    return new OnQuitout();
                case ConditionType.Warp:
                    return new OnWarp();
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        static Control CreateBossConditionOption(OnBossDied condition)
        {
            ComboBox bossList = new ComboBox();
            for (int i = 0; i < Flags.Bosses.Length; i++)
            {
                bossList.Items.Add(Flags.Bosses[i].Name);
            }
            bossList.SelectedIndexChanged += (sender, e) =>
            {
                condition.boss = Flags.Bosses[bossList.SelectedIndex];
            };
            return bossList;
        }

        static Control CreateItemConditionOption(OnItemPickup condition, Panel conditionPanel)
        {
            ComboBox itemType = new ComboBox();
            itemType.Items.AddRange(Flags.Items.Keys.ToArray());
            itemType.SelectedIndexChanged += (sender, e) =>
            {
                ComboBox itemNames = new ComboBox();

                List<ItemFlag> flags = Flags.Items[Flags.Items.Keys.ToArray()[itemType.SelectedIndex]];
                foreach (ItemFlag flag in flags)
                    itemNames.Items.Add(flag.name);

                itemNames.SelectedIndexChanged += (_sender, _e) =>
                {
                    condition.item = flags[itemNames.SelectedIndex];
                };

                // Remove all options after the condition type selection box, then add the options specific to this condition type
                for (int j = conditionPanel.Controls.IndexOf(itemType) + 1; j < conditionPanel.Controls.Count; j++)
                    conditionPanel.Controls.RemoveAt(j);
                conditionPanel.Controls.Add(itemNames);
            };
            return itemType;
        }

        static Control CreateQuitoutConditionOption(OnQuitout condition)
        {
            NumericUpDown count = new NumericUpDown();
            count.Minimum = 1;
            count.ValueChanged += (sender, e) =>
            {
                condition.total = (int)count.Value;
            };
            return count;
        }

        static Control CreateWarpConditionOption(OnWarp condition)
        {
            NumericUpDown count = new NumericUpDown();
            count.Minimum = 1;
            count.ValueChanged += (sender, e) =>
            {
                condition.total = (int)count.Value;
            };
            return count;
        }
    }
}
