namespace LiteSpecs
{
    public class SpecificationResult
    {
        public const string UnknownReason = "Unknown violation.";

        public bool IsSatisfied { get; }
        public string Reason { get; }

        private SpecificationResult(bool isSatisfied, string reason = null)
        {
            IsSatisfied = isSatisfied;
            Reason = reason;
        }

        public static readonly SpecificationResult Satisfied = new SpecificationResult(true);

        public static SpecificationResult NotSatisfied(string reason = null)
            => new SpecificationResult(false, reason ?? UnknownReason);

        public static implicit operator bool(SpecificationResult result)
            => result.IsSatisfied;
    }
}