using System;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Cryptography
{
	public static class RSA
	{
		public class KeyBinding
		{
			internal BigInteger n;
			internal BigInteger e;
			internal BigInteger d;

			public KeyBinding()
			{
				Generate();
			}

			public (BigInteger, BigInteger, BigInteger) Generate()
			{
				var p = Utils.GeneratePrime();
				var q = Utils.GeneratePrime();
				n = p * q;
				var phi = (p - 1) * (q - 1);
				e = Utils.RandomRange(2, phi);
				while (BigInteger.GreatestCommonDivisor(e, phi) != 1)
				{
					e = Utils.RandomRange(2, phi);
				}

				d = e.ModInverse(phi, isCoPrime: true);
				return (n, e, d);
			}

			public override string ToString() => $"n: {n}\ne: {e}\nd: {d}";
		}

		public static KeyBinding Keys { get; private set; } = new KeyBinding();

		public static BinaryReader Reader {get;set;}
		public static BinaryWriter Writer { get; set; }

		public static void Encrypt()
		{
			var len = Reader.BaseStream.Length;
			var pos = Reader.BaseStream.Position;
			var zero = new byte[] { 0x00 };
			Writer.Write(Extend(Keys.d.ToByteArray()).Reverse().ToArray());
			Writer.Write(Extend(Keys.n.ToByteArray()).Reverse().ToArray());
			while (pos < len)
			{
				var bytes = Reader.ReadBytes(8);
				if (bytes.Length < 8)
				{
					bytes = Extend(bytes, 8, (byte)Math.Abs(8 - bytes.Length)).ToArray();
				}
				bytes = bytes.Reverse().Concat(zero).ToArray();
				var m = new BigInteger(bytes);
				bytes = BigInteger.ModPow(m, Keys.e, Keys.n).ToByteArray();
				if (bytes.LongLength < 32)
					bytes = Extend(bytes);
				Writer.Write(bytes.Reverse().ToArray());
				pos += 8;
			}

			if (len % 8 == 0)
			{
				var m = new BigInteger(Enumerable.Repeat((byte)0x08, 8).Concat(zero).ToArray());
				var bytes = BigInteger.ModPow(m, Keys.e, Keys.n).ToByteArray();
				if (bytes.LongLength < 32)
					bytes = Extend(bytes);
				Writer.Write(bytes.Reverse().ToArray());
			}
			Writer.Close();
			Reader.Close();
		}

		public static void Decrypt()
		{
			var len = Reader.BaseStream.Length;
			var pos = Reader.BaseStream.Position;
			var zero = new byte[] { 0x00 };
			byte[] bytes;
			BigInteger c;
			while (pos < len - 32)
			{
				bytes = Reader.ReadBytes(32).Reverse().ToArray().Extend();
				c = new BigInteger(bytes);
				bytes = BigInteger.ModPow(c, Keys.d, Keys.n).ToByteArray();
				bytes = bytes.Length > 8 ? bytes.Take(8).ToArray() : bytes;
				bytes = bytes.Extend(times: 8 - bytes.Length).Reverse().ToArray();
				Writer.Write(bytes);
				pos += 32;
			}

			bytes = Reader.ReadBytes(32).Reverse().ToArray().Extend();
			c = new BigInteger(bytes);
			bytes = BigInteger.ModPow(c, Keys.d, Keys.n).ToByteArray();
			bytes = bytes.Length > 8 ? bytes.Take(8).ToArray() : bytes;
			bytes = bytes.Extend(times: 8 - bytes.Length).Reverse().ToArray();
			if (bytes[7] != 0x08)
			{
				Writer.Write(bytes, 0, 8 - bytes[7]);
			}
			Writer.Close();
			Reader.Close();
		}

		private static byte[] Extend(byte[] bytes, int to = 32, byte with = 0x00)
		{
			return bytes.Concat(Enumerable.Repeat(with, to - bytes.Length)).ToArray();
		}
	}
}
