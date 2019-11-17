# AnnoFCConverter
Allows to convert Anno 1800 and 2070 .fc files into html format which can be read by a human with a text editor like vsc and back into .fc
to use it in Anno.

The converter works through powershell. Shift + Rightclick in windows explorer to open up a Powershell window and then start 
AnnoFCConverter.exe with some of those arguments.

-r <InputFilename> for 1800 and -rz <InputFilename> for 2070 to make  <InputFilename> editable
-w <InputFilename> for 1800 and -wz <InputFilename> for 2070 to bring <InputFilename> back into .fc
-o <OutputFilename> to set a specific output filename
-y to overwrite the output File

So i.e. to make crops_farm.fc (hypothetical 1800 .fc) for you should type AnnoFCConverter.exe -r crops_farm.fc

thanks to judekw for figuring out how the .fc files work and to meow for taking a look at my code!


How CDATA is coded inside island files: 

Height_Map_v2: 16 bit uint

m_Orientation, Position, p: float

m_Position, m_StreetGrid, i: 32 bit int (values are devided by 4096 in the converted .html for readability)

m_BitGrid, m_RenderParameters, m_Connections> 32 bit int

Data: 32 bit float for height and 8 bit int for alpha
