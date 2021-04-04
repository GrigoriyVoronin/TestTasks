from flask import json


def get_courier_from_db_row(db_row):
    decoder = json.JSONDecoder()
    return Courier(db_row[0], db_row[1], decoder.decode(db_row[2]), decoder.decode(db_row[3]))


class Courier:
    id: int
    type: str
    regions: list[int]
    working_hours: list[str]

    id_str = "courier_id"
    type_str = "courier_type"
    regions_str = "regions"
    working_hours_str = "working_hours"

    type_weight = {
        "foot": 10,
        "bike": 15,
        "car": 50
    }

    properties_str = {
        id_str,
        type_str,
        regions_str,
        working_hours_str
    }

    def __init__(self, courier_id: int, courier_type: str, regions: list[int], working_hours: list[str]):
        self.id = courier_id
        self.type = courier_type
        self.regions = regions
        self.working_hours = working_hours

    def get_json_id(self):
        return {"id": self.id}

    def json(self):
        return {self.id_str: self.id, self.type_str: self.type,
                self.regions_str: self.regions,
                self.working_hours_str: self.working_hours
                }

    def json_with_info(self, rating: float, earnings: int):
        return {self.id_str: self.id, self.type_str: self.type,
                self.regions_str: self.regions,
                self.working_hours_str: self.working_hours,
                "rating": rating, "earnings": earnings
                }

    def get_db_insert_str(self):
        encoder = json.JSONEncoder()
        return f"""
        (id, type, regions, working_hours)
        VALUES
        ({self.id}, '{self.type}', '{encoder.encode(self.regions)}', '{encoder.encode(self.working_hours)}')
"""
