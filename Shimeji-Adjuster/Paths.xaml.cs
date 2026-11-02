using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace Shimeji_Adjuster
{
    public partial class Paths : Window
    {
        public Paths()
        {
            InitializeComponent();
            btn_actionsSelect.Click += (sender, e) => SelectFile(sender, e, txt_actions);
            btn_behaviorsSelect.Click += (sender, e) => SelectFile(sender, e, txt_behaviors);
            btn_settingsSelect.Click += (sender, e) => SelectFile(sender, e, txt_settings, "Properties files (*.properties)|*.properties|All files (*.*)|*.*");
            btn_iconSelect.Click += (sender, e) => SelectFile(sender, e, txt_icon, "PNG files (*.png)|*.png|All files (*.*)|*.*");
        }

        public void PopulatePaths()
        {
            txt_actions.Text = Files.actionsPath;
            txt_behaviors.Text = Files.behaviorPath;
            txt_settings.Text = Files.settingsPath;
            txt_icon.Text = Files.iconPath;
        }

        private void SelectFile(object sender, RoutedEventArgs e, TextBox txtbox, string pattern = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = pattern != null ? pattern : "XML files (*.xml)|*.xml|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                // Get the path of the selected file
                string filePath = openFileDialog.FileName;
                txtbox.Text = filePath;
            }
        }
    }
}
