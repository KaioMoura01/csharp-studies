FROM node:22-alpine

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm install

COPY . .

# 5174 e não 5173: a 5173 é do TodoForSso, e o realm compartilhado registra
# http://localhost:5174/* como redirect URI deste client.
EXPOSE 5174
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5174"]
