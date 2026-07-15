using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace HRMS_HikeFlow.NexusProperties
{
    public class Response<T>
    {
        [DataMember]
        public int ReturnCode { set; get; } = 0;

        [DataMember]
        public string? ReturnMessage { set; get; } = string.Empty;

        [DataMember]
        public string? ResponseTime { set; get; } = string.Empty;

        [DataMember]
        public long RecordCount { set; get; } = 0;

        [DataMember]
        public T? Headers { set; get; } = default(T);

        [DataMember]
        public T? Data { set; get; } = default(T);
    }
}
