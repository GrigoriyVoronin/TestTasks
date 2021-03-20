class InputLine:

    def __init__(self, input_line: list[str]):
        self.__product_id = input_line[0]
        self.__recommendation_id = input_line[1]
        self.__rank = float(input_line[2])

    @property
    def product_id(self):
        return self.__product_id

    @property
    def recommendation_id(self):
        return self.__recommendation_id

    @property
    def rank(self):
        return self.__rank
