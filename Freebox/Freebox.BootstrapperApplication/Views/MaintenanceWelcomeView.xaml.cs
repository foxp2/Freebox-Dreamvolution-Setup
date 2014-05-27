using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Freebox.BootstrapperApplication.ViewModels;
using System.Windows.Media.Animation;

namespace Freebox.BootstrapperApplication.Views
{
    /// <summary>
    /// Interaction logic for MaintenanceWelcomeView.xaml
    /// </summary>
    public partial class MaintenanceWelcomeView : UserControl
    {
        public MaintenanceWelcomeView(MaintenanceWelcomeViewModel maintenanceWelcomeViewModel)
        {
            // Set the databinding to the view
            DataContext = maintenanceWelcomeViewModel;

            InitializeComponent();

            this.Loaded += SlideIn;  
        }

        private void SlideIn(object sender, System.Windows.RoutedEventArgs e)
        {
            Storyboard SlideIn = (Storyboard)FindResource("SlideIn");
            SlideIn.Begin();
        }
    }
}
