namespace LiteSpecs
{
    public static class SpecificationIs
    {
        private static readonly string[] EmptyReasons = new string[0];

        public static readonly ISpecificationResult Satisfied = new SpecificationResult(true, EmptyReasons);

        public static ISpecificationResult NotSatisfied(params string[] reasons)
            => new SpecificationResult(false, reasons);

        private struct SpecificationResult : ISpecificationResult
        {
            public bool IsSatisfied { get; }
            public string[] Reasons { get; }

            internal SpecificationResult(bool isSatisfied, string[] reasons)
            {
                IsSatisfied = isSatisfied;
                Reasons = reasons;
            }
        }
    }
}