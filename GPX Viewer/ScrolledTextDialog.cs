using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScrolledText {
    public partial class ScrolledTextDialog : Form {
        public static readonly String NL = Environment.NewLine;

        public ScrolledTextDialog(Size size, string text) {
            InitializeComponent();

            // Resize the Form
            if (size != null) {
                this.Size = size;
            }
            if (!String.IsNullOrEmpty(text)) {
                this.textBox.Text = text;
            }
        }

        public ScrolledTextDialog(Size size) : this(size, "") { }

        public void appendText(string text) {
            if (!String.IsNullOrEmpty(text)) {
                this.textBox.AppendText(text);
            }
        }

        public void appendTextAndNL(string text) {
            if (!String.IsNullOrEmpty(text)) {
                this.textBox.AppendText(text + NL);
            }
        }

        public void clear() {
                this.textBox.Text = "";
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e) {
            // Just hide rather than close if the user did it
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                Visible = false;
            }
        }

        private void OnButtonCancelClick(object sender, EventArgs e) {
            this.Visible = false;
        }

        private void OnButtonClearClick(object sender, EventArgs e) {
            this.clear();
        }
    }
}
