from isodate import isodatetime
from flask_restful import Resource
from flask import request
from models.order import Order
from models.courier import Courier
from repository import db
from utils import date_to_iso_str, filter_orders_for_courier


def is_key_with_value_in_data(key: str, order_data, item_type):
    if key not in order_data or order_data[key] is None or not isinstance(order_data[key], item_type):
        return False
    return True


def is_valid_order_weight(data):
    if not is_key_with_value_in_data(Order.weight_str, data, (float, int)):
        return False
    weight = data[Order.weight_str]
    if weight < 0.001 or weight > 50:
        return False
    return True


def is_valid_order_region(data):
    if not is_key_with_value_in_data(Order.region_str, data, int):
        return False
    if not data[Order.region_str] > 0:
        return False
    return True


def is_valid_order_delivery_hours(data):
    if not is_key_with_value_in_data(Order.delivery_hours_str, data, list):
        return False
    periods_list = data[Order.delivery_hours_str]
    return all(map(lambda x: isinstance(x, str), periods_list))


class OrdersController(Resource):
    def post(self):
        data = request.get_json()
        validation_result = self.validate_input_data(data)
        if not validation_result[0]:
            try:
                return {"validation_error": self.convert_orders_ids_to_response(
                    list(map(lambda x: {"id": x[Order.id_str]}, validation_result[1])))}, 400
            except KeyError:
                return {"validation_error": dict()}, 400
        added_orders = list()
        for key in data:
            for order in data[key]:
                new_order = Order(order[Order.id_str], order[Order.weight_str],
                                  order[Order.region_str], order[Order.delivery_hours_str]
                                  )
                added_orders.append(new_order)
        db.insert_orders(added_orders)

        return self.convert_orders_ids_to_response(list(map(lambda x: x.get_json_id(), added_orders))), 201

    @staticmethod
    def convert_orders_ids_to_response(orders_ids: list[dict[str, int]]):
        return {"orders": orders_ids}

    def validate_input_data(self, data):
        try:
            data_str = "data"
            if len(data) != 1 or data_str not in data:
                return False, list()
            invalid_orders = list()
            for order in data[data_str]:
                if not self.valid_order(order):
                    invalid_orders.append(order)
            return len(invalid_orders) == 0, invalid_orders
        except Exception:
            return False, list()

    @staticmethod
    def valid_order(order_data):
        if not is_key_with_value_in_data(Order.id_str, order_data, int) or db.is_order_exist(
                order_data[Order.id_str]):
            return False
        if not is_valid_order_weight(order_data):
            return False
        if not is_valid_order_region(order_data):
            return False
        if not is_valid_order_delivery_hours(order_data):
            return False
        if len(order_data) != 4:
            return False
        return True


class OrdersAssignController(Resource):
    def post(self):
        data = request.get_json()
        validation_result = self.validate_input_data(data)
        if not validation_result[0]:
            return {"validation_error": validation_result[1]}, 400
        courier_id = data[Courier.id_str]
        courier_orders = db.get_active_courier_orders(courier_id)
        if len(courier_orders) > 0:
            return self.create_response(courier_orders)
        issued_orders = self.get_issued_orders_for_courier(courier_id)
        return self.create_response(issued_orders)

    @staticmethod
    def get_issued_orders_for_courier(courier_id: int):
        courier = db.get_courier_by_id(courier_id)
        courier_weight = Courier.type_weight[courier.type]
        free_orders = db.get_free_orders_ordered_by_weight(courier_weight)
        issued_orders = filter_orders_for_courier(courier, free_orders)
        if len(issued_orders) == 0:
            return issued_orders

        update_issued_orders = db.issue_orders(issued_orders, courier)
        return update_issued_orders

    @staticmethod
    def create_response(courier_orders: list[Order]):
        ids_list = list(map(lambda x: x.get_json_id(), courier_orders))
        if len(courier_orders) > 0:
            assign_time = db.get_assign_time(courier_orders[0])
            return {
                "orders": ids_list,
                Order.assign_time_str: date_to_iso_str(assign_time)
            }, 200
        return {
            "orders": ids_list
        }, 200

    @staticmethod
    def validate_input_data(data):
        try:
            if len(data) != 1 or Courier.id_str not in data:
                return False, dict()
            if not is_key_with_value_in_data(Courier.id_str, data, int):
                return False, dict()
            if not db.is_courier_exist(data[Courier.id_str]):
                return False, data
            return True, dict()
        except Exception:
            return False, dict()


class OrdersCompleteController(Resource):
    def post(self):
        data = request.get_json()
        validation_result = self.validate_input_data(data)
        if not validation_result[0]:
            return {"validation_error": {"invalid_fields": validation_result[1]}}, 400
        order_id = data[Order.id_str]
        self.set_order_complete(order_id, data[Order.complete_time_str])
        return {Order.id_str: order_id}, 200

    @staticmethod
    def set_order_complete(order_id: int, complete_time: str):
        order = db.get_order_by_id(order_id)
        delivery = db.get_delivery_by_id(order.delivery_id)
        complete_datetime = isodatetime.parse_datetime(complete_time)
        if delivery.previous_order_complete_time is None:
            delivery_time = (complete_datetime - delivery.assign_time).total_seconds()
        else:
            delivery_time = (complete_datetime - delivery.previous_order_complete_time).total_seconds()
        delivery.previous_order_complete_time = complete_datetime
        db.set_order_complete(order_id, delivery_time)
        user_orders = db.get_active_courier_orders(order.courier_id)
        if len(user_orders) == 0:
            delivery.is_complete = True
        db.update_delivery(delivery)

    def validate_input_data(self, data):
        validation_fields_result = self.validate_fields(data)
        if not validation_fields_result[0]:
            return validation_fields_result
        validate_fields_values_result = self.validate_fields_values(data)
        if not validate_fields_values_result[0]:
            return validate_fields_values_result
        return True, list()

    @staticmethod
    def validate_fields_values(data):
        order_id = data[Order.id_str]
        courier_id = data[Courier.id_str]
        order = db.get_order_by_id(order_id)
        if order.is_complete:
            return False, list()
        if order.courier_id is None or order.courier_id != courier_id:
            invalid_fields = list()
            invalid_fields.append(Courier.id_str)
            return False, invalid_fields
        return True, list()

    @staticmethod
    def validate_fields(data):
        try:
            if len(data) != 3:
                return False, list()
            invalid_fields = list()
            if not is_key_with_value_in_data(Courier.id_str, data, int) or not db.is_courier_exist(
                    data[Courier.id_str]):
                invalid_fields.append(Courier.id_str)
            if not is_key_with_value_in_data(Order.id_str, data, int) or not db.is_order_exist(data[Order.id_str]):
                invalid_fields.append(Order.id_str)
            if not is_key_with_value_in_data(Order.complete_time_str, data, str):
                invalid_fields.append(Order.complete_time_str)
            if len(invalid_fields) != 0:
                return False, invalid_fields
            return True, invalid_fields
        except Exception:
            return False, list()
