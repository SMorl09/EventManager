FROM node:16-alpine AS build
WORKDIR /app

# Установка зависимостей
COPY package.json package-lock.json ./
RUN npm install

# Копирование исходного кода и сборка приложения
COPY . .
RUN npm run build

# Второй этап: создание образа для исполнения
FROM node:16-alpine AS runtime
WORKDIR /app

# Установка сервера для раздачи статических файлов
RUN npm install -g serve

# Копирование сборки приложения
COPY --from=build /app/build .

# Открытие порта
EXPOSE 3000

# Команда для запуска сервера
CMD ["serve", "-s", ".", "-l", "3000"]
