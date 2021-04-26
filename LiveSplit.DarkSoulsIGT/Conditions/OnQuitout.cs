using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.DarkSoulsIGT.Conditions {
    class OnQuitout : Condition {
        int count = 0;
        int total = 1;

        public OnQuitout(int total = 1)
        {
            this.total = total;
        }

        public override void Reset()
        {
            count = 0;
        }

        public override void Start()
        {
            Model.Instance.OnQuitout += Instance_OnQuitout;
        }

        public override void Stop()
        {
            Model.Instance.OnQuitout -= Instance_OnQuitout;
        }

        private void Instance_OnQuitout()
        {
            count++;

            if (count >= total)
            {
                RaiseOnConditionComplete();
                Stop();
            }
        }
    }
}
