Запуск compose: docker-compose up -d

Применение миграций: dotnet ef database update -s EventManager -p Infrastucture

Через postman создаем пользователя с ролью Admin:(post) http://localhost:8080/api/Users
{
  "name": "Admin",
  "password": "Admin",
  "role": "Admin",
  "surename": "Admin",
  "birthDate": "2000-01-01",
  "email": "test@test.com"
}

Открытие swagger: http://localhost:8080/swagger/index.html

Открытие frontend: http://localhost:3000/