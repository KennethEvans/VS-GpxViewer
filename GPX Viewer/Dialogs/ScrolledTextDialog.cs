using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScrolledText {
    public partial class ScrolledTextDialog : Form {
        public static readonly String NL = Environment.NewLine;

        public ScrolledTextDialog(Size size, string text) {
            InitializeComponent();

            this.Text = text;

            // Do this to use as both model and modeless
            this.DialogResult = DialogResult.None;

            // Resize the Form
            if (size != null) {
                this.Size = size;
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

        public string getText() {
            return this.textBox.Text;
        }

        public void setText(string text) {
            textBox.Text = text;
        }

        public void clear() {
            this.textBox.Text = "";
        }

        public Button ButtonClear {
            get {
                return buttonClear;
            }
        }

        public Button ButtonCancel {
            get {
                return buttonCancel;
            }
        }

        public Button ButtonOK {
            get {
                return buttonOk;
            }
        }

        public TextBox TextBox {
            get {
                return textBox;
            }
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e) {
            // Just hide rather than close if the user did it
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                Visible = false;
            }
        }

        private void OnVisibleChanged(object sender, EventArgs e) {
            if (this.Visible) {
                this.DialogResult = DialogResult.None;
            }
        }

        private void OnButtonClearClick(object sender, EventArgs e) {
            this.clear();
        }

        private void OnButtonCancelClick(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Visible = false;
        }

        private void OnButtonOkClick(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
            this.Visible = false;
        }
    }
}
