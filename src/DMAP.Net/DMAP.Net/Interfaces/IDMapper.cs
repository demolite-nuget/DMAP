namespace DMAP.Net.Interfaces;

public interface IDMapper<in T1, in T2>
{
	public void Map(T1 source, T2 destination);

	public void Map(T2 source, T1 destination);
}