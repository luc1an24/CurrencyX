# Stage 1: Build Angular application
FROM node:18 AS build
WORKDIR /app
COPY ./package.json ./package-lock.json ./
RUN npm install
COPY . .
RUN npm run build

# Stage 2: Serve Angular with Nginx
FROM nginx:alpine
COPY --from=build /app/dist/exchange-rates /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
