using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace TripleZeroApi.Models
{
    public class HttpServiceResult
    {
        public string ErrorCode { get; set; }
        public HttpStatusCode HttpStatus { get; set; }
        public string Message { get; set; }
    }

    public class HttpServiceResultItemList<T> : HttpServiceResult where T : class, new()
    {
        public List<T> Item { get; set; }
    }

    public class HttpServiceResultItem<T> : HttpServiceResult where T : class, new()
    {
        public object Item { get; set; }
    }

    public class HttpServiceResultItem : HttpServiceResult 
    {
        public int Item { get; set; }
    }
}

