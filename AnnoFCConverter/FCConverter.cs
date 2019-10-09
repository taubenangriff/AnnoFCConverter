using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnoFCConverter
{
    class FCConverter
    {
        static void Main(string[] args)
        {
            string appName = "Anno 1800 FC-Converter";
            string version = "Alpha 1.1.0 Non Public";
            Console.WriteLine("{0}: Version {1}", appName, version);

            string ReadCDATA = "-r";
            string WriteCDATA = "-w";
            string ReadCDATA2070Mode = "-rz";
            string WriteCDATA2070Mode = "-wz";
            string SpecificOutputFile = "-o";
            string Overwrite = "-y";

            Console.WriteLine("How to Use:");
            Console.WriteLine(ReadCDATA + " <InputFilename> to make an Anno 1800 <InputFilename>.fc editable");
            Console.WriteLine(WriteCDATA + " <InputFilename> to convert <InputFilename>.html into an Anno 1800 .fc");
            Console.WriteLine(ReadCDATA2070Mode + " <InputFilename> to make an Anno 2070 <InputFilename>.fc <InputFilename> editable");
            Console.WriteLine(WriteCDATA + " <InputFilename> to convert <InputFilename>.html into an Anno 2070 .fc");
            Console.WriteLine(SpecificOutputFile + " <OutputFilename> to set a specific output filename");
            Console.WriteLine(Overwrite + " to overwrite the output File"); 

            FCConverter p = new FCConverter();

            string mode = "";
            bool modeSet = false;
            string InputFileName = "";
            bool hasSpecificOutputFile = false;
            string OutputFileName = "";
            bool OverwriteFile = false;
            string LastArg = "";

            foreach (string s in args)
            {
                if (s.StartsWith("-"))
                {
                    if (s.Equals(ReadCDATA) && !modeSet)
                    {
                        mode = "readCDATA";
                        modeSet = true;
                        LastArg = "mode";
                    }
                    else if (s.Equals(ReadCDATA2070Mode) && !modeSet)
                    {
                        mode = "readCDATA2070Mode";
                        modeSet = true;
                        LastArg = "mode";
                    }
                    else if (s.Equals(WriteCDATA) && !modeSet)
                    {
                        mode = "writeCDATA";
                        modeSet = true;
                        LastArg = "mode";
                    }
                    else if (s.Equals(WriteCDATA2070Mode) && !modeSet)
                    {
                        mode = "writeCDATA2070Mode";
                        modeSet = true;
                        LastArg = "mode";
                    }
                    else if (s.Equals(SpecificOutputFile))
                    {
                        hasSpecificOutputFile = true;
                        LastArg = "output";
                    }
                    else if (s.Equals(Overwrite))
                    {
                        OverwriteFile = true;
                        LastArg = "overwrite";
                    }
                }
                else
                {
                    if (LastArg.Equals("mode"))
                    {
                        InputFileName = s;
                        LastArg = "";
                    }
                    else if (LastArg.Equals("output"))
                    {
                        OutputFileName = s;
                        LastArg = "";
                    }
                    else
                    {
                        LastArg = "";
                    }
                }
            }

            if (!hasSpecificOutputFile)
            {
                OutputFileName = InputFileName;
            }

            switch (mode)
            {
                case "readCDATA":
                    if (File.Exists(OutputFileName + ".html") && !OverwriteFile)
                    {
                        Console.WriteLine("OutputFile Already Exists. Use " + Overwrite + " as argument to overwrite the file");
                    }
                    if (!File.Exists(InputFileName + ".fc"))
                    {
                        Console.WriteLine("Input File does not exist");
                    }
                    else
                    {
                        p.ConvertToHTMLFile(InputFileName + ".fc", OutputFileName + ".html");
                    }

                    break;
                case "writeCDATA":
                    if (File.Exists(OutputFileName + ".fc") && !OverwriteFile)
                    {
                        Console.WriteLine("OutputFile Already Exists. Use " + Overwrite + " as argument to overwrite the file");
                    }
                    if (!File.Exists(InputFileName + ".html"))
                    {
                        Console.WriteLine("Input File does not exist");
                    }
                    else
                    {
                        p.ConvertToFCFile(InputFileName + ".html", OutputFileName + ".fc");
                    }
                    break;
                case "readCDATA2070Mode":
                    if (File.Exists(OutputFileName + ".html") && !OverwriteFile)
                    {
                        Console.WriteLine("OutputFile Already Exists. Use " + Overwrite + " as argument to overwrite the file");
                    }
                    if (!File.Exists(InputFileName + ".fc"))
                    {
                        Console.WriteLine("Input File does not exist");
                    }
                    else
                    {
                        p.Convert2070IntoHTML(InputFileName + ".fc", OutputFileName + ".html");
                    }
                    break;
                case "writeCDATA2070Mode":
                    if (File.Exists(OutputFileName + ".fc") && !OverwriteFile)
                    {
                        Console.WriteLine("OutputFile Already Exists. Use " + Overwrite + " as argument to overwrite the file");
                    }
                    if (!File.Exists(InputFileName + ".html"))
                    {
                        Console.WriteLine("Input File does not exist");
                    }
                    else
                    {
                        p.ConvertTo2070FCFile(InputFileName + ".html", OutputFileName + ".fc");
                    }
                    break;
                case "":
                    Console.WriteLine("Error: No Mode for conversion was set");
                    break;
            }
            return; 
        }


        private void ConvertToFCFile(string InputPath, string OutputPath)
        {
            FileStream fs = new FileStream(OutputPath, FileMode.CreateNew);
            using (StreamReader sr = new StreamReader(InputPath))
            using (BinaryWriter bw = new BinaryWriter(fs))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                char c = (char)sr.Read();
                string token = "";
                while (!sr.EndOfStream)
                {
                    if (c == '<')
                    {
                        while (!(c == '>'))
                        {
                            token += c;
                            c = (char)sr.Read();
                        }
                        token += c;
                        c = (char)sr.Read();
                        //one token should be complete
                        sw.Write(token);
                        sw.Flush();
                        //Check if token matches any important stuff, if so decrypt the CDATA
                        if (token.Equals("<GuidVariationList>") || token.Equals("<m_SequenceIds>") || token.Equals("<DummyGroup>") || token.Equals("<ValidSequenceIDs>") || token.Equals("<ApproximationMapping>"))
                        {
                            //for implementation
                            //add the next six characters (CDATA[) get the content of the brackets, split it on " " into an array, convert each one into 4 bytes and write them with the binary writer 
                            for (int k = 0; k < 6; k++)
                            {
                                sw.Write(c);
                                c = (char)sr.Read();
                            }

                            sw.Flush();

                            string cdata = "";
                            while (c != ']')
                            {
                                cdata += c;
                                c = (char)sr.Read();
                            }
                            string[] CdataArr = cdata.Split(' ');
                            foreach (String s in CdataArr)
                            {
                                //convert s to int and let the binary writer write it
                                int IntForm = Int32.Parse(s);
                                bw.Write(IntForm);
                            }
                        }
                        //reset the token
                        token = "";
                    }

                    //as long as there is no opening < character the file gets parsed and added directly to the output 
                    if (c != '<')
                    {
                        while (c != '<')
                        {
                            token += c;
                            c = (char)sr.Read();
                        }
                        token = token.Replace("\b", "");
                        token = token.Replace("\n", "");
                        sw.Write(token);
                        sw.Flush();
                        token = "";
                    }
                }
            }
        }


        private void ConvertTo2070FCFile(string InputPath, string OutputPath)
        {
            FileStream fs = new FileStream(OutputPath, FileMode.CreateNew);
            using (StreamReader sr = new StreamReader(InputPath))
            using (BinaryWriter bw = new BinaryWriter(fs))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                char c = (char)sr.Read();
                string token = "";
                while (!sr.EndOfStream)
                {
                    if (c == '<')
                    {
                        while (!(c == '>'))
                        {
                            token += c;
                            c = (char)sr.Read();
                        }
                        token += c;
                        c = (char)sr.Read();
                        //one token should be complete
                        sw.Write(token);
                        sw.Flush();
                        //Check if token matches any important stuff, if so decrypt the CDATA
                        if (token.Equals("<GuidVariationList>") || token.Equals("<m_SequenceIds>") || token.Equals("<DummyGroup>") || token.Equals("<ValidSequenceIDs>") || token.Equals("<Position>") || token.Equals("<Extents>") || token.Equals("<i>"))
                        {
                            //for implementation
                            //add the next six characters (CDATA[) get the content of the brackets, split it on " " into an array, convert each one into 4 bytes and write them with the binary writer 
                            string check = "";
                            for (int k = 0; k < 6; k++)
                            {
                                check += c;
                                c = (char)sr.Read();
                            }

                            if (check.Equals("CDATA["))
                            {
                                sw.Write(check);
                                sw.Flush();
                                string cdata = "";
                                while (c != ']')
                                {
                                    cdata += c;
                                    c = (char)sr.Read();
                                }
                                string[] CdataArr = cdata.Split(' ');
                                foreach (String s in CdataArr)
                                {
                                    //convert s to int and let the binary writer write it
                                    int IntForm = Int32.Parse(s);
                                    bw.Write(IntForm);
                                }
                            }

                        }
                        if (token.Equals("<Orientation>"))
                        {
                            //for implementation
                            //add the next six characters (CDATA[) get the content of the brackets, split it on " " into an array, convert each one into 4 bytes and write them with the binary writer 
                            string check = "";
                            for (int k = 0; k < 6; k++)
                            {
                                check += c;
                                c = (char)sr.Read();
                            }

                            if (check.Equals("CDATA["))
                            {
                                sw.Write(check);
                                sw.Flush();
                                string cdata = "";
                                while (c != ']')
                                {
                                    cdata += c;
                                    c = (char)sr.Read();
                                }
                                string[] CdataArr = cdata.Split(' ');
                                bool isFirst = true;
                                foreach (String s in CdataArr)
                                {
                                    if (isFirst)
                                    {
                                        isFirst = false;
                                        int intForm = Int32.Parse(s);
                                        bw.Write(intForm);
                                    }
                                    else
                                    {
                                        float FloatForm = float.Parse(s);
                                        bw.Write(FloatForm);
                                    }
                                }
                            }

                        }
                        //reset the token
                        token = "";
                    }

                    //as long as there is no opening < character the file gets parsed and added directly to the output 
                    if (c != '<')
                    {
                        while (c != '<')
                        {
                            token += c;
                            c = (char)sr.Read();
                        }
                        token = token.Replace("\b", "");
                        token = token.Replace("\n", "");
                        sw.Write(token);
                        sw.Flush();
                        token = "";
                    }
                }
            }
            Console.WriteLine("f");
        }

        private string[] ReadFileToHex(string Path)
        {
            using (StreamReader sr = new StreamReader(Path))
            {
                if (!File.Exists(Path))
                {
                    Console.WriteLine("File" + Path + " Could not be read");
                }
                string hex;
                using (BinaryReader br = new BinaryReader(sr.BaseStream))
                {
                    byte[] allData = br.ReadBytes((int)br.BaseStream.Length);
                    hex = BitConverter.ToString(allData);
                }
                return hex.Split('-');
            }

        }

        private void CheckFilesForHTML(string path, string OutputPath)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("File" + path + " Could not be read");
            }
            else if (File.Exists(OutputPath))
            {
                Console.WriteLine("File " + OutputPath + " Already exists. Type c to continue");
                string ConsoleInput = Console.ReadLine();
                if (ConsoleInput.Equals("c"))
                {
                    Console.WriteLine("Writing File " + OutputPath);
                    ConvertToHTMLFile(path, OutputPath);
                }
            }
            else
            {
                Console.WriteLine("Writing File " + OutputPath);
                ConvertToHTMLFile(path, OutputPath);
            }
        }

        private void CheckFilesFor2070HTML(string path, string OutputPath)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("File" + path + " Could not be read");
            }
            else if (File.Exists(OutputPath))
            {
                Console.WriteLine("File " + OutputPath + " Already exists. Type c to continue");
                string ConsoleInput = Console.ReadLine();
                if (ConsoleInput.Equals("c"))
                {
                    Console.WriteLine("Writing File " + OutputPath);
                    Convert2070IntoHTML(path, OutputPath);
                }
            }
            else
            {
                Console.WriteLine("Writing File " + OutputPath);
                Convert2070IntoHTML(path, OutputPath);
            }
        }

        private void CheckFilesForFC(string InputPath, string OutputPath)
        {
            if (!File.Exists(InputPath))
            {
                Console.WriteLine("File " + InputPath + " Could not be read");
            }
            else if (File.Exists(OutputPath))
            {
                Console.WriteLine("File " + OutputPath + " Already exists. Type c to continue");
                string ConsoleInput = Console.ReadLine();
                if (ConsoleInput.Equals("c"))
                {
                    File.Delete(OutputPath);
                    Console.WriteLine("Writing File " + OutputPath);
                    ConvertToFCFile(InputPath, OutputPath);
                }
            }
            else
            {
                Console.WriteLine("Writing File " + OutputPath);
                ConvertToFCFile(InputPath, OutputPath);
            }
        }


        private void CheckFilesFor2070FC(string InputPath, string OutputPath)
        {
            if (!File.Exists(InputPath))
            {
                Console.WriteLine("File " + InputPath + " Could not be read");
            }
            else if (File.Exists(OutputPath))
            {
                Console.WriteLine("File " + OutputPath + " Already exists. Type c to continue");
                string ConsoleInput = Console.ReadLine();
                if (ConsoleInput.Equals("c"))
                {
                    File.Delete(OutputPath);
                    Console.WriteLine("Writing File " + OutputPath);
                    ConvertTo2070FCFile(InputPath, OutputPath);
                }
            }
            else
            {
                Console.WriteLine("Writing File " + OutputPath);
                ConvertTo2070FCFile(InputPath, OutputPath);
            }
        }
        /// <summary>
        /// Converts an Anno 1800 .fc file into an html file
        /// </summary>
        /// <param name="path">path of the input file</param>
        /// <param name="OutputPath">path of the output file</param>
        private void ConvertToHTMLFile(string path, string OutputPath)
        {
            string token = "";
            ///every time we advance c forward we need to tick a position forward to get the right hex when we need it
            string[] HexData = ReadFileToHex(path);
            int offset = 0;
            using (StreamWriter sw = new StreamWriter(OutputPath))
            {
                char c = ToChar(HexData[offset]);
                offset++;
                while (offset < HexData.Length)
                {
                    if (c == '<')
                    {
                        while (c != '>')
                        {
                            token += c;
                            c = ToChar(HexData[offset]);
                            offset++;
                        }
                        token += c;
                        if (offset < HexData.Length)
                        {
                            c = ToChar(HexData[offset]);
                        }
                        offset++;
                        //one token should be complete
                        sw.Write(token);
                        sw.Flush();
                        //Check if token matches any important stuff, if so decrypt the CDATA
                        if (token.Equals("<GuidVariationList>") || token.Equals("<m_SequenceIds>") || token.Equals("<DummyGroup>") || token.Equals("<ValidSequenceIDs>") || token.Equals("<ApproximationMapping>"))
                        {
                            //jump by CDATA so c equals '['
                            while (c != '[')
                            {
                                c = ToChar(HexData[offset]);
                                offset++;
                            }

                            //write CDATA[ and advance by one character
                            c = ToChar(HexData[offset]);
                            offset++;
                            sw.Write("CDATA[");

                            //Alternative Way: 
                            //as long as we dont get a hex which converted into char gives ']' we will squeeze blocks of four, reverse them and append the result. with each hex we choose we will advance c without doing anything. 
                            bool IsFirstInt = true;
                            int NumberOfSpaces = 0;
                            int ByteSize = 4;
                            while (ByteSize >= 0)
                            {
                                //makean int out of the next three positions (flipped) in hexData
                                string intInHexForm = HexData[offset + 2] + HexData[offset + 1] + HexData[offset + 0] + HexData[offset - 1];
                                int intAgain = int.Parse(intInHexForm, System.Globalization.NumberStyles.HexNumber);

                                if (IsFirstInt)
                                {
                                    ByteSize = intAgain;
                                    NumberOfSpaces = ByteSize / 4;
                                    IsFirstInt = false;
                                }
                                sw.Write(intAgain);
                                sw.Flush();

                                if (NumberOfSpaces > 0)
                                {
                                    sw.Write(" ");
                                    NumberOfSpaces--;
                                }

                                //advance 4 chars; 
                                for (int j = 0; j <= 3; j++)
                                {
                                    c = ToChar(HexData[offset]);
                                    offset++;
                                }
                                ByteSize -= 4;
                            }
                            sw.Flush();
                        }
                        //reset the token
                        token = "";
                    }



                    //as long as there is no opening < character the file gets parsed and added directly to the output 
                    if (c != '<' && offset < HexData.Length - 1)
                    {
                        while (c != '<')
                        {
                            token += c;
                            c = ToChar(HexData[offset]);
                            offset++;
                        }
                        token = token.Replace("\b", "");
                        token = token.Replace("\n", "");
                        sw.Write(token);
                        sw.Flush();

                        token = "";
                    }
                }
            }
        }
        /// <summary>
        /// Converts an Anno 2070 .fc file into an html file
        /// </summary>
        /// <param name="path">path of the input file</param>
        /// <param name="OutputPath">path of the output file</param>
        private void Convert2070IntoHTML(string InputPath, string OutputPath)
        {
            string[] HexData = ReadFileToHex(InputPath);
            int offset = 0;
            char c;
            string token = "";
            c = ToChar(HexData[offset]);
            offset++;
            using (StreamWriter sw = new StreamWriter(OutputPath))
            {
                while (offset < HexData.Length)
                {
                    if (c == '<')
                    {
                        while (!(c == '>'))
                        {
                            token += c;
                            c = ToChar(HexData[offset]);
                            offset++;
                        }
                        token += c;
                        if (offset < HexData.Length)
                        {
                            c = ToChar(HexData[offset]);
                        }
                        offset++;
                        //one token should be complete
                        sw.Write(token);
                        sw.Flush();

                        //check if the token is useful
                        if (token.Equals("<Position>") || token.Equals("<Extents>") || token.Equals("<DummyGroup>") || token.Equals("<m_SequenceIds>"))
                        {
                            if (c == 'C')
                            {
                                //jump by cdata
                                while (c != '[')
                                {
                                    c = ToChar(HexData[offset]);
                                    offset++;
                                }
                                //write CDATA[ and advance by one character
                                c = ToChar(HexData[offset]);
                                offset++;

                                sw.Write("CDATA[");

                                //as long as we dont get a hex which converted into char gives ']' we will squeeze blocks of four, reverse them and append the result. with each hex we choose we will advance c without doing anything. 
                                bool IsFirstInt = true;
                                int NumberOfSpaces = 0;
                                int ByteSize = 4;
                                while (c != ']')
                                {
                                    //makean int out of the next three positions (flipped) in hexData //apparently i jumped to far but fuck it
                                    string intInHexForm = HexData[offset + 2] + HexData[offset + 1] + HexData[offset] + HexData[offset - 1];
                                    int intAgain = int.Parse(intInHexForm, System.Globalization.NumberStyles.HexNumber);

                                    if (IsFirstInt)
                                    {
                                        NumberOfSpaces = intAgain / 4;
                                        IsFirstInt = false;
                                    }
                                    sw.Write(intAgain);

                                    if (NumberOfSpaces > 0)
                                    {
                                        sw.Write(" ");
                                        NumberOfSpaces--;
                                    }

                                    //advance 4 chars; 
                                    for (int j = 0; j <= 3 && c != ']'; j++)
                                    {
                                        offset++;
                                        c = ToChar(HexData[offset]);
                                    }
                                }
                                //I dont get my own code boi wtf do i have to advance 
                                offset++;
                                sw.Flush();
                            }
                        }


                        if (token.Equals("<i>"))
                        {
                            if (c == 'C')
                            {
                                //actually check if the next 5 chars are CDATA
                                string check = "";
                                while (c != '[')
                                {
                                    check += c;
                                    c = ToChar(HexData[offset]);
                                    offset++;
                                }
                                if (check.Equals("CDATA"))
                                {
                                    //write CDATA[ and advance by one character
                                    c = ToChar(HexData[offset]);
                                    offset++;

                                    sw.Write("CDATA[");

                                    //as long as we dont get a hex which converted into char gives ']' we will squeeze blocks of four, reverse them and append the result. with each hex we choose we will advance c without doing anything. 
                                    bool IsFirstInt = true;
                                    int NumberOfSpaces = 0;
                                    while (c != ']')
                                    {
                                        //makean int out of the next three positions (flipped) in hexData //apparently i jumped to far but fuck it
                                        string intInHexForm = HexData[offset + 2] + HexData[offset + 1] + HexData[offset] + HexData[offset - 1];
                                        int intAgain = int.Parse(intInHexForm, System.Globalization.NumberStyles.HexNumber);

                                        if (IsFirstInt)
                                        {
                                            NumberOfSpaces = intAgain / 4;
                                            IsFirstInt = false;
                                        }
                                        sw.Write(intAgain);

                                        if (NumberOfSpaces > 0)
                                        {
                                            sw.Write(" ");
                                            NumberOfSpaces--;
                                        }

                                        //advance 4 chars; 
                                        for (int j = 0; j <= 3 && c != ']'; j++)
                                        {
                                            offset++;
                                            c = ToChar(HexData[offset]);
                                        }
                                    }
                                    //I dont get my own code boi wtf do i have to advance 
                                    offset++;
                                    sw.Flush();
                                }
                            }
                        }

                        if (token.Equals("<Orientation>"))
                        {
                            if (c == 'C')
                            {
                                //jump by cdata
                                while (c != '[')
                                {
                                    c = ToChar(HexData[offset]);
                                    offset++;
                                }
                                //write CDATA[ and advance by one character
                                c = ToChar(HexData[offset]);
                                offset++;

                                sw.Write("CDATA[");

                                string ByteSizeStr = HexData[offset + 2] + HexData[offset + 1] + HexData[offset + 0] + HexData[offset - 1];
                                int ByteSize = int.Parse(ByteSizeStr, System.Globalization.NumberStyles.HexNumber);
                                int NumberOfSpaces = ByteSize / 4;
                                sw.Write(ByteSize);
                                sw.Write(" ");
                                NumberOfSpaces--;

                                //advance 4 chars; 
                                for (int j = 0; j <= 3; j++)
                                {
                                    c = ToChar(HexData[offset]);
                                    offset++;
                                }

                                //as long as we dont get a hex which converted into char gives ']' we will squeeze blocks of four, reverse them and append the result. with each hex we choose we will advance c without doing anything. 
                                while (c != ']')
                                {
                                    //makean int out of the next three positions (flipped) in hexData
                                    string intInHexForm = HexData[offset + 2] + HexData[offset + 1] + HexData[offset] + HexData[offset - 1];
                                    uint num = uint.Parse(intInHexForm, System.Globalization.NumberStyles.AllowHexSpecifier);

                                    byte[] floatVals = BitConverter.GetBytes(num);
                                    float floatAgain = BitConverter.ToSingle(floatVals, 0);
                                    sw.Write(floatAgain);
                                    sw.Flush();

                                    if (NumberOfSpaces > 0)
                                    {
                                        sw.Write(" ");
                                        sw.Flush();
                                        NumberOfSpaces--;
                                    }

                                    //advance 4 chars; 
                                    for (int j = 0; j <= 3 && c != ']'; j++)
                                    {
                                        c = ToChar(HexData[offset]);
                                        offset++;
                                    }
                                }
                                sw.Flush();
                            }
                        }

                        if (token.Equals("<GuidVariationList>") || token.Equals("<ValidSequenceIDs>"))
                        {
                            if (c == 'C')
                            {
                                //jump by CDATA so c equals '['
                                while (c != '[')
                                {
                                    c = ToChar(HexData[offset]);
                                    offset++;
                                }

                                //write CDATA[ and advance by one character
                                c = ToChar(HexData[offset]);
                                offset++;
                                sw.Write("CDATA[");

                                //Alternative Way: 
                                //as long as we dont get a hex which converted into char gives ']' we will squeeze blocks of four, reverse them and append the result. with each hex we choose we will advance c without doing anything. 
                                bool IsFirstInt = true;
                                int NumberOfSpaces = 0;
                                while (c != ']')
                                {
                                    //makean int out of the next three positions (flipped) in hexData
                                    string intInHexForm = HexData[offset + 2] + HexData[offset + 1] + HexData[offset] + HexData[offset - 1];
                                    int intAgain = int.Parse(intInHexForm, System.Globalization.NumberStyles.HexNumber);

                                    if (IsFirstInt)
                                    {
                                        NumberOfSpaces = intAgain / 4;
                                        IsFirstInt = false;
                                    }
                                    sw.Write(intAgain);

                                    if (NumberOfSpaces > 0)
                                    {
                                        sw.Write(" ");
                                        NumberOfSpaces--;
                                    }

                                    //advance 4 chars; 
                                    for (int j = 0; j <= 3; j++)
                                    {
                                        c = ToChar(HexData[offset]);
                                        offset++;
                                    }
                                }
                                sw.Flush();
                            }
                        }
                        //reset it
                        token = "";
                    }


                    //as long as there is no opening < character the file gets parsed and added directly to the output 
                    if (c != '<' && offset < HexData.Length - 1)
                    {
                        while (c != '<')
                        {
                            token += c;
                            c = ToChar(HexData[offset]);
                            offset++;
                        }
                        token = token.Replace("\b", "");
                        token = token.Replace("\n", "");
                        sw.Write(token);
                        sw.Flush();

                        token = "";
                    }
                }
            }
        }
        private char ToChar(String s)
        {
            char c = System.Convert.ToChar(System.Convert.ToUInt32(s, 16));
            return c;
        }
    }
}

