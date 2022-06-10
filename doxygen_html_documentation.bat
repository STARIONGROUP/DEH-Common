@ECHO OFF

REM doxygen_html_documentation.bat
REM
REM Copyright (c) 2020-2022 RHEA System S.A.
REM
REM Author: Sam Gerené, Alex Vorobiev, Nathanael Smiechowski, Théate Antoine
REM
REM This file is part of DEHP Common Library
REM 
REM The DEHPCommon is free software; you can redistribute it and/or
REM modify it under the terms of the GNU Lesser General Public
REM License as published by the Free Software Foundation; either
REM version 3 of the License, or (at your option) any later version.
REM 
REM The DEHPCommon is distributed in the hope that it will be useful,
REM but WITHOUT ANY WARRANTY; without even the implied warranty of
REM MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
REM Lesser General Public License for more details.
REM 
REM You should have received a copy of the GNU Lesser General Public License
REM along with this program; if not, write to the Free Software Foundation,
REM Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

set QuietMode="-q"
set DOXYGEN_GENERATE_LATEX=NO
set CurrentDir=%cd%
set ZipFileName="html.zip"

:ParseArgs
if "%1"=="" goto :ParsingEnded
if /I "%1" == "-v" set QuietMode=
if /I "%1" == "-p" set DOXYGEN_GENERATE_LATEX=YES
if /I "%1" == "-h" goto :Help
shift
goto :ParseArgs

:Help
echo ^=======================================================================================================
echo ^=                   Generate the HTML documentation for the DEH-Common Library                        =
echo ^=                                                                                                     =
echo ^= --Arguments--                                                                                       =
echo ^= \-h ===^> Shows this menu                                                                            =
echo ^= \-v ===^> Runs the script on verbose mode                                                            =
echo ^= \-p ===^> Generate the PDF Documentation. LaTeX has to be installed to fully perform                 =
echo ^=======================================================================================================
goto :ExitStatement

:ParsingEnded

echo HTML Documentation generation started... 

doxygen %QuietMode% DEH-Common-doxygen

if not "%ERRORLEVEL%"=="0" goto :ExitStatement

del %ZipFileName% 2> nul
cd doc/html
zip -r ../../%ZipFileName% * %QuietMode%
cd "%CurrentDir%"

if %DOXYGEN_GENERATE_LATEX%==NO goto :ExitStatement
echo Generating PDF Starting...
if %QuietMode%=="-q" goto :SilentMakeFile else goto :NotSilentMakeFile

:NotSilentMakeFile
call doc/latex/make.bat 

:SilentMakeFile
call doc/latex/make.bat > nul 

:ExitStatement
cd "%CurrentDir%"
echo doxygen_html_documentation.bat is done
