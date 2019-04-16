from Elliptic.Point import Point


class Curve:
    def __init__(self, a, b, h, p, n):
        self.a = a
        self.b = b
        self.h = h
        self.p = p
        self.n = n

    def __contains__(self, point):
        if point is not Point:
            return False
        elif point is None:
            return True

        x, y = point.x, point.y
        return (y * y - x * x * x - self.a * x - self.b) % self.p == 0

