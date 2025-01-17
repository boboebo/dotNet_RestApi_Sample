
### Aquí tienes una lista de pasos para crear una API REST en C# utilizando Entity Framework (EF) para consultar una base de datos en MSSQL con tres tablas: **Clientes**, **Productos** y **Facturas**.

---

### **Paso 1: Configurar el entorno de desarrollo**
**Contexto:** Configurar Visual Studio (o tu IDE preferido) es el primer paso para desarrollar una aplicación en .NET. MSSQL será la base de datos donde almacenaremos los datos.

1. Descarga e instala **Visual Studio 2022** o una versión compatible.
2. Asegúrate de instalar la carga de trabajo de **Desarrollo .NET y Web** durante la instalación.
3. Instala MSSQL Server (o asegúrate de tener acceso a una instancia existente) y SQL Server Management Studio (SSMS) para administrar la base de datos.

---

### **Paso 2: Crear un nuevo proyecto de API**
**Contexto:** .NET facilita la creación de APIs mediante plantillas integradas. Elegimos una API Web para construir nuestra interfaz RESTful.

1. Abre Visual Studio y selecciona **Crear un nuevo proyecto**.
2. Escoge la plantilla **ASP.NET Core Web API**.
3. Configura el nombre del proyecto, la ubicación y selecciona .NET 6 (o superior).
4. En las opciones adicionales, habilita **Use controllers (Controladores)**.

---

### **Paso 3: Configurar el acceso a la base de datos**
**Contexto:** Entity Framework Core es un ORM (Object-Relational Mapper) que facilita la interacción entre la base de datos y tu aplicación.

1. Instala los paquetes necesarios en tu proyecto:
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   ```
2. En el archivo `appsettings.json`, agrega la cadena de conexión para tu base de datos MSSQL:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=TU_SERVIDOR;Database=TU_BD;User Id=USUARIO;Password=CONTRASEÑA;"
   }
   ```
   si se usa windows authentication:
      ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=EN1810451\\SQLEXPRESS;Database=ApiAppProyecto;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```
---

Si estás utilizando **Windows Authentication** y tu servidor SQL es `EN1810451\SQLEXPRESS`, puedes construir la cadena de conexión de la siguiente manera:

### **Formato para conexión con Windows Authentication**
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=EN1810451\\SQLEXPRESS;Database=NombreDeTuBaseDeDatos;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

### **Explicación de los componentes:**
1. **`Server=EN1810451\\SQLEXPRESS`**:
   - `EN1810451\SQLEXPRESS` es el nombre de tu instancia de SQL Server.
   - Dado que la barra invertida (`\`) es un carácter especial en JSON, necesitas usar `\\` para que se interprete correctamente.

2. **`Database=NombreDeTuBaseDeDatos`**:
   - Cambia `NombreDeTuBaseDeDatos` por el nombre de tu base de datos en SQL Server.

3. **`Trusted_Connection=True`**:
   - Indica que estás utilizando Windows Authentication para conectarte a la base de datos.

4. **`MultipleActiveResultSets=true`**:
   - Permite ejecutar múltiples consultas al mismo tiempo en la misma conexión (opcional pero útil para EF Core).

---


### **Paso 4: Crear el modelo de datos**
**Contexto:** Los modelos representan las tablas de tu base de datos y las relaciones entre ellas.

1. Define las clases para **Clientes**, **Productos** y **Facturas** en un nuevo directorio llamado `Models`. Ejemplo:
   ```csharp
   public class Cliente
   {
       public int Id { get; set; }
       public string Nombre { get; set; }
       public string Correo { get; set; }
       public ICollection<Factura> Facturas { get; set; }
   }
   public class Producto
   {
       public int Id { get; set; }
       public string Nombre { get; set; }
       public decimal Precio { get; set; }
   }
   public class Factura
   {
       public int Id { get; set; }
       public int ClienteId { get; set; }
       public Cliente Cliente { get; set; }
       public ICollection<Producto> Productos { get; set; }
   }
   ```

---

### **Paso 5: Configurar el contexto de datos**
**Contexto:** El `DbContext` es la puerta de enlace principal de EF Core para interactuar con la base de datos.

1. Crea una clase `AppDbContext` en un nuevo directorio `Data`:
   ```csharp
   public class AppDbContext : DbContext
   {
       public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

       public DbSet<Cliente> Clientes { get; set; }
       public DbSet<Producto> Productos { get; set; }
       public DbSet<Factura> Facturas { get; set; }
   }
   ```
2. En el archivo `Program.cs`, registra el `DbContext`:
   ```csharp
   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```

---

El Paso 5: Configurar el contexto de datos es fundamental porque establece la conexión entre tu aplicación y la base de datos. 
---

### **¿Qué es un `DbContext`?**
El `DbContext` es la clase principal que proporciona Entity Framework (EF) Core para interactuar con tu base de datos. Sirve como una puerta de enlace para:
- **Administrar entidades**: Crea, lee, actualiza y elimina datos (CRUD).
- **Mapear tablas**: Conecta las clases de tu modelo a las tablas de la base de datos.
- **Configurar relaciones**: Define las claves foráneas, cardinalidad, índices, etc.
- **Realizar consultas**: Permite ejecutar consultas LINQ para manipular datos.

---

### **Pasos detallados para configurar el `DbContext`**

#### **1. Crear la clase `AppDbContext`**
Esta clase hereda de `DbContext` y se utiliza para especificar las tablas de tu base de datos que quieres manejar.

Ejemplo:
```csharp
using Microsoft.EntityFrameworkCore;
using TuProyecto.Models; // Cambia 'TuProyecto' por el namespace donde estén tus modelos

