using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace LiteSpecs.UnitTests
{
    public class LiteSpecsTests
    {
        private static class TestSpecs
        {
            internal static readonly Specification<int> AllNumbers = Specification<int>.All;
            internal static readonly Specification<int> OddNumbers = Specification.Generic<int>(i => i % 2 != 0
                ? SpecificationResult.Satisfied
                : SpecificationResult.NotSatisfied("Not an odd integer"));
            internal static readonly Specification<int> OddNumbersTyped = new OddIntsSpecification();
            internal static readonly Specification<int> Ones = Specification.Generic<int>(i => i == 1
                ? SpecificationResult.Satisfied
                : SpecificationResult.NotSatisfied("Not a 1"));
            internal static readonly Specification<int> Threes = Specification.Generic<int>(i => i == 3
                ? SpecificationResult.Satisfied
                : SpecificationResult.NotSatisfied("Not a 3"));
            internal static readonly Specification<int> NotOnes = Ones.Not("Is a one");
        }

        private readonly List<int> _data = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private readonly int[] _oddValues = { 1, 3, 5, 7, 9 };
        private readonly int[] _evenValues = { 2, 4, 6, 8, 10 };

        public class OddIntsSpecification : Specification<int>
        {
            public OddIntsSpecification() : base(
                i => i % 2 != 0
                ? SpecificationResult.Satisfied
                : SpecificationResult.NotSatisfied("Not an odd integer"))
            { }
        }

        [Fact]
        public void Can_filter_enumerables()
        {
            var oddsByEnum1 = _data.Where(TestSpecs.OddNumbers).ToArray();
            var oddsByEnum2 = TestSpecs.OddNumbers.ApplyOn(_data).ToArray();
            var oddsByEnum3 = TestSpecs.OddNumbersTyped.ApplyOn(_data).ToArray();

            oddsByEnum1.Should().BeEquivalentTo(_oddValues);
            oddsByEnum2.Should().BeEquivalentTo(_oddValues);
            oddsByEnum3.Should().BeEquivalentTo(_oddValues);
        }

        [Fact]
        public void Can_negate_filter_against_enumerables()
        {
            var evens = _data.Where(TestSpecs.OddNumbers.Not("Is odd"));
            var allButOnes = _data.Where(TestSpecs.NotOnes).ToArray();

            evens.Should().BeEquivalentTo(_evenValues);
            allButOnes.Should().BeEquivalentTo(_data.Where(i => i != 1));
        }

        [Fact]
        public void Can_combine_specs_and_filter_enumerables()
        {
            var combined = TestSpecs
                .AllNumbers
                .And(TestSpecs.OddNumbersTyped)
                .And(TestSpecs.Ones.Or(TestSpecs.Threes));

            var onesAndThrees = _data.Where(combined).ToArray();

            onesAndThrees.Should().BeEquivalentTo(new[] { 1, 3 });
        }

        [Fact]
        public void Spec_Should_not_provide_a_reason_When_satisfied()
        {
            var result = TestSpecs.Ones.IsSatisfiedBy(1);

            result.IsSatisfied.Should().BeTrue();
            result.Reason.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void Or_Should_not_provide_a_reason_When_satisfied(int value)
        {
            var result = TestSpecs.Ones.Or(TestSpecs.Threes).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeTrue();
            result.Reason.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void And_Should_not_provide_a_reason_When_satisfied(int value)
        {
            var result = TestSpecs.AllNumbers.And(TestSpecs.OddNumbers).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeTrue();
            result.Reason.Should().BeNull();
        }

        [Fact]
        public void Spec_Should_provide_a_reason_When_not_satisfied()
        {
            var result = TestSpecs.Ones.IsSatisfiedBy(2);

            result.IsSatisfied.Should().BeFalse();
            result.Reason.Should().Be("Not a 1");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void Or_Should_provide_a_reason_When_non_satisfied_with_a_specified_reason(int value)
        {
            var result = TestSpecs.Ones.Or(TestSpecs.Threes, "Not a 1 or a 3").IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reason.Should().Be("Not a 1 or a 3");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void Or_Should_provide_a_unknown_reason_When_non_satisfied_without_a_specified_reason(int value)
        {
            var result = TestSpecs.Ones.Or(TestSpecs.Threes).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reason.Should().Be(SpecificationResult.UnknownReason);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void And_Should_provide_a_reason_When_non_satisfied_with_a_specified_reason(int value)
        {
            var result = TestSpecs.Ones.And(TestSpecs.Threes, "That didn't work").IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reason.Should().Be("That didn't work");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void And_Should_provide_a_unknown_reason_When_non_satisfied_without_a_specified_reason(int value)
        {
            var result = TestSpecs.Ones.And(TestSpecs.Threes).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reason.Should().Be(SpecificationResult.UnknownReason);
        }

        [Fact]
        public void Not_Should_provide_a_reason_When_non_satisfied_with_a_specified_reason()
        {
            var result = TestSpecs.Ones.Not("Should not have been a 1.").IsSatisfiedBy(1);

            result.IsSatisfied.Should().BeFalse();
            result.Reason.Should().Be("Should not have been a 1.");
        }

        [Fact]
        public void Not_Should_provide_a_unknown_reason_When_non_satisfied_with_a_specified_reason()
        {
            var result = TestSpecs.Ones.Not().IsSatisfiedBy(1);

            result.IsSatisfied.Should().BeFalse();
            result.Reason.Should().Be(SpecificationResult.UnknownReason);
        }
    }
}
