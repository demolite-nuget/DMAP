using DMAP.Net.Implementation;

namespace DMAP.Net.Tests.Setup.Simple;

public class SimpleTestMapper : AbstractDMapper<SimpleSourceClass, SimpleTargetClass>
{
	public SimpleTestMapper()
	{
		MapProperty(src => src.TestString, tgt => tgt.TestString);
		MapProperty(src => src.TestInt, tgt => tgt.TestInt);
		MapProperty(src => src.TestDouble, tgt => tgt.TestDouble);
		MapProperty(src => src.TestDecimal, tgt => tgt.TestDecimal);
	}
}