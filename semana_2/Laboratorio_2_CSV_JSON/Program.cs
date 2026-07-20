using System.Text.Json;

List<Estudiante> estudiantes = new List<Estudiante>();
string[] lineas = File.ReadAllLines("estudiantes.csv");
for (int i = 1; i < lineas.Length; i++)
{
    string[] datos = lineas[i].Split(',');
    Estudiante estudiante = new Estudiante
    {
        Id = int.Parse(datos[0]),Nombre = datos[1],Carrera = datos[2]
    };
    estudiantes.Add(estudiante);

}
foreach (Estudiante estudiante in estudiantes)
{
    Console.WriteLine($"{estudiante.Id} - {estudiante.Nombre} - {estudiante.Carrera}");
}
string json = JsonSerializer.Serialize(estudiantes, new JsonSerializerOptions
{
    WriteIndented = true
});
File.WriteAllText("estudiantes.json", json);

Console.WriteLine("Archivo estudiantes.json creado correctamente.");