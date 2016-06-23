using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Data;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;

namespace MyHub.ComponentModel
{
    public class IncrementalLoadingCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        private bool _isBusy;
        private Func<Task<Tuple<IList<T>, bool>>> _loadMoreData;
        private int _count;

        public IncrementalLoadingCollection(Func<Task<Tuple<IList<T>, bool>>> loadMoreData)
        {
            if (loadMoreData == null)
                throw new ArgumentNullException("loadMoreData");

            _isBusy = false;
            _loadMoreData = loadMoreData;
            HasMoreItems = true;
            _count = 0;
        }

        public bool HasMoreItems { get; private set; }

        public event EventHandler LoadMoreStarted;
        public event EventHandler LoadMoreComplated;

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            if(_isBusy)
            {
                throw new InvalidOperationException("当前增量加载尚未完成");
            }

            _isBusy = true;

            return AsyncInfo.Run(c => LoadMoreItemsAsync(c, count));
        }

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count)
        {
            try
            {
                LoadMoreStarted?.Invoke(this, EventArgs.Empty);
                ++_count;
                var result = await _loadMoreData();
                var items = result.Item1;
                if (items != null && items.Count > 0)
                {
                    foreach (T item in items)
                    {
                        Add(item);
                    }
                }
                //if (items != null && items.Count > 0)
                //{
                //    for(int i = 0; i < count; ++i)
                //    {
                //        Add(items[0]);
                //    }
                //}
                HasMoreItems = result.Item2;
                
                LoadMoreComplated?.Invoke(this, EventArgs.Empty);
                return new LoadMoreItemsResult() { Count = items == null ? 0 : (uint)items.Count };
            }
            finally
            {
                _isBusy = false;
            }
        }
    }
}
