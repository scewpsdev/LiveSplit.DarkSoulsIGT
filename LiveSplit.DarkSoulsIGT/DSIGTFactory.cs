using LiveSplit.DarkSoulsIGT;
using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Reflection;

[assembly: ComponentFactory(typeof(DSIGTFactory))]
namespace LiveSplit.DarkSoulsIGT
{
    internal class DSIGTFactory : IComponentFactory
    {
        public string ComponentName => "Dark Souls In-Game Timer";
        public string Description => "Dark Souls In-Game Timer by Jiiks(jiiks.net) & CapitaineToinon";
        public string UpdateName => ComponentName;

        public ComponentCategory Category => ComponentCategory.Timer;
        public string XMLURL => "https://raw.githubusercontent.com/CapitaineToinon/LiveSplit.DarkSoulsIGT/master/LiveSplit.DarkSoulsIGT/";
        public string UpdateURL => "Components/update.LiveSplit.DarkSoulsIGT.xml";

        public Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        public IComponent Create(LiveSplitState state)
        {
            return new DSIGTComponent(state);
        }
    }
}
