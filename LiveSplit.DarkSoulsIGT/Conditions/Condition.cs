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
        ItemPickup,
        Quitout,
        Warp,
    }

    public abstract class Condition
    {
        public static readonly string[] Names =
        {
            "Boss",
            "Item",
            "Quitout",
            "Warp",
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

        private int current = 0;
        private List<Condition> conditions;

        public ConditionList(List<Condition> conditions)
        {
            this.conditions = conditions;
            /*
            foreach (var condition in conditions)
            {
                condition.OnConditionComplete += Condition_OnConditionComplete;
            }
            */
        }

        public void AddCondition(Condition condition)
        {
            conditions.Add(condition);
            if (condition != null)
                condition.OnConditionComplete += Condition_OnConditionComplete;
        }

        public void AddCondition(int index, Condition condition)
        {
            conditions[index] = condition;
            if (condition != null)
                condition.OnConditionComplete += Condition_OnConditionComplete;
        }

        public void RemoveCondition(int index)
        {
            Condition condition = conditions[index];
            condition.Stop();
            conditions.RemoveAt(index);
        }

        public int ConditionCount
        {
            get { return conditions.Count; }
        }

        private void Condition_OnConditionComplete()
        {
            if (conditions.Count > 0)
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
            if (conditions.Count > 0)
            {
                conditions[current].Start();
            }
        }

        public void Stop()
        {
            if (conditions.Count > 0)
            {
                conditions[current].Stop();
            }
        }


        public delegate void OnConditionListCompleteEventHandler();
    }
}
