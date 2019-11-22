using LdapParserTest;
using Xunit;

namespace UnitTests
{
	public class ParserTests
	{
		[Theory]
		[ClassData(typeof(TheoryData))]
		public void ValidData_v1(string input, string expected)
		{
			Assert.Equal(expected, Parser.ParseLdapPathAndGetDomain_v1(input));
		}
		
		[Theory]
		[ClassData(typeof(TheoryData))]
		public void ValidData_v2(string input, string expected)
		{
			Assert.Equal(expected, Parser.ParseLdapPathAndGetDomain_v2(input));
		}
		
		[Theory]
		[ClassData(typeof(TheoryData))]
		public void ValidData_v3(string input, string expected)
		{
			Assert.Equal(expected, Parser.ParseLdapPathAndGetDomain_v3(input));
		}
	}
}