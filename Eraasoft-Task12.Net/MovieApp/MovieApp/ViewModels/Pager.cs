using System;
using System.Collections.Generic;

namespace MovieApp.ViewModels
{
    public class Pager
    {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        public int StartRecord { get; private set; }
        public int EndRecord { get; private set; }

        public Pager(int totalItems, int currentPage = 1, int pageSize = 10, int maxPages = 5)
        {
            // Calculate total pages
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

            // Ensure current page isn't out of range
            if (currentPage < 1)
            {
                CurrentPage = 1;
            }
            else if (currentPage > TotalPages)
            {
                CurrentPage = TotalPages == 0 ? 1 : TotalPages;
            }

            // Calculate start and end pages
            int startPage = currentPage - (maxPages / 2);
            int endPage = currentPage + (maxPages / 2) - (maxPages % 2 == 0 ? 1 : 0);

            if (startPage < 1)
            {
                endPage = Math.Min(endPage + (1 - startPage), TotalPages);
                startPage = 1;
            }

            if (endPage > TotalPages)
            {
                startPage = Math.Max(1, startPage - (endPage - TotalPages));
                endPage = TotalPages;
            }

            StartPage = startPage;
            EndPage = endPage;

            // Calculate start and end record indexes
            StartRecord = (currentPage - 1) * pageSize + 1;
            EndRecord = Math.Min(StartRecord + pageSize - 1, totalItems);
        }

        public List<int> Pages()
        {
            var pages = new List<int>();
            for (var i = StartPage; i <= EndPage; i++)
            {
                pages.Add(i);
            }
            return pages;
        }
    }
}