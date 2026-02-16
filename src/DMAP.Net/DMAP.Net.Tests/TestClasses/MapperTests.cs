using DMAP.Net.Interfaces;
using DMAP.Net.Tests.Setup.Simple;

namespace DMAP.Net.Tests.TestClasses;

[TestClass]
public sealed class MapperTests
{
	private readonly IDMapper<SimpleSourceClass, SimpleTargetClass> _mapper = new SimpleTestMapper();
	
	[TestMethod]
	public void Map_SimplePropertiesMapped()
	{
		var input = new SimpleSourceClass();
		var output = new SimpleTargetClass();
		
		_mapper.Map(input, output);
		
		Assert.AreEqual(input.TestString, output.TestString);
	}
}