using System;
using System.IO;
using System.Windows.Input;

namespace FileMirroringTool.ViewModels.Commands
{
    internal class OpenExplorer : ICommand
    {
        readonly MainWindowViewModel _mwvm;
        public OpenExplorer(MainWindowViewModel mwvm)
        {
            _mwvm = mwvm;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public bool CanExecute(object parameter)
        {
            var path = parameter as string;
            return !string.IsNullOrEmpty(path) && new FileInfo(path).Exists;
        }

        public void Execute(object parameter)
        {
            var path = parameter.ToString();
            var file = new FileInfo(path);
            System.Diagnostics.Process.Start(file.DirectoryName);
        }
    }
}
