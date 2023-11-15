
namespace Contraseña
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Diagnostics;
    using System.Threading;
    public static class Funciones
    {
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

            try
            {
                string path = @"D:\Programacion\Segundo\C#\Contraseña\2151220-passwords.txt"; // Ruta del archivo a leer

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
                    division = numeroLineas / 8;
                    int parte2 = division * 2;
                    int parte3 = division * 3;
                    int parte4 = division * 4;
                    int parte5 = division * 5;
                    int parte6 = division * 6;   
                    int parte7 = division * 7;
                    
                    Thread hilo1 = new Thread(() => threadPassword(0, division, path, newPass));
                    hilo1.Start();
                    Thread hilo2 = new Thread(() => threadPassword(division, parte2, path, newPass));
                    hilo2.Start();
                    Thread hilo3 = new Thread(() => threadPassword(parte2, parte3, path, newPass));
                    hilo3.Start();
                    Thread hilo4 = new Thread(() => threadPassword(parte3, parte4, path, newPass));
                    hilo4.Start();
                    Thread hilo5 = new Thread(() => threadPassword(parte4, parte5, path, newPass));
                    hilo5.Start();
                    Thread hilo6 = new Thread(() => threadPassword(parte5, parte6, path, newPass));
                    hilo6.Start();
                    Thread hilo7 = new Thread(() => threadPassword(parte6, parte7, path, newPass));
                    hilo7.Start();
                    Thread hilo8 = new Thread(() => threadPassword(parte7, numeroLineas, path, newPass));
                    hilo8.Start();


                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ocurrió un error al leer el archivo: " + e.Message);
            }


            void threadPassword(int start, int finish, string path, string newpass)
            {
                sw.Start();
                using (StreamReader sr2 = new StreamReader(path))
                {
                    int numeroLineaActual = 0;
                    string newPasst = newpass;
                    string linea2;
                    string newPass2;
                    while ((linea2 = sr2.ReadLine()) != null) // recorremos el archivo línea a línea de nuevo
                    {
                        if (numeroLineaActual.inRange(start, finish)) 
                        {
                            byte[] byteLinea2 = Encoding.UTF8.GetBytes(linea2);
                            using (SHA256 sha256 = SHA256.Create())
                            {
                                byte[] hasBytes2 = sha256.ComputeHash(byteLinea2);
                                StringBuilder sb2 = new StringBuilder();
                                for (int i = 0; i < hasBytes2.Length; i++)
                                {
                                    sb2.Append(hasBytes2[i].ToString("x2"));
                                }
                                newPass2 = sb2.ToString();
                                if (newPasst == newPass2)
                                {
                                    sw.Stop();
                                    Console.WriteLine("La contraseña: ");
                                    Console.WriteLine(newPass2);
                                    Console.WriteLine("y la contraseña: ");
                                    Console.WriteLine(newPasst);
                                    Console.WriteLine("coinciden, la contraseña ha sido hackeada! ");                                  
                                    Console.WriteLine(sw.Elapsed.ToString());
                                    break;
                                }                              
                            }
                        }
                        numeroLineaActual++;
                    }
                }
            }
        }
    }     
}
