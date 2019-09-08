namespace Heracles.DataModel
{
    /// <summary>
    /// Describes an <see cref="IOperation" /> that uses an URI template to point to the target of the request.
    /// </summary>
    public interface ITemplatedOperation : IOperation, ITemplatedResource<IOperation>
    {
    }
}
