using AutoMapper;

using DFC.App.Banners.AutoMapperProfiles;
using DFC.App.Banners.Data.Models.CmsApiModels;
using DFC.App.Banners.Data.Models.ContentModels;

using FluentAssertions;

using Xunit;

namespace DFC.App.Banners.UnitTests.MapperTests
{
    public class PageBannerApiModelMapperTests
    {
        [Theory]
        [InlineData("", "/")]
        [InlineData("randompage", "/randompage")]
        [InlineData("alerts/404", "/alerts/404")]
        [InlineData("/alerts/404", "/alerts/404")]
        [InlineData("https://nationalcareers.service.gov.uk/", "/")]
        [InlineData("https://nationalcareers.service.gov.uk/alerts/404", "/alerts/404")]
        public void MapPageBannersPageLocation(string pageLocation, string expectedPartitionKey)
        {
            // Arrange
            var pageBannerApiModel = new PageBannerContentItemApiDataModel
            {
                ItemId = System.Guid.NewGuid(),
                PageName = "some page",
                PageLocation = pageLocation,
            };

            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<PageBannerContentItemModelProfile>());
            var context = new Mapper(configuration);

            // Act
            var result = context.Map<PageBannerContentItemModel>(pageBannerApiModel);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(pageBannerApiModel.ItemId.Value);
            result.PageName.Should().Be(pageBannerApiModel.PageName);
            result.PartitionKey.Should().Be(expectedPartitionKey);
            result.PageLocation.Should().Be(expectedPartitionKey);
        }
    }
}