public class AppDbContext : DbContext
{
    // Constructor que pasa las opciones al base (DbContext)
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DbSets que representan las tablas de la base de datos
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Factura> Facturas { get; set; }
}
```

#### **2. Agregar la configuración del `DbContext` en `Program.cs`**
El archivo `Program.cs` configura los servicios necesarios para que tu API funcione. Aquí es donde debes registrar tu `DbContext` para que tu aplicación lo use.

En el método principal, agrega esta línea:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**¿Qué hace esta línea?**
1. **`AddDbContext<AppDbContext>`**: Registra la clase `AppDbContext` en el [contenedor de dependencias](Registrar la clase) de ASP.NET Core.
2. **`options.UseSqlServer`**: Especifica que EF Core debe usar SQL Server como proveedor de base de datos.
3. **`builder.Configuration.GetConnectionString("DefaultConnection")`**: Obtiene la cadena de conexión que configuraste en `appsettings.json`.

#### **3. Conectar los modelos con las tablas**
En el `DbContext`, cada `DbSet` corresponde a una tabla de la base de datos. Por ejemplo:
- `public DbSet<Cliente> Clientes { get; set; }`:
  - Mapea la tabla **Clientes** a la clase `Cliente`.
- `public DbSet<Producto> Productos { get; set; }`:
  - Mapea la tabla **Productos** a la clase `Producto`.
- `public DbSet<Factura> Facturas { get; set; }`:
  - Mapea la tabla **Facturas** a la clase `Factura`.

---

### **Opcional: Configurar relaciones personalizadas**
Si tus modelos tienen relaciones específicas (por ejemplo, una relación uno-a-muchos o muchos-a-muchos), puedes usar el método `OnModelCreating` para configurarlas.

Ejemplo:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configurar relación uno-a-muchos entre Cliente y Facturas
    modelBuilder.Entity<Factura>()
        .HasOne(f => f.Cliente)
        .WithMany(c => c.Facturas)
        .HasForeignKey(f => f.ClienteId);

    // Configurar relación muchos-a-muchos entre Producto y Factura
    modelBuilder.Entity<FacturaProducto>()
        .HasKey(fp => new { fp.FacturaId, fp.ProductoId });
}
```

---

