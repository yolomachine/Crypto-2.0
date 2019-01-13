using System.Numerics;
using System.Security.Cryptography;
using System.Linq;

namespace Cryptography
{
	public static class ByteExtensions
	{
		public static byte[] Extend(this byte[] bytes, byte with = 0x00, int times = 1)
		{
			return bytes.Concat(Enumerable.Repeat(with, times)).ToArray();
		}

		public static byte[] ExtendTo(this byte[] bytes, int to = 32, byte with = 0x00)
		{
			return bytes.Concat(Enumerable.Repeat(with, to - bytes.Length)).ToArray();
		}

		public static byte[] Trim(this byte[] bytes)
		{
			var zeros = 0;
			for (var i = bytes.Length - 1; i >= 0; --i)
			{
				if (bytes[i] != 0x00)
				{
					break;
				}
				++zeros;
			}

			return bytes.Take(bytes.Length - zeros).ToArray();
		}

		public static byte[] Xor(this byte[] a, byte[] b)
		{
			var c = new byte[a.Length];
			int i = 0;
			int j = 0;
			while (i < a.Length)
			{
				c[i] = (byte)(a[i] ^ b[j]);
				++i;
				j = (j + 1) % b.Length;
			}

			return c;
		}

	}

	public static class BigIntegerExtensions
	{
		public static bool IsProbablePrime(this BigInteger source, int witnesses = 100)
		{
			if (source == 2 || source == 3)
			{
				return true;
			}
			if (source < 2 || source % 2 == 0)
			{
				return false;
			}

			var d = source - 1;
			var s = 0;

			while (d % 2 == 0)
			{
				d /= 2;
				s += 1;
			}

			var rng = RandomNumberGenerator.Create();
			var bytes = new byte[source.ToByteArray().LongLength];
			var a = BigInteger.Zero;

			for (var i = 0; i < witnesses; ++i)
			{
				do
				{
					rng.GetBytes(bytes);
					a = new BigInteger(bytes);
				} while (a < 2 || a >= source - 2);

				var x = BigInteger.ModPow(a, d, source);
				if (x == 1 || x == source - 1)
				{
					continue;
				}

				for (var r = 1; r < s; r++)
				{
					x = BigInteger.ModPow(x, 2, source);
					if (x == 1)
					{
						return false;
					}
					if (x == source - 1)
					{
						break;
					}
				}

				if (x != source - 1)
				{
					return false;
				}
			}

			return true;
		}

		public static BigInteger ModInverse(this BigInteger a, BigInteger m, bool isModulusPrime = false, bool isCoPrime = false)
		{
			if (isModulusPrime)
			{
				return BigInteger.ModPow(a, m - 2, m);
			}

			if (isCoPrime)
			{
				var m0 = m;
				BigInteger y = 0, x = 1;

				if (m == 1)
				{
					return 0;
				}

				while (a > 1)
				{
					var q = a / m;
					var t = m;
					m = a % m;
					a = t;
					t = y;

					y = x - q * y;
					x = t;
				}

				if (x < 0)
				{
					x += m0;
				}

				return x;
			}

			a = a % m;
			for (BigInteger x = 1; x < m; ++x)
			{
				if ((a * x) % m == 1)
				{
					return x;
				}
			}

			return 1;
		}

		public static BigInteger EuclidianMod(this BigInteger a, BigInteger m)
		{
			var r = a % m;
			if (r < 0)
			{
				r += BigInteger.Abs(m);
			}
			return r;
		}

		public static int Length(this BigInteger a)
		{
			return a.ToString().Length;
		}
	}
}
