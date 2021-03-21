ARG NODE_IMAGE=node:12.0
FROM node:13.12.0-alpine
WORKDIR /web
ENV PATH /app/node_modules/.bin:$PATH
EXPOSE 80

FROM ${NODE_IMAGE} as node-build
COPY Web/the-room/package.json .
# COPY Web/the-room/package-lock.json .
COPY Web/the-room .
RUN npm i npm@6.14.11 -g && npm update && npm install
RUN npm install react-scripts@3.4.1 -g --silent

# start app
CMD ["npm", "start"]