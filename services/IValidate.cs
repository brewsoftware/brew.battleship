namespace Services
{
    public interface IValidate<T> where T: IAggregate
    {
        bool Validate(T state);
    }
}