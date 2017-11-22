using FluentAssertions;
using Xunit;

namespace LiteSpecs.UnitTests
{
    public class SpecificationResultTests
    {
        [Fact]
        public void Satisfied_Should_be_true_and_have_empty_reasons()
        {
            var x = SpecificationIs.Satisfied;

            x.IsSatisfied.Should().BeTrue();
            x.Reasons.Should().BeEmpty();
        }

        [Fact]
        public void NonSatisfied_Should_be_false_and_have_empty_reasons_When_nothing_is_passed()
        {
            var x = SpecificationIs.NotSatisfied();

            x.IsSatisfied.Should().BeFalse();
            x.Reasons.Should().BeEmpty();
        }

        [Fact]
        public void NonSatisfied_Should_be_false_and_have_passed_reasons_When_reasons_are_passed()
        {
            var reasons = new[] {"a", "b"};

            var x = SpecificationIs.NotSatisfied(reasons);

            x.IsSatisfied.Should().BeFalse();
            x.Reasons.Should().BeSameAs(reasons);
        }
    }
}