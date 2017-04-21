.NET Mass Downloader

With the ms release of sourcecode in msi files, netmassdownloader became somehow absolute, you can still use it for your express editions, but for vs2010 there is no longer a need to use netmassdownloader.
Just download the sourcecode from referencesourcecode.microsoft.com.

Welcome to the .NET Mass Downloader project. While it’s great that Microsoft has released the .NET Reference Source Code, you can only get it one file at a time while you’re debugging. If you’d like to batch download it for reading or to populate the cache, you’d have to write a program that instantiated and called each method in the Framework Class Library. Fortunately, .NET Mass Downloader comes to the rescue! 

Mass Downloader For the .Net Framework which allows you do download .Net Framework source code in batch mode.
The tool which enables offline debugging of .Net Framework in VS2010 , VS2008 (including Express Editions) , VS2005 (including Express Editions), and Codegear Rad Studio.
Reference Source Server is up again.

Check The Documentation Section For The Usage Details.

Warning

If you've got an error message: There is no value set for the Visual Studio debugger symbol cache. Do next steps: Within VS Tools -> Options ->Debugging ->General check "Enable .NET Framework source stepping" option and specify symbol cache folder over there: Tools -> Options ->Debugging ->Symbols Don't place that folder too deep. Threre will be created deep-embeded folders as it is, and you likely to get an error that the length of path + filename is longer than 260 chars.
Improvements in Version 1.6

Once MS Releases The SourceCodes, this version can be used to download them for VS2010.

NetMassDownloader Updated To Support VS2010, but it seems that MS didn't released the sourcecodes for VS2010 now,
the pdbs are there, but the sourcecodes are not ready and also cannot be downloaded from VS2010.

+ NetMassDownloader compitable with VS2010 and .Net Framework 4.0.
+The sourcefile targets are now extracted from srvsrc files and no longer hardcoded.(Allowing Correct Download Of SourceCodes For Older Versions For Example For Vista SP2)
+The supported VS Versions are added to the config file and can be extended without changing the sourcecode.
+General Code Cleanup Applied.
+Solution File is Upgraded From 2005 To VS2008 and a VS2010 Solution File Added.
+The Project binary version is still targeted for .Net Framework v2.0.
Improvements in Version 1.5

.NET Mass Downloader now supports downloading the source code for .NET 3.5 SP1 and filling the symbol and source server cache for VS 2008 SP1. It also includes numerous bug fixes and feature tweaks that we are sure you will enjoy.
Improvements In Version 1.3

Thanks to Doug’s efforts we have the following improvements in this release.
- Allows processing DLL or EXE files in sub-folders recursively (in addition to the top directory).
- Allows cleanup of temporary compressed *.pd_ files.
- Allows downloading retail symbols for native DLL or EXE files from the Microsoft Symbol Server (msdl.microsoft.com) (by setting UseReferenceSourceServer to "false" in NetMassDownloader.exe.config).
- Makes the URLs of the symbol servers configurable in NetMassDownloader.exe.config.
- Allows specifying whether to download only symbols without source code via the setting DownloadSourceCode ("true" by default) in NetMassDownloader.exe.config.
- Use the original date-time of files from the server if available.
- Download is now asynchronous; the download progress can be seen for each file (symbol or source code).
- Improves the console output. It is less verbose and now easy to read.
- Allows resuming the download remaining source code files for an existing symbol file (by setting SkipExistingSourceFiles to "true" (default value) in NetMassDownloader.exe.config).
All of these features can be controlled via the NetMassDownloader.exe.config file which has the following switches, which are self explaining.

<appSettings>
        <add key="ProcessInputDirRecursively" value="true" />
        <add key="CleanupTempCompressedSymbols" value="true" />      
        <add key="UseReferenceSourceServer" value="true" />
        <add key="DownloadSymbols" value="true" />
        <add key="DownloadSourceCode" value="true" />
        <add key="SkipExistingSourceFiles" value="true" />
        <add key="DefaultSymbolServerUrl" value="http://msdl.microsoft.com/download/symbols/" />
        <add key="ReferenceSourceServerUrl" value="http://referencesource.microsoft.com/symbols/" />
    </appSettings>

Using .NET Mass Downloader

Open a command or PowerShell prompt and navigate to where you extracted the current release. The tool itself is NetMassDownloader.exe and when run without parameters shows the following help screen:
.Net Mass Downloader 1.5.0.0 - (c) 2008 by Kerem Kusmezer, John Robbins

Batch download the Microsoft .NET Reference Source code.

