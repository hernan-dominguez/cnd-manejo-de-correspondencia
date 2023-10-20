using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Shared.Helpers
{
    public class PageOperationResult
    {
        public string Id { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        public DateTime? TimeValue { get; set; }

        public string TimeString { get; set; }

        public object Content { get; set; }
    }
}
