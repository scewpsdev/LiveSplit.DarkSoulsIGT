using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveSplit.DarkSoulsIGT.Conditions
{
    class OnItemPickup : Condition
    {
        public ItemFlag item = null;
        bool isObtained = false;


        public OnItemPickup() : base(ConditionType.ItemPickup)
        {
        }

        public override void Reset()
        {
            isObtained = false;
        }

        public override void Start()
        {
            // TODO add this
            //Model.Instance.OnItemObtained += Instance_OnItemObtained;
        }

        public override void Stop()
        {
            // TODO add this
            //Model.Instance.OnItemObtained -+= Instance_OnItemObtained;
        }

        private void Instance_OnItemObtained(ItemFlag item)
        {
            if (item.FlagID != item.FlagID) return;
            if (isObtained) return;

            isObtained = true;
            RaiseOnConditionComplete();
            Stop();
        }
    }
}
