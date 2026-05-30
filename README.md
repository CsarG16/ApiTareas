# API Inteligente de Tareas y Análisis

API RESTful construida con **ASP.NET Core Web API (.NET 8)**, **Entity Framework Core**, **SQLite** y **ML.NET** para gestionar tareas internas, consumir datos de APIs externas y analizar sentimientos de comentarios con inteligencia artificial.

---

## 🚀 Pasos para ejecutar localmente

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Git](https://git-scm.com/)

### Instalación

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/TU_USUARIO/ApiTareas.git
   cd ApiTareas
   ```

2. Restaurar las dependencias:
   ```bash
   dotnet restore
   ```

3. Aplicar las migraciones de base de datos (ver sección siguiente).

4. Ejecutar la aplicación:
   ```bash
   dotnet run
   ```

5. Abrir Swagger en el navegador:
   ```
   https://localhost:{puerto}/swagger
   ```

---

## 🗄️ Comandos de migración

### Crear la migración inicial
```bash
dotnet ef migrations add InitialCreate
```

### Aplicar las migraciones a la base de datos SQLite
```bash
dotnet ef database update
```

> La base de datos se crea automáticamente como un archivo `tareas.db` en la raíz del proyecto.

### Eliminar la última migración (si es necesario)
```bash
dotnet ef migrations remove
```

---

## 📡 Endpoints implementados

### Pregunta 1 — CRUD de Tareas

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/tareas` | Obtener todas las tareas |
| `GET` | `/api/tareas/{id}` | Obtener una tarea por ID |
| `POST` | `/api/tareas` | Crear una nueva tarea |
| `PUT` | `/api/tareas/{id}` | Actualizar una tarea existente |
| `DELETE` | `/api/tareas/{id}` | Eliminar una tarea |

#### Modelo de Tarea
```json
{
  "id": 1,
  "titulo": "Revisar código del módulo",
  "descripcion": "Revisar los cambios del sprint actual",
  "estado": "Pendiente",
  "prioridad": "Alta",
  "fechaCreacion": "2026-05-29T00:00:00",
  "fechaVencimiento": "2026-06-15T00:00:00"
}
```

#### Validaciones
- El **título** es obligatorio.
- El **estado** es obligatorio. Valores permitidos: `Pendiente`, `EnProceso`, `Completada`.
- La **prioridad** es obligatoria. Valores permitidos: `Baja`, `Media`, `Alta`.
- La **fecha de vencimiento** no puede ser menor a la fecha actual.

---

### Pregunta 2 — Filtros y búsqueda

El endpoint `GET /api/tareas` permite filtrar por parámetros opcionales:

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| `estado` | string | Filtrar por estado (`Pendiente`, `EnProceso`, `Completada`) |
| `prioridad` | string | Filtrar por prioridad (`Baja`, `Media`, `Alta`) |
| `fechaInicio` | DateTime | Fecha mínima de vencimiento |
| `fechaFin` | DateTime | Fecha máxima de vencimiento |

#### Ejemplos de uso
```
GET /api/tareas?estado=Pendiente
GET /api/tareas?prioridad=Alta
GET /api/tareas?estado=EnProceso&prioridad=Media
GET /api/tareas?fechaInicio=2026-05-01&fechaFin=2026-05-31
```

#### Validaciones
- Si `fechaInicio` es mayor que `fechaFin`, se devuelve error `400 Bad Request`.
- Si el `estado` no es válido, se devuelve error `400 Bad Request`.
- Si la `prioridad` no es válida, se devuelve error `400 Bad Request`.

---

### Pregunta 3 — Consumo de API externa

Se consume la API pública: `https://jsonplaceholder.typicode.com/todos`

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/tareas-externas` | Obtener todas las tareas externas |
| `GET` | `/api/tareas-externas/{id}` | Obtener una tarea externa por ID |

#### Ejemplo de respuesta
```json
{
  "externalId": 1,
  "titulo": "delectus aut autem",
  "completado": false
}
```

#### Ejemplo de uso de la API externa

La API externa (`jsonplaceholder.typicode.com/todos`) devuelve un JSON con la siguiente estructura original:
```json
{
  "userId": 1,
  "id": 1,
  "title": "delectus aut autem",
  "completed": false
}
```

Nuestro servicio (`TareasExternasService`) consume esta API usando `HttpClient`, deserializa el JSON original en un DTO interno (`JsonPlaceholderTodoDto`) y luego lo **mapea** a nuestro DTO propio (`ExternalTodoDto`) con los campos renombrados al español (`externalId`, `titulo`, `completado`). Nunca se expone el JSON original directamente al cliente.

#### Manejo de errores
- Si la API externa no responde, se devuelve un error `503 Service Unavailable` con un mensaje controlado.
- Si el ID solicitado no existe, se devuelve `404 Not Found`.

---

### Pregunta 4 — ML.NET: Análisis de sentimiento

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/ml/sentimiento` | Analizar el sentimiento de un comentario |

#### Request
```json
{
  "comentario": "La tarea fue completada correctamente y el sistema funciona bien"
}
```

#### Response
```json
{
  "comentario": "La tarea fue completada correctamente y el sistema funciona bien",
  "sentimiento": "Positivo"
}
```

---

## 🤖 Explicación del modelo ML.NET

### Tipo de modelo
Se utiliza **Clasificación Binaria** (`BinaryClassification`) con el algoritmo **SDCA Logistic Regression** de ML.NET.

### ¿Qué hace?
Clasifica comentarios de texto en dos categorías:
- **Positivo**: Comentarios que reflejan resultados favorables, buen desempeño o logros.
- **Negativo**: Comentarios que reflejan problemas, errores, retrasos o insatisfacción.

### Dataset de entrenamiento
El dataset se encuentra en `Data/sentiment_dataset.csv` y contiene **80 frases** en español (40 positivas y 40 negativas) relacionadas con gestión de tareas y desarrollo de software.

Formato del CSV:
```csv
Comentario,Sentimiento
La tarea fue completada correctamente y el sistema funciona bien,true
El sistema no funciona correctamente y tiene muchos errores,false
```

### Pipeline de entrenamiento
1. **Featurización de texto**: Convierte el texto del comentario en un vector numérico de características (`FeaturizeText`).
2. **Entrenamiento**: Se aplica el algoritmo `SdcaLogisticRegression` sobre las características generadas.
3. **Predicción**: El modelo entrenado clasifica nuevos comentarios como positivos (`true`) o negativos (`false`).

### Ciclo de vida
El modelo se entrena **una sola vez** al iniciar la aplicación (registrado como `Singleton`) y se reutiliza para todas las predicciones, garantizando un rendimiento óptimo.

---

## 🌿 Estructura de ramas

| Rama | Descripción |
|------|-------------|
| `main` | Rama principal con todo el código integrado |
| `feature/api-tareas` | Pregunta 1: CRUD de tareas y modelo de datos |
| `feature/filtros-tareas` | Pregunta 2: Filtros y búsqueda |
| `feature/api-externa-todos` | Pregunta 3: Consumo de API externa |
| `feature/mlnet-basico` | Pregunta 4: Análisis de sentimiento con ML.NET |

Cada rama fue creada desde `main`, desarrollada de forma independiente y fusionada mediante **Pull Request**.

---

## 📁 Estructura del proyecto

```
ApiTareas/
├── Controllers/
│   ├── TareasController.cs            # CRUD y filtros de tareas
│   ├── TareasExternasController.cs    # Consumo de API externa
│   └── MlController.cs               # Análisis de sentimiento
├── Data/
│   ├── TareasDbContext.cs             # Contexto de Entity Framework Core
│   └── sentiment_dataset.csv          # Dataset de entrenamiento para ML.NET
├── Models/
│   ├── Tarea.cs                       # Entidad de dominio
│   ├── EstadoTarea.cs                 # Enum de estados
│   ├── PrioridadTarea.cs              # Enum de prioridades
│   ├── DTOs/
│   │   ├── ExternalTodoDto.cs         # DTO de respuesta para tareas externas
│   │   ├── JsonPlaceholderTodoDto.cs  # DTO interno para deserializar API externa
│   │   ├── SentimientoRequest.cs      # DTO de request para análisis de sentimiento
│   │   └── SentimientoResponse.cs     # DTO de response para análisis de sentimiento
│   └── ML/
│       └── SentimentData.cs           # Modelos de entrada/salida para ML.NET
├── Services/
│   ├── ITareasExternasService.cs      # Interfaz del servicio de API externa
│   ├── TareasExternasService.cs       # Implementación del consumo de API externa
│   ├── IMlService.cs                  # Interfaz del servicio de ML.NET
│   └── MlService.cs                   # Implementación del análisis de sentimiento
├── Migrations/                        # Migraciones de Entity Framework Core
├── Program.cs                         # Configuración y registro de servicios
├── appsettings.json                   # Cadena de conexión a SQLite
└── ApiTareas.csproj                   # Dependencias del proyecto
```

---

## 🛠️ Tecnologías utilizadas

- **ASP.NET Core Web API** (.NET 8)
- **Entity Framework Core** 8.0.13
- **SQLite** (base de datos local)
- **ML.NET** 3.0.1 (análisis de sentimiento)
- **Swagger / OpenAPI** (documentación interactiva)
- **HttpClient** (consumo de API externa)
