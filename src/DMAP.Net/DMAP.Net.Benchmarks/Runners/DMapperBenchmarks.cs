using BenchmarkDotNet.Attributes;
using DMAP.Net.Interfaces;
using DMAP.Net.Tests.Setup;
using DMAP.Net.Tests.Setup.Simple;

namespace DMAP.Net.Benchmarks.Runners;

public class DMapperBenchmarks
{
	private readonly IDMapper<SimpleSourceClass, SimpleTargetClass> _mapper = new SimpleTestMapper();
	
	private readonly SimpleSourceClass _simpleSource = new ();
	private readonly SimpleTargetClass _simpleTarget = new ();

	[Benchmark]
	public void GeneralMapBenchmark()
	{
		_mapper.Map(_simpleSource, _simpleTarget);
	}
	
}