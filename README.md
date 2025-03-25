Запуск compose: docker-compose up --build

Для возмтжности создания и изменения событий необходимо облодать ролью Admin

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

Переходим к frontend входим в созданный аккаунт  
или через postman (post) http://localhost:8080/api/Auth/login  

Создаем событие через frontend по странице http://localhost:3000/create-event  
или через postman (post) http://localhost:8080/api/Events  

Просмотр созданных событий через frontend по странице http://localhost:3000/events  
или через poatman (Get) http://localhost:8080/api/Events

Просмотр события через frontend странице http://localhost:3000/events/{id}  
или через poatman (Get) http://localhost:8080/api/Events/WithAddress/{id}