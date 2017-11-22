namespace LiteSpecs
{
    public interface ISpecificationResult
    {
        bool IsSatisfied { get; }
        string[] Reasons { get; }
    }
}