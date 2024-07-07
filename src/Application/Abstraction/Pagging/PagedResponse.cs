using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Abstraction.Pagging
{
    public class PagedResponse<T>
    {
        public List<T> Items { get; init; } = new List<T>();
        public int TotalPages { get; init; }
        public int TotalCount { get; init; }
    }
}