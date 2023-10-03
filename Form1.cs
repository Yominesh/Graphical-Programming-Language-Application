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
    public partial class Form1 : Form
    {
        /// <summary>
        /// draws on panel1 using graphics
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            g = panel1.CreateGraphics();
        }

        /// <summary>
        /// int a and int b are for the moveto coordinates AND check_run checks if the code has been runned before saving
        /// </summary>
        static int a = 0;
        static int b = 0;
        bool check_Run = false;
        int rows;
        string[] lines;
        ShapeFactory sf = new ShapeFactory();
        CommandLineParser cl1 = new CommandLineParser();
        Pen myPen = new Pen(Color.Black, 2);

        /// <summary>
        /// to draw in panel
        /// </summary>
        public static Graphics g;

        private void Form1_Load(object sender, EventArgs e)
        { }

        /// <summary>
        /// TO SAVE A FILE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (check_Run == true)
            {
                String line = textBox2.Text;
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "txt files (*.txt)|*.txt";
                try
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {

                        StreamWriter fWriter = File.CreateText(saveFileDialog1.FileName);
                        fWriter.WriteLine(line);
                        fWriter.Close();
                    }
                }
                catch (IOException ie)
                {
                    string error = ie.Message;
                    DialogResult res = MessageBox.Show(error, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                DialogResult res = MessageBox.Show("Error! \r\nRun the program before saving",
                "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// TO LOAD A FILE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            a = 0; b = 0;
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.ShowDialog();
                StreamReader fReader = File.OpenText(openFileDialog1.FileName);
                string line;
                string n = null;
                while ((line = fReader.ReadLine()) != null)
                {
                    n += line + "\r\n";
                    textBox2.Text = n.Trim();
                }
                fReader.Close();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Error", "Cannot find text file");
            }
            catch (IOException ie)
            {
                MessageBox.Show("Error", "IO Exception:" + ie);
            }

        }

        /// <summary>
        /// TO EXIT THE APPLICATION
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        /// <summary>
        /// INFO ABOUT THE APPLICATION
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("GPU Program \r\nInputs Commands and draws on the panel",
                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// BUTTON THAT REFRESHES THE PANEL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
        }
        

        /// <summary>
        /// BUTTON THAT EXECUTES THE COMMAND FROM BOTH TEXTBOXES
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            // Inputs Single-line
            if (!String.IsNullOrEmpty(textBox1.Text) && String.IsNullOrEmpty(textBox2.Text))
            {
                string input = (textBox1.Text).ToUpper();
                Pass(cl1.CLIPass(input), 0);
            }
            // Inputs Muti-line text box
            else if (!String.IsNullOrEmpty(textBox2.Text) && String.IsNullOrEmpty(textBox1.Text))
            {
                TextReader read = new System.IO.StringReader((textBox2.Text).ToUpper());
                rows = textBox2.Lines.Length;
                lines = new string[rows];
                String[] store = new String[rows];

                for (int r = 0; r < rows; r++)
                {
                    lines[r] = read.ReadLine();
                    store[r] = cl1.CLIPass(lines[r]);
                    //Console.WriteLine("Original Lines : " + lines[r]);
                    //Console.WriteLine("return LINES " + store[r]);

                }
                for (int i = 0; i < store.Length; i++)
                {
                    string[] store2 = store[i].Split();
                    // FOR MULTIPLE LINE IF
                    if ((store[i] == "MULTIPLE-IF" || store[i] == "MULTIPLE-IF-FALSE") && store.Contains("END-IF"))
                    {
                        int index = Array.IndexOf(store, "END-IF");
                        if (index > i)
                        {
                            for (int j = i + 1; j < index; j++)
                            {
                                if (store[i] == "MULTIPLE-IF")
                                {
                                    Pass(store[j], j); 
                                }
                                store[j] = "checked";
                            }
                        }
                        else
                        {
                            DialogResult res = MessageBox.Show("Error in the multiple line if-loop", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }//FOR LOOP
                    else if ((store[i] == "LOOP" || store[i] == "LOOP-FALSE" || store[i] == "COUNTER-REACHED") && store.Contains("LOOP-END"))
                    {
                        int counter = 0;
                        string operational_data;
                        int loops_value;
                        int index = Array.IndexOf(store, "LOOP-END");
                        if (index > i)
                        {
                            //Console.WriteLine("WHAT :" + lines[i]);
                            string[] newline = lines[i].Split();
                            counter = cl1.variables.BinarySearch(newline[2]);
                            // = 100;
                            operational_data = newline[3];
                            loops_value = Convert.ToInt32(newline[4]);  // x<= ((100))
                            //Console.WriteLine("ope "+operational_data);
                            if (operational_data == "<=")
                            {
                                while (counter <= loops_value)
                                {
                                    //Console.WriteLine("UUUUUU");
                                    for (int j = i + 1; j < index; j++)
                                    {
                                        //Console.WriteLine("store[j] " + store[j]);
                                        if (store[j] == "var-update-add")
                                        {
                                            //Console.WriteLine("newline: " + newline[2]);
                                            String[] value_that = lines[j].Split();
                                            //Console.WriteLine("value_that[0] " + value_that[0]);
                                            //Console.WriteLine("value_that[2] " + value_that[2]);
                                            //Console.WriteLine("counter " + counter);
                                            if (newline[2] == value_that[0])
                                            {
                                                counter = counter + Convert.ToInt32(value_that[2]);
                                            }
                                        }
                                        //Console.WriteLine("RIZU LAI " + lines[j]);
                                        Pass(cl1.CLIPass(lines[j]), j);
                                    }
                                }
                            }
                            else if(operational_data==">=")
                            {
                                Console.WriteLine("loops="+ loops_value+" counter="+ counter);
                                while (counter >= loops_value)
                                {
                                    Console.WriteLine("UUUUU");
                                    for (int j = i + 1; j < index; j++)
                                    {
                                        //Console.WriteLine("store[j] " + store[j]);
                                        if (store[j] == "var-update-substract")
                                        {
                                            Console.WriteLine("newline: " + newline[2]);
                                            String[] value_that = lines[j].Split();
                                            //Console.WriteLine("value_that[0] " + value_that[0]);
                                            //Console.WriteLine("value_that[2] " + value_that[2]);
                                            //Console.WriteLine("counter " + counter);
                                            if (newline[2] == value_that[0])
                                            {
                                                counter = counter - Convert.ToInt32(value_that[2]);
                                            }
                                        }
                                        //Console.WriteLine("RIZU LAI " + lines[j]);
                                        Pass(cl1.CLIPass(lines[j]), j);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DialogResult res = MessageBox.Show("Error in the loop", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }//FOR METHOD
                    else if ((store2[0]) == "METHOD" && cl1.dic.ContainsKey(store2[1]) && store.Contains("ENDMETHOD"))
                    {
                        int end_method = Array.IndexOf(store, "ENDMETHOD");
                        String method_body = "";
                        //Console.WriteLine("SEEE" + end_method + "  " + i);
                        for (int j = i + 1; j < end_method; j++)
                        {
                            method_body = method_body + "\n " + lines[j];
                            //Console.WriteLine("1 " + method_body);
                            store[j] = "checked";
                        }
                        cl1.dic[store2[1]] = method_body;
                        //Console.WriteLine("2 " + method_body);

                    }
                    else if (store2[0] == "METHOD-CALL" && cl1.dic.ContainsKey(store2[1]))
                    {
                       // Console.WriteLine("Checking");
                        string method_body = cl1.dic[store2[1]];
                        //Console.WriteLine(method_body);
                        //Console.WriteLine("Checking222");
                        executeMethod(method_body);
                        //Console.WriteLine("Checking");
                    }
                    else
                    {
                        Pass(store[i], i);
                    }
                }
            }
            else
            {
                DialogResult res = MessageBox.Show("Input Command in any ONE of the TextBoxes", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// executes the method_body Method
        /// </summary>
        /// <param name="commands"></param>
        public void executeMethod(string commands)
        {
            string[] lines = commands.Split('\n');
            for (int i = 1; i < lines.Length; i++)
            {
                //Console.WriteLine("method vitra"+lines[i].Trim());
                string s = cl1.CLIPass(lines[i].Trim());
                //Console.WriteLine("method s" + s);
                Pass(s, 0);
            }
        }
        /// <summary>
        /// Draws the given syntactically correct commands 
        /// </summary>
        /// <param name="linep"></param>
        /// <param name="indexx"></param>
        public void Pass(string linep, int indexx)
        {
            string[] command = linep.Split();
            if (command[0] == "RECTANGLE")
            {
                string[] point = command[1].Split(',');
                int point1 = Convert.ToInt32(point[0]);
                int point2 = Convert.ToInt32(point[1]);
                Shape shape1 = sf.getShape("RECTANGLE");
                shape1.DrawTo(g, a, b, point1, point2);
            }
            else if (command[0] == "TRIANGLE")
            {

                string[] point = command[1].Split(',');
                int point1 = Convert.ToInt32(point[0]);
                int point2 = Convert.ToInt32(point[1]);
                int point3 = Convert.ToInt32(point[2]);
                Shape shape2 = sf.getShape("TRIANGLE");
                shape2.DrawTo(g, a, b, point1, point2, point3);
            }
            else if (command[0] == "CIRCLE")
            {
                int point1 = Convert.ToInt32(command[1]);
                Shape shape1 = sf.getShape("CIRCLE");
                shape1.DrawTo(g, a, b, point1);

            }
            else if (command[0] == "MOVETO")
            {
                string[] point = command[1].Split(',');
                int point1 = Convert.ToInt32(point[0]);
                int point2 = Convert.ToInt32(point[1]);
                a = point1;
                b = point2;
            }
            else if (command[0] == "DRAWTO")
            {
                string[] point = command[1].Split(',');
                int point1 = Convert.ToInt32(point[0]);
                int point2 = Convert.ToInt32(point[1]);
                Pen p = new Pen(Color.Black, 1);
                g.DrawLine(p, new Point(a, b), new Point(point1, point2));
                //Console.WriteLine(a);
            }
            check_Run = true;
        }

        /// <summary>
        /// BUTTON THAT RESETS THE PEN POSITION
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            a = 0;
            b = 0;
        }

        /// <summary>
        /// BUTTON THAT CLEARS THE MULTILINE TEXTBOX
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Text = " ";
        }
    }
}
