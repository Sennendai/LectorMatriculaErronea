using System;
using System.Collections.Generic;
using System.Linq;

namespace LectorMatriculaErronea
{
    class Program
    {
        private static List<Matricula> Matriculas;
        private static string matriculaEncontrada = string.Empty;
        private static List<Matricula> PosiblesMatriculas = new List<Matricula>();
        private static char[] palabraPorPasos;
        private static int[,] matriz;

        static void Main(string[] args)
        {
            CrearMatriculas();
            Console.WriteLine("Introduce tu matricula: ");
            string input = Console.ReadLine();

            ComprobarMatricula(input);

            if (!string.IsNullOrEmpty(matriculaEncontrada))
            {
                Console.WriteLine("Matricula encontrada: " + matriculaEncontrada);

                matriculaEncontrada = string.Empty;
                Main(null);
            }

            ComprobarPosiblesMatricula(input);
            if (PosiblesMatriculas.Count() != 0)
            {
                Console.WriteLine("Posibles matricula encontradas: ");
                foreach (var item in PosiblesMatriculas)
                {
                    Console.WriteLine(item.matricula);
                }
            }
            else
                Console.WriteLine("Dios te ayude.");

            PosiblesMatriculas = new List<Matricula>();
            Main(null);
        }

        private static void ComprobarMatricula(string matricula)
        {
            foreach (var item in Matriculas)
            {
                if (item.matricula.Equals(matricula))
                {
                    matriculaEncontrada = item.matricula;
                }
            }
        }

        private static void ComprobarPosiblesMatricula(string matricula)
        {
            foreach (var item in Matriculas)
            {
                int fallos = DamerauLevenshteinDistance(item.matricula, matricula);
                if (fallos <= 1)
                    PosiblesMatriculas.Add(new Matricula { matricula = item.matricula });
            }
        }

        private static int DamerauLevenshteinDistance(string matriculaLista, string matriculaInput)
        {
            if (string.IsNullOrEmpty(matriculaLista) || string.IsNullOrEmpty(matriculaInput))
                throw new ArgumentNullException("Esto esta vacio");

            //matriculaLista = "elenco";
            //matriculaInput = "ejemplo"; 

            int longitudMatrInput = matriculaInput.Length;
            int longitudMatrLista = matriculaLista.Length;

            /*********** Distancia Levestein **********/
            // R|I
            // D|[i,j]
            // Replace Insert Delete, se coje la distancia mas pequeña +1 si se realiza una accion de insert/delete
            // Si se reemplaza se coje en diagonal y +0

            //  _|_|e|j|e|m|p|l|o            
            //  _ 0|1|2|3|4|5|6|7
            //  e 1|0|1|2|3|4|5|6
            //  l 2|1|1|2|3|4|4|5
            //  e 3|2|2|1|2|3|4|5
            //  n 4|3|3|2|2|3|4|5
            //  c 5|4|4|3|3|3|4|5
            //  o 6|5|5|4|4|4|4|4 <-- el ultimo valor son los minimos pasos necesarios para pasar de 'ejemplo' a 'elenco'

            int[,] distancias = new int[longitudMatrInput + 1, longitudMatrLista + 1];

            for (int i = 0; i <= longitudMatrInput; distancias[i, 0] = i++) ;
            for (int j = 0; j <= longitudMatrLista; distancias[0, j] = j++) ;

            for (int i = 1; i <= longitudMatrInput; i++)
            {
                for (int j = 1; j <= longitudMatrLista; j++)
                {
                    // diferencia a sumar 0 si es igual
                    int diferencia = matriculaLista[j - 1] == matriculaInput[i - 1] ? 0 : 1;
                    // Insert, Delete
                    // Replace
                    distancias[i, j] = Math.Min(Math.Min(distancias[i - 1, j] + 1, distancias[i, j - 1] + 1),
                                                         distancias[i - 1, j - 1] + diferencia);

                    //if (i == longitudMatrInput && j == longitudMatrLista)
                    //{
                    //    Console.WriteLine(matriculaLista);
                    //    palabraPorPasos = matriculaLista.ToCharArray();
                    //    matriz = distancias;
                    //    AlgWagnerFisher(longitudMatrInput, longitudMatrLista, matriculaLista, matriculaInput);
                    //}
                }
            }

            return distancias[longitudMatrInput, longitudMatrLista];
        }

        private static void AlgWagnerFisher(int filas, int columnas, string original, string input)
        {
            if (filas < 0) filas = 0;
            if (columnas < 0) columnas = 0;

            int diagonal = 0;

            int posicion = matriz[filas, columnas];
            if (posicion != 0)
            {
                if (filas!=0 && columnas!=0) diagonal = matriz[filas - 1, columnas - 1];

                if (posicion == diagonal && diagonal != 0)
                {
                    AlgWagnerFisher(filas - 1, columnas - 1, original, input);
                }
                else if (filas != 0 && columnas != 0 && diagonal != 0)
                {
                    if (columnas == 0)
                        MoverArriba(filas, columnas, original, input, posicion);
                    else if (filas == 0)
                        MoverIzquierda(filas, columnas, original, input, posicion);
                    else
                    {
                        int izquierda = matriz[filas, columnas - 1];
                        int arriba = matriz[filas - 1, columnas];

                        int decididor = Math.Min(Math.Min(izquierda, arriba), diagonal);
                        if (decididor == izquierda)
                            MoverIzquierda(filas, columnas, original, input, posicion);
                        else if (decididor == arriba)
                            MoverArriba(filas, columnas, original, input, posicion);
                        else if (decididor == diagonal)
                            MoverDiagonal(filas, columnas, original, input, posicion);
                    }
                }
            }
        }

        private static void MoverArriba(int filas, int columnas, string original, string input, int posicion)
        {
            palabraPorPasos = new string(palabraPorPasos).Insert(posicion, input[posicion].ToString()).ToCharArray();
            Console.WriteLine(new string(palabraPorPasos));

            AlgWagnerFisher(filas - 1, columnas, original, input);
        }

        private static void MoverIzquierda(int filas, int columnas, string original, string input, int posicion)
        {
            palabraPorPasos = new string(palabraPorPasos).Remove(posicion, 1).ToCharArray();
            Console.WriteLine(new string(palabraPorPasos));

            AlgWagnerFisher(filas, columnas - 1, original, input);
        }

        private static void MoverDiagonal(int filas, int columnas, string original, string input, int posicion)
        {
            palabraPorPasos[columnas - 1] = input[filas - 1];
            Console.WriteLine(new string(palabraPorPasos));

            AlgWagnerFisher(filas - 1, columnas - 1, original, input);
        }

        private static void CrearMatriculas()
        {
            Matriculas = new List<Matricula>
            {
                new Matricula{ matricula= "HDU376" },
                new Matricula{ matricula= "XIC716" },
                new Matricula{ matricula= "ABC123" }
            };
        }
    }

    public class Matricula
    {
        public string matricula;
    }
}
