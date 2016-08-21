using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wuake.Helpers
{
    public sealed class KeyboardHook : IDisposable
    { 
        /*
         *  This is a P/Invoke used to register hotkeys with user32.dll.
         *  The reason for this is that we want our hotkeys to be detected even when the application is not focused.
         */
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        
        private class Window : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;

            public Window()
            {
                this.CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_HOTKEY)
                {
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFFFF);

                    if (KeyPressed != null)
                        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public void Dispose()
            {
                this.DestroyHandle();
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;
        }

        

        private Window _window = new Window();
        private int _currentId;
        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public KeyboardHook()
        {
            _window.KeyPressed += delegate (object sender, KeyPressedEventArgs args)
            {
                if (KeyPressed != null)
                    KeyPressed(this, args);
            };
        }

        public void RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            _currentId++;
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
                throw new InvalidOperationException("Unable to register Hotkey");
        }

        

        public void Dispose()
        {
            for (int i = _currentId; i > 0; i--)
            {
                UnregisterHotKey(_window.Handle, i);
            }
            _window.Dispose();
        }

    }
}


