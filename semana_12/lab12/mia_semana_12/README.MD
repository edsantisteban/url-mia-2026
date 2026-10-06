# MIA - Azure Blob Storage

## Objetivo de la aplicación

Esta aplicación de consola en C# permite administrar archivos utilizando **Azure Blob Storage**. El usuario puede subir, listar, descargar y eliminar archivos almacenados en un contenedor de Azure Blob Storage.

El proyecto tiene como objetivo demostrar la conexión de una aplicación C# con Azure Storage y el manejo de archivos mediante operaciones básicas sobre blobs.

## Tecnologías utilizadas

* **C#**
* **.NET**
* **Azure Blob Storage**
* **Azure.Storage.Blobs**
* **DotNetEnv**
* **Git y GitHub**
* **GitHub Secrets**

## Configuración de Azure

Para utilizar la aplicación se necesita una cuenta de **Azure Storage Account** con un contenedor de blobs.

En este proyecto se utiliza el contenedor:

```text
miaarchivos
```

La aplicación se conecta a Azure Blob Storage utilizando la Connection String de la cuenta de almacenamiento.

La Connection String **no se encuentra escrita directamente en el código fuente**.

## Arquitectura de la solución

La aplicación utiliza una arquitectura sencilla basada en una aplicación de consola.

```text
Usuario
   |
   v
Aplicación C#
   |
   v
Environment Variable
AZURE_STORAGE_CONNECTION_STRING
   |
   v
BlobServiceClient
   |
   v
BlobContainerClient
   |
   v
Azure Blob Storage
   |
   v
Container: miaarchivos
```

El programa utiliza `BlobServiceClient` para establecer la conexión con Azure Storage y `BlobContainerClient` para trabajar con el contenedor.

Las operaciones sobre los archivos se realizan mediante `BlobClient`.

## Operaciones disponibles

La aplicación presenta un menú con cuatro operaciones principales.

### 1. Subir archivo

Permite seleccionar la ruta de un archivo almacenado localmente y subirlo al contenedor `miaarchivos`.

Antes de realizar la operación se verifica que el archivo exista en la ruta indicada.

La aplicación utiliza:

```csharp
await blobClient.UploadAsync(ruta, overwrite: true);
```

Si ya existe un archivo con el mismo nombre, se reemplaza.

### 2. Listar archivos

Muestra en consola los archivos almacenados actualmente en el contenedor.

También muestra el tamaño de cada archivo en bytes.

La operación utiliza:

```csharp
await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
```

Si el contenedor está vacío, se muestra un mensaje indicando que no existen archivos.

### 3. Descargar archivo

Permite seleccionar un archivo almacenado en Azure y descargarlo a una carpeta local.

Antes de descargarlo se verifica que el blob exista mediante:

```csharp
await blobClient.ExistsAsync()
```

El usuario debe indicar la carpeta donde desea guardar el archivo.

### 4. Eliminar archivo

Permite eliminar un archivo almacenado en Azure.

Antes de eliminarlo:

1. Se verifica que el archivo exista.
2. Se solicita confirmación al usuario.
3. Si el usuario confirma, se realiza la eliminación.

La operación utiliza:

```csharp
await blobClient.DeleteIfExistsAsync();
```

## Manejo de errores

La aplicación realiza diferentes validaciones para evitar errores durante la ejecución.

Entre ellas:

* Verificación de que la Connection String exista.
* Verificación de que la ruta del archivo a subir sea válida.
* Verificación de que el archivo local exista.
* Verificación de que el nombre del blob no esté vacío.
* Verificación de que el archivo solicitado exista en Azure.
* Verificación de que la carpeta de destino sea válida.
* Confirmación antes de eliminar un archivo.
* Validación de las opciones seleccionadas en el menú.

Si la variable `AZURE_STORAGE_CONNECTION_STRING` no está configurada, la aplicación muestra un mensaje de error y finaliza.

## Protección de la Connection String

La Connection String de Azure Storage no se almacena directamente dentro de `Program.cs`.

En su lugar, se utiliza la variable de entorno:

```text
AZURE_STORAGE_CONNECTION_STRING
```

Para el entorno local se utiliza un archivo `.env`, el cual contiene la Connection String real.

Ejemplo:

```text
AZURE_STORAGE_CONNECTION_STRING=TU_CONNECTION_STRING
```

El programa carga las variables del archivo `.env` mediante `DotNetEnv`:

```csharp
Env.Load();

string? connectionString =
    Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
```

El archivo `.env` y otros archivos de configuración que contienen credenciales no deben publicarse en GitHub.

Por este motivo, `.env` y `.env.production` se encuentran incluidos en `.gitignore`.

Para GitHub Actions, la Connection String se almacena como un **Repository Secret** utilizando el nombre:

```text
AZURE_STORAGE_CONNECTION_STRING
```

De esta manera, la credencial no queda expuesta directamente en el código fuente.

## Instrucciones para ejecutar el proyecto

### Requisitos

Antes de ejecutar el proyecto se necesita tener instalado:

* .NET SDK
* Una cuenta de Azure Storage
* Una cuenta de GitHub, si se utilizará el repositorio remoto

### 1. Clonar el proyecto

Clonar el repositorio desde GitHub:

```bash
git clone URL_DEL_REPOSITORIO
```

Ingresar a la carpeta del proyecto:

```bash
cd MIA_AzureBlob_ES
```

### 2. Configurar la Connection String

Crear un archivo llamado:

```text
.env
```

En la raíz del proyecto.

Agregar:

```text
AZURE_STORAGE_CONNECTION_STRING=TU_CONNECTION_STRING
```

Reemplazar `TU_CONNECTION_STRING` por la Connection String correspondiente al Azure Storage Account.

### 3. Restaurar las dependencias

Ejecutar:

```bash
dotnet restore
```

### 4. Ejecutar la aplicación

Ejecutar:

```bash
dotnet run
```

### 5. Utilizar el menú

Al iniciar la aplicación aparecerá:

```text
=================================
     MIA - AZURE BLOB STORAGE
=================================
1. Subir archivo
2. Listar archivos
3. Descargar archivo
4. Eliminar archivo
5. Salir
=================================
```

Seleccionar la opción deseada y seguir las instrucciones mostradas en la consola.

## Seguridad

Por seguridad, nunca se debe publicar la Connection String ni la Account Key de Azure Storage en GitHub.

Los archivos que contienen credenciales deben permanecer fuera del repositorio mediante `.gitignore` y las credenciales utilizadas por GitHub deben almacenarse mediante **GitHub Secrets**.
