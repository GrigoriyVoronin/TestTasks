# Pochta App

## Развертывание приложения:
1. Для запуска приложения вам понадобиться установленный Docker. Как начать использовать докер можете посмотреть на https://www.docker.com/get-started. 
2. Склонируйте себе репозиторий в папку pochta_app
    ```bash
    $ git clone https://github.com/GrigoriyVoronin/pochta_app.git pochta_app
    ```
3. Перейдите в склонирвоанный репозиторий
    ```bash
    $ cd pochta_app
    ```
4. Используйте docker-compose для запуска контейнера с приложением и БД
    ```bash
    $ sudo docker-compose up
    ```
5. Готово! Приложение доступно по адресу http://localhost:8080