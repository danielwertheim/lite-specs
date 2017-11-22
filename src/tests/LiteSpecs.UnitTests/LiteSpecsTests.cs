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
                : SpecificationResult.NotSatisfied("Not an odd integer."));
            internal static readonly Specification<int> OddNumbersTyped = new OddIntsSpecification();
            internal static readonly Specification<int> Ones = Specification.Generic<int>(i => i == 1
                ? SpecificationResult.Satisfied
                : SpecificationResult.NotSatisfied("Not a 1."));
            internal static readonly Specification<int> Threes = Specification.Generic<int>(i => i == 3
                ? SpecificationResult.Satisfied
                : SpecificationResult.NotSatisfied("Not a 3."));
            internal static readonly Specification<int> NotOnes = Ones.Not("Should have been a 1.");
        }

        private readonly List<int> _data = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private readonly int[] _oddValues = { 1, 3, 5, 7, 9 };
        private readonly int[] _evenValues = { 2, 4, 6, 8, 10 };

        public class OddIntsSpecification : Specification<int>
        {
            public OddIntsSpecification() : base(
                i => i % 2 != 0
                ? SpecificationResult.Satisfied
                : SpecificationResult.NotSatisfied("Not an odd integer."))
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
            var evens = _data.Where(TestSpecs.OddNumbers.Not("Should be odd."));
            var allButOnes = _data.Where(TestSpecs.NotOnes).ToArray();

            evens.Should().BeEquivalentTo(_evenValues);
            allButOnes.Should().BeEquivalentTo(_data.Where(i => i != 1));
        }

        [Fact]
        public void Can_negate_filter_against_enumerables_using_operator()
        {
            var evens = _data.Where(!TestSpecs.OddNumbers);
            var allButOnes = _data.Where(!TestSpecs.Ones).ToArray();

            evens.Should().BeEquivalentTo(_evenValues);
            allButOnes.Should().BeEquivalentTo(_data.Where(i => i != 1));
        }

        [Fact]
        public void Can_use_And_to_filter_enumerables()
        {
            var combined = TestSpecs
                .OddNumbers
                .And(TestSpecs.Ones);

            var onesAndThrees = _data.Where(combined).ToArray();

            onesAndThrees.Should().BeEquivalentTo(new[] { 1 });
        }

        [Fact]
        public void Can_use_AndAlso_to_filter_enumerables()
        {
            var combined = TestSpecs
                .OddNumbers
                .AndAlso(TestSpecs.Ones);

            var onesAndThrees = _data.Where(combined).ToArray();

            onesAndThrees.Should().BeEquivalentTo(new[] { 1 });
        }

        [Fact]
        public void Can_use_Or_to_filter_enumerables()
        {
            var combined = TestSpecs
                .Ones
                .Or(TestSpecs.Threes);

            var onesOrThrees = _data.Where(combined).ToArray();

            onesOrThrees.Should().BeEquivalentTo(new[] { 1, 3 });
        }

        [Fact]
        public void Can_combine_specs_and_filter_enumerables()
        {
            var combined = TestSpecs.AllNumbers
                .AndAlso(TestSpecs.OddNumbers)
                .And(TestSpecs.Ones.Or(TestSpecs.Threes));

            var onesAndThrees = _data.Where(combined).ToArray();

            onesAndThrees.Should().BeEquivalentTo(new[] { 1, 3 });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void Or_Should_not_provide_a_reason_When_satisfied(int value)
        {
            var result = TestSpecs.Ones.Or(TestSpecs.Threes).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void And_Should_not_provide_a_reason_When_satisfied(int value)
        {
            var result = TestSpecs.AllNumbers.And(TestSpecs.OddNumbers).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void AndAlso_Should_not_provide_a_reason_When_satisfied(int value)
        {
            var result = TestSpecs.AllNumbers.AndAlso(TestSpecs.OddNumbers).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void Or_Should_provide_a_reason_When_not_satisfied(int value)
        {
            var result = TestSpecs.Ones.Or(TestSpecs.Threes).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Not a 1.", "Not a 3.");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void Or_Should_provide_a_deap_reason_When_not_satisfied(int value)
        {
            var result = TestSpecs.OddNumbers.Or(TestSpecs.Ones.Or(TestSpecs.Threes)).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Not an odd integer.", "Not a 1.", "Not a 3.");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void And_Should_provide_a_reason_When_not_satisfied(int value)
        {
            var result = TestSpecs.Ones.And(TestSpecs.Threes).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Not a 1.", "Not a 3.");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void And_Should_provide_a_deap_reason_When_not_satisfied(int value)
        {
            var result = TestSpecs.OddNumbers.And(TestSpecs.Ones.And(TestSpecs.Threes)).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Not an odd integer.", "Not a 1.", "Not a 3.");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void AndAlso_Should_provide_a_reason_for_left_predicate_When_not_satisfied(int value)
        {
            var result = TestSpecs.Ones.AndAlso(TestSpecs.Threes).IsSatisfiedBy(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Not a 1.");
        }

        [Fact]
        public void Not_Should_provide_a_reason_When_not_satisfied()
        {
            var result = TestSpecs.NotOnes.IsSatisfiedBy(1);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Should have been a 1.");
        }
    }
}
