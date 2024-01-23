using GalaSoft.MvvmLight.Command;
using ModManager.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using Xceed.Wpf.Toolkit;

namespace ModManager.ViewModel
{
    public class MainMenuViewModel : BaseViewModel
    {
        #region Делегаты
        public delegate void DeleteGame(UIElement element, string nameGame);
        #endregion

        #region Поля
        private System.Windows.WindowState _stateWindow = System.Windows.WindowState.Normal; //Состояние окна
        //private ImageBrush _backgroundWindow = new ImageBrush(new BitmapImage(new Uri("../../../Source/Images/Backgrounds/Background.png", UriKind.Relative)));
        private ImageBrush _backgroundWindow = new ImageBrush(GetBackground());


        private FileSystemWatcher _watcher = new FileSystemWatcher(); //Просматривает папку
        private bool _newAssembler = false;

        #region Поля видимости различных элементов
        
        private Visibility _optionsVisible = Visibility.Hidden;
        private Visibility _importVisibleIndicatorOn;
        private Visibility _importVisibleIndicatorOff;
        private Visibility _assemblerModlistInfoVisible = Visibility.Hidden;
        private Visibility _controlsAssemblerVisible = Visibility.Visible;
        private Visibility _infoVisible = Visibility.Visible;
        #endregion


        #region Settings Assembler поля
        private Visibility _settingsAssemblerVisible = Visibility.Hidden;
        private string _assemblerNameChange = "";
        private string _assemblerGameChange = "";
        private ObservableCollection<string> _gameList = GetGameList();
        private bool _saveWorlds = false;
        private bool _saveConfigs = false;
        private Visibility _pathToWorldsInGameVisible = Visibility.Collapsed;
        private Visibility _pathToConfigsInGameVisible = Visibility.Collapsed;
        private string _pathToWorldsInGame = "";
        private string _pathToConfigsInGame = "";
        private string _loadOrUnloadAssemblerText = "Загрузить";

        #endregion

        #region Settings поля
        private double _opacityPanels = GetOpacityPanels();
        //private System.Windows.Media.Brush _colorPanels; //= (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7");
        private System.Windows.Media.SolidColorBrush _colorPanels = new SolidColorBrush(GetColorPanels());//(System.Windows.Media.SolidColorBrush)new BrushConverter().ConvertFrom("#FFC7C7C7");
        private string _pathToTheBackground = "";
        private ColorPicker? _colorPicker;
        #endregion

        private bool _canEditNameAssembler = true; // Поле отвечающее на вопрос - можно ли редактировать название сборки (Переключается карандашиком)
        private string _assemblerName = ""; // Поле - название сборки (Необходимо для отображения шапки)
        private string _assemblerGame = "";
        private string _pathToTheModsFolder = ""; //Хранит путь до папки mods у игры
        private string _pathToTheAssemblersFolder = ""; //Хранит путь до папки, куда будут сохранятся сборки;
        private System.Windows.Controls.Button? _assemblerImageButton; //ImageButton отображает иконку сборки и позволяет её менять;
        private StackPanel? _stackPanelSlideMenu; // Боковое меню
        private StackPanel? _stackPanelModList; //Список модов
        private StackPanel? _stackPanelGameList;




        private string _coutMods = "Кол-во модов: 0";
        private string _fileWeight = "Вес: 0 Мб";
        private ObservableCollection<GameInfo> _gameInfoList = new ObservableCollection<GameInfo>();




        #endregion
        #region Свойства

        public string LoadOrUnloadAssemblerText
        {
            get => _loadOrUnloadAssemblerText;
            set
            {
                _loadOrUnloadAssemblerText = value;
                OnPropertyChanged(nameof(LoadOrUnloadAssemblerText));
            }
        }

        #region Свойства окна настроек сборки

