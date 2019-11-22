using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LdapParserTest
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			var stopwatch = new Stopwatch();

			const int iterations = 100000;
			const string input =
				"ldap://ds.example.com:389/dc=child,dc=example,dc=com?givenName,sn,cn?sub?(uid=john.doe)";


			stopwatch.Start();
			//for (var i = 0; i < iterations; i++)
			{
				Parser.ParseLdapPathAndGetDomain_v1(input);
			}

			stopwatch.Stop();

			Console.WriteLine($"Elapsed: {stopwatch.Elapsed.TotalMilliseconds} ms");
		}
	}
}