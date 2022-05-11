import sqlite3
from models.courier import Courier, get_courier_from_db_row
from models.order import Order, get_order_from_db_row
from models.delivery import Delivery, get_delivery_from_db_row
from isodate import isodatetime
from utils import date_to_iso_str


def get_connection():
    return sqlite3.connect("database.db")


def init():
    connection = get_connection()
    with open("repository/schema.sql") as file:
        connection.executescript(file.read())
    connection.commit()
    connection.close()


def get_items_by_id(item_id: int, collection_name: str):
    connection = get_connection()
    cursor = connection.cursor()
    cursor.execute(f"""
        select * from {collection_name}
        where id = {item_id}
    """)
    items = cursor.fetchall()
    cursor.close()
    connection.close()
    return items


def insert_items(collection_name: str, items: list):
    connection = get_connection()
    cursor = connection.cursor()
    for item in items:
        sql = f"""
        INSERT INTO {collection_name}
        {item.get_db_insert_str()}
"""
        cursor.execute(sql)
    connection.commit()
    cursor.close()
    connection.close()


def insert_couriers(couriers: list[Courier]):
    insert_items("couriers", couriers)


def get_courier_by_id(courier_id: int):
    response = get_items_by_id(courier_id, "couriers")
    if len(response) != 1:
        return None
    db_row = response[0]
    return get_courier_from_db_row(db_row)


def is_courier_exist(courier_id: int):
    courier = get_courier_by_id(courier_id)
    return courier is not None


def patch_courier(courier_id: int, data):
    connection = get_connection()
    cursor = connection.cursor()
    for field in data:
        sql = f"""
        update couriers
        set {field} = '{data[field]}'
        where id = {courier_id}
"""
        cursor.execute(sql)
    connection.commit()
    cursor.close()
    connection.close()
    return get_courier_by_id(courier_id)


def get_order_by_id(order_id: int):
    response = get_items_by_id(order_id, "orders")
    if len(response) != 1:
        return None
    db_row = response[0]
    return get_order_from_db_row(db_row)


def is_order_exist(order_id: int):
    order = get_order_by_id(order_id)
    return order is not None


def insert_orders(orders: list[Order]):
    insert_items("orders", orders)


def set_order_complete(order_id: int, delivery_time: str):
    connection = get_connection()
    cursor = connection.cursor()
    sql = f"""
        update orders
        set
        {Order.is_complete_str} = 1,
        {Order.delivery_time_str} = {delivery_time}
        where id = {order_id}
    """
    cursor.execute(sql)
    connection.commit()
    cursor.close()
    connection.close()


def get_active_courier_orders(courier_id: int):
    connection = get_connection()
    cursor = connection.cursor()
    sql = f"""
        select * from orders
        where {Order.courier_id_str} = {courier_id} and
        ({Order.is_complete_str} is null or {Order.is_complete_str} = 0)
    """
    cursor.execute(sql)
    db_rows = cursor.fetchall()
    cursor.close()
    connection.close()
    return list(map(lambda x: get_order_from_db_row(x), db_rows))


def get_free_orders_ordered_by_weight(max_weight: float):
    connection = get_connection()
    cursor = connection.cursor()
    sql = f"""
        select * from orders
        where {Order.weight_str} <= {max_weight} and {Order.courier_id_str} is null
        order by {Order.weight_str}
    """
    cursor.execute(sql)
    db_rows = cursor.fetchall()
    cursor.close()
    connection.close()
    return list(map(lambda x: get_order_from_db_row(x), db_rows))


def get_current_courier_delivery(courier_id: int):
    connection = get_connection()
    cursor = connection.cursor()
    sql = f"""
        select * from deliveries
        where {Delivery.courier_id_str} = '{courier_id}' and {Delivery.is_complete_str} = {0}
    """
    cursor.execute(sql)
    db_row = cursor.fetchone()
    return get_delivery_from_db_row(db_row)


def insert_delivery(courier: Courier):
    assign_time = isodatetime.datetime.now()
    insert_items("deliveries", [Delivery(courier.type, None, courier.id, assign_time, False)])
    delivery = get_current_courier_delivery(courier.id)
    return delivery


def issue_orders(orders: list[Order], courier: Courier):
    connection = get_connection()
    cursor = connection.cursor()
    delivery = insert_delivery(courier)
    for order in orders:
        sql = f"""
            update orders
            set {Order.delivery_id_str} = {delivery.id},
            {Order.courier_id_str} = {courier.id}
            where id = {order.id}
    """
        cursor.execute(sql)
    connection.commit()
    cursor.close()
    connection.close()
    issued_orders = list()
    for order in orders:
        issued_orders.append(get_order_by_id(order.id))
    return issued_orders


def return_order(order_id: int):
    connection = get_connection()
    cursor = connection.cursor()

    sql = f"""
        update orders
        set {Order.delivery_id_str} = null,
        {Order.courier_id_str} = null
        where id = {order_id}
"""
    cursor.execute(sql)
    connection.commit()
    cursor.close()
    connection.close()


def get_delivery_by_id(delivery_id: int):
    response = get_items_by_id(delivery_id, "deliveries")
    if len(response) != 1:
        return None
    db_row = response[0]
    return get_delivery_from_db_row(db_row)


def get_assign_time(order: Order):
    delivery = get_delivery_by_id(order.delivery_id)
    return delivery.assign_time


def update_delivery(delivery: Delivery):
    connection = get_connection()
    cursor = connection.cursor()
    sql = f"""
            update deliveries
            set
            {Delivery.is_complete_str} = {int(delivery.is_complete)},
            {Delivery.previous_order_complete_time_str} = '{date_to_iso_str(delivery.previous_order_complete_time)}'
            where id = {delivery.id}
        """
    cursor.execute(sql)
    connection.commit()
    cursor.close()
    connection.close()


def get_courier_complete_deliveries(courier_id: int):
    connection = get_connection()
    cursor = connection.cursor()
    sql = f"""
    select * from deliveries
    where {Delivery.is_complete_str} = 1 and
    {Delivery.courier_id_str} = {courier_id}
"""
    cursor.execute(sql)
    db_rows = cursor.fetchall()
    return list(map(lambda x: get_delivery_from_db_row(x), db_rows))


def get_orders_from_deliveries(deliveries: list[Delivery]):
    connection = get_connection()
    cursor = connection.cursor()
    orders: list[Order] = list()
    for delivery in deliveries:
        sql = f"""
        select * from orders
        where {Order.delivery_id_str} = {delivery.id}
    """
        cursor.execute(sql)
        orders.extend(map(lambda x: get_order_from_db_row(x), cursor.fetchall()))
    return orders