### **¿Por qué es importante este paso?**
1. **Centraliza la configuración**: Define cómo interactúa tu aplicación con la base de datos.
2. **Simplifica el código**: Permite manipular los datos directamente como objetos (sin escribir SQL manualmente).
3. **Mapea correctamente las relaciones**: Evita errores en las relaciones entre tablas.
4. **Proporciona extensibilidad**: Puedes personalizar el comportamiento del mapeo y optimizar la base de datos.

---

## Registrar la clase 
en el **contenedor de dependencias** de ASP.NET Core significa que estás configurando una instancia de esa clase (o su implementación) para que pueda ser **inyectada automáticamente** en otras partes de tu aplicación cuando sea necesaria.

---

### **¿Qué es el contenedor de dependencias?**
El contenedor de dependencias es un mecanismo integrado en ASP.NET Core que administra las dependencias de los objetos de tu aplicación. 
En lugar de crear manualmente instancias de clases cada vez que las necesitas, el contenedor se encarga de:
- Crear instancias de las clases.
- Administrar su ciclo de vida (como liberar memoria cuando ya no se necesitan).
- Resolver automáticamente las dependencias entre las clases.

Esto se hace mediante un patrón de diseño llamado **Inyección de Dependencias (Dependency Injection, DI)**.

---

### **¿Por qué registrar en el contenedor de dependencias?**
Registrar una clase como `AppDbContext` le dice a ASP.NET Core cómo crear y administrar instancias de esa clase. Esto es útil porque:
1. **Centraliza la configuración de tus dependencias.**
2. **Promueve el desacoplamiento**, permitiendo que las clases dependan de abstracciones en lugar de implementaciones concretas.
3. **Facilita el testeo**, ya que puedes reemplazar dependencias reales con dependencias simuladas (mocks) durante las pruebas.

---

### **¿Cómo se registra una clase?**
La línea en el archivo `Program.cs`:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

**¿Qué hace?**
1. **`builder.Services.AddDbContext<AppDbContext>`**:
   - Registra la clase `AppDbContext` en el contenedor de dependencias.
   - Cada vez que una clase necesite un `AppDbContext`, el contenedor creará una nueva instancia o reutilizará una existente, dependiendo del ciclo de vida configurado.

2. **`options.UseSqlServer`**:
   - Configura el proveedor de base de datos que debe usar EF Core (en este caso, SQL Server).
   - Usa la cadena de conexión para establecer cómo conectarse a la base de datos.

---

