using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wuake.Helpers;

namespace Wuake.Forms
{
    public partial class MainForm : Form
    {
        #region Variable Declarations

        private readonly double HEIGHT_PERCENTAGE = 0.40;

        private readonly KeyboardHook hook = new KeyboardHook();
        private readonly TextBox boxOutput = new TextBox();
        private readonly TextBox boxInput = new TextBox();

        private Process p;
        private StreamWriter inputWriter;

        public delegate void outputCallback_t(string text);
        public outputCallback_t outputBoxCallback;
        #endregion

        public MainForm()
        {
            InitializeComponent();
            InitializeHotkey();
        }


        #region Initialization Methods
        public void InitializeComponent()
        {

            SetComponentConfig();
            CreateOutputBox();
            CreateInputBox();

            this.Invalidate();
            this.ActiveControl = boxInput;

            InitializeAndRunTerminalProcess();
        }

        private void SetComponentConfig()
        {
            var currentBounds = Screen.FromControl(this).Bounds;

            this.FormBorderStyle = FormBorderStyle.None;
            this.Width = currentBounds.Width;
            this.Left = 0;
            this.Top = 0;
            this.Height = (int)(currentBounds.Height * HEIGHT_PERCENTAGE);
            this.BackColor = Color.Black;
            this.Opacity = 0.7;
        }

        private void CreateInputBox()
        {
            boxInput.Height = 20;
            boxInput.Width = this.Width - 20;

            boxInput.Top = this.Height - 20;
            boxInput.Left = 20;

            boxInput.BorderStyle = BorderStyle.None;

            boxInput.BackColor = Color.Black;
            boxInput.ForeColor = Color.White;

            boxInput.Font = new Font(FontFamily.GenericMonospace, boxOutput.Font.Size);
            
            boxInput.KeyDown += new KeyEventHandler(hasKeysPressedInInputBox);

            this.Controls.Add(boxInput);
        }

        private void CreateOutputBox()
        {
            string defaultText = GetDefaultText();

            boxOutput.Height = this.Height - 20;
            boxOutput.Width = this.Width - 20;
            boxOutput.Left = 20;
            boxOutput.ReadOnly = true;
            boxOutput.Multiline = true;
            boxOutput.Font = new Font(FontFamily.GenericMonospace, boxOutput.Font.Size);
            boxOutput.BackColor = Color.Black;
            boxOutput.ForeColor = Color.White;
            boxOutput.BorderStyle = BorderStyle.None;
            boxOutput.Text = defaultText;
            boxOutput.Click += new EventHandler(hasBeenClicked);
            this.Controls.Add(boxOutput);
        }

        private void InitializeHotkey()
        {
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(ToggleVisibilityPressed);
            hook.RegisterHotKey(Wuake.Helpers.ModifierKeys.Control, Keys.Oem5);
        }


        private void InitializeAndRunTerminalProcess()
        {

            outputBoxCallback = new outputCallback_t(AddTextToBoxOutput);

            p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.ErrorDataReceived += new DataReceivedEventHandler(hasTerminalOutput);
            p.OutputDataReceived += new DataReceivedEventHandler(hasTerminalOutput);
            p.Start();

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            inputWriter = p.StandardInput;
        }

        #endregion

        #region Event Handlers
        private void hasBeenClicked(object sender, EventArgs e)
        {
            this.boxInput.Focus();
        }

        private void hasTerminalOutput(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                this.Invoke(outputBoxCallback, Environment.NewLine + e.Data);
            }
        }

        private void hasKeysPressedInInputBox(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (boxInput.Text.Trim().ToLower() == "cls")
                    this.boxOutput.Clear();
                else
                {
                    string command = boxInput.Text;
                    inputWriter.WriteLine(command);
                }
                this.boxInput.Clear();
            }
        }

        #endregion

        #region Helpers

        private string GetDefaultText()
        {
            var nl = Environment.NewLine;
            string defaultText =
                nl + nl + nl
                + "Welcome to Wuake!                                                        " + nl + nl
                + "Wuake is a quake/guake/yakuake-style terminal for windows using cmd.exe. " + nl
                + "Toggle the console visibility with Ctrl+§.                               " + nl + nl
                + "------------------------------------------------------------------------ " + nl;
            return defaultText;
        }


        private void AddTextToBoxOutput(string text)
        {
            this.boxOutput.AppendText(text);
        }
        #endregion

        #region Animation and Visibility

        private void ToggleVisibilityPressed(object sender, KeyPressedEventArgs e)
        {
            this.Height = 0;
            this.Visible = !this.Visible;
            if (this.Visible)
            {
                this.SlideDown();
                this.ActiveControl = boxInput;
            }
        }

        private void SlideDown()
        {
            var currentBounds = Screen.FromControl(this).Bounds;
            while(this.Height < (int)(currentBounds.Height * HEIGHT_PERCENTAGE))
            {
                this.Height = this.Height + 5;
                Application.DoEvents();
            }
            
        }
        #endregion


    }
}
