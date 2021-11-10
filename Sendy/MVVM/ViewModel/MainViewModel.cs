using System;
using Sendy.Core;

namespace Sendy.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand CreateViewCommand { get; set; }
        public RelayCommand ConvertViewCommand { get; set; }
        public RelayCommand DirectViewCommand { get; set; }
        public RelayCommand DirectTableViewCommand { get; set; }
        public RelayCommand SettingsViewCommand { get; set; }
        public RelayCommand InfoViewCommand { get; set; }

        public CreateViewModel CreateVM { get; set; }
        public ConvertViewModel ConvertVM { get; set; }
        public DirectViewModel DirectVM { get; set; }
        public DirectTableViewModel DirectTableVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }
        public InfoViewModel InfoVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
            CreateVM = new CreateViewModel();
            ConvertVM = new ConvertViewModel();
            DirectVM = new DirectViewModel();
            DirectTableVM = new DirectTableViewModel();
            SettingsVM = new SettingsViewModel();
            InfoVM = new InfoViewModel();
            CurrentView = CreateVM;

            CreateViewCommand = new RelayCommand(o =>
            {
                CurrentView = CreateVM;
            });

            ConvertViewCommand = new RelayCommand(o =>
            {
                CurrentView = ConvertVM;
            });

            DirectViewCommand = new RelayCommand(o =>
            {
                CurrentView = DirectVM;
            });

            DirectTableViewCommand = new RelayCommand(o =>
            {
                CurrentView = DirectTableVM;
            });

            SettingsViewCommand = new RelayCommand(o =>
            {
                CurrentView = SettingsVM;
            });

            InfoViewCommand = new RelayCommand(o =>
            {
                CurrentView = InfoVM;
            });
        }
    }
}
