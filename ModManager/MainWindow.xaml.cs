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
            StackPanelSlideMenu.Children.Add(new System.Windows.Controls.Button
            {
                Template = (ControlTemplate)System.Windows.Application.Current.Resources["SlideMenuButton"],
                Content = "Test1",
                FontSize = 15,
                Command = (this.DataContext as MainMenuViewModel)?.OpenAssembler,
                Resources = {
                    { "Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/gear.png", UriKind.Absolute))}
                }
            });
            (DataContext as MainMenuViewModel).StackPanelSlideMenu = this.StackPanelSlideMenu;
            (DataContext as MainMenuViewModel).StackPanelModList = this.StackPanelModList;
            (DataContext as MainMenuViewModel).AssemblerImageButton = this.btnImageAssembler;
            //(DataContext as MainMenuViewModel).GetAssemblers( StackPanelSlideMenu);
            //AddAssemblers();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void AddAssemblers()
        {
            foreach(string str in Directory.GetDirectories(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder")))
            {
                DirectoryInfo directory = new DirectoryInfo(str);
                StackPanelSlideMenu.Children.Add(new System.Windows.Controls.Button
                {
                    Template = (ControlTemplate)System.Windows.Application.Current.Resources["SlideMenuButton"],
                    Content = directory.Name,
                    Command = (this.DataContext as MainMenuViewModel)?.OpenAssembler,
                    CommandParameter = "Button1",
                    Resources = {
                    { "Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/gear.png", UriKind.Absolute))}
                }
                });
            }
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
