using System.ComponentModel;

namespace MiBocaRecuerda
{
    public class Counter : INotifyPropertyChanged
    {
        private readonly int _inf;
        private readonly int _sup;

        private int _cnt;
        public int Cnt
        {
            get => _cnt;
            set
            {
                int c = value;

                // 下限より下は下限に、上限より上は上限にする
                if (c < _inf) c = _inf;
                if (c > _sup) c = _sup;

                _cnt = c;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Cnt)));
            }
        }

        public Counter(int cnt, int infimum = 0, int supremum = 10000)
        {
            _cnt = cnt;
            _inf = infimum;
            _sup = supremum;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
