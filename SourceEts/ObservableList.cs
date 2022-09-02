using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace SourceEts
{
    [Serializable]
    public class ObservableList<T> : ObservableCollection<T>
    {
        private bool suppressNotification;

        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChangedMultiItem(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler handlers = CollectionChanged;
            if (handlers != null)
            {
                foreach (NotifyCollectionChangedEventHandler handler in
                    handlers.GetInvocationList())
                {
                    if (handler.Target is System.Windows.Data.CollectionView view)
                    {
                        view.Refresh();
                    }
                    else
                    {
                        handler(this, e);
                    }
                }
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!suppressNotification)
            {
                base.OnCollectionChanged(e);
                if (CollectionChanged != null)
                {
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        CollectionChanged.Invoke(this, e);
                    });
                }
            }
        }

        public void AddRange(IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            suppressNotification = true;

            foreach (T item in list)
            {
                Add(item);
            }

            suppressNotification = false;

            OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, list));
        }

        public void RemoveRange(IEnumerable<T> list)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            suppressNotification = true;

            List<T> removed = new List<T>();
            foreach (T item in list)
            {
                if (IndexOf(item) > -1)
                {
                    Remove(item);
                    removed.Add(item);
                }
            }
            suppressNotification = false;

            OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
        }

        public void RemoveRange(int startIndex, int count)
        {
            if (startIndex < 0)
            {
                throw new ArgumentNullException("list");
            }
            if (startIndex + count > this.Count)
            {
                throw new ArgumentNullException("list");
            }

            suppressNotification = true;

            List<T> removed = new List<T>();

            for (int n = startIndex + count - 1; n >= startIndex; n--)
            {
                removed.Add(this[n]);
                RemoveAt(n);
            }

            suppressNotification = false;

            OnCollectionChangedMultiItem(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed));
        }
    }
}