REM
REM  Builds the cs files and the cxx file from the BWAPI  and BWTA headers for the bwapi-clr-client implementation.
REM



:start
SET SWIGPATH=..\..\..\..\swigwin-2.0.1

erase /s /q Classes\*.*
erase /s /q Wrapper\*.*

%SWIGPATH%\swig.exe -csharp -c++ -I..\BWAPI\Include -outdir Classes\BWAPI -namespace SWIG.BWAPI -dllimport \"+importdll+\" -o Wrapper\bwapi-bridge.cxx Interfaces\bwapi-bridge.i

%SWIGPATH%\swig.exe -csharp -c++ -I..\BWAPI\Include -outdir Classes\BWTA -namespace SWIG.BWTA -dllimport \"+importdll+\" -o Wrapper\bwta-bridge.cxx Interfaces\bwta-bridge.i

%SWIGPATH%\swig.exe -csharp -c++ -I..\BWAPI\Include -outdir Classes\BWAPIC -namespace SWIG.BWAPIC -dllimport \"+importdll+\" -o Wrapper\bwapiclient-bridge.cxx Interfaces\bwapiclient-bridge.i

erase /q Classes\BWAPIC\swigtype_p_void.cs
erase /q Classes\BWAPIC\swigtype_p_int.cs

pause
goto start

