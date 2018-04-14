using System;
using System.Diagnostics;

namespace Ds1Igt {
    internal class Ds1Pointer {

        private long _address;
        public long GetAddress() => _address;
        private IntPtr _handle = IntPtr.Zero;
        public IntPtr GetHandle() => _handle;
        private Process _process;
        public Process GetProcess() => _process;

        public Ds1Pointer() {
            GetProcess(Config.Module);
            FindAddress();
        }

        private void FindAddress() {
            if (_process == null) return;
            _handle = _process.GetHandle();

            var fileVer = _process.MainModule.FileVersionInfo.FileVersion;
            // TODO File version checking

            _address = Config.BasePointerFinal.GetAddress(Config.OffsetsFinal[0], _handle, Config.OffsetSize);
        }

        private void GetProcess(string name) {
            var processes = Process.GetProcessesByName(name);
            if (processes.Length <= 0) return;

            _process = processes[0];
            _process.EnableRaisingEvents = true;
            _process.Exited += (sender, args) => DoReset();
        }

        private void DoReset() {
            _process = null;
            _handle = IntPtr.Zero;
            _address = 0;
        }

        public int GetIgt() {
            if (_process == null) {
                GetProcess(Config.Module);
                return 0;
            }

            if(_handle == IntPtr.Zero) FindAddress();
            if(_address == 0) FindAddress();

            var millis = Native.GetGameTimeMilliseconds(_address, _handle, Config.OffsetSize);

            if(millis < 200) FindAddress();

            return Native.GetGameTimeMilliseconds(_address, _handle, Config.OffsetSize);
        }

    }
}
