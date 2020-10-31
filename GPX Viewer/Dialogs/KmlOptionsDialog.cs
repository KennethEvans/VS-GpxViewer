

using GPXViewer.KML;
using GPXViewer.Properties;
using KEUtils.Utils;
using System;
using System.Windows.Forms;
using static GPXViewer.KML.KmlOptions;

namespace GPXViewer.Dialogs {
    /// <summary>
    /// This is a dialog to get and set KmlOptions. The property KmlOptions is
    /// only set in OnOkClick and hence only available if DialogResult is
    /// DialogResult.OK.
    /// </summary>
    public partial class KmlOptionsDialog : Form {
        /// <summary>
        /// This property is only set in OnOkClick.
        /// </summary>
        public KmlOptions KmlOptions { get; set; }

        /// <summary>
        /// CTOR that initializes the controls from Settings.
        /// </summary>
        public KmlOptionsDialog() {
            InitializeComponent();

            getControlsFromSettings();
        }

        /// <summary>
        /// CTOR that initializes the controls from the given KmlOptions.
        /// </summary>
        /// <param name="options">The KmlOptions to use.</param>
        public KmlOptionsDialog(KmlOptions options) {
            InitializeComponent();

            getControlsFromOptions(options);
        }

        /// <summary>
        /// Gets the controls from the given KmlOptions.
        /// </summary>
        /// <param name="options">The KmlOptions to use.</param>
        private void getControlsFromOptions1(KmlOptions options) {
            textBoxFileName.Text = options.KmlFileName;
            textBoxIconScale.Text = options.IconScale.ToString();

            textBoxTrkColor.Text = options.TrkColor;
            textBoxTrkAlpha.Text = options.TrkAlpha;
            textBoxTrkLinewidth.Text = options.TrkLineWidth.ToString();
            comboBoxTrkColorMode.SelectedIndex = comboBoxTrkColorMode.FindString(
                options.TrkColorMode.ToString());
            checkBoxUseTrkIcon.Checked = options.UseTrkIcon;
            checkBoxUseTrkTrack.Checked = options.UseTrkTrack;
            checkBoxUseTrkLines.Checked = options.UseTrkLines;
            textBoxTrkIconUrl.Text = options.TrkIconUrl;

            textBoxRteColor.Text = options.RteColor;
            textBoxRteAlpha.Text = options.RteAlpha;
            textBoxRteLinewidth.Text = options.RteLineWidth.ToString();
            comboBoxRteColorMode.SelectedIndex = comboBoxRteColorMode.FindString(
                options.RteColorMode.ToString());
            checkBoxUseRteIcon.Checked = options.UseRteIcon;
            textBoxRteIconUrl.Text = options.RteIconUrl;

            textBoxWptColor.Text = options.WptColor;
            textBoxWptAlpha.Text = options.WptAlpha;
            textBoxWptLinewidth.Text = options.WptLineWidth.ToString();
            comboBoxWptColorMode.SelectedIndex = comboBoxWptColorMode.FindString(
                options.WptColorMode.ToString());
            checkBoxUseWptIcon.Checked = options.UseWptIcon;
            textBoxWptIconUrl.Text = options.WptIconUrl;

            checkBoxPromptOverwrite.Checked = options.PromptToOverwrite;
            checkBoxSendGoogleEarth.Checked = options.SendToGoogleEarth;
        }

        /// <summary>
        /// Gets the controls from the given KmlOptions.
        /// </summary>
        /// <param name="options">The KmlOptions to use.</param>
        private void getControlsFromOptions(KmlOptions options) {
            setValidString(textBoxFileName, options.KmlFileName);
            setValidDouble(textBoxIconScale, options.IconScale);

            setValidString(textBoxTrkColor, options.TrkColor);
            setValidString(textBoxTrkAlpha, options.TrkAlpha); ;
            setValidDouble(textBoxTrkLinewidth, options.TrkLineWidth);
            setValidColorMode(comboBoxTrkColorMode, options.TrkColorMode);
            checkBoxUseTrkIcon.Checked = options.UseTrkIcon;
            checkBoxUseTrkTrack.Checked = options.UseTrkTrack;
            checkBoxUseTrkLines.Checked = options.UseTrkLines;
            setValidString(textBoxTrkIconUrl, options.TrkIconUrl);

            setValidString(textBoxRteColor, options.RteColor);
            setValidString(textBoxRteAlpha, options.RteAlpha);
            setValidDouble(textBoxRteLinewidth, options.RteLineWidth);
            setValidColorMode(comboBoxRteColorMode, options.RteColorMode);
            checkBoxUseRteIcon.Checked = options.UseRteIcon;
            setValidString(textBoxRteIconUrl, options.RteIconUrl);

            setValidString(textBoxWptColor, options.WptColor);
            setValidString(textBoxWptAlpha, options.WptAlpha);
            setValidDouble(textBoxWptLinewidth, options.WptLineWidth);
            setValidString(textBoxWptIconUrl, options.WptIconUrl);
            setValidColorMode(comboBoxWptColorMode, options.WptColorMode);
            checkBoxUseWptIcon.Checked = options.UseWptIcon;
            setValidString(textBoxWptIconUrl, options.WptIconUrl);

            checkBoxPromptOverwrite.Checked = options.PromptToOverwrite;
            checkBoxSendGoogleEarth.Checked = options.SendToGoogleEarth;
        }

