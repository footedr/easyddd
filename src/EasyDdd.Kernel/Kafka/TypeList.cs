using System.Reflection;

namespace EasyDdd.Kernel.Kafka
{
    public class TypeList<T> : List<Type>
    {
        public void AddImplementationsFromAssemblyOf<TMarker>()
        {
            var implementations = typeof(TMarker).Assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(T)));
            AddRange(implementations);
        }

        public void AddImplementationsFromAssembly(Assembly assembly)
        {
            var implementations = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(T)));
            AddRange(implementations);
        }

        public void AddImplementationsFromAllAssemblies()
        {
            var implementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsAssignableTo(typeof(T)));

            AddRange(implementations);
        }
    }
}
