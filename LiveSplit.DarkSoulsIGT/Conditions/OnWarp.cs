using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.DarkSoulsIGT.Conditions
{
    class OnWarp : Condition
    {
        public int total;
        int count = 0;

        public OnWarp(int total = 1) : base(ConditionType.Warp)
        {
            this.total = total;
        }

        public override void Reset()
        {
            count = 0;
        }

        public override void Start()
        {
            //Model.Instance.OnWarp += Instance_OnWarp;
        }

        public override void Stop()
        {
            //Model.Instance.OnWarp -= Instance_OnWarp;
        }

        private void Instance_OnWarp()
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
