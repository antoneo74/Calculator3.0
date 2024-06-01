BUILD_DIR = build
EXECUTE_DIR = bin
REPORT_DIR = report
TEST_RESULTS_DIR = TestResults

ifeq ($(OS),Windows_NT)
# GENERATOR = -G "Visual Studio 17 2022"
GENERATOR = -G "MinGW Makefiles"
CLEAN_FLAGS = rd /s /q
else
UNAME_S := $(shell uname -s)
	ifeq ($(UNAME_S),Linux)
	OPEN_FILE	= xdg-open
	CLEAN_FLAGS= rm -rf
	endif
endif

all: test

test: clean
	CC=gcc CXX=g++ cmake -B $(BUILD_DIR) &&\
	cd $(BUILD_DIR) &&\
	$(MAKE)
	dotnet test --collect:"XPlat Code Coverage" --logger:xunit -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover &&\
	reportgenerator -reports:"**/*.xml" -targetdir:"report" --reporttypes: "Html"
	$(OPEN_FILE) ./report/index.html

clean:
	${CLEAN_FLAGS} ${BUILD_DIR} ${EXECUTE_DIR} ${REPORT_DIR} ${TEST_RESULTS_DIR}


# dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.3.4