import sqlite3


if __name__ == '__main__':
    db_name = input()
    first_db_condition = input()
    second_db_condition = input()
    order_field = input()
    db_connection = sqlite3.connect(db_name)
    db_cursor = db_connection.cursor()
    db_cursor.execute(f"""
        SELECT condition
        FROM Talks
        WHERE {first_db_condition}
        AND {second_db_condition}
        ORDER BY {order_field}
    """)
    for line in db_cursor.fetchall():
        print(line[0])
