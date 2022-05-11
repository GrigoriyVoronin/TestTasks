from flask import json


def get_order_from_db_row(db_row):
    decoder = json.JSONDecoder()
    order = Order(db_row[0], db_row[1], db_row[2], decoder.decode(db_row[3]))
    order.is_complete = bool(db_row[4])
    order.courier_id = db_row[5]
    order.delivery_time = db_row[6]
    order.delivery_id = db_row[7]
    return order


class Order:
    id: int
    weight: float
    region: int
    delivery_hours: list[str]

    is_complete: bool
    courier_id: int
    delivery_time: int
    delivery_id: int

    id_str = "order_id"
    weight_str = "weight"
    region_str = "region"
    delivery_hours_str = "delivery_hours"
    delivery_id_str = "delivery_id"
    delivery_time_str = "delivery_time"

    is_complete_str = "is_complete"
    courier_id_str = "courier_id"
    assign_time_str = "assign_time"
    complete_time_str = "complete_time"

    def __init__(self, order_id: int, weight: float, region: int, delivery_hours: list[str]):
        self.id = order_id
        self.weight = weight
        self.region = region
        self.delivery_hours = delivery_hours

    def get_json_id(self):
        return {"id": self.id}

    def get_db_insert_str(self):
        encoder = json.JSONEncoder()
        return f"""
        (id, weight, region, delivery_hours)
        VALUES
        ({self.id}, '{self.weight}', '{self.region}', '{encoder.encode(self.delivery_hours)}')
    """
