from isodate import isodatetime
from datetime import datetime
from models.courier import Courier
from models.order import Order


def date_to_iso_str(date: datetime):
    return isodatetime.datetime_isoformat(date, "%Y-%m-%dT%H:%M:%S.%fZ")


def filter_orders_for_courier(courier: Courier, orders: list[Order]):
    issued_orders = list()
    issued_weight = 0
    courier_weight = Courier.type_weight[courier.type]
    for order in orders:
        if issued_weight + order.weight < courier_weight:
            if order.region in courier.regions:
                if is_periods_intersected(order.delivery_hours, courier.working_hours):
                    issued_orders.append(order)
                    issued_weight += order.weight
        else:
            break
    return issued_orders


def is_periods_intersected(order_hours: list[str], courier_hours: list[str]):
    tuples_order_hours: list[tuple[datetime, datetime]] = parse_hours_periods(order_hours)
    tuples_courier_hours: list[tuple[datetime, datetime]] = parse_hours_periods(courier_hours)
    for courier_period in tuples_courier_hours:
        for order_period in tuples_order_hours:
            if courier_period[0] <= order_period[0]:
                if order_period[0] - courier_period[0] < courier_period[1] - courier_period[0]:
                    return True
            if order_period[0] <= courier_period[0]:
                if courier_period[0] - order_period[0] < order_period[1] - order_period[0]:
                    return True
    return False


def parse_hours_periods(periods: list[str]):
    return list(map(lambda x: (datetime.strptime(x[0], "%H:%M"), datetime.strptime(x[1], "%H:%M")),
                    map(lambda x: x.split("-"), periods)))
