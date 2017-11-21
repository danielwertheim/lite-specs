namespace LiteSpecs
{
    public sealed class AllSpecification<T> : Specification<T>
    {
        private AllSpecification() : base(_ => SpecificationResult.Satisfied) { }

        internal static AllSpecification<T> Create() => new AllSpecification<T>();
    }
}