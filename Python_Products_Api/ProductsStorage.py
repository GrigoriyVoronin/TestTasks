from typing import Generator, Any
from models.InputLine import InputLine


class ProductsStorage:
    __PRODUCTS_COUNT = 1_200_100
    __products: list[str] = list()
    __products_int_ids: dict[str, int] = dict()
    products_recommendations: dict[int, dict[float, list[int]]] = dict()

    def __init__(self, input_lines: Generator[InputLine, Any, None]):
        for line in input_lines:
            self.__add_line_to_storage(line)

    def get_product_str_id(self, int_id: int):
        return self.__products[int_id]

    def get_product_int_id(self, str_id: str):
        if str_id in self.__products_int_ids:
            return self.__products_int_ids[str_id]

        return -1

    def __add_line_to_storage(self, input_line: InputLine):
        product_int_id = self.__init_product(input_line.product_id)
        recommendation_int_id = self.__init_product(input_line.recommendation_id)
        if product_int_id not in self.products_recommendations:
            self.products_recommendations[product_int_id] = dict()
        if input_line.rank not in self.products_recommendations[product_int_id]:
            self.products_recommendations[product_int_id][input_line.rank] = list()
        self.products_recommendations[product_int_id][input_line.rank].append(recommendation_int_id)

    def __init_product(self, product_id: str):
        if product_id in self.__products_int_ids:
            return self.__products_int_ids[product_id]
        product_index = len(self.__products)
        self.__products_int_ids[product_id] = product_index
        self.__products.append(product_id)
        return product_index
