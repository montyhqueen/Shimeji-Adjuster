using System.IO;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Shimeji_Adjuster
{
    public partial class MainWindow : Window
    {
        string? shimejiName;
        XDocument doc;
        Paths paths;

        public MainWindow()
        {
            InitializeComponent();
            SetUpGUI();

            paths = new Paths();
            paths.PopulatePaths();
        }

        public void SetUpGUI()
        {
            string currentPath = Directory.GetCurrentDirectory();
            string? frequency = "";
            shimejiName = "";

            try
            {
                //--- Set the name for the current Shimeji---//
                string fullPath = AppContext.BaseDirectory;
                string? cleanPath = Path.GetDirectoryName(fullPath);
                shimejiName = Path.GetFileName(cleanPath);
                lbl_name.Content = shimejiName;
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to read current folder.");
            }

            //---Get the actions file---//
            Files.actionsPath = currentPath + @"\conf\actions.xml";
            if (!File.Exists(Files.actionsPath))
                MessageBox.Show("Failed to load actions file: \n" + Files.actionsPath, "Load Failed", MessageBoxButton.OK);

            //---and the settings file---//
            Files.settingsPath = currentPath + @"\conf\settings.properties";
            if (!File.Exists(Files.settingsPath))
                MessageBox.Show("Failed to load settings file: \n" + Files.settingsPath, "Load Failed", MessageBoxButton.OK);

            try
            {
                //---Set the icon---//
                Files.iconPath = currentPath + @"\img\Shimeji\shime1.png";
                img_shimeji.Source = new BitmapImage(new Uri(Files.iconPath, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load icon: \n" + Files.iconPath, "Load Failed", MessageBoxButton.OK);
            }

            try
            {
                XElement? behavior = null;

                //---Load the behaviors file---//
                Files.behaviorPath = currentPath + @"\conf\behaviors.xml";
                doc = XDocument.Load(Files.behaviorPath);

                string behaviorName = "SitDown";
                behavior = GetBehavior(behaviorName, slider_sittingAnimation);

                behaviorName = "ClimbAlongWall";
                behavior = GetBehavior(behaviorName, slider_climb);

                behaviorName = "PullUpShimeji";
                behavior = GetBehavior(behaviorName, slider_multiply);

                behaviorName = "ThrowIEFromLeft";
                behavior = GetBehavior(behaviorName, slider_throw);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load behaviors file: \n" + Files.behaviorPath, "Load Failed", MessageBoxButton.OK);
            }

            sLabel.Text = $"Value: {ValueConverter(slider_sittingAnimation.Value):F0}";
            tLabel.Text = $"Value: {ValueConverter(slider_throw.Value):F0}";
            cLabel.Text = $"Value: {ValueConverter(slider_climb.Value):F0}";
            multLabel.Text = $"Value: {ValueConverter(slider_multiply.Value):F0}";
            mLabel.Text = $"Value: {ValueConverter(slider_misc.Value):F0}";
        }

        public XElement? GetBehavior(string behaviorName, Slider slider)
        {
            XElement? behavior = null;
            XNamespace ns = "http://www.group-finity.com/Mascot";

            behavior = doc.Descendants(ns + "Behavior")
                          .FirstOrDefault(b => (string?)b.Attribute("Name") == behaviorName);

            if (behavior == null)
            {
                Console.WriteLine(
                    $"Behavior '{behaviorName}' was not found.");
            }
            else
            {
                string? frequency = (string?)behavior?.Attribute("Frequency");
                slider.Value = double.Parse(frequency) / 25;
            }

            return behavior;
        }

        // ----- Setting labels and stuff -----
        private double ValueConverter(double i)
        {
            return i * 25;
        }

        private void slider_misc_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double val = ValueConverter(e.NewValue);
            mLabel.Text = $"Value: {val:F0}";
        }

        private void multiply_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double val = ValueConverter(e.NewValue);
            multLabel.Text = $"Value: {val:F0}";
        }

        private void climb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double val = ValueConverter(e.NewValue);
            cLabel.Text = $"Value: {val:F0}";
        }

        private void throw_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double val = ValueConverter(e.NewValue);
            tLabel.Text = $"Value: {val:F0}";
        }

        private void HandleClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            paths.Close();
        }

        private void sit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double val = ValueConverter(e.NewValue);
            sLabel.Text = $"Value: {val:F0}";
        }

        private bool UpdateBehaviorFrequency(string behaviorName, double frequency)
        {
            XNamespace ns = "http://www.group-finity.com/Mascot";
            XDocument doc = XDocument.Load(Files.behaviorPath);

            frequency = ValueConverter(frequency);

            XElement? behavior = doc
                .Descendants(ns + "Behavior")
                .FirstOrDefault(b =>
                    (string?)b.Attribute("Name") == behaviorName);

            if (behavior == null)
                return false;

            behavior.SetAttributeValue("Frequency", frequency);
            doc.Save(Files.behaviorPath);

            return true;
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            // Stealing window actions
            UpdateBehaviorFrequency("ThrowIEFromLeft", slider_throw.Value);
            UpdateBehaviorFrequency("ThrowIEFromRight", slider_throw.Value);
            UpdateBehaviorFrequency("WalkAndThrowIEFromLeft", slider_throw.Value);
            UpdateBehaviorFrequency("WalkAndThrowIEFromRight", slider_throw.Value);

            // Multiplying actions
            UpdateBehaviorFrequency("PullUpShimeji", slider_multiply.Value);
            UpdateBehaviorFrequency("SplitIntoTwo", slider_multiply.Value);

            // Climbing actions
            UpdateBehaviorFrequency("ClimbHalfwayAlongWall", slider_throw.Value);
            UpdateBehaviorFrequency("ClimbAlongWall", slider_throw.Value);
            UpdateBehaviorFrequency("ClimbIEWall", slider_throw.Value);
            UpdateBehaviorFrequency("ClimbIEBottom", slider_throw.Value);

            // Sitting
            UpdateBehaviorFrequency("SitDown", slider_throw.Value);

            return;
        }

        // Menu bar functions
        private void SetPaths(object sender, RoutedEventArgs e)
        {
            paths.ShowDialog();
        }
    }
}