namespace LiteSpecs
{
    public class SpecificationResult
    {
        private static readonly string[] EmptyReasons = new string[0];

        public bool IsSatisfied { get; }
        public string[] Reasons { get; }

        private SpecificationResult(bool isSatisfied, string[] reasons)
        {
            IsSatisfied = isSatisfied;
            Reasons = reasons;
        }

        public static readonly SpecificationResult Satisfied = new SpecificationResult(true, EmptyReasons);

        public static SpecificationResult NotSatisfied(params string[] reasons)
            => new SpecificationResult(false, reasons);
    }
}