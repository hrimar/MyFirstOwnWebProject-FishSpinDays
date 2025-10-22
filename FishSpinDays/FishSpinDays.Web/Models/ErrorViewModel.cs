namespace FishSpinDays.Web.Models
{
    using System;

    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public int? StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public bool HasStatusInfo => StatusCode.HasValue && !string.IsNullOrEmpty(StatusMessage);
    }
}