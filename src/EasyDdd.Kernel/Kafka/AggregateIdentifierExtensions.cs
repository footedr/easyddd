
namespace EasyDdd.Kernel.Kafka
{
    public static class AggregateIdentifierExtensions
    {
        public static string DefaultTopicNamingStrategy(this AggregateIdentifier aggregateIdentifier)
            => $"{aggregateIdentifier.BoundedContextName.ToLowerInvariant()}-{aggregateIdentifier.CollectionName.ToLowerInvariant()}";
    }
}
