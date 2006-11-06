
TARGET = exe
ASSEMBLY = $(ADMIN)
REFERENCES = -r:$(OUTDIR)/$(TOOLS) -r:$(OUTDIR)/$(CORE)
RESPONSE_FILE = db4oadmin.sources

include ../include.mk

precompile:
	[ -d $(OUTDIR) ] || mkdir $(OUTDIR)
	find Db4oAdmin -name "*.cs" > $(RESPONSE_FILE)
