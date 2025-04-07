FROM node:20 AS build-stage

WORKDIR /app

COPY ./rzd.front/package*.json ./
RUN npm install

COPY ./rzd.front .
RUN npm run build

FROM nginx:stable-alpine AS production-stage

COPY ./rzd.front/nginx.conf /etc/nginx/nginx.conf

COPY --from=build-stage /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
