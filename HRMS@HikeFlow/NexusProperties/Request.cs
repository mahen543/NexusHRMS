using System;
using System.Collections.Generic;
using System.Text;

namespace HRMS_HikeFlow.NexusProperties
{
    public class Request
    {
        public string? SearchWord { get; set; } = null;

        public string? Id { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DateDeleted { get; set; }

    }
}
