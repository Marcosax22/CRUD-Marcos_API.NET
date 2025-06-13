//Marcos Ariel 2024-1785
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;


namespace GestionReciclaje
{
    public class Material
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Material> Materiales { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
    }

    public class MaterialService
    {
        private readonly ApplicationDbContext _context;

        public MaterialService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddMaterial(string nombre, string tipo)
        {
            var material = new Material { Nombre = nombre, Tipo = tipo };
            _context.Materiales.Add(material);
            _context.SaveChanges();
            Console.WriteLine("\n[OK] Material agregado correctamente.\n");
        }

        public void ListMaterials()
        {
            var materiales = _context.Materiales.ToList();
            if (!materiales.Any())
            {
                Console.WriteLine("\n[!] No hay materiales registrados.\n");
                return;
            }

            Console.WriteLine("\n--- Lista de materiales ---\n");
            foreach (var m in materiales)
            {
                Console.WriteLine($"ID: {m.Id} | Nombre: {m.Nombre} | Tipo: {m.Tipo}");
            }
            Console.WriteLine();
        }

        public void DeleteMaterial(int id)
        {
            var material = _context.Materiales.Find(id);
            if (material != null)
            {
                _context.Materiales.Remove(material);
                _context.SaveChanges();
                Console.WriteLine("\n[OK] Material eliminado correctamente.\n");
            }
            else
            {
                Console.WriteLine("\n[ERROR] Material no encontrado.\n");
            }
        }

        public void UpdateMaterial(int id, string nuevoNombre, string nuevoTipo)
        {
            var material = _context.Materiales.Find(id);
            if (material != null)
            {
                material.Nombre = nuevoNombre;
                material.Tipo = nuevoTipo;
                _context.SaveChanges();
                Console.WriteLine("\n[OK] Material actualizado correctamente.\n");
            }
            else
            {
                Console.WriteLine("\n[ERROR] Material no encontrado.\n");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlite("Data Source=reciclaje.db");

            using (var context = new ApplicationDbContext(optionsBuilder.Options))
            {
                context.Database.EnsureCreated();
                var service = new MaterialService(context);

                while (true)
                {
                    MostrarMenu();

                    Console.Write("Elige una opción: ");
                    var opcion = Console.ReadLine();

                    switch (opcion)
                    {
                        case "1":
                            Console.Write("Nombre del material: ");
                            var nombre = Console.ReadLine();
                            Console.Write("Tipo del material: ");
                            var tipo = Console.ReadLine();
                            service.AddMaterial(nombre!, tipo!);
                            break;

                        case "2":
                            service.ListMaterials();
                            break;

                        case "3":
                            Console.Write("ID del material a eliminar: ");
                            if (int.TryParse(Console.ReadLine(), out int idEliminar))
                                service.DeleteMaterial(idEliminar);
                            else
                                MostrarError("ID inválido.");
                            break;

                        case "4":
                            Console.Write("ID del material a actualizar: ");
                            if (int.TryParse(Console.ReadLine(), out int idActualizar))
                            {
                                Console.Write("Nuevo nombre: ");
                                var nuevoNombre = Console.ReadLine();
                                Console.Write("Nuevo tipo: ");
                                var nuevoTipo = Console.ReadLine();
                                service.UpdateMaterial(idActualizar, nuevoNombre!, nuevoTipo!);
                            }
                            else
                            {
                                MostrarError("ID inválido.");
                            }
                            break;

                        case "5":
                            Console.WriteLine("\nGracias por usar el sistema de gestión de reciclaje.\n");
                            return;

                        default:
                            MostrarError("Opción no válida.");
                            break;
                    }
                }
            }
        }

        static void MostrarMenu()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("|   SISTEMA DE GESTIÓN DE MATERIALES    |");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("1. Agregar material");
            Console.WriteLine("2. Listar materiales");
            Console.WriteLine("3. Eliminar material");
            Console.WriteLine("4. Actualizar material");
            Console.WriteLine("5. Salir");
            Console.WriteLine("----------------------------------------");
        }

        static void MostrarError(string mensaje)
        {
            Console.WriteLine($"\n[ERROR] {mensaje}\n");
        }
    }
}