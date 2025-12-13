using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WorkflowEnabler.Forms
{
    public partial class ResultsForm : Form
    {
        private readonly TextBox _txt;
        private readonly Button _btnCopy;
        private readonly Button _btnClose;

        public ResultsForm(string operation, List<string> succeeded, List<string> skipped, List<(string Name, string Error)> failed)
        {
            Text = $"{operation} - Details";
            Width = 800;
            Height = 500;
            StartPosition = FormStartPosition.CenterParent;

            _txt = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Consolas", 10),
                WordWrap = false
            };

            _btnCopy = new Button { Text = "Copy", Dock = DockStyle.Right, Width = 100 };
            _btnClose = new Button { Text = "Close", Dock = DockStyle.Right, Width = 100 };

            var panel = new Panel { Dock = DockStyle.Bottom, Height = 45 };
            panel.Controls.Add(_btnClose);
            panel.Controls.Add(_btnCopy);

            Controls.Add(_txt);
            Controls.Add(panel);

            _txt.Text = BuildText(operation, succeeded, skipped, failed);

            _btnCopy.Click += (s, e) =>
            {
                Clipboard.SetText(_txt.Text);
                MessageBox.Show("Copied to clipboard.", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            _btnClose.Click += (s, e) => Close();
        }

        private static string BuildText(string operation, List<string> succeeded, List<string> skipped, List<(string Name, string Error)> failed)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{operation} - Summary");
            sb.AppendLine($"Succeeded: {succeeded.Count} | Skipped: {skipped.Count} | Errors: {failed.Count}");
            sb.AppendLine();

            if (succeeded.Any())
            {
                sb.AppendLine("✅ Succeeded:");
                foreach (var n in succeeded.OrderBy(x => x))
                    sb.AppendLine($"- {n}");
                sb.AppendLine();
            }

            if (skipped.Any())
            {
                sb.AppendLine("🟡 Skipped (already in desired state):");
                foreach (var n in skipped.OrderBy(x => x))
                    sb.AppendLine($"- {n}");
                sb.AppendLine();
            }

            if (failed.Any())
            {
                sb.AppendLine("❌ Errors:");
                foreach (var f in failed.OrderBy(x => x.Name))
                    sb.AppendLine($"- {f.Name} | {f.Error}");
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
