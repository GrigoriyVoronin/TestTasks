import DataParser
from ProductsStorage import ProductsStorage


class ProductsService:

    __products_storage: ProductsStorage

    def __init__(self):
        input_lines = DataParser.parse_csv_file()
        self.__products_storage = ProductsStorage(input_lines)

    def get_recommended_products_ids(self, product_id: str):
        int_id = self.__products_storage.get_product_int_id(product_id)
        if int_id == -1:
            return list()

        result: list[str] = list()
        recommendations = self.__products_storage.products_recommendations[int_id]
        for recommendationsList in recommendations.values():
            for recommendation in recommendationsList:
                str_id = self.__products_storage.get_product_str_id(recommendation)
                result.append(str_id)
        return result

    def get_recommended_products_ids_with_min_rank(self, product_id: str, min_rank: float):
        int_id = self.__products_storage.get_product_int_id(product_id)
        if int_id == -1:
            return list()

        result: list[str] = list()
        recommendations = self.__products_storage.products_recommendations[int_id]
        for recommendation_rank in recommendations:
            if recommendation_rank >= min_rank:
                for recommendation in recommendations[recommendation_rank]:
                    str_id = self.__products_storage.get_product_str_id(recommendation)
                    result.append(str_id)
        return result
