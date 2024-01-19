using GalaSoft.MvvmLight.CommandWpf;
using ModManager.Model;
using ModManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
using Xceed.Wpf.Toolkit;

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
            (DataContext as MainMenuViewModel).ColorPicker = this.ColorPickerForPanels;
            //ColorPickerForPanels.SelectedColor = (Color)ColorConverter.ConvertFromString(new IniFile("../../../Configs/Settings.ini").Read("ColorPanels"));
            //RedrawBackground();
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            //System.Windows.MessageBox.Show((sender as ColorPicker).SelectedColor.ToString());
            (DataContext as MainMenuViewModel).ColorPanels = new SolidColorBrush((Color)((sender as ColorPicker).SelectedColor));
            //new IniFile("../../../Configs/Settings.ini").Write("ColorPanels", (sender as ColorPicker).SelectedColor.ToString());
            //
            //
            //
            //
            //
            //Сделал что бы можно было менять цвет осталось всё структурировать и настроить и можно доделывать основной функционал 
            //
            //
            //
            //
            //
            //(System.Windows.Media.Brush)new BrushConverter().ConvertFrom((sender as ColorPicker).SelectedColor);

        }
        /*private void RedrawBackground()
        {
            // Создание нового объекта BitmapImage из файла
            BitmapImage image = new BitmapImage(new Uri("../../../Source/Images/Backgrounds/BackgroundStandartV4.png", UriKind.Relative)); // замените "image_path.jpg" на путь к вашему изображению
            // Создание нового объекта ImageBrush с использованием BitmapImage
            ImageBrush imageBrush = new ImageBrush(image);

            // Установка ImageBrush в качестве фонового изображения для окна
            this.Background = imageBrush;
            //this.Background = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7");
        }*/
    }
}
