from Elliptic.Curve import Curve
from Elliptic.Point import Point
from Cryptography.DH import DH
import time

if __name__ == "__main__":
    curve = Curve(
        0,
        7,
        1,
        0xfffffffffffffffffffffffffffffffffffffffffffffffffffffffefffffc2f,
        0xfffffffffffffffffffffffffffffffebaaedce6af48a03bbfd25e8cd0364141,
    )

    point = Point(
        0x79be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798,
        0x483ada7726a3c4655da4fbfc0e1108a8fd17b448a68554199c47d08ffb10d4b8,
        curve
    )

    dh = DH(point)

    v = time.time()
    t = time.time()

    private, public = dh.gen_keys()
    print("Keys:")
    print("Private:", hex(private))
    print("Public: (0x{:x}, 0x{:x})".format(public.x, public.y))
    print("Time gen: ", time.time() - t)

    t = time.time()
    msg = b'Hello world'
    signature = dh.sign_message(private, msg)
    print("Time sign: ", time.time() - t)

    t = time.time()
    print()
    print('Message:', msg)
    print('Signature: (0x{:x}, 0x{:x})'.format(signature[0], signature[1]))
    print(dh.check_signature(public, msg, signature))
    print("Time check: ", time.time() - t)

    t = time.time()
    msg = b'Hello w0rld'
    print()
    print('Message:', msg)
    print(dh.check_signature(public, msg, signature))
    print("Time check false: ", time.time() - t)

    print("Time all: ", time.time() - v)
