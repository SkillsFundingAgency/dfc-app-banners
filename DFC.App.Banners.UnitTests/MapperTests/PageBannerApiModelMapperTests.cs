//using System.Collections.Generic;

//using AutoMapper;

//using DFC.App.Banners.AutoMapperProfiles;
//using DFC.App.Banners.Data.Models.CmsApiModels;
//using DFC.App.Banners.Data.Models.ContentModels;
//using DFC.Content.Pkg.Netcore.Data.Contracts;

//using FluentAssertions;

//using Xunit;

//namespace DFC.App.Banners.UnitTests.MapperTests
//{
//    public class PageBannerApiModelMapperTests
//    {
//        [Theory]
//        [InlineData("", "/")]
//        [InlineData("/", "/")]
//        [InlineData("randompage", "/randompage")]
//        [InlineData("randompage/", "/randompage")]
//        [InlineData("/randompage", "/randompage")]
//        [InlineData("/randompage/", "/randompage")]
//        [InlineData("alerts/404", "/alerts/404")]
//        [InlineData("alerts/404/", "/alerts/404")]
//        [InlineData("/alerts/404", "/alerts/404")]
//        [InlineData("/alerts/404/", "/alerts/404")]
//        [InlineData("https://nationalcareers.service.gov.uk", "/")]
//        [InlineData("https://nationalcareers.service.gov.uk/", "/")]
//        [InlineData("https://nationalcareers.service.gov.uk/alerts/404", "/alerts/404")]
//        [InlineData("https://nationalcareers.service.gov.uk/alerts/404/", "/alerts/404")]
//        public void MapPageBannersPageLocation(string pageLocation, string expectedPartitionKey)
//        {
//            // Arrange
//            var pageBannerApiModel = new PageBannerContentItemApiDataModel
//            {
//                ItemId = System.Guid.NewGuid(),
//                PageName = "some page",
//                PageLocation = pageLocation,
//            };

//            var configuration = new MapperConfiguration(config => config.AddProfile<PageBannerContentItemModelProfile>());
//            var context = new Mapper(configuration);

//            // Act
//            var result = context.Map<PageBannerContentItemModel>(pageBannerApiModel);

//            // Assert
//            result.Should().NotBeNull();
//            result.Id.Should().Be(pageBannerApiModel.ItemId.Value);
//            result.PageName.Should().Be(pageBannerApiModel.PageName);
//            result.PartitionKey.Should().Be(expectedPartitionKey);
//            result.PageLocation.Should().Be(expectedPartitionKey);
//        }

//        [Fact]
//        public void MapBannersFromBaseType()
//        {
//            // Arrange
//            var bannerApiModel = new BannerContentItemApiDataModel
//            {
//                ContentType = Constants.ContentTypes.Banner,
//                Title = "Some Banner",
//                Ordinal = 0,
//                IsActive = true,
//                IsGlobal = true,
//                UseBrowserWidth = true,
//                Content = "Some Content",
//            };

//            var pageBannerApiModel = new PageBannerContentItemApiDataModel
//            {
//                ItemId = System.Guid.NewGuid(),
//                PageName = "some page",
//                PageLocation = "/",
//                ContentItems = new List<IBaseContentItemModel> { bannerApiModel },
//            };

//            var configuration = new MapperConfiguration(config => config.AddProfile<PageBannerContentItemModelProfile>());
//            var context = new Mapper(configuration);

//            // Act
//            var result = context.Map<PageBannerContentItemModel>(pageBannerApiModel);

//            // Assert
//            result.Should().NotBeNull();
//            result.Banners.Should().NotBeEmpty()
//                .And.HaveCount(1);
//            result.Banners[0].Title.Should().Be(bannerApiModel.Title);
//            result.Banners[0].Ordinal.Should().Be(bannerApiModel.Ordinal);
//            result.Banners[0].IsActive.Should().Be(bannerApiModel.IsActive);
//            result.Banners[0].IsGlobal.Should().Be(bannerApiModel.IsGlobal);
//            result.Banners[0].UseBrowserWidth.Should().Be(bannerApiModel.UseBrowserWidth);
//            result.Banners[0].Content.Should().Be(bannerApiModel.Content);
//        }
//    }
//}
