# NISTscan

NISTscan is a Windows application for scanning fingerprint imagery using a third-party TWAIN library (Atalasoft DotTWAIN**). 

NISTscan supports the digitization of fingerprint impressions on physical media, such as 10-print FD-249 fingerprint cards, at sample rates of 500, 1000, and 2000 PPI in 8-bit or 16-bit grayscale as well as 24-bit color depths. NISTscan also supports semi-automated (user-assisted) segmentation of scanned fingerprint imagery as well as batch processing.


## Contact
Developer:    John Grantham (john.grantham@nist.gov)  
Project Lead: John Libert (john.libert@nist.gov)


## System Requirements for Running NISTscan

In order to properly digitize fingerprint imagery at all of the resolutions and depths specified above, NISTscan requires the following hardware specifications:

* Microsoft Windows 7 or higher
* 2.0 GHz or faster processor
* 8 GB of RAM
* 5 GB of hard drive space (for storing scanned images)


## Pre-requisites for Building NISTscan

NISTscan utilizes the third-party Atalasoft DotTWAIN** library to support scanning using TWAIN-compliant devices. The DotTWAIN library (version 10.7 or higher) must be installed, along with a valid license (full or trial), prior to building the NISTscan project. Please see http://www.atalasoft.com for more information. 


## Visual Studio 2013 Build Instructions

1. Open the Solution File (NISTscan.sln)
2. Check that the build mode is set to "Release"
3. Click "Build Solution" under the "Build" menu option
4. Check the Visual Studio output console for any errors (warnings are OK)
5. If the build is successful, the resulting executable (NISTscan.exe) will be placed in the "bin" sub-directory

NOTE: If you plan to distribute or copy the NISTscan.exe, the .DLL files in the bin directory must be distributed along with the executable. The target machine may also require the Visual Studio 2013 redistributable package: https://www.microsoft.com/en-us/download/details.aspx?id=40784


## Configuring NISTscan

NISTscan's settings are stored in the NISTscan.ini file (under the "bin" directory). Settings that may be altered include:

* Save Path: The system path where scanned images should be saved
* Scanner ID: An identifier that will be inserted into the filenames of scanned images

We do not currently recommend altering any other settings, however if you have questions regarding these settings you may contact the author or project leader listed in the Contact section above.


## Disclaimer

** In no case does identification of any commercial product imply endorsement by the National Institute of Standards and Technology, nor does it imply that the products and equipment identified are necessarily the best available for the purpose.

See the NIST disclaimer at https://www.nist.gov/disclaimer
