# LiteSpecs
Contains an opinionated implementation of in-memory specifications, allowing you to report multiple reasons upon why an evaluated specificaton was not "happy".

## Sample

Define some specifications. Either generic or typed:

```csharp
static class TestSpecs
{
    static readonly ISpecification<int> AllNumbers = Specification<int>.All;

    static readonly ISpecification<int> OddNumbers = Specification.Generic<int>(
        i => i % 2 != 0,
        "Not an odd integer.");

    static readonly ISpecification<int> Ones = Specification.Generic<int>(
        i => i == 1,
        "Not a 1.");

    static readonly ISpecification<int> Threes = Specification.Generic<int>(
        i => i == 3,
        "Not a 3.");

    static readonly ISpecification<VehicleObservation> CanBeObservationOfMotorcycle
        = new CanBeObservationOfMotorcycle();
    
    static readonly ISpecification<VehicleObservation> CanBeObservationOfCar
        = new CanBeObservationOfCar();
    
    static readonly ISpecification<VehicleObservation> CanBeObservationOfCar2
        = Specification.Generic<VehicleObservation>(i =>
        {
            if (!i.HasEngine)
                return SpecificationIs.NotSatisfied("A car must have an engine.");

            if (i.WheelCount != 4)
                return SpecificationIs.NotSatisfied("A car must have 4 wheels.");

            return SpecificationIs.Satisfied;
        });
}

public class CanBeObservationOfCar : Specification<VehicleObservation>
{
    public CanBeObservationOfCar() : base(o =>
    {
        if (!o.HasEngine)
            return SpecificationIs.NotSatisfied("A car must have an engine.");

        if (o.WheelCount != 4)
            return SpecificationIs.NotSatisfied("A car must have 4 wheels.");

        return SpecificationIs.Satisfied;
    })
    { }
}

public class CanBeObservationOfMotorcycle : Specification<VehicleObservation>
{
    public CanBeObservationOfMotorcycle() : base(
        o => o.HasEngine && o.WheelCount == 2,
        "It can not be a motorcycle.")
    { }
}
```

Use them:

```csharp
[Fact]
public void Can_combine_specs_to_filter_enumerables()
{
    var combined = TestSpecs
        .AllNumbers
        .AndAlso(TestSpecs.OddNumbers)
        .And(TestSpecs.Ones.Or(TestSpecs.Threes));

    var onesAndThrees = new[] { 1, 2, 3, 4 }.Matching(combined).ToArray();

    onesAndThrees.Should().BeEquivalentTo(new[] { 1, 3 });
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

[Fact]
public void Can_handle_non_satisfying_evaluations()
{
    var r = TestSpecs.CanBeObservationOfCar.Eval(_fourWheelerWithoutEngine);
    r.IsSatisfied.Should().BeFalse();
    r.Reasons.Should().BeEquivalentTo("A car must have an engine.");
}
```