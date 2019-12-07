using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LdapParserTest
{
	public static class Parser
	{
		public static string ParseLdapPathAndGetDomain_v1(string path)
		{
			const string dc = "DC=";
			const string dcLower = "dc=";
			var parts = path.Split('/')
				.SelectMany(s => s.Split(','))
				.SelectMany(s => s.Split('?'))
				.SelectMany(s => s.Split('#'))
				.SelectMany(s => s.Split(':'))
				.SelectMany(s => s.Split('('))
				.SelectMany(s => s.Split(')'))
				.Where(x => x.StartsWith(dc, StringComparison.InvariantCultureIgnoreCase))
				.Select(x => x.Replace(dc, string.Empty).Replace(dcLower, string.Empty))
				.ToList();
			return string.Join(".", parts);
		}

		public static string ParseLdapPathAndGetDomain_v2(string path)
		{
			var parts = new List<string>();

			var totalLength = path.Length;

			var i = 0;
			var start = 0;
			var dcFound = false;
			while (i < totalLength)
			{
				switch (path[i])
				{
					case 'D':
					case 'd':
						if (i + 3 < totalLength) // we need 4 chars for at least 'DC=x'
						{
							if ((path[i + 1] == 'C' || path[i + 1] == 'c') && path[i + 2] == '=')
							{
								dcFound = true;
								i += 3;
								start = i;
								continue;
							}
						}
						break;
					case ',':
					case '?':
					case '/':
					case ':':
					case '#':
					case '(':
					case ')':
						if (start < i - 1)
						{
							if (dcFound)
							{
								parts.Add(path.Substring(start, i - start));
								dcFound = false;
							}
						}

						break;
				}

				i++;
			}

			if (dcFound)
			{
				parts.Add(path.Substring(start, i - start));
			}

			return string.Join(".", parts.ToArray());
		}


		public static string ParseLdapPathAndGetDomain_v3(string path)
		{
			var strBuilder = new StringBuilder();

			var totalLength = path.Length;

			var i = 0;
			var start = 0;
			var dcFound = false;
			while (i < totalLength)
			{
				switch (path[i])
				{
					case 'D':
					case 'd':
						if (i + 3 < totalLength) // we need 4 chars for at least 'DC=x'
						{
							if ((path[i + 1] == 'C' || path[i + 1] == 'c') && path[i + 2] == '=')
							{
								dcFound = true;
								i += 3;
								start = i;
								continue;
							}
						}
						break;
					case ',':
					case '?':
					case '/':
					case ':':
					case '#':
					case '(':
					case ')':
						if (start < i - 1)
						{
							if (dcFound)
							{
								strBuilder.Append(path.Substring(start, i - start));
								strBuilder.Append('.');
								dcFound = false;
							}
						}

						break;
				}

				i++;
			}

			if (dcFound)
			{
				strBuilder.Append(path.Substring(start, i - start));
			}
			else if (strBuilder.Length > 0 && strBuilder[strBuilder.Length - 1] == '.')
			{
				strBuilder.Remove(strBuilder.Length - 1, 1);
			}

			return strBuilder.ToString();
		}

		public static string ParseLdapPathAndGetDomain_v4(string path)
		{
			var strBuilder = new StringBuilder();

			var inputSpan = path.AsSpan();

			var totalLength = path.Length;

			var i = 0;
			var start = 0;
			var dcFound = false;
			while (i < totalLength)
			{
				switch (inputSpan[i])
				{
					case 'D':
					case 'd':
						if (i + 3 < totalLength) // we need 4 chars for at least 'DC=x'
						{
							if ((inputSpan[i + 1] == 'C' || inputSpan[i + 1] == 'c') && inputSpan[i + 2] == '=')
							{
								dcFound = true;
								i += 3;
								start = i;
								continue;
							}
						}
						break;
					case ',':
					case '?':
					case '/':
					case ':':
					case '#':
					case '(':
					case ')':
						if (start < i - 1)
						{
							if (dcFound)
							{
								strBuilder.Append(inputSpan.Slice(start, i - start));
								strBuilder.Append('.');
								dcFound = false;
							}
						}

						break;
				}

				i++;
			}

			if (dcFound)
			{
				strBuilder.Append(inputSpan.Slice(start, i - start));
			}
			else if (strBuilder.Length > 0 && strBuilder[strBuilder.Length - 1] == '.')
			{
				strBuilder.Remove(strBuilder.Length - 1, 1);
			}

			return strBuilder.ToString();
		}

		public static string ParseLdapPathAndGetDomain_v5(string path)
		{
			static void Append(char[] target, ReadOnlySpan<char> spanToAppend, int at)
			{
				for (int i = at, j = 0; j < spanToAppend.Length; ++i, ++j)
				{
					target[i] = spanToAppend[j];
				}
			}

			var strBuffer = new char[path.Length];

			var inputSpan = path.AsSpan();

			var totalLength = path.Length;

			var bufferC = 0;
			var i = 0;
			var start = 0;
			var dcFound = false;
			while (i < totalLength)
			{
				switch (inputSpan[i])
				{
					case 'D':
					case 'd':
						if (i + 3 < totalLength) // we need 4 chars for at least 'DC=x'
						{
							if ((inputSpan[i + 1] == 'C' || inputSpan[i + 1] == 'c') && inputSpan[i + 2] == '=')
							{
								dcFound = true;
								i += 3;
								start = i;
								continue;
							}
						}
						break;
					case ',':
					case '?':
					case '/':
					case ':':
					case '#':
					case '(':
					case ')':
						if (start < i - 1)
						{
							if (dcFound)
							{
								var segment = inputSpan.Slice(start, i - start);
								Append(strBuffer, segment, bufferC);
								bufferC += segment.Length;
								strBuffer[bufferC++] = '.';
								dcFound = false;
							}
						}

						break;
				}

				i++;
			}

			if (dcFound)
			{
				var segment = inputSpan.Slice(start, i - start);
				Append(strBuffer, segment, bufferC);
				bufferC += segment.Length;
			}
			else if (bufferC > 0 && strBuffer[bufferC - 1] == '.')
			{
				--bufferC;
			}

			return new string(strBuffer.AsSpan(0, bufferC));
		}

		public static string ParseLdapPathAndGetDomain_v6(string path)
		{
			static void Append(char[] target, ReadOnlySpan<char> spanToAppend, int at)
			{
				for (int i = at, j = 0; j < spanToAppend.Length; ++i, ++j)
				{
					target[i] = spanToAppend[j];
				}
			}

			var strBuffer = ArrayPool<char>.Shared.Rent(path.Length);
			try
			{
				var inputSpan = path.AsSpan();

				var totalLength = path.Length;

				var bufferC = 0;
				var i = 0;
				var start = 0;
				var dcFound = false;
				while (i < totalLength)
				{
					switch (inputSpan[i])
					{
						case 'D':
						case 'd':
							if (i + 3 < totalLength) // we need 4 chars for at least 'DC=x'
							{
								if ((inputSpan[i + 1] == 'C' || inputSpan[i + 1] == 'c') && inputSpan[i + 2] == '=')
								{
									dcFound = true;
									i += 3;
									start = i;
									continue;
								}
							}
							break;
						case ',':
						case '?':
						case '/':
						case ':':
						case '#':
						case '(':
						case ')':
							if (start < i - 1)
							{
								if (dcFound)
								{
									var segment = inputSpan.Slice(start, i - start);
									Append(strBuffer, segment, bufferC);
									bufferC += segment.Length;
									strBuffer[bufferC++] = '.';
									dcFound = false;
								}
							}

							break;
					}

					i++;
				}

				if (dcFound)
				{
					var segment = inputSpan.Slice(start, i - start);
					Append(strBuffer, segment, bufferC);
					bufferC += segment.Length;
				}
				else if (bufferC > 0 && strBuffer[bufferC - 1] == '.')
				{
					--bufferC;
				}

				return new string(strBuffer.AsSpan(0, bufferC));
			}
			finally
			{
				ArrayPool<char>.Shared.Return(strBuffer);
			}
		}

		public static string ParseLdapPathAndGetDomain_v7(string path)
		{
			static void Append(char[] target, ReadOnlySpan<char> spanToAppend, int at)
			{
				if (spanToAppend.Length % 2 == 0)
				{
					for (int i = at, j = 0; j < spanToAppend.Length;)
					{
						target[i++] = spanToAppend[j++];
						target[i++] = spanToAppend[j++];
					}
				}
				else
				{
					for (int i = at, j = 0; j < spanToAppend.Length;)
					{
						target[i++] = spanToAppend[j++];
					}
				}
			}

			var strBuffer = ArrayPool<char>.Shared.Rent(path.Length);
			try
			{
				var inputSpan = path.AsSpan();

				var totalLength = path.Length;

				var bufferC = 0;
				var i = 0;
				var start = 0;
				var dcFound = false;
				while (i < totalLength)
				{
					switch (inputSpan[i])
					{
						case 'D':
						case 'd':
							if (i + 3 < totalLength) // we need 4 chars for at least 'DC=x'
							{
								if ((inputSpan[i + 1] == 'C' || inputSpan[i + 1] == 'c') && inputSpan[i + 2] == '=')
								{
									dcFound = true;
									i += 3;
									start = i;
									continue;
								}
							}
							break;
						case ',':
						case '?':
						case '/':
						case ':':
						case '#':
						case '(':
						case ')':
							if (start < i - 1)
							{
								if (dcFound)
								{
									var segment = inputSpan.Slice(start, i - start);
									Append(strBuffer, segment, bufferC);
									bufferC += segment.Length;
									strBuffer[bufferC++] = '.';
									dcFound = false;
								}
							}

							break;
					}

					i++;
				}

				if (dcFound)
				{
					var segment = inputSpan.Slice(start, i - start);
					Append(strBuffer, segment, bufferC);
					bufferC += segment.Length;
				}
				else if (bufferC > 0 && strBuffer[bufferC - 1] == '.')
				{
					--bufferC;
				}

				return new string(strBuffer.AsSpan(0, bufferC));
			}
			finally
			{
				ArrayPool<char>.Shared.Return(strBuffer);
			}
		}
	}
}