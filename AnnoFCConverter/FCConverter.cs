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
        //commands 
        private static string FcToCf7 = "-r";
        private static string Cf7ToFc = "-w";
        private static string FcToCf5 = "-rz";
        private static string Cf5ToFc = "-wz";
        private static string SpecificOutputFile = "-o";
        private static string Overwrite = "-y";
        private static string IsdToCi5 = "-i";
        private static string Ci5ToIsd = "-wi";
        private static string ActivateErrorLogging = "-e";
        private static string Help = "-help"; 

        //file formats
        private static string FileFormatFc = "fc";
        private static string FileFormatCf7 = "cf7";
        private static string FileFormatCf5 = "cf5";
        private static string FileFormatIsd = "isd";
        private static string FileFormatCi5 = "ci5"; 

        private enum Modes { noMode = -1, FcToCf7, Cf7ToFc, FcToCf5, Cf5ToFc, IsdToCi5, Ci5ToIsd }

        static void Main(string[] args)
        {
            string appName = "Anno 1800 FC-Converter";
            string version = "1.3.0";
            Console.WriteLine("{0}: Version {1}, use -help for explanation. ", appName, version);


            FCConverter p = new FCConverter();
            p.HandleFileRequest(args);
        }

        private void PrintHelp() {
            string HelpText = "Usage: \n" +
                FcToCf7 + " to convert Anno 1800 ." + FileFormatFc + " Files into ." + FileFormatCf7 + " Files. \n" +
                FcToCf5 + " to convert Anno 2070 ." + FileFormatFc + " Files into ." + FileFormatCf5 + " Files. \n" +
                Cf7ToFc + " to convert ." + FileFormatCf7 + " Files into Anno 1800 ." + FileFormatFc + " Files. \n" +
                Cf5ToFc + " to convert ." + FileFormatCf5 + " Files into Anno 2070 ." + FileFormatFc + " Files. \n" +
                IsdToCi5 + " to convert Anno 2070 ." + FileFormatIsd + " Files into ." + FileFormatCi5 + " Files. \n" +
                Ci5ToIsd + " to convert ." + FileFormatCi5 + " Files into Anno 2070 ." + FileFormatIsd + " Files. \n" +
                SpecificOutputFile + " <FileName> to set a specific output filename. \n" +
                Overwrite + " to overwrite an existing output file. \n" +
                ActivateErrorLogging + " to activate detailed Error Logging";

            Console.WriteLine(HelpText); 
        }

        private void HandleFileRequest(String[] args) 
        {
            Modes mode = Modes.noMode; 
            bool modeSet = false;
            string InputFileName = "";
            bool hasSpecificOutputFile = false;
            string OutputFileName = "";
            bool OverwriteFile = false;
            bool ErrorLoggingActivated = false;
            bool HelpRequested = false; 

            string ExpectedInputFormat = "";
            string ExpectedOutputFormat = "";

            for (int i = 0; i < args.Length; i++) {

                //Mode Fc to Cf7
                if (args[i].Equals(FcToCf7) && !modeSet)
                {
                    mode = Modes.FcToCf7;
                    modeSet = true;
                    ExpectedInputFormat = FileFormatFc;
                    ExpectedOutputFormat = FileFormatCf7;
                }

                //Mode Cf7 to Fc
                else if (args[i].Equals(Cf7ToFc) && !modeSet)
                {
                    mode = Modes.Cf7ToFc;
                    modeSet = true;
                    ExpectedInputFormat = FileFormatCf7;
                    ExpectedOutputFormat = FileFormatFc;
                }

                //Mode Fc to Cf5 (Anno 2070) 
                else if (args[i].Equals(FcToCf5) && !modeSet)
                {
                    mode = Modes.FcToCf5;
                    modeSet = true;
                    ExpectedInputFormat = FileFormatFc;
                    ExpectedOutputFormat = FileFormatCf5;
                }

                //Mode Cf5 to Fc (Anno 2070
                else if (args[i].Equals(Cf5ToFc) && !modeSet)
                {
                    mode = Modes.Cf5ToFc;
                    modeSet = true;
                    ExpectedInputFormat = FileFormatCf5;
                    ExpectedOutputFormat = FileFormatFc;
                }

                //Mode Isd to Ci5 (Anno 2070)
                else if (args[i].Equals(IsdToCi5) && !modeSet)
                {
                    mode = Modes.IsdToCi5;
                    modeSet = true;
                    ExpectedInputFormat = FileFormatIsd;
                    ExpectedOutputFormat = FileFormatCi5;
                }

                //Mode Ci5 to Isd (Anno 2070)
                else if (args[i].Equals(Ci5ToIsd) && !modeSet)
                {
                    mode = Modes.Ci5ToIsd;
                    modeSet = true;
                    ExpectedInputFormat = FileFormatCi5;
                    ExpectedOutputFormat = FileFormatIsd;
                }

                //parsing help request
                else if (args[i].Equals(Help))
                {
                    PrintHelp();
                    HelpRequested = true;
                }

                //parsing overwrite
                else if (args[i].Equals(Overwrite))
                {
                    OverwriteFile = true;
                }

                //parsing ErrorLogging 
                else if (args[i].Equals(ActivateErrorLogging)) {
                    ErrorLoggingActivated = true;
                }

                //Specific Output Files, will automatically take the next arg as Output Filename, unless it starts with a '-'. 
                else if (args[i].Equals(SpecificOutputFile))
                {
                    try
                    {
                        if (!args[i].StartsWith("-"))
                        {
                            OutputFileName = args[i + 1];
                            hasSpecificOutputFile = true;
                        }
                        else
                        {
                            Console.WriteLine("You forgot giving me an Output Filename.");
                        }

                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Console.WriteLine("You forgot giving me an Output Filename.");
                    }
                }

                //if the mode is set, you can now start with 
                else if (modeSet && args[i].EndsWith(ExpectedInputFormat))
                {
                    InputFileName = args[i];
                }

                else
                {
                    Console.WriteLine("Error: " + args[i] + " is an unrecognized Argument! Make sure to set the mode before you give the input filename!");
                }
            }

            //now everything should be parsed :) start with conversion. 

            //set the output file name
            if (!hasSpecificOutputFile) {
                OutputFileName = InputFileName.Split('.')[0] + "." + ExpectedOutputFormat; 
            }

            //check for wrong input formats and already existing output file. 
            bool FileTypeError = false; 
            if (!InputFileName.EndsWith(ExpectedInputFormat)) {
                Console.WriteLine("Error: Wrong Input Format!");
                FileTypeError = true; 
            }
            if (!OutputFileName.EndsWith(ExpectedOutputFormat)) {
                Console.WriteLine("Error: Wrong Output Format!");
                FileTypeError = true; 
            }
            if (File.Exists(OutputFileName) && !OverwriteFile) {
                Console.WriteLine("Error: Output File Already Exists! use " + Overwrite + " to Overwrite.");
                FileTypeError = true; 
            }

            if (!FileTypeError)
            {
                try
                {
                    if (mode == Modes.Cf7ToFc)
                    {
                        ConvertToFCFile(InputFileName, OutputFileName);
                        Console.WriteLine(InputFileName + " was converted to " + OutputFileName);
                    }
                    else if (mode == Modes.FcToCf7)
                    {
                        ConvertToHTMLFile(InputFileName, OutputFileName);
                        Console.WriteLine(InputFileName + " was converted to " + OutputFileName);
                    }
                    else if (!HelpRequested) {
                        Console.WriteLine("Error: No Mode for Conversion was set!"); 
                    }

                }
                catch (Exception e)
                {
                    if (ErrorLoggingActivated)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine();
                        Console.WriteLine(e.StackTrace);
                    }
                    else
                    {
                        Console.WriteLine("An unknown Error has occured. Please create an Issue on the Github Page with the file that caused this!");
                    }
                }
            }
        }   


        private void ConvertToFCFile(string InputPath, string OutputPath)
        {
            if (File.Exists(OutputPath))
            {
                File.Delete(OutputPath);
            }
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

                            //fuck empty tags... Check if Cdata exists first.
                            String CdataCheck = ""; 
                            for (int k = 0; k < 6; k++)
                            {
                                CdataCheck += c; 
                                c = (char)sr.Read();
                            }
                            //we advanced six characters - write them nonetheless, we still need em. 
                            sw.Write(CdataCheck);
                            if (CdataCheck.Equals("CDATA[")) {
                                
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
                        //reset the token
                        token = "";
                    }//as long as there is no opening < character the file gets parsed and added directly to the output 
                    else if (c != '<')
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
            if (File.Exists(OutputPath))
            {
                File.Delete(OutputPath);
            }
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
                            //write the check nonetheless, we need it. 
                            sw.Write(check);
                            if (check.Equals("CDATA["))
                            {
                                
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
                    else if (c != '<')
                    {
                        while (c != '<')
                        {
                            token += c;
                            c = (char)sr.Read();
                        }
                        sw.Write(token);
                        sw.Flush();
                        token = "";
                    }
                }
            }
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

        /// <summary>
        /// interprets CDATA as a sequence of ints. The first position in the returned Array is the byteSize which has to be used to advance to the end of cdata after calling this method. 
        /// </summary>
        /// <param name="hexData">The given File read into an array of hex numbers</param>
        /// <param name="offset">The Offset at which CDATA starts</param>
        /// <returns>CDATA interpreted soly as 32 bit integers</returns>
        private int[] ConvertCDATAToInt32(string[] hexData, int offset)
        {
            //parse the first four characters for the byte size
            string ByteSizeInHex = hexData[offset + 3] + hexData[offset + 2] + hexData[offset + 1] + hexData[offset];
            int ByteSize = int.Parse(ByteSizeInHex, System.Globalization.NumberStyles.HexNumber);
            List<int> IntList = new List<int>();
            IntList.Add(ByteSize);
            int BytesRead = 0;
            offset += 4;
            while (BytesRead <= ByteSize)
            {
                //makean int out of the next three positions (flipped) in hexData
                string intInHex = hexData[offset + 3] + hexData[offset + 2] + hexData[offset + 1] + hexData[offset];
                int intAgain = int.Parse(intInHex, System.Globalization.NumberStyles.HexNumber);
                IntList.Add(intAgain);

                //advance 4 times;
                offset += 4;
                BytesRead += 4;
            }
            return IntList.ToArray();
        }
        /// <summary>
        /// interprets CDATA as a sequence of ints. The first position in the returned Array is the byteSize which has to be used to advance to the end of cdata after calling this method. 
        /// </summary>
        /// <param name="hexData">The given File read into an array of hex numbers</param>
        /// <param name="offset">The Offset at which CDATA starts</param>
        /// <returns>CDATA interpreted as 32 bit floats (bytesize is still an int but has to be typecasted)</returns>
        private float[] ConvertCDATAToFloat(string[] hexData, int offset)
        {
            //parse the first four characters for the byte size
            string ByteSizeInHex = hexData[offset + 3] + hexData[offset + 2] + hexData[offset + 1] + hexData[offset];
            int ByteSize = int.Parse(ByteSizeInHex, System.Globalization.NumberStyles.HexNumber);
            List<float> FloatList = new List<float>();
            FloatList.Add(ByteSize);
            int BytesRead = 0;
            offset += 4;
            while (BytesRead <= ByteSize)
            {
                //makean int out of the next three positions (flipped) in hexData
                string floatInHex = hexData[offset + 3] + hexData[offset + 2] + hexData[offset + 1] + hexData[offset];
                uint num = uint.Parse(floatInHex, System.Globalization.NumberStyles.AllowHexSpecifier);
                byte[] floatVals = BitConverter.GetBytes(num);
                float floatAgain = BitConverter.ToSingle(floatVals, 0);

                FloatList.Add(floatAgain);

                //advance 4 times;
                offset += 4;
                BytesRead += 4;
            }
            return FloatList.ToArray();
        }

        /// <summary>
        /// interprets CDATA as a sequence of ints. The first position in the returned Array is the byteSize which has to be used to advance to the end of cdata after calling this method. 
        /// </summary>
        /// <param name="hexData">The given File read into an array of hex numbers</param>
        /// <param name="offset">The Offset at which CDATA starts</param>
        /// <returns>CDATA interpreted soly as 32 bit integers</returns>
        private uint[] ConvertCDATAToUint16(string[] hexData, int offset)
        {
            //parse the first four characters for the byte size
            string ByteSizeInHex = hexData[offset + 3] + hexData[offset + 2] + hexData[offset + 1] + hexData[offset];
            uint ByteSize = uint.Parse(ByteSizeInHex, System.Globalization.NumberStyles.HexNumber);
            List<uint> IntList = new List<uint>();
            IntList.Add(ByteSize);
            int BytesRead = 0;
            offset += 4;
            while (BytesRead <= ByteSize)
            {
                //makean int out of the current and next position (flipped) in hexData
                string intInHex = hexData[offset + 1] + hexData[offset];
                uint intAgain = uint.Parse(intInHex, System.Globalization.NumberStyles.HexNumber);
                IntList.Add(intAgain);

                //advance 2 times;
                offset += 2;
                BytesRead += 2;
            }
            return IntList.ToArray();
        }
        private uint[] ConvertCDATAToUint8(string[] hexData, int offset)
        {
            //parse the first four characters for the byte size
            string ByteSizeInHex = hexData[offset + 3] + hexData[offset + 2] + hexData[offset + 1] + hexData[offset];
            uint ByteSize = uint.Parse(ByteSizeInHex, System.Globalization.NumberStyles.HexNumber);
            List<uint> IntList = new List<uint>();
            IntList.Add(ByteSize);
            int BytesRead = 0;
            offset += 4;
            while (BytesRead <= ByteSize)
            {
                //makean int out of the current and next position (flipped) in hexData
                string intInHex = hexData[offset];
                uint intAgain = uint.Parse(intInHex, System.Globalization.NumberStyles.HexNumber);
                IntList.Add(intAgain);
                offset++;
                BytesRead++;
            }
            return IntList.ToArray();
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
            int offset = -1;

            if (File.Exists(OutputPath))
            {
                File.Delete(OutputPath);
            }
            FileStream fs = new FileStream(OutputPath, FileMode.CreateNew);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                offset++;
                char c = ToChar(HexData[offset]);

                while (offset < HexData.Length - 1)
                {
                    if (c == '<')
                    {
                        while (c != '>')
                        {
                            token += c;
                            offset++;
                            c = ToChar(HexData[offset]);

                        }
                        token += c;
                        if (offset < HexData.Length - 1)
                        {
                            offset++;
                            c = ToChar(HexData[offset]);
                        }

                        //one token should be complete
                        sw.Write(token);
                        sw.Flush();
                        //Check if token matches any important stuff, if so decrypt the CDATA
                        if (token.Equals("<GuidVariationList>") || token.Equals("<m_SequenceIds>") || token.Equals("<DummyGroup>") || token.Equals("<ValidSequenceIDs>") || token.Equals("<ApproximationMapping>") || token.Equals("<i>"))
                        {
                            //jump by CDATA so c equals '['
                            if (c == 'C')
                            {
                                while (c != '[')
                                {
                                    offset++;
                                    c = ToChar(HexData[offset]);
                                }
                                //write CDATA[ and advance by one character
                                offset++;
                                c = ToChar(HexData[offset]);


                                {
                                    sw.Write("CDATA[");
                                    //interpret cdata as an 32 bit int
                                    int[] CDATAAsInt = ConvertCDATAToInt32(HexData, offset);
                                    int ByteSize = CDATAAsInt[0];
                                    //advance to the end of cdata
                                    offset += ByteSize + 4;
                                    for (int i = 0; i <= ByteSize / 4; i++)
                                    {
                                        sw.Write(CDATAAsInt[i]);
                                        if (i < ByteSize / 4)
                                        {
                                            sw.Write(" ");
                                        }
                                    }
                                    c = ToChar(HexData[offset]);
                                    sw.Flush();
                                }
                            }
                        }
                        //reset the token
                        token = "";
                    }

                    //as long as there is no opening < character the file gets parsed and added directly to the output 
                    if (c != '<')
                    {
                        while (c != '<' && offset < HexData.Length - 1)
                        {
                            token += c;
                            offset++;
                            c = ToChar(HexData[offset]);

                        }
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
            int offset = -1;
            string token = "";
            if (File.Exists(OutputPath))
            {
                File.Delete(OutputPath);
            }
            FileStream fs = new FileStream(OutputPath, FileMode.CreateNew);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                offset++;
                char c = ToChar(HexData[offset]);

                while (offset < HexData.Length - 1)
                {
                    if (c == '<')
                    {
                        while (c != '>')
                        {
                            token += c;
                            offset++;
                            c = ToChar(HexData[offset]);

                        }
                        token += c;
                        if (offset < HexData.Length - 1)
                        {
                            offset++;
                            c = ToChar(HexData[offset]);
                        }
                        //one token should be complete
                        sw.Write(token);
                        sw.Flush();

                        //check if the token is useful
                        if (token.Equals("<GuidVariationList>") || token.Equals("<ValidSequenceIDs>") || token.Equals("<i>") || token.Equals("<Position>") || token.Equals("<Extents>") || token.Equals("<DummyGroup>") || token.Equals("<m_SequenceIds>"))
                        {
                            {
                                //jump by CDATA so c equals '['
                                if (c == 'C')
                                {
                                    while (c != '[')
                                    {
                                        offset++;
                                        c = ToChar(HexData[offset]);
                                    }
                                    //write CDATA[ and advance by one character
                                    offset++;
                                    c = ToChar(HexData[offset]);


                                    {
                                        sw.Write("CDATA[");
                                        //interpret cdata as an 32 bit int
                                        int[] CDATAAsInt = ConvertCDATAToInt32(HexData, offset);
                                        int ByteSize = CDATAAsInt[0];
                                        //advance to the end of cdata
                                        offset += ByteSize + 4;
                                        for (int i = 0; i <= ByteSize / 4; i++)
                                        {
                                            sw.Write(CDATAAsInt[i]);
                                            if (i < ByteSize / 4)
                                            {
                                                sw.Write(" ");
                                            }
                                        }
                                        c = ToChar(HexData[offset]);
                                        sw.Flush();
                                    }
                                }
                            }
                        }
                        else if (token.Equals("<Orientation>"))
                        {
                            {
                                //jump by CDATA so c equals '['
                                if (c == 'C')
                                {
                                    while (c != '[')
                                    {
                                        offset++;
                                        c = ToChar(HexData[offset]);
                                    }
                                    //write CDATA[ and advance by one character
                                    offset++;
                                    c = ToChar(HexData[offset]);


                                    {
                                        sw.Write("CDATA[");
                                        //interpret cdata as an 32 bit int
                                        float[] CDATAAsFloat = ConvertCDATAToFloat(HexData, offset);
                                        int ByteSize = (int)CDATAAsFloat[0];
                                        //advance to the end of cdata
                                        offset += ByteSize + 4;
                                        for (int i = 0; i <= ByteSize / 4; i++)
                                        {
                                            sw.Write(CDATAAsFloat[i]);
                                            if (i < ByteSize / 4)
                                            {
                                                sw.Write(" ");
                                            }
                                        }
                                        c = ToChar(HexData[offset]);
                                        sw.Flush();
                                    }
                                }
                                sw.Flush();
                            }
                        }
                        //reset it
                        token = "";
                    }


                    //as long as there is no opening < character the file gets parsed and added directly to the output 
                    if (c != '<')
                    {
                        while (c != '<' && offset < HexData.Length - 1)
                        {
                            token += c;
                            offset++;
                            c = ToChar(HexData[offset]);
                        }
                        sw.Write(token);
                        sw.Flush();

                        token = "";
                    }
                }
            }
        }




        /// <summary>
        /// First try to convert anno 2070 island files to html because they are similar in the way that they also use cdata for many things like heightmaps and positions. 
        /// The same algorithm, slightly modified and with other interpretations for cdata should give the possibility to analyze anno 2070 islands easily.
        /// </summary>
        /// 
        /// <param name="path">path of the input file</param>
        /// <param name="OutputPath">path of the output file</param>
        private void ConvertISDToHTML(string InputPath, string OutputPath)
        {
            string[] HexData = ReadFileToHex(InputPath);
            string token = "";
            string DataParseMode = "none";
            if (File.Exists(OutputPath))
            {
                File.Delete(OutputPath);
            }
            int offset = -1;
            FileStream fs = new FileStream(OutputPath, FileMode.CreateNew);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                offset++;
                char c = ToChar(HexData[offset]);

                while (offset < HexData.Length - 1)
                {
                    if (c == '<')
                    {
                        while (c != '>')
                        {
                            token += c;
                            offset++;
                            c = ToChar(HexData[offset]);

                        }
                        token += c;
                        if (offset < HexData.Length - 1)
                        {
                            offset++;
                            c = ToChar(HexData[offset]);
                        }
                        //one token should be complete
                        sw.Write(token);
                        sw.Flush();

                        //check if the token is useful and also setting if there was a height or alpha map mentioned
                        if (token.Equals("<HeightMap>"))
                        {
                            DataParseMode = "height";
                        }
                        if (token.Equals("<AlphaMap>"))
                        {
                            DataParseMode = "alpha";
                        }

                        //32 bit int conversion without division
                        if (token.Equals("<m_Connections>") || token.Equals("<m_BitGrid>") || token.Equals("<m_RenderParameters>"))
                        {

                            //jump by CDATA so c equals '['
                            if (c == 'C')
                            {
                                while (c != '[')
                                {
                                    offset++;
                                    c = ToChar(HexData[offset]);
                                }
                                //write CDATA[ and advance by one character
                                offset++;
                                c = ToChar(HexData[offset]);

                                {
                                    sw.Write("CDATA[");
                                    //interpret cdata as an 32 bit int
                                    int[] CDATAAsInt = ConvertCDATAToInt32(HexData, offset);
                                    int ByteSize = CDATAAsInt[0];
                                    //advance to the end of cdata
                                    offset += ByteSize + 4;
                                    for (int i = 0; i <= ByteSize / 4; i++)
                                    {
                                        sw.Write(CDATAAsInt[i]);
                                        if (i < ByteSize / 4)
                                        {
                                            sw.Write(" ");
                                        }
                                    }
                                    c = ToChar(HexData[offset]);
                                }
                            }
                            sw.Flush();

                        }
                        //conversion to 32 bit int with division by 4096
                        else if (token.Equals("<i>") || token.Equals("<m_Position>") || token.Equals("<m_StreetGrid>"))
                        {

                            //jump by CDATA so c equals '['
                            if (c == 'C')
                            {
                                while (c != '[')
                                {
                                    offset++;
                                    c = ToChar(HexData[offset]);
                                }
                                //write CDATA[ and advance by one character
                                offset++;
                                c = ToChar(HexData[offset]);
                                {
                                    sw.Write("CDATA[");
                                    //interpret cdata as an 32 bit int
                                    int[] CDATAAsInt = ConvertCDATAToInt32(HexData, offset);
                                    int ByteSize = CDATAAsInt[0];
                                    //advance to the end of cdata
                                    offset += ByteSize + 4;
                                    sw.Write(ByteSize);
                                    sw.Write(" ");
                                    for (int i = 1; i <= ByteSize / 4; i++)
                                    {
                                        sw.Write((float)CDATAAsInt[i] / 4096);
                                        if (i < ByteSize / 4)
                                        {
                                            sw.Write(" ");
                                        }
                                    }
                                    c = ToChar(HexData[offset]);
                                }
                            }
                            sw.Flush();

                        }
                        //float
                        else if (token.Equals("<m_Orientation>") || token.Equals("<Position>") || token.Equals("<p>"))
                        {

                            //jump by CDATA so c equals '['
                            if (c == 'C')
                            {
                                while (c != '[')
                                {
                                    offset++;
                                    c = ToChar(HexData[offset]);
                                }
                                //write CDATA[ and advance by one character
                                offset++;
                                c = ToChar(HexData[offset]);


                                {
                                    sw.Write("CDATA[");
                                    //interpret cdata as an 32 bit int
                                    float[] CDATAAsFloat = ConvertCDATAToFloat(HexData, offset);
                                    int ByteSize = (int)CDATAAsFloat[0];
                                    //advance to the end of cdata
                                    offset += ByteSize + 4;
                                    for (int i = 0; i <= ByteSize / 4; i++)
                                    {
                                        sw.Write(CDATAAsFloat[i]);
                                        if (i < ByteSize / 4)
                                        {
                                            sw.Write(" ");
                                        }
                                    }
                                    c = ToChar(HexData[offset]);
                                }
                            }
                            sw.Flush();

                        }

                        //16 Bit uint 
                        else if (token.Equals("<m_HeightMap_v2>"))
                        {

                            //jump by CDATA so c equals '['
                            if (c == 'C')
                            {
                                while (c != '[')
                                {
                                    offset++;
                                    c = ToChar(HexData[offset]);
                                }
                                //write CDATA[ and advance by one character
                                offset++;
                                c = ToChar(HexData[offset]);
                                int breakpointInt = 1;
                                {
                                    sw.Write("CDATA[");
                                    //interpret cdata as an 32 bit int
                                    uint[] CDATAAsInt = ConvertCDATAToUint16(HexData, offset);
                                    int ByteSize = (int)CDATAAsInt[0];
                                    //advance to the end of cdata
                                    offset += ByteSize + 4;
                                    sw.Write(ByteSize);
                                    sw.Write("\n\t\t");
                                    for (int i = 1; i <= ByteSize / 2; i++)
                                    {
                                        sw.Write((float)CDATAAsInt[i] / 4096);
                                        if (breakpointInt % 17 == 0)
                                        {
                                            sw.Write("\n\t\t");
                                            breakpointInt++;
                                        }
                                        else if (i < ByteSize / 2)
                                        {
                                            sw.Write(" ");
                                            breakpointInt++;
                                        }
                                    }
                                    c = ToChar(HexData[offset]);
                                }
                            }
                            sw.Flush();
                        }


                        //for a heightmap this needs to be 32 bit int, for a alphamap this needs to be 8 bit uint
                        else if (token.Equals("<Data>"))
                        {
                            if (DataParseMode.Equals("height"))
                            {
                                //jump by CDATA so c equals '['
                                if (c == 'C')
                                {
                                    while (c != '[')
                                    {
                                        offset++;
                                        c = ToChar(HexData[offset]);
                                    }
                                    //write CDATA[ and advance by one character
                                    offset++;
                                    c = ToChar(HexData[offset]);
                                    int breakpointInt = 0;

                                    {
                                        sw.Write("CDATA[");
                                        //interpret cdata as an 32 bit float
                                        float[] CDATAAsFloat = ConvertCDATAToFloat(HexData, offset);
                                        int ByteSize = (int)CDATAAsFloat[0];
                                        //advance to the end of cdata
                                        offset += ByteSize + 4;
                                        for (int i = 0; i <= ByteSize / 4; i++)
                                        {
                                            sw.Write(CDATAAsFloat[i]);
                                            if (breakpointInt % 17 == 0)
                                            {
                                                sw.Write("\n\t\t\t\t\t");
                                                breakpointInt++;
                                            }
                                            else if (i < ByteSize / 4)
                                            {
                                                sw.Write(" ");
                                                breakpointInt++;
                                            }
                                        }
                                        c = ToChar(HexData[offset]);
                                    }
                                }
                                sw.Flush();
                            }
                            else if (DataParseMode.Equals("alpha"))
                            {
                                {
                                    //jump by CDATA so c equals '['
                                    if (c == 'C')
                                    {
                                        while (c != '[')
                                        {
                                            offset++;
                                            c = ToChar(HexData[offset]);
                                        }
                                        //write CDATA[ and advance by one character
                                        offset++;
                                        c = ToChar(HexData[offset]);
                                        int breakpointInt = 0;
                                        {
                                            sw.Write("CDATA[");
                                            //interpret cdata as an 8 bit uint
                                            uint[] CDATAAsInt = ConvertCDATAToUint8(HexData, offset);
                                            int ByteSize = (int)CDATAAsInt[0];
                                            //advance to the end of cdata
                                            offset += ByteSize + 4;
                                            for (int i = 0; i <= ByteSize; i++)
                                            {
                                                sw.Write(CDATAAsInt[i]);
                                                if (breakpointInt % 17 == 0)
                                                {
                                                    sw.Write("\n\t\t\t\t\t");
                                                    breakpointInt++;
                                                }
                                                else if (i < ByteSize)
                                                {
                                                    sw.Write(" ");
                                                    breakpointInt++;
                                                }
                                            }
                                            c = ToChar(HexData[offset]);
                                        }
                                    }
                                    sw.Flush();
                                }
                            }
                        }
                        token = "";
                    }


                    //as long as there is no opening < character the file gets parsed and added directly to the output 
                    if (c != '<')
                    {
                        while (c != '<' && offset < HexData.Length - 1)
                        {
                            token += c;
                            offset++;
                            c = ToChar(HexData[offset]);

                        }
                        sw.Write(token);
                        sw.Flush();
                        token = "";
                    }
                }
            }
        }
        /// <summary>
        /// Converts html code into an anno 2070 island file. 
        /// How CDATA gets written:
        /// 
        /// <Height_Map_v2>: 16 bit uint
        /// <m_Orientation>, <Position>, <p>: float
        /// <m_Position>, <m_StreetGrid>, <i>: 32 bit int * 4096
        /// <m_BitGrid>, <m_RenderParameters>, <m_Connections>: 32 bit int
        /// <Data>: 32 bit float for Height and 8 bit int for alpha 
        /// </summary>
        /// <param name="InputPath"></param>
        /// <param name="OutputPath"></param>
        private void ConvertToISDFile(String InputPath, String OutputPath)
        {
            if (File.Exists(OutputPath))
            {
                File.Delete(OutputPath);
            }
            FileStream fs = new FileStream(OutputPath, FileMode.CreateNew);
            using (StreamReader sr = new StreamReader(InputPath))
            using (BinaryWriter bw = new BinaryWriter(fs))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                char c = (char)sr.Read();
                string token = "";
                String DataParseMode = "";
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

                        //check if its useful
                        if (token.Equals("<HeightMap>"))
                        {
                            DataParseMode = "height";
                        }
                        if (token.Equals("<AlphaMap>"))
                        {
                            DataParseMode = "alpha";
                        }
                        if (token.Equals("<m_BitGrid>") || token.Equals("m_Connections") || token.Equals("<m_RenderParameters>"))
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

                        //reset the token
                        token = "";
                    }
                    //conversion with float
                    if (token.Equals("<m_Orientation>") || token.Equals("<Position>") || token.Equals("<p>"))
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
                    //conversion with 32 bit int * 4096
                    if (token.Equals("<m_Orientation>") || token.Equals("<Position>") || token.Equals("<p>"))
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
                                    int IntForm = Int32.Parse(s) * 4096;
                                    bw.Write(IntForm);
                                }
                            }
                        }
                    }

                    //conversion with 16 bit int (short)
                    if (token.Equals("<m_HeightMap_v2>"))
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
                                    short ShortForm = short.Parse(s);
                                    bw.Write(ShortForm);
                                }
                            }
                        }
                    }

                    //flexible conversion for <Data>
                    if (token.Equals("<Data>"))
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
                                else if (!isFirst && DataParseMode.Equals("height"))
                                {
                                    float FloatForm = float.Parse(s);
                                    bw.Write(FloatForm);
                                }

                                else if (!isFirst && DataParseMode.Equals("alpha"))
                                {
                                    byte ByteForm = byte.Parse(s);
                                    bw.Write(ByteForm);
                                }
                            }
                        }
                    }
                    //as long as there is no opening < character the file gets parsed and added directly to the output 
                    else if (c != '<')
                    {
                        while (c != '<')
                        {
                            token += c;
                            c = (char)sr.Read();
                        }
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
