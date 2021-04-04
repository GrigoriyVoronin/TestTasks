from datetime import datetime
from isodate import isodatetime
from utils import date_to_iso_str


def get_delivery_from_db_row(db_row):
    if db_row[2] is not None:
        previous_time = isodatetime.parse_datetime(db_row[2])
    else:
        previous_time = None
    delivery = Delivery(db_row[1], previous_time, db_row[3], isodatetime.parse_datetime(db_row[4]), db_row[5])
    delivery.id = db_row[0]
    return delivery


class Delivery:
    id: int
    courier_type: str
    previous_order_complete_time: datetime
    courier_id: int
    assign_time: datetime
    is_complete: bool
    previous_order_complete_time_str = "previous_order_complete_time"
    courier_type_str = "courier_type"
    courier_id_str = "courier_id"
    assign_time_str = "assign_time"
    is_complete_str = "is_complete"

    courier_type_to_rate = {
        "foot": 2,
        "bike": 5,
        "car": 9
    }

    def __init__(self, courier_type: str, previous_order_complete_time: datetime,
                 courier_id: int, assign_time: datetime, is_complete: bool):
        self.courier_type = courier_type
        self.previous_order_complete_time = previous_order_complete_time
        self.courier_id = courier_id
        self.assign_time = assign_time
        self.is_complete = is_complete

    def get_db_insert_str(self):
        if self.previous_order_complete_time is None:
            previous_time = "null"
        else:
            previous_time = f"'{date_to_iso_str(self.previous_order_complete_time)}'"
        return f"""
                ({self.courier_type_str}, {self.previous_order_complete_time_str}, {self.courier_id_str},
                {self.assign_time_str}, {self.is_complete_str})
                VALUES
                ('{self.courier_type}', {previous_time}, '{self.courier_id}', 
                '{date_to_iso_str(self.assign_time)}', '{int(self.is_complete)}')
        """
