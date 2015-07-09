ifeq ($(OS),Windows_NT)
    BUILDFILE += .\Build\Windows\build.bat
    TESTFILE += .\Build\Windows\test.bat
else
    BUILDFILE += sh ./Build/Unix/build.sh
    TESTFILE += sh ./Build/Unix/test.sh
endif

.PHONY: all compile test

all: compile

compile:
	@$(BUILDFILE)

test:
	@$(TESTFILE)