        public Visibility SettingsAssemblerVisible
        {
            get => _settingsAssemblerVisible;
            set
            {
                _settingsAssemblerVisible = value;
                OnPropertyChanged(nameof(SettingsAssemblerVisible));
            }
        }
        public string AssemblerGameChange
        {
            get => _assemblerGameChange;
            set
            {
                _assemblerGameChange = value;
                IniFile iniFile = new IniFile("../../../Configs/Settings.ini");
                try
                {
                    PathToWorldsInGame = new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, "Configs.ini")).Read("PathToGameWorlds", "Configs");
                    PathToConfigsInGame = new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, "Configs.ini")).Read("PathToGameConfigs", "Configs");
                }
                catch
                {
                    PathToWorldsInGame = "";
                    PathToConfigsInGame = "";
                }
                
                OnPropertyChanged(nameof(AssemblerGameChange));
            }
        }
        public string AssemblerNameChange
        {
            get => _assemblerNameChange;
            set
            {
                _assemblerNameChange = value;
                OnPropertyChanged(nameof(AssemblerNameChange));
            }
        }
        public ObservableCollection<string> GameList
        {
            get => _gameList;
            set
            {
                _gameList = value;
                OnPropertyChanged(nameof(GameList));
            }
        }

        public bool SaveWorlds
        {
            get => _saveWorlds;
            set
            {
                if (value) 
                {
                    PathToWorldsInGameVisible = Visibility.Visible;
                }
                else PathToWorldsInGameVisible = Visibility.Collapsed;
                _saveWorlds = value;
                OnPropertyChanged(nameof(SaveWorlds));
            }
        }
        public bool SaveConfigs
        {
            get => _saveConfigs;
            set
            {
                if (value)
                {
                    PathToConfigsInGameVisible = Visibility.Visible;
                }
                else PathToConfigsInGameVisible = Visibility.Collapsed;
                _saveConfigs = value;
                OnPropertyChanged(nameof(SaveConfigs));
            }
        }
        public Visibility PathToWorldsInGameVisible
        {
            get => _pathToWorldsInGameVisible;
            set
            {
                _pathToWorldsInGameVisible = value;
                OnPropertyChanged(nameof(PathToWorldsInGameVisible));
            }
        }
        public Visibility PathToConfigsInGameVisible
        {
            get => _pathToConfigsInGameVisible;
            set
            {
                _pathToConfigsInGameVisible = value;
                OnPropertyChanged(nameof(PathToConfigsInGameVisible));
            }
        }

        public string PathToWorldsInGame
        {
            get => _pathToWorldsInGame;
            set
            {
                _pathToWorldsInGame = value;
                OnPropertyChanged(nameof(PathToWorldsInGame));
            }
        }
        public string PathToConfigsInGame
        {
            get => _pathToConfigsInGame;
            set
            {
                _pathToConfigsInGame = value;
                OnPropertyChanged(nameof(PathToConfigsInGame));
            }
        }


        #endregion

        public ImageBrush BackgroundWindow
        {
            get => _backgroundWindow;
            set
            {
                _backgroundWindow = value;
                OnPropertyChanged(nameof(BackgroundWindow));
            }
        }
        public ObservableCollection<GameInfo> GameInfoList
        {
            get => _gameInfoList;
            set
            {
                _gameInfoList = value;
            }
        }

        #region Settings свойства
        public ColorPicker ColorPicker
        {
            get => _colorPicker;
            set
            {
                _colorPicker = value;
                OnPropertyChanged(nameof(ColorPicker));
            }
        }
        public string PathToTheBackground
        {
            get => _pathToTheBackground;
            set
            {
                _pathToTheBackground = value;
                OnPropertyChanged(nameof(PathToTheBackground));
            }
        }
        
        public double OpacityPanels
        {
            get => _opacityPanels;
            set
            {
                _opacityPanels = double.Parse(value.ToString("F2"));
                OnPropertyChanged(nameof(OpacityPanels));
            }
        }
        public System.Windows.Media.SolidColorBrush ColorPanels
        {
            get => _colorPanels;
            set
            {
                _colorPanels = value;
                OnPropertyChanged(nameof(ColorPanels));
            }
        }
        #endregion

        public StackPanel StackPanelGameList
        {
            get => _stackPanelGameList;
            set
            {
                _stackPanelGameList = value;
            }
        }
        public StackPanel StackPanelModList //Свойство взаимодействующее с полем _stackPanelModList
        {
            get => _stackPanelModList;
            set
            {
                _stackPanelModList = value;
                OnPropertyChanged(nameof(StackPanelModList));
            }
        }
        public string FileWeight
        {
            get => _fileWeight;
            set
            {
                _fileWeight = "Вес: " + value + "Мб";
                OnPropertyChanged(nameof(FileWeight));
            }
        }
        public string CoutMods
        {
            get => _coutMods;
            set
            {
                _coutMods = "Кол-во модов: " + value;
                OnPropertyChanged(nameof(CoutMods));
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
                _assemblerImageButton.IsEnabled = true;
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
        public Visibility InfoVisible
        {
            get => _infoVisible;
            set
            {
                _infoVisible = value;
                OnPropertyChanged(nameof(InfoVisible));
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
                AssemblerNameChange = value;
                OnPropertyChanged(nameof(AssemblerName));
            }
        }

        public string AssemblerGame //Свойство для взаимодействия с полем _assemblerName
        {
            get => _assemblerGame;
            set
            {
                _assemblerGame = value;
                OnPropertyChanged(nameof(AssemblerGame));
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
        public System.Windows.WindowState StateWindow //Свойство для взаимодействия с полем _stateWindow
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

        public ICommand OpenInfo
        {
            get
            {
                return new RelayCommand(() => {
                    AssemblerModlistInfoVisible = Visibility.Hidden;
                    InfoVisible = Visibility.Visible;
                    CancleAssemblerOption.Execute(null);
                    CancleOptions.Execute(null);
                });
            }
        }

        #region Команды окна настроек сборки
        public ICommand CancleAssemblerOption
        {
            get
            {
                return new RelayCommand(() => {
                    _newAssembler = false;
                    SettingsAssemblerVisible = Visibility.Hidden;
                });
            }
        }

        public ICommand SaveAssemblerOption
        {
            get
            {
                return new RelayCommand(() =>
                {
                    /*ControlAssemblerVisibility = Visibility.Visible;
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
                    }*/

                    SaveAssemblerSettings();

                    //SettingsAssemblerVisible = Visibility.Hidden;

                });
            }
        }

        public ICommand BrowseAssemblerPathToWorldsOrConfigs
        {
            get
            {
                return new RelayCommand<object>((parameter) =>
                {
                    var dialog = new FolderBrowserDialog();
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        switch ((string)parameter)
                        {
                            case "BrowseWorlds":
                                PathToWorldsInGame = dialog.SelectedPath;
                                break;
                            case "BrowseConfigs":
                                PathToConfigsInGame = dialog.SelectedPath;
                                break;
                        }
                    }
                });
            }
        }


        #endregion


        #region Команды настроек

        public ICommand CancleOptions
        {
            get
            {
                return new RelayCommand(() => {
                    OptionsVisible = Visibility.Hidden;
                    BackgroundWindow = new ImageBrush(GetBackground());
                    OpacityPanels = GetOpacityPanels();
                    ColorPanels = new SolidColorBrush(GetColorPanels());
                    GameList = GetGameList();
                    ClearGameList();
                    GetAssemblers();
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
                        GameList = GetGameList();
                        IniFile ini = new IniFile("../../../Configs/Settings.ini");
                        //ini.Write("PathToTheModsFolder", PathToTheModsFolder);
                        ini.Write("ColorPanels", ColorPanels.ToString());
                        ini.Write("OpacityPanels", OpacityPanels.ToString());
                        //System.Windows.MessageBox.Show(ColorPanels.ToString());
                        _watcher.Path = PathToTheModsFolder;
                        if (ini.Read("PathToTheAssemblersFolder") != PathToTheAssemblersFolder)
                        {
                            ini.Write("PathToTheAssemblersFolder", PathToTheAssemblersFolder);
                            GetAssemblers();
                        }
                        if(PathToTheBackground != "" && new FileInfo(PathToTheBackground).Exists)
                        {
                            
                            new FileInfo("../../../Source/Images/Backgrounds/Background.png").Delete();
                            File.Copy(PathToTheBackground, "../../../Source/Images/Backgrounds/Background.png");
                            //File.Replace("../../../Source/Images/Backgrounds/Background.png", PathToTheBackground, null);
                            BackgroundWindow = null;
                            BackgroundWindow = new ImageBrush(GetBackground()); //Я исправил ошибку!!!!!!!!!!! "Пишет файл используется - ОПЯТЬ!!!!!!!!!" УРААААААА!!!!!!!!!
                        }
                        OptionsVisible = Visibility.Hidden;
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

        public ICommand BrowseBackgroundImage
        {
            get
            {
                return new RelayCommand(() => {

                    var dialog = new OpenFileDialog();
                    dialog.Filter = "PNG (*.png)|*.png"; //"Image files (*.png;*.jpg)|*.png;*.jpg";
                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK && new FileInfo(dialog.FileName).Exists)
                    {
                        //BitmapImage image = new BitmapImage(new Uri("../../../Source/Images/Backgrounds/BackgroundV1.jpg", UriKind.Relative));
                        PathToTheBackground = dialog.FileName;
                        BitmapImage image = new BitmapImage(new Uri(PathToTheBackground, UriKind.Relative));
                        BackgroundWindow = new ImageBrush(image);
                        //System.Windows.MessageBox.Show(dialog.FileName);
                    }
                    //BitmapImage image = new BitmapImage(new Uri("../../../Source/Images/Backgrounds/BackgroundV1.jpg", UriKind.Relative)); // замените "image_path.jpg" на путь к вашему изображению
                                                                                                                                       // Создание нового объекта ImageBrush с использованием BitmapImage
 
                    //BackgroundWindow = new ImageBrush(image);
                });
            }
        }

        #endregion
        public ICommand CloseApplication 
        {
            get
            {
                return new RelayCommand(() => { foreach (System.Windows.Window w in App.Current.Windows) w.Close(); });
            }
        }
        public ICommand MinimizeWindow
        {
            get
            {
                return new RelayCommand(() => { StateWindow = System.Windows.WindowState.Minimized; });
            }
        }
        public ICommand BrowseModsFolder
        {
            get
            {
                return new RelayCommand(() => {
                    /*var dialog = new FolderBrowserDialog();
                    dialog.InitialDirectory= PathToTheModsFolder;
                    dialog.SelectedPath = PathToTheModsFolder;

                    DialogResult result = dialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        PathToTheModsFolder = dialog.SelectedPath;
                    }*/
                });
            }
        }
        
        public ICommand OpenAssembler
        {
            get
            {
                return new RelayCommand<object>((parameter) => {
                    InfoVisible = Visibility.Hidden;
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
                        OpacityPanels = double.Parse(ini.Read("OpacityPanels").Replace('.', ',')); 
                        RedrawingGameListMethod();
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
        public ICommand AddGame
        {
            get
            {
                return new RelayCommand(() =>{
                    AddGameToList();
                });
            }
        }

        public ICommand OnOffMods
        {
            get
            {
                return new RelayCommand<object>((parameter) =>
                {
                    IniFile iniFile = new IniFile("../../../Configs/Settings.ini");
                    System.Windows.Controls.CheckBox checkBox = StackPanelModList.Children.OfType<System.Windows.Controls.Grid>().FirstOrDefault(x => x.Name == "grid_" + parameter.ToString().Split('#')[2]).Children.OfType<System.Windows.Controls.CheckBox>().FirstOrDefault(checkBox => checkBox.Name.ToString() == "checkBox_" + parameter.ToString().Split('#')[2]);
                    new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), Path.Combine(parameter.ToString().Split('#')[0], Path.Combine("Assets", "Configs.ini")))).Write(parameter.ToString().Split('#')[1], checkBox.IsChecked.ToString(), "Mods");
                });
            }
        }
        

        public ICommand CreateAssembler //Нужно добавить бордер, для создания сборки, где нужно выбрать игру и название, а после, чтение модов
        {
            get
            {
                return new RelayCommand(() => {

                    IniFile iniFile = new IniFile("../../../Configs/Settings.ini");
                    AssemblerGameChange = "";
                    AssemblerNameChange = "";
                    SaveWorlds = false;
                    SaveConfigs = false; ;
                    SettingsAssemblerVisible = Visibility.Visible;
                    _newAssembler = true;
                    //PathToWorldsInGame = new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, "Configs.ini")).Read("PathToGameWorlds", "Configs");
                    //PathToConfigsInGame = new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, "Configs.ini")).Read("PathToGameConfigs", "Configs");

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
                    //dialog.InitialDirectory = PathToTheAssemblersFolder;
                    dialog.Filter = "PNG (*.png)|*.png";
                    DialogResult result = dialog.ShowDialog();

                    

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        IniFile ini = new IniFile("../../../Configs/Settings.ini");
                        AssemblerImageButton.Resources.Remove("Img");
                        System.Windows.Controls.Button button = StackPanelSlideMenu.Children.OfType<System.Windows.Controls.StackPanel>().FirstOrDefault(x => x.Name == "stackPanel_" + _assemblerGame.Replace(' ', '_')).Children.OfType<System.Windows.Controls.Button>().FirstOrDefault(button => button.Content.ToString() == AssemblerName);
                        button.Resources.Remove("Img");
                        string path = Path.Combine(ini.Read("PathToTheAssemblersFolder"), Path.Combine(Path.Combine(_assemblerGame, AssemblerName), Path.Combine("Assets", "img.png"))); ;

                        //File.Copy(dialog.FileName, Path.Combine(ini.Read("PathToTheAssemblersFolder"),));
                        if (File.Exists(Path.Combine(ini.Read("PathToTheAssemblersFolder"), Path.Combine(Path.Combine(_assemblerGame, AssemblerName), Path.Combine("Assets", "img.png")))))
                        {
                            
                            //_assemblerImageButton.Resources.Remove("Img");
                            //System.Windows.Controls.Button button = StackPanelSlideMenu.Children.OfType<System.Windows.Controls.Button>().FirstOrDefault(button => button.Content.ToString() == AssemblerName);
                           
                            


                            //string path = Path.Combine(ini.Read("PathToTheAssemblersFolder"), Path.Combine(AssemblerName, Path.Combine("Assets", "img.png")));

       

                            File.Delete(path);
                            File.Copy(dialog.FileName, path);


                            //-----

                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            image.UriSource = new Uri(path, UriKind.Absolute);
                            image.EndInit();

                            //------


                            AssemblerImageButton.Resources.Add("Img", image);
                            button.Resources.Add("Img", image);
                            //Закончил на том, что теперь ошибка не возникает и программа позволяет менять картинку сборки!!!!!!!!!!!!!!!!!!!!!!!!
                            //!!!!!!!!!!!!!!!!!!!!!!!!!
                        }
                        else
                        {
                            //AssemblerImageButton.Resources.Remove("Img");
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                            File.Copy(dialog.FileName, path);
                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            image.UriSource = new Uri(path, UriKind.Absolute);
                            image.EndInit();
                            AssemblerImageButton.Resources.Add("Img", image);
                            button.Resources.Add("Img", image);
                            //GetAssemblers();
                            //System.Windows.MessageBox.Show("Картинки нету, ставлю новую");
                        }
                    }
                });
            }
        }

        #region Кнопки одной сборки
        public ICommand LoadOrUnloadAssembler
        {
            get
            {
                return new RelayCommand(() => {
                    IniFile iniFile = new IniFile("../../../Configs/Settings.ini");
                    IniFile ini = new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName, "Assets", "Configs.ini"));
                    if (!bool.TryParse(ini.Read("SaveWorlds", "Configs"), out bool worldsSave)) worldsSave = false;
                    if (!bool.TryParse(ini.Read("SaveConfigs", "Configs"), out bool configsSave)) configsSave = false;
                    if (LoadOrUnloadAssemblerText == "Загрузить")
                    {
                        LoadOrUnloadAssemblerText = "Выгрузить";



                        foreach (string str in Directory.GetFiles(iniFile.Read(AssemblerGame, "GameList")))
                        {
                            File.Delete(str);
                        }
                        foreach (string file in Directory.GetFiles(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName, "Mods")))
                        {
                            if(bool.Parse(ini.Read(new FileInfo(file).Name, "Mods")))
                            File.Copy(file, Path.Combine(iniFile.Read(AssemblerGame, "GameList"), new FileInfo(file).Name));
                        }

                        if (worldsSave)
                        {
                            foreach (string str in Directory.GetDirectories(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, "Configs.ini")).Read("PathToGameWorlds", "Configs")))
                            {
                                Directory.Delete(str, true);
                            }
                            foreach (string str in Directory.GetDirectories(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName, "GameWorlds")))
                            {
                                CopyDirectory(str, Path.Combine(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, "Configs.ini")).Read("PathToGameWorlds", "Configs"), new DirectoryInfo(str).Name));
                            }
                        }
                        if (configsSave)
                        {
                            foreach (string str in Directory.GetFiles(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, "Configs.ini")).Read("PathToGameConfigs", "Configs")))
                            {
                                File.Delete(str);
                            }
                            foreach (string str in Directory.GetFiles(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName, "GameConfigs")))
                            {
                                File.Copy(str, Path.Combine(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, "Configs.ini")).Read("PathToGameConfigs", "Configs"), new FileInfo(str).Name));
                            }
                        }
                        new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, "Configs.ini")).Write("LoadAssembler", AssemblerName, "Configs");
                    }
                    else
                    {
                        foreach (string str in Directory.GetFiles(iniFile.Read(AssemblerGame, "GameList")))
                        {
                            File.Delete(str);
                        }
                        if (worldsSave)
                        {
                            foreach (string str in Directory.GetDirectories(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName, "GameWorlds")))
                            {
                                Directory.Delete(str, true);
                            }

                            foreach (string str in Directory.GetDirectories(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, "Configs.ini")).Read("PathToGameWorlds", "Configs")))
                            {
                                CopyDirectory(str, Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName, "GameWorlds", new DirectoryInfo(str).Name));
                                //System.Windows.MessageBox.Show(str);
                            }
                            foreach (string str in Directory.GetDirectories(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, "Configs.ini")).Read("PathToGameWorlds", "Configs")))
                            {
                                Directory.Delete(str, true);
                            }
                        }
                        if (configsSave)
                        {
                            foreach (string str in Directory.GetFiles(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName, "GameConfigs")))
                            {
                                File.Delete(str);
                            }

                            foreach (string str in Directory.GetFiles(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, "Configs.ini")).Read("PathToGameConfigs", "Configs")))
                            {
                                File.Copy(str, Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName, "GameConfigs", new FileInfo(str).Name));
                                //System.Windows.MessageBox.Show(str);
                            }
                            foreach (string str in Directory.GetFiles(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, "Configs.ini")).Read("PathToGameConfigs", "Configs")))
                            {
                                File.Delete(str);
                            }
                        }


                        new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, "Configs.ini")).Write("LoadAssembler", "", "Configs");
                        LoadOrUnloadAssemblerText = "Загрузить";
                    }
                    
                });
            }
        }
        public ICommand DeleteAssembler
        {
            get
            {
                return new RelayCommand(() => {
                    DialogResult result = (DialogResult)System.Windows.MessageBox.Show($"Вы действительно хотите удалить данную сборку ({AssemblerName})?", "Удалить сборку?", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if(result == DialogResult.Yes)
                    {
                        IniFile iniFile = new IniFile("../../../Configs/Settings.ini");

                        
                        Directory.Delete(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName), true);
                        GetAssemblers();
                        OpenInfo.Execute(null);
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
                    IniFile iniFile = new IniFile("../../../Configs/Settings.ini");
                    _newAssembler = false;
                    AssemblerGameChange = AssemblerGame;
                    AssemblerNameChange = AssemblerName;
                    bool result = false;
                    if(bool.TryParse(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName, "Assets", "Configs.ini")).Read("SaveWorlds", "Configs"),out result))
                        SaveWorlds = result;
                    if (bool.TryParse(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName, "Assets", "Configs.ini")).Read("SaveConfigs", "Configs"), out result))
                        SaveConfigs = result;
                    SettingsAssemblerVisible = Visibility.Visible;
                });
            }
        }

        #endregion
        #endregion

        #region Методы
        public void ClearAssemblersList() => StackPanelSlideMenu.Children.Clear();

        public void ClearModsList() => StackPanelModList.Children.Clear();
        private void ClearGameList()
        {
            StackPanelGameList.Children.Clear();
            GameInfoList.Clear();
        }

        private void SaveAssemblerSettings()
        {
            IniFile iniFile = new IniFile("../../../Configs/Settings.ini");
            if (AssemblerNameChange == "")
            {
                System.Windows.MessageBox.Show("Поле \"Название сборки\" не может быть пустым!", "Ошибка..", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (AssemblerGameChange == "")
            {
                System.Windows.MessageBox.Show("Вы не выбрали игру!", "Ошибка..", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (_newAssembler && new DirectoryInfo(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, AssemblerNameChange)).Exists)
            {
                System.Windows.MessageBox.Show("Сборка модификаций с таким именем уже существует!", "Ошибка..", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            new DirectoryInfo(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange)).Create();
            if (_saveWorlds)
            {
                if (PathToWorldsInGame != "" && new DirectoryInfo(PathToWorldsInGame).Exists)
                {
                    
                    new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, "Configs.ini")).Write("PathToGameWorlds", PathToWorldsInGame, "Configs");
                }
                else
                {
                    SaveWorlds = false;
                    PathToWorldsInGame = "";
                    System.Windows.MessageBox.Show("Папка с игровыми мирами не найдена...\nСохранение игровых миров отключено!", "Ошибка..", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (_saveConfigs)
            {
                if (PathToConfigsInGame != "" && new DirectoryInfo(PathToConfigsInGame).Exists)
                {
                    new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, "Configs.ini")).Write("PathToGameConfigs", PathToConfigsInGame, "Configs");
                }
                else
                {
                    SaveConfigs = false;
                    PathToConfigsInGame = "";
                    System.Windows.MessageBox.Show("Папка с файлами конфигурации не найдена...\nСохранение файлов конфигурации отключено!", "Ошибка..", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (_newAssembler)
            {

                new DirectoryInfo(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, AssemblerNameChange, "Assets")).Create();
                new DirectoryInfo(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, AssemblerNameChange, "Mods")).Create();
                new DirectoryInfo(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, AssemblerNameChange, "GameWorlds")).Create();
                new DirectoryInfo(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, AssemblerNameChange, "GameConfigs")).Create();
                IniFile ini = new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, AssemblerNameChange, "Assets", "Configs.ini"));
                ini.Write("SaveWorlds", _saveWorlds.ToString(), "Configs");
                ini.Write("SaveConfigs", _saveConfigs.ToString(), "Configs");

                
                foreach (string str in Directory.GetFiles(iniFile.Read(AssemblerGameChange, "GameList")))
                {
                    File.Copy(str, Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, AssemblerNameChange, "Mods", new FileInfo(str).Name));
                    ini.Write(new FileInfo(str).Name, true.ToString(), "Mods");
                }
                if(_saveWorlds)
                {
                    /*System.Windows.MessageBox.Show("PathToTheAssemblersFolder = " + PathToTheAssemblersFolder + "\n" +
                        "InI_PathToTheAssemblersFolder" + iniFile.Read("PathToTheAssemblersFolder") + "\n" +
                        "Combine_PathToTheAssemblersFolder" + Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, "Configs.ini") + "\n");*/
                        
                        //new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, "Configs.ini")).Read("PathToGameWorlds", "Configs"));
                    foreach (string str in Directory.GetDirectories(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, "Configs.ini")).Read("PathToGameWorlds", "Configs")))
                    {
                        CopyDirectory(str, Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, AssemblerNameChange, "GameWorlds", new DirectoryInfo(str).Name));
                        //System.Windows.MessageBox.Show(str);
                    }
                }
                if (_saveConfigs)
                {
                    foreach (string str in Directory.GetFiles(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, "Configs.ini")).Read("PathToGameConfigs", "Configs")))
                    {
                        File.Copy(str, Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, AssemblerNameChange, "GameConfigs", new FileInfo(str).Name));
                    }
                }
                


                _newAssembler = false;
            }
            else
            {
                if (AssemblerName != AssemblerNameChange)
                {
                    if(Directory.Exists(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName)) && !Directory.Exists(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerNameChange)))
                    {
                        Directory.Move(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName), Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerNameChange)); // Переименовываем папку
                        AssemblerName = AssemblerNameChange;
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Сборка с таким именем уже существует!", "Ошибка..", MessageBoxButton.OK, MessageBoxImage.Error);
                        
                        AssemblerNameChange = AssemblerName;
                    }
                }

                IniFile ini = new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName, "Assets", "Configs.ini"));
                ini.Write("SaveWorlds", _saveWorlds.ToString(), "Configs");
                ini.Write("SaveConfigs", _saveConfigs.ToString(), "Configs");

                if (AssemblerGame != AssemblerGameChange)
                {
                    if (Directory.Exists(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName)) && !Directory.Exists(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, AssemblerName)))
                    {
                        Directory.Move(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, AssemblerName), Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGameChange, AssemblerName)); // Переименовываем папку
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Данная сборка уже существует в этой игре!", "Ошибка..", MessageBoxButton.OK, MessageBoxImage.Error);

                        AssemblerGameChange = AssemblerGame;
                    }
                }


            }
            GetAssemblers();
            System.Windows.Controls.Button button = StackPanelSlideMenu.Children.OfType<StackPanel>().FirstOrDefault(x => x.Name == "stackPanel_" + AssemblerGameChange.Replace(' ', '_')).Children.OfType<System.Windows.Controls.Button>().FirstOrDefault(button => button.Content.ToString() == _assemblerNameChange);
            button.Focus();
            OpenAssembler.Execute(Path.Combine(AssemblerGameChange, AssemblerNameChange));
            _newAssembler = false;
            SettingsAssemblerVisible = Visibility.Hidden;
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
            DirectoryInfo directoryWithInfo = new DirectoryInfo(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder"));
            foreach (var str in directoryWithInfo.GetDirectories().OrderBy(d => d.CreationTime))
            {
                if(new IniFile("../../../Configs/Settings.ini").KeyExists(str.Name, "GameList"))
                {
                    System.Windows.Controls.StackPanel stackPanel = new System.Windows.Controls.StackPanel
                    {
                        Name = "stackPanel_" + str.Name.Replace(' ', '_')
                    };
                    System.Windows.Controls.TextBlock textBlock = new System.Windows.Controls.TextBlock
                    {
                        Text = str.Name,
                        FontSize = 20,
                        Margin = new Thickness(30, 0, 30, 0),
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7")
                        
                    };
                    stackPanel.Children.Add(textBlock);
                    foreach (var gameDirectWithAssemblers in str.GetDirectories().OrderBy(d => d.CreationTime))
                    {
                        System.Windows.Controls.Button button = new System.Windows.Controls.Button
                        {
                            Template = (ControlTemplate)System.Windows.Application.Current.Resources["SlideMenuButton"],
                            Name = gameDirectWithAssemblers.Name,
                            Content = gameDirectWithAssemblers.Name,
                            Command = OpenAssembler,
                            CommandParameter = Path.Combine(str.Name, gameDirectWithAssemblers.Name),
                        };
                        if (File.Exists(Path.Combine(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder"), Path.Combine(Path.Combine(str.Name, gameDirectWithAssemblers.Name), Path.Combine("Assets", "img.png")))))
                        {

                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            image.UriSource = new Uri(Path.Combine(new IniFile("../../../Configs/Settings.ini").Read("PathToTheAssemblersFolder"), Path.Combine(Path.Combine(str.Name, gameDirectWithAssemblers.Name), Path.Combine("Assets", "img.png"))), UriKind.Absolute);
                            image.EndInit();

                            button.Resources.Remove("Img");
                            button.Resources.Add("Img", image);
                        }
                        else
                        {
                            button.Resources.Remove("Img");
                            button.Resources.Add("Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/img.png", UriKind.Absolute)));
                        }
                        stackPanel.Children.Add(button);
                    }
                    StackPanelSlideMenu.Children.Add(stackPanel);
                }
                
            }

        }
        public void GetInformationInSelectedAssembler(string assembler)
        {
            IniFile iniFile = new IniFile("../../../Configs/Settings.ini");
            
            if (File.Exists(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), Path.Combine(assembler, Path.Combine("Assets", "img.png")))))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                image.UriSource = new Uri(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), Path.Combine(assembler, Path.Combine("Assets", "img.png"))), UriKind.Absolute);
                image.EndInit();

                AssemblerImageButton.Resources.Remove("Img");
                AssemblerImageButton.Resources.Add("Img", image);
            }
            else 
            {
                AssemblerImageButton.Resources.Remove("Img");
                AssemblerImageButton.Resources.Add("Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/img.png", UriKind.Absolute))); 
            }

            AssemblerName = assembler.Split('\\').Last();
            _assemblerGame = assembler.Split('\\').First();
            if (new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), AssemblerGame, "Configs.ini")).Read("LoadAssembler", "Configs") == AssemblerName)
            {
                LoadOrUnloadAssemblerText = "Выгрузить";
            }
            else
            {
                LoadOrUnloadAssemblerText = "Загрузить";
            }
            if (StackPanelModList.Children.Count > 0)
            {
                StackPanelModList.Children.Clear();
            }
            double fileWeight = 0;
            int coutMods = 0;
            foreach (string str in Directory.GetFiles(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), assembler, "Mods")))
            { 
                coutMods++;  
                bool isChecked;
                if (!bool.TryParse(new IniFile(Path.Combine(iniFile.Read("PathToTheAssemblersFolder"), Path.Combine(assembler, Path.Combine("Assets", "Configs.ini")))).Read(new FileInfo(str).Name, "Mods"), out isChecked))
                    isChecked = false;
                fileWeight += Math.Ceiling((double)new FileInfo(str).Length);
                Grid gridLine = new Grid
                {
                    Name = "grid_" + coutMods
                };
                gridLine.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(50)
                });
                gridLine.ColumnDefinitions.Add(new ColumnDefinition());
                gridLine.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(100)
                });
                System.Windows.Controls.Label labelNumberMod = new System.Windows.Controls.Label
                {
                    FontSize = 15,
                    Margin = new Thickness(0, 0, 0, 0),
                    Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                    Content = coutMods.ToString()
                };
                System.Windows.Controls.Label labelNameMod = new System.Windows.Controls.Label
                {
                    FontSize = 15,
                    Margin = new Thickness(0, 0, 30, 0),
                    Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                    Content = new FileInfo(str).Name
                };
                System.Windows.Controls.Border border = new System.Windows.Controls.Border
                {
                    BorderBrush = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(3)
                };
                System.Windows.Controls.CheckBox checkbox = new System.Windows.Controls.CheckBox
                {
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    IsChecked = isChecked,
                    Style = (Style)System.Windows.Application.Current.Resources["CheckBoxStyle"],
                    VerticalAlignment = VerticalAlignment.Center,
                    Name = "checkBox_" + coutMods,
                    Command = OnOffMods
                };
                checkbox.CommandParameter = assembler + "#" + new FileInfo(str).Name + "#" + coutMods;
                Grid.SetColumn(labelNumberMod, 0);
                Grid.SetColumn(labelNameMod, 1);
                Grid.SetColumnSpan(border, 3);
                Grid.SetColumn(checkbox, 2);
                gridLine.Children.Add(border);
                gridLine.Children.Add(labelNumberMod);
                gridLine.Children.Add(labelNameMod);
                gridLine.Children.Add(checkbox);


                StackPanelModList.Children.Add(gridLine);
            }
            FileWeight = Math.Ceiling((fileWeight/(1024*1024))).ToString();
            CoutMods = StackPanelModList.Children.Count.ToString();
        }

        private static void CreateMainSettingsINI()
        {
            IniFile ini = new IniFile("../../../Configs/Settings.ini");
            ini.Write("PathToTheAssemblersFolder", "");
            ini.Write("ColorPanels", "#FF000000");
            ini.Write("OpacityPanels", "0,5");


        }
        private void AddGameToList(string Name, string Path)
        {
            GameInfoList.Add(new GameInfo(Name, Path, DeleteGameFromList));
            StackPanelGameList.Children.Add(GameInfoList[GameInfoList.ToArray().Length - 1].GridLine);
        }
        private void AddGameToList()
        {
            GameInfoList.Add(new GameInfo(DeleteGameFromList));
            StackPanelGameList.Children.Add(GameInfoList[GameInfoList.ToArray().Length - 1].GridLine);
        }
        private void DeleteGameFromList(UIElement element, string nameGame)
        {
            GameInfoList.RemoveAt(StackPanelGameList.Children.IndexOf(element));
            StackPanelGameList.Children.Remove(element);
        }

        private void RedrawingGameListMethod()
        {
            StackPanelGameList.Children.Clear();
            IniFile ini = new IniFile("../../../Configs/Settings.ini");
            foreach (var key in ini.GetKeys("GameList")) //Закончил на получении списка ключей из секции в ini файле (Необходимо для загрузки списка игр)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    GameList.Add(key);
                    string path = ini.Read(key, "GameList");
                    AddGameToList(key, path);
                }
            }
        }

        private static ObservableCollection<string> GetGameList()
        {

            ObservableCollection<string> gameList = new ObservableCollection<string>();
            IniFile ini = new IniFile("../../../Configs/Settings.ini");
            foreach (var key in ini.GetKeys("GameList")) //Закончил на получении списка ключей из секции в ini файле (Необходимо для загрузки списка игр)
            {
                if (!string.IsNullOrEmpty(key))
                {
                    gameList.Add(key);
                }
            }
            return gameList;
        }



        private static BitmapImage GetBackground()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            //image.UriSource = new Uri("../../../Source/Images/Backgrounds/Background.png", UriKind.Relative);
            image.UriSource = new Uri("../../../Source/Images/Backgrounds/Background.png", UriKind.Relative);
            image.EndInit();

            return image;
            
        }

        private static double GetOpacityPanels()
        {
            if (!File.Exists("../../../Configs/Settings.ini"))
            {
                IniFile ini = new IniFile("../../../Configs/Settings.ini");
                CreateMainSettingsINI();
            }
            return double.Parse(new IniFile("../../../Configs/Settings.ini").Read("OpacityPanels").Replace('.', ','));
        }

        private static Color GetColorPanels()
        {
            return (Color)ColorConverter.ConvertFromString(new IniFile("../../../Configs/Settings.ini").Read("ColorPanels"));
        }

        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            // Создаем новую директорию
            Directory.CreateDirectory(destDirName);

            // Получаем список файлов и поддиректорий в исходной директории
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Копируем файлы
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // Копируем поддиректории
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDirName, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
        }

        #endregion
    }
}
