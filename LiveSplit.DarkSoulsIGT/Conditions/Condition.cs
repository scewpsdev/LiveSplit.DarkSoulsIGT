using System.Collections.Generic;
using System.Linq;

namespace LiveSplit.DarkSoulsIGT.Conditions {
    public abstract class Condition {
        public event OnConditionCompleteEventHandler OnConditionComplete;

        public void RaiseOnConditionComplete()
        {
            OnConditionComplete?.Invoke();
        }

        public abstract void Start();
        public abstract void Reset();
        public abstract void Stop();

        public delegate void OnConditionCompleteEventHandler();
    }

    public class ConditionList {

        public event OnConditionListCompleteEventHandler OnConditionListComplete;

        bool enabled;
        LinkedListNode<Condition> current;
        LinkedList<Condition> conditions;

        public ConditionList(List<Condition> conditions)
        {
            this.enabled = false;
            this.conditions = new LinkedList<Condition>(conditions);
        }

        public void Reset()
        {
            enabled = false;
            var e = conditions.GetEnumerator();
            while (e.MoveNext())
            {
                e.Current.Stop();
                e.Current.Reset();
                e.Current.OnConditionComplete -= Current_OnConditionComplete;
            };
        }

        public void Start()
        {
            if (!enabled)
            {
                enabled = true;
                if (conditions.First != null)
                {
                    current = conditions.First;
                    WaitNext();
                }
            }
        }

        public void WaitNext()
        {
            current.Value.OnConditionComplete += Current_OnConditionComplete;
            current.Value.Start();
        }

        private void Current_OnConditionComplete()
        {
            current.Value.OnConditionComplete -= Current_OnConditionComplete;
            current.Value.Stop();

            if (current.Next != null)
            {
                current = current.Next;
                WaitNext();
            }
            else
            {
                OnConditionListComplete?.Invoke();
            }
        }

        public delegate void OnConditionListCompleteEventHandler();
    }
}
