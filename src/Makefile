GXX = g++
GXX_FLAGS = -Wall -Wextra -O2 -g
DOXYGEN=doxygen
INNO=iscc
DOTNET=dotnet
CMAKE=cmake
MAKE=make
PYTHON=python3

.PHONY: clean

default: all

all: publish profile

build:
	@.\IvsCalc\build.bat '\' 'q'

publish: build
	$(DOTNET) publish ./IvsCalc/IvsCalc/IvsCalc.csproj /p:PublishProfile=./IvsCalc/IvsCalc/Properties/PublishProfiles/win10-x86.pubxml
	$(DOTNET) publish ./IvsCalc/IvsCalc/IvsCalc.csproj /p:PublishProfile=./IvsCalc/IvsCalc/Properties/PublishProfiles/win10-x64.pubxml

installer: publish
	$(INNO) IvsCalc\installer\install_x86.iss
	$(INNO) IvsCalc\installer\install_x64.iss

pack: doc installer profile
	mkdir ..\xshche05_xklepa05_xroman18_xborys03
	mkdir ..\xshche05_xklepa05_xroman18_xborys03\install
	mkdir ..\xshche05_xklepa05_xroman18_xborys03\repo

	copy ..\src\IvsCalc\installer\output\* ..\xshche05_xklepa05_xroman18_xborys03\install
	copy profile.exe ..\xshche05_xklepa05_xroman18_xborys03\install
	Xcopy ..\doc\ ..\xshche05_xklepa05_xroman18_xborys03\doc /E /H /C /I
	
	git clone .. ..\xshche05_xklepa05_xroman18_xborys03\repo
	
	cd .. && tar -a -cf xshche05_xklepa05_xroman18_xborys03.zip xshche05_xklepa05_xroman18_xborys03

	rmdir /s /q ..\xshche05_xklepa05_xroman18_xborys03

clean_test:
ifneq ($(OS),Windows_NT)
	@echo "Cleaning..."
	@rm -rf ./IvsCalc/IvsCalcMathLib/build/
	@echo "Done"
else
	@if exist .\IvsCalc\IvsCalcMathLib\build (rmdir /s /q .\IvsCalc\IvsCalcMathLib\build 2>nul)
endif

test: clean_test
ifneq ($(OS),Windows_NT)
	@echo "Generating tests..."
	@cd ./IvsCalc/IvsCalcMathLib/ && ($(PYTHON) generate_tests.py 100) > /dev/null
	@echo "Configuring CMake Project..."
	@$(CMAKE) -S ./IvsCalc/IvsCalcMathLib/ -B ./IvsCalc/IvsCalcMathLib/build/ -G "Unix Makefiles"
	@echo "Building..."
	@$(MAKE) -C ./IvsCalc/IvsCalcMathLib/build/
	@echo "Running..."
	@./IvsCalc/IvsCalcMathLib/build/IvsCalcMathLibTest
	@echo "Done"
else
	@echo "Use WSL to run tests on Windows"
endif

doc:
	$(DOXYGEN) doxyfile

run: publish
	.\IvsCalc\IvsCalc\bin\publish\win10-x64\IvsCalc.exe

profile: ./Profiling/main.cpp ./IvsCalc/IvsCalcMathLib/IvsCalcMathLib.cpp ./IvsCalc/IvsCalcMathLib/IvsCalcMathLib.h
	$(GXX) $(GXX_FLAGS) -pg -c -o lib.o ./IvsCalc/IvsCalcMathLib/IvsCalcMathLib.cpp
	$(GXX) $(GXX_FLAGS) -c -o main.o ./Profiling/main.cpp
	$(GXX) $(GXX_FLAGS) -pg -o profile main.o lib.o -static-libstdc++ -static-libgcc

clean: clean_test
	@del /q profile 2>nul
	@del /q report.* 2>nul
	@del /q *.o 2>nul
	@del /q *.out 2>nul
	@del /q IvsCalc\IvsCalcMathLib\*.obj 2>nul
	@del /q IvsCalc\IvsCalcMathLib\*.pdb 2>nul
	@del /q IvsCalc\IvsCalcMathLib\*.ilk 2>nul
	@del /q IvsCalc\IvsCalcMathLib\*.exp 2>nul
	@del /q IvsCalc\IvsCalcMathLib\*.lib 2>nul
	@del /q IvsCalc\IvsCalcMathLib\*.log 2>nul
	@del /q IvsCalc\IvsCalcMathLib\*.pch 2>nul
	@del /q IvsCalc\IvsCalcMathLib\*.recipe 2>nul
	@del /q IvsCalc\IvsCalcMathLib\*.iobj 2>nul
	@del /q IvsCalc\IvsCalcMathLib\*.ipdb 2>nul
	@del /q IvsCalc\IvsCalcMathLib\*.idb 2>nul
	@del /q profile.exe 2>nul
	@del /q IvsCalc\installer\output\*.exe 2>nul
	@del /q IvsCalc\IvsCalc\Libraries\* 2>nul
	@if exist IvsCalc\IvsCalc\bin (rmdir /s /q IvsCalc\IvsCalc\bin 2>nul)
	@if exist IvsCalc\IvsCalc\obj (rmdir /s /q IvsCalc\IvsCalc\obj 2>nul)
	@if exist IvsCalc\IvsCalcMathLib\obj (rmdir /s /q IvsCalc\IvsCalcMathLib\obj 2>nul)
	@if exist IvsCalc\IvsCalcMathLib\IvsCalcMathLib.tlog (rmdir /s /q IvsCalc\IvsCalcMathLib\IvsCalcMathLib.tlog 2>nul)
	@if exist IvsCalc\.vs (rmdir /s /q IvsCalc\.vs 2>nul)
	@if exist ..\.vs (rmdir /s /q ..\.vs 2>nul)
	@if exist ..\.idea (rmdir /s /q ..\.idea 2>nul)
	@if exist ..\doc (rmdir /s /q ..\doc 2>nul)