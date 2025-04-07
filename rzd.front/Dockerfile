FROM node:21.5.0-alpine as build
WORKDIR /app

COPY ./rzd.front/package*.json ./

RUN npm install

COPY ./rzd.front .

EXPOSE 80

CMD [ "npm", "run", "dev" ]