        private KmlOptions getOptionsFromDialog() {
            KmlOptions options = new KmlOptions();

            options.KmlFileName = getValidString(textBoxFileName); ;
            options.IconScale = getValidDouble(textBoxIconScale);

            options.TrkColor = getValidString(textBoxTrkColor);
            options.TrkAlpha = getValidString(textBoxTrkAlpha); ;
            options.TrkLineWidth = getValidDouble(textBoxTrkLinewidth);
            options.TrkColorMode = getValidColorMode(comboBoxTrkColorMode);
            options.UseTrkIcon = checkBoxUseTrkIcon.Checked;
            options.UseTrkTrack = checkBoxUseTrkTrack.Checked;
            options.UseTrkLines = checkBoxUseTrkLines.Checked;
            options.TrkIconUrl = getValidString(textBoxTrkIconUrl);

            options.RteColor = getValidString(textBoxRteColor);
            options.RteAlpha = getValidString(textBoxRteAlpha);
            options.RteLineWidth = getValidDouble(textBoxRteLinewidth);
            options.RteColorMode = getValidColorMode(comboBoxRteColorMode);
            options.UseRteIcon = checkBoxUseRteIcon.Checked;
            options.RteIconUrl = getValidString(textBoxRteIconUrl);

            options.WptColor = getValidString(textBoxWptLinewidth);
            options.WptAlpha = getValidString(textBoxWptAlpha);
            options.WptIconUrl = getValidString(textBoxWptIconUrl);
            options.WptLineWidth = getValidDouble(textBoxWptLinewidth);
            options.WptColorMode = getValidColorMode(comboBoxWptColorMode);
            options.UseWptIcon = checkBoxUseWptIcon.Checked;
            options.WptIconUrl = getValidString(textBoxWptIconUrl);

            options.PromptToOverwrite = checkBoxPromptOverwrite.Checked;
            options.SendToGoogleEarth = checkBoxSendGoogleEarth.Checked;

            return options;
        }

        /// <summary>
        /// Get the controls from Settings.
        /// </summary>
        private void getControlsFromSettings() {
            textBoxFileName.Text = Properties.KmlSettings.Default.KmlFileName;
            textBoxIconScale.Text = Properties.KmlSettings.Default.IconScale.ToString();

            textBoxTrkColor.Text = Properties.KmlSettings.Default.TrkColor;
            textBoxTrkAlpha.Text = Properties.KmlSettings.Default.TrkAlpha;
            textBoxTrkLinewidth.Text = Properties.KmlSettings.Default.TrkLineWidth.ToString();
            comboBoxTrkColorMode.SelectedIndex = comboBoxTrkColorMode.FindString(
                Properties.KmlSettings.Default.TrkColorMode);
            checkBoxUseTrkIcon.Checked = Properties.KmlSettings.Default.UseTrkIcon;
            checkBoxUseTrkTrack.Checked = Properties.KmlSettings.Default.UseTrkTrack;
            checkBoxUseTrkLines.Checked = Properties.KmlSettings.Default.UseTrkLines;
            textBoxTrkIconUrl.Text = Properties.KmlSettings.Default.TrkIconUrl;

            textBoxRteColor.Text = Properties.KmlSettings.Default.RteColor;
            textBoxRteAlpha.Text = Properties.KmlSettings.Default.RteAlpha;
            textBoxRteLinewidth.Text = Properties.KmlSettings.Default.RteLineWidth.ToString();
            comboBoxRteColorMode.SelectedIndex = comboBoxRteColorMode.FindString(
                Properties.KmlSettings.Default.RteColorMode);
            checkBoxUseRteIcon.Checked = Properties.KmlSettings.Default.UseRteIcon;
            textBoxRteIconUrl.Text = Properties.KmlSettings.Default.RteIconUrl;

            textBoxWptColor.Text = Properties.KmlSettings.Default.WptColor;
            textBoxWptAlpha.Text = Properties.KmlSettings.Default.WptAlpha;
            textBoxWptLinewidth.Text = Properties.KmlSettings.Default.WptLineWidth.ToString();
            comboBoxWptColorMode.SelectedIndex = comboBoxWptColorMode.FindString(
                Properties.KmlSettings.Default.WptColorMode);
            checkBoxUseWptIcon.Checked = Properties.KmlSettings.Default.UseWptIcon;
            textBoxWptIconUrl.Text = Properties.KmlSettings.Default.WptIconUrl;

            checkBoxPromptOverwrite.Checked = Properties.KmlSettings.Default.PromptToOverwrite;
            checkBoxSendGoogleEarth.Checked = Properties.KmlSettings.Default.SendToGoogleEarth;
        }