### **Ejemplo práctico de inyección de dependencias**
Supongamos que tienes un controlador llamado `ClientesController` que necesita acceso a la base de datos. En lugar de crear una instancia de `AppDbContext` manualmente, puedes pedirle al contenedor que lo haga por ti:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;

    // El contenedor de dependencias inyecta automáticamente el AppDbContext aquí
    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetClientes()
    {
        var clientes = await _context.Clientes.ToListAsync();
        return Ok(clientes);
    }
}
```

**Ventajas de esto:**
- No necesitas preocuparte por cómo se crea `AppDbContext` (ASP.NET Core lo hace por ti).
- El contenedor gestiona automáticamente el ciclo de vida de `AppDbContext`, asegurando que se liberen recursos correctamente.

---

### **Tipos de ciclo de vida en el contenedor de dependencias**
Cuando registras una clase en el contenedor, puedes definir su ciclo de vida:
1. **Transient** (Por defecto en `AddDbContext`):
   - Se crea una nueva instancia cada vez que se necesita.
2. **Scoped**:
   - Una instancia por cada solicitud HTTP.
3. **Singleton**:
   - Una única instancia para toda la aplicación.

En el caso de `AddDbContext`, el ciclo de vida **scoped** es ideal porque asegura que cada solicitud HTTP reciba una instancia nueva y aislada de `AppDbContext`.

---

Registrar tu clase en el contenedor es un paso esencial para aprovechar las capacidades de ASP.NET Core, promoviendo aplicaciones limpias, modulares y fáciles de mantener. 😊


### **Paso 6: Generar la base de datos**
**Contexto:** Usa las migraciones de EF Core para sincronizar los modelos con la base de datos.

1. Ejecuta los siguientes comandos en la consola de administración de paquetes:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

---

### **Paso 7: Crear controladores para cada entidad**
**Contexto:** Los controladores procesan las solicitudes HTTP y devuelven respuestas. Aquí implementarás la lógica CRUD.

1. Crea controladores para **Clientes**, **Productos** y **Facturas** utilizando la plantilla de controlador con EF Core.
   ```bash
   dotnet aspnet-codegenerator controller -name ClientesController -async -api -m Cliente -dc AppDbContext -outDir Controllers
   ```
2. Repite este comando para **Productos** y **Facturas**.

---

Si no puedes ejecutar el comando `dotnet aspnet-codegenerator` debido a restricciones de seguridad, puedes usar las herramientas integradas de Visual Studio para generar controladores en un proyecto ASP.NET Core. Aquí te dejo los pasos para hacerlo:

---

### 1. **Abrir tu proyecto en Visual Studio**
1. Abre Visual Studio y carga tu solución o proyecto.
2. Asegúrate de que tu proyecto esté compilando correctamente (`Ctrl + Shift + B`).

---

### 2. **Agregar un nuevo controlador usando el asistente**
1. Haz clic derecho en la carpeta **Controllers** en el Explorador de Soluciones.
2. Selecciona **Agregar** > **Nuevo elemento scaffold...**.
3. En el cuadro de diálogo que aparece:
   - Selecciona **API Controller with actions, using Entity Framework** (Controlador de API con acciones, usando Entity Framework).
   - Haz clic en **Agregar**.

---

### 3. **Configurar el controlador**
En la ventana de configuración que aparece:
1. Selecciona el modelo que quieres usar (en este caso, `Cliente`).
2. Selecciona el contexto de datos (`AppDbContext`).
3. Configura las opciones adicionales si es necesario, como el nombre del controlador (por defecto será `ClientesController`).
4. Haz clic en **Agregar** para que Visual Studio genere automáticamente el controlador.

---

### 4. **Revisar y ajustar el controlador**
Una vez generado, Visual Studio creará el archivo del controlador en la carpeta **Controllers**. Puedes revisarlo y personalizarlo según sea necesario.

---

### 5. **Compilar y probar**
- Compila tu proyecto para verificar que no haya errores.
- Ejecuta la aplicación para probar los endpoints generados.

---

### Notas adicionales
Si no ves la opción "Nuevo elemento scaffold", asegúrate de que tienes instalado el paquete **Microsoft.VisualStudio.Web.CodeGeneration.Design**:
1. Ve al **Administrador de paquetes NuGet** (haz clic derecho en el proyecto > **Administrar paquetes NuGet**).
2. Busca e instala el paquete **Microsoft.VisualStudio.Web.CodeGeneration.Design**.

---

### **Paso 8: Probar la API**
**Contexto:** Usa herramientas como Postman o Swagger para verificar que las rutas funcionan como se espera.

1. Ejecuta la aplicación (`Ctrl + F5`) y accede a Swagger en `https://localhost:5001/swagger`.
2. Prueba las rutas generadas automáticamente, como:
   - GET `/api/Clientes`
   - POST `/api/Productos`

---

### **Paso 9: Agregar validaciones y relaciones**
**Contexto:** Añade validaciones y ajusta las relaciones entre tablas según tu modelo.

1. Utiliza atributos como `[Required]`, `[StringLength]` en las propiedades del modelo.
2. Configura relaciones específicas en el `DbContext` usando `modelBuilder` si necesitas personalizar las claves foráneas o índices.

---

### **Paso 10: Documentación y publicación**
**Contexto:** Asegúrate de que la API esté bien documentada y lista para su despliegue.

1. Añade descripciones en Swagger utilizando comentarios XML.
2. Publica la aplicación en Azure, AWS, o cualquier servidor de tu preferencia.

---