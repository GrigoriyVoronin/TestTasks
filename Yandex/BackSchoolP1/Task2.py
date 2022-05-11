class AbstractCat:
    weight = 0
    remainder = 0

    def eat(self, count):
        self.weight += count // 10
        self.remainder += count % 10
        if self.remainder >= 10:
            self.weight += self.remainder // 10
            self.remainder = self.remainder % 10
        if self.weight > 100:
            self.weight = 100

    def __str__(self):
        return f"{type(self).__name__} ({self.weight})"


class Kitten(AbstractCat):
    def __init__(self, weight):
        self.weight = weight

    def meow(self):
        return "meow..."

    def sleep(self):
        answer = ""
        for i in range(self.weight // 5):
            answer += "Snore"
        return answer


class Cat(Kitten):
    name: str

    def __init__(self, weight, name):
        super().__init__(weight)
        self.name = name

    def meow(self):
        return "MEOW..."

    def get_name(self):
        return self.name

    @staticmethod
    def catch_mice():
        return "Got it!"
