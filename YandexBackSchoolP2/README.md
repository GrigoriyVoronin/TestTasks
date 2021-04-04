# Вторая часть тестового задания в бэкенд школу

Пакеты необходимые для приложения перечислены в файле requirements.txt

## Развертывание приложения на сервере:
1. Установить Docker Engine:

    ```bash
    $ curl -fsSL https://get.docker.com -o get-docker.sh
    $ sudo sh get-docker.sh
    ```
2. Склонировать текущий репозиторий в папку api
    ```bash
    $ git clone https://github.com/GrigoriyVoronin/yandex_back_p2.git api
    ```
3. Перейти в склонирвоанный репозиторий
    ```bash
    $ cd api
    ```
4. Выполнить сборку образа контейнера
    ```bash
    $ sudo docker build -t api .
    ```
5. Запустить контейнер с проброской на порт 8080, и параметром перезапуска.
    ```bash
    $ sudo docker run --name api -d -p 8080:8080 --restart always api
    ```
