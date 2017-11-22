namespace LiteSpecs
{
    public sealed class AllSpecification<T> : Specification<T>
    {
        private AllSpecification() : base(_ => SpecificationIs.Satisfied) { }

        internal static AllSpecification<T> Create() => new AllSpecification<T>();
    }
}