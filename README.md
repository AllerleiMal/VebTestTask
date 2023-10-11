# Техническое задание

## Описание:

Вам нужно создать ASP.NET Core Web API, который будет предоставлять **CRUD** (Create, Read, Update, Delete) операции для управления списком пользователей. У каждого пользователя должны быть следующие атрибуты: _Id, Имя (Name), Возраст (Age), Email и связанная сущность "Роль" (Role) со значениями (User, Admin, Support, SuperAdmin), где у одного юзера может несколько ролей_.

### Требования:
- [x] Создайте новый проект ASP.NET Core Web API с использованием языка C# и .NET Core.
- [x] Создайте модель данных для пользователя (User) с атрибутами Id, Name, Age, Email и связанной сущностью "Роль" (Role).
- [x] Создайте контроллер (UserController) с методами для выполнения следующих операций:
    - [x] Получение списка всех пользователей( ОБЯЗАТЕЛЬНО реализовать пагинацию, сортировку и фильтрацию по каждому атрибуту в модели User и в модели Role).
    - [x] Получение пользователя по Id и всех его ролей.
    - [x] Добавление новой роли пользователю.
    - [x] Создание нового пользователя.
    - [x] Обновление информации о пользователе по Id.
    - [x] Удаление пользователя по Id.
- [x] Добавьте в контроллер бизнес-логику для валидации данных:
    - [x] Проверка наличия обязательных полей (Имя, Возраст, Email).
    - [x] Проверка уникальности Email.
    - [x] Проверка возраста на положительное число.
- [x] Используйте Entity Framework Core (или любой другой ORM по вашему выбору) для доступа к данным и сохранения пользователей и ролей в базе данных.
- [x] Создайте миграцию использую ORM для создания необходимой таблицы в базе данных.
- [x] Добавьте обработку ошибок и возврат соответствующих статусных кодов HTTP (например, 404 при отсутствии пользователя).
- [x] Документируйте ваш API с использованием инструментов Swagger или подобных.
### Дополнительные задания (по желанию):
- [x] Добавьте аутентификацию и авторизацию к вашему API с использованием JWT-токенов.
- [x] Настройте логирование действий в API (например, с использованием Serilog).