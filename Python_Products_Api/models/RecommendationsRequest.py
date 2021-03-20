class RecommendationsRequest:

    def __init__(self, product_id: str, min_rank: float):
        self.__product_id = product_id
        self.__min_rank = min_rank

    @property
    def get_product_id(self):
        return self.__product_id

    @property
    def get_min_rank(self):
        return self.__min_rank
