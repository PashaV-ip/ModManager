using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;

namespace ModManager.Model
{
    public class GameInfo
    {
        private Grid _gridLine;
        private TextBox _textBoxGameName;
        private TextBox _textBoxGamePath;
        private StackPanel _stackPanelControlButtonsForGame;
        private Button _buttonBrowseFolderForGame;
        private Button _buttonDeleteGame;
        private Button _buttonCheckSaveGame;

        public Grid GridLine
        {
            get => _gridLine;
            set
            {
                _gridLine = value;
            }
        }
        public TextBox TextBoxGameName
        {
            get => _textBoxGameName;
            set
            {
                _textBoxGameName = value;
            }
        }
        public TextBox TextBoxGamePath
        {
            get => _textBoxGamePath; 
            set
            {
                _textBoxGamePath = value;
            }
        }
        public StackPanel StackPanelControlButtonsForGame
        {
            get => _stackPanelControlButtonsForGame; 
            set
            {
                _stackPanelControlButtonsForGame = value;
            }
        }
        public Button ButtonBrowseFolderForGame
        {
            get => _buttonBrowseFolderForGame; 
            set
            {
                _buttonBrowseFolderForGame = value;
            }
        }
        public Button ButtonDeleteGame
        {
            get => _buttonDeleteGame; 
            set
            {
                _buttonDeleteGame = value;
            }
        }
        public Button ButtonCheckSaveGame
        {
            get => _buttonCheckSaveGame; 
            set
            {
                _buttonCheckSaveGame = value;
            }
        }
        public GameInfo()
        {
            GridLine = new Grid
            {
                Margin = new Thickness(20, 10, 5, 0)
            };
            GridLine.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(200)
            });
            GridLine.ColumnDefinitions.Add(new ColumnDefinition
            {

            });
            GridLine.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(150)
            });
            TextBoxGameName = new System.Windows.Controls.TextBox
            {
                Name = "TextBoxGameName",
                TextAlignment = TextAlignment.Center,
                Text = "",
                Style = (Style)System.Windows.Application.Current.Resources["TextBoxStyle"],
                Margin = new Thickness(10, 0, 10, 0),
                Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                CaretBrush = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                Padding = new Thickness(0, 5, 0, 5),
                MinWidth = 105,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 20,
                IsReadOnly = false
            };
            Grid.SetColumn(TextBoxGameName, 0);
            TextBoxGamePath = new System.Windows.Controls.TextBox
            {
                Name = "TextBoxFolderPath",
                TextAlignment = TextAlignment.Center,
                Text = "",
                Style = (Style)System.Windows.Application.Current.Resources["TextBoxStyle"],
                Margin = new Thickness(10, 0, 10, 0),
                Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                CaretBrush = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                Padding = new Thickness(0, 5, 0, 5),
                MinWidth = 105,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 20,
                IsReadOnly = false
            };
            Grid.SetColumn(TextBoxGamePath, 1);

            StackPanelControlButtonsForGame = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                Margin = new Thickness(0, 3, 0, 3)
            };
            Grid.SetColumn(StackPanelControlButtonsForGame, 2);

            ButtonBrowseFolderForGame = new System.Windows.Controls.Button
            {
                Width = 30,
                Height = 30,
                Focusable = false,
                Margin = new Thickness(5, 0, 0, 0),
                Template = (ControlTemplate)System.Windows.Application.Current.Resources["SmallImageButton"],
                Resources = {
                            { "Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/folder.png", UriKind.Absolute))},
                        }
            };
            ButtonDeleteGame = new System.Windows.Controls.Button
            {
                Width = 30,
                Height = 30,
                Focusable = false,
                Margin = new Thickness(10, 0, 0, 0),
                Template = (ControlTemplate)System.Windows.Application.Current.Resources["SmallImageButton"],
                Resources = {
                            { "Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/delete.png", UriKind.Absolute))},
                        },
                Command = new RelayCommand(() =>
                {

                })
            };
            ButtonCheckSaveGame = new System.Windows.Controls.Button
            {
                Width = 30,
                Height = 30,
                Focusable = false,
                Margin = new Thickness(10, 0, 0, 0),
                Template = (ControlTemplate)System.Windows.Application.Current.Resources["SmallImageButton"],
                Resources = {
                    { "Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/check.png", UriKind.Absolute))},
                },
                Command = new RelayCommand(() =>
                {
                    if (TextBoxGameName.IsReadOnly || TextBoxGamePath.IsReadOnly)
                    {
                        TextBoxGameName.IsReadOnly = false;
                        TextBoxGamePath.IsReadOnly = false;
                        ButtonCheckSaveGame.Resources.Remove("Img");
                        ButtonCheckSaveGame.Resources.Add("Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/check.png", UriKind.Absolute)));
                        System.Windows.MessageBox.Show("Нажми галочку для сохранения");
                    }
                    else
                    {
                        TextBoxGameName.IsReadOnly = true;
                        TextBoxGamePath.IsReadOnly = true;
                        IniFile ini = new IniFile("../../../Configs/Test.ini");
                        ini.Write(TextBoxGameName.Text, TextBoxGamePath.Text, "TestGameSection");
                        System.Windows.MessageBox.Show(TextBoxGameName.Text + " " + TextBoxGamePath.Text, "Добавлено в секцию TestGameSection");
                        ButtonCheckSaveGame.Resources.Remove("Img");
                        ButtonCheckSaveGame.Resources.Add("Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/pencil.png", UriKind.Absolute)));
                        System.Windows.MessageBox.Show("Нажми карандашь что бы изменить пути и игру");
                    }
                })
            };

            StackPanelControlButtonsForGame.Children.Add(ButtonBrowseFolderForGame);
            StackPanelControlButtonsForGame.Children.Add(ButtonDeleteGame);
            StackPanelControlButtonsForGame.Children.Add(ButtonCheckSaveGame);

            GridLine.Children.Add(TextBoxGameName);
            GridLine.Children.Add(TextBoxGamePath);
            GridLine.Children.Add(StackPanelControlButtonsForGame);
        }



        public GameInfo(string GameName, string GamePath)
        {
            GridLine = new Grid
            {
                Margin = new Thickness(20, 10, 5, 0)
            };
            GridLine.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(200)
            });
            GridLine.ColumnDefinitions.Add(new ColumnDefinition
            {

            });
            GridLine.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(150)
            });
            TextBoxGameName = new System.Windows.Controls.TextBox
            {
                Name = "TextBoxGameName",
                TextAlignment = TextAlignment.Center,
                Text = GameName,
                Style = (Style)System.Windows.Application.Current.Resources["TextBoxStyle"],
                Margin = new Thickness(10, 0, 10, 0),
                Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                CaretBrush = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                Padding = new Thickness(0, 5, 0, 5),
                MinWidth = 105,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 20,
                IsReadOnly = true
            };
            Grid.SetColumn(TextBoxGameName, 0);
            TextBoxGamePath = new System.Windows.Controls.TextBox
            {
                Name = "TextBoxFolderPath",
                TextAlignment = TextAlignment.Center,
                Text = GamePath,
                Style = (Style)System.Windows.Application.Current.Resources["TextBoxStyle"],
                Margin = new Thickness(10, 0, 10, 0),
                Foreground = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                CaretBrush = (System.Windows.Media.Brush)new BrushConverter().ConvertFrom("#FFC7C7C7"),
                Padding = new Thickness(0, 5, 0, 5),
                MinWidth = 105,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 20,
                IsReadOnly = true
            };
            Grid.SetColumn(TextBoxGamePath, 1);


            //-------------------------------------------------------------------------------------------------------------
            StackPanelControlButtonsForGame = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                Margin = new Thickness(0, 3, 0, 3)
            };
            Grid.SetColumn(StackPanelControlButtonsForGame, 2);

            ButtonBrowseFolderForGame = new System.Windows.Controls.Button
            {
                Width = 30,
                Height = 30,
                Focusable = false,
                Margin = new Thickness(5, 0, 0, 0),
                Template = (ControlTemplate)System.Windows.Application.Current.Resources["SmallImageButton"],
                Resources = {
                            { "Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/folder.png", UriKind.Absolute))},
                }
            };
            ButtonDeleteGame = new System.Windows.Controls.Button
            {
                Width = 30,
                Height = 30,
                Focusable = false,
                Margin = new Thickness(10, 0, 0, 0),
                Template = (ControlTemplate)System.Windows.Application.Current.Resources["SmallImageButton"],
                Resources = {
                            { "Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/delete.png", UriKind.Absolute))},
                },
                Command = new RelayCommand(() =>
                {
                    GridLine = new Grid();
                })
            };
            ButtonCheckSaveGame = new System.Windows.Controls.Button
            {
                Width = 30,
                Height = 30,
                Focusable = false,
                Margin = new Thickness(10, 0, 0, 0),
                Template = (ControlTemplate)System.Windows.Application.Current.Resources["SmallImageButton"],
                Resources = {
                            { "Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/pencil.png", UriKind.Absolute))},
                        },
                Command = new RelayCommand(() =>
                {
                    if (TextBoxGameName.IsReadOnly || TextBoxGamePath.IsReadOnly)
                    {
                        TextBoxGameName.IsReadOnly = false;
                        TextBoxGamePath.IsReadOnly = false;
                        ButtonCheckSaveGame.Resources.Remove("Img");
                        ButtonCheckSaveGame.Resources.Add("Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/check.png", UriKind.Absolute)));
                    }
                    else
                    {
                        TextBoxGameName.IsReadOnly = true;
                        TextBoxGamePath.IsReadOnly = true;
                        IniFile ini = new IniFile("../../../Configs/Test.ini");
                        ini.Write(TextBoxGameName.Text, TextBoxGamePath.Text, "TestGameSection");
                        System.Windows.MessageBox.Show(TextBoxGameName.Text + " " + TextBoxGamePath.Text, "Добавлено в секцию TestGameSection");
                        ButtonCheckSaveGame.Resources.Remove("Img");
                        ButtonCheckSaveGame.Resources.Add("Img", new BitmapImage(new Uri("pack://application:,,,/Source/Images/pencil.png", UriKind.Absolute)));
                    }
                })
            };

            StackPanelControlButtonsForGame.Children.Add(ButtonBrowseFolderForGame);
            StackPanelControlButtonsForGame.Children.Add(ButtonDeleteGame);
            StackPanelControlButtonsForGame.Children.Add(ButtonCheckSaveGame);
            //-------------------------------------------------------------------------------------------------------------

            GridLine.Children.Add(TextBoxGameName);
            GridLine.Children.Add(TextBoxGamePath);
            GridLine.Children.Add(StackPanelControlButtonsForGame);
        }
    }
}
