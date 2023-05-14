using GalaSoft.MvvmLight.Command;
using ModManager.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ModManager.ViewModel
{
    public class MainMenuViewModel : BaseViewModel
    {
        #region Поля
        private WindowState _stateWindow = WindowState.Normal;
        private FileSystemWatcher _watcher = new FileSystemWatcher();
        private bool _newAssembler = false;

        #region Поля видимости различных элементов
        private Visibility _optionsVisible = Visibility.Hidden;
        private Visibility _importVisibleIndicatorOn;
        private Visibility _importVisibleIndicatorOff;
        private Visibility _assemblerModlistInfoVisible = Visibility.Visible;
        private Visibility _controlsAssemblerVisible = Visibility.Visible;
        #endregion
        private bool _canEditNameAssembler = true; // Поля отвечающее на вопрос - можно ли редактировать название сборки (Переключается карандашиком)
        private string _assemblerName = ""; // Поле - название сборки (Необходимо для отображения шапки)
        private string _pathToTheModsFolder = ""; //Хранит путь до папки mods у игры
        private string _pathToTheAssemblersFolder = ""; //Хранит путь до папки, куда будут сохранятся сборки;
        private System.Windows.Controls.Button _assemblerImageButton; //ImageButton отображает иконку сборки и позволяет её менять;
        private StackPanel _stackPanelSlideMenu; // Боковое меню
        private StackPanel _stackPanelModList; //Список модов
        #endregion
        #region Свойства
        public StackPanel StackPanelModList //Свойство взаимодействующее с полем _stackPanelModList
        {
            get => _stackPanelModList;
            set
            {
                _stackPanelModList = value;
            }
        }
        public StackPanel StackPanelSlideMenu //Свойство взаимодействующее с полем _stackPanelSlideMenu
        {
            get => _stackPanelSlideMenu;
            set
            {
                _stackPanelSlideMenu = value;
                GetAssemblers();
            }
        }
        public System.Windows.Controls.Button AssemblerImageButton //Свойство взаимодействующее с полем _aassemblerImageButton
        {
            get => _assemblerImageButton;
            set
            {
                _assemblerImageButton = value;
                _assemblerImageButton.IsEnabled = false;
            }
        }

        #region Свойства для определения, какую панельку кнопок использовать у сборки (Редактриование или выбор)
        public Visibility ControlAssemblerVisible //Свойство взаимодействующее с полем _controlsAssemblerVisible, служит для получения точного значения
        {
            get => _controlsAssemblerVisible;
        }
        public Visibility ControlAssemblerVisibility //Свойство взаимодействующее с полем _controlsAssemblerVisible, служит для установки значения;
        {
            set
            {
                _controlsAssemblerVisible = value;
                OnPropertyChanged(nameof(ControlAssemblerVisible));
                OnPropertyChanged(nameof(ControlUpdateAssemblerVisible));
            }
        }
        public Visibility ControlUpdateAssemblerVisible //Свойство взаимодействующее с полем _controlsAssemblerVisible, служит для получения противопроржного значения
        {
            get
            {
                if (_controlsAssemblerVisible == Visibility.Visible)
                {
                    return Visibility.Hidden;
                }
                else return Visibility.Visible;
            }
        }
        #endregion
        public bool CanEditNameAssembler //Свойство взаимодействующее с полем _canEditNameAssembler служит для получения значения
        {
            get => _canEditNameAssembler;
        }
        public bool CanEditNameAssemblerSwitch //Свойство взаимодействующее с полем _canEditNameAssembler служит для получения инверсированного значения и его установки
        {
            get => !_canEditNameAssembler;
            set
            {
                _canEditNameAssembler = !value;
                OnPropertyChanged(nameof(CanEditNameAssembler));
            }
        }
        public Visibility AssemblerModlistInfoVisible //Свойство для взаимодействия с полем _assemblerModlistInfoVisible
        {
            get => _assemblerModlistInfoVisible;
            set
            {
                _assemblerModlistInfoVisible = value;
                OnPropertyChanged(nameof(AssemblerModlistInfoVisible));
            }
        }
        public Visibility ImportVisibleIndicatorOn //Свойство для взаимодействия с полем _importVisibleIndicatorOn
        {
            get => _importVisibleIndicatorOn;
            set 
            { 
                _importVisibleIndicatorOn = value;
                OnPropertyChanged(nameof(ImportVisibleIndicatorOn));
            }
        }


        public Visibility ImportVisibleIndicatorOff //Свойство для взаимодействия с полем _importVisibleIndicatorOff
        {
            get => _importVisibleIndicatorOff;
            set
            {
                _importVisibleIndicatorOff = value;
                OnPropertyChanged(nameof(ImportVisibleIndicatorOff));
            }
        }
        public string AssemblerName //Свойство для взаимодействия с полем _assemblerName
        {
            get => _assemblerName;
            set
            {
                _assemblerName = value;
                OnPropertyChanged(nameof(AssemblerName));
            }
        }

        public string PathToTheAssemblersFolder //Свойство для взаимодействия с полем _pathToTheAssemblersFolder
        {
            get => _pathToTheAssemblersFolder;
            set
            {
                _pathToTheAssemblersFolder = value;
                OnPropertyChanged(nameof(PathToTheAssemblersFolder));
            }
        }
        public string PathToTheModsFolder //Свойство для взаимодействия с полем _pathToTheModsFolder
        {
            get => _pathToTheModsFolder;
            set
            {
                _pathToTheModsFolder = value;
                OnPropertyChanged(nameof(PathToTheModsFolder));
            }
        }
        public WindowState StateWindow //Свойство для взаимодействия с полем _stateWindow
        {
            get => _stateWindow;
            set
            {
                _stateWindow = value;
                OnPropertyChanged(nameof(StateWindow));
            }
        }
        public Visibility OptionsVisible //Свойство для взаимодействия с полем _optionsVisible
        {
            get => _optionsVisible;
            set
            {
                _optionsVisible = value;
                OnPropertyChanged(nameof(OptionsVisible));
            }
        }
        #endregion
        #region Команды кнопок
        public ICommand CloseApplication 
        {
            get
            {
                return new RelayCommand(() => { foreach (Window w in App.Current.Windows) w.Close(); });
            }
        }
        public ICommand MinimizeWindow
        {
            get
            {
                return new RelayCommand(() => { StateWindow = WindowState.Minimized; });
            }
        }
        public ICommand BrowseModsFolder
        {
            get
            {
                return new RelayCommand(() => {
                    var dialog = new FolderBrowserDialog();
                    dialog.InitialDirectory= PathToTheModsFolder;
                    dialog.SelectedPath = PathToTheModsFolder;

                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        PathToTheModsFolder = dialog.SelectedPath;
                    }
                });
            }
        }
        public ICommand BrowseAssemblersFolder
        {
            get
            {
                return new RelayCommand(() => {
                    var dialog = new FolderBrowserDialog();
                    dialog.InitialDirectory = PathToTheAssemblersFolder;
                    dialog.SelectedPath = PathToTheAssemblersFolder;

                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        PathToTheAssemblersFolder = dialog.SelectedPath;
                    }
                });
            }
        }
        public ICommand OpenAssembler
        {
            get
            {
                return new RelayCommand<object>((parameter) => {
                    // здесь можно использовать информацию о том, какая кнопка вызвала эту команду
                    /*if (parameter is System.Windows.Controls.Button button)
                    {
                        System.Windows.MessageBox.Show($"Команда вызвана с кнопки: {button.Content}");
                    }*/
                    //System.Windows.MessageBox.Show(parameter.ToString());
                    AssemblerModlistInfoVisible = Visibility.Visible;
                    GetInformationInSelectedAssembler(parameter.ToString());
                });
            }
        }
        public ICommand OpenOptions
        {
            get
            {
                return new RelayCommand(() => {
                    //PathToTheModsFolder = IniFile()
                    if (File.Exists("../../../Configs/Settings.ini"))
                    {
                        IniFile ini = new IniFile("../../../Configs/Settings.ini");
                        PathToTheModsFolder = ini.Read("PathToTheModsFolder");
                        PathToTheAssemblersFolder = ini.Read("PathToTheAssemblersFolder");
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Файл конфигурации Settings.ini не найден", "Ошибка..", MessageBoxButton.OK, MessageBoxImage.Error);
                        PathToTheModsFolder = "Missing file...";
                        PathToTheAssemblersFolder = "Missing file...";
                    }
                    OptionsVisible = Visibility.Visible;
                });
            }
        }
        
        public ICommand CancleOptions
        {
            get
            {
                return new RelayCommand(() => {
                    OptionsVisible = Visibility.Hidden;
                });
            }
        }
        public ICommand SaveOptions
        {
            get
            {
                return new RelayCommand(() => {
                    if (File.Exists("../../../Configs/Settings.ini"))
                    {
                        IniFile ini = new IniFile("../../../Configs/Settings.ini");
                        ini.Write("PathToTheModsFolder", PathToTheModsFolder);
                        _watcher.Path = PathToTheModsFolder;
                        if (ini.Read("PathToTheAssemblersFolder") != PathToTheAssemblersFolder)
                        {
                            ini.Write("PathToTheAssemblersFolder", PathToTheAssemblersFolder);
                            GetAssemblers();
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Файл конфигурации Settings.ini не найден", "Ошибка..", MessageBoxButton.OK, MessageBoxImage.Error);
                        PathToTheModsFolder = "Missing file...";
                    }
                    //OptionsVisible = Visibility.Hidden;
                });
            }
        }

        public ICommand CreateAssembler
        {
            get
            {
                return new RelayCommand(() => {
                    AssemblerModlistInfoVisible= Visibility.Visible;
                    AssemblerImageButton.IsEnabled = true;
                    IniFile iniFile = new IniFile("../../../Configs/Settings.ini");
                    DirectoryInfo directoryInfoAssembler = new DirectoryInfo(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), "Сборка_#" + iniFile.Read("NumberOfAssembler")));
                    directoryInfoAssembler.Create();
                    iniFile.Write("NumberOfAssembler", (int.Parse(iniFile.Read("NumberOfAssembler")) + 1).ToString());
                    new DirectoryInfo(Path.Combine(directoryInfoAssembler.FullName, "Assets")).Create();
                    GetAssemblers();
                    System.Windows.Controls.Button button = StackPanelSlideMenu.Children.OfType<System.Windows.Controls.Button>().LastOrDefault();
                    button.Focus();
                    OpenAssembler.Execute(button.Content);
                    ClearModsList();
                    foreach (string str in Directory.GetFiles(new IniFile("../../../Configs/Settings.ini").Read("PathToTheModsFolder")))
                    {
                        if (new FileInfo(str).Extension == ".jar")
                            StackPanelModList.Children.Add(new System.Windows.Controls.Label
                            {
                                FontSize = 15,
                                Margin = new Thickness(30, 0, 30, 0),
                                Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                                Content = new FileInfo(str).Name
                            });
                    }
                    _newAssembler = true;
                    _watcher.Path = new IniFile("../../../Configs/Settings.ini").Read("PathToTheModsFolder");

                    _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                    _watcher.Filter = "*.jar"; // Здесь указывается фильтр файлов, которые нужно отслеживать (в данном случае все файлы)
                    _watcher.Changed += OnChanged;
                    _watcher.Created += OnChanged;
                    _watcher.Deleted += OnChanged;
                    _watcher.Renamed += OnChanged;
                    _watcher.EnableRaisingEvents = true;


                    //if (_controlsAssemblerVisible == Visibility.Visible)
                    ControlAssemblerVisibility = Visibility.Hidden;
                    //else ControlAssemblerVisibility = Visibility.Visible;
                });
            }
        }
        
        public ICommand EditImageAssembler
        {
            get
            {
                return new RelayCommand(() =>
                {
                    var dialog = new OpenFileDialog();
                    dialog.InitialDirectory = PathToTheAssemblersFolder;
                    dialog.Filter = "PNG (*.png)|*.png";
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        IniFile ini = new IniFile("../../../Configs/Settings.ini");
                        //File.Copy(dialog.FileName, Path.Combine(ini.Read("PathToTheAssemblersFolder"),));
                        if (File.Exists(Path.Combine(ini.Read("PathToTheAssemblersFolder"), Path.Combine(AssemblerName, Path.Combine("Assets", "img.png")))))
                        {
                            //AssemblerImageButton.Resources.Remove("Img");
                            //_assemblerImageButton.Resources.Remove("Img");
                            //System.Windows.Controls.Button button = StackPanelSlideMenu.Children.OfType<System.Windows.Controls.Button>().FirstOrDefault(button => button.Content.ToString() == AssemblerName);
                            //button.Resources.Remove("Img");
                            /*string path = Path.Combine(ini.Read("PathToTheAssemblersFolder"), Path.Combine(AssemblerName, Path.Combine("Assets", "img.png")));
                            File.Copy(dialog.FileName, path);
                            AssemblerImageButton.Resources.Add("Img", new BitmapImage(new Uri(path)));
                            button.Resources.Add("Img", new BitmapImage( new Uri(path)));*/
                            //File.Delete(Path.Combine(ini.Read("PathToTheAssemblersFolder"), Path.Combine(AssemblerName, Path.Combine("Assets", "img.png"))));
                        }
                        else
                        {
                            AssemblerImageButton.Resources.Remove("Img");
                            File.Copy(dialog.FileName, Path.Combine(ini.Read("PathToTheAssemblersFolder"), Path.Combine(AssemblerName, Path.Combine("Assets", "img.png"))));
                            AssemblerImageButton.Resources.Add("Img", new BitmapImage(new Uri(Path.Combine(ini.Read("PathToTheAssemblersFolder"), Path.Combine(AssemblerName, Path.Combine("Assets", "img.png"))), UriKind.Absolute)));
                            GetAssemblers();
                        }
                    }
                });
            }
        }

        #region Кнопки одной сборки
        public ICommand LoadAssembler
        {
            get
            {
                return new RelayCommand(() => {
                    foreach (string str in Directory.GetFiles(new IniFile("../../../Configs/Settings.ini").Read("PathToTheModsFolder")))
                    {
                        if (new FileInfo(str).Extension == ".jar")
                        {
                            File.Delete(str);
                        }
                            
                    }
                    foreach (var file in Directory.GetFiles(Path.Combine(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder"), AssemblerName)))
                    {
                        if (new FileInfo(file).Extension == ".jar")
                            File.Copy(file, Path.Combine(new IniFile("../../../Configs/Settings.ini").Read("PathToTheModsFolder"), new FileInfo(file).Name));
                    }
                });
            }
        }
        public ICommand UnloadAssembler
        {
            get
            {
                return new RelayCommand(() => {

                });
            }
        }
        public ICommand DeleteAssembler
        {
            get
            {
                return new RelayCommand(() => {

                });
            }
        }
        public ICommand SaveAssemblerOption
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ControlAssemblerVisibility = Visibility.Visible;
                    AssemblerImageButton.IsEnabled = false;
                    if (_newAssembler)
                    {
                        _watcher.EnableRaisingEvents = false;
                        foreach (var file in Directory.GetFiles(new IniFile("../../../Configs/Settings.ini").Read("PathToTheModsFolder")))
                        {
                            if(new FileInfo(file).Extension == ".jar")
                            File.Copy(file, Path.Combine(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder"), Path.Combine(AssemblerName, new FileInfo(file).Name)));
                        }
                        _newAssembler = false;
                        System.Windows.Controls.Button button = StackPanelSlideMenu.Children.OfType<System.Windows.Controls.Button>().LastOrDefault();
                        button.Focus();
                        OpenAssembler.Execute(button.Content);
                    }
                    
                });
            }
        }
        public ICommand OptionAssembler
        {
            get
            {
                return new RelayCommand(() =>
                {
                    ControlAssemblerVisibility = Visibility.Hidden;
                    AssemblerImageButton.IsEnabled = true;
                });
            }
        }
        public ICommand CancleAssemblerOption
        {
            get
            {
                return new RelayCommand(() => {

                });
            }
        }
        #endregion
        #endregion

        #region Методы
        public void ClearAssemblersList()
        {
            StackPanelSlideMenu.Children.Clear();
        }
        public void ClearModsList()
        {
            StackPanelModList.Children.Clear();
        }

        public void GetAssemblers()
        {
            if(StackPanelSlideMenu.Children.Count > 0)
            {
                StackPanelSlideMenu.Children.Clear();
            }
            if (!File.Exists("../../../Configs/Settings.ini"))
            {
                IniFile ini = new IniFile("../../../Configs/Settings.ini");
                CreateMainSettingsINI();
            }
            if (new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder") == "")
            {
                return;
            }
            foreach (string str in Directory.GetDirectories(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder")))
            {
                DirectoryInfo directory = new DirectoryInfo(str);
                System.Windows.Controls.Button button = new System.Windows.Controls.Button
                {
                    Template = (ControlTemplate)System.Windows.Application.Current.Resources["SlideMenuButton"],
                    Content = directory.Name,
                    Command = OpenAssembler,
                    CommandParameter = directory.Name,
                    Resources = {
                        { "Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/img.png", UriKind.Absolute))},
                    }
                };
                if (File.Exists(Path.Combine(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder"), Path.Combine(directory.Name, Path.Combine("Assets", "img.png")))))
                {
                    button.Resources.Remove("Img");
                    button.Resources.Add("Img", new BitmapImage(new Uri(Path.Combine(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder"), Path.Combine(directory.Name, Path.Combine("Assets", "img.png"))), UriKind.Absolute)));
                }
                else
                {
                    button.Resources.Remove("Img");
                    button.Resources.Add("Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/img.png", UriKind.Absolute)));
                }
                StackPanelSlideMenu.Children.Add(button);
            }
            
        }
        public void GetInformationInSelectedAssembler(string? assembler)
        {
            if (File.Exists(Path.Combine(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder"), Path.Combine(assembler, Path.Combine("Assets", "img.png")))))
            {
                AssemblerImageButton.Resources.Remove("Img");
                AssemblerImageButton.Resources.Add("Img", new BitmapImage(new Uri(Path.Combine(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder"), Path.Combine(assembler, Path.Combine("Assets", "img.png"))), UriKind.Absolute)));
            }
            else 
            {
                AssemblerImageButton.Resources.Remove("Img");
                AssemblerImageButton.Resources.Add("Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/img.png", UriKind.Absolute))); 
            }
            

            AssemblerName = assembler;
            if (StackPanelModList.Children.Count > 0)
            {
                StackPanelModList.Children.Clear();
            }
            foreach (string str in Directory.GetFiles(Path.Combine(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder"), assembler)))
            {
                if(new FileInfo(str).Extension == ".jar")
                StackPanelModList.Children.Add(new System.Windows.Controls.Label
                {
                    //FontSize = "25" Margin = "30,0" Foreground = "#FFC7C7C7" Content = "Test"
                    FontSize = 15,
                    Margin = new Thickness(30, 0, 30, 0),
                    Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                    Content = new FileInfo(str).Name
                });
            }
            /*foreach ()
            if(Directory.GetFiles(Path.Combine(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder"), AssemblerName), "*.jar") == Directory.GetFiles(new IniFile("../../../Configs/Settings.ini").Read("PathToTheModsFolder"), "*.jar"))
            {
                System.Windows.MessageBox.Show("Соответствует");
            }
            else
            {
                System.Windows.MessageBox.Show("Не соответствует");
            }*/
                
        }
        private void StreamReadDirectory(string path)
        {
            var watcher = new FileSystemWatcher();
            watcher.Path = @"C:\MyFolder"; // Здесь указывается путь к папке, которую нужно отслеживать
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.*"; // Здесь указывается фильтр файлов, которые нужно отслеживать (в данном случае все файлы)

            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnChanged;
            

            watcher.EnableRaisingEvents = true; // Запуск отслеживания

            
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // Здесь можно написать код для реагирования на изменения в папке
            System.Windows.Application.Current.Dispatcher.Invoke(() => 
            {
                ClearModsList();
                foreach (string str in Directory.GetFiles(new IniFile("../../../Configs/Settings.ini").Read("PathToTheModsFolder")))
                {
                    if (new FileInfo(str).Extension == ".jar")
                        StackPanelModList.Children.Add(new System.Windows.Controls.Label
                        {
                            FontSize = 15,
                            Margin = new Thickness(30, 0, 30, 0),
                            Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                            Content = new FileInfo(str).Name
                        });
                }
            });
        }

        private void CreateMainSettingsINI()
        {
            IniFile ini = new IniFile("../../../Configs/Settings.ini");
            ini.Write("PathToTheModsFolder", "");
            ini.Write("PathToTheAssemblersFolder", "");
            ini.Write("NumberOfAssembler", "1");
            ini.Write("SelectedAssembler", "");
        }
        #endregion

    }
}
