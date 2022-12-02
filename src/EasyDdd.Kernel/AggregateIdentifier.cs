
namespace EasyDdd.Kernel
{
    public record AggregateIdentifier(string BoundedContextName, string CollectionName, string Identifier)
    {
        public override string ToString() => $"{BoundedContextName}.{CollectionName}.{Identifier}";
    }
}
