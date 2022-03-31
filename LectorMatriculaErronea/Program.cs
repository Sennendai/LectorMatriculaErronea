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
        static void Main(string[] args)
        {
            CrearMatriculas();
            Console.WriteLine("Introduce tu matricula: ");
            string input = Console.ReadLine();

            ComprobarMatricula(input);

            if (!string.IsNullOrEmpty(matriculaEncontrada))
                Console.WriteLine("Matricula encontrada: " + matriculaEncontrada);
            else if (PosiblesMatriculas.Count() != 0)
            {
                Console.WriteLine("Posibles matricula encontradas: ");
                foreach (var item in PosiblesMatriculas)
                {
                    Console.WriteLine(item.matricula);
                }
            }
            else
                Console.WriteLine("Dios te ayude.");
        }

        private static void ComprobarMatricula(string matricula)
        {
            foreach (var item in Matriculas)
            {
                if (item.matricula.Equals(matricula))
                {
                    matriculaEncontrada = item.matricula;
                    return;
                }
            }

            foreach (var item in Matriculas)
            {
                int fallos = DamerauLevenshteinDistance(item.matricula, matricula);
                if (fallos <= 1) PosiblesMatriculas.Add(new Matricula { matricula = item.matricula });
            }
        }

        private static int DamerauLevenshteinDistance(string matriculaLista, string matriculaInput)
        {
            if (string.IsNullOrEmpty(matriculaLista) || string.IsNullOrEmpty(matriculaInput))
                throw new ArgumentNullException("Esto esta vacio");

            int longitudMatrInput = matriculaInput.Length;
            int longitudMatrLista = matriculaLista.Length;

            // + 1 por que se usara la posicion 0 para guardar info de posicion
            int[,] distancias = new int[longitudMatrInput + 1, longitudMatrLista + 1];

            for (int i = 0; i <= longitudMatrInput; distancias[i, 0] = i++) ;
            for (int j = 0; j <= longitudMatrLista; distancias[0, j] = j++) ;
                        
            for (int i = 1; i <= longitudMatrInput; i++)
            {
                for (int j = 1; j <= longitudMatrLista; j++)
                {
                    // diferencia a sumar 0 si es igual
                    int diferencia = matriculaLista[j - 1] == matriculaInput[i - 1] ? 0 : 1;

                    distancias[i, j] = Math.Min(Math.Min(distancias[i - 1, j], distancias[i, j - 1] + 1),
                                                        distancias[i - 1, j - 1] + diferencia);
                }
            }

            // Se suma la diferencia de longitud de los caracteres por que no se tiene en cuenta si el input es mas largo
            int diferenciaLongitud = longitudMatrInput - longitudMatrLista;
            int aDevolver = distancias[longitudMatrInput, longitudMatrLista];

            if (diferenciaLongitud != 0 && diferenciaLongitud > 0) aDevolver += diferenciaLongitud;

            return aDevolver;
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
