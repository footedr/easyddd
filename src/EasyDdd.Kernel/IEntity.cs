namespace EasyDdd.Kernel
{
	public interface IEntity<TId>
	{
		TId Identifier { get; }
	}
}