Usage: NetMassDownloader [-file <file>]
                         [-directory <directory]
                         [-output <directory>]
                         [-vsver <version>]
                         [-proxy server|username|password|domainname]
                         [-force] [-nologo] [-verbose] [-?]

    -file      - Download an individual file's PDB and source code. You can
                 specify multiple file parameters. (Short -f).
    -directory - Download all the found PDB and source code for all files in
                 the specified directory. You can specify multiple
                 directory parameters (Short -d).
    -output    - The output directory for PDB and source files. The default
                 directory is the cache directory set in Visual Studio 2008.
                 By using the cache directory, you'll have the PDB and source
                 files available to Visual Studio 2008. However, to use the
                 .NET Reference Source Code with VS 2005, use the -output
                 switch and in the Options dialog, Debugging, Symbols property
                 page, add the specified output directory to the "Symbol file
                 (.pdb) locations." Also, add the directory to the Solution
                 Properties, Common Properties, Debug Source Files, Directories
                 containing source code location. The Visual Studio 2005
                 debugger will automatically load the source code. (Short -o)
    -vsver     - The Visual Studio version number to use for finding the cache
                 directory. The default is Visual Studio 2008,
                 but if you want to use the cache directory for Visual Studio
                 2005, you would pass '-vsver 8.0' (without quotes) (Short -vs)
    -force     - If specified, forces the downloading the PDB files into the
                 symbol server. When downloading to a symbol server if the PDB
                 exists, it's not downloaded. Using the -output switch will
                 always download and process the PDB. (Short -fo)
    -nologo    - Don't show the logo information. (Short -n)
    -verbose   - Do verbose output. May be worth turning on as the downloading
                 source code can take a long time. (Short -v)
    -proxy     - Some proxies require credentials in order for the download to
                 work. The syntax required is
                 "http://testserver:80|username|password|domainname" or
                 "http://testserver:80|username|password"
                 For more information about using this flag, see discussion of
                 bug 1133 at http://www.codeplex.com/NetMassDownloader for more
                 details. (Short -p)
    -?         - This help message.

The only required arguments are –file or –directory, both of which can be specified as many times as you’d like. When you specify a directory, only the .DLL and .EXE files from that directory will be processed. If you wanted to download all the source code from binaries in the .NET 2.0 32-bit and 64-bit directories, the command line you’d pass is: -d C:\Windows\Microsoft.NET\Framework\v2.0.50727 –d "c:\Program Files\Reference Assemblies”.

The main purpose of Net Mass Downloader is to populate the source code download cache for debugging, the default download location is the cache you specified to Visual Studio 2008. The –vsver switch to account for future Visual Studio versions so Mass Downloader could work with future CTPs and versions. 

While it’s great to see the .NET Reference Source Code in Visual Studio 2008, there are a lot of developers out there who can’t upgrade yet, but would love to be able to debug into the .NET Reference Source Code. If you specify the -output parameter, the PDB and .NET Reference Source Code will be written to the specified directory. In Visual Studio 2005, place that directory in the Options dialog, Debugging, Symbols property page. In the “Symbol file (.pdb) locations” list box as the first item. Also in the Options dialog, Debugging, General property page, uncheck "Require source files to exactly match the original version" and "Enable Just My Code (Manged code only)". Finally, in each Visual Studio 2005 project go into the solution property pages, Common Properties, Debugging Source Files, and in the "Directories containing source code" add the output directory to the top of the list. That's enough for Visual Studio 2005 to debug into the .NET Reference Source Code.

When you first run Net Mass Downloader, you will be prompted with the current EULA for accessing the source code. If you don’t agree with the Microsoft EULA, clicking the Decline button will not download the source code.

The code download includes a detailed documentation about the steps required to use it with VS2005 

If none of the files can be downloaded please download and install the RTM Version of the .NET Framework 3.5 SP1. Nearly all the problems people have with NetMassDownloader is that they do not have the appropriate version of the .NET Framework on your machine. Microsoft only supports the .NET Framework 3.5 and 3.5 SP1 for the .NET Reference Source. Even if you are only using Visual Studio 2005, you must install the .NET Framework 3.5/3.5 SP1.

According to the Shawn Burke's blog the following DLLs are enabled for the .NET Reference Source Code:
Mscorlib.DLL 
System.DLL 
System.Data.DLL 
System.Drawing.DLL 
System.Web.DLL 
System.Web.Extensions.DLL 
System.Windows.Forms.DLL 
System.XML.DLL 
WPF (UIAutomation.DLL, 
System.Windows.DLL, 
System.Printing.DLL, 
System.Speech.DLL, 
WindowsBase.DLL, 
WindowsFormsIntegration.DLL, 
Presentation.DLL 
Microsoft.VisualBasic.DLL 

Net Mass Downloader will download any additional DLLs Microsoft configures to use with .NET Reference Source Code in the future, provided Microsoft doesn't change the download engine. 

The original .NET Reference Source Code announcement can be found here:
Shawn's Announcement
Downloading .Net Framework 3.0 And 3.5 Libraries

The framework v3.5 SP1, v3.5 and v3.0 assemblies are located under c:\program files\reference assemblies\microsoft\framework\v3.0 and c:\program files\reference assemblies\microsoft\framework\v3.5. To download everything, issue the following command:

netmassdownloader –d c:\windows\Microsoft.NET –d “c:\Program Files\Reference Assemblies”
Acknowledgements

Thanks to the Developer Division at Microsoft. First they released the .NET Reference Source Code, and second for allowing a couple of developers to have some fun and provide a utility for the community. Thank you for using .NET Mass Downloader. We just ask that you log any bugs and features into the project Issue Tracker.
Contact Information

You can reach us via the discussions or you can directly mail Kerem at izzetkeremskusmezer@gmail.com and John at john@wintellect.com
Development Details

If you have questions about particular pieces of the code, Kerem Kusmezer did the following parts: the PE (Portable Executable) Parser, the PDB Parser, the Webclient Class. and the SrcSrv class. John Robbins did the console driver, testing, and served as Kerem's code monkey.

Kerem Kusmezer and John Robbins
