# FlashSale

Plataforma de e-commerce en flash sale en tiempo real. Generador de intentos de compra → Kafka → Consumer → MongoDB → Api + Dashboard.

## 1. Levantar Kafka y MongoDB

Desde la raíz del proyecto (donde está `docker-compose.yml`):

docker compose up -d

Verificar que todo esté arriba:

docker ps

## 2. Abrir la solución

Abre `FlashSale.sln` en Visual Studio.

## 3. Configurar los proyectos de inicio

Clic derecho en la solución → **Configure Startup Projects** → **Multiple startup projects** → pon `Start` en:
- `FlashSale.Generator`
- `FlashSale.Consumer`
- `FlashSale.Api`
- `FlashSale.Shared` como "None"

## 4. Ejecutar

Presiona **F5**. Abrir los localhost:
- El formulario del **Generator** (ventana de envío individual y lote masivo). 
- El MongoDB para gestionar la base de datos y sus eventos.
- La consola del **Consumer** (procesa los eventos de Kafka).
- **Swagger** de la **Api** en `/swagger`: https://localhost:7020/swagger/index.html

## 5. Ver el dashboard

En el navegador:

https://localhost:7020/dashboard.html

## 6. Apagar todo

docker compose down
