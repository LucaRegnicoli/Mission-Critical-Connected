using AlwaysOn.Shared;
using AlwaysOn.Shared.Models;
using AutoFixture;

namespace AlwaysOn.BackgroundProcessor.UnitTests.Services.Fixtures
{
    public class ItemBodyCustomisation : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            var item = fixture.Create<CatalogItem>();

            fixture.Customize<ItemBody>(composer => composer
                .With(request => request.Body, Helpers.JsonSerialize(item)));
        }
    }
}