BUILD_DIR = build
EXECUTE_DIR = bin

ifeq ($(OS),Windows_NT)
# GENERATOR = -G "Visual Studio 17 2022"
GENERATOR = -G "MinGW Makefiles"
CLEAN_FLAGS = rd /s /q
endif

ifeq ($(OS),Linux)
GENERATOR = -G "Unix Makefiles"
endif

all: build
	
build: clean
	CC=gcc CXX=g++ cmake -B $(BUILD_DIR) &&\
	cd $(BUILD_DIR) &&\
	$(MAKE) 
	dotnet run

clean:
	rm -rf $(BUILD_DIR) ${EXECUTE_DIR}

deb: clean
	CC=gcc CXX=g++ cmake -B $(BUILD_DIR) &&\
	cd $(BUILD_DIR) &&\
	$(MAKE) 
	dotnet build
	cp -r documentation ../package
	cp -r bin/Debug/net6.0 ../package
	cd ../package && dpkg-buildpackage -uc -us -b &&\
	rm -rf documentation net6.0 debian/.debhelper debian/calculator3 debian/calculator3.post* debian/calculator3.substvars debian/files

install:
	cd .. && sudo dpkg -i calculator3_1.0.0_amd64.deb





