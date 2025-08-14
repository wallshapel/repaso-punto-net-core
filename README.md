# API .NET Core - Employee Management

## Descripción

Proyecto API REST desarrollado en **.NET Core** para la gestión de empleados. Actualmente implementa operaciones CRUD básicas sobre el modelo `Employee` y utiliza **Entity Framework Core** para la comunicación con una base de datos **SQL Server**.

La aplicación está preparada para:

- Ampliarse con más modelos y relaciones.
- Integrar seguridad con **JWT** (similar a Spring Security).
- Implementar patrones **DTO** y **Repository**.
- Añadir validaciones y manejo global de excepciones.
- Dockerización para despliegue.

## Estructura del Proyecto

```
api.sln                # Archivo de solución
/api                   # Proyecto principal (.csproj)
    /Controllers       # Controladores API
    /Data              # DbContext y configuración de acceso a datos
    /Models            # Entidades (POCO classes)
    /Services          # Interfaces y lógica de negocio
    appsettings.json   # Configuración de la aplicación
    Program.cs         # Configuración y arranque de la aplicación
```

## Tecnologías Utilizadas

- **.NET Core**
- **Entity Framework Core**
- **SQL Server** (contenedor Docker)
- **Swagger** (habilitado por defecto en desarrollo)

## Configuración

### Base de datos

El proyecto está configurado para conectarse a una instancia de SQL Server en contenedor Docker.

Cadena de conexión (en `appsettings.json`):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=api;User Id=sa;Password=Legato1218*;Encrypt=True;TrustServerCertificate=True"
}
```

### Puertos

Configuración de **Kestrel** para desarrollo:

```json
"Kestrel": {
  "Endpoints": {
    "Http": {
      "Url": "http://localhost:5000"
    },
    "Https": {
      "Url": "https://localhost:5001"
    }
  }
}
```

## Endpoints Actuales

**Base URL:** `http://localhost:5000/api/employee`

| Método | Ruta    | Descripción                     | Código de respuesta |
| ------ | ------- | ------------------------------- | ------------------- |
| GET    | `/`     | Lista todos los empleados       | 200 OK              |
| POST   | `/`     | Crea un nuevo empleado          | 201 Created         |
| PUT    | `/{id}` | Actualiza un empleado existente | 204 No Content      |
| DELETE | `/{id}` | Elimina un empleado             | 204 No Content      |

## Ejecución

1. Asegúrate de tener corriendo el contenedor de SQL Server con puerto `1433` publicado.
2. Ejecuta el proyecto:
   ```bash
   dotnet run
   ```
3. Consumir la API usando Postman o cualquier cliente HTTP.

## Próximos pasos

- Implementar autenticación y autorización con JWT.
- Añadir validaciones y DTOs.
- Crear capa Repository.
- Manejo global de excepciones.
- Contenerización con Docker.

