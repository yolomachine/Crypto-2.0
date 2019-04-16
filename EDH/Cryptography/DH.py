import hashlib
import random
from Cryptography import Utils


class DH:
    def __init__(self, point):
        self.point = point

    def gen_keys(self):
        private_key = random.randrange(1, self.point.curve.n)
        public_key = self.point * private_key
        return private_key, public_key

    def get_hash(self, message):
        message_hash = hashlib.sha512(message).digest()
        e = int.from_bytes(message_hash, 'big')
        z = e >> (e.bit_length() - self.point.curve.n.bit_length())
        return z

    def sign_message(self, private_key, message):
        z = self.get_hash(message)
        r = 0
        s = 0
        while not r or not s:
            k = random.randrange(1, self.point.curve.n)
            point = self.point * k

            x, y = point.x, point.y

            r = x % self.point.curve.n
            s = ((z + r * private_key) * Utils.invert_modular(k, self.point.curve.n)) % self.point.curve.n
        return r, s

    def check_signature(self, public_key, message, signature):
        z = self.get_hash(message)
        print(z)
        r, s = signature

        w = Utils.invert_modular(s, self.point.curve.n)
        u1 = (z * w) % self.point.curve.n
        u2 = (r * w) % self.point.curve.n

        print((self.point * u1).x)
        point = (self.point * u1) + (public_key * u2)

        if (r % self.point.curve.n) == (point.x % self.point.curve.n):
            return '[Signature Valid]'
        else:
            return '[Signature Invalid]'
