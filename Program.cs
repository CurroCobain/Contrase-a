namespace Contraseña
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Diagnostics;
    using System.Threading;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    //Usamos esta clase para crear las funciones que vamos a usar en el programa
    public static class Funciones
    {
        // Devuelve si un valor tipo int dado está entre dos números concretos
        public static Boolean inRange(this int valor, int valMin, int valMax)
        {
            return valor >= valMin && valor <= valMax;
        }
    }

    class Program
    {
        static void Main()
        {
            Random rnd = new Random();
            int line = 0; // variable para contar el número de línea del archivo por la que vamos
            String pass = ""; // variable para encontrar una contraseña
            String newPass = "";//variable para asignar la contraseña encriptada
            Stopwatch sw = new Stopwatch();
            int division = 0;
            bool encontrado = false;

            try
            {
                string path = @"..\..\2151220-passwords.txt"; // Ruta del archivo a leer

                // Lee el archivo línea por línea
                using (StreamReader sr = new StreamReader(path))
                {
                    int numeroLineas = File.ReadLines(path).Count(); // contamos el número total de líneas del archivo 
                    division = numeroLineas / 2;
                    int numLine = rnd.Next(0, numeroLineas + 1); // generamos un número aleatorio entre 0 y el número total de líneas del archivo
                    string linea;
                    Console.WriteLine("Contenido del archivo línea por línea:");
                    while ((linea = sr.ReadLine()) != null) // recorremos el archivo línea a línea
                    {
                        line++;
                        if (numLine == line)
                        {
                            pass = linea; // si el número de línea coincide con el número aleatorio asignamos el valor de la linea a la contraseña
                            byte[] bytesContraseña = Encoding.UTF8.GetBytes(pass); // obtenemos la lista de bytes de la contraseña
                            Console.WriteLine(pass);
                            // encriptamos la contraseña usando SHA256
                            using (SHA256 sha256 = SHA256.Create())
                            {
                                byte[] hasBytes = sha256.ComputeHash(bytesContraseña);
                                StringBuilder sb = new StringBuilder();
                                for (int i = 0; i < hasBytes.Length; i++)
                                {
                                    sb.Append(hasBytes[i].ToString("x2"));
                                }
                                newPass = sb.ToString(); // este es el nuevo valor de la contraseña encriptado con SHA256
                            }
                        }
                    }
                    Console.WriteLine(newPass);
                    List<string> listArchivo = archivoLista();
                    // Creamos 8 hilos normales
                    for (int i = 0; i <= 8; i++)
                    {
                        int parte = numeroLineas / 8;
                        Thread hilo = new Thread(() => encontrado = threadPassword(parte * i, parte * (i + 1), path, newPass,listArchivo));
                        hilo.Start();
                    }

                    for (int x = 8; x >= 0; x--)
                    {
                        int parte = numeroLineas / 8;
                        Thread hilo = new Thread(() => encontrado = threadPasswordInverso(parte *(x - 1), parte * x,path, newPass, listArchivo ));
                        hilo.Start();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ocurrió un error al leer el archivo: " + e.Message);
            }

            // Código de cada hilo: Recorre una parte concreta del archivo y va codificando y comparando con una cadena dada
            bool threadPassword(int start, int finish, string path, string newpass, List<String> lineas)
            {
                string newPass2 = "";
                bool result = false;
                sw.Start();
                // Recorre las líneas y procesa las que están en el rango deseado
                for (int i = 0; i <= lineas.Count; i++)
                {
                    if (i >= start && i + 1<= finish)
                    {
                        // Procesa la línea actual
                        string linea2 = lineas[i];
                        byte[] byteLinea2 = Encoding.UTF8.GetBytes(linea2);
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            //Codificamos la línea actual
                            byte[] hasBytes2 = sha256.ComputeHash(byteLinea2);
                            StringBuilder sb2 = new StringBuilder();
                            for (int x = 0; x < hasBytes2.Length; x++)
                            {
                                sb2.Append(hasBytes2[x].ToString("x2"));
                            }
                            newPass2 = sb2.ToString();
                            //Comparamos la línea actual codificada con la contraseña dada
                            if (newPass == newPass2)
                            {
                                sw.Stop();
                                Console.WriteLine("Hackeado por hilo normal");
                                Console.WriteLine("La contraseña es " + newPass2);
                                Console.WriteLine(sw.Elapsed.ToString());
                                result = true;
                                return result;
                            }
                        }
                    }
                }
                return result;
            }
            // Código de los hilos inversos: Recorre una parte concreta  del archivo en orden inverso al normal
            bool threadPasswordInverso(int start, int finish, string path, string newpass, List<String> lineas)
            {
                string newPass2 = "";
                bool result = false;
                sw.Start();
                // Recorre las líneas en orden inverso y procesa las que están en el rango deseado
                for (int i = lineas.Count - 1; i >= 0; i--)
                {
                    if (i + 1 >= start && i + 1 <= finish)
                    {
                        // Procesa la línea actual
                        string linea2 = lineas[i];
                        byte[] byteLinea2 = Encoding.UTF8.GetBytes(linea2);
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            //Codificamos la línea actual
                            byte[] hasBytes2 = sha256.ComputeHash(byteLinea2);
                            StringBuilder sb2 = new StringBuilder();
                            for (int x = 0; x < hasBytes2.Length; x++)
                            {
                                sb2.Append(hasBytes2[x].ToString("x2"));
                            }
                            newPass2 = sb2.ToString();
                            //Comparamos la línea actual codificada con la contraseña dada
                            if (newPass == newPass2)
                            {
                                sw.Stop();
                                Console.WriteLine("Hackeado por hilo inverso");
                                Console.WriteLine("La contraseña es " +newPass2);
                                Console.WriteLine(sw.Elapsed.ToString());
                                result = true;
                                return result;
                            }
                        }
                    }
                }
                return result;
            }

            List<String> archivoLista() //Convertimos el archivo en una lista
            {
                using (StreamReader sr2 = new StreamReader(@"..\..\2151220-passwords.txt"))
                {
                    // Lee todas las líneas del archivo
                    List<string> lineas = new List<string>();
                    while (!sr2.EndOfStream)
                    {
                        lineas.Add(sr2.ReadLine());
                    }
                    return lineas;
                }
            }
        }
    }
}
/* ------------------------------------------------------- Código viejo no refactorizado --------------------------------------------------
 * division = numeroLineas / 6;
                    int parte2 = division * 2;
                    int parte3 = division * 3;
                    int parte4 = division * 4;
                    int parte5 = division * 5;
                    

                    //Primer hilo
                    Thread hilo1 = new Thread(() => encontrado = threadPassword(0, division, path, newPass));
                    hilo1.Start();
                    //Segundo hilo
                    Thread hilo2 = new Thread(() => encontrado = threadPassword(division, parte2, path, newPass));
                    hilo2.Start();
                    //Tercer hilo
                    Thread hilo3 = new Thread(() => encontrado = threadPassword(parte2, parte3, path, newPass));
                    hilo3.Start();
                    //Cuarto hilo
                    Thread hilo4 = new Thread(() => encontrado = threadPassword(parte3, parte4, path, newPass));
                    hilo4.Start();
                    Thread hilo5 = new Thread(() => encontrado = threadPassword(parte4, parte5, path, newPass));
                    hilo5.Start();
                    Thread hilo6 = new Thread(() => encontrado = threadPassword(parte5, numeroLineas, path, newPass));
                    hilo6.Start();

                    List<string> listArchivo = archivoLista();

                    // Hilos inversos
                    Thread hilo7 = new Thread(() => encontrado = threadPasswordInverso(parte5, numeroLineas, path, newPass, listArchivo));
                    hilo7.Start();
                    Thread hilo8 = new Thread(() => encontrado = threadPasswordInverso(parte4, parte5, path, newPass, listArchivo));
                    hilo8.Start();
                    Thread hilo9 = new Thread(() => encontrado = threadPasswordInverso(parte3, parte4, path, newPass, listArchivo));
                    hilo9.Start();
                    Thread hilo10 = new Thread(() => encontrado = threadPasswordInverso(parte2, parte3, path, newPass, listArchivo));
                    hilo10.Start();
                    Thread hilo11 = new Thread(() => encontrado = threadPasswordInverso(division, parte2, path, newPass, listArchivo));
                    hilo11.Start();
                    Thread hilo12 = new Thread(() => encontrado = threadPasswordInverso(0, division, path, newPass, listArchivo));
                    hilo12.Start();
 */
