ARG NODE_IMAGE=node:12.0
FROM node:13.12.0-alpine
WORKDIR /web
ENV PATH /app/node_modules/.bin:$PATH
EXPOSE 80

FROM ${NODE_IMAGE} as node-build
COPY Web/the-room/package.json .
# COPY Web/the-room/package-lock.json .
COPY Web/the-room .
RUN npm i react-scripts -g --silent

# start app
CMD ["npm", "start"]