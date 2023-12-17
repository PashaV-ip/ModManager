using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        public Grid GridLine { get; set; }
        public TextBox TextBoxGameName { get; set; }
        public TextBox TextBoxGamePath { get; set; }
        public StackPanel StackPanelControlButtonsForGame;
        public Button ButtonBrowseFolderForGame;
        public Button ButtonDeleteGame;
        public Button ButtonCheckSaveGame;



    }
}