        public static KmlOptions getOptionsFromSettings() {
            KmlOptions options = new KmlOptions();

            options.KmlFileName = Properties.KmlSettings.Default.KmlFileName;
            options.IconScale = Properties.KmlSettings.Default.IconScale;

            options.TrkColor = Properties.KmlSettings.Default.TrkColor;
            options.TrkAlpha = Properties.KmlSettings.Default.TrkAlpha;
            options.TrkLineWidth = Properties.KmlSettings.Default.TrkLineWidth;
            try {
                options.TrkColorMode = (KmlColorMode)Enum.Parse(
                    typeof(KmlColorMode), Properties.KmlSettings.Default.TrkColorMode);
            } catch (Exception ex) {
                Utils.excMsg("Failed to parse Properties.KmlSettings.Default.TrkColorMode="
                    + Properties.KmlSettings.Default.TrkColorMode, ex);
            }
            options.UseTrkIcon = Properties.KmlSettings.Default.UseTrkIcon;
            options.UseTrkTrack = Properties.KmlSettings.Default.UseTrkTrack;
            options.UseTrkLines = Properties.KmlSettings.Default.UseTrkLines;
            options.TrkIconUrl = Properties.KmlSettings.Default.TrkIconUrl;

            options.RteColor = Properties.KmlSettings.Default.RteColor;
            options.RteAlpha = Properties.KmlSettings.Default.RteAlpha;
            options.RteLineWidth = Properties.KmlSettings.Default.RteLineWidth;
            try {
                options.RteColorMode = (KmlColorMode)Enum.Parse(
                    typeof(KmlColorMode), Properties.KmlSettings.Default.RteColorMode);
            } catch (Exception ex) {
                Utils.excMsg("Failed to parse Properties.KmlSettings.Default.RteColorMode="
                    + Properties.KmlSettings.Default.TrkColorMode, ex);
            }
            options.UseRteIcon = Properties.KmlSettings.Default.UseRteIcon;
            options.RteIconUrl = Properties.KmlSettings.Default.RteIconUrl;

            options.WptColor = Properties.KmlSettings.Default.WptColor;
            options.WptAlpha = Properties.KmlSettings.Default.WptAlpha;
            options.WptLineWidth = Properties.KmlSettings.Default.WptLineWidth;
            try {
                options.WptColorMode = (KmlColorMode)Enum.Parse(
                    typeof(KmlColorMode), Properties.KmlSettings.Default.WptColorMode);
            } catch (Exception ex) {
                Utils.excMsg("Failed to parse Properties.KmlSettings.Default.WptColorMode="
                    + Properties.KmlSettings.Default.TrkColorMode, ex);
            }
            options.UseWptIcon = Properties.KmlSettings.Default.UseWptIcon;
            options.WptIconUrl = Properties.KmlSettings.Default.WptIconUrl;

            options.PromptToOverwrite = Properties.KmlSettings.Default.PromptToOverwrite;
            options.SendToGoogleEarth = Properties.KmlSettings.Default.SendToGoogleEarth;

            return options;
        }

