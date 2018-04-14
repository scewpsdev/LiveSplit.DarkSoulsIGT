using System;
using Ds1Igt;
using LiveSplit.Model;
using LiveSplit.UI.Components;

[assembly: ComponentFactory(typeof(Ds1Factory))]

namespace Ds1Igt {
    internal class Ds1Factory : IComponentFactory {

        public ComponentCategory Category => ComponentCategory.Timer;
        public string ComponentName => "DsIgt";
        public IComponent Create(LiveSplitState state) => new Ds1Component(state);

        public string UpdateName => ComponentName;
        public string Description => "Dark Souls In-game time Splits by Jiiks(jiiks.net)";

        public string XMLURL => ""; //TODO Github
        public string UpdateURL => ""; //TODO Github

        public Version Version => Version.Parse("1.3.0");
    }
}
