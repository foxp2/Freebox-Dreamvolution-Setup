using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

using Freebox.BootstrapperApplication.Views;
using System.Windows.Media.Animation;

namespace Freebox.BootstrapperApplication.Views
{
    /// <summary>
    /// Interaction logic for InstallView.xaml
    /// </summary>
    public partial class PopupView : UserControl
    {
        
        public PopupView(PopupViewModel PopupViewModel)
        {
            DataContext = PopupViewModel;

            InitializeComponent();

            popupChildWindow.Show();
                        
        }
    }
}
