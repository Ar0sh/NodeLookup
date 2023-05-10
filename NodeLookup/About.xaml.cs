using System.Windows;

namespace NodeLookup
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public static string version = "2.1.2";
        public About()
        {
            InitializeComponent();
            lbVersion.Content = "Version " + version;
        }
    }
}
