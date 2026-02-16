namespace DMAP.Net.Tests.Setup.Simple;

public class SimpleSourceClass : SimpleTypesClass
{
	public SimpleSourceClass()
	{
		TestString = "Hello World";
		TestDouble = 3.14;
		TestDecimal = 2.7m;
		TestInt = 1337;
	}
}