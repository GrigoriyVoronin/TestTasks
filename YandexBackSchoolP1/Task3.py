class PearsBasket:
    count: int

    def __init__(self, pears_count):
        self.count = pears_count

    def __floordiv__(self, children_count):
        baskets_size = self.count // children_count
        baskets = list()
        for i in range(children_count):
            baskets.append(PearsBasket(baskets_size))
        remainder = self.count % children_count
        if remainder > 0:
            baskets.append(PearsBasket(remainder))
        return baskets

    def __mod__(self, count: int):
        return self.count % count

    def __add__(self, other):
        return PearsBasket(self.count + other.count)

    def __sub__(self, count):
        self.count -= count
        if self.count < 0:
            self.count = 0

    def __str__(self):
        return str(self.count)

    def __repr__(self):
        return f"PearsBasket({self.count})"
