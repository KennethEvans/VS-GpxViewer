using KEGpsUtils;
using System;
using System.Windows.Forms;

namespace KEGpsUtils {
    public partial class TimeIntervalDialog : Form {
        public TimeIntervalDialog() {
            InitializeComponent();
        }

        public string Title
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        public string Label
        {
            get
            {
                return labelMsg.Text;
            }
            set
            {
                labelMsg.Text = value;
            }
        }

        public DateTime StartDate
        {
            get
            {
                try {
                    return DateTime.Parse(textBoxStartDate.Text);
                } catch (Exception) {
                    return DateTime.MinValue;
                }
            }
            set
            {
                textBoxStartDate.Text = value.ToString(GpsData.TimeFormatUTC);
            }
        }

        public DateTime EndDate
        {
            get
            {
                try {
                    return DateTime.Parse(textBoxEndDate.Text);
                } catch (Exception) {
                    return DateTime.MinValue;
                }
            }
            set
            {
                textBoxEndDate.Text = value.ToString(GpsData.TimeFormatUTC);
            }
        }

        public void setStartDateVisible(bool visible) {
            labelStart.Visible = visible;
            textBoxStartDate.Visible = visible;
        }

        private void btn_clicked(object sender, EventArgs e) {
            Button btn = (Button)sender;
            if (btn == buttonOk) {
                DialogResult = DialogResult.OK;
            } else if (btn == buttonCancel) {
                DialogResult = DialogResult.Cancel;
                return;
            }
            Close();
        }
    }
}
