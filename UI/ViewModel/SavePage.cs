using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UI.Model;

namespace UI.ViewModel
{
    class SavePage : DefaultViewModel
    {
        public string? SaveName { get; set; }

        private bool isValid(string? filename)
        {
            return
                !string.IsNullOrEmpty(filename)
                && filename.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
        }

        private Command? _saveGame;
        public Command SaveGame
        {
            get
            {
                _saveGame ??= new Command(
                    p => isValid(SaveName),
                    p =>
                    {
                        MODEL.GM.SaveGame(SaveName);
                        Mediator.SendPropertyChanged<string>("SavePanel", SaveName);
                        Mediator.SendPropertyChanged<string>("CurrentGamePage", "PlayerPage");

                        var info = $"Игра сохранена ./GameSaves/{SaveName}.bin";
                        MessageBox.Show(info);
                    });
                return _saveGame;
            }
        }
    }
}
