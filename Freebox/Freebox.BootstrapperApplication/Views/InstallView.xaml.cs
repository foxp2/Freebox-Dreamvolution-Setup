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
    /// Interaction logic for InstallView.xaml
    /// </summary>
    public partial class InstallView : UserControl
    {
        public InstallView(InstallViewModel installViewModel)
        {
            // Set the databinding to the view
            DataContext = installViewModel;

            InitializeComponent();

            this.Loaded += SlideIn;  
        }

        private void SlideIn(object sender, System.Windows.RoutedEventArgs e)
        {
            Storyboard SlideIn = (Storyboard)FindResource("SlideIn");
            SlideIn.Begin();
        }

        private void SlideOut(object sender, RoutedEventArgs e)
        {
            Storyboard SlideOut = (Storyboard)FindResource("SlideOut");
            SlideOut.Begin();
        }

        private void Open_Popup(object sender, RoutedEventArgs e)
        {
            Storyboard FadeIn = (Storyboard)FindResource("ChildFadeIn");
            FadeIn.Begin();
            Storyboard TopCanvasIn = (Storyboard)FindResource("TopCanvasIn");
            TopCanvasIn.Begin();
            childpopupChildWindow.Show();
        }

        private void Close_Popup(object sender, RoutedEventArgs e)
        {
            Storyboard FadeOut = (Storyboard)FindResource("ChildFadeOut");
            FadeOut.Begin();
            Storyboard TopCanvasOut = (Storyboard)FindResource("TopCanvasOut");
            TopCanvasOut.Begin();
            FadeOut.Completed += FadeOut_Completed;
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            childpopupChildWindow.Close();
        }
    }
}