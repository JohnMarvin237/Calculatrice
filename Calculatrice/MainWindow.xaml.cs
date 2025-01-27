using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculatrice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double previousNumber, result;
        // Cette variable conserve uniquement l'opérateur courant
        string selectedOperator;
        private bool isNewNumber = true;
        // Cette liste permet de stocker les opérateurs et les nombres saisis
        private List<string> operatorsNumbers = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            // Mettre la zone de résultat à 0 au début de chaque opération
            resultZone.Content = "0";

            //Vider la zone de calcul après chaque calcul
            calculationZone.Content = "";
        }

        /// <summary>
        /// Gérer les nombres et leur affichage dans le calculationZone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickNumber(object sender, RoutedEventArgs e)
        {
            var numberButton = sender as Button;
            if (numberButton != null)
            {
                string numberValue = numberButton.Content.ToString();

                // Affichage du nombre dans la zone de saisie
                calculationZone.Content += numberValue;

                if (isNewNumber)
                {
                    resultZone.Content = numberValue;
                    isNewNumber = false;
                }
                else
                {
                    resultZone.Content += numberValue;
                }
            }
        }

        /// <summary>
        /// Gestion des opérateurs: +, -, x, ÷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OperatorClick(object sender, RoutedEventArgs e)
        {
            var operatorButton = sender as Button;
            if (operatorButton != null)
            {
                string operatorValue = operatorButton.Content.ToString();

                // Mettre le dernier nombre dans la liste des opérations
                operatorsNumbers.Add(resultZone.Content.ToString());
                operatorsNumbers.Add(operatorValue);

                // Mettre l'opérateur dans la zone de saisie
                calculationZone.Content += $" {operatorValue} ";

                resultZone.Content = "0";
                isNewNumber = true;
            }
        }

        /// <summary>
        /// Implémentation des différentes fonctions avec le bouton égale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void equalButton_Click(object sender, RoutedEventArgs e)
        {
            //On commence ici avec les fonctions trigonométriques
            if (!string.IsNullOrEmpty(selectedOperator))
            {
                double operandTrigo = double.Parse(resultZone.Content.ToString());

                // Entrer dans le switch case pour la gestion des fonctions trigonométriques
                switch (selectedOperator)
                {
                    // Ici on va directement convertir les angles en radians car les fonctions trigonométriques déclarées ne prennent en compte que les angles en radians

                    case "sin":
                        result = Sinus(operandTrigo * (Math.PI / 180));
                        calculationZone.Content += $" )";
                        break;

                    case "cos":
                        result = Cosinus(operandTrigo * (Math.PI / 180));
                        calculationZone.Content += $" )";
                        break;

                    case "tan":
                        result = Tangente(operandTrigo * (Math.PI / 180));
                        calculationZone.Content += $" )";
                        break;

                    case "arcsin":
                        result = ( ArcSinus(operandTrigo) * (180 / Math.PI));
                        calculationZone.Content += $" )";
                        break;

                    case "arccos":
                        result = (ArcCosinus(operandTrigo) * (180 / Math.PI));
                        calculationZone.Content += $" )";
                        break;

                    case "arctan":
                        result = (ArcTangente(operandTrigo) * (180 / Math.PI));
                        calculationZone.Content += $" )";
                        break;
                }

                // Afficher le résultat
                resultZone.Content = result.ToString();
                isNewNumber = true;

                // Réinitialiser l'opérateur trigonométrique
                selectedOperator = null;

                // Sortir de la boucle 
                return;
            }

            // Mettre le dernier nombre dans la liste des opérations
            operatorsNumbers.Add(resultZone.Content.ToString());

            // Calculer en suivant l'ordre des priorités
            result = CalculateWithPriority(operatorsNumbers);

            // Affichage du résultat de l'opération dans le resultZone
            resultZone.Content = result.ToString();
            isNewNumber = true;

            // Réinitialiser la liste des opérations
            operatorsNumbers.Clear();
        }

        private double CalculateWithPriority(List<string> listeOperations)
        {
            // On priorise la multiplication et la division
            for (int i = 1; i < listeOperations.Count; i++)
            {
                if (listeOperations[i] == "x" || listeOperations[i] == "÷")
                {
                    double operand1 = double.Parse(listeOperations[i - 1]);
                    double operand2 = double.Parse(listeOperations[i + 1]);
                    double interResult = 0;

                    if (listeOperations[i] == "x")
                        interResult = Multiplication(operand1, operand2);
                    else if (listeOperations[i] == "÷")
                        interResult = Division(operand1, operand2);

                    // Remplacer l'opération par le résultat dans la liste
                    listeOperations[i - 1] = interResult.ToString();

                    // Supprimer l'opérateur
                    listeOperations.RemoveAt(i);
                    // Supprimer l'opérande suivante
                    listeOperations.RemoveAt(i);
                    // Reévalutation 
                    i--;
                }
            }

            // Récupération de la valeur 
            double result = double.Parse(listeOperations[0]);

            // Gestion de l'addition et de la soustraction
            for (int i = 1; i < listeOperations.Count - 1; i++)
            {
                string operatorValue = listeOperations[i].ToString();
                double nextNumber = double.Parse(listeOperations[i + 1]);

                if (operatorValue == "+")
                    result += nextNumber;
                else if (operatorValue == "-")
                    result -= nextNumber;
            }

            return result;
        }

        /// <summary>
        /// Permet de mettre le champ resultZone à 0 et de vider le chanmp CalculationZone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CButton_Click(object sender, RoutedEventArgs e)
        {
            resultZone.Content = "0";
            calculationZone.Content = "";
            previousNumber = 0;
            isNewNumber = true;
            operatorsNumbers.Clear();
        }

        /// <summary>
        /// Effacer uniquement l'opérande courante
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CEButton_Click(object sender, RoutedEventArgs e)
        {
            if (calculationZone.Content != null && resultZone.Content.ToString() != "0")
            {
                calculationZone.Content = calculationZone.Content.ToString().Substring(0, (calculationZone.Content.ToString().Length - resultZone.Content.ToString().Length));
                resultZone.Content = "";
            }
            else
                resultZone.Content = "0";

        }

        /// <summary>
        /// Gestion de la fonction sin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sinButton_Click(object sender, RoutedEventArgs e)
        {
            selectedOperator = "sin";
            calculationZone.Content = "sin( ";
            resultZone.Content = "0";
            isNewNumber = true;
        }

        /// <summary>
        /// Gestion de la fonction cos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cosButton_Click(object sender, RoutedEventArgs e)
        {
            selectedOperator = "cos";
            calculationZone.Content = "cos( ";
            resultZone.Content = "0";
            isNewNumber = true;
        }

        /// <summary>
        /// Gestion de la fonction tan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tanButton_Click(object sender, RoutedEventArgs e)
        {
            selectedOperator = "tan";
            calculationZone.Content = "tan( ";
            resultZone.Content = "0";
            isNewNumber = true;
        }

        /// <summary>
        ///  Gestion de la fonction arcsin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arcSinButton_Click(object sender, RoutedEventArgs e)
        {
            selectedOperator = "arcsin";
            calculationZone.Content = "arcsin( ";
            resultZone.Content = "0";
            isNewNumber = true;
        }


        /// <summary>
        /// Gestion de la fonction arccos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arcCosButton_Click(object sender, RoutedEventArgs e)
        {
            selectedOperator = "arccos";
            calculationZone.Content = "arccos( ";
            resultZone.Content = "0";
            isNewNumber = true;
        }

        /// <summary>
        /// Gestion de la fonction arctan
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arcTanButton_Click(object sender, RoutedEventArgs e)
        {
            selectedOperator = "arctan";
            calculationZone.Content = "arctan( ";
            resultZone.Content = "0";
            isNewNumber = true;
        }

        /// <summary>
        /// Ajouter un point décimal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pointButton_Click(object sender, RoutedEventArgs e)
        {
            resultZone.Content += ".";
            calculationZone.Content += ".";
            isNewNumber = false;
        }

        /// <summary>
        /// Gestion du Backspace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            // Supprimer le dernier caractère saisi sur la calculatrice
            if (calculationZone.Content.ToString().Length > 0)
            {
                calculationZone.Content = calculationZone.Content.ToString().Substring(0, (calculationZone.Content.ToString().Length - 1));
                resultZone.Content = resultZone.Content.ToString().Substring(0, (resultZone.Content.ToString().Length - 1));

                if (calculationZone.Content.ToString().Length == 0)
                    calculationZone.Content = "0";
            }
        }

        /// <summary>
        /// Inverser le signe d'un nombre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plusOrMinButton_Click(object sender, RoutedEventArgs e)
        {
            double temporaryNum;
            if (double.TryParse(resultZone.Content.ToString(), out temporaryNum))
            {
                temporaryNum *= -1;
                resultZone.Content = temporaryNum.ToString();
                calculationZone.Content = $"{calculationZone.Content.ToString().Substring(0, (calculationZone.Content.ToString().Length - temporaryNum.ToString().Length))} {temporaryNum.ToString()}";
            }
        }

        /// <summary>
        /// Gestion du bouton π
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void piButton_Click(object sender, RoutedEventArgs e)
        {
            calculationZone.Content += "π";
            resultZone.Content = Math.PI.ToString();
        }

        /// <summary>
        /// Gestion du bouton epsilon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void epsButton_Click(object sender, RoutedEventArgs e)
        {
            calculationZone.Content += "e";
            resultZone.Content = Math.E.ToString();
        }

        /// <summary>
        /// Fonction qui additionne deux nombres
        /// </summary>
        /// <param name="nombre1"></param>
        /// <param name="nombre2"></param>
        /// <returns></returns>
        private double Addition(double nombre1, double nombre2)
        {
            return (nombre1 + nombre2);
        }

        /// <summary>
        /// Fonction qui fait la multiplication entre deux nombres
        /// </summary>
        /// <param name="nombre1"></param>
        /// <param name="nombre2"></param>
        /// <returns></returns>
        private double Multiplication(double nombre1, double nombre2)
        {
            return (nombre1 * nombre2);
        }

        /// <summary>
        /// Fonction qui fait la soustraction entre deux nombres
        /// </summary>
        /// <param name="nombre1"></param>
        /// <param name="nombre2"></param>
        /// <returns></returns>
        private double Soustraction(double nombre1, double nombre2)
        {
            return (nombre1 - nombre2);
        }

        /// <summary>
        /// Fonction qui fait la division entre deux nombres
        /// </summary>
        /// <param name="nombre1"></param>
        /// <param name="nombre2"></param>
        /// <returns></returns>
        private double Division(double nombre1, double nombre2)
        {
            return (nombre1 / nombre2);
        }

        /// <summary>
        /// Retourne le sinus d'un angle
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        private double Sinus(double nombre)
        {
            return Math.Sin(nombre);
        }

        /// <summary>
        /// Retourne le cosinus d'un angle
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        private double Cosinus(double nombre)
        {
            return Math.Cos(nombre);
        }

        /// <summary>
        /// Retourne la tangente d'un angle
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        private double Tangente(double nombre)
        {
            return Math.Tan(nombre);
        }

        /// <summary>
        /// Retourne l'angle en ayant la valeur de son sinus
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        private double ArcSinus(double nombre)
        {
            return Math.Asin(nombre);
        }

        /// <summary>
        /// Retourne l'angle en ayant la valeur de son cosinus
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        private double ArcCosinus(double nombre)
        {
            return Math.Acos(nombre);
        }

        /// <summary>
        /// Retourne un angle en ayant la valeur de sa tangente
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns></returns>
        private double ArcTangente(double nombre)
        {
            return Math.Atan(nombre);
        }
    }
}