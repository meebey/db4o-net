
MONO = mono
MCS = gmcs
KEY_FILE = ../db4objects.snk
MCS_FLAGS = -keyfile:$(KEY_FILE) -define:NET_2_0,MONO,EMBEDDED
OUTDIR = ../bin

CORE = Db4objects.Db4o.dll
TESTS = Db4objects.Db4o.Tests.exe
TOOLS = Db4objects.Db4o.Tools.dll
UNIT = Db4oUnit.dll
UNIT_EXT = Db4oUnit.Extensions.dll

build: precompile compile postcompile

precompile:
	find . -name "*.cs" > $(RESPONSE_FILE)

compile:
	$(MCS) -t:$(TARGET) $(REFERENCES) -out:$(OUTDIR)/$(ASSEMBLY) $(MCS_FLAGS) $(OPT_MCS_FLAGS) @$(RESPONSE_FILE)

postcompile:
	rm -f $(RESPONSE_FILE)
