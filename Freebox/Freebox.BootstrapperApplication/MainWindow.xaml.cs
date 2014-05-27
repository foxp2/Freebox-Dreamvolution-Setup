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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Freebox.BootstrapperApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            // Set the databinding of the main window
            DataContext = viewModel;

            // The view model needs the handle of the current window for the Bootstrapper.
            viewModel.Hwnd = new WindowInteropHelper(this).EnsureHandle();

            InitializeComponent();

            Closing += delegate
            {
                viewModel.Model.Bootstrapper.Dispatcher.InvokeShutdown();
            };
            MouseDown += MainWindow_MouseDown;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Drag the window.
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();

        }

        private void Open_Popup(object sender, RoutedEventArgs e)
        {
            Storyboard FadeIn = (Storyboard)FindResource("FadeIn");
            FadeIn.Begin();
            Storyboard TopCanvasIn = (Storyboard)FindResource("TopCanvasIn");
            TopCanvasIn.Begin();
            popupChildWindow.Show();
        }

        private void Close_Popup(object sender, RoutedEventArgs e)
        {
            Storyboard FadeOut = (Storyboard)FindResource("FadeOut");
            FadeOut.Begin();
            Storyboard TopCanvasOut = (Storyboard)FindResource("TopCanvasOut");
            TopCanvasOut.Begin();
            FadeOut.Completed += FadeOut_Completed;
        }

        private void FadeOut_Completed(object sender, EventArgs e)
        {
            popupChildWindow.Close();
        }
    }
}
