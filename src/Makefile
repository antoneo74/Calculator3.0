BUILD_DIR = build
EXECUTE_DIR = bin/Debug/net6.0
DLL = CalculatorDll
CLEAN_FLAGS = rm -r

ifeq '$(findstring ;,$(PATH))' ';'
    UNAME := Windows
else
    UNAME := $(shell uname 2>/dev/null || echo Unknown)
    UNAME := $(patsubst CYGWIN%,Cygwin,$(UNAME))
    UNAME := $(patsubst MSYS%,MSYS,$(UNAME))
    UNAME := $(patsubst MINGW%,MSYS,$(UNAME))
endif

ifeq ($(UNAME),Windows)
BUILD_FLAGS = -G "MinGW Makefiles"
endif

ifeq ($(UNAME),Linux)
BUILD_FLAGS =
endif

all: build

build:
	mkdir ${BUILD_DIR} &&\
    mkdir -p ${EXECUTE_DIR} &&\
    cmake -B ${BUILD_DIR} ${BUILD_FLAGS} &&\
    cd ${BUILD_DIR} &&\
    make &&\
    cp ${DLL}.* ../${EXECUTE_DIR} &&\
    cd .. &&\
    dotnet run

clean:	
	$(CLEAN_FLAGS) ${BUILD_DIR}
