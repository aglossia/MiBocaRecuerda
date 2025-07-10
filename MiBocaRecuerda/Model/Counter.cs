using System.ComponentModel;

namespace MiBocaRecuerda
{
    public class Counter : INotifyPropertyChanged
    {
        private int _cnt;
        public int Cnt
        {
            get => _cnt;
            set
            {
                //if (_cnt != value)
                //{
                    _cnt = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cnt)));
                //}
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
