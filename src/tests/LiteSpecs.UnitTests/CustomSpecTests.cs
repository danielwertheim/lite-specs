using FluentAssertions;
using Xunit;

namespace LiteSpecs.UnitTests
{
    public class CustomSpecTests
    {
        private readonly VehicleObservation _fourWheelerWithEngine = new VehicleObservation
        {
            HasEngine = true,
            WheelCount = 4
        };

        private readonly VehicleObservation _fourWheelerWithoutEngine = new VehicleObservation
        {
            HasEngine = false,
            WheelCount = 4
        };

        [Fact]
        public void Can_handle_satisfying_evaluations()
        {
            var r = TestSpecs.CanBeObservationOfCar.Eval(_fourWheelerWithEngine);
            r.IsSatisfied.Should().BeTrue();
            r.Reasons.Should().BeEmpty();
        }

        [Fact]
        public void Can_handle_non_satisfying_evaluations()
        {
            var r = TestSpecs.CanBeObservationOfCar.Eval(_fourWheelerWithoutEngine);
            r.IsSatisfied.Should().BeFalse();
            r.Reasons.Should().BeEquivalentTo("A car must have an engine.");
        }
    }
}