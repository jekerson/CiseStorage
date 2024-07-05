namespace Application.Abstraction.Pagging
{
    public enum PageSizeType
    {
        Small,
        Medium,
        Large
    }
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public PageSizeType PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> items, int count, int pageNumber, PageSizeType pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)GetPageSize(pageSize));

            AddRange(items);
        }

        public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber, PageSizeType pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * GetPageSize(pageSize)).Take(GetPageSize(pageSize)).ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        private static int GetPageSize(PageSizeType pageSizeType)
        {
            return pageSizeType switch
            {
                PageSizeType.Small => 25,
                PageSizeType.Medium => 50,
                PageSizeType.Large => 100,
                _ => throw new ArgumentOutOfRangeException(nameof(pageSizeType), pageSizeType, null)
            };
        }
    }

}
