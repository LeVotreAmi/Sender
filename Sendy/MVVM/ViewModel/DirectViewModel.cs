using Sendy.Core;

namespace Sendy.MVVM.ViewModel
{
    class DirectViewModel : ObservableObject
    {
        public RelayCommand DirectTableViewCommand { get; set; }

        public DirectTableViewModel DirectTableVM { get; set; }

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
        public DirectViewModel()
        {
            DirectTableVM = new DirectTableViewModel();

            DirectTableViewCommand = new RelayCommand(o =>
            {
                CurrentView = DirectTableVM;
            });
        }
    }
}