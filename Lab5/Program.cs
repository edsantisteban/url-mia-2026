using System;
using System.IO;

namespace MIA_Lab5
{
    class Program
    {
        static void Main(string[] args)
        {
            // ===================== ENTRADAS =====================
            // Se solicita el nombre completo del usuario
            Console.Write("Ingrese su nombre completo: ");
            string nombreCompleto = Console.ReadLine();

            // Se reemplaza el espacio por guion bajo para usarlo en rutas y nombres de archivo
            string nombreArchivo = nombreCompleto.Trim().Replace(" ", "_");

            // Ruta fija donde se ubica el archivo de texto de entrada
            string carpeta = @"C:\MIA_Lab_5\project";
            string rutaArchivo = Path.Combine(carpeta, nombreArchivo + ".txt");

            Console.WriteLine("Usuario: " + nombreCompleto);
            Console.WriteLine("Archivo: " + rutaArchivo);

            // Validar que el archivo exista antes de continuar
            if (!File.Exists(rutaArchivo))
            {
                Console.WriteLine("El archivo no existe en la ruta indicada.");
                return;
            }

            // ===================== PROCESO =====================
            // Variables acumuladoras para el conteo manual
            int lineas = 0;
            int palabras = 0;
            int caracteres = 0;

            // Abrir el archivo en modo lectura
            StreamReader lector = new StreamReader(rutaArchivo);

            string lineaActual;
            // Leer el archivo línea por línea hasta llegar al final
            while ((lineaActual = lector.ReadLine()) != null)
            {
                lineas++; // se encontró una línea nueva

                // Contar caracteres de la línea (uno por uno, sin usar Length de forma indirecta a otra función)
                for (int i = 0; i < lineaActual.Length; i++)
                {
                    caracteres++;
                }

                // Contar palabras: se recorre carácter por carácter detectando espacios
                bool dentroDePalabra = false;
                for (int i = 0; i < lineaActual.Length; i++)
                {
                    char c = lineaActual[i];

                    if (c != ' ' && c != '\t')
                    {
                        if (!dentroDePalabra)
                        {
                            palabras++;
                            dentroDePalabra = true;
                        }
                    }
                    else
                    {
                        dentroDePalabra = false;
                    }
                }
            }

            // Cerrar el archivo tras terminar la lectura
            lector.Close();

            // ===================== SALIDAS =====================
            // Mostrar resultados en pantalla
            Console.WriteLine("El archivo contiene: " + lineas + " líneas, " + palabras +
                               " palabras, " + caracteres + " caracteres.");

            // Guardar los resultados en un archivo CSV dentro de la misma carpeta
            string rutaCsv = Path.Combine(carpeta, "resultados_" + nombreArchivo + ".csv");
            StreamWriter escritor = new StreamWriter(rutaCsv);

            // Formato: <Nombre_Apellido>,Lineas,Palabras,Caracteres
            escritor.WriteLine(nombreArchivo + "," + lineas + "," + palabras + "," + caracteres);
            escritor.Close();

            Console.WriteLine("Resultados guardados en " + rutaCsv);
        }
    }
}
 
