using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using DFC.App.Banners.Data.Models.ContentModels;
using Xunit;

namespace DFC.App.Banners.Data.UnitTests.ValidationTests
{
    [Trait("Category", "SharedContentItemModel Validation Unit Tests")]
    public class SharedContentItemModelValidationTests
    {
        private const string FieldInvalidGuid = "The field {0} has to be a valid GUID and cannot be an empty GUID.";
        private const string GuidEmpty = "00000000-0000-0000-0000-000000000000";

        [Theory]
        [InlineData(GuidEmpty)]
        public void CanCheckIfDocumentIdIsInvalid(Guid documentId)
        {
            // Arrange
            var model = CreateModel(documentId, "<p>some content</p>");

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count == 1);
            Assert.NotNull(vr.First(f => f.MemberNames.Any(a => a == nameof(model.Id))));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, FieldInvalidGuid, nameof(model.Id)), vr.First(f => f.MemberNames.Any(a => a == nameof(model.Id))).ErrorMessage);
        }

        private PageBannerContentItemModel CreateModel(Guid documentId, string content)
        {
            var model = new PageBannerContentItemModel
            {
                Id = documentId,
                Url = new Uri("aaa-bbb", UriKind.Relative),
                Content = content,
                LastReviewed = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                LastCached = DateTime.UtcNow,
            };

            return model;
        }

        private List<ValidationResult> Validate(PageBannerContentItemModel model)
        {
            var vr = new List<ValidationResult>();
            var vc = new ValidationContext(model);
            Validator.TryValidateObject(model, vc, vr, true);

            return vr;
        }
    }
}
