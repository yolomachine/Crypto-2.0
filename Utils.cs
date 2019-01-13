using System;
using System.Collections.Generic;
using System.Numerics;

namespace Cryptography
{
	internal static class Utils
	{ 
		public static BigInteger GeneratePrime(int bitLength = 128)
		{
			var n = BigInteger.Zero;
			do
			{
				n = RandomInteger(bitLength);
			} while (!n.IsProbablePrime());
			return n;
		}

		public static BigInteger GeneratePrimeRange(BigInteger start, BigInteger end)
		{
			var n = BigInteger.Zero;
			do
			{
				n = RandomRange(start, end);
			} while (!n.IsProbablePrime());
			return n;
		}

		public static BigInteger RandomInteger(int bitLength = 128)
		{
			if (bitLength < 1)
			{
				return BigInteger.Zero;
			}

			int bytes = bitLength / 8;
			int bits = bitLength % 8;
			using (var RNG = new System.Security.Cryptography.RNGCryptoServiceProvider())
			{
				byte[] bs = new byte[bytes + 1];
				RNG.GetBytes(bs);
				byte mask = (byte)(0xFF >> (8 - bits));
				bs[bs.Length - 1] &= mask;
				return new BigInteger(bs);
			}
		}

		public static BigInteger RandomRange(BigInteger start, BigInteger end)
		{
			var bytes = end.ToByteArray();
			var res = BigInteger.Zero;
			using (var RNG = new System.Security.Cryptography.RNGCryptoServiceProvider())
			{
				do
				{
					RNG.GetBytes(bytes);
					bytes[bytes.Length - 1] &= 0x7F;
					res = new BigInteger(bytes);
				} while (res >= end || res <= start);
			}
			return res;
		}

		public static IEnumerable<BigInteger> Factorize(BigInteger number)
		{
			for (BigInteger i = 2; i * i < number; ++i)
				if (number % i == 0)
				{
					yield return i;
					while (number % i == 0)
					{
						number /= i;
					}
				}

			if (number > 1)
			{
				yield return number;
			}
		}

		public static BigInteger GetGroupGenerator(BigInteger modulus, bool isPrime = false)
		{
			var phi = isPrime ? modulus - 1 : throw new NotImplementedException();
			var dividers = Factorize(phi);
			for (BigInteger res = 2; res <= modulus; ++res)
			{
				var isGenerator = true;
				foreach (var divider in dividers)
				{
					isGenerator &= BigInteger.ModPow(res, phi / divider, modulus) != 1;
					if (!isGenerator)
						break;
				}

				if (isGenerator)
					return res;
			}
			throw new Exception("Group generator not found.");
		}
	}
}
