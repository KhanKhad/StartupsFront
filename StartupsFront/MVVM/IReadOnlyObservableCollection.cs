using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace StartupsFront.MVVM
{
    public interface IReadOnlyObservableCollection<T> : IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        int IndexOf(T element);
        T this[int i] { get; }
        int Count { get; }

        event wCollectionChanged<T> Added;

        event wCollectionChanged<T> Removed;
    }
}
