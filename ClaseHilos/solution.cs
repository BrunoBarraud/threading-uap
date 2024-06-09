using System;
using System.Collections.Generic;
using System.Threading;

namespace ClaseHilos
{
    internal class Producto
    {
        public string Nombre { get; set; }
        public decimal PrecioUnitarioDolares { get; set; }
        public int CantidadEnStock { get; set; }

        public Producto(string nombre, decimal precioUnitario, int cantidadEnStock)
        {
            Nombre = nombre;
            PrecioUnitarioDolares = precioUnitario;
            CantidadEnStock = cantidadEnStock;
        }
    }

    internal class Solution
    {
        static List<Producto> productos = new List<Producto>
        {
            new Producto("Camisa", 10, 50),
            new Producto("Pantalón", 8, 30),
            new Producto("Zapatilla/Champión", 7, 20),
            new Producto("Campera", 25, 100),
            new Producto("Gorra", 16, 10)
        };

        static int precio_dolar = 500;
        static readonly object lockObject = new object();
        static ManualResetEvent tarea1Completed = new ManualResetEvent(false);
        static ManualResetEvent tarea2Completed = new ManualResetEvent(false);
        static ManualResetEvent tarea4Completed = new ManualResetEvent(false);

        static void Tarea1()
        {
            lock (lockObject)
            {
                foreach (var producto in productos)
                {
                    producto.CantidadEnStock += 10;
                }
                tarea1Completed.Set();
            }
        }

        static void Tarea2()
        {
            lock (lockObject)
            {
                precio_dolar = 520; //suponemos un precio dolar de 520
                tarea2Completed.Set();
            }
        }

        static void Tarea4()
        {
            lock (lockObject)
            {
                foreach (var producto in productos)
                {
                    producto.PrecioUnitarioDolares *= 1.10m; // aumentamos un 10%
                }
                tarea4Completed.Set();
            }
        }

        static void Tarea3()
        {
            // esperamos que se completen las primera 2
            WaitHandle.WaitAll(new WaitHandle[] { tarea1Completed, tarea2Completed, tarea4Completed });

            lock (lockObject)
            {
                decimal totalInventario = 0;
                Console.WriteLine("Informe de productos:");
                foreach (var producto in productos)
                {
                    decimal precioEnPesos = producto.PrecioUnitarioDolares * precio_dolar;
                    totalInventario += precioEnPesos * producto.CantidadEnStock;
                    Console.WriteLine($"Producto: {producto.Nombre}, Cantidad: {producto.CantidadEnStock}, Precio Unitario en Pesos: {precioEnPesos}");
                }
                Console.WriteLine($"Precio total del inventario en pesos: {totalInventario}");
            }
        }

        internal static void Excecute()
        {
            Thread thread1 = new Thread(Tarea1);
            Thread thread2 = new Thread(Tarea2);
            Thread thread3 = new Thread(Tarea3);
            Thread thread4 = new Thread(Tarea4);

            thread1.Start();
            thread2.Start();
            thread4.Start();
            thread3.Start();
            thread1.Join();
            thread2.Join();
            thread4.Join();
            thread3.Join();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Solution.Excecute();
        }
    }
}
