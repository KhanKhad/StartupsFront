using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace StartupsFront.MVVM
{
    public class wObservableCollection<T> : ObservableCollection<T>, IReadOnlyObservableCollection<T>
    {
        public wObservableCollection()
        {
            CollectionChanged += sObservableCollection_CollectionChanged;
        }

        public wObservableCollection(IEnumerable<T> collection) : base(collection)
        {
            CollectionChanged += sObservableCollection_CollectionChanged;
        }

        /// <summary>
        /// блокировка посылки сообщения об добавление или удаление элементов
        /// </summary>
        bool lockAddOrRemove = false;

        /// <summary>
        /// Добавляет коллекцию элементов
        /// </summary>
        /// <param name="newItems"></param>
        public void AddRange(IEnumerable<T> newItems)
        {
            var nia = newItems.ToArray();
            lockAddOrRemove = true;
            foreach (var ni in nia)
                this.Add(ni);
            lockAddOrRemove = false;
            Added?.Invoke(this, nia);
        }

        /// <summary>
        /// Удаляет коллекцию элементов
        /// </summary>
        /// <param name="oldItems"></param>
        public void RemoveRange(IEnumerable<T> oldItems)
        {
            lockAddOrRemove = true;
            foreach (var ni in oldItems)
                this.Remove(ni);
            lockAddOrRemove = false;
            Removed?.Invoke(this, oldItems.ToArray());
        }

        /// <summary>
        /// Удаляет Набор элементов
        /// </summary>
        /// <param name="oldItems"></param>
        public void RemoveRange(int startIndex, int count)
        {
            var removed = new List<T>(count);
            lockAddOrRemove = true;
            for (int i = startIndex; i < count; i++)
                removed.Add(this[i]);
            foreach (var ni in removed)
                this.Remove(ni);
            lockAddOrRemove = false;

            Removed?.Invoke(this, removed.ToArray());
        }

        /// <summary>
        /// Сначала полностью чистит эту коллекцию, затем добавляет newItems
        /// </summary>
        /// <param name="newItems"></param>
        public void ClearThenAddRange(IEnumerable<T> newItems)
        {
            var newItemsArr = newItems.ToArray();
            this.Clear();

            lockAddOrRemove = true;

            foreach (var ni in newItemsArr)
                this.Add(ni);

            lockAddOrRemove = false;
            if (newItemsArr.Length > 0)
                Added?.Invoke(this, newItemsArr);
        }

        /// <summary>
        /// Очищает эту коллекцию
        /// </summary>
        protected override void ClearItems()
        {
            var old = Items.ToArray();

            base.ClearItems();

            if (!lockAddOrRemove && old.Length > 0)
                Removed?.Invoke(this, old);
        }

        void sObservableCollection_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (lockAddOrRemove || e.Action == NotifyCollectionChangedAction.Move) return;
            if (e.NewItems != null)
                Added?.Invoke(this, e.NewItems.Cast<T>());
            if (e.OldItems != null)
                Removed?.Invoke(this, e.OldItems.Cast<T>());
        }
        /// <summary>
        /// кто-то добавился
        /// </summary>
        public event wCollectionChanged<T> Added;
        /// <summary>
        /// кто-то удалился
        /// </summary>
        public event wCollectionChanged<T> Removed;
    }

    public delegate void wCollectionChanged<T>(object sender, IEnumerable<T> Elements);
}
