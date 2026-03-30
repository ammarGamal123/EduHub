using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProject.Core.Wrappers
{
    public class PaginationResult<T>
    {
        public PaginationResult(List<T> data)
        {
            Data = data; 
        }
        public List<T> Data { get; set;}

        internal PaginationResult(bool succeeded, List<T> data = default,
                                  List<string> messages = null,
                                  int count = 0,
                                  int page = 1 , 
                                  int pageSize = 10)
        {
            Data = data;
            CurrentPage = page;
            Succeeded = succeeded;
            PageSize = pageSize;
            TotalPage = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
        }

        public static PaginationResult<T> Success (List<T> data , int count ,
                                                   int page , 
                                                   int pageSize)
        {
            return new(true, data, null, count, page, pageSize);
        }
        
        public int CurrentPage { get; set; }
        public bool Succeeded { get; set;}
        public int TotalCount {  get; set; }    
        public int TotalPage { get; set; }  
        public int PageSize { get; set; }
        public object Meta {  get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPage;
        public List<string> Messages { get; set; } = new();
    }
}