        private void setSettingsFromDialog() {
            Properties.KmlSettings.Default.KmlFileName = textBoxFileName.Text;
            try {
                Properties.KmlSettings.Default.IconScale = Convert.ToDouble(textBoxIconScale.Text);
            } catch (Exception) {
                // Do nothing
            }

            Properties.KmlSettings.Default.TrkColor = textBoxTrkColor.Text;
            Properties.KmlSettings.Default.TrkAlpha = textBoxTrkAlpha.Text;
            try {
                Properties.KmlSettings.Default.TrkLineWidth = Convert.ToDouble(textBoxTrkLinewidth.Text);
            } catch (Exception) {
                // Do nothing
            }
            if (comboBoxTrkColorMode.SelectedItem != null) {
                Properties.KmlSettings.Default.TrkColorMode = comboBoxTrkColorMode.SelectedItem.ToString();
            }
            Properties.KmlSettings.Default.UseTrkIcon = checkBoxUseTrkIcon.Checked;
            Properties.KmlSettings.Default.UseTrkTrack = checkBoxUseTrkTrack.Checked;
            Properties.KmlSettings.Default.UseTrkLines = checkBoxUseTrkLines.Checked;
            Properties.KmlSettings.Default.TrkIconUrl = textBoxTrkIconUrl.Text;

            Properties.KmlSettings.Default.RteColor = textBoxRteColor.Text;
            Properties.KmlSettings.Default.RteAlpha = textBoxRteAlpha.Text;
            try {
                Properties.KmlSettings.Default.RteLineWidth = Convert.ToDouble(textBoxRteLinewidth.Text);
            } catch (Exception) {
                // Do nothing
            }
            if (comboBoxRteColorMode.SelectedItem != null) {
                Properties.KmlSettings.Default.RteColorMode = comboBoxRteColorMode.SelectedItem.ToString();
            }
            Properties.KmlSettings.Default.UseRteIcon = checkBoxUseTrkIcon.Checked;
            Properties.KmlSettings.Default.RteIconUrl = textBoxRteIconUrl.Text;

            Properties.KmlSettings.Default.WptColor = textBoxWptColor.Text;
            Properties.KmlSettings.Default.WptAlpha = textBoxWptAlpha.Text;
            try {
                Properties.KmlSettings.Default.WptLineWidth = Convert.ToDouble(textBoxWptLinewidth.Text);
            } catch (Exception) {
                // Do nothing
            }
            if (comboBoxWptColorMode.SelectedItem != null) {
                Properties.KmlSettings.Default.WptColorMode = comboBoxWptColorMode.SelectedItem.ToString();
            }
            Properties.KmlSettings.Default.UseWptIcon = checkBoxUseTrkIcon.Checked;
            Properties.KmlSettings.Default.WptIconUrl = textBoxWptIconUrl.Text;

            Properties.KmlSettings.Default.PromptToOverwrite = checkBoxPromptOverwrite.Checked;
            Properties.KmlSettings.Default.SendToGoogleEarth = checkBoxSendGoogleEarth.Checked;
        }

        private void setValidString(TextBox textBox, string strVal) {
            if (string.IsNullOrEmpty(strVal)) textBox.Text = "";
            else textBox.Text = strVal;
        }

        private string getValidString(TextBox textBox) {
            if (string.IsNullOrEmpty(textBox.Text)) return null;
            else return textBox.Text;
        }

        private KmlColorMode getValidColorMode(ComboBox comboBox) {
            try {
                KmlColorMode mode = (KmlColorMode)Enum.Parse(typeof(KmlColorMode), comboBox.Text);
                return mode;
            } catch (Exception) {
                return KmlColorMode.RAINBOW;
            }
        }

        private void setValidColorMode(ComboBox comboBox, KmlColorMode mode) {
            comboBox.SelectedItem = mode.ToString();
        }

        private double getValidDouble(TextBox textBox) {
            if (string.IsNullOrEmpty(textBox.Text)) return Double.NaN;
            try {
                return Convert.ToDouble(textBox.Text);
            } catch (Exception) {
                return Double.NaN;
            }
        }

        private void setValidDouble(TextBox textBox, double val) {
            if (Double.IsNaN(val)) textBox.Text = "";
            else textBox.Text = val.ToString();
        }

        private void OnCancelClick(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Visible = false;
        }

        private void OnOkClick(object sender, System.EventArgs e) {
            // This is the only place KmlOptions is set.
            KmlOptions = getOptionsFromDialog();
            DialogResult = DialogResult.OK;
            Visible = false;
        }

        private void OnUseDefaultsClick(object sender, EventArgs e) {
            KmlOptions options = new KmlOptions();
            getControlsFromOptions(options);
        }

        private void OnSaveClick(object sender, System.EventArgs e) {
            setSettingsFromDialog();
            Properties.KmlSettings.Default.Save();
        }

        private void OnUseSavedClick(object sender, EventArgs e) {
            getControlsFromSettings();
        }

        private void OnBrowseFileClick(object sender, EventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "KML|*.kml";
            dlg.Title = "Select where to save the KML file";
            if (!String.IsNullOrEmpty(textBoxFileName.Text)) {
                dlg.FileName = textBoxFileName.Text;
            }
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                if (dlg.FileName == null) {
                    Utils.warnMsg("Failed to open files to process");
                    return;
                }
                textBoxFileName.Text = dlg.FileName;
            }
        }

        private void textBoxRteLinewidth_TextChanged(object sender, EventArgs e) {

        }
    }
}
