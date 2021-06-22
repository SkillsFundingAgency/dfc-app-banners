using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DFC.App.Banners.Data.Helpers
{
    [ExcludeFromCodeCoverage]
    public static class SharedContentKeyHelper
    {
        private static Guid SpeakToAnAdviserSharedContentKey => Guid.Parse("2c9da1b3-3529-4834-afc9-9cd741e59788");

        public static IEnumerable<Guid> GetSharedContentKeys()
        {
            return new List<Guid>
            {
                SpeakToAnAdviserSharedContentKey,
            };
        }
    }
}
