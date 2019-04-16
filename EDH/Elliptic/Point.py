from Cryptography import Utils


class Point:
    def __init__(self, x, y, curve):
        self.x = x
        self.y = y
        self.curve = curve

    def __mul__(self, other):
        if not (isinstance(other, int) or isinstance(other, float)):
            return self

        if other % self.curve.n == 0 or self is None:
            return None

        if other < 0:
            return Point(self.x, -self.y % self.curve.p, self.curve) * (-other)

        result = None
        addend = self

        while other:
            if other & 1:
                result = addend + result
            addend = addend + addend

            other >>= 1

        return result

    def __add__(self, other):
        if not (isinstance(other, Point)):
            return self

        x1, y1 = self.x, self.y
        x2, y2 = other.x, other.y

        if x1 == x2 and y1 != y2:
            return None

        if x1 == x2:
            m = (3 * x1 * x1 + self.curve.a) * Utils.invert_modular(2 * y1, self.curve.p)
        else:
            m = (y1 - y2) * Utils.invert_modular(x1 - x2, self.curve.p)

        x3 = m * m - x1 - x2
        y3 = y1 + m * (x3 - x1)
        return Point(x3 % self.curve.p, -y3 % self.curve.p, self.curve)

    def __neg__(self):
        if self is None:
            return None
        return Point(self.x, -self.y % self.curve.p, self.curve)

