using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using LiveSplit.UI;
using LiveSplit.UI.Components;

namespace Ds1Igt {
    class Ds1Component : IComponent {

        private InfoTimeComponent _infoTimeComponent;
        private RegularTimeFormatter _regularTimeFormatter;
        private TimerModel _timerModel;

        private Ds1Pointer _pointer;
        private Ds1Control _control;

        private int _oldMillis;
        private bool _latch;

        public string ComponentName => "Dark Souls IGT";

        #region 'Layout'
        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
        public IDictionary<string, Action> ContextMenuControls => null;
        public float HorizontalWidth => _infoTimeComponent.HorizontalWidth;
        public float VerticalHeight => _control.SecIgtEnabled ? _infoTimeComponent.VerticalHeight : 0;
        public float MinimumHeight => _infoTimeComponent.MinimumHeight;
        public float MinimumWidth => _infoTimeComponent.MinimumWidth;
        public float PaddingBottom => _infoTimeComponent.PaddingBottom;
        public float PaddingLeft => _infoTimeComponent.PaddingLeft;
        public float PaddingRight => _infoTimeComponent.PaddingRight;
        public float PaddingTop => _infoTimeComponent.PaddingTop;

        public XmlNode GetSettings(XmlDocument doc) => _control.GetSettings(doc);

        public Control GetSettingsControl(LayoutMode mode) {
            if(_control == null) _control = new Ds1Control();
            _control.Mode = mode;
            return _control;
        }

        public void SetSettings(XmlNode settings) => _control.SetSettings(settings);
        #endregion

        public Ds1Component(LiveSplitState state) {
            _regularTimeFormatter = new RegularTimeFormatter(TimeAccuracy.Hundredths);
            _infoTimeComponent = new InfoTimeComponent(ComponentName, TimeSpan.Zero, _regularTimeFormatter);

            _control = new Ds1Control();

            _pointer = new Ds1Pointer();

            state.OnReset += (sender, value) => _oldMillis = 0;
            state.OnStart += (sender, args) => _oldMillis = 0;
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) {

            var millis = _pointer.GetIgt();
            if (millis > 100) {
                _oldMillis = millis;
                _latch = false;
            }

            if (millis == 0 && !_latch) {
                _oldMillis -= 594;
                _latch = true;
            }

            if (_oldMillis <= 0) _oldMillis = 0;

            state.SetGameTime(new TimeSpan(0,0,0,0,_oldMillis <= 1 ? 1 : _oldMillis));
        }

        public void Dispose() {
            _infoTimeComponent.Dispose();
        }
    }
}
