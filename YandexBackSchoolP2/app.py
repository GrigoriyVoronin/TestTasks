from flask import Flask
from flask_restful import Api
from controllers.couriers import CouriersController, CourierController
from controllers.orders import OrdersController, OrdersAssignController, OrdersCompleteController
from repository import db

app = Flask(__name__)
app.config['SQLALCHEMY_DATABASE_URI'] = 'sqlite:///database.db'
api = Api(app)
api.add_resource(CouriersController, "/couriers")
api.add_resource(CourierController, "/couriers/<int:courier_id>")
api.add_resource(OrdersController, "/orders")
api.add_resource(OrdersAssignController, "/orders/assign")
api.add_resource(OrdersCompleteController, "/orders/complete")


@app.before_first_request
def create_table():
    db.init()


if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8080)
