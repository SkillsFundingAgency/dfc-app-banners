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
        [InlineData(null)]
        [InlineData(GuidEmpty)]
        public void CanCheckIfDocumentIdIsInvalid(Guid documentId)
        {
            // Arrange
            var model = CreateModel(documentId, "A title", "<p>some content</p>");

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count == 1);
            Assert.NotNull(vr.First(f => f.MemberNames.Any(a => a == nameof(model.Id))));
            Assert.Equal(string.Format(CultureInfo.InvariantCulture, FieldInvalidGuid, nameof(model.Id)), vr.First(f => f.MemberNames.Any(a => a == nameof(model.Id))).ErrorMessage);
        }

        [Theory]
        [InlineData("abcdefghijklmnopqrstuvwxyz")]
        [InlineData("0123456789")]
        [InlineData("abc")]
        [InlineData("xyz123")]
        [InlineData("abc_def")]
        [InlineData("abc-def")]
        public void CanCheckITitleIsValid(string title)
        {
            // Arrange
            var model = CreateModel(Guid.NewGuid(), title, "<p>some content</p>");

            // Act
            var vr = Validate(model);

            // Assert
            Assert.True(vr.Count == 0);
        }

        private BannerContentItemModel CreateModel(Guid documentId, string title, string content)
        {
            var model = new BannerContentItemModel
            {
                Id = documentId,
                Title = title,
                Url = new Uri("aaa-bbb", UriKind.Relative),
                Content = content,
                LastReviewed = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                LastCached = DateTime.UtcNow,
            };

            return model;
        }

        private List<ValidationResult> Validate(BannerContentItemModel model)
        {
            var vr = new List<ValidationResult>();
            var vc = new ValidationContext(model);
            Validator.TryValidateObject(model, vc, vr, true);

            return vr;
        }
    }
}
