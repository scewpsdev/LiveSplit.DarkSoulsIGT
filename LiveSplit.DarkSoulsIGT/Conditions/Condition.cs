using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace LiveSplit.DarkSoulsIGT.Conditions
{

    public enum InputTypes
    {
        Dropdown,
        number
    }

    public abstract class ConditionBuilder
    {

        public abstract event OnConditionReadyEventHandler OnConditionReady;

        public static List<ConditionBuilder> Builders = new List<ConditionBuilder>()
        {
            new BossDiedBuilder(),
        };

        public abstract string Name { get; }

        public abstract TableLayoutPanel GetControl();

        public delegate void OnConditionReadyEventHandler(Condition condition);
    }

    public class BossDiedBuilder : ConditionBuilder
    {
        public override string Name => "Bosses";

        public override event OnConditionReadyEventHandler OnConditionReady;

        public override TableLayoutPanel GetControl()
        {
            TableLayoutPanel panel = new TableLayoutPanel();
            ComboBox dropdown = new ComboBox();

            List<BossFlag> sorted = new List<BossFlag>(Flags.Bosses);
            sorted.Sort((a, b) =>
            {
                return a.Name.CompareTo(b.Name);
            });

            foreach (var boss in sorted)
            {
                dropdown.Items.Add(boss);
            }

            dropdown.SelectedIndexChanged += (s, e) =>
            {
                OnConditionReady?.Invoke(new OnBossDied((BossFlag)dropdown.SelectedItem));
            };

            panel.Controls.Add(dropdown);
            return panel;
        }
    }

    public enum ConditionType : int
    {
        None = -1,
        BossDied,
        Quitout,
        ItemObtained,
    }

    public abstract class Condition
    {
        public static readonly string[] Names =
        {
            "Boss",
            "Quitout",
            "Item",
        };

        public event OnConditionCompleteEventHandler OnConditionComplete;

        public ConditionType type = ConditionType.None;

        public Condition(ConditionType type)
        {
            this.type = type;
        }

        public void RaiseOnConditionComplete()
        {
            OnConditionComplete?.Invoke();
        }

        public abstract void Start();
        public abstract void Reset();
        public abstract void Stop();

        public delegate void OnConditionCompleteEventHandler();
    }

    public class ConditionList
    {

        public event OnConditionListCompleteEventHandler OnConditionListComplete;

        int current = 0;
        public List<Condition> conditions;

        public ConditionList(List<Condition> conditions)
        {
            this.conditions = conditions;
            foreach (var condition in conditions)
            {
                condition.OnConditionComplete += Condition_OnConditionComplete;
            }
        }

        private void Condition_OnConditionComplete()
        {
            conditions[current].Stop();

            if (current == conditions.Count - 1)
            {
                OnConditionListComplete?.Invoke();
            }
            else
            {
                current += 1;
                conditions[current].Start();
            }
        }

        public void Reset()
        {
            foreach (var condition in conditions)
            {
                condition.Reset();
                condition.Stop();
            }
        }

        public void Start()
        {
            conditions[current].Start();
        }

        public void Stop()
        {
            conditions[current].Stop();
        }


        public delegate void OnConditionListCompleteEventHandler();
    }
}
