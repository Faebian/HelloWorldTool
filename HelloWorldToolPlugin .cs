using System;
using System.Windows.Forms;
using UnicornOne.Abstractions.Tool;

namespace HelloWorldTool
{
    public sealed class HelloWorldToolPlugin : IToolPlugin
    {
        private IToolHost _host;

        public string Name => "Hello World";
        public string Version => "1.0.0";

        public void Initialize(
            string jsonConfig,
            IToolHost host)
        {
            _host = host;

            _host?.AppendLog(
                "System",
                "Info",
                "Hello World Tool initialized.");
        }

        public Form CreateForm()
        {
            return new HelloWorldForm(_host);
        }

        public void Dispose()
        {
        }
    }


    internal sealed class HelloWorldForm : Form
    {
        private int _clickCount;

        private readonly Label _label;
        private readonly Button _button;

        public HelloWorldForm(
            IToolHost host)
        {
            Text = "Hello World";

            _label = new Label
            {
                AutoSize = true,
                Left = 30,
                Top = 30,
                Text = "You clicked the button 0 times"
            };

            _button = new Button
            {
                Left = 30,
                Top = 70,
                Width = 140,
                Height = 35,
                Text = "Click me"
            };

            _button.Click += (sender, e) =>
            {
                _clickCount++;

                _label.Text =
                    $"You clicked the button {_clickCount} times";
            };

            Controls.Add(_label);
            Controls.Add(_button);

            if (host?.Theme != null)
                host.Theme.ApplyBaseStyles(this);
        }
    }
}