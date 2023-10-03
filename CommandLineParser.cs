using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;

namespace Assignment1
{
    /// <summary>
    /// checks the syntax and returns appropriate value
    /// </summary>
    public class CommandLineParser
    {
        ShapeFactory sf = new ShapeFactory();
        /// <summary>
        /// to add the defined variables 
        /// </summary>
        public List<string> variables = new List<string>();
        /// <summary>
        /// to add and check the corresponding values in the code 
        /// </summary>
        public List<int> values = new List<int>();
        /// <summary>
        /// FOR Storing the method body commands corresponding to the methodname
        /// </summary>
        public Dictionary<string, string> dic = new Dictionary<string, string>();

        /// <summary>
        /// helps check the syntax of the lines of code
        /// </summary>
        /// <param name="linep"></param>
        /// <returns></returns>
        public string CLIPass(string linep)
        {
            string[] command = linep.Split();
            int piece = command.Length; //counts the number words/value in a line
            Console.WriteLine(command[0]);

            //--------------FOR VARIABLE DECLARATION/ UPDATE AND METHOD--------------- 
            if (piece == 3)
            {
                if (command[0] != "METHOD") //for declaration of a variable and updating them
                {
                    string variable = command[0];
                    string operator_value = command[1];
                    int value = Convert.ToInt32(command[2]);
                    if (variables.Contains(variable))  //updating a variable
                    {
                        if (operator_value == "=")
                        {
                            int index = variables.BinarySearch(variable);
                            values.Insert(index, value);
                            return "var-update-replace";
                        }
                        else if (operator_value == "+")
                        {
                            int index = variables.BinarySearch(variable);
                            int original = values[index];
                            int new_value = original + value;
                            values.Insert(index, new_value);
                            return "var-update-add";
                        }
                        else if (operator_value == "-")
                        {
                            int index = variables.BinarySearch(variable);
                            int original = values[index];
                            int new_value = original - value;
                            values.Insert(index, new_value);
                            return "Var-update-substract";
                        }
                        else //wrong relational operator shown
                        {
                            DialogResult res = MessageBox.Show("Relational operator is incorrect!!",
                            "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "ERROR";
                        }
                    }
                    else if (!variables.Contains(variable)) //creating a variable
                    {
                        variables.Add(variable);
                        values.Add(value);
                        return "var-add";
                    }
                    else // syntax error while defining declaring variable
                    {
                        DialogResult res = MessageBox.Show("Syntax Error while defining/declaring a variable",
                            "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return "ERROR";
                    }
                }
                else if (command[0] == "METHOD")
                {
                    //------------------METHOD ------------------
                    string method_name = command[1];
                    string yes_yes = command[2];
                    if (yes_yes == "()")
                    {
                        Console.Write("It's here222");
                        dic.Add(method_name, "");
                        return "METHOD " + method_name;
                    }
                    return "METHOD " + method_name;
                }
                else //syntax error in declaring variable or a method
                {
                    DialogResult res = MessageBox.Show("Syntax Error",
                            "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return "ERROR";
                }
            }
            else if (command[0] == "ENDMETHOD")
            {
                Console.WriteLine("It'shere 3333");
                return "ENDMETHOD";
            }
            else if (dic.ContainsKey(command[0]) && command.Length == 2)
            {
                Console.WriteLine("MEthod ko name chha");
                return "METHOD-CALL " + command[0];
            }
            else if (piece != 3)
            {
                //------------- if ------------------
                if (piece == 7) // single line if
                {
                    if (command[0] == "IF" && command[4] == "THEN")
                    {
                        string if_variable = command[1];
                        int if_value = Convert.ToInt32(command[3]);
                        string compare = command[2];
                        string if_command = command[5];
                        string if_values = command[6];
                        if (variables.Contains(if_variable)) //checks if variable is declared and defined
                        {
                            if (compare == "=")
                            {
                                if (if_value == values[variables.BinarySearch(if_variable)])
                                {
                                    return CLIPass(if_command + " " + if_values);
                                    // return if_command + if_values;
                                }
                                else
                                {
                                    return "Single-line-if-ERROR";
                                }
                            }
                            else if (compare == "<")
                            {
                                if (values[variables.BinarySearch(if_variable)] < if_value)
                                {
                                    return CLIPass(if_command + " " + if_values);
                                }
                                else
                                {
                                    return "Single-line-if-ERROR";
                                }
                            }
                            else if (compare == ">")
                            {
                                if (values[variables.BinarySearch(if_variable)] > if_value)
                                {
                                    return CLIPass(if_command + " " + if_values);
                                }
                                else
                                {
                                    return "Single-line-if-ERROR";
                                }
                            }
                            else
                            {
                                DialogResult res = MessageBox.Show("Relational operator is incorrect!!",
                            "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return "Single-line-if-ERROR";
                            }
                        }
                        else
                        {
                            DialogResult res = MessageBox.Show("Define the value of " + if_variable + " in the if-loop",
                            "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "Single-line-if-ERROR";
                        }
                    }
                    else
                    {
                        DialogResult res = MessageBox.Show("Syntax error in single line if",
                            "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return "Single-line-if-ERROR";
                    }
                }
                else if (piece == 4) //multiple line if
                {
                    if (command[0] == "IF" && variables.Contains(command[1]) && (command[2] == "<" || command[2] == ">"))
                    {
                        int if_value = Convert.ToInt32(command[3]);
                        if (command[2] == "<")
                        {
                            if (values[variables.BinarySearch(command[1])] < if_value)
                            {
                                return "MULTIPLE-IF";
                            }
                            else   //if condition is false
                            {
                                return "MULTIPLE-IF-FALSE";
                            }
                        }
                        else if (command[2] == ">")
                        {
                            if (values[variables.BinarySearch(command[1])] > if_value)
                            {
                                return "MULTIPLE-IF";
                            }
                            else //if condition is false
                            {
                                return "MULTIPLE-IF-FALSE";
                            }
                        }
                        else if (command[2] == "=")
                        {
                            if (if_value == values[variables.BinarySearch(command[1])])
                            {
                                return "MULTIPLE-IF";
                            }
                            else //if condition is false
                            {
                                return "MULTIPLE-IF-FALSE";
                            }
                        }
                        else //wrong relational operator defined
                        {
                            DialogResult res = MessageBox.Show("Relational operator is incorrect for multiple-loop!!",
                        "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "ERROR";
                        }
                    }
                    else //syntax error in multiple-if code 
                    {
                        DialogResult res = MessageBox.Show("Error in multiple if " + command[0] + piece,
                        "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return "ERROR";
                    }
                }
                else if (command[0] == "ENDIF")
                {
                    return "END-IF";
                }
                else if (command[0] == "LOOP")
                {
                    if (command[1] == "FOR" && variables.Contains(command[2]) && (command[3] == "<=" || command[3] == ">="))
                    {
                        int loop_value = Convert.ToInt32(command[4]);
                        if (command[3] == "<=")
                        {
                            if (values[variables.BinarySearch(command[2])] < loop_value)
                            {
                                //do { return "LOOP"; } while (loop_value >= values[variables.BinarySearch(command[2])]);
                                return "LOOP";
                            }
                            else   //loop condition is false
                            {
                                return "LOOP-FALSE";
                            }
                        }
                        else if (command[3] == ">=")
                        {
                            if (values[variables.BinarySearch(command[2])] >= loop_value)
                            {
                                return "LOOP";
                            }
                            else //loop condition is false
                            {
                                return "LOOP-FALSE";
                            }
                        }
                        else
                        {
                            DialogResult res = MessageBox.Show("Relational operator is incorrect for loop!!",
                       "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "ERROR";
                        }
                    }
                    else
                    {
                        DialogResult res = MessageBox.Show("Syntax error for loop!!",
                       "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return "ERROR";
                    }

                }
                else if (command[0] == "ENDLOOP")
                {
                    return "LOOP-END";
                }
                else if (command[0] == "RECTANGLE")
                {
                    try
                    {
                        string[] point = command[1].Split(',');
                        int rpoint1 = Convert.ToInt32(point[0]);
                        int rpoint2 = Convert.ToInt32(point[1]);
                        if (rpoint1 == 0 || rpoint2 == 0)
                        {
                            DialogResult res = MessageBox.Show("Values of width or height should not be 0.",
                                "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "ERROR";
                        }
                        else if (point.Length != 2)
                        {
                            DialogResult res = MessageBox.Show("Rectangle should have only 2 parameters.",
                                "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "ERROR";
                        }
                    }
                    catch (FormatException ex)
                    {
                        string[] point = command[1].Split(',');
                        string point1 = point[0];
                        string point2 = point[1];

                        if (variables.Contains(point1) && variables.Contains(point2))
                        {

                            int index1 = variables.BinarySearch(point1);
                            int index2 = variables.BinarySearch(point2);
                            int tpoint1 = values[index1];
                            int tpoint2 = values[index2];
                            return "RECTANGLE " + tpoint1 + "," + tpoint2;
                        }
                        else
                        {
                            DialogResult res = MessageBox.Show(ex.Message, "Syntax Error",
                         MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "ERROR";
                        }
                    }
                    catch (Exception ex)
                    {
                        DialogResult res = MessageBox.Show(ex.Message, "Syntax Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return "ERROR";
                    }
                    return linep;
                }

                else if (command[0] == "TRIANGLE")
                {
                    try
                    {
                        string[] point = command[1].Split(',');
                        int point1 = Convert.ToInt32(point[0]);
                        int point2 = Convert.ToInt32(point[1]);
                        int point3 = Convert.ToInt32(point[2]);
                        if (point1 == 0 || point2 == 0 || point3 == 0)
                        {
                            DialogResult res = MessageBox.Show("Value of sides should not be 0\r\n Triangle should have only 3 parameters",
                                    "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "ERROR";
                        }
                        else if (point.Length != 3)
                        {
                            DialogResult res = MessageBox.Show("Triangle should have only 3 parameters",
                                    "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "ERROR";
                        }
                    }
                    catch (FormatException ex)
                    {

                        string[] point = command[1].Split(',');
                        string point1 = point[0];
                        string point2 = point[1];
                        string point3 = point[2];

                        if (variables.Contains(point1) && variables.Contains(point2) && variables.Contains(point3))
                        {

                            int index1 = variables.BinarySearch(point1);
                            int index2 = variables.BinarySearch(point2);
                            int index3 = variables.BinarySearch(point3);
                            int tpoint1 = values[index1];
                            int tpoint2 = values[index2];
                            int tpoint3 = values[index3];
                            return "TRIANGLE " + tpoint1 + "," + tpoint2 + "," + tpoint3;
                        }
                        else
                        {
                            DialogResult res = MessageBox.Show(ex.Message, "Syntax Error",
                         MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "ERROR";
                        }
                    }
                    catch (Exception ex)
                    {
                        DialogResult res = MessageBox.Show("Enter sides of Triangle in integer" + ex.Message, "Syntax Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return "ERROR";
                    }
                    return linep;
                }

                else if (command[0] == "CIRCLE")
                {
                    try
                    {
                        int point1 = Convert.ToInt32(command[1]);
                        if (point1 == 0)
                        {
                            DialogResult res = MessageBox.Show("Value of radius should not be 0",
                                        "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "ERROR";
                        }
                    }
                    catch (FormatException ex)
                    {
                        if (variables.Contains(command[1]))
                        {
                            int index = variables.BinarySearch(command[1]);
                            int cpoint1 = values[index];
                            return "CIRCLE " + cpoint1;
                        }
                        else
                        {
                            DialogResult res = MessageBox.Show("Single value of radius of circle" + ex.Message, "Syntax Error",
                         MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return "ERROR";
                        }
                    }
                    catch (Exception ex)
                    {
                        DialogResult res = MessageBox.Show(ex.Message, "Syntax Error",
                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return "ERROR";
                    }
                    return linep;
                }

                else if (command[0] == "MOVETO")
                {
                    try
                    {
                        string[] point = command[1].Split(',');
                        int point1 = Convert.ToInt32(point[0]);
                        int point2 = Convert.ToInt32(point[1]);
                        if (point.Length != 2)
                        {
                            return "ERROR";
                        }
                    }
                    catch (Exception ex)
                    {
                        DialogResult res = MessageBox.Show("Enter points to move the pen" + ex.Message, "Syntax Error",
                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return "ERROR";
                    }
                    return linep;
                }

                else if (command[0] == "DRAWTO")
                {
                    try
                    {
                        string[] point = command[1].Split(',');
                        int point1 = Convert.ToInt32(point[0]);
                        int point2 = Convert.ToInt32(point[1]);
                        Pen p = new Pen(Color.Black, 1);
                        if (point.Length != 2)
                        {
                            return "ERROR";
                        }
                    }
                    catch (Exception ex)
                    {
                        DialogResult res = MessageBox.Show("Enter points to move the pen" + ex.Message, "Syntax Error",
                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return "ERROR";
                    }
                    return linep;
                }

                else
                {
                    DialogResult res = MessageBox.Show("Choose one of the following commands\r\n-MoveTo <x,y>\r\n-DrawTo <x,y>\r\n-Circle <radius>\r\n-Triangle <side>,<side>,<side>\r\n-Rectangle <width><height>", "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return "ERROR";
                }
            }

            else
            {
                DialogResult res = MessageBox.Show("ERROR",
                           "Syntax Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return "ERROR";
            }
        }
    }
}
