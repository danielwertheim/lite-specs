using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace LiteSpecs.UnitTests
{
    public class LiteSpecsTests
    {
        private readonly List<int> _data = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private readonly int[] _oddValues = { 1, 3, 5, 7, 9 };
        private readonly int[] _evenValues = { 2, 4, 6, 8, 10 };

        [Fact]
        public void Can_filter_enumerables()
        {
            var oddsByEnum1 = _data.Matching(TestSpecs.OddNumbers).ToArray();
            var oddsByEnum2 = TestSpecs.OddNumbers.ApplyOn(_data).ToArray();
            var oddsByEnum3 = TestSpecs.OddNumbersTyped.ApplyOn(_data).ToArray();

            oddsByEnum1.Should().BeEquivalentTo(_oddValues);
            oddsByEnum2.Should().BeEquivalentTo(_oddValues);
            oddsByEnum3.Should().BeEquivalentTo(_oddValues);
        }

        [Fact]
        public void Can_negate_filter_against_enumerables()
        {
            var evens = _data.Matching(TestSpecs.OddNumbers.Not("Was even."));
            var allButOnes = _data.Matching(TestSpecs.NegatedOnes).ToArray();

            evens.Should().BeEquivalentTo(_evenValues);
            allButOnes.Should().BeEquivalentTo(_data.Where(i => i != 1));
        }

        [Fact]
        public void Can_negate_filter_against_enumerables_using_operator()
        {
            var oddNumbers = Specification.Generic<int>(i => i % 2 != 0, "Not an odd number");
            var evens = _data.Where(!oddNumbers);

            evens.Should().BeEquivalentTo(_evenValues);
        }

        [Fact]
        public void Can_use_And_to_filter_enumerables()
        {
            var combined = TestSpecs
                .OddNumbers
                .And(TestSpecs.Ones);

            var onesAndThrees = _data.Matching(combined).ToArray();

            onesAndThrees.Should().BeEquivalentTo(new[] { 1 });
        }

        [Fact]
        public void Can_use_AndAlso_to_filter_enumerables()
        {
            var combined = TestSpecs
                .OddNumbers
                .AndAlso(TestSpecs.Ones);

            var onesAndThrees = _data.Matching(combined).ToArray();

            onesAndThrees.Should().BeEquivalentTo(new[] { 1 });
        }

        [Fact]
        public void Can_use_Or_to_filter_enumerables()
        {
            var combined = TestSpecs
                .Ones
                .Or(TestSpecs.Threes);

            var onesOrThrees = _data.Matching(combined).ToArray();

            onesOrThrees.Should().BeEquivalentTo(new[] { 1, 3 });
        }

        [Fact]
        public void Can_combine_specs_and_filter_enumerables()
        {
            var combined = TestSpecs.AllNumbers
                .AndAlso(TestSpecs.OddNumbers)
                .And(TestSpecs.Ones.Or(TestSpecs.Threes));

            var onesAndThrees = _data.Matching(combined).ToArray();

            onesAndThrees.Should().BeEquivalentTo(new[] { 1, 3 });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void Or_Should_not_provide_a_reason_When_satisfied(int value)
        {
            var result = TestSpecs.Ones.Or(TestSpecs.Threes).Eval(value);

            result.IsSatisfied.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void And_Should_not_provide_a_reason_When_satisfied(int value)
        {
            var result = TestSpecs.AllNumbers.And(TestSpecs.OddNumbers).Eval(value);

            result.IsSatisfied.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public void AndAlso_Should_not_provide_a_reason_When_satisfied(int value)
        {
            var result = TestSpecs.AllNumbers.AndAlso(TestSpecs.OddNumbers).Eval(value);

            result.IsSatisfied.Should().BeTrue();
            result.Reasons.Should().BeEmpty();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void Or_Should_provide_a_reason_When_not_satisfied(int value)
        {
            var result = TestSpecs.Ones.Or(TestSpecs.Threes).Eval(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Not a 1.", "Not a 3.");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void Or_Should_provide_a_deap_reason_When_not_satisfied(int value)
        {
            var result = TestSpecs.OddNumbers.Or(TestSpecs.Ones.Or(TestSpecs.Threes)).Eval(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Not an odd integer.", "Not a 1.", "Not a 3.");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void And_Should_provide_a_reason_When_not_satisfied(int value)
        {
            var result = TestSpecs.Ones.And(TestSpecs.Threes).Eval(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Not a 1.", "Not a 3.");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void And_Should_provide_a_deap_reason_When_not_satisfied(int value)
        {
            var result = TestSpecs.OddNumbers.And(TestSpecs.Ones.And(TestSpecs.Threes)).Eval(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Not an odd integer.", "Not a 1.", "Not a 3.");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        public void AndAlso_Should_provide_a_reason_for_left_predicate_When_not_satisfied(int value)
        {
            var result = TestSpecs.Ones.AndAlso(TestSpecs.Threes).Eval(value);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Not a 1.");
        }

        [Fact]
        public void Not_Should_provide_a_reason_When_not_satisfied()
        {
            var result = TestSpecs.NegatedOnes.Eval(1);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("Was a 1.");
        }

        [Fact]
        public void Not_Should_provide_a_generic_reason_When_one_is_provided_and_it_is_not_satisfied()
        {
            var spec = TestSpecs.Ones.Not("That did not work!");

            var result = spec.Eval(1);

            result.IsSatisfied.Should().BeFalse();
            result.Reasons.Should().BeEquivalentTo("That did not work!");
        }
    }
}
