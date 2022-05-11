import http.server
import socketserver
from ProductsService import ProductsService


class MyHandler(http.server.SimpleHTTPRequestHandler):
    PRODUCT_ID_PARAMETER = "product_id"
    MIN_RANK_PARAMETER = "min_rank"
    FILE_NAME = "response.json"
    __products_service = ProductsService()

    def do_GET(self):
        try:
            parameters = self.__parse_request_path(self.path[1:])
            recommendations_ids = self.get_products_ids(parameters)
            self.create_response(recommendations_ids)
        except IndexError:
            self.create_response(None)
        except ValueError:
            self.create_response(None)
        http.server.SimpleHTTPRequestHandler.do_GET(self)

    def create_response(self, recommendations: set[str]):
        if recommendations is None:
            self.send_response(400, "Bad Request")
            self.write_to_file("Bad Request")
        elif len(recommendations) > 0:
            self.send_response(200)
            self.write_to_file(str(recommendations))
        else:
            self.send_response(404, "Not Found")
            self.write_to_file("Not Found")
        self.path = "/" + self.FILE_NAME

    def write_to_file(self, data: str):
        with open(self.FILE_NAME, "w") as file:
            file.write(data)

    def get_products_ids(self, parameters: dict[str, str]):
        if self.PRODUCT_ID_PARAMETER in parameters:
            product_id = parameters[self.PRODUCT_ID_PARAMETER]
        else:
            return None
        if self.MIN_RANK_PARAMETER in parameters:
            min_rank = parameters[self.MIN_RANK_PARAMETER]
            return self.__products_service.get_recommended_products_ids_with_min_rank(product_id, float(min_rank))
        return self.__products_service.get_recommended_products_ids(product_id)

    @staticmethod
    def __parse_request_path(path: str):
        parameters: dict[str, str] = dict()
        parameters_names_values = path.split('&')
        for name_value in parameters_names_values:
            name_value_arr = name_value.split('=')
            parameters[name_value_arr[0]] = name_value_arr[1]
        return parameters


PORT = 8000

handler = MyHandler

myServer = socketserver.TCPServer(("", PORT), handler)

myServer.serve_forever()
