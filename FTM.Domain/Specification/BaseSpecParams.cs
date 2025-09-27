using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTM.Domain.Specification
{
    public class BaseSpecParams
    {
        private const int MaxPageSize = 50;
        public int Skip { get; set; } = 0;

        private int _pageSize = 6;
        public int Take
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string OrderBy { get; set; }
        private string _search;
        public string Search
        {
            get => _search;
            set => _search = value?.ToLower();
        }

        public bool Manage { get; set; }

        public string PropertyFilters { get; set; }
    }
}
