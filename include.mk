
MONO = mono
MCS = gmcs
KEY_FILE = ../db4objects.snk
MCS_FLAGS = -keyfile:$(KEY_FILE) -define:NET_2_0,MONO,EMBEDDED
OUTDIR = ../bin
WORKDIR = .
RESPONSE_FILE = $(WORKDIR)/sources

CORE = Db4objects.Db4o.dll
TESTS = Db4objects.Db4o.Tests.exe
TOOLS = Db4objects.Db4o.Tools.dll
UNIT = Db4oUnit.dll
UNIT_EXT = Db4oUnit.Extensions.dll
ADMIN = Db4oAdmin.exe

CECIL = Mono.Cecil.dll
FLOWANALYSIS = Cecil.FlowAnalysis.dll
GETOPTIONS = Mono.GetOptions.dll

build: precompile compile postcompile

precompile:
	[ -d $(OUTDIR) ] || mkdir $(OUTDIR)
	find $(WORKDIR) -name "*.cs" > $(RESPONSE_FILE)

compile:
	$(MCS) -t:$(TARGET) $(REFERENCES) -warn:0 -out:$(OUTDIR)/$(ASSEMBLY) $(MCS_FLAGS) $(OPT_MCS_FLAGS) @$(RESPONSE_FILE)

postcompile:
	rm -f $(RESPONSE_FILE)
