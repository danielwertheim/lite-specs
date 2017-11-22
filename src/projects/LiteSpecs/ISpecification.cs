namespace LiteSpecs
{
    public interface ISpecification<in T>
    {
        ISpecificationResult Eval(T item);
    }
}