using Xunit;

namespace UnitTests
{
	public class TheoryData: TheoryData<string, string>
	{
		public TheoryData()
		{
			Add("ldap://ldap.example.com/cn=John%20Doe,dc=example,dc=com", "example.com");
			Add("LDAP://ldap.example.com/CN=John%20Doe,DC=example,DC=com", "example.com");
			Add("ldap:///dc=example,dc=com??sub?(givenName=John)", "example.com");
			Add("ldap://ds.example.com:389/dc=child,dc=example,dc=com", "child.example.com");
			Add("ldap://ds.example.com:389/dc=example,dc=com?givenName,sn,cn?sub?(uid=john.doe)", "example.com");
			Add("ldap://ds.example.com:389/dc=example,dc=com", "example.com");
			Add("ldap://[2001:db8::7]/c=GB?objectClass?one", "");
		}
	}
}