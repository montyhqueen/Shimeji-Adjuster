using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Shimeji_Adjuster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath;
        string shimejiIcon;
        string shimejiName;
        XDocument doc;
        string? frequency = "";

        public MainWindow()
        {
            InitializeComponent();

            string currentPath = Directory.GetCurrentDirectory();
            shimejiIcon = "";
            filePath = "";
            shimejiName = "";

            try
            {
                string fullPath = AppContext.BaseDirectory;
                string cleanPath = Path.GetDirectoryName(fullPath);
                shimejiName = Path.GetFileName(cleanPath);
                lbl_name.Content = shimejiName;

                shimejiIcon = currentPath + @"\img\Shimeji\shime1.png";
                img_shimeji.Source = new BitmapImage(new Uri(shimejiIcon, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load icon: \n" + shimejiIcon, "Load Failed", MessageBoxButton.OK);
            }

            try
            {
                filePath = currentPath + @"\conf\behaviors.xml";
                doc = XDocument.Load(filePath);

                //string behaviorName = "ClimbAlongCeiling";
                XNamespace ns = "http://www.group-finity.com/Mascot";
                string behaviorName = "SitDown";

                XElement? behavior = doc
                .Descendants(ns + "Behavior")
                .FirstOrDefault(b =>
                    (string?)b.Attribute("Name") == behaviorName);

                if (behavior == null)
                {
                    Console.WriteLine(
                        $"Behavior '{behaviorName}' was not found.");
                    return;
                }
                frequency = (string?)behavior?.Attribute("Frequency");
                slider_sittingAnimation.Value = double.Parse(frequency) / 25;


                behaviorName = "ClimbAlongWall";

                behavior = doc
                .Descendants(ns + "Behavior")
                .FirstOrDefault(b =>
                    (string?)b.Attribute("Name") == behaviorName);

                if (behavior == null)
                {
                    Console.WriteLine(
                        $"Behavior '{behaviorName}' was not found.");
                    return;
                }
                frequency = (string?)behavior?.Attribute("Frequency");
                slider_climb.Value = Double.Parse(frequency) / 25;


                behaviorName = "PullUpShimeji";
                behavior = doc
                .Descendants(ns + "Behavior")
                .FirstOrDefault(b =>
                    (string?)b.Attribute("Name") == behaviorName);

                if (behavior == null)
                {
                    Console.WriteLine(
                        $"Behavior '{behaviorName}' was not found.");
                    return;
                }
                frequency = (string?)behavior?.Attribute("Frequency");
                slider_multiply.Value = Double.Parse(frequency) / 25;


                behaviorName = "ThrowIEFromLeft";
                behavior = doc
                .Descendants(ns + "Behavior")
                .FirstOrDefault(b =>
                    (string?)b.Attribute("Name") == behaviorName);

                if (behavior == null)
                {
                    Console.WriteLine(
                        $"Behavior '{behaviorName}' was not found.");
                    return;
                }
                frequency = (string?)behavior?.Attribute("Frequency");
                slider_throw.Value = Double.Parse(frequency) / 25;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load behaviors file: \n" + filePath, "Load Failed", MessageBoxButton.OK);
            }

            sLabel.Text = $"Value: {ValueConverter(slider_sittingAnimation.Value):F0}";
            tLabel.Text = $"Value: {ValueConverter(slider_throw.Value):F0}";
            cLabel.Text = $"Value: {ValueConverter(slider_climb.Value):F0}";
            multLabel.Text = $"Value: {ValueConverter(slider_multiply.Value):F0}";
            mLabel.Text = $"Value: {ValueConverter(slider_misc.Value):F0}";
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



        private void sit_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double val = ValueConverter(e.NewValue);
            sLabel.Text = $"Value: {val:F0}";
        }

        private bool UpdateBehaviorFrequency(string behaviorName, double frequency)
        {
            XNamespace ns = "http://www.group-finity.com/Mascot";

            frequency = ValueConverter(frequency);

            XDocument doc = XDocument.Load(filePath);

            XElement? behavior = doc
                .Descendants(ns + "Behavior")
                .FirstOrDefault(b =>
                    (string?)b.Attribute("Name") == behaviorName);

            if (behavior == null)
                return false;

            behavior.SetAttributeValue("Frequency", frequency);
            doc.Save(filePath);

            return true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateBehaviorFrequency("ThrowIEFromLeft", slider_throw.Value);
            UpdateBehaviorFrequency("ThrowIEFromRight", slider_throw.Value);
            UpdateBehaviorFrequency("WalkAndThrowIEFromLeft", slider_throw.Value);
            UpdateBehaviorFrequency("WalkAndThrowIEFromRight", slider_throw.Value);

            UpdateBehaviorFrequency("PullUpShimeji", slider_multiply.Value);

            return;
        }
    }
}