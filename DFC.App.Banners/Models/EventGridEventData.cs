﻿using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Banners.Models
{
    [ExcludeFromCodeCoverage]
    public class EventGridEventData
    {
        public string? Api { get; set; }

        public string? ItemId { get; set; }

        public string? VersionId { get; set; }

        public string? DisplayText { get; set; }

        public string? Author { get; set; }

        public string? ContentType { get; set; }
    }
}