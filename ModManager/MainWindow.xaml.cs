using GalaSoft.MvvmLight.CommandWpf;
using ModManager.Model;
using ModManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ModManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            (DataContext as MainMenuViewModel).StackPanelSlideMenu = this.StackPanelSlideMenu;
            (DataContext as MainMenuViewModel).StackPanelModList = this.StackPanelModList;
            (DataContext as MainMenuViewModel).AssemblerImageButton = this.btnImageAssembler;
            (DataContext as MainMenuViewModel).StackPanelGameList = this.StackPanelGameList;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
