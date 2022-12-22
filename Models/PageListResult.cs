using System;
using System.Collections.Generic;
using System.Text;

namespace Rio.Common.Models
{
    public interface IPageListResult<out T>: IListResultWithTotal<T>
    {
        int Count { get; }

        int PageNumber { get; }

        int PageSize { get; }

        int PageCount { get; }
    }


    public interface IListResultWithTotal<out T>
    {
        IReadOnlyList<T> Data { get; }
        
        int TotalCount { get; }
    }

    public static class EnumerableExtension
    {
        public static IEnumerator<T> GetEnumerator<T>(this IListResultWithTotal<T> listResult)
            => listResult.Data.GetEnumerator();
    }

    public class ListResultWithTotal<T>: IListResultWithTotal<T>
    {
        public static readonly ListResultWithTotal<T> Empty = new();

        private IReadOnlyList<T> _data = Array.Empty<T>();

        public IReadOnlyList<T> Data
        {
            get => _data;
            set=>_data=Guard.NotNull(value,nameof(value));
        }

        public int TotalCount { get; set; }
    }

    [Serializable]
    public class PageListResult<T>:IPageListResult<T>
    {
        public static readonly PageListResult<T> Empty = new();

        private IReadOnlyList<T> _data = Array.Empty<T>();

        public IReadOnlyList<T> Data
        {
            get => _data;
            set=>Guard.NotNull(value,nameof(_data));
        }

        private int _pageNumber = 1;

        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                if(value>0)
                {
                    _pageNumber = value;
                }
            }
        }

        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if(value>0)
                {
                    _pageSize = value;
                }
            }
        }

        public int _totalCount;

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                if(value>0)
                {
                    _totalCount = value;
                }
            }
        }

        public int PageCount => (_totalCount + _pageSize - 1) / _pageSize;

        public T this[int index]=>Data[index];

        public int Count => Data.Count;
    }
}
