using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DotNetEnv;

Env.Load();

string? connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("ERROR: No se encontró la variable AZURE_STORAGE_CONNECTION_STRING.");
    return;
}

string containerName = "miaarchivos";

BlobServiceClient blobServiceClient =
    new BlobServiceClient(connectionString);

BlobContainerClient containerClient =
    blobServiceClient.GetBlobContainerClient(containerName);

await containerClient.CreateIfNotExistsAsync();

bool salir = false;

while (!salir)
{
    Console.WriteLine();
    Console.WriteLine("=================================");
    Console.WriteLine("     MIA - AZURE BLOB STORAGE");
    Console.WriteLine("=================================");
    Console.WriteLine("1. Subir archivo");
    Console.WriteLine("2. Listar archivos");
    Console.WriteLine("3. Descargar archivo");
    Console.WriteLine("4. Eliminar archivo");
    Console.WriteLine("5. Salir");
    Console.WriteLine("=================================");
    Console.Write("Selecciona una opción: ");
    string? opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await SubirArchivo(containerClient);
            break;
        case "2":
            await ListarArchivos(containerClient);
            break;
        case "3":
            await DescargarArchivo(containerClient);
            break;
        case "4":
            await EliminarArchivo(containerClient);
            break;
        case "5":
            salir = true;
            Console.WriteLine("Saliendo...");
            break;
        default:
            Console.WriteLine("Opción no válida. Intenta de nuevo.");
            break;
    }
}

// 3.2 Subir archivo
async Task SubirArchivo(BlobContainerClient containerClient)
{
    Console.Write("\nRuta del archivo local a subir: ");
    string? ruta = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(ruta) || !File.Exists(ruta))
    {
        Console.WriteLine("ERROR: El archivo no existe en la ruta indicada.");
        return;
    }

    string nombreArchivo = Path.GetFileName(ruta);
    BlobClient blobClient = containerClient.GetBlobClient(nombreArchivo);

    await blobClient.UploadAsync(ruta, overwrite: true);

    Console.WriteLine($"Archivo '{nombreArchivo}' subido correctamente a Azure Blob Storage.");
}

// 3.3 Listar archivos
async Task ListarArchivos(BlobContainerClient containerClient)
{
    Console.WriteLine();
    Console.WriteLine("Nombre                          Tamaño");
    Console.WriteLine("--------------------------------------------");

    bool hayArchivos = false;

    await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
    {
        hayArchivos = true;
        long tamano = blobItem.Properties.ContentLength ?? 0;
        Console.WriteLine($"{blobItem.Name,-32} {tamano} bytes");
    }

    if (!hayArchivos)
        Console.WriteLine("(el container está vacío)");
}

// 3.4 Descargar archivo
async Task DescargarArchivo(BlobContainerClient containerClient)
{
    Console.Write("\nNombre del archivo (blob) a descargar: ");
    string? nombreBlob = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(nombreBlob))
    {
        Console.WriteLine("ERROR: Debes indicar un nombre de archivo.");
        return;
    }

    BlobClient blobClient = containerClient.GetBlobClient(nombreBlob);

    if (!await blobClient.ExistsAsync())
    {
        Console.WriteLine($"ERROR: El archivo '{nombreBlob}' no existe en el container.");
        return;
    }

    Console.Write("Carpeta de destino (ej. C:\\descargas): ");
    string? carpetaDestino = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(carpetaDestino))
    {
        Console.WriteLine("ERROR: Debes indicar una carpeta de destino.");
        return;
    }

    if (!Directory.Exists(carpetaDestino))
        Directory.CreateDirectory(carpetaDestino);

    string rutaDestino = Path.Combine(carpetaDestino, nombreBlob);
    await blobClient.DownloadToAsync(rutaDestino);

    Console.WriteLine($"Archivo descargado correctamente en: {rutaDestino}");
}

// 3.5 Eliminar archivo
async Task EliminarArchivo(BlobContainerClient containerClient)
{
    Console.Write("\nNombre del archivo (blob) a eliminar: ");
    string? nombreBlob = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(nombreBlob))
    {
        Console.WriteLine("ERROR: Debes indicar un nombre de archivo.");
        return;
    }

    BlobClient blobClient = containerClient.GetBlobClient(nombreBlob);

    if (!await blobClient.ExistsAsync())
    {
        Console.WriteLine($"ERROR: El archivo '{nombreBlob}' no existe en el container.");
        return;
    }

    Console.Write($"¿Confirmas que deseas eliminar '{nombreBlob}'? (s/n): ");
    string? confirmacion = Console.ReadLine();

    if (confirmacion?.Trim().ToLower() != "s")
    {
        Console.WriteLine("Operación cancelada.");
        return;
    }

    bool eliminado = await blobClient.DeleteIfExistsAsync();

    Console.WriteLine(eliminado
        ? $"Archivo '{nombreBlob}' eliminado correctamente."
        : $"No se pudo eliminar el archivo '{nombreBlob}'.");
}