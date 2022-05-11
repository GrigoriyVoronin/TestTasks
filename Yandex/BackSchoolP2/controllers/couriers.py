from flask_restful import Resource
from flask import request
from models.order import Order
from models.courier import Courier
from models.delivery import Delivery
from repository import db
from utils import filter_orders_for_courier


def is_key_with_value_in_data(key: str, courier_data, type_tuple):
    if key not in courier_data or courier_data[key] is None or not isinstance(courier_data[key], type_tuple):
        return False
    return True


def is_valid_courier_type(data):
    if not is_key_with_value_in_data(Courier.type_str, data, str) or data[Courier.type_str] not in Courier.type_weight:
        return False
    return True


def is_valid_courier_regions(data):
    if not is_key_with_value_in_data(Courier.regions_str, data, list):
        return False
    regions_list = data[Courier.regions_str]
    return all(map(lambda x: isinstance(x, int) and x > 0, regions_list))


def is_valid_courier_working_hours(data):
    if not is_key_with_value_in_data(Courier.working_hours_str, data, list):
        return False
    periods_list = data[Courier.working_hours_str]
    return all(map(lambda x: isinstance(x, str), periods_list))


class CouriersController(Resource):
    def post(self):
        data = request.get_json()
        validation_result = self.validate_input_data(data)
        if not validation_result[0]:
            try:
                return {"validation_error": self.convert_couriers_ids_to_response(
                    list(map(lambda x: {"id": x[Courier.id_str]}, validation_result[1])))}, 400
            except KeyError:
                return {"validation_error": dict()}, 400
        added_couriers = list()
        for key in data:
            for courier in data[key]:
                new_courier = Courier(courier[Courier.id_str], courier[Courier.type_str],
                                      courier[Courier.regions_str], courier[Courier.working_hours_str]
                                      )

                added_couriers.append(new_courier)
        db.insert_couriers(added_couriers)

        return self.convert_couriers_ids_to_response(list(map(lambda x: x.get_json_id(), added_couriers))), 201

    @staticmethod
    def convert_couriers_ids_to_response(couriers_ids: list[dict[str, int]]):
        return {"couriers": couriers_ids}

    def validate_input_data(self, data):
        try:
            data_str = "data"
            if len(data) != 1 or data_str not in data:
                return False, list()
            invalid_couriers = list()
            for courier in data[data_str]:
                if not self.valid_courier(courier):
                    invalid_couriers.append(courier)
            return len(invalid_couriers) == 0, invalid_couriers
        except Exception:
            return False, list()

    @staticmethod
    def valid_courier(courier_data):
        if not is_key_with_value_in_data(Courier.id_str, courier_data, int) or db.is_courier_exist(
                courier_data[Courier.id_str]):
            return False
        if not is_valid_courier_type(courier_data):
            return False
        if not is_valid_courier_regions(courier_data):
            return False
        if not is_valid_courier_working_hours(courier_data):
            return False
        if len(courier_data) != 4:
            return False
        return True


class CourierController(Resource):
    def patch(self, courier_id: int):
        data = request.get_json()
        validation_result = self.validate_input_data(data, courier_id)
        if not validation_result[0]:
            if validation_result[1] == 404:
                return {"courier_id": courier_id}, 404
            return {"validation_error": {"invalid_fields": validation_result[2]}}, 400
        patched_courier: Courier = db.patch_courier(courier_id, data)
        courier_orders = db.get_active_courier_orders(courier_id)
        filtered_orders = list(map(lambda x: x.id, filter_orders_for_courier(patched_courier, courier_orders)))
        for order in courier_orders:
            if order.id not in filtered_orders:
                db.return_order(order.id)
        if len(courier_orders) > 0 and len(filtered_orders) == 0:
            delivery = db.get_delivery_by_id(courier_orders[0].delivery_id)
            delivery.is_complete = True
            db.update_delivery(delivery)

        return patched_courier.json(), 200

    def get(self, courier_id: int):
        courier = db.get_courier_by_id(courier_id)
        if courier is None:
            return {"courier_id": courier_id}, 404

        deliveries = db.get_courier_complete_deliveries(courier_id)
        if len(deliveries) == 0:
            return courier.json(), 200
        orders = db.get_orders_from_deliveries(deliveries)
        rating = self.calculate_rating(orders)
        earnings = self.calculate_earnings(deliveries)
        return courier.json_with_info(rating, earnings), 200

    @staticmethod
    def calculate_earnings(deliveries: list[Delivery]):
        earnings = 0
        for delivery in deliveries:
            earnings += 500 * Delivery.courier_type_to_rate[delivery.courier_type]
        return earnings

    @staticmethod
    def calculate_rating(orders: list[Order]):
        regions = set(map(lambda x: x.region, orders))
        min_average: float = -1
        for region in regions:
            orders_counter = 0
            time_counter = 0
            for order in orders:
                if order.region == region:
                    orders_counter += 1
                    time_counter += order.delivery_time
            average = time_counter / orders_counter
            if min_average == -1:
                min_average = average
            else:
                min_average = min(min_average, average)
        return round((60*60 - min(min_average, 60*60))/(60*60) * 5, 2)

    @staticmethod
    def validate_input_data(data, courier_id: int):
        try:
            if not db.is_courier_exist(courier_id):
                return False, 404
            invalid_fields = list()
            for key in data:
                if key not in Courier.properties_str:
                    invalid_fields.append(key)
            for key in data:
                if key == Courier.type_str and not is_valid_courier_type(data):
                    invalid_fields.append(Courier.type_str)
                if key == Courier.regions_str and not is_valid_courier_regions(data):
                    invalid_fields.append(Courier.regions_str)
                if key == Courier.working_hours_str and not is_valid_courier_working_hours(data):
                    invalid_fields.append(Courier.working_hours_str)
            if len(invalid_fields) != 0:
                return False, 400, invalid_fields
            return True, 100, invalid_fields
        except Exception:
            return False, 400, list()
