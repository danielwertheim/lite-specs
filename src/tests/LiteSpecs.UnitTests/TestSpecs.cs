namespace LiteSpecs.UnitTests
{
    internal static class TestSpecs
    {
        internal static readonly ISpecification<int> AllNumbers = Specification<int>.All;
        internal static readonly ISpecification<int> OddNumbers = Specification.Generic<int>(i => i % 2 != 0, "Not an odd integer.");
        internal static readonly ISpecification<int> OddNumbersTyped = new OddIntsSpecification();
        internal static readonly ISpecification<int> Ones = Specification.Generic<int>(i => i == 1, "Not a 1.");
        internal static readonly ISpecification<int> Threes = Specification.Generic<int>(i => i == 3, "Not a 3.");
        internal static readonly ISpecification<int> NegatedOnes = Ones.Not("Was a 1.");

        internal static readonly ISpecification<VehicleObservation> CanBeObservationOfCar = new CanBeObservationOfCar();
        internal static readonly ISpecification<VehicleObservation> CanBeObservationOfMotorcycle = new CanBeObservationOfMotorcycle();
        internal static readonly ISpecification<VehicleObservation> CanBeObervationOfSkateboard = new CanBeObervationOfSkateboard();
    }

    public class OddIntsSpecification : Specification<int>
    {
        public OddIntsSpecification() : base(i => i % 2 != 0, "Not an odd integer.") { }
    }

    public class VehicleObservation
    {
        public int WheelCount { get; set; }
        public bool HasEngine { get; set; }
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

    public class CanBeObervationOfSkateboard : ISpecification<VehicleObservation>
    {
        public ISpecificationResult Eval(VehicleObservation item)
        {
            if (!item.HasEngine && item.WheelCount == 4)
                return SpecificationIs.Satisfied;

            return SpecificationIs.NotSatisfied("No engines are allowed and only 4 wheels are applicable.");
        }
    }
}