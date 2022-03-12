using AutoFixture;
using AutoFixture.Xunit2;

namespace AlwaysOn.BackgroundProcessor.UnitTests.Services.Fixtures
{
    public class ItemBodyAutoDataAttribute : AutoDataAttribute
    {
        public ItemBodyAutoDataAttribute() : base(() => new Fixture()
            .Customize(new ItemBodyCustomisation()))
        {
        }
    